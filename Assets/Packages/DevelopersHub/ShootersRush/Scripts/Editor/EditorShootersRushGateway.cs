namespace DevelopersHub.ShootersRush.Editor
{
    using UnityEngine;
    using System;
    using UnityEditor;
    using UnityEditor.UIElements;
    using UnityEngine.UI;
    using UnityEngine.UIElements;
    using UnityEditor.SceneManagement;
    using System.IO;
    using System.Linq;
    
    [CustomEditor(typeof(ShootersRushGateway))] public class EditorShootersRushGateway : Editor
    {
        
        private ShootersRushGateway _gateway = null;
        private SerializedProperty _id = null;
        private SerializedProperty _prefabId = null;
        private SerializedProperty _textMesh = null;
        private SerializedProperty _spriteRenderer = null;
        private bool _duplicateID = false;
        
        private void OnEnable()
        {
            EditorShootersRushTools.AddTag("Gateway");
            _gateway = (ShootersRushGateway)target;
            _id = serializedObject.FindProperty("_id");
            _prefabId = serializedObject.FindProperty("_prefabId");
            _textMesh = serializedObject.FindProperty("_textMesh");
            _spriteRenderer = serializedObject.FindProperty("_spriteRenderer");
            CheckID();
        }

        public override void OnInspectorGUI()
        {
            if (EditorShootersRushTools.IsInPrefabEditMode())
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
                _duplicateID = ShootersRushLibrary.GetGatewayPrefabsCount(_id.stringValue) > 1;
                modified = true;
            }
            if (string.IsNullOrEmpty(_id.stringValue))
            {
                EditorGUILayout.HelpBox("The ID should not be empty. Please choose an ID for this gateway.", MessageType.Error);
                EditorGUILayout.Space();
            }
            else if (_duplicateID)
            {
                EditorGUILayout.HelpBox("This ID is also used by other gateways. Please choose another ID to avoid any issues in your project.", MessageType.Error);
                EditorGUILayout.Space();
            }
            
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("Prefab ID", _prefabId.stringValue);
            EditorGUI.EndDisabledGroup();
            
            EditorGUI.BeginChangeCheck();
            _spriteRenderer.objectReferenceValue = EditorGUILayout.ObjectField("Sprite Renderer", _spriteRenderer.objectReferenceValue, typeof(SpriteRenderer), true);
            _textMesh.objectReferenceValue = EditorGUILayout.ObjectField("Text Mesh", _textMesh.objectReferenceValue, typeof(TMPro.TextMeshPro), true);
            if (EditorGUI.EndChangeCheck())
            {
                modified = true;
            }
            
            EditorGUILayout.Space();
            if (modified)
            {
                serializedObject.ApplyModifiedProperties();
                _gateway.Initialize(_gateway.action, _gateway.actionAmount, _gateway.spriteRenderer.color, true);
            }
            
            if (PrefabUtility.GetPrefabAssetType(target) == PrefabAssetType.NotAPrefab && !Application.isPlaying)
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("This gateway is not a prefab. You can not access it from the library.", MessageType.Warning);
                if (GUILayout.Button("Save As Prefab"))
                {
                    string path = new string[] { "Assets", "Packages", "DevelopersHub", "ShootersRush", "Prefabs" }.Aggregate(Path.Combine);
                    string prefabPath = Path.Combine(path, "Gateways");
                    if (!Directory.Exists(prefabPath))
                    {
                        AssetDatabase.CreateFolder(path, "Gateways");
                    }
                    prefabPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(prefabPath, _gateway.gameObject.name + ".prefab"));
                    GameObject prefab = PrefabUtility.SaveAsPrefabAsset(_gateway.gameObject, prefabPath, out bool saved);
                    if (saved)
                    { 
                        DestroyImmediate(_gateway.gameObject);
                        UnityEditor.Selection.activeObject = prefab;
                        return;
                    }
                }
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

    }
}