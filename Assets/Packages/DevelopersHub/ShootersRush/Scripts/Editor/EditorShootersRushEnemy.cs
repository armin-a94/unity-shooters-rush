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
    
    [CustomEditor(typeof(ShootersRushEnemy))] public class EditorShootersRushEnemy : Editor
    {

        private ShootersRushEnemy _enemy = null;
        private SerializedProperty _id = null;
        private SerializedProperty _prefabId = null;
        private SerializedProperty _health = null;
        private SerializedProperty _radius = null;
        private SerializedProperty _damage = null;
        private SerializedProperty _damageApplyTime = null;
        private bool _duplicateID = false;
        
        private void OnEnable()
        {
            if (IsInPrefabEditMode())
            {
                return;
            }
            _enemy = (ShootersRushEnemy)target;
            _id = serializedObject.FindProperty("_id");
            _prefabId = serializedObject.FindProperty("_prefabId");
            _health = serializedObject.FindProperty("_health");
            _radius = serializedObject.FindProperty("_radius");
            _damage = serializedObject.FindProperty("_damage");
            _damageApplyTime = serializedObject.FindProperty("_damageApplyTime");
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
                EditorGUILayout.HelpBox("The ID should not be empty. Please choose an ID for this enemy.", MessageType.Error);
                EditorGUILayout.Space();
            }
            else if (_duplicateID)
            {
                EditorGUILayout.HelpBox("This ID is also used by other enemies. Please choose another ID to avoid any issues in your project.", MessageType.Error);
                EditorGUILayout.Space();
            }
            
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("Prefab ID", _prefabId.stringValue);
            EditorGUI.EndDisabledGroup();
            
            EditorGUI.BeginChangeCheck();
            _health.floatValue = EditorGUILayout.FloatField("Health", _health.floatValue);
            _radius.floatValue = EditorGUILayout.FloatField("Radius", _radius.floatValue);
            _damage.floatValue = EditorGUILayout.FloatField("Damage", _damage.floatValue);
            _damageApplyTime.floatValue = EditorGUILayout.Slider("Damage Apply Time", _damageApplyTime.floatValue, 0f, 1f);
            if (EditorGUI.EndChangeCheck())
            {
                modified = true;
                if (_health.floatValue <= 0)
                {
                    _health.floatValue = 1f;
                }
                if (_radius.floatValue < 0.1)
                {
                    _radius.floatValue = 0.1f;
                }
                if (_damage.floatValue <= 0)
                {
                    _damage.floatValue = 1f;
                }
            }
            
            EditorGUILayout.Space();
            if (modified)
            {
                serializedObject.ApplyModifiedProperties();
            }
            
            if (PrefabUtility.GetPrefabAssetType(target) == PrefabAssetType.NotAPrefab && !Application.isPlaying)
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("This enemy is not a prefab. You can not access it from the library.", MessageType.Warning);
                if (GUILayout.Button("Save As Prefab"))
                {
                    string path = new string[] { "Assets", "Packages", "DevelopersHub", "ShootersRush", "Prefabs" }.Aggregate(Path.Combine);
                    string prefabPath = Path.Combine(path, "Enemies");
                    if (!Directory.Exists(prefabPath))
                    {
                        AssetDatabase.CreateFolder(path, "Enemies");
                    }
                    prefabPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(prefabPath, _enemy.gameObject.name + ".prefab"));
                    GameObject prefab = PrefabUtility.SaveAsPrefabAsset(_enemy.gameObject, prefabPath, out bool saved);
                    if (saved)
                    { 
                        DestroyImmediate(_enemy.gameObject);
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
    
    public class EditorShootersRushEnemyWindow : EditorWindow
    {
        
        private TextField idField = null;
        private ObjectField animatorField = null;
        private ObjectField modelField = null;
        
        [MenuItem("Tools/Developers Hub/Shooters Rush/Create Enemy")]
        public static void Open()
        {
            EditorShootersRushEnemyWindow window = GetWindow<EditorShootersRushEnemyWindow>();
            window.titleContent = new GUIContent("Enemy Setup");
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

            animatorField = new ObjectField();
            animatorField.name = "animator";
            animatorField.label = "Animator";
            animatorField.allowSceneObjects = false;
            animatorField.objectType = typeof(RuntimeAnimatorController);
            root.Add(animatorField);
            
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
                
                EditorShootersRushTools.AddTag("Enemy");
                GameObject enemyObject = new GameObject(id);
                enemyObject.tag = "Enemy";
                GameObject modelObject = Instantiate(model, enemyObject.transform);
                modelObject.transform.localPosition = Vector3.zero;
                modelObject.transform.localEulerAngles = Vector3.zero;
                
                ShootersRushEnemy enemy = enemyObject.AddComponent<ShootersRushEnemy>();
                
                CapsuleCollider collider = enemyObject.AddComponent<CapsuleCollider>();
                collider.radius = 0.4f;
                collider.height = 2f;
                collider.center = new Vector3(0, 1f, 0);

                Animator animator = enemyObject.GetComponentInChildren<Animator>();
                Transform weaponHolderParent = null;
                
                if (animator != null)
                {
                    if (animatorField.value != null)
                    {
                        animator.runtimeAnimatorController = (RuntimeAnimatorController)animatorField.value;
                        animator.applyRootMotion = false;
                    }
                    weaponHolderParent = animator.GetBoneTransform(HumanBodyBones.RightHand);
                }

                if (weaponHolderParent == null)
                {
                    weaponHolderParent = enemy.transform;
                }
                
                Transform weaponHolder = new GameObject("WeaponsHolder").transform;
                weaponHolder.SetParent(weaponHolderParent);
                weaponHolder.localPosition = Vector3.zero;
                weaponHolder.localEulerAngles = Vector3.zero;
                
                enemy._EditorInitialize(id, weaponHolder, Guid.NewGuid().ToString());
                
                UnityEditor.Selection.activeObject = enemyObject;
                Close();
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Model can not be empty.", "OK");
            }
        }
        
    }
}