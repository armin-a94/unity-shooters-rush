namespace DevelopersHub.ShootersRush.Editor
{
    using UnityEngine;
    using System;
    using UnityEditor;

    [CustomEditor(typeof(ShootersRushSpawner))] public class EditorShootersRushSpawner : Editor
    {
        
        private ShootersRushSpawner _spawner = null;
        private SerializedProperty _type = null;
        private SerializedProperty _enemyPrefab = null;
        private SerializedProperty _containerPrefab = null;
        private SerializedProperty _gatewayPrefab = null;
        private SerializedProperty _action = null;
        private SerializedProperty _actionAmount = null;
        private SerializedProperty _actionColor = null;
        private SerializedProperty _containerHealth = null;
        private SerializedProperty _characterPrefab = null;
        private SerializedProperty _weaponPrefab = null;
        private SerializedProperty _characterCount = null;
        private SerializedProperty _contentHeight = null;
        private SerializedProperty _contentScale = null;
        private SerializedProperty _contentPosition = null;
        private SerializedProperty _contentSpace = null;
        private bool _managerExists = false;
        private ShootersRushContainer _containerContent = null;
        
        private void OnEnable()
        {
            if (Application.isPlaying)
            {
                return;
            }
            _spawner = (ShootersRushSpawner)target;
            _managerExists = ShootersRushManager.Singleton != null;
            _type = serializedObject.FindProperty("_type");
            _enemyPrefab = serializedObject.FindProperty("_enemyPrefab");
            _containerPrefab = serializedObject.FindProperty("_containerPrefab");
            _gatewayPrefab = serializedObject.FindProperty("_gatewayPrefab");
            _action = serializedObject.FindProperty("_action");
            _actionAmount = serializedObject.FindProperty("_actionAmount");
            _actionColor = serializedObject.FindProperty("_actionColor");
            _containerHealth = serializedObject.FindProperty("_containerHealth");
            _characterPrefab = serializedObject.FindProperty("_characterPrefab");
            _weaponPrefab = serializedObject.FindProperty("_weaponPrefab");
            _characterCount = serializedObject.FindProperty("_characterCount");
            _contentHeight = serializedObject.FindProperty("_contentHeight");
            _contentScale = serializedObject.FindProperty("_contentScale");
            _contentPosition = serializedObject.FindProperty("_contentPosition");
            _contentSpace = serializedObject.FindProperty("_contentSpace");
            _containerContent = _spawner._EditorCreateContent();
        }

        private void OnDisable()
        {
            if (Application.isPlaying || _spawner == null)
            {
                return;
            }
            _spawner.RemoveContent();
            if (_spawner.transform.childCount > 0)
            {
                DestroyImmediate(_spawner.transform.GetChild(0).gameObject);
            }
        }
        
        public override void OnInspectorGUI()
        {
            if (Application.isPlaying)
            {
                return;
            }
            EditorGUILayout.Space();
            if (_managerExists)
            {
                int typeIndex = _type.enumValueIndex;
                EditorGUI.BeginChangeCheck();
                Vector3 localPosition = ShootersRushManager.Singleton.transform.InverseTransformPoint(_spawner.transform.position);
                Vector3 targetLocalPosition = localPosition;
                targetLocalPosition.y = 0;
                _type.enumValueIndex = (int)(ShootersRushSpawner.Type)EditorGUILayout.EnumPopup("Type", (ShootersRushSpawner.Type)Enum.GetValues(typeof(ShootersRushSpawner.Type)).GetValue(_type.enumValueIndex));
                switch (_spawner.type)
                {
                    case ShootersRushSpawner.Type.Enemy:
                        _enemyPrefab.objectReferenceValue = EditorGUILayout.ObjectField("Prefab", _enemyPrefab.objectReferenceValue, typeof(ShootersRushEnemy), false);
                        if (targetLocalPosition.z < 0)
                        {
                            targetLocalPosition.z = 0;
                        }
                        if (targetLocalPosition.x > ShootersRushManager.Singleton.roadWidth * 0.5f)
                        {
                            targetLocalPosition.x = ShootersRushManager.Singleton.roadWidth * 0.5f;
                        }
                        else if (targetLocalPosition.x < -ShootersRushManager.Singleton.roadWidth * 0.5f)
                        {
                            targetLocalPosition.x = -ShootersRushManager.Singleton.roadWidth * 0.5f;
                        }
                        break;
                    case ShootersRushSpawner.Type.Container:
                        _containerHealth.intValue = EditorGUILayout.IntField("Hit Points", _containerHealth.intValue);
                        if (_containerHealth.intValue <= 0)
                        {
                            _containerHealth.intValue = 1;
                        }
                        _containerPrefab.objectReferenceValue = EditorGUILayout.ObjectField("Prefab", _containerPrefab.objectReferenceValue, typeof(ShootersRushContainer), false);
                        if (_spawner.containerPrefab != null)
                        {
                            CapsuleCollider _collider = _spawner.containerPrefab.GetComponent<CapsuleCollider>();
                            if (_collider != null)
                            {
                                float minZ = _collider.radius;
                                float minX = -(ShootersRushManager.Singleton.roadWidth - _collider.height) * 0.5f;
                                float maxX = (ShootersRushManager.Singleton.roadWidth - _collider.height) * 0.5f;
                                if (targetLocalPosition.z < minZ)
                                {
                                    targetLocalPosition.z = minZ;
                                }
                                if (targetLocalPosition.x > maxX)
                                {
                                    targetLocalPosition.x = maxX;
                                }
                                else if (targetLocalPosition.x < minX)
                                {
                                    targetLocalPosition.x = minX;
                                }
                            }
                            _contentHeight.floatValue = EditorGUILayout.FloatField("Height", _contentHeight.floatValue);
                            _contentScale.floatValue = EditorGUILayout.FloatField("Scale", _contentScale.floatValue);
                            _contentPosition.floatValue = EditorGUILayout.FloatField("Position", _contentPosition.floatValue);
                            _contentSpace.floatValue = EditorGUILayout.FloatField("Space", _contentSpace.floatValue);
                            _weaponPrefab.objectReferenceValue = EditorGUILayout.ObjectField("Weapon Prefab", _weaponPrefab.objectReferenceValue, typeof(ShootersRushWeapon), false);
                            _characterPrefab.objectReferenceValue = EditorGUILayout.ObjectField("Character Prefab", _characterPrefab.objectReferenceValue, typeof(ShootersRushCharacter), false);
                            if (_contentSpace.floatValue < 0.1f)
                            {
                                _contentSpace.floatValue = 0.1f;
                            }
                            if (_characterPrefab.objectReferenceValue != null)
                            {
                                _characterCount.intValue = EditorGUILayout.IntField("Character Count", _characterCount.intValue);
                                if (_characterCount.intValue <= 0)
                                {
                                    _characterCount.intValue = 1;
                                }
                            }
                        }
                        break;
                    case ShootersRushSpawner.Type.Gateway:
                        _gatewayPrefab.objectReferenceValue = EditorGUILayout.ObjectField("Prefab", _gatewayPrefab.objectReferenceValue, typeof(ShootersRushGateway), false);
                        if (_spawner.gatewayPrefab != null)
                        {
                            BoxCollider _collider = _spawner.gatewayPrefab.GetComponent<BoxCollider>();
                            if (_collider != null)
                            {
                                float minZ = _collider.size.z * 0.5f;
                                float minX = -(ShootersRushManager.Singleton.roadWidth - _collider.size.x) * 0.5f;
                                float maxX = (ShootersRushManager.Singleton.roadWidth - _collider.size.x) * 0.5f;
                                if (targetLocalPosition.z < minZ)
                                {
                                    targetLocalPosition.z = minZ;
                                }
                                if (targetLocalPosition.x > maxX)
                                {
                                    targetLocalPosition.x = maxX;
                                }
                                else if (targetLocalPosition.x < minX)
                                {
                                    targetLocalPosition.x = minX;
                                }
                            }
                            _action.enumValueIndex = (int)(ShootersRushGateway.Action)EditorGUILayout.EnumPopup("Action", (ShootersRushGateway.Action)Enum.GetValues(typeof(ShootersRushGateway.Action)).GetValue(_action.enumValueIndex));
                            _actionAmount.intValue = EditorGUILayout.IntField("Amount", _actionAmount.intValue);
                            if (_actionAmount.intValue <= 0)
                            {
                                _actionAmount.intValue = 1;
                            }
                            _actionColor.colorValue = EditorGUILayout.ColorField("Color", _actionColor.colorValue);
                        }
                        break;
                }
                if (localPosition != targetLocalPosition)
                {
                    _spawner.transform.position = ShootersRushManager.Singleton.transform.TransformPoint(targetLocalPosition);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    _containerContent = _spawner._EditorCreateContent();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("You need to have a manager in the scene in order to setup the spawners.", MessageType.Warning);
            }
            EditorGUILayout.Space();
        }
        
    }
}