namespace DevelopersHub.ShootersRush
{
    using System.Collections.Generic;
    using UnityEngine;
    using Unity.AI.Navigation;
    
    public class ShootersRushManager : MonoBehaviour
    {
     
        [SerializeField] private int _startCharacterCount = 1; public int startCharacterCount { get { return _startCharacterCount; } }
        [SerializeField] private int _characterGroupMaxColumns = 5; public int characterGroupMaxColumns { get { return _characterGroupMaxColumns; } }
        [SerializeField] private float _characterGroupColumnsSpace = 1f; public float characterGroupColumnsSpace { get { return _characterGroupColumnsSpace; } }
        [SerializeField] private float _characterGroupRowsSpace = 1f; public float characterGroupRowsSpace { get { return _characterGroupRowsSpace; } }
        [SerializeField] private bool _setCharacterLimit = false; public bool setCharacterLimit { get { return _setCharacterLimit; } }
        [SerializeField] private int _characterLimit = 10; public int characterLimit { get { return _characterLimit; } }
        [SerializeField] private ShootersRushCharacter _startCharacterPrefab = null;
        [SerializeField] private ShootersRushWeapon _startWeaponPrefab = null;
        [SerializeField] private bool _randomizeFireTime = false; public bool randomizeFireTime { get { return _randomizeFireTime; } }
        [SerializeField] private bool _autoRotateWeapon = true; public bool autoRotateWeapon { get { return _autoRotateWeapon; } }
        [SerializeField] private bool _randomizeRotateAngle = true; public bool randomizeRotateAngle { get { return _randomizeRotateAngle; } }
        [SerializeField] private float _weaponAutoRotateRange = 5f; public float weaponAutoRotateRange { get { return _weaponAutoRotateRange; } }
        [SerializeField] private float _weaponAutoRotateSpeed = 20f; public float weaponAutoRotateSpeed { get { return _weaponAutoRotateSpeed; } }
        [SerializeField] private float _roadWidth = 8f; public float roadWidth { get { return _roadWidth; } }
        [SerializeField] private float _shootersPositionOffset = 0.5f; public float shootersPositionOffset { get { return _shootersPositionOffset; } }
        [SerializeField] private float _visibilityRange = 100f; public float visibilityRange { get { return _visibilityRange; } }
        [SerializeField] private float _enemyChargeRange = 10f; public float enemyChargeRange { get { return _enemyChargeRange; } }
        [SerializeField] private float _enemyAttackRange = 0.5f; public float enemyAttackRange { get { return _enemyAttackRange; } }
        [SerializeField] private float _enemySpeed = 2f; public float enemySpeed { get { return _enemySpeed; } }
        [SerializeField] private float _enemyChargeSpeed = 3f; public float enemyChargeSpeed { get { return _enemyChargeSpeed; } }
        [SerializeField] private bool _controlCamera = true;
        [SerializeField] private Camera _camera = null;
        [SerializeField] private float _cameraDistance = 115f; public float cameraDistance { get { return _cameraDistance; } }
        [SerializeField] private float _cameraAngle = 36f; public float cameraAngle { get { return _cameraAngle; } }
        [SerializeField] private float _cameraTargetOffset = 3f; public float cameraTargetOffset { get { return _cameraTargetOffset; } }
        
        private Transform _cameraTarget = null;
        private ShootersRushCharacterGroup _group = null; public ShootersRushCharacterGroup group { get { return _group; } }
        private Transform _startLocation = null;
        private Transform _container = null;
        
        private HashSet<ShootersRushEnemy> _enemies = new HashSet<ShootersRushEnemy>();
        private HashSet<ShootersRushContainer> _containers = new HashSet<ShootersRushContainer>();
        private HashSet<ShootersRushGateway> _gateways = new HashSet<ShootersRushGateway>();
        private Dictionary<ShootersRushSpawner, float> _spawners = new Dictionary<ShootersRushSpawner, float>();

        private float _minSpawnerDistance = 0;
        private Vector3 _basePosition = Vector3.zero;
        private int _gameStatus = 0; public int gameStatus { get { return _gameStatus; } }
        private bool _openFire = true; public bool openFire { get { return _openFire; } set { _openFire = value; } }
        public static event EnemyHitDelegate OnEnemyDamaged;
        public delegate void EnemyHitDelegate(ShootersRushEnemy enemy, float damage, bool dead);
        public static event CharacterHitDelegate OnCharacterDamaged;
        public delegate void CharacterHitDelegate(ShootersRushCharacter character, float damage, bool dead);
        public static event ContainerHitDelegate OnContainerDamaged;
        public delegate void ContainerHitDelegate(ShootersRushContainer container, float damage, bool dead);
        public static event GatewayDelegate OnGatewayTriggered;
        public delegate void GatewayDelegate(ShootersRushGateway gateway);
        
        private static ShootersRushManager _singleton = null; public static ShootersRushManager Singleton
        {
            get
            {
                if (_singleton == null)
                {
                    _singleton = FindFirstObjectByType<ShootersRushManager>();
                }
                return _singleton;
            }
        }
        
        private void Awake()
        {
            _startLocation = transform;
            CreateSurface();
            StartGame();
        }

        private void CreateSurface()
        {
            GameObject surfaceObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            surfaceObject.name = "Surface";
            NavMeshSurface surface = surfaceObject.AddComponent<NavMeshSurface>();
            surface.collectObjects = CollectObjects.Children;
            surfaceObject.transform.localScale = new Vector3((_roadWidth + surface.GetBuildSettings().agentRadius * 2f) * 0.1f, 1f, _visibilityRange * 0.1f);
            surfaceObject.transform.rotation = _startLocation.transform.rotation;
            surfaceObject.transform.position = _startLocation.position + surfaceObject.transform.localScale.z * 0.5f * 10f * _startLocation.forward.normalized;
            surface.BuildNavMesh();
            surfaceObject.GetComponent<MeshRenderer>().enabled = false;
        }
        
        public void StartGame()
        {
            if (_cameraTarget == null)
            {
                _cameraTarget = new GameObject("CameraTarget").transform;
            }
            if (_group != null)
            {
                Destroy(_group.gameObject);
            }

            _group = new GameObject("PlayerGroup").AddComponent<ShootersRushCharacterGroup>();
            _group.transform.position = _startLocation.position - _startLocation.forward.normalized * _shootersPositionOffset;
            _group.transform.rotation = _startLocation.rotation;
            for (int i = 0; i < _startCharacterCount; i++)
            {
                ShootersRushCharacter character = Instantiate(_startCharacterPrefab, _startLocation.position, Quaternion.identity);
                _group.AddCharacter(character);
            }
            _group.ChangeWeapon(_startWeaponPrefab);
            SetupCamera();
            
            if (_container == null)
            {
                _container = new GameObject("Container").transform;
            }
            _container.transform.position = transform.position;
            _container.transform.rotation = transform.rotation;
            _basePosition = _container.position;
            _enemies.Clear();
            _containers.Clear();
            _gateways.Clear();
            _spawners.Clear();
            _minSpawnerDistance = Mathf.Infinity;
            ShootersRushSpawner[] spawners = FindObjectsByType<ShootersRushSpawner>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            if (spawners != null)
            {
                for (int i = 0; i < spawners.Length; i++)
                {
                    spawners[i].transform.SetParent(_container);
                    float distance = Vector3.Distance(spawners[i].transform.position, _basePosition);
                    _spawners.Add(spawners[i], distance);
                    if (distance < _minSpawnerDistance)
                    {
                        _minSpawnerDistance = distance;
                    }
                }
            }
        }

        private void SetupCamera()
        {
            if (!_controlCamera) { return; }
            if (_camera == null)
            {
                _camera = Camera.main;
            }
            _cameraTarget.SetParent(_group.transform);
            _camera.orthographic = false;
            ResetCameraPosition();
        }

        public void ResetCameraPosition()
        {
            Vector3 targetPosition = _group.pivot.position + _group.pivot.forward.normalized * _cameraTargetOffset;
            float x = Mathf.Sin(_cameraAngle * Mathf.Deg2Rad) * _cameraDistance;
            float k = Mathf.Cos(_cameraAngle* Mathf.Deg2Rad) * _cameraDistance;
            _cameraTarget.localPosition = new Vector3(_group.pivot.localPosition.x, x, -k + _cameraTargetOffset);
            _cameraTarget.rotation = Quaternion.LookRotation(targetPosition - _cameraTarget.transform.position);
            _camera.transform.position = _cameraTarget.position;
            _camera.transform.rotation = _cameraTarget.rotation;
        }
        
        private int _interval = 3;
        
        private void Update()
        {
            _container.position -= transform.forward.normalized * _enemySpeed * Time.deltaTime;
            if (_gameStatus != 0) { return; }
            if (Time.frameCount % _interval != 0) { return; }
            HashSet<ShootersRushSpawner> toRemove = new HashSet<ShootersRushSpawner>();
            float distance = Vector3.Distance(_container.position, _basePosition) + _visibilityRange;
            foreach (var spawner in _spawners)
            {
                if (spawner.Value - distance <= 0)
                {
                    switch (spawner.Key.type)
                    {
                        case ShootersRushSpawner.Type.Enemy:
                            if (spawner.Key.enemyPrefab != null)
                            {
                                ShootersRushEnemy enemy = Instantiate(spawner.Key.enemyPrefab, spawner.Key.transform.position, Quaternion.LookRotation(-transform.forward));
                                _enemies.Add(enemy);
                            }
                            break;
                        case ShootersRushSpawner.Type.Container:
                            if (spawner.Key.containerPrefab != null)
                            {
                                ShootersRushContainer container = Instantiate(spawner.Key.containerPrefab, spawner.Key.transform.position, Quaternion.LookRotation(-transform.forward));
                                container.Initialize(spawner.Key.containerHealth);
                                Vector3 position = container.transform.position;
                                position.y += container.CapsuleCollider.radius;
                                container.transform.position = position;
                                container.transform.SetParent(_container);
                                container.CreateContent(spawner.Key.contentHeight, spawner.Key.characterPrefab, spawner.Key.weaponPrefab, spawner.Key.characterCount, spawner.Key.contentScale, spawner.Key.contentPosition, spawner.Key.contentSpace);
                                _containers.Add(container);
                            }
                            break;
                        case ShootersRushSpawner.Type.Gateway:
                            if (spawner.Key.gatewayPrefab != null)
                            {
                                ShootersRushGateway gateway = Instantiate(spawner.Key.gatewayPrefab, spawner.Key.transform.position, Quaternion.LookRotation(-transform.forward));
                                gateway.Initialize(spawner.Key.action, spawner.Key.actionAmount, spawner.Key.actionColor);
                                // Vector3 position = gateway.transform.position;
                                // position.y += gateway.boxCollider.size.y * 0.5f;
                                // gateway.transform.position = position;
                                gateway.transform.SetParent(_container);
                                _gateways.Add(gateway);
                            }
                            break;
                    }
                    toRemove.Add(spawner.Key);
                }
            }
            
            foreach (var remove in toRemove)
            {
                _spawners.Remove(remove);
                Destroy(remove.gameObject);
            }
            
            if (_group.charactersCount <= 0)
            {
                _gameStatus = -1;
                Debug.Log("Lost");
                GameDone();
            }
            else if (_spawners.Count <= 0 && _enemies.Count <= 0)
            {
                _gameStatus = 1;
                Debug.Log("Won");
                GameDone();
            }
           
            /*
            #if UNITY_EDITOR
            if (Application.isPlaying)
            {
                
            }
            #endif
            */
        }

        private void GameDone()
        {
            
        }
        
        private void LateUpdate()
        {
            ResetCameraPosition();
        }
        
        public void _GatewayTriggered(ShootersRushGateway gateway)
        {
            if (gateway != null)
            {
                if (_group.charactersCount > 0)
                {
                    switch (gateway.action)
                    {
                        case ShootersRushGateway.Action.Multiply:
                        case ShootersRushGateway.Action.Add:
                            int toAdd = gateway.action == ShootersRushGateway.Action.Add ? (int)gateway.actionAmount : ((int)gateway.actionAmount - 1) * _group.charactersCount;
                            if (_setCharacterLimit && _group.charactersCount + toAdd > _characterLimit)
                            {
                                toAdd = _characterLimit - _group.charactersCount;
                            }
                            if (toAdd <= 0)
                            {
                                break;
                            }
                            ShootersRushCharacter prefab = ShootersRushLibrary.GetCharacterPrefab(_group.characters[0].id);
                            ShootersRushWeapon weaponPrefab = ShootersRushLibrary.GetWeaponPrefab(_group.characters[0].weapon.id);
                            if (prefab != null && weaponPrefab != null)
                            {
                                for (int i = 0; i < toAdd; i++)
                                {
                                    ShootersRushCharacter character = Instantiate(prefab, _group.transform.position, Quaternion.identity);
                                    character.EquipWeapon(weaponPrefab);
                                    _group.AddCharacter(character);
                                }
                            }
                            break;
                        case ShootersRushGateway.Action.Subtract:
                            int count = (int)gateway.actionAmount;
                            if (count > _group.charactersCount)
                            {
                                count = _group.charactersCount;
                            }
                            for (int i = count - 1; i >= 0; i--)
                            {
                                ShootersRushCharacter character = _group.characters[i];
                                _group.RemoveCharacter(character);
                                Destroy(character.gameObject);
                            }
                            break;
                    }
                }
                if (OnGatewayTriggered != null)
                {
                    OnGatewayTriggered.Invoke(gateway);
                }
                _gateways.Remove(gateway);
                Destroy(gateway.gameObject);
            }
        }
        
        public void _EnemyDamaged(ShootersRushEnemy enemy, float damage, bool dead)
        {
            if (enemy != null)
            {
                if (dead)
                {
                    _enemies.Remove(enemy);
                }
                if (OnEnemyDamaged != null)
                {
                    OnEnemyDamaged.Invoke(enemy, damage, dead);
                }
            }
        }
        
        public void _ContainerDamaged(ShootersRushContainer container, float damage, bool dead)
        {
            if (container != null)
            {
                if (dead)
                {
                    _containers.Remove(container);
                }
                if (OnContainerDamaged != null)
                {
                    OnContainerDamaged.Invoke(container, damage, dead);
                }
            }
        }
        
        public void _ContainerTimeout(ShootersRushContainer container)
        {
            if (container != null)
            {
                _containers.Remove(container);
            }
        }
        
        public void _GatewayTimeout(ShootersRushGateway gateway)
        {
            if (gateway != null)
            {
                _gateways.Remove(gateway);
            }
        }

        public void _CharacterDamaged(ShootersRushCharacter character, float damage, bool dead)
        {
            if (character != null)
            {
                if (OnCharacterDamaged != null)
                {
                    OnCharacterDamaged.Invoke(character, damage, dead);
                }
            }
        }
        
        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Vector3 start = transform.position - transform.right.normalized * _roadWidth * 0.5f;
            Vector3 end = transform.position + transform.right.normalized * _roadWidth * 0.5f;
            Gizmos.DrawLine(start, end);
            // Gizmos.DrawLine(start, start - transform.forward.normalized * 5f);
            // Gizmos.DrawLine(end, end - transform.forward.normalized * 5f);
            //UnityEditor.Handles.Label(transform.position, "Front Line");
            
            Gizmos.color = Color.green;
            Gizmos.DrawLine(start - transform.forward.normalized * _shootersPositionOffset, end - transform.forward.normalized * _shootersPositionOffset);
            UnityEditor.Handles.Label(transform.position - transform.forward.normalized * _shootersPositionOffset, "Shooters Position");
            
            Gizmos.color = Color.red;
            Gizmos.DrawLine(start + transform.forward.normalized * _enemyChargeRange, end + transform.forward.normalized * _enemyChargeRange);
            UnityEditor.Handles.Label(transform.position + transform.forward.normalized * _enemyChargeRange, "Charge Line");
            
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(start, start + transform.forward.normalized * _visibilityRange);
            Gizmos.DrawLine(end, end + transform.forward.normalized * _visibilityRange);
            Gizmos.DrawLine(start + transform.forward.normalized * _visibilityRange, end + transform.forward.normalized * _visibilityRange);
           
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(start + transform.forward.normalized * _visibilityRange, start + transform.forward.normalized * 10000f);
            Gizmos.DrawLine(end + transform.forward.normalized * _visibilityRange, end + transform.forward.normalized * 10000f);
            
            /*
            float lineSpace = _roadWidth / _roadLines;
            start = transform.position + transform.forward.normalized * 1f - transform.right.normalized * _roadWidth * 0.5f;
            for (int i = 1; i <= _roadLines; i++)
            {
                UnityEditor.Handles.Label(start + transform.right.normalized * ((i - 1) * lineSpace + lineSpace / 2f), "Line " + i.ToString());
            }
            */
            /*
            if (!Application.isPlaying && _controlCamera)
            {
                Gizmos.color = Color.blue;
                Camera cam = Camera.main;
                float x = Mathf.Sqrt(_cameraDistance * _cameraDistance - _cameraHeight * _cameraHeight);
                Vector3 cameraPosition = transform.position + Vector3.up * _cameraHeight - transform.forward.normalized * x;
                Gizmos.matrix = Matrix4x4.TRS( cameraPosition, Quaternion.LookRotation(transform.position - cameraPosition), Vector3.one);
                Gizmos.DrawFrustum(cameraPosition, cam.fieldOfView, cam.farClipPlane, cam.nearClipPlane, cam.aspect);
                // UnityEditor.Handles.Label(cameraPosition, "Camera");
            }
            */
        }
        #endif
        
    }
}