namespace DevelopersHub.ShootersRush.Editor
{
    using UnityEngine;
    using System;
    using UnityEditor;
    
    [CustomEditor(typeof(ShootersRushLibrary))] public class EditorShootersRushLibrary : Editor
    {

        private ShootersRushLibrary _library = null;
        private SerializedProperty _characterPrefabs = null;
        private SerializedProperty _enemyPrefabs = null;
        private SerializedProperty _weaponPrefabs = null;
        private SerializedProperty _projectilePrefabs = null;
        private SerializedProperty _muzzleFlashPrefabs = null;
        private SerializedProperty _containerPrefabs = null;
        private SerializedProperty _gatewayPrefabs = null;
        
        private void OnEnable()
        {
            _library = (ShootersRushLibrary)target;
            _characterPrefabs = serializedObject.FindProperty("_characterPrefabs");
            _enemyPrefabs = serializedObject.FindProperty("_enemyPrefabs");
            _weaponPrefabs = serializedObject.FindProperty("_weaponPrefabs");
            _projectilePrefabs = serializedObject.FindProperty("_projectilePrefabs");
            _muzzleFlashPrefabs = serializedObject.FindProperty("_muzzleFlashPrefabs");
            _containerPrefabs = serializedObject.FindProperty("_containerPrefabs");
            _gatewayPrefabs = serializedObject.FindProperty("_gatewayPrefabs");
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();
            
            EditorGUILayout.PropertyField(_characterPrefabs, true);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_enemyPrefabs, true);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_weaponPrefabs, true);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_projectilePrefabs, true);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_muzzleFlashPrefabs, true);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_containerPrefabs, true);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_gatewayPrefabs, true);
            
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
    
            EditorGUILayout.Space();
            if (GUILayout.Button("Refresh"))
            {
                _library.LoadAllPrefabs();
                _library.ClearNullValues();
            }
        }
        
        public class PrefabCreationWatcher : AssetPostprocessor
        {
            private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
            {
                if (importedAssets != null)
                {
                    ShootersRushLibrary library = ShootersRushLibrary.Singleton;
                    foreach (string assetPath in importedAssets)
                    {
                        if (assetPath.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase))
                        {
                            library.LoadPrefabFromPath(assetPath, true);
                        }
                    }
                }
                if (deletedAssets != null && deletedAssets.Length > 0)
                {
                    foreach (string assetPath in deletedAssets)
                    {
                        if (assetPath.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase))
                        {
                            ShootersRushWeapon weapon = AssetDatabase.LoadAssetAtPath<ShootersRushWeapon>(assetPath);
                            if(weapon)
                            {
                                ShootersRushLibrary.Singleton.RemoveWeaponSettings(weapon.id);
                            }
                        }
                    }
                    ShootersRushLibrary.Singleton.ClearNullValues();
                }
            }
        }
        
    }
}