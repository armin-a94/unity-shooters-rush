namespace DevelopersHub.ShootersRush.Editor
{
    using UnityEngine;
    using System;
    using UnityEditor;
    using UnityEditor.SceneManagement;

    [CustomEditor(typeof(ShootersRushMuzzleFlash))] public class EditorShootersRushMuzzleFlash : Editor
    {

        private ShootersRushMuzzleFlash _muzzleFlash = null;
        private SerializedProperty _id = null;
        private SerializedProperty _prefabId = null;
        private SerializedProperty _lifetime = null;
        private bool _duplicateID = false;
        
        private void OnEnable()
        {
            if (IsInPrefabEditMode())
            {
                return;
            }
            _muzzleFlash = (ShootersRushMuzzleFlash)target;
            _id = serializedObject.FindProperty("_id");
            _prefabId = serializedObject.FindProperty("_prefabId");
            _lifetime = serializedObject.FindProperty("_lifetime");
            CheckID();
        }

        public override void OnInspectorGUI()
        {
            if (IsInPrefabEditMode())
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("The tools are not available in prefab editing mode.", MessageType.Warning);
                EditorGUILayout.Space();
                return;
            }
            serializedObject.Update();
            bool modified = false;
            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();
            _id.stringValue = EditorGUILayout.TextField("ID", _id.stringValue);
            if (EditorGUI.EndChangeCheck())
            {
                _duplicateID = ShootersRushLibrary.GetMuzzleFlashPrefabsCount(_id.stringValue) > 1;
                modified = true;
            }
            if (string.IsNullOrEmpty(_id.stringValue))
            {
                EditorGUILayout.HelpBox("The ID should not be empty. Please choose an ID for this muzzle flash.", MessageType.Error);
                EditorGUILayout.Space();
            }
            else if (_duplicateID)
            {
                EditorGUILayout.HelpBox("This ID is also used by other muzzle flashes. Please choose another ID to avoid any issues in your project.", MessageType.Error);
                EditorGUILayout.Space();
            }
            
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("Prefab ID", _prefabId.stringValue);
            EditorGUI.EndDisabledGroup();
            
            EditorGUI.BeginChangeCheck();
            _lifetime.floatValue = EditorGUILayout.FloatField("Lifetime", _lifetime.floatValue);
            if (EditorGUI.EndChangeCheck())
            {
                modified = true;
            }
            
            EditorGUILayout.Space();
            if (modified)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
        
        private void CheckID()
        {
            if (string.IsNullOrEmpty(_prefabId.stringValue))
            {
                _prefabId.stringValue = Guid.NewGuid().ToString();
                serializedObject.ApplyModifiedProperties();
            }
        }
        
        public static bool IsInPrefabEditMode()
        {
            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            return prefabStage != null;
        }
        
    }
}