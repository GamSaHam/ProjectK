using UnityEngine;
using System.Collections.Generic;

// WARNING: This code is crap. Do not use it in your game. Do not read it. It will make your brain bleed funny colors.
namespace FoW
{
    [AddComponentMenu("FogOfWar/Test/FogOfWarTestGUI")]
    public class FogOfWarTestGUI : MonoBehaviour
    {
        public float cameraSpeed = 20.0f;
  

        FogOfWar _fog;
        Texture2D _texture;
        GUIStyle _panelStyle;


        Camera _camera;
        Transform _cameraTransform;

 

        void Start()
        {
            _fog = GetComponent<FogOfWar>();
            _camera = GetComponent<Camera>();
            _cameraTransform = transform;
        }

        void Update()
        {



            if (Input.GetKeyDown(KeyCode.Escape))
            {
                //UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
                UnityEngine.SceneManagement.SceneManager.LoadScene(2);
                return;
            }

  
            // update camera
            _cameraTransform.position += new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * cameraSpeed;
            
            if (Input.touchCount == 1)
            {
                Vector2 delta = Input.GetTouch(0).deltaPosition;
                
                if (Input.mousePosition.y > 230)
                {
                    _cameraTransform.position += new Vector3(-delta.x * Time.deltaTime * cameraSpeed, 0, -delta.y * Time.deltaTime * cameraSpeed);
                }

                
            }

            if (Input.touchCount == 2)
            {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                Vector2 zero_touch = touchZero.position - touchZero.deltaPosition;
                Vector2 one_touch = touchOne.position - touchOne.deltaPosition;

                float prev_touch_delta_mag = (zero_touch - one_touch).magnitude;
                float touch_delta_mag = (touchZero.position - touchOne.position).magnitude;

                float delta_magitudediff = -(prev_touch_delta_mag - touch_delta_mag);


                _cameraTransform.position = new Vector3(_cameraTransform.position.x, Mathf.Clamp(_cameraTransform.position.y - delta_magitudediff * Time.deltaTime * cameraSpeed, 15, 30), _cameraTransform.position.z);

            }

            // update camera zooming
            //float zoomchange = Input.GetAxis("Mouse ScrollWheel");
           
        }

        void DrawOnMap(string text, Vector3 position, int panelwidth)
        {
            Vector2i mappos = new Vector2i(_fog.WorldPositionToFogPositionNormalized(position) * (panelwidth - 20));
            GUI.Label(new Rect(10 + mappos.x - 10, Screen.height - mappos.y - 10, 20, 20), text);
        }




	



        void OnGUI()
        {
            bool minimal = Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;

            if (_texture == null)
            {
                _texture = new Texture2D(_fog.texture.width, _fog.texture.height);
                _texture.wrapMode = TextureWrapMode.Clamp;
            }

            if (_panelStyle == null)
            {
                Texture2D panelTex = new Texture2D(1, 1);
                panelTex.SetPixels32(new Color32[] { new Color32(255, 255, 255, 64) });
                panelTex.Apply();
                _panelStyle = new GUIStyle();
                _panelStyle.normal.background = panelTex;
            }

            byte[] original = _fog.texture.GetRawTextureData();

        
            Color32[] pixels = new Color32[original.Length];
            for (int i = 0; i < pixels.Length; ++i)
                pixels[i] = original[i] < 255 ? new Color32(0, 255, 50, 255) : new Color32(0, 0, 0, 255);
            _texture.SetPixels32(pixels);
            _texture.Apply();

            int panelwidth = 128 + 20;

            // draw panel
          //  GUI.Box(new Rect(0, 0, panelwidth, Screen.height), "", _panelStyle);
            GUILayout.BeginArea(new Rect(10, 10, panelwidth - 20, Screen.height - panelwidth - 20));


      
            GUILayout.EndArea();

            // draw map
            GUI.DrawTexture(new Rect(10, Screen.height - panelwidth + 10, panelwidth - 20, panelwidth - 20), _texture);

            DrawOnMap("C", _cameraTransform.position, panelwidth);
		
			List<GameObject> _unit_list = UnitManager.getInstance ()._unit_list;

			for (int i = 0; i < _unit_list.Count; ++i) 
			{
				if(_unit_list[i].GetComponent<UnitStaus>()._is_enemy ==false)
					DrawOnMap ("U", _unit_list[i].transform.position, panelwidth);
			}

			for (int i = 0; i < _unit_list.Count; ++i) 
			{
                if (_unit_list[i].GetComponent<UnitStaus>()._is_enemy == true)
                {
                    if(FogOfWar.current.IsInFog(_unit_list[i].transform.position, 0.2f))
                        DrawOnMap("E", _unit_list[i].transform.position, panelwidth);
                }
			}


        }
    }
}