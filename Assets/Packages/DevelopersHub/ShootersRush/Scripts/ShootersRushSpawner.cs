namespace DevelopersHub.ShootersRush
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class ShootersRushSpawner : MonoBehaviour
    {

        [SerializeField] private Type _type = Type.Enemy; public Type type { get { return _type; } }
        [SerializeField] private ShootersRushEnemy _enemyPrefab = null; public ShootersRushEnemy enemyPrefab { get { return _enemyPrefab; } }
        [SerializeField] private ShootersRushContainer _containerPrefab = null; public ShootersRushContainer containerPrefab { get { return _containerPrefab; } }
        [SerializeField] private ShootersRushGateway _gatewayPrefab = null; public ShootersRushGateway gatewayPrefab { get { return _gatewayPrefab; } }
        [SerializeField] private ShootersRushGateway.Action _action = ShootersRushGateway.Action.Add; public ShootersRushGateway.Action action { get { return _action; } }
        [SerializeField] private uint _actionAmount = 1; public uint actionAmount { get { return _actionAmount; } }
        [SerializeField] private Color _actionColor = new Color(0, 1, 0, 1); public Color actionColor { get { return _actionColor; } }
        [SerializeField] private int _containerHealth = 100; public int containerHealth { get { return _containerHealth; } }
        [SerializeField] private ShootersRushCharacter _characterPrefab = null; public ShootersRushCharacter characterPrefab { get { return _characterPrefab; } }
        [SerializeField] private ShootersRushWeapon _weaponPrefab = null; public ShootersRushWeapon weaponPrefab { get { return _weaponPrefab; } }
        [SerializeField] private int _characterCount = 1; public int characterCount { get { return _characterCount; } }
        [SerializeField] private float _contentHeight = 1f; public float contentHeight { get { return _contentHeight; } }
        [SerializeField] private float _contentScale = 1f; public float contentScale { get { return _contentScale; } }
        [SerializeField] private float _contentPosition = 0f; public float contentPosition { get { return _contentPosition; } }
        [SerializeField] private float _contentSpace = 0.5f; public float contentSpace { get { return _contentSpace; } }
        private ShootersRushContainer _container = null;
        private ShootersRushEnemy _enemy = null;
        private ShootersRushGateway _gateway = null;
        
        public enum Type
        {
            Enemy = 0, Container = 1, Gateway = 2
        }

        public ShootersRushContainer _EditorCreateContent()
        {
            RemoveContent();
            if (_type == Type.Container && _containerPrefab != null)
            {
                _container = Instantiate(_containerPrefab, transform);
                float y = 0;
                if (_container.GetComponent<CapsuleCollider>())
                {
                    y = _container.GetComponent<CapsuleCollider>().radius;
                }
                _container.transform.localPosition =  Vector3.zero + transform.up.normalized * y;
                _container.UpdateHealthText(_containerHealth);
                if (ShootersRushManager.Singleton != null)
                {
                    _container.transform.rotation = Quaternion.LookRotation(-ShootersRushManager.Singleton.transform.forward);
                }
                else
                {
                    _container.transform.rotation = Quaternion.LookRotation(transform.forward);
                }
                _container.CreateContent(_contentHeight, _characterPrefab, _weaponPrefab, _characterCount, _contentScale, _contentPosition, _contentSpace);
                return _container;
            }
            else if (_type == Type.Enemy && _enemyPrefab != null)
            {
                _enemy = Instantiate(_enemyPrefab, transform);
                _enemy.Initialize(true);
                _enemy.transform.localPosition = Vector3.zero;
                if (ShootersRushManager.Singleton != null)
                {
                    _enemy.transform.rotation = Quaternion.LookRotation(-ShootersRushManager.Singleton.transform.forward);
                }
                else
                {
                    _enemy.transform.rotation = Quaternion.LookRotation(transform.forward);
                }
            }
            else if (_type == Type.Gateway && _gatewayPrefab != null)
            {
                _gateway = Instantiate(_gatewayPrefab, transform);
                _gateway.Initialize(true);
                _gateway.transform.localPosition = Vector3.zero;
                _gateway.Initialize(_action, _actionAmount, _actionColor, true);
                if (ShootersRushManager.Singleton != null)
                {
                    _gateway.transform.rotation = Quaternion.LookRotation(-ShootersRushManager.Singleton.transform.forward);
                }
                else
                {
                    _gateway.transform.rotation = Quaternion.LookRotation(transform.forward);
                }
            }
            return null;
        }
        
        public void RemoveContent()
        {
            #if UNITY_EDITOR
            if (Application.isPlaying)
            {
                if (_container != null)
                {
                    Destroy(_container.gameObject);
                }
                if (_enemy != null)
                {
                    Destroy(_enemy.gameObject);
                }
                if (_gateway != null)
                {
                    Destroy(_gateway.gameObject);
                }
            }
            else
            {
                ShootersRushContainer[] c = GetComponentsInChildren<ShootersRushContainer>();
                if (c != null) { for (int i = 0; i < c.Length; i++) { DestroyImmediate(c[i].gameObject); } }
                ShootersRushEnemy[] e = GetComponentsInChildren<ShootersRushEnemy>();
                if (e != null) { for (int i = 0; i < e.Length; i++) { DestroyImmediate(e[i].gameObject); } }
                ShootersRushGateway[] g = GetComponentsInChildren<ShootersRushGateway>();
                if (g != null) { for (int i = 0; i < g.Length; i++) { DestroyImmediate(g[i].gameObject); } }
            }
            #else
            if (_container != null)
            {
                Destroy(_container.gameObject);
            }
            if (_enemy != null)
            {
                Destroy(_enemy.gameObject);
            }
            if (_gateway != null)
            {
                Destroy(_gateway.gameObject);
            }
            #endif
        }
        
        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            UnityEditor.Handles.Label(transform.position, gameObject.name);
            if (ShootersRushManager.Singleton == null)
            {
                return;
            }
            Transform directions = transform;
            if (ShootersRushManager.Singleton != null)
            {
                directions = ShootersRushManager.Singleton.transform;
            }
            switch (_type)
            {
                case Type.Enemy:
                    //UnityEditor.Handles.color = Color.red;
                    //UnityEditor.Handles.DrawWireArc(transform.position, transform.up, transform.forward, 360, _enemyPrefab != null ? _enemyPrefab.radius : 0.5f);
                    Gizmos.color = Color.red;
                    Vector3 p01 = transform.position - directions.right.normalized * _enemyPrefab.radius - directions.forward.normalized * _enemyPrefab.radius;
                    Vector3 p02 = p01 + directions.right.normalized * _enemyPrefab.radius * 2f;
                    Vector3 p03 = p02 + directions.forward.normalized * _enemyPrefab.radius * 2f;
                    Vector3 p04 = p01 + directions.forward.normalized * _enemyPrefab.radius * 2f;
                    Gizmos.DrawLine(p01, p02);
                    Gizmos.DrawLine(p02, p03);
                    Gizmos.DrawLine(p03, p04);
                    Gizmos.DrawLine(p04, p01);
                    break;
                case Type.Container:
                    if (_containerPrefab != null)
                    {
                        CapsuleCollider capsuleCollider = _containerPrefab.GetComponent<CapsuleCollider>();
                        if (capsuleCollider != null)
                        {
                            Gizmos.color = Color.blue;
                            Vector3 p1 = transform.position - directions.right.normalized * capsuleCollider.height * 0.5f - directions.forward.normalized * capsuleCollider.radius;
                            Vector3 p2 = p1 + directions.right.normalized * capsuleCollider.height;
                            Vector3 p3 = p2 + directions.forward.normalized * capsuleCollider.radius * 2f;
                            Vector3 p4 = p1 + directions.forward.normalized * capsuleCollider.radius * 2f;
                            Gizmos.DrawLine(p1, p2);
                            Gizmos.DrawLine(p2, p3);
                            Gizmos.DrawLine(p3, p4);
                            Gizmos.DrawLine(p4, p1);
                        }
                    }
                    break;
                case Type.Gateway:
                    if (_gatewayPrefab != null)
                    {
                        BoxCollider boxCollider = _gatewayPrefab.GetComponent<BoxCollider>();
                        if (boxCollider != null)
                        {
                            Gizmos.color = Color.cyan;
                            Vector3 p1 = transform.position - directions.right.normalized * boxCollider.size.x * 0.5f - directions.forward.normalized * boxCollider.size.z * 0.5f;
                            Vector3 p2 = p1 + directions.right.normalized * boxCollider.size.x;
                            Vector3 p3 = p2 + directions.forward.normalized * boxCollider.size.z;
                            Vector3 p4 = p1 + directions.forward.normalized * boxCollider.size.z;
                            Gizmos.DrawLine(p1, p2);
                            Gizmos.DrawLine(p2, p3);
                            Gizmos.DrawLine(p3, p4);
                            Gizmos.DrawLine(p4, p1);
                        }
                    }
                    break;
            }
        }
        #endif
        
    }
}