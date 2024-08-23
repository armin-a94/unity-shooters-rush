namespace DevelopersHub.ShootersRush
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class ShootersRushMuzzleFlash : MonoBehaviour
    {

        [SerializeField] private string _id = ""; public string id { get { return _id; } }
        [SerializeField] private string _prefabId = ""; public string prefabID { get { return _prefabId; } }
        [SerializeField] private float _lifetime = 0.1f;

        private ParticleSystem[] _particles = null;
        private bool _initialized = false;
        
        private void Awake()
        {
            Initialize();
        }
        
        public void Initialize()
        {
            if (_initialized) { return; }
            _initialized = true;
            _particles = GetComponentsInChildren<ParticleSystem>();
        }
        
        public static ShootersRushMuzzleFlash Spawn(string id, Transform parent, Vector3 position, Quaternion rotation)
        {
            ShootersRushMuzzleFlash muzzleFlash = ShootersRushObjectPool.GetMuzzleFlashInstance(id);
            muzzleFlash.transform.SetParent(parent);
            muzzleFlash.transform.position = position;
            muzzleFlash.transform.rotation = rotation;
            muzzleFlash.Spawned();
            return muzzleFlash;
        }

        public void Spawned()
        {
            if (_particles != null)
            {
                for (int i = 0; i < _particles.Length; i++)
                {
                    _particles[i].Play();
                }
            }
            Invoke(nameof(Despawn), _lifetime);
        }

        private void Despawn()
        {
            ShootersRushObjectPool.SetMuzzleFlashInstance(this);
        }
        
        public void Reset()
        {
            
        }
        
    }
}