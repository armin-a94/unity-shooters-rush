namespace DevelopersHub.ShootersRush
{
    using UnityEngine;
    using UnityEngine.AI;
    
    public class ShootersRushEnemy : MonoBehaviour
    {

        [SerializeField] private string _id = ""; public string id { get { return _id; } }
        [SerializeField] private string _prefabId = ""; public string prefabID { get { return _prefabId; } }
        [SerializeField] private float _health = 100; public float maxHealth { get { return _health; } }
        [SerializeField] private float _damage = 5; public float damage { get { return _damage; } }
        [SerializeField] private float _damageApplyTime = 0.5f; public float damageApplyTime { get { return _damageApplyTime; } }
        [SerializeField] private float _radius = 0.3f; public float radius { get { return _radius; } }
        [SerializeField] private float _attackPeriod = 1;
        [SerializeField] private Transform _weaponsHolder = null; public Transform weaponsHolder { get { return _weaponsHolder; } }
        
        private float _currentHealth = 100; public float health { get { return _health; } }
        private NavMeshAgent _agent = null;
        private CapsuleCollider _collider = null;
        private Animator _animator = null;
        private ShootersRushCharacter _target = null;
        private float _attackTimer = 0;
        private Vector3 _targetPosition = Vector3.zero;
        private float _animatorMove = 1f;
        private float _bodyDisposeDelay = 2f;
        private float _bodyDisposeTime = 2f;
        private float _deathTimer = 0f;
        private bool _initialized = false;
        private bool _dummy = false;
        
        private void Awake()
        {
            gameObject.tag = "Enemy";
            _currentHealth = _health;
        }
        
        public void Initialize(bool isEditor = false)
        {
            if (_initialized) { return; }
            _initialized = true;
            _dummy = isEditor;
            if (isEditor) { return; }
            _collider = GetComponent<CapsuleCollider>();
            _animator = GetComponentInChildren<Animator>();
            if (_collider == null)
            {
                _collider = gameObject.AddComponent<CapsuleCollider>();
            }
            _collider.height = 2f;
            _collider.radius = _radius;
            _collider.center = new Vector3(0, 1f, 0);
            _collider.isTrigger = true;
            _agent = GetComponent<NavMeshAgent>();
            if (_agent == null)
            {
                _agent = gameObject.AddComponent<NavMeshAgent>();
            }
            _agent.radius = _radius;
            _agent.speed = ShootersRushManager.Singleton.enemySpeed;
            _agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
            Vector3 localPosition = ShootersRushManager.Singleton.transform.InverseTransformPoint(transform.position);
            _targetPosition = transform.position - ShootersRushManager.Singleton.transform.forward.normalized * Mathf.Abs(localPosition.z);
            _agent.SetDestination(_targetPosition);
        }
        
        private void Start()
        {
            Initialize();
        }

        private void Update()
        {
            if (_currentHealth <= 0)
            {
                _deathTimer += Time.deltaTime;
                if (_deathTimer >= _bodyDisposeDelay)
                {
                    transform.position -= ShootersRushManager.Singleton.transform.up.normalized * 1f * Time.deltaTime;
                }
                if (_deathTimer >= _bodyDisposeTime + _bodyDisposeDelay)
                {
                    Destroy(gameObject);
                }
                return;
            }
            
            float distance = Vector3.Distance(transform.position, _targetPosition);
            
            if ((_target == null || _target.currentHealth <= 0) && distance <= ShootersRushManager.Singleton.enemyChargeRange)
            {
                Charge();
                _target = ShootersRushManager.Singleton.group._GetTarget(this);
            }
            
            if (_target != null && Vector3.Distance(transform.position, _target.transform.position) > ShootersRushManager.Singleton.enemyAttackRange + 0.1f)
            {
                _targetPosition = _target.transform.position + _target.transform.forward.normalized * ShootersRushManager.Singleton.shootersPositionOffset;
                _agent.SetDestination(_targetPosition);
                distance = Vector3.Distance(transform.position, _targetPosition);
            }
            
            if (distance > ShootersRushManager.Singleton.enemyAttackRange)
            {
                _animator.SetFloat("move", _animatorMove);
                return;
            }
            
            _animator.SetFloat("move", 0f);
            _attackTimer -= Time.deltaTime;
            if (_attackTimer <= 0)
            {
                _animator.SetTrigger("attack");
                _attackTimer = _attackPeriod;
                Invoke(nameof(ApplyAttackDamage), _damageApplyTime);
            }
        }
        
        private void Charge()
        {
            _animatorMove = 2f;
            _agent.speed = ShootersRushManager.Singleton.enemyChargeSpeed;
        }
        
        private void ApplyAttackDamage()
        {
            if (_target != null)
            {
                _target.ApplyDamage(_damage);
            }
        }
        
        public void ApplyDamage(ShootersRushProjectile projectile)
        {
            if (_currentHealth > 0 && projectile != null)
            {
                _currentHealth -= projectile.damage;
                if (_currentHealth <= 0)
                {
                    _currentHealth = 0;
                }
                bool dead = _currentHealth <= 0;
                ShootersRushManager.Singleton._EnemyDamaged(this, projectile.damage, dead);
                if (dead)
                {
                    if (_animator != null)
                    {
                        _animator.SetTrigger("death");
                    }
                    _agent.isStopped = true;
                    _collider.enabled = false;
                    _agent.enabled = false;
                }
            }
        }
        
        public void _EditorInitialize(string identifier, Transform holder, string prefabId)
        {
            #if UNITY_EDITOR
            _id = identifier;
            _weaponsHolder = holder;
            _prefabId = prefabId;
            #endif
        }
        
        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            UnityEditor.Handles.color = Color.red;
            UnityEditor.Handles.DrawWireArc(transform.position, transform.up, transform.forward, 360, _radius);
        }
        #endif
        
    }
}