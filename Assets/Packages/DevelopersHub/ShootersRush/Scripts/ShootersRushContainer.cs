namespace DevelopersHub.ShootersRush
{
    using UnityEngine;
    using TMPro;
    
    public class ShootersRushContainer : MonoBehaviour
    {

        [SerializeField] private string _id = ""; public string id { get { return _id; } }
        [SerializeField] private string _prefabId = ""; public string prefabID { get { return _prefabId; } }
        [SerializeField] private float _health = 100; public float health { get { return _health; } }
        [SerializeField] private Transform _pivot = null; public Transform pivot { get { return _pivot; } }
        [SerializeField] private Transform _model = null; public Transform model { get { return _model; } }
        [SerializeField] private TextMeshPro _textMesh = null;
        
        //[SerializeField] private Type _type = Type.Weapon; public Type type { get { return _type; } }
        [SerializeField] private ShootersRushCharacter _characterPrefab = null; public ShootersRushCharacter characterPrefab { get { return _characterPrefab; } }
        [SerializeField] private ShootersRushWeapon _weaponPrefab = null; public ShootersRushWeapon weaponPrefab { get { return _weaponPrefab; } }
        [SerializeField] private int _characterCount = 1; public int characterCount { get { return _characterCount; } }
        
        private CapsuleCollider _collider = null; public CapsuleCollider CapsuleCollider { get { return _collider; } }
        private float _currentHealth = 100; public float currentHealth { get { return _currentHealth; } }
        private float _speed = 10f;
        private bool _initialized = false;
        private Transform _content = null;
        private float _checkTimer = 0;
        
        private void Awake()
        {
            Initialize();
        }
        
        public void Initialize()
        {
            if (_initialized) { return; }
            _initialized = true;
            gameObject.tag = "Container";
            _currentHealth = _health;
            _collider = GetComponent<CapsuleCollider>();
            _collider.isTrigger = true;
            _speed = ShootersRushManager.Singleton.enemySpeed * 20f;
            UpdateHealthText();
        }

        public void Initialize(int hitPoints)
        {
            Initialize();
            _health = hitPoints;
            _currentHealth = hitPoints;
            UpdateHealthText();
        }
        
        private void UpdateHealthText()
        {
            UpdateHealthText(_currentHealth);
        }
        
        public void UpdateHealthText(float value)
        {
            _textMesh.text = Mathf.CeilToInt(value).ToString();
        }
        
        private void Update()
        {
            _pivot.Rotate(Vector3.right, _speed * Time.deltaTime, Space.Self);
            _checkTimer += Time.deltaTime;
            if (_checkTimer > 1f)
            {
                _checkTimer = 0;
                Vector3 localPosition = ShootersRushManager.Singleton.transform.InverseTransformPoint(transform.position);
                if (localPosition.z < 0 && -localPosition.z >= ShootersRushManager.Singleton.cameraDistance)
                {
                    ShootersRushManager.Singleton._ContainerTimeout(this);
                    Destroy(gameObject);
                }
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
                ShootersRushManager.Singleton._ContainerDamaged(this, projectile.damage, dead);
                UpdateHealthText();
                if (dead)
                {
                    if (_characterPrefab != null && _weaponPrefab != null)
                    {
                        ShootersRushManager.Singleton.group.RemoveAll();
                        for (int i = 0; i < _characterCount; i++)
                        {
                            ShootersRushCharacter character = Instantiate(_characterPrefab);
                            character.EquipWeapon(_weaponPrefab);
                            ShootersRushManager.Singleton.group.AddCharacter(character);
                        }
                    }
                    else if (_weaponPrefab != null)
                    {
                        ShootersRushManager.Singleton.group.ChangeWeapon(_weaponPrefab);
                    }
                    _collider.enabled = false;
                    Destroy(gameObject);
                }
            }
        }
        
        public Transform CreateContent(float height, ShootersRushCharacter characterPrefab, ShootersRushWeapon weaponPrefab, int characterCount, float scale, float contentPosition, float contentSpace)
        {
            RemoveContent();
            _characterPrefab = characterPrefab;
            _weaponPrefab = weaponPrefab;
            _characterCount = characterCount;
            _content = new GameObject("Content").transform;
            _content.SetParent(transform);
            _content.localPosition = new Vector3(contentPosition, height, 0);
            _content.localEulerAngles = Vector3.zero;
            _content.rotation = Quaternion.LookRotation(transform.forward);
            if (characterPrefab != null && weaponPrefab != null)
            {
                /*
                float radius = 0.5f;
                CapsuleCollider cc = characterPrefab.GetComponent<CapsuleCollider>();
                if (cc)
                {
                    radius = cc.radius;
                }
                */
                float w = contentSpace * (_characterCount - 1);
                Vector3 basePosition = new Vector3(-w * 0.5f, 0, 0);
                for (int i = 0; i < _characterCount; i++)
                {
                    ShootersRushCharacter character = Instantiate(characterPrefab, _content.transform);
                    character.Clear();
                    character.Initialize();
                    character.transform.localPosition = basePosition + new Vector3(i * contentSpace, 0, 0);
                    character.transform.localEulerAngles = Vector3.zero;
                    character.EquipWeapon(weaponPrefab);
                    character.SetAsDummy();
                }
            }
            else if (weaponPrefab != null)
            {
                ShootersRushWeapon weapon = Instantiate(weaponPrefab, _content.transform);
                weapon.transform.localPosition = Vector3.zero;
                weapon.transform.localEulerAngles = new Vector3(0, 90, 0);
            }
            _content.localScale = Vector3.one * scale;
            return _content;
        }
        
        public void RemoveContent()
        {
            if (_content != null)
            {
                #if UNITY_EDITOR
                if (Application.isPlaying)
                {
                    Destroy(_content.gameObject);
                }
                else
                {
                    DestroyImmediate(_content.gameObject);
                }
                #else
                Destroy(_content.gameObject);
                #endif
            }
        }
        
        public void _EditorInitialize(string identifier, string prefabId, Transform pivot, Transform model, TextMeshPro text)
        {
            #if UNITY_EDITOR
            _id = identifier;
            _prefabId = prefabId;
            _pivot = pivot;
            _model = model;
            _textMesh = text;
            #endif
        }
        
    }
}