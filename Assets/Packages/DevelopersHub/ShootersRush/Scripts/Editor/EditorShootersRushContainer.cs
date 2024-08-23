namespace DevelopersHub.ShootersRush.Editor
{
    using UnityEngine;
    using System;
    using UnityEditor;
    using UnityEditor.UIElements;
    using UnityEngine.UIElements;
    using UnityEditor.SceneManagement;
    using System.IO;
    using System.Linq;
    using TMPro;
    
    [CustomEditor(typeof(ShootersRushContainer))] public class EditorShootersRushContainer : Editor
    {
        
        private ShootersRushContainer _container = null;
        private SerializedProperty _id = null;
        private SerializedProperty _prefabId = null;
        //private SerializedProperty _health = null;
        private SerializedProperty _textMesh = null;
        private bool _duplicateID = false;
        ShootersRushManager _manager = null;
        
        private void OnEnable()
        {
            _container = (ShootersRushContainer)target;
            _id = serializedObject.FindProperty("_id");
            _prefabId = serializedObject.FindProperty("_prefabId");
            //_health = serializedObject.FindProperty("_health");
            _textMesh = serializedObject.FindProperty("_textMesh");
            _manager = ShootersRushManager.Singleton;
            CheckID();
            UnityEditor.EditorApplication.update += EditorUpdate;
        }
        
        private void OnDisable()
        {
            UnityEditor.EditorApplication.update -= EditorUpdate;
        }

        private void EditorUpdate()
        {
            if (_container != null && _container.pivot != null)
            {
                _container.pivot.Rotate(Vector3.right, 20f * Time.deltaTime, Space.Self);
            }
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
                _duplicateID = ShootersRushLibrary.GetContainerPrefabsCount(_id.stringValue) > 1;
                modified = true;
            }
            if (string.IsNullOrEmpty(_id.stringValue))
            {
                EditorGUILayout.HelpBox("The ID should not be empty. Please choose an ID for this container.", MessageType.Error);
                EditorGUILayout.Space();
            }
            else if (_duplicateID)
            {
                EditorGUILayout.HelpBox("This ID is also used by other containers. Please choose another ID to avoid any issues in your project.", MessageType.Error);
                EditorGUILayout.Space();
            }
            
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("Prefab ID", _prefabId.stringValue);
            EditorGUI.EndDisabledGroup();
            
            EditorGUI.BeginChangeCheck();
            //_health.floatValue = EditorGUILayout.FloatField("Health", _health.floatValue);
            _textMesh.objectReferenceValue = EditorGUILayout.ObjectField("Text Mesh", _textMesh.objectReferenceValue, typeof(TMPro.TextMeshPro), true);
            if (EditorGUI.EndChangeCheck())
            {
                modified = true;
                /*
                if (_health.floatValue <= 0)
                {
                    _health.floatValue = 1f;
                }
                */
            }
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Adjust Rotation");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("X"))
            {
                _container.model.Rotate(Vector3.right, 90, Space.Self);
                _container.pivot.localEulerAngles = Vector3.zero;
            }
            if (GUILayout.Button("Y"))
            {
                _container.model.Rotate(Vector3.up, 90, Space.Self);
                _container.pivot.localEulerAngles = Vector3.zero;
            }
            if (GUILayout.Button("Z"))
            {
                _container.model.Rotate(Vector3.forward, 90, Space.Self);
                _container.pivot.localEulerAngles = Vector3.zero;
            }
            EditorGUILayout.EndHorizontal();
            /*
            Vector3 rotation = _container.transform.GetChild(0).localRotation.eulerAngles;
            Dictionary<int, int> angles = new Dictionary<int, int>{ { -90, -1 }, { 0, 0 } , { 90, 1 }, { 180, 2 } };
            int rx = 0;
            int ry = 0;
            int rz = 0;
            angles.TryGetValue((int)rotation.x, out rx);
            angles.TryGetValue((int)rotation.y, out ry);
            angles.TryGetValue((int)rotation.z, out rz);
            EditorGUI.BeginChangeCheck();
            rx = EditorGUILayout.IntSlider("X", rx, -1, 2);
            ry = EditorGUILayout.IntSlider("Y", ry, -1, 2);
            rz = EditorGUILayout.IntSlider("Z", rz, -1, 2);
            if (EditorGUI.EndChangeCheck())
            {
                rotation.x = angles.FirstOrDefault(x => x.Value == rx).Key;
                rotation.y = angles.FirstOrDefault(x => x.Value == ry).Key;
                rotation.z = angles.FirstOrDefault(x => x.Value == rz).Key;
                _container.transform.GetChild(0).localRotation = Quaternion.Euler(rotation);
            }
            */
            
            EditorGUILayout.Space();
            if (modified)
            {
                serializedObject.ApplyModifiedProperties();
            }
            
            if (PrefabUtility.GetPrefabAssetType(target) == PrefabAssetType.NotAPrefab && !Application.isPlaying)
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("This container is not a prefab. You can not access it from the library.", MessageType.Warning);
                if (GUILayout.Button("Save As Prefab"))
                {
                    string path = new string[] { "Assets", "Packages", "DevelopersHub", "ShootersRush", "Prefabs" }.Aggregate(Path.Combine);
                    string prefabPath = Path.Combine(path, "Containers");
                    if (!Directory.Exists(prefabPath))
                    {
                        AssetDatabase.CreateFolder(path, "Containers");
                    }
                    prefabPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(prefabPath, _container.gameObject.name + ".prefab"));
                    GameObject prefab = PrefabUtility.SaveAsPrefabAsset(_container.gameObject, prefabPath, out bool saved);
                    if (saved)
                    { 
                        DestroyImmediate(_container.gameObject);
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
        
        public static bool IsInPrefabEditMode()
        {
            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            return prefabStage != null;
        }
        
    }
    
    public class EditorShootersRushContainerWindow : EditorWindow
    {
        
        private TextField idField = null;
        private ObjectField modelField = null;
        
        [MenuItem("Tools/Developers Hub/Shooters Rush/Create Container")]
        public static void Open()
        {
            EditorShootersRushContainerWindow window = GetWindow<EditorShootersRushContainerWindow>();
            window.titleContent = new GUIContent("Container Setup");
            window.minSize = new Vector2(300, 200);
            window.maxSize = new Vector2(300, 200);
        }

        private void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            root.Clear();

            idField = new TextField();
            idField.label = "ID";
            root.Add(idField);
            
            modelField = new ObjectField();
            modelField.name = "model";
            modelField.label = "Model";
            modelField.allowSceneObjects = false;
            modelField.objectType = typeof(GameObject);
            root.Add(modelField);
            
            UnityEngine.UIElements.Button button = new UnityEngine.UIElements.Button();
            button.name = "button";
            button.text = "Create";
            button.clicked += OnClick;
            root.Add(button);
        }

        private void OnClick()
        {
            if (modelField.value != null)
            {
                GameObject model = (GameObject)modelField.value;
                string id = idField.value.Trim();
                if (string.IsNullOrEmpty(id)) { id = model.name; }
                EditorShootersRushTools.AddTag("Container");
                GameObject containerObject = new GameObject(id);
                containerObject.tag = "Container";
                Transform pivot = new GameObject("Pivot").transform;
                pivot.SetParent(containerObject.transform);
                pivot.transform.localPosition = Vector3.zero;
                pivot.transform.localEulerAngles = Vector3.zero;
                GameObject modelObject = Instantiate(model, pivot);
                modelObject.transform.localPosition = Vector3.zero;
                modelObject.transform.localEulerAngles = Vector3.zero;
                ShootersRushContainer container = containerObject.AddComponent<ShootersRushContainer>();
                CapsuleCollider collider = containerObject.AddComponent<CapsuleCollider>();
                GameObject textObject = new GameObject("Text");
                textObject.transform.SetParent(containerObject.transform);
                textObject.transform.localPosition = Vector3.zero;
                textObject.transform.localEulerAngles = new Vector3(0, 180, 0);
                TextMeshPro text = textObject.AddComponent<TextMeshPro>();
                text.text = "+99";
                text.fontSize = 15;
                collider.height = 3.5f;
                collider.radius = 1f;
                collider.direction = 0;
                container._EditorInitialize(id, Guid.NewGuid().ToString(), pivot, modelObject.transform, text);
                UnityEditor.Selection.activeObject = containerObject;
                Close();
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Model can not be empty.", "OK");
            }
        }
        
    }
}