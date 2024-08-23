namespace DevelopersHub.ShootersRush
{
    using UnityEngine;
    using TMPro;
    
    public class ShootersRushGateway : MonoBehaviour
    {
       
        [SerializeField] private string _id = ""; public string id { get { return _id; } }
        [SerializeField] private string _prefabId = ""; public string prefabID { get { return _prefabId; } }
        [SerializeField] private Action _action = Action.Add; public Action action { get { return _action; } }
        [SerializeField] private uint _actionAmount = 1; public uint actionAmount { get { return _actionAmount; } }
        [SerializeField] private TextMeshPro _textMesh = null; public TextMeshPro textMesh { get { return _textMesh; } }
        [SerializeField] private SpriteRenderer _spriteRenderer = null; public SpriteRenderer spriteRenderer { get { return _spriteRenderer; } }
        
        private bool _initialized = false;
        private BoxCollider _collider = null; public BoxCollider boxCollider { get { return _collider; } }
        [SerializeField] private bool _triggered = false; public bool triggered { get { return _triggered; }  set { _triggered = value; }}
        private float _checkTimer = 0;
        private bool _dummy = false;
        
        public enum Action
        {
            Multiply = 0, Add = 1, Subtract = 2
        }
        
        private void Awake()
        {
            Initialize();
        }
        
        public void Initialize(bool isEditor = false)
        {
            if (_initialized) { return; }
            _initialized = true;
            _dummy = isEditor;
            if (isEditor) { return; }
            #if UNITY_EDITOR
            ShootersRushTools.AddTag("Gateway");
            #endif
            gameObject.tag = "Gateway";
            _collider = GetComponent<BoxCollider>();
            _collider.isTrigger = true;
        }

        public void Initialize(Action action, uint actionAmount, Color color, bool isEditor = false)
        {
            Initialize();
            if (!isEditor)
            {
                _actionAmount = actionAmount;
                _action = action;
            }
            switch (action)
            {
                case Action.Multiply:
                    _textMesh.SetText("x" + actionAmount.ToString());
                    break;
                case Action.Add:
                    _textMesh.SetText("+" + actionAmount.ToString());
                    break;
                case Action.Subtract:
                    _textMesh.SetText("-" + actionAmount.ToString());
                    break;
            }
            #if UNITY_EDITOR
            if (isEditor)
            {
                _textMesh.ForceMeshUpdate();
            }
            #endif
            _spriteRenderer.color = color;
        }
        
        private void Update()
        {
            _checkTimer += Time.deltaTime;
            if (_checkTimer > 1f)
            {
                _checkTimer = 0;
                Vector3 localPosition = ShootersRushManager.Singleton.transform.InverseTransformPoint(transform.position);
                if (localPosition.z < 0 && -localPosition.z >= ShootersRushManager.Singleton.cameraDistance)
                {
                    ShootersRushManager.Singleton._GatewayTimeout(this);
                    Destroy(gameObject);
                }
            }
        }
        
        public void _EditorInitialize(string identifier, string prefabId)
        {
            #if UNITY_EDITOR
            _id = identifier;
            _prefabId = prefabId;
            #endif
        }
        
    }
}