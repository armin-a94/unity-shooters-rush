namespace DevelopersHub.ShootersRush
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    
    public class ShootersRushCharacter : MonoBehaviour
    {
   
        [SerializeField] private string _id = ""; public string id { get { return _id; } }
        [SerializeField] private string _prefabId = ""; public string prefabID { get { return _prefabId; } }
        [SerializeField] private float _health = 100; public float health { get { return _health; } }
        [SerializeField] private float _radius = 0.4f; public float radius { get { return _radius; } }
        [SerializeField] private List<ShootersRushWeapon.Settings> _weaponSettings = new List<ShootersRushWeapon.Settings>(); 
        [SerializeField] private Transform _weaponsHolder = null; public Transform weaponsHolder { get { return _weaponsHolder; } }
        private ShootersRushCharacterAnimationIK _ik = null;
        private Animator _animator = null;
        private ShootersRushCharacterGroup _group = null;
        private CapsuleCollider _collider = null;
        private Vector3 _targetLocalPosition = Vector3.zero; public Vector3 targetLocalPosition { get { return _targetLocalPosition; } }
        private Vector3 _smootDampVelocity = Vector3.zero;
        private Vector2 _moveInput = Vector2.zero; public Vector2 moveInput { get { return _moveInput; } }
        private Vector3 _lastPosition = Vector3.zero; 
        private ShootersRushWeapon _weapon = null; public ShootersRushWeapon weapon { get { return _weapon; } }
        private bool _cleared = false;
        private int _row = 0; public int row { get { return _row; } }
        private int _column = 0; public int column { get { return _column; } }
        private float _currentHealth = 100; public float currentHealth { get { return _currentHealth; } }
        private Rigidbody _rigidbody = null;
        private bool _initialized = false;
        private bool _dummy = false;
        private float _bodyDisposeDelay = 2f;
        private float _bodyDisposeTime = 2f;
        private float _deathTimer = 0f;
        
        private void Awake()
        {
            Initialize();
        }
        
        public void Initialize()
        {
            if (_initialized) { return; }
            _initialized = true;
            gameObject.tag = "Character";
            _currentHealth = _health;
            Clear();
            _animator = GetComponentInChildren<Animator>();
            _ik = GetComponentInChildren<ShootersRushCharacterAnimationIK>();
            if (_ik == null && _animator != null)
            {
                _ik = _animator.gameObject.AddComponent<ShootersRushCharacterAnimationIK>();
            }
            _collider = GetComponent<CapsuleCollider>();
            if (_collider == null)
            {
                _collider = gameObject.AddComponent<CapsuleCollider>();
            }
            _collider.height = 2f;
            _collider.radius = _radius;
            _collider.center = new Vector3(0, 1f, 0);
            _collider.isTrigger = true;
            _rigidbody = gameObject.AddComponent<Rigidbody>();
            _rigidbody.isKinematic = true;
            _rigidbody.useGravity = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            string tag = other.gameObject.tag;
            if (tag == "Container")
            {
                ApplyDamage(_currentHealth);
            }
            else if (tag == "Gateway")
            {
                ShootersRushGateway gateway = other.GetComponent<ShootersRushGateway>();
                if (!gateway.triggered)
                {
                    gateway.triggered = true;
                    ShootersRushManager.Singleton._GatewayTriggered(gateway);
                }
            }
        }

        public bool Fire()
        {
            if (_weapon != null)
            {
                return _weapon.Fire();
            }
            return false;
        }
        
        public bool Fire(AudioSource fireAudioSource)
        {
            if (_weapon != null && _weapon.Fire())
            {
                if (_weapon.fireSound != null && fireAudioSource != null)
                {
                    fireAudioSource.PlayOneShot(_weapon.fireSound);
                    // fireAudioSource.clip = _weapon.fireSound;
                    // fireAudioSource.Play();
                }
                return true;
            }
            return false;
        }
        
        public void ApplyDamage(float damage)
        {
            if (_currentHealth > 0 && damage > 0)
            {
                _currentHealth -= damage;
                if (_currentHealth <= 0) { _currentHealth = 0; }
                bool dead = _currentHealth <= 0;
                ShootersRushManager.Singleton._CharacterDamaged(this, damage, dead);
                if (dead)
                {
                    _collider.enabled = false;
                    _group.RemoveCharacter(this);
                    if (_animator != null)
                    {
                        _animator.SetTrigger("death");
                    }
                    _collider.enabled = false;
                }
            }
        }
        
        public void SetGroup(ShootersRushCharacterGroup group)
        {
            _group = group;
        }
        
        public void SetLocalPosition(Vector3 position, int column, int row)
        {
            _targetLocalPosition = position;
            _lastPosition = transform.localPosition;
            _row = row;
            _column = column;
        }

        public void SetAsDummy()
        {
            if (_collider != null)
            {
                #if UNITY_EDITOR
                if (Application.isPlaying)
                {
                    Destroy(_collider);
                }
                else
                {
                    DestroyImmediate(_collider);
                }
                #else
                   Destroy(_collider);
                #endif
            }
            _dummy = true;
        }
        
        private void Update()
        {
            if (_dummy) { return; }
            
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
            
            if (_group != null && transform.localPosition != _targetLocalPosition)
            {
                transform.localPosition = Vector3.SmoothDamp(transform.localPosition, _targetLocalPosition, ref _smootDampVelocity, 1f);
                if (Vector3.Distance(transform.localPosition, _targetLocalPosition) < 0.1f)
                {
                    transform.localPosition = _targetLocalPosition;
                    _moveInput = Vector2.zero;
                }
                else
                {
                    _moveInput = transform.localPosition - _lastPosition;
                }
            }
            else
            {
                _moveInput = Vector2.zero;
            }
            _lastPosition = transform.localPosition;

            if (_animator != null)
            {
                float moveX = _moveInput.x;
                float moveY = _moveInput.y;
                if (_group != null && _moveInput == Vector2.zero)
                {
                    moveX = _group.animatorInput.x;
                    moveY = _group.animatorInput.y;
                }
                else
                {
                    if (_moveInput != Vector2.zero)
                    {
                        moveX = moveX > 0 ? 1 : moveX < 0 ? -1 : 0;
                        moveY = moveY > 0 ? 1 : moveY < 0 ? -1 : 0;
                    }
                }
                _animator.SetFloat("move_x", moveX);
                _animator.SetFloat("move_y", moveY);
            }
        }
        
        public ShootersRushWeapon EquipWeapon(string id)
        {
            ShootersRushWeapon prefab = ShootersRushLibrary.GetWeaponPrefab(id);
            if (prefab != null)
            {
                return EquipWeapon(prefab);
            }
            return null;
        }
        
        public ShootersRushWeapon EquipWeapon(int index)
        {
            ShootersRushWeapon prefab = ShootersRushLibrary.GetWeaponPrefab(index);
            if (prefab != null)
            {
                return EquipWeapon(prefab);
            }
            return null;
        }
        
        public ShootersRushWeapon EquipWeapon(ShootersRushWeapon prefab)
        {
            if (prefab != null)
            {
                if (Application.isPlaying) { Clear(); }
                RemoveWeapon();
                if (_animator == null)
                {
                    _animator = GetComponentInChildren<Animator>();
                }
                _weapon = Instantiate(prefab, _weaponsHolder, true);
                ShootersRushWeapon.Settings settings = GetWeaponSettings(_weapon.prefabID);
                _weapon.ApplySettings(settings, transform);
                if (_ik != null)
                {
                    _ik.Initialize(settings, transform, _weapon);
                }
                _weapon.Equipped(this);
                return _weapon;
            }
            return null;
        }
        
        public static Transform GetWeaponHolder(Animator animator)
        {
            Transform target = null;
            target = animator.GetBoneTransform(HumanBodyBones.RightShoulder);
            if (target == null)
            {
                target = animator.GetBoneTransform(HumanBodyBones.UpperChest);
                if (target == null)
                {
                    target = animator.GetBoneTransform(HumanBodyBones.Chest);
                    if (target == null)
                    {
                        target = animator.GetBoneTransform(HumanBodyBones.Spine);
                    }
                }
            }
            return target;
        }
        
        public ShootersRushWeapon.Settings GetWeaponSettings(string prefabId)
        {
            for (int i = 0; i < _weaponSettings.Count; i++)
            {
                if (_weaponSettings[i].prefabID == prefabId)
                {
                    return _weaponSettings[i];
                }
            }
            ShootersRushWeapon.Settings settings = CreateNewWeaponSettings(prefabId);
            if (!string.IsNullOrEmpty(prefabId))
            {
                _weaponSettings.Add(settings);
            }
            return settings;
        }
        
        public ShootersRushWeapon.Settings CreateNewWeaponSettings(string prefabId)
        {
            ShootersRushWeapon.Settings settings = new ShootersRushWeapon.Settings();
            if (_weaponSettings.Count > 0)
            {
                settings.rightHandPosition = _weaponSettings[0].rightHandPosition;
                settings.rightHandRotation = _weaponSettings[0].rightHandRotation;
                settings.leftHandPosition = _weaponSettings[0].leftHandPosition;
                settings.leftHandRotation = _weaponSettings[0].leftHandRotation;
                settings.useLeftHand = _weaponSettings[0].useLeftHand;
                settings.weaponPosition = _weaponSettings[0].weaponPosition;
                settings.weaponRotation = _weaponSettings[0].weaponRotation;
            }
            settings.prefabID = prefabId;
            return settings;
        }
        
        public bool AddWeaponSettings(ShootersRushWeapon.Settings settings)
        {
            if (string.IsNullOrEmpty(settings.prefabID))
            {
                return false;
            }
            for (int i = 0; i < _weaponSettings.Count; i++)
            {
                if (_weaponSettings[i].prefabID == settings.prefabID)
                {
                    return false;
                }
            }
            _weaponSettings.Add(settings);
            return true;
        }
        
        public bool HaveWeaponSettings(string prefabId)
        {
            for (int i = 0; i < _weaponSettings.Count; i++)
            {
                if (_weaponSettings[i].prefabID == prefabId)
                {
                    return true;
                }
            }
            return false;
        }
        
        public void Clear()
        {
            #if UNITY_EDITOR
            if (Application.isPlaying)
            {
                if (_cleared) { return; }
                _cleared = true;
            }
            #else
            if (_cleared) { return; }
            _cleared = true;
            #endif
            ShootersRushWeapon[] weapons = GetComponentsInChildren<ShootersRushWeapon>();
            if (weapons != null)
            {
                for (int i = 0; i < weapons.Length; i++)
                {
                    #if UNITY_EDITOR
                    if (Application.isPlaying)
                    {
                        Destroy(weapons[i].gameObject);
                    }
                    else
                    {
                        DestroyImmediate(weapons[i].gameObject);
                    }
                    #else
                    Destroy(weapons[i].gameObject);
                    #endif
                }
            }
        }
        
        public void RemoveWeapon()
        {
            if (_weapon != null)
            {
                #if UNITY_EDITOR
                if (Application.isPlaying)
                {
                    Destroy(_weapon.gameObject);
                }
                else
                {
                    DestroyImmediate(_weapon.gameObject);
                }
                #else 
                Destroy(_weapon.gameObject);
                #endif
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
        
        public void _EditorSetWeaponSettings(ShootersRushWeapon.Settings settings)
        {
            #if UNITY_EDITOR
            if (settings == null || string.IsNullOrEmpty(settings.prefabID))
            {
                return;
            }
            bool found = false;
            for (int i = 0; i < _weaponSettings.Count; i++)
            {
                if (_weaponSettings[i].prefabID == settings.prefabID)
                {
                    _weaponSettings[i] = settings;
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                _weaponSettings.Add(settings);
            }
            #endif
        }
        
        public bool _EditorRemoveWeaponSettings(string prefabID)
        {
            #if UNITY_EDITOR
            if (string.IsNullOrEmpty(prefabID))
            {
                return false;
            }
            for (int i = 0; i < _weaponSettings.Count; i++)
            {
                if (_weaponSettings[i].prefabID == prefabID)
                {
                    _weaponSettings.RemoveAt(i);
                    return true;
                }
            }
            #endif
            return false;
        }
        
        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            UnityEditor.Handles.color = Color.cyan;
            UnityEditor.Handles.DrawWireArc(transform.position, transform.up, transform.forward, 360, radius);
        }
        #endif
        
    }
}