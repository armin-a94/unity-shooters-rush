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
    
    [CustomEditor(typeof(ShootersRushWeapon))] public class EditorShootersRushWeapon : Editor
    {
  
        private ShootersRushWeapon _weapon = null;
        private SerializedProperty _id = null;
        private SerializedProperty _prefabId = null;
        private SerializedProperty _damage = null;
        private SerializedProperty _roundsPerMinute = null;
        private SerializedProperty _muzzle = null;
        private SerializedProperty _muzzleFlashPrefab = null;
        private SerializedProperty _projectilePrefab = null;
        private SerializedProperty _roundsSpeed = null;
        private SerializedProperty _roundsTrailColor = null;
        private SerializedProperty _pivot = null;
        private SerializedProperty _leftHandIK = null;
        private SerializedProperty _rightHandIK = null;
        private SerializedProperty _fireSound = null;
        
        private int _toolbarInt = 0;
        private readonly string[] _toolbarStrings = {"General", "References", "Adjustments" };
        private bool _duplicateID = false;
        
        private void OnEnable()
        {
            if (IsInPrefabEditMode())
            {
                return;
            }
            _weapon = (ShootersRushWeapon)target;
            _id = serializedObject.FindProperty("_id");
            _prefabId = serializedObject.FindProperty("_prefabId");
            _damage = serializedObject.FindProperty("_damage");
            _roundsPerMinute = serializedObject.FindProperty("_roundsPerMinute");
            _muzzle = serializedObject.FindProperty("_muzzle");
            _muzzleFlashPrefab = serializedObject.FindProperty("_muzzleFlashPrefab");
            _projectilePrefab = serializedObject.FindProperty("_projectilePrefab");
            _roundsSpeed = serializedObject.FindProperty("_roundsSpeed");
            _roundsTrailColor = serializedObject.FindProperty("_roundsTrailColor");
            _pivot = serializedObject.FindProperty("_pivot");
            _leftHandIK = serializedObject.FindProperty("_leftHandIK");
            _rightHandIK = serializedObject.FindProperty("_rightHandIK");
            _fireSound = serializedObject.FindProperty("_fireSound");
            CheckID();
            _weapon.editorSelected = true;
        }

        private void OnDisable()
        {
            _weapon.editorSelected = false;
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
            _toolbarInt = GUILayout.Toolbar(_toolbarInt, _toolbarStrings);
            EditorGUILayout.Space();
            switch (_toolbarInt)
            {
                case 0:
                    EditorGUI.BeginChangeCheck();
                    _id.stringValue = EditorGUILayout.TextField("ID", _id.stringValue);
                    if (EditorGUI.EndChangeCheck())
                    {
                        _duplicateID = ShootersRushLibrary.GetWeaponPrefabsCount(_id.stringValue) > 1;
                        modified = true;
                    }
                    if (string.IsNullOrEmpty(_id.stringValue))
                    {
                        EditorGUILayout.HelpBox("The ID should not be empty. Please choose an ID for this weapon.", MessageType.Error);
                        EditorGUILayout.Space();
                    }
                    else if (_duplicateID)
                    {
                        EditorGUILayout.HelpBox("This ID is also used by other weapons. Please choose another ID to avoid any issues in your project.", MessageType.Error);
                        EditorGUILayout.Space();
                    }
                    
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.TextField("Prefab ID", _prefabId.stringValue);
                    EditorGUI.EndDisabledGroup();
                    
                    EditorGUI.BeginChangeCheck();
                    _damage.floatValue = EditorGUILayout.FloatField("Damage", _damage.floatValue);
                    _roundsPerMinute.intValue = EditorGUILayout.IntField("Rounds Per Minute", _roundsPerMinute.intValue);
                    _roundsSpeed.floatValue = EditorGUILayout.FloatField("Rounds Speed", _roundsSpeed.floatValue);
                    _roundsTrailColor.colorValue = EditorGUILayout.ColorField("Rounds Trail Color", _roundsTrailColor.colorValue);
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (_damage.floatValue <= 0)
                        {
                            _damage.floatValue = 1f;
                        }
                        if (_roundsSpeed.floatValue <= 0)
                        {
                            _roundsSpeed.floatValue = 1f;
                        }
                        modified = true;
                    }
                    if (PrefabUtility.GetPrefabAssetType(target) == PrefabAssetType.NotAPrefab && !Application.isPlaying)
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.HelpBox("This weapon is not a prefab. You can not access it from the library.", MessageType.Warning);
                        if (GUILayout.Button("Save As Prefab"))
                        {
                            string path = new string[] { "Assets", "Packages", "DevelopersHub", "ShootersRush", "Prefabs" }.Aggregate(Path.Combine);
                            string prefabPath = Path.Combine(path, "Weapons");
                            if (!Directory.Exists(prefabPath))
                            {
                                AssetDatabase.CreateFolder(path, "Weapons");
                            }
                            prefabPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(prefabPath, _weapon.gameObject.name + ".prefab"));
                            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(_weapon.gameObject, prefabPath, out bool saved);
                            if (saved)
                            { 
                                DestroyImmediate(_weapon.gameObject);
                                UnityEditor.Selection.activeObject = prefab;
                                return;
                            }
                        }
                    }
                    break;
                case 1:
                    EditorGUI.BeginChangeCheck();
                    
                    _muzzle.objectReferenceValue = EditorGUILayout.ObjectField("Muzzle", _muzzle.objectReferenceValue, typeof(Transform), true);
                    _muzzleFlashPrefab.objectReferenceValue = EditorGUILayout.ObjectField("Muzzle Flash Prefab", _muzzleFlashPrefab.objectReferenceValue, typeof(ShootersRushMuzzleFlash), false);
                    _projectilePrefab.objectReferenceValue = EditorGUILayout.ObjectField("Projectile Prefab", _projectilePrefab.objectReferenceValue, typeof(ShootersRushProjectile), false);
                    _pivot.objectReferenceValue = EditorGUILayout.ObjectField("Pivot", _pivot.objectReferenceValue, typeof(Transform), true);
                    _leftHandIK.objectReferenceValue = EditorGUILayout.ObjectField("Left Hand IK", _leftHandIK.objectReferenceValue, typeof(Transform), true);
                    _rightHandIK.objectReferenceValue = EditorGUILayout.ObjectField("Right Hand IK", _rightHandIK.objectReferenceValue, typeof(Transform), true);
                    _fireSound.objectReferenceValue = EditorGUILayout.ObjectField("Fire Sound", _fireSound.objectReferenceValue, typeof(AudioClip), false);
                    
                    if (EditorGUI.EndChangeCheck())
                    {
                        modified = true;
                    }
                    break;
                case 2:
                    if (_weapon.muzzle != null)
                    {
                        EditorGUILayout.LabelField("Muzzle");
                        Vector3 position = _weapon.muzzle.localPosition;
                        EditorGUI.BeginChangeCheck();
                        position.x = EditorGUILayout.Slider("X", position.x, -5, 5);
                        position.y = EditorGUILayout.Slider("Y", position.y, -5, 5);
                        position.z = EditorGUILayout.Slider("Z", position.z, -5, 5);
                        EditorGUILayout.Space();
                        if (EditorGUI.EndChangeCheck())
                        {
                            _weapon.muzzle.localPosition = position;
                            modified = true;
                        }
                    }
                    if (_weapon.pivot != null)
                    {
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.LabelField("Rotation");
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("X"))
                        {
                            _weapon.pivot.Rotate(Vector3.right, 90, Space.Self);
                        }
                        if (GUILayout.Button("Y"))
                        {
                            _weapon.pivot.Rotate(Vector3.up, 90, Space.Self);
                        }
                        if (GUILayout.Button("Z"))
                        {
                            _weapon.pivot.Rotate(Vector3.forward, 90, Space.Self);
                        }
                        EditorGUILayout.EndHorizontal();
                        if (EditorGUI.EndChangeCheck())
                        {
                            modified = true;
                        }
                    }
                    break;
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
    
    public class EditorShootersRushWeaponWindow : EditorWindow
    {
        
        private TextField idField = null;
        private ObjectField modelField = null;
        
        [MenuItem("Tools/Developers Hub/Shooters Rush/Create Weapon")]
        public static void Open()
        {
            EditorShootersRushWeaponWindow window = GetWindow<EditorShootersRushWeaponWindow>();
            window.titleContent = new GUIContent("Weapon Setup");
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
                
                GameObject weaponObject = new GameObject(id);
                Transform pivot = new GameObject("Pivot").transform;
                pivot.SetParent(weaponObject.transform);
                pivot.localPosition = Vector3.zero;
                pivot.localEulerAngles = Vector3.zero;
                
                GameObject modelObject = Instantiate(model, pivot);
                modelObject.transform.localPosition = Vector3.zero;
                modelObject.transform.localEulerAngles = Vector3.zero;
                modelObject.transform.localScale = Vector3.one;
                
                Transform muzzle = new GameObject("Muzzle").transform;
                muzzle.SetParent(pivot);
                muzzle.localPosition = Vector3.zero;
                muzzle.localEulerAngles = Vector3.zero;
                
                Transform rhik = new GameObject("RightHandIK").transform;
                rhik.SetParent(pivot);
                rhik.localPosition = Vector3.zero;
                rhik.localEulerAngles = Vector3.zero;
                
                Transform lhik = new GameObject("LeftHandIK").transform;
                lhik.SetParent(pivot);
                lhik.localPosition = Vector3.zero;
                lhik.localEulerAngles = Vector3.zero;
                
                ShootersRushWeapon weapon = weaponObject.AddComponent<ShootersRushWeapon>();
                
                weapon._EditorInitialize(id, pivot, muzzle, rhik, lhik, Guid.NewGuid().ToString());
                
                UnityEditor.Selection.activeObject = weaponObject;
                Close();
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Model can not be empty.", "OK");
            }
        }
        
    }
}