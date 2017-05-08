using UnityEngine;

namespace FoW
{
    public enum FogOfWarPhysics
    {
        None,
        Physics2D,
        Physics3D
    }

    public enum FogOfWarPlane
    {
        XY, // 2D
        YZ,
        XZ // 3D
    }

    class FoWIDs
    {
        public int mainTex;
        public int skyboxTex;
        public int fogColorTex;
        public int cameraDir;
        public int cameraWS;
        public int frustumCornersWS;
        public int fogColor;
        public int mapOffset;
        public int mapSize;
        public int fogTextureSize;
        public int fogTex;
        public int outsideFogStrength;

        public void Initialise()
        {
            mainTex = Shader.PropertyToID("_MainTex");
            skyboxTex = Shader.PropertyToID("_SkyboxTex");
            fogColorTex = Shader.PropertyToID("_FogColorTex");
            cameraDir = Shader.PropertyToID("_CameraDir");
            cameraWS = Shader.PropertyToID("_CameraWS");
            frustumCornersWS = Shader.PropertyToID("_FrustumCornersWS");
            fogColor = Shader.PropertyToID("_FogColor");
            mapOffset = Shader.PropertyToID("_MapOffset");
            mapSize = Shader.PropertyToID("_MapSize");
            fogTextureSize = Shader.PropertyToID("_FogTextureSize");
            fogTex = Shader.PropertyToID("_FogTex");
            outsideFogStrength = Shader.PropertyToID("_OutsideFogStrength");
        }
    }

    [AddComponentMenu("FogOfWar/FogOfWar")]
    public class FogOfWar : MonoBehaviour
    {
        public int mapResolution = 128;
        public float mapSize = 128;
        public Vector2 mapOffset = Vector2.zero;
        public FogOfWarPlane plane = FogOfWarPlane.XZ;
        public FogOfWarPhysics physics = FogOfWarPhysics.Physics3D;
        public FilterMode filterMode = FilterMode.Bilinear;
        public Color fogColor = Color.black;
        public Texture2D fogColorTexture = null;
        public bool fogFarPlane = true;
        [Range(0.0f, 1.0f)]
        public float outsideFogStrength = 1;
        public bool clearFog = false;
        public LayerMask clearFogMask = -1;
        public float pixelSize { get { return mapSize / mapResolution; } }
        public float fieldOfViewPenetration = 1.0f;

        [Range(0.0f, 1.0f)]
        public float fogEdgeRadius = 0.2f;

        [Range(0.0f, 1.0f)]
        public float partialFogAmount = 0.5f;

        Material _material;
        public Texture2D texture { get; private set; }
        public byte[] fogValues { get; set; }

        public float updateFrequency = 0.1f;
        float _nextUpdate = 0.0f;

        public bool experimentalLineOfSightOptimisation = false; // when enabled, this will use of the collider rects optimisation (which appears to be buggy!)

        Transform _transform;
        Camera _camera;
        
        public static FogOfWar current = null;
        static FoWIDs _ids = null;

        static Shader _fogOfWarShader = null;
        public static Shader fogOfWarShader { get { if (_fogOfWarShader == null) _fogOfWarShader = Resources.Load<Shader>("FogOfWarShader"); return _fogOfWarShader; } }
        static Shader _clearFogShader = null;
        public static Shader clearFogShader { get { if (_clearFogShader == null) _clearFogShader = Resources.Load<Shader>("ClearFogShader"); return _clearFogShader; } }

        void Awake()
        {
            current = this;
            if (_ids == null)
            {
                _ids = new FoWIDs();
                _ids.Initialise();
            }

            Reinitialize();
        }

        // Call this whenever you change any of the size values of the map
        public void Reinitialize()
        {
            texture = new Texture2D(mapResolution, mapResolution, TextureFormat.Alpha8, false);

            _material = new Material(fogOfWarShader);
            _material.name = "FogMaterial";

            fogValues = new byte[mapResolution * mapResolution];
            SetAll(255);
            UpdateTexture();
        }

        // Increase skip to improve performance but sacrifice accuracy
        public float ExploredArea(int skip = 1)
        {
            skip = Mathf.Max(skip, 1);
            int total = 0;
            for (int i = 0; i < fogValues.Length; i += skip)
                total += fogValues[i];
            return (1.0f - (float)total / (fogValues.Length * 255.0f / skip)) * 2;
        }

        void Start()
        {
            _transform = transform;
            _camera = GetComponent<Camera>();
            _camera.depthTextureMode |= DepthTextureMode.Depth;
        }

        public void SetAll(byte value)
        {
            for (int i = 0; i < fogValues.Length; ++i)
                fogValues[i] = value;
        }

        public Vector2 WorldToFogPlane(Vector3 position)
        {
            if (plane == FogOfWarPlane.XY)
                return new Vector2(position.x, position.y);
            else if (plane == FogOfWarPlane.YZ)
                return new Vector2(position.y, position.z);
            else if (plane == FogOfWarPlane.XZ)
                return new Vector2(position.x, position.z);
            else
            {
                Debug.LogError("FogOfWarPlane is an invalid value!");
                return Vector2.zero;
            }
        }

        public Vector2i WorldPositionToFogPosition(Vector3 position)
        {
            float mapmultiplier = mapResolution / mapSize;
            Vector2i mappos = new Vector2i((WorldToFogPlane(position) - mapOffset) * mapmultiplier);
            mappos += new Vector2i(mapResolution >> 1, mapResolution >> 1);
            return mappos;
        }

        public Vector2 WorldPositionToFogPositionNormalized(Vector3 position)
        {
            return (WorldToFogPlane(position) - mapOffset) / mapSize + new Vector2(0.5f, 0.5f);
        }

        public Vector3 FogPlaneToWorld(Vector2 position, float v = 0)
        {
            if (plane == FogOfWarPlane.XY)
                return new Vector3(position.x, position.y, v);
            else if (plane == FogOfWarPlane.YZ)
                return new Vector3(v, position.x, position.y);
            else if (plane == FogOfWarPlane.XZ)
                return new Vector3(position.x, v, position.y);
            else
            {
                Debug.LogError("FogOfWarPlane is an invalid value!");
                return Vector3.zero;
            }
        }

        // Returns a value between 0 (not in fog) and 255 (fully fogged)
        public byte GetFogValue(Vector3 position)
        {
            Vector2i mappos = WorldPositionToFogPosition(position);
            mappos.x = Mathf.Clamp(mappos.x, 0, mapResolution - 1);
            mappos.y = Mathf.Clamp(mappos.y, 0, mapResolution - 1);
			return _raw_texture_data[mappos.y * mapResolution + mappos.x];
        }

		byte[] _raw_texture_data;
		void FixedUpdate()
		{
			_raw_texture_data =  texture.GetRawTextureData();

		}

        public bool IsInFog(Vector3 position, byte minfog)
        {
            return GetFogValue(position) > minfog;
        }

        public bool IsInFog(Vector3 position, float minfog)
        {
            return IsInFog(position, (byte)(minfog * 255));
        }

        public bool IsInCompleteFog(Vector3 position)
        {
            return IsInFog(position, 240);
        }

        public bool IsInPartialFog(Vector3 position)
        {
            return IsInFog(position, 20);
        }

        ColliderFogRectList GetExtendedColliders(FogFill fogfill, int layermask)
        {
            // quick check to see if all raycasts will hit something
            if (layermask == 0 || physics == FogOfWarPhysics.None)
                return null;

            ColliderFogRectList colliderrects = null;
            if (physics == FogOfWarPhysics.Physics2D)
            {
                // is there anything overlapping with the area?
                Collider2D[] colliders = Physics2D.OverlapCircleAll(fogfill.worldPosition, fogfill.worldRadius, layermask);
                if (colliders.Length == 0)
                    return null;

                // extend the colliders outwards away from the center
                colliderrects = new ColliderFogRectList(this);
                colliderrects.Add(colliders);
            }
            else if (physics == FogOfWarPhysics.Physics3D)
            {
                // is there anything overlapping with the area?
                Collider[] colliders = Physics.OverlapSphere(fogfill.worldPosition, fogfill.worldRadius, layermask);
                if (colliders.Length == 0)
                    return null;

                // extend the colliders outwards away from the center
                colliderrects = new ColliderFogRectList(this);
                colliderrects.Add(colliders);
            }
            else
            {
                Debug.LogError("FogOfWarPhysics is an invalid value!");
                return null;
            }

            colliderrects.ExtendToCircleEdge(fogfill.fogPosition, fogfill.fogRadius);
            colliderrects.Optimise();
            return colliderrects.Count == 0 ? null : colliderrects;
        }

        public void Unfog(Vector3 position, float radius, float angle, Vector3 forward, int layermask = 0)
        {
            FogFill fogfill = new FogFill(this, position, radius, angle, forward);
            ColliderFogRectList colliderrects = GetExtendedColliders(fogfill, layermask);
            fogfill.UnfogCircle(colliderrects, layermask);
        }

        public void Unfog(Rect rect)
        {
            Vector2i min = WorldPositionToFogPosition(rect.min);
            Vector2i max = WorldPositionToFogPosition(rect.max);
            for (int y = min.y; y < max.y; ++y)
            {
                for (int x = min.x; x < max.x; ++x)
                    fogValues[y * mapResolution + x] = 0;
            }
        }

        void UpdateTexture()
        {
            texture.LoadRawTextureData(fogValues);
            texture.filterMode = filterMode;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.Apply();

            byte partialfog = (byte)(partialFogAmount * 255);

            for (int y = 0; y < mapResolution; ++y)
            {
                for (int x = 0; x < mapResolution; ++x)
                {
                    int index = y * mapResolution + x;
                    if (fogValues[index] < partialfog)
                        fogValues[index] = partialfog;
                }
            }
        }

        void Update()
        {
            _nextUpdate -= Time.deltaTime;
            if (_nextUpdate > 0)
                return;

            _nextUpdate = updateFrequency;
            UpdateTexture();
        }

        // Returns the corner points relative to the camera's position (not rotation)
        static Matrix4x4 CalculateCameraFrustumCorners(Camera cam, Transform camtransform)
        {
            // Most of this was copied from the GlobalFog image effect standard asset due to the weird way to reconstruct the world position
            Matrix4x4 frustumCorners = Matrix4x4.identity;
            float camAspect = cam.aspect;
            float camNear = cam.nearClipPlane;
            float camFar = cam.farClipPlane;

            if (cam.orthographic)
            {
                float orthoSize = cam.orthographicSize;

                Vector3 far = camtransform.forward * camFar;
                Vector3 rightOffset = camtransform.right * (orthoSize * camAspect);
                Vector3 topOffset = camtransform.up * orthoSize;

                frustumCorners.SetRow(0, far + topOffset - rightOffset);
                frustumCorners.SetRow(1, far + topOffset + rightOffset);
                frustumCorners.SetRow(2, far - topOffset + rightOffset);
                frustumCorners.SetRow(3, far - topOffset - rightOffset);
            }
            else // perspective
            {
                float fovWHalf = cam.fieldOfView * 0.5f;
                float fovWHalfTan = Mathf.Tan(fovWHalf * Mathf.Deg2Rad);

                Vector3 toRight = camtransform.right * (camNear * fovWHalfTan * camAspect);
                Vector3 toTop = camtransform.up * (camNear * fovWHalfTan);

                Vector3 topLeft = (camtransform.forward * camNear - toRight + toTop);
                float camScale = topLeft.magnitude * camFar / camNear;

                topLeft.Normalize();
                topLeft *= camScale;

                Vector3 topRight = (camtransform.forward * camNear + toRight + toTop);
                topRight.Normalize();
                topRight *= camScale;

                Vector3 bottomRight = (camtransform.forward * camNear + toRight - toTop);
                bottomRight.Normalize();
                bottomRight *= camScale;

                Vector3 bottomLeft = (camtransform.forward * camNear - toRight - toTop);
                bottomLeft.Normalize();
                bottomLeft *= camScale;

                frustumCorners.SetRow(0, topLeft);
                frustumCorners.SetRow(1, topRight);
                frustumCorners.SetRow(2, bottomRight);
                frustumCorners.SetRow(3, bottomLeft);
            }
            return frustumCorners;
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            RenderFog(source, destination, _camera, _transform);
        }

        public void RenderFog(RenderTexture source, RenderTexture destination, Camera cam, Transform camtransform)
        {
            if (clearFog)
            {
                RenderTexture temprendertex = new RenderTexture(source.width, source.height, source.depth);
                RenderFogFull(source, temprendertex, cam, camtransform);
                RenderClearFog(temprendertex, destination);
                Destroy(temprendertex);
            }
            else
                RenderFogFull(source, destination, cam, camtransform);
        }

        void RenderFogFull(RenderTexture source, RenderTexture destination, Camera cam, Transform camtransform)
        {
            _material.SetTexture(_ids.fogTex, texture);
            _material.SetInt(_ids.fogTextureSize, mapResolution);
            _material.SetFloat(_ids.mapSize, mapSize);
            _material.SetVector(_ids.mapOffset, mapOffset);
            _material.SetColor(_ids.fogColor, fogColor);
            _material.SetMatrix(_ids.frustumCornersWS, CalculateCameraFrustumCorners(cam, camtransform));
            _material.SetVector(_ids.cameraWS, camtransform.position);
            _material.SetFloat(_ids.outsideFogStrength, outsideFogStrength);

            // orthographic is treated very differently in the shader, so we have to make sure it executes the right code
            if (cam.orthographic)
            {
                _material.DisableKeyword("CAMERA_PERSPECTIVE");
                _material.EnableKeyword("CAMERA_ORTHOGRAPHIC");

                Vector4 camdir = camtransform.forward;
                camdir.w = cam.nearClipPlane;
                _material.SetVector(_ids.cameraDir, camdir);
            }
            else
            {
                _material.DisableKeyword("CAMERA_ORTHOGRAPHIC");
                _material.EnableKeyword("CAMERA_PERSPECTIVE");
            }

            // which plane will the fog be rendered to?
            if (plane == FogOfWarPlane.XY)
            {
                _material.DisableKeyword("PLANE_XZ");
                _material.DisableKeyword("PLANE_YZ");
                _material.EnableKeyword("PLANE_XY");
            }
            else if (plane == FogOfWarPlane.YZ)
            {
                _material.DisableKeyword("PLANE_XY");
                _material.DisableKeyword("PLANE_XZ");
                _material.EnableKeyword("PLANE_YZ");
            }
            else if (plane == FogOfWarPlane.XZ)
            {
                _material.DisableKeyword("PLANE_XY");
                _material.DisableKeyword("PLANE_YZ");
                _material.EnableKeyword("PLANE_XZ");
            }
            else
                Debug.LogError("FogOfWarPlane is an invalid value!");

            if (fogColorTexture != null)
            {
                _material.EnableKeyword("TEXTUREFOG");
                _material.SetTexture(_ids.fogColorTex, fogColorTexture);
            }
            else
                _material.DisableKeyword("TEXTUREFOG");

            if (fogFarPlane)
                _material.EnableKeyword("FOGFARPLANE");
            else
                _material.DisableKeyword("FOGFARPLANE");

            CustomGraphicsBlit(source, destination, _material);
        }

        void RenderClearFog(RenderTexture source, RenderTexture destination)
        {
            // create skybox camera
            Camera skyboxcamera = new GameObject("TempSkyboxFogCamera").AddComponent<Camera>();
            skyboxcamera.transform.parent = transform;
            skyboxcamera.transform.position = transform.position;
            skyboxcamera.transform.rotation = transform.rotation;
            skyboxcamera.fieldOfView = _camera.fieldOfView;
            skyboxcamera.clearFlags = CameraClearFlags.Skybox;
            skyboxcamera.targetTexture = new RenderTexture(source.width, source.height, source.depth);
            skyboxcamera.cullingMask = clearFogMask;
            skyboxcamera.orthographic = _camera.orthographic;
            skyboxcamera.orthographicSize = _camera.orthographicSize;
            skyboxcamera.rect = _camera.rect;

            // render skyboxcamera to texture
            RenderTexture currentRT = RenderTexture.active;
            RenderTexture.active = skyboxcamera.targetTexture;
            skyboxcamera.Render();
            Texture2D skyboximage = new Texture2D(skyboxcamera.targetTexture.width, skyboxcamera.targetTexture.height);
            skyboximage.ReadPixels(new Rect(0, 0, skyboxcamera.targetTexture.width, skyboxcamera.targetTexture.height), 0, 0);
            skyboximage.Apply();
            RenderTexture.active = currentRT;

            // overlay renders on eachother
            RenderTexture.active = destination;
            Material clearfogmat = new Material(clearFogShader);
            clearfogmat.SetTexture(_ids.skyboxTex, skyboximage);
            CustomGraphicsBlit(source, destination, clearfogmat);

            // ensure temp objects are destroyed
            Destroy(skyboxcamera.targetTexture);
            Destroy(skyboxcamera.gameObject);
            Destroy(clearfogmat);
            Destroy(skyboximage);
        }

        static void CustomGraphicsBlit(RenderTexture source, RenderTexture dest, Material material)
        {
            RenderTexture.active = dest;
            material.SetTexture(_ids.mainTex, source);
            material.SetPass(0);

            GL.PushMatrix();
            GL.LoadOrtho();

            GL.Begin(GL.QUADS);

            GL.MultiTexCoord2(0, 0.0f, 0.0f);
            GL.Vertex3(0.0f, 0.0f, 3.0f); // BL

            GL.MultiTexCoord2(0, 1.0f, 0.0f);
            GL.Vertex3(1.0f, 0.0f, 2.0f); // BR

            GL.MultiTexCoord2(0, 1.0f, 1.0f);
            GL.Vertex3(1.0f, 1.0f, 1.0f); // TR

            GL.MultiTexCoord2(0, 0.0f, 1.0f);
            GL.Vertex3(0.0f, 1.0f, 0.0f); // TL

            GL.End();
            GL.PopMatrix();
        }

        // KILLME
        //public FogOfWarUnit killme; // this is for testing collider rects (which appears to be buggy). Make sure experimentalLineOfSignOptimisation is set to true before testing!!
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Vector3 offset = FogPlaneToWorld(mapOffset);
            Vector3 size = FogPlaneToWorld(new Vector2(mapSize, mapSize));
            Gizmos.DrawWireCube(offset, size);

            Gizmos.color = new Color(1, 0, 0, 0.2f);
            Gizmos.DrawCube(offset, size);
            /*
            // KILLME
            if (Application.isPlaying)
            {
                FogFill fogfill = new FogFill(this, killme.transform.position, killme.radius, killme.angle, killme.transform.forward + killme.transform.up);
                ColliderFogRectList colliderrects = GetExtendedColliders(fogfill, killme.lineOfSightMask);

                if (colliderrects != null)
                {
                    Gizmos.color = new Color(1, 1, 0, 0.4f);
                    for (int i = 0; i < colliderrects.Count; ++i)
                    {
                        Gizmos.DrawCube((Vector2)offset - Vector2.one * (mapSize * 0.5f) + colliderrects[i].center, colliderrects[i].size.vector2);
                    }
                }
            }*/
        }
    }
}