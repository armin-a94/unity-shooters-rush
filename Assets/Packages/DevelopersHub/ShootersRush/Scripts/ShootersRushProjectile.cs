namespace DevelopersHub.ShootersRush
{
    using UnityEngine;
    
    [RequireComponent(typeof(Rigidbody))] public class ShootersRushProjectile : MonoBehaviour
    {

        [SerializeField] private string _id = ""; public string id { get { return _id; } }
        [SerializeField] private string _prefabId = ""; public string prefabID { get { return _prefabId; } }
        [SerializeField] private float _lifetime = 5f;
        
        private Rigidbody _rigidbody = null;
        private SphereCollider _collider = null;
        private TrailRenderer _trail = null;
        private bool _initialized = false;
        private bool _fired = false;
        private float _spawnTimer = 0;
        private float _damage = 10; public float damage { get { return _damage; } }
        
        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (_initialized) { return; }
            _initialized = true;
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.useGravity = false;
            _trail = GetComponentInChildren<TrailRenderer>();
            _collider = GetComponent<SphereCollider>();
            if (_collider == null)
            {
                _collider = gameObject.AddComponent<SphereCollider>();
                _collider.radius = 0.05f;
            }
            _collider.isTrigger = true;
        }

        public static ShootersRushProjectile Fire(string id, Vector3 position, Vector3 direction, float speed, float damage, Color color)
        {
            ShootersRushProjectile projectile = ShootersRushObjectPool.GetProjectileInstance(id);
            projectile.transform.position = position;
            projectile.Fire(speed, direction, damage, color);
            return projectile;
        }

        private void Despawn()
        {
            _fired = false;
            ShootersRushObjectPool.SetProjectileInstance(this);
        }
        
        public void Fire(float speed, Vector3 direction, float damage)
        {
            _damage = damage;
            _spawnTimer = 0;
            _fired = true;
            _rigidbody.velocity = direction * speed;
        }
        
        public void Fire(float speed, Vector3 direction, float damage, Color color) 
        {
            if (_trail != null)
            {
                _trail.sharedMaterial.color = color;
                _trail.sharedMaterial.SetColor("_EmissionColor", color * Mathf.LinearToGammaSpace(2f));
                _trail.Clear();
            }
            Fire(speed, direction, damage);
        }
        
        private void Update()
        {
            if (_fired)
            {
                _spawnTimer += Time.deltaTime;
                if (_spawnTimer >= _lifetime)
                {
                    Despawn();
                }
            }
        }
        
        public void Reset()
        {
            _rigidbody.velocity = Vector3.zero;
        }

        private void OnTriggerEnter(Collider other)
        {
            string tag = other.gameObject.tag;
            if (!other.gameObject.CompareTag("Character") && !other.gameObject.CompareTag("Gateway"))
            {
                if (other.gameObject.CompareTag("Enemy"))
                {
                    ShootersRushEnemy enemy = other.GetComponent<ShootersRushEnemy>();
                    if (enemy != null)
                    {
                        enemy.ApplyDamage(this);
                    }
                }
                else if (other.gameObject.CompareTag("Container"))
                {
                    ShootersRushContainer container = other.GetComponent<ShootersRushContainer>();
                    if (container != null)
                    {
                        container.ApplyDamage(this);
                    }
                }
                Despawn();
            }
        }
        
    }
}