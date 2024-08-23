namespace DevelopersHub.ShootersRush
{
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;
    using System.Linq;

    public class ShootersRushLibrary : ScriptableObject
    {
 
        [SerializeField] private List<ShootersRushCharacter> _characterPrefabs = new List<ShootersRushCharacter>();
        [SerializeField] private List<ShootersRushEnemy> _enemyPrefabs = new List<ShootersRushEnemy>();
        [SerializeField] private List<ShootersRushWeapon> _weaponPrefabs = new List<ShootersRushWeapon>();
        [SerializeField] private List<ShootersRushProjectile> _projectilePrefabs = new List<ShootersRushProjectile>();
        [SerializeField] private List<ShootersRushMuzzleFlash> _muzzleFlashPrefabs = new List<ShootersRushMuzzleFlash>();
        [SerializeField] private List<ShootersRushContainer> _containerPrefabs = new List<ShootersRushContainer>();
        [SerializeField] private List<ShootersRushGateway> _gatewayPrefabs = new List<ShootersRushGateway>();
        
        public static int characterPrefabsCount { get { return Singleton._characterPrefabs.Count; } }
        public static int enemyPrefabsCount { get { return Singleton._enemyPrefabs.Count; } }
        public static int weaponPrefabsCount { get { return Singleton._weaponPrefabs.Count; } }
        public static int projectilePrefabsCount { get { return Singleton._projectilePrefabs.Count; } }
        public static int muzzleFlashPrefabsCount { get { return Singleton._muzzleFlashPrefabs.Count; } }
        public static int containerPrefabsCount { get { return Singleton._containerPrefabs.Count; } }
        public static int gatewayPrefabsCount { get { return Singleton._gatewayPrefabs.Count; } }
        
        public static ShootersRushCharacter GetCharacterPrefab(int index)
        {
            if(Singleton != null && index >= 0 && index < _singleton._characterPrefabs.Count)
            {
                return _singleton._characterPrefabs[index];
            }
            return null;
        }
        
        public static ShootersRushCharacter GetCharacterPrefab(string id)
        {
            if(Singleton != null)
            {
                for (int i = 0; i < _singleton._characterPrefabs.Count; i++)
                {
                    if(_singleton._characterPrefabs[i] != null && _singleton._characterPrefabs[i].id == id)
                    {
                        return _singleton._characterPrefabs[i];
                    }
                }
            }
            return null;
        }
        
        public static int GetCharacterPrefabsCount(string id)
        {
            int count = 0;
            if(Singleton != null)
            {
                for (int i = 0; i < _singleton._characterPrefabs.Count; i++)
                {
                    if(_singleton._characterPrefabs[i] != null && _singleton._characterPrefabs[i].id == id)
                    {
                        count++;
                    }
                }
            }
            return count;
        }
        
        public static ShootersRushGateway GetGatewayPrefab(int index)
        {
            if(Singleton != null && index >= 0 && index < _singleton._gatewayPrefabs.Count)
            {
                return _singleton._gatewayPrefabs[index];
            }
            return null;
        }
        
        public static ShootersRushGateway GetGatewayPrefab(string id)
        {
            if(Singleton != null)
            {
                for (int i = 0; i < _singleton._gatewayPrefabs.Count; i++)
                {
                    if(_singleton._gatewayPrefabs[i] != null && _singleton._gatewayPrefabs[i].id == id)
                    {
                        return _singleton._gatewayPrefabs[i];
                    }
                }
            }
            return null;
        }
        
        public static int GetGatewayPrefabsCount(string id)
        {
            int count = 0;
            if(Singleton != null)
            {
                for (int i = 0; i < _singleton._gatewayPrefabs.Count; i++)
                {
                    if(_singleton._gatewayPrefabs[i] != null && _singleton._gatewayPrefabs[i].id == id)
                    {
                        count++;
                    }
                }
            }
            return count;
        }
        
        public static ShootersRushContainer GetContainerPrefab(int index)
        {
            if(Singleton != null && index >= 0 && index < _singleton._containerPrefabs.Count)
            {
                return _singleton._containerPrefabs[index];
            }
            return null;
        }
        
        public static ShootersRushContainer GetContainerPrefab(string id)
        {
            if(Singleton != null)
            {
                for (int i = 0; i < _singleton._containerPrefabs.Count; i++)
                {
                    if(_singleton._containerPrefabs[i] != null && _singleton._containerPrefabs[i].id == id)
                    {
                        return _singleton._containerPrefabs[i];
                    }
                }
            }
            return null;
        }
        
        public static int GetContainerPrefabsCount(string id)
        {
            int count = 0;
            if(Singleton != null)
            {
                for (int i = 0; i < _singleton._containerPrefabs.Count; i++)
                {
                    if(_singleton._containerPrefabs[i] != null && _singleton._containerPrefabs[i].id == id)
                    {
                        count++;
                    }
                }
            }
            return count;
        }
        
        public static ShootersRushEnemy GetEnemyPrefab(int index)
        {
            if(Singleton != null && index >= 0 && index < _singleton._enemyPrefabs.Count)
            {
                return _singleton._enemyPrefabs[index];
            }
            return null;
        }
        
        public static ShootersRushEnemy GetEnemyPrefab(string id)
        {
            if(Singleton != null)
            {
                for (int i = 0; i < _singleton._enemyPrefabs.Count; i++)
                {
                    if(_singleton._enemyPrefabs[i] != null && _singleton._enemyPrefabs[i].id == id)
                    {
                        return _singleton._enemyPrefabs[i];
                    }
                }
            }
            return null;
        }
        
        public static ShootersRushWeapon GetWeaponPrefab(int index)
        {
            if(Singleton != null && index >= 0 && index < _singleton._weaponPrefabs.Count)
            {
                return _singleton._weaponPrefabs[index];
            }
            return null;
        }
        
        public static ShootersRushWeapon GetWeaponPrefab(string id)
        {
            if(Singleton != null)
            {
                for (int i = 0; i < _singleton._weaponPrefabs.Count; i++)
                {
                    if(_singleton._weaponPrefabs[i] != null && _singleton._weaponPrefabs[i].id == id)
                    {
                        return _singleton._weaponPrefabs[i];
                    }
                }
            }
            return null;
        }
        
        public static int GetWeaponPrefabsCount(string id)
        {
            int count = 0;
            if(Singleton != null)
            {
                for (int i = 0; i < _singleton._weaponPrefabs.Count; i++)
                {
                    if(_singleton._weaponPrefabs[i] != null && _singleton._weaponPrefabs[i].id == id)
                    {
                        count++;
                    }
                }
            }
            return count;
        }
        
        public static ShootersRushProjectile GetProjectilePrefab(int index)
        {
            if(Singleton != null && index >= 0 && index < _singleton._projectilePrefabs.Count)
            {
                return _singleton._projectilePrefabs[index];
            }
            return null;
        }
        
        public static ShootersRushProjectile GetProjectilePrefab(string id)
        {
            if(Singleton != null)
            {
                for (int i = 0; i < _singleton._projectilePrefabs.Count; i++)
                {
                    if(_singleton._projectilePrefabs[i] != null && _singleton._projectilePrefabs[i].id == id)
                    {
                        return _singleton._projectilePrefabs[i];
                    }
                }
            }
            return null;
        }
        
        public static int GetProjectilePrefabsCount(string id)
        {
            int count = 0;
            if(Singleton != null)
            {
                for (int i = 0; i < _singleton._projectilePrefabs.Count; i++)
                {
                    if(_singleton._projectilePrefabs[i] != null && _singleton._projectilePrefabs[i].id == id)
                    {
                        count++;
                    }
                }
            }
            return count;
        }
        
        public static ShootersRushMuzzleFlash GetMuzzleFlashPrefab(int index)
        {
            if(Singleton != null && index >= 0 && index < _singleton._muzzleFlashPrefabs.Count)
            {
                return _singleton._muzzleFlashPrefabs[index];
            }
            return null;
        }
        
        public static ShootersRushMuzzleFlash GetMuzzleFlashPrefab(string id)
        {
            if(Singleton != null)
            {
                for (int i = 0; i < _singleton._muzzleFlashPrefabs.Count; i++)
                {
                    if(_singleton._muzzleFlashPrefabs[i] != null && _singleton._muzzleFlashPrefabs[i].id == id)
                    {
                        return _singleton._muzzleFlashPrefabs[i];
                    }
                }
            }
            return null;
        }
        
        public static int GetMuzzleFlashPrefabsCount(string id)
        {
            int count = 0;
            if(Singleton != null)
            {
                for (int i = 0; i < _singleton._muzzleFlashPrefabs.Count; i++)
                {
                    if(_singleton._muzzleFlashPrefabs[i] != null && _singleton._muzzleFlashPrefabs[i].id == id)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        private static ShootersRushLibrary _singleton = null; public static ShootersRushLibrary Singleton
        {
            get
            {
                if (_singleton == null)
                {
                    _singleton = Load();
                    #if UNITY_EDITOR
                    if (_singleton == null)
                    {
                        _singleton = CreatePrefabLibrary();
                    }
                    #endif
                }
                return _singleton;
            }
        }

        private static ShootersRushLibrary Load()
        {
            ShootersRushLibrary[] libraries = Resources.LoadAll<ShootersRushLibrary>("");
            if(libraries != null && libraries.Length > 0)
            {
                return libraries[0];
            }
            return null;
        }
        
        public bool AddCharacterPrefab(ShootersRushCharacter prefab)
        {
            if(prefab != null)
            {
                int index = -1;
                for (int i = 0; i < _characterPrefabs.Count; i++)
                {
                    if(_characterPrefabs[i] != null) { if(_characterPrefabs[i].id == prefab.id) { return false; } }
                    else if(index < 0) { index = i; }
                }
                AddCharacterPrefabInternal(prefab, index); return true;
            }
            return false;
        }
        
        private void AddCharacterPrefabInternal(ShootersRushCharacter prefab, int index = -1)
        {
            if(index >= 0) { _characterPrefabs[index] = prefab; } else { _characterPrefabs.Add(prefab); }
        }
        
        public bool AddEnemyPrefab(ShootersRushEnemy prefab)
        {
            if(prefab != null)
            {
                int index = -1;
                for (int i = 0; i < _enemyPrefabs.Count; i++)
                {
                    if(_enemyPrefabs[i] != null) { if(_enemyPrefabs[i].id == prefab.id) { return false; } }
                    else if(index < 0) { index = i; }
                }
                AddEnemyPrefabInternal(prefab, index); return true;
            }
            return false;
        }
        
        private void AddEnemyPrefabInternal(ShootersRushEnemy prefab, int index = -1)
        {
            if(index >= 0) { _enemyPrefabs[index] = prefab; } else { _enemyPrefabs.Add(prefab); }
        }
        
        public bool AddContainerPrefab(ShootersRushContainer prefab)
        {
            if(prefab != null)
            {
                int index = -1;
                for (int i = 0; i < _containerPrefabs.Count; i++)
                {
                    if(_containerPrefabs[i] != null) { if(_containerPrefabs[i].id == prefab.id) { return false; } }
                    else if(index < 0) { index = i; }
                }
                AddContainerPrefabInternal(prefab, index); return true;
            }
            return false;
        }
        
        private void AddContainerPrefabInternal(ShootersRushContainer prefab, int index = -1)
        {
            if(index >= 0) { _containerPrefabs[index] = prefab; } else { _containerPrefabs.Add(prefab); }
        }
        
        public bool AddGatewayPrefab(ShootersRushGateway prefab)
        {
            if(prefab != null)
            {
                int index = -1;
                for (int i = 0; i < _gatewayPrefabs.Count; i++)
                {
                    if(_gatewayPrefabs[i] != null) { if(_gatewayPrefabs[i].id == prefab.id) { return false; } }
                    else if(index < 0) { index = i; }
                }
                AddGatewayPrefabInternal(prefab, index); return true;
            }
            return false;
        }
        
        private void AddGatewayPrefabInternal(ShootersRushGateway prefab, int index = -1)
        {
            if(index >= 0) { _gatewayPrefabs[index] = prefab; } else { _gatewayPrefabs.Add(prefab); }
        }
        
        public bool AddWeaponPrefab(ShootersRushWeapon prefab)
        {
            if(prefab != null)
            {
                int index = -1;
                for (int i = 0; i < _weaponPrefabs.Count; i++)
                {
                    if(_weaponPrefabs[i] != null) { if(_weaponPrefabs[i].id == prefab.id) { return false; } }
                    else if(index < 0) { index = i; }
                }
                AddWeaponPrefabInternal(prefab, index); return true;
            }
            return false;
        }
        
        private void AddWeaponPrefabInternal(ShootersRushWeapon prefab, int index = -1)
        {
            if(index >= 0) { _weaponPrefabs[index] = prefab; } else { _weaponPrefabs.Add(prefab); }
        }
        
        public bool AddMuzzleFlashPrefab(ShootersRushMuzzleFlash prefab)
        {
            if(prefab != null)
            {
                int index = -1;
                for (int i = 0; i < _muzzleFlashPrefabs.Count; i++)
                {
                    if(_muzzleFlashPrefabs[i] != null) { if(_muzzleFlashPrefabs[i].id == prefab.id) { return false; } }
                    else if(index < 0) { index = i; }
                }
                AddMuzzleFlashInternal(prefab, index); return true;
            }
            return false;
        }
        
        private void AddMuzzleFlashInternal(ShootersRushMuzzleFlash prefab, int index = -1)
        {
            if(index >= 0) { _muzzleFlashPrefabs[index] = prefab; } else { _muzzleFlashPrefabs.Add(prefab); }
        }
        
        public bool AddProjectilePrefab(ShootersRushProjectile prefab)
        {
            if(prefab != null)
            {
                int index = -1;
                for (int i = 0; i < _projectilePrefabs.Count; i++)
                {
                    if(_projectilePrefabs[i] != null) { if(_projectilePrefabs[i].id == prefab.id) { return false; } }
                    else if(index < 0) { index = i; }
                }
                AddProjectilePrefabInternal(prefab, index); return true;
            }
            return false;
        }
        
        private void AddProjectilePrefabInternal(ShootersRushProjectile prefab, int index = -1)
        {
            if(index >= 0) { _projectilePrefabs[index] = prefab; } else { _projectilePrefabs.Add(prefab); }
        }
        
        public void ClearNullValues()
        {
            for (int i = _characterPrefabs.Count - 1; i >= 0; i--) { if(_characterPrefabs[i] == null) { _characterPrefabs.RemoveAt(i); } }
            for (int i = _enemyPrefabs.Count - 1; i >= 0; i--) { if(_enemyPrefabs[i] == null) { _enemyPrefabs.RemoveAt(i); } }
            for (int i = _weaponPrefabs.Count - 1; i >= 0; i--) { if(_weaponPrefabs[i] == null) { _weaponPrefabs.RemoveAt(i); } }
            for (int i = _projectilePrefabs.Count - 1; i >= 0; i--) { if(_projectilePrefabs[i] == null) { _projectilePrefabs.RemoveAt(i); } }
            for (int i = _muzzleFlashPrefabs.Count - 1; i >= 0; i--) { if(_muzzleFlashPrefabs[i] == null) { _muzzleFlashPrefabs.RemoveAt(i); } }
            for (int i = _containerPrefabs.Count - 1; i >= 0; i--) { if(_containerPrefabs[i] == null) { _containerPrefabs.RemoveAt(i); } }
            for (int i = _gatewayPrefabs.Count - 1; i >= 0; i--) { if(_gatewayPrefabs[i] == null) { _gatewayPrefabs.RemoveAt(i); } }
        }
        
        public void LoadAllPrefabs()
        {
            #if UNITY_EDITOR
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:Prefab");
            foreach(var guid in guids)
            {
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                LoadPrefabFromPath(path, false);
            }
            UnityEditor.EditorUtility.SetDirty(this);
            #endif
        }

        public void LoadPrefabFromPath(string path, bool setDirty = true)
        {
            #if UNITY_EDITOR
            ShootersRushCharacter character = UnityEditor.AssetDatabase.LoadAssetAtPath<ShootersRushCharacter>(path);
            if(character)
            {
                AddCharacterPrefab(character);
            }
            else
            {
                ShootersRushWeapon weapon = UnityEditor.AssetDatabase.LoadAssetAtPath<ShootersRushWeapon>(path);
                if(weapon)
                {
                    AddWeaponPrefab(weapon);
                }
                else
                {
                    ShootersRushProjectile projectile = UnityEditor.AssetDatabase.LoadAssetAtPath<ShootersRushProjectile>(path);
                    if(projectile)
                    {
                        AddProjectilePrefab(projectile);
                    }
                    else
                    {
                        ShootersRushMuzzleFlash muzzleFlash = UnityEditor.AssetDatabase.LoadAssetAtPath<ShootersRushMuzzleFlash>(path);
                        if(muzzleFlash)
                        {
                            AddMuzzleFlashPrefab(muzzleFlash);
                        }
                        else
                        {
                            ShootersRushEnemy enemy = UnityEditor.AssetDatabase.LoadAssetAtPath<ShootersRushEnemy>(path);
                            if(enemy)
                            {
                                AddEnemyPrefab(enemy);
                            }
                            else
                            {
                                ShootersRushContainer container = UnityEditor.AssetDatabase.LoadAssetAtPath<ShootersRushContainer>(path);
                                if(container)
                                {
                                    AddContainerPrefab(container);
                                }
                                else
                                {
                                    ShootersRushGateway gateway = UnityEditor.AssetDatabase.LoadAssetAtPath<ShootersRushGateway>(path);
                                    if(gateway)
                                    {
                                        AddGatewayPrefab(gateway);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (setDirty)
            {
                UnityEditor.EditorUtility.SetDirty(this);
            }
            #endif
        }
        
        public void RemoveWeaponSettings(string prefabID)
        {
            #if UNITY_EDITOR
            foreach (var prefab in _characterPrefabs)
            {
                if (prefab._EditorRemoveWeaponSettings(prefabID))
                {
                    UnityEditor.PrefabUtility.SavePrefabAsset(prefab.gameObject);
                }
            }
            #endif
        }
        
        #if UNITY_EDITOR
        [UnityEditor.MenuItem("Tools/Developers Hub/Shooters Rush/Library")]
        private static void SelectPrefabLibrary()
        {
            UnityEngine.Object obj = null;
            ShootersRushLibrary library = Load();
            if(library == null)
            {
                obj = CreatePrefabLibrary();
            }
            else
            {
                obj = library;
            }
            UnityEditor.EditorUtility.FocusProjectWindow();
            UnityEditor.Selection.activeObject = obj;
        }

        private static ShootersRushLibrary CreatePrefabLibrary()
        {
            string path = new string[] { "Assets", "Packages", "DevelopersHub", "ShootersRush", "Resources" }.Aggregate(Path.Combine);
            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            ShootersRushLibrary asset = ScriptableObject.CreateInstance<ShootersRushLibrary>();
            UnityEditor.AssetDatabase.CreateAsset(asset, Path.Combine(path, "Library.asset"));
            UnityEditor.AssetDatabase.SaveAssets();
            return asset;
        }
        #endif
        
    }
}