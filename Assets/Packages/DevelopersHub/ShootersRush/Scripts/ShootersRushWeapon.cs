namespace DevelopersHub.ShootersRush
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    
    public class ShootersRushWeapon : MonoBehaviour
    {
    
        [SerializeField] private string _id = ""; public string id { get { return _id; } }
        [SerializeField] private string _prefabId = ""; public string prefabID { get { return _prefabId; } }
        [SerializeField] private float _damage = 10; public float damage { get { return _damage; } }
        [SerializeField] private int _roundsPerMinute = 200; public int roundsPerMinute { get { return _roundsPerMinute; } }
        [SerializeField] private float _roundsSpeed = 10; public float roundsSpeed { get { return _roundsSpeed; } }
        [SerializeField] private Color _roundsTrailColor = new Color(1f, 0f, 0f, 1f); public Color roundsTrailColor { get { return _roundsTrailColor; } }
        [SerializeField] private Transform _muzzle = null; public Transform muzzle { get { return _muzzle; } }
        [SerializeField] private Transform _pivot = null; public Transform pivot { get { return _pivot; } }
        [SerializeField] private Transform _leftHandIK = null; public Transform leftHandIK { get { return _leftHandIK; } }
        [SerializeField] private Transform _rightHandIK = null; public Transform rightHandIK { get { return _rightHandIK; } }
        [SerializeField] private ShootersRushMuzzleFlash _muzzleFlashPrefab = null; public ShootersRushMuzzleFlash muzzleFlashPrefab { get { return _muzzleFlashPrefab; } }
        [SerializeField] private ShootersRushProjectile _projectilePrefab = null; public ShootersRushProjectile projectilePrefab { get { return _projectilePrefab; } }
        [SerializeField] private AudioClip _fireSound = null; public AudioClip fireSound { get { return _fireSound; } }
        
        private bool _initialized = false;
        private float _fireRate = 1f;
        private float _fireTime = 0;
        private Transform _forwardFactor = null;
        [HideInInspector] public bool editorSelected = false;
        private float _pivotAngleRange = 0;
        private float _pivotAngle = 0;
        private float _pivotSpeed = 0;
        private float _pivotRotateSign = 1;
        private Quaternion _pivotLocalRotation = Quaternion.identity;
        // private AudioSource _fireAudioSource = null;
        
        private Vector3 _leftHandIKPosition = Vector3.zero; public Vector3 leftHandIKPosition { get { return _leftHandIKPosition; } }
        private Quaternion _leftHandIKRotation = Quaternion.identity; public Quaternion leftHandIKRotation { get { return _leftHandIKRotation; } }
        private Vector3 _rightHandIKPosition = Vector3.zero; public Vector3 rightHandIKPosition { get { return _rightHandIKPosition; } }
        private Quaternion _rightHandIKRotation = Quaternion.identity; public Quaternion rightHandIKRotation { get { return _rightHandIKRotation; } }

        [Serializable] public class Settings
        {
            public string prefabID = "";
            public Vector3 weaponPosition = Vector3.zero;
            public Vector3 weaponRotation = Vector3.zero;
            public Vector3 rightHandPosition = Vector3.zero;
            public Vector3 rightHandRotation = Vector3.zero;
            public bool useLeftHand = true;
            public Vector3 leftHandPosition = Vector3.zero;
            public Vector3 leftHandRotation = Vector3.zero;
        }
        
        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (_initialized) { return; }
            _initialized = true;
            _fireRate = 1f / (_roundsPerMinute / 60f);
            /*
            if (_fireSound != null)
            {
                _fireAudioSource = _muzzle.gameObject.AddComponent<AudioSource>();
                _fireAudioSource.playOnAwake = false;
                _fireAudioSource.loop = false;
                _fireAudioSource.clip = _fireSound;
                _fireAudioSource.priority = 256;
                _fireAudioSource.volume = 0.6f;
            }
            */
        }

        public void ResetFireTimer()
        {
            _fireTime = Time.realtimeSinceStartup;
        }
        
        public void Equipped(ShootersRushCharacter character)
        {
            if (Application.isPlaying)
            {
                if (ShootersRushManager.Singleton.randomizeFireTime)
                {
                    _fireTime = Time.realtimeSinceStartup + UnityEngine.Random.Range(_fireRate * 0.2f, _fireRate);
                }
                if (ShootersRushManager.Singleton.autoRotateWeapon)
                {
                    _pivotAngleRange = ShootersRushManager.Singleton.weaponAutoRotateRange;
                    _pivotSpeed = ShootersRushManager.Singleton.weaponAutoRotateSpeed;
                    _pivotRotateSign = 1;
                    if (ShootersRushManager.Singleton.randomizeRotateAngle)
                    {
                        _pivotAngle = UnityEngine.Random.Range(0f, _pivotAngleRange);
                        if (UnityEngine.Random.Range(0, 2) == 0)
                        {
                            _pivotRotateSign = -1;
                        }
                    }
                    else
                    {
                        _pivotAngle = 0;
                    }
                }
                else
                {
                    _pivotAngleRange = 0; 
                    _pivotAngle = 0;
                }
            }
        }
        
        public void LateUpdate()
        {
            if (Application.isPlaying && _pivotAngleRange != 0 && ShootersRushManager.Singleton.gameStatus == 0)
            {
                if (_pivotRotateSign > 0)
                {
                    _pivotAngle += Time.deltaTime * _pivotSpeed;
                    if (_pivotAngle >= _pivotAngleRange * 2f)
                    {
                        _pivotAngle = _pivotAngleRange * 2f;
                        _pivotRotateSign = -1;
                    }
                }
                else
                {
                    _pivotAngle -= Time.deltaTime * _pivotSpeed;
                    if (_pivotAngle <= 0)
                    {
                        _pivotAngle = 0;
                        _pivotRotateSign = 1;
                    }
                }
                _pivot.rotation = Quaternion.Lerp(_pivot.rotation ,(transform.rotation * _pivotLocalRotation) * Quaternion.AngleAxis(_pivotAngleRange - _pivotAngle, transform.forward.normalized), 10f * Time.deltaTime);
            }
            UpdateIK(_forwardFactor);
        }
        
        public void LateUpdate(Transform forwardFactor)
        {
            UpdateIK(forwardFactor);
        }
        
        private void UpdateIK(Transform forwardFactor)
        {
            if (forwardFactor != null)
            {
                Vector3 forward = forwardFactor.forward;
                forward.y = 0;
                transform.rotation = Quaternion.LookRotation(forward);
            }
            if (_leftHandIK != null)
            {
                _leftHandIKPosition = _leftHandIK.position;
                _leftHandIKRotation = _leftHandIK.rotation;
            }
            if (_rightHandIK != null)
            {
                _rightHandIKPosition = _rightHandIK.position;
                _rightHandIKRotation = _rightHandIK.rotation;
            }
        }
        
        public bool Fire()
        {
            if (Time.realtimeSinceStartup - _fireTime >= _fireRate)
            {
                _fireTime = Time.realtimeSinceStartup;
                if (_projectilePrefab != null)
                {
                    ShootersRushProjectile.Fire(_projectilePrefab.id, _muzzle.position, _muzzle.forward.normalized, _roundsSpeed, _damage, _roundsTrailColor);
                }
                if (_muzzleFlashPrefab != null)
                {
                    ShootersRushMuzzleFlash.Spawn(_muzzleFlashPrefab.id, _muzzle, _muzzle.position, Quaternion.LookRotation(_muzzle.forward));
                }
                /*
                if (_fireAudioSource != null)
                {
                    _fireAudioSource.PlayOneShot(_fireSound);
                }
                */
                return true;
            }
            return false;
        }
        
        public void ApplySettings(Settings settings, Transform forwardFactor)
        {
            if (settings != null)
            {
                transform.localPosition = settings.weaponPosition;
                _pivot.localEulerAngles = settings.weaponRotation;
                _pivotLocalRotation = _pivot.localRotation;
                _leftHandIK.localPosition = settings.leftHandPosition;
                _leftHandIK.localEulerAngles = settings.leftHandRotation;
                _rightHandIK.localPosition = settings.rightHandPosition;
                _rightHandIK.localEulerAngles = settings.rightHandRotation;
                if (forwardFactor != null)
                {
                    _forwardFactor = forwardFactor;
                }
                _muzzle.localRotation = Quaternion.LookRotation(_pivot.InverseTransformDirection(transform.forward));
                UpdateIK(_forwardFactor);
            }
        }
        
        public void _EditorInitialize(string identifier, Transform pivot, Transform muzzle, Transform rhik, Transform lhik, string prefabId)
        {
            #if UNITY_EDITOR
            _id = identifier;
            _pivot = pivot;
            _prefabId = prefabId;
            _muzzle = muzzle;
            _rightHandIK = rhik;
            _leftHandIK = lhik;
            _muzzleFlashPrefab = ShootersRushLibrary.GetMuzzleFlashPrefab(0);
            _projectilePrefab = ShootersRushLibrary.GetProjectilePrefab(0);
            #endif
        }
        
        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (editorSelected)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, transform.position + transform.forward.normalized * 0.5f);
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, transform.position + transform.up.normalized * 0.5f);
                if (_muzzle != null)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(_muzzle.position, 0.01f);
                }
            }
        }
        #endif
        
    }
}