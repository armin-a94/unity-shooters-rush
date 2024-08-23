namespace DevelopersHub.ShootersRush.Editor
{
    using UnityEngine;
    using System;
    using UnityEditor;
    using UnityEditor.UIElements;
    using UnityEngine.UI;
    using UnityEngine.UIElements;
    using UnityEditor.SceneManagement;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    
    [CustomEditor(typeof(ShootersRushCharacter))] public class EditorShootersRushCharacter : Editor
    {

        private ShootersRushCharacter _character = null;
        private SerializedProperty _id = null;
        private SerializedProperty _health = null;
        private SerializedProperty _radius = null;
        private SerializedProperty _weaponsSettings = null;
        private SerializedProperty _prefabId = null;
        private SerializedProperty _weaponsHolder = null;
        private Animator _animator = null;
        private ShootersRushEditorAnimation _animation = null;
        private ShootersRushWeapon _weapon = null;
        private ShootersRushWeapon _weaponPrefab = null;
        private ShootersRushWeapon.Settings _weaponSettings = null;
        private ShootersRushCharacter _characterPrefab = null;
        private int _weaponIndex = 0;
        private int _weaponPrefabsCount = 0;
        private Transform _rightHand = null;
        private Transform _leftHand = null;
        private int _toolbarInt = 0;
        private readonly string[] _toolbarStrings = {"General", "Weapon"};
        private bool _duplicateID = false;
        private readonly float _rightHandPositionRange = 0.25f;
        private readonly float _leftHandPositionRange = 1f;
        private readonly float _weaponPositionRange = 2f;
        private bool _characterPrefabModified = false;
        
        private void OnEnable()
        {
            if (IsInPrefabEditMode())
            {
                _character = (ShootersRushCharacter)target;
                ShootersRushEditorAnimation[] animations = _character.gameObject.GetComponentsInChildren<ShootersRushEditorAnimation>();
                if (animations != null)
                {
                    for (int i = 0; i < animations.Length; i++)
                    {
                        DestroyImmediate(animations[i]);
                    }
                }
                return;
            }
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            _character = (ShootersRushCharacter)target;
            _id = serializedObject.FindProperty("_id");
            _health = serializedObject.FindProperty("_health");
            _radius = serializedObject.FindProperty("_radius");
            _weaponsSettings = serializedObject.FindProperty("_weaponSettings");
            _prefabId = serializedObject.FindProperty("_prefabId");
            _weaponsHolder = serializedObject.FindProperty("_weaponsHolder");
            CheckID();
            
            _characterPrefab = PrefabUtility.GetCorrespondingObjectFromSource(_character);
            _duplicateID = ShootersRushLibrary.GetCharacterPrefabsCount(_id.stringValue) > 1;
            if (!Application.isPlaying && Application.isEditor && _character.gameObject.scene.IsValid() && _character.gameObject.scene.name.Equals(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name))
            {
                _character.Clear();
                
                _animator = _character.GetComponentInChildren<Animator>();
                if (_animator != null)
                {
                    _animation =  _animator.gameObject.GetComponent<ShootersRushEditorAnimation>();
                    if (_animation == null)
                    {
                        _animation = _animator.gameObject.AddComponent<ShootersRushEditorAnimation>();
                    }
                    _rightHand = _animator.GetBoneTransform(HumanBodyBones.RightHand);
                    _leftHand = _animator.GetBoneTransform(HumanBodyBones.LeftHand);
                }
                
                _weaponIndex = 0;
                _weaponPrefab = ShootersRushLibrary.GetWeaponPrefab(_weaponIndex);
                if (_weaponPrefab != null)
                {
                    bool haveSettings = _characterPrefab != null && _characterPrefab.HaveWeaponSettings(_weaponPrefab.prefabID);
                    if (_characterPrefab != null)
                    {
                        _weaponSettings = _characterPrefab.GetWeaponSettings(_weaponPrefab.prefabID);
                    }
                    else
                    {
                        ShootersRushCharacter any = ShootersRushLibrary.GetCharacterPrefab(0);
                        if (any != null && !_character.HaveWeaponSettings(_weaponPrefab.prefabID))
                        {
                            _weaponSettings = any.GetWeaponSettings(_weaponPrefab.prefabID);
                            _character.AddWeaponSettings(_weaponSettings);
                        }
                        else
                        {
                            _weaponSettings = _character.GetWeaponSettings(_weaponPrefab.prefabID);
                        }
                    }
                    if (!haveSettings && _characterPrefab != null)
                    {
                        PrefabUtility.SavePrefabAsset(_characterPrefab.gameObject);
                    }
                    _weapon = _character.EquipWeapon(_weaponPrefab);
                    _animation.Initialize(_weaponSettings, _character.transform, _weapon);
                }
                _weaponPrefabsCount = ShootersRushLibrary.weaponPrefabsCount;
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

        private void OnDisable()
        {
            if (IsInPrefabEditMode())
            {
                return;
            }
            if (!Application.isPlaying && Application.isEditor)
            {
                if (_characterPrefabModified)
                {
                    PrefabUtility.SavePrefabAsset(_characterPrefab.gameObject);
                }
                if (_weapon != null)
                {
                    _character.RemoveWeapon();
                }
                if (_animation != null)
                {
                    DestroyImmediate(_animation);
                }
            }
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }
        
        private void OnPlayModeStateChanged(PlayModeStateChange state) 
        {
            if (IsInPrefabEditMode())
            {
                return;
            }
            if (state == PlayModeStateChange.EnteredPlayMode) 
            {
                if (_characterPrefabModified)
                {
                      PrefabUtility.SavePrefabAsset(_characterPrefab.gameObject);
                }
                if (_weapon != null)
                {
                    _character.RemoveWeapon();
                }
                if (_animation != null)
                {
                    DestroyImmediate(_animation);
                }
            }
        }
        
        private void ChangeWeapon(int direction)
        {
            _weaponPrefabsCount = ShootersRushLibrary.weaponPrefabsCount;
            int index = _weaponIndex + direction;
            if (index < 0) { index = _weaponPrefabsCount - 1; }
            else if (index >= _weaponPrefabsCount) { index = 0; }
            if (index != _weaponIndex)
            {
                _weaponIndex = index;
                _weaponPrefab = ShootersRushLibrary.GetWeaponPrefab(_weaponIndex);
                if (_weaponPrefab != null)
                {
                    bool haveSettings = _characterPrefab.HaveWeaponSettings(_weaponPrefab.prefabID);
                    _weaponSettings = _characterPrefab.GetWeaponSettings(_weaponPrefab.prefabID);
                    if (!haveSettings)
                    {
                        PrefabUtility.SavePrefabAsset(_characterPrefab.gameObject);
                    }
                    _weapon = _character.EquipWeapon(_weaponPrefab);
                    _animation.Initialize(_weaponSettings, _character.transform, _weapon);
                }
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
                        _duplicateID = ShootersRushLibrary.GetCharacterPrefabsCount(_id.stringValue) > 1;
                        modified = true;
                    }
                    if (string.IsNullOrEmpty(_id.stringValue))
                    {
                        EditorGUILayout.HelpBox("The ID should not be empty. Please choose an ID for this character.", MessageType.Error);
                        EditorGUILayout.Space();
                    }
                    else if (_duplicateID)
                    {
                        EditorGUILayout.HelpBox("This ID is also used by other characters. Please choose another ID to avoid any issues in your project.", MessageType.Error);
                        EditorGUILayout.Space();
                    }
                    
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.TextField("Prefab ID", _prefabId.stringValue);
                    EditorGUI.EndDisabledGroup();
                    
                    EditorGUI.BeginChangeCheck();
                    _health.floatValue = EditorGUILayout.FloatField("Health", _health.floatValue);
                    _radius.floatValue = EditorGUILayout.FloatField("Radius", _radius.floatValue);
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (_health.floatValue <= 0)
                        {
                            _health.floatValue = 1f;
                        }
                        if (_radius.floatValue < 0.1)
                        {
                            _radius.floatValue = 0.1f;
                        }
                        modified = true;
                    }
                    if (PrefabUtility.GetPrefabAssetType(target) == PrefabAssetType.NotAPrefab && !Application.isPlaying)
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.HelpBox("This character is not a prefab. You can not access it from the library.", MessageType.Warning);
                        if (GUILayout.Button("Save As Prefab"))
                        {
                            string path = new string[] { "Assets", "Packages", "DevelopersHub", "ShootersRush", "Prefabs" }.Aggregate(Path.Combine);
                            string prefabPath = Path.Combine(path, "Characters");
                            if (!Directory.Exists(prefabPath))
                            {
                                AssetDatabase.CreateFolder(path, "Characters");
                            }
                            prefabPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(prefabPath, _character.gameObject.name + ".prefab"));
                            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(_character.gameObject, prefabPath, out bool saved);
                            if (saved)
                            { 
                                // Vector3 position = _character.transform.position;
                                // Quaternion rotation = _character.transform.rotation;
                                DestroyImmediate(_character.gameObject);
                                // GameObject instance = Instantiate(prefab, position, rotation);
                                UnityEditor.Selection.activeObject = prefab;
                                return;
                            }
                        }
                    }
                    break;
                case 1:
                    if (PrefabUtility.GetPrefabAssetType(target) == PrefabAssetType.NotAPrefab && !Application.isPlaying)
                    {
                        EditorGUILayout.HelpBox("This character is not a prefab. You can not access the weapon settings.", MessageType.Warning);
                        break;
                    }
                    EditorGUI.BeginChangeCheck();
                    
                    EditorGUI.BeginDisabledGroup(true);
                    _weaponsHolder.objectReferenceValue = EditorGUILayout.ObjectField("Weapon Holder", _weaponsHolder.objectReferenceValue, typeof(Transform), true);
                    EditorGUI.EndDisabledGroup();
                    
                    if (EditorGUI.EndChangeCheck())
                    {
                        modified = true;
                    }
                    EditorGUILayout.Space();

                    if (_weapon != null)
                    {
                        /*
                        EditorGUI.BeginChangeCheck();
                        _weaponSettings.rightHandRight = (ShootersRushTools.Axis)EditorGUILayout.EnumPopup("Right Hand Right", (ShootersRushTools.Axis)Enum.GetValues(typeof(ShootersRushTools.Axis)).GetValue((int)_weaponSettings.rightHandRight));
                        _weaponSettings.rightHandForward = (ShootersRushTools.Axis)EditorGUILayout.EnumPopup("Right Hand Forward", (ShootersRushTools.Axis)Enum.GetValues(typeof(ShootersRushTools.Axis)).GetValue((int)_weaponSettings.rightHandForward));
                        _weaponSettings.rightHandAngle = EditorGUILayout.Slider("Right Hand Angle", _weaponSettings.rightHandAngle, -180f, 180f);
                        if (EditorGUI.EndChangeCheck())
                        {
                            modified = true;
                            _characterPrefabModified = true;
                            _animation.Initialize(_weaponSettings, _character.transform, _weapon.transform);
                        }
                        */
                        EditorGUILayout.Space();
                        EditorGUILayout.LabelField("Weapon Position");
                        Vector3 weaponPosition = _weaponSettings.weaponPosition;
                        EditorGUI.BeginChangeCheck();
                        weaponPosition.x = EditorGUILayout.Slider("X", weaponPosition.x, -_weaponPositionRange, _weaponPositionRange);
                        weaponPosition.y = EditorGUILayout.Slider("Y", weaponPosition.y, -_weaponPositionRange, _weaponPositionRange);
                        weaponPosition.z = EditorGUILayout.Slider("Z", weaponPosition.z, -_weaponPositionRange, _weaponPositionRange);
                        if (EditorGUI.EndChangeCheck())
                        {
                            _weaponSettings.weaponPosition = weaponPosition;
                            modified = true;
                            _characterPrefabModified = true;
                            _animation.Initialize(_weaponSettings, _character.transform, _weapon);
                            _weapon.ApplySettings(_weaponSettings, _character.transform);
                        }
                        EditorGUILayout.Space();
                        
                        EditorGUILayout.LabelField("Weapon Rotation");
                        Vector3 weaponRotation = _weaponSettings.weaponRotation;
                        Dictionary<int, int> angles = new Dictionary<int, int>{ { -90, -1 }, { 0, 0 } , { 90, 1 }, { 180, 2 } };
                        int rx = 0;
                        int ry = 0;
                        int rz = 0;
                        angles.TryGetValue((int)weaponRotation.x, out rx);
                        angles.TryGetValue((int)weaponRotation.y, out ry);
                        angles.TryGetValue((int)weaponRotation.z, out rz);
                        EditorGUI.BeginChangeCheck();
                        rx = EditorGUILayout.IntSlider("X", rx, -1, 2);
                        ry = EditorGUILayout.IntSlider("Y", ry, -1, 2);
                        rz = EditorGUILayout.IntSlider("Z", rz, -1, 2);
                        /*
                        weaponRotation.x = EditorGUILayout.Slider("X", weaponRotation.x, -179, 179);
                        weaponRotation.y = EditorGUILayout.Slider("Y", weaponRotation.y, -179, 179);
                        weaponRotation.z = EditorGUILayout.Slider("Z", weaponRotation.z, -179, 179);
                        */
                        if (EditorGUI.EndChangeCheck())
                        {
                            weaponRotation.x = angles.FirstOrDefault(x => x.Value == rx).Key;
                            weaponRotation.y = angles.FirstOrDefault(x => x.Value == ry).Key;
                            weaponRotation.z = angles.FirstOrDefault(x => x.Value == rz).Key;
                            _weaponSettings.weaponRotation = weaponRotation;
                            _animation.Initialize(_weaponSettings, _character.transform, _weapon);
                            modified = true;
                            _characterPrefabModified = true;
                            _weapon.ApplySettings(_weaponSettings, _character.transform);
                        }
                        EditorGUILayout.Space();
                        
                        EditorGUILayout.LabelField("Right Hand Position");
                        Vector3 position = _weapon.rightHandIK.localPosition;
                        EditorGUI.BeginChangeCheck();
                        // _positionRange = EditorGUILayout.Slider("S", _positionRange, 0.01f, 10f);
                        position.x = EditorGUILayout.Slider("X", position.x, -_rightHandPositionRange, _rightHandPositionRange);
                        position.y = EditorGUILayout.Slider("Y", position.y, -_rightHandPositionRange, _rightHandPositionRange);
                        position.z = EditorGUILayout.Slider("Z", position.z, -_rightHandPositionRange, _rightHandPositionRange);
                        if (EditorGUI.EndChangeCheck())
                        {
                            _weaponSettings.rightHandPosition = position;
                            _weapon.ApplySettings(_weaponSettings, _character.transform);
                            modified = true;
                            _characterPrefabModified = true;
                        }
                        EditorGUILayout.Space();
                        
                        EditorGUILayout.LabelField("Right Hand Rotation");
                        // Vector3 rotation = TransformUtils.GetInspectorRotation(_weapon.rightHandIK);
                        Vector3 rotation =  _weaponSettings.rightHandRotation;
                        EditorGUI.BeginChangeCheck();
                        rotation.x = EditorGUILayout.Slider("X", rotation.x, -179, 179);
                        rotation.y = EditorGUILayout.Slider("Y", rotation.y, -179, 179);
                        rotation.z = EditorGUILayout.Slider("Z", rotation.z, -179, 179);
                        if (EditorGUI.EndChangeCheck())
                        {
                            _weaponSettings.rightHandRotation = rotation;
                            _weapon.ApplySettings(_weaponSettings, _character.transform);
                            modified = true;
                            _characterPrefabModified = true;
                        }
                        EditorGUILayout.Space();

                        bool useLeftHand = _weaponSettings.useLeftHand;
                        /*
                        EditorGUI.BeginChangeCheck();
                        useLeftHand = EditorGUILayout.Toggle("Use Left Hand", useLeftHand);
                        if (EditorGUI.EndChangeCheck())
                        {
                            _weaponSettings.useLeftHand = useLeftHand;
                            _animation.Initialize(_weaponSettings, _character.transform, _weapon);
                            modified = true;
                            _characterPrefabModified = true;
                        }
                        */
                        if (useLeftHand)
                        {
                            EditorGUILayout.LabelField("Left Hand Position");
                            position = _weapon.leftHandIK.localPosition;
                            EditorGUI.BeginChangeCheck();
                            position.x = EditorGUILayout.Slider("X", position.x, -_leftHandPositionRange, _leftHandPositionRange);
                            position.y = EditorGUILayout.Slider("Y", position.y, -_leftHandPositionRange, _leftHandPositionRange);
                            position.z = EditorGUILayout.Slider("Z", position.z, -_leftHandPositionRange, _leftHandPositionRange);
                            if (EditorGUI.EndChangeCheck())
                            {
                                _weaponSettings.leftHandPosition = position;
                                _animation.Initialize(_weaponSettings, _character.transform, _weapon);
                                _weapon.ApplySettings(_weaponSettings, _character.transform);
                                modified = true;
                                _characterPrefabModified = true;
                            }
                            EditorGUILayout.Space();

                            EditorGUILayout.LabelField("Left Hand Rotation");
                            // rotation = TransformUtils.GetInspectorRotation(_weapon.leftHandIK);
                            rotation =  _weaponSettings.leftHandRotation;
                            EditorGUI.BeginChangeCheck();
                            rotation.x = EditorGUILayout.Slider("X", rotation.x, -179, 179);
                            rotation.y = EditorGUILayout.Slider("Y", rotation.y, -179, 179);
                            rotation.z = EditorGUILayout.Slider("Z", rotation.z, -179, 179);
                            if (EditorGUI.EndChangeCheck())
                            {
                                _weaponSettings.leftHandRotation = rotation;
                                _animation.Initialize(_weaponSettings, _character.transform, _weapon);
                                _weapon.ApplySettings(_weaponSettings, _character.transform);
                                modified = true;
                                _characterPrefabModified = true;
                            }
                        }
                        EditorGUILayout.Space();
                        
                        if (modified)
                        {
                            _character._EditorSetWeaponSettings(_weaponSettings);
                            _characterPrefab._EditorSetWeaponSettings(_weaponSettings);
                        }
                        
                        EditorGUILayout.Space();
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("Prev Weapon"))
                        {
                            ChangeWeapon(1);
                        }
                        GUILayout.FlexibleSpace();
                        GUILayout.Label( (_weaponIndex + 1).ToString()  + "/" + _weaponPrefabsCount.ToString());
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Next Weapon"))
                        {
                            ChangeWeapon(-1);
                        }
                        GUILayout.EndHorizontal();
                        EditorGUILayout.Space();
                    }
                    /*
                    if (Application.isPlaying && _character.weapon != null)
                    {
                        if (GUILayout.Button("Fire"))
                        {
                            ShootersRushProjectile.Fire(_character.weapon.projectilePrefab.id, _character.weapon.muzzle.position, _character.weapon.muzzle.forward, _character.weapon.roundsSpeed, _character.weapon.damage, _character.weapon.roundsTrailColor);
                        }
                        EditorGUILayout.Space();
                    }
                    */
                    EditorGUILayout.Space();
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.PropertyField(_weaponsSettings, true);
                    EditorGUI.EndDisabledGroup();
                    EditorGUILayout.Space();
                    break;
                case 2:
                    GUILayout.Button("Button for Details");
                    break;
            }
            EditorGUILayout.Space();
            if (modified)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
        
        public static bool IsInPrefabEditMode()
        {
            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            return prefabStage != null;
        }
        
    }

    public class EditorShootersRushCharacterWindow : EditorWindow
    {
        
        private TextField idField = null;
        private ObjectField animatorField = null;
        private ObjectField modelField = null;
        
        [MenuItem("Tools/Developers Hub/Shooters Rush/Create Character")]
        public static void Open()
        {
            EditorShootersRushCharacterWindow window = GetWindow<EditorShootersRushCharacterWindow>();
            window.titleContent = new GUIContent("Character Setup");
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
                
                EditorShootersRushTools.AddTag("Character");
                GameObject characterObject = new GameObject(id);
                characterObject.tag = "Character";
                GameObject modelObject = Instantiate(model, characterObject.transform);
                modelObject.transform.localPosition = Vector3.zero;
                modelObject.transform.localEulerAngles = Vector3.zero;
                
                ShootersRushCharacter character = characterObject.AddComponent<ShootersRushCharacter>();
                
                /*
                CapsuleCollider collider = characterObject.AddComponent<CapsuleCollider>();
                collider.radius = 0.4f;
                collider.height = 2f;
                collider.center = new Vector3(0, 1f, 0);
                */
                
                Animator animator = characterObject.GetComponentInChildren<Animator>();
                Transform weaponHolderParent = null;
                
                if (animator != null)
                {
                    if (animatorField.value != null)
                    {
                        animator.runtimeAnimatorController = (RuntimeAnimatorController)animatorField.value;
                        animator.applyRootMotion = false;
                    }
                    weaponHolderParent = ShootersRushCharacter.GetWeaponHolder(animator);
                }

                if (weaponHolderParent == null)
                {
                    weaponHolderParent = character.transform;
                }
                
                Transform weaponHolder = new GameObject("WeaponsHolder").transform;
                weaponHolder.SetParent(weaponHolderParent);
                weaponHolder.localPosition = Vector3.zero;
                weaponHolder.localEulerAngles = Vector3.zero;
                
                character._EditorInitialize(id, weaponHolder, Guid.NewGuid().ToString());
                
                UnityEditor.Selection.activeObject = characterObject;
                Close();
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Model can not be empty.", "OK");
            }
        }
        
    }
    
}