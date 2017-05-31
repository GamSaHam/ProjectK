using UnityEngine;

namespace FoW
{
    [AddComponentMenu("FogOfWar/HideInFog")]
    [RequireComponent(typeof(Renderer))]
    public class HideInFog : MonoBehaviour
    {
        [Range(0.0f, 1.0f)]
        public float minFogStrength = 0.2f;
        
        Transform _transform;
        Renderer _renderer;

        

        void Start()
        {
            _transform = transform;
            _renderer = GetComponent<Renderer>();
        }

        void Update()
        {
            _renderer.enabled = !FogOfWar.current.IsInFog(_transform.position, minFogStrength);
        }
    }
}
