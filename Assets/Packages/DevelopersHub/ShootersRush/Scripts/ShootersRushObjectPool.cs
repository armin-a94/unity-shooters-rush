namespace DevelopersHub.ShootersRush
{
    using System.Collections.Generic;
    using UnityEngine;
    
    public class ShootersRushObjectPool : MonoBehaviour
    {

        private static ShootersRushObjectPool _singleton = null; public static ShootersRushObjectPool Singleton
        {
            get
            {
                if (_singleton == null)
                {
                    _singleton = FindObjectOfType<ShootersRushObjectPool>();
                    if (_singleton == null)
                    {
                        _singleton = new GameObject("ObjectPool").AddComponent<ShootersRushObjectPool>();
                    }
                }
                return _singleton;
            }
        }
        
        private Dictionary<ShootersRushProjectile, string> _projectilePool = new Dictionary<ShootersRushProjectile, string>();
        private Dictionary<ShootersRushMuzzleFlash, string> _muzzleFlashPool = new Dictionary<ShootersRushMuzzleFlash, string>();
        private Dictionary<ShootersRushEnemy, string> _enemyPool = new Dictionary<ShootersRushEnemy, string>();
        
        public static ShootersRushProjectile GetProjectileInstance(string id)
        {
            ShootersRushProjectile projectile = null;
            if (Singleton._projectilePool.Count > 0)
            {
                foreach (var item in _singleton._projectilePool)
                {
                    if (item.Value == id)
                    {
                        projectile = item.Key;
                        _singleton._projectilePool.Remove(projectile);
                        projectile.Reset();
                        projectile.transform.SetParent(null);
                        projectile.gameObject.SetActive(true);
                        break;
                    }
                }
            }
            if (projectile == null)
            {
                ShootersRushProjectile prefab = ShootersRushLibrary.GetProjectilePrefab(id);
                projectile = Instantiate(prefab);
                projectile.Initialize();
            }
            return projectile;
        }
        
        public static void SetProjectileInstance(ShootersRushProjectile projectile)
        {
            if (projectile != null && !Singleton._projectilePool.ContainsKey(projectile))
            {
                projectile.Reset();
                projectile.gameObject.SetActive(false);
                projectile.transform.SetParent(_singleton.transform);
                _singleton._projectilePool.Add(projectile, projectile.id);
            }
        }
        
        public static ShootersRushMuzzleFlash GetMuzzleFlashInstance(string id)
        {
            ShootersRushMuzzleFlash muzzleFlash = null;
            if (Singleton._muzzleFlashPool.Count > 0)
            {
                foreach (var item in _singleton._muzzleFlashPool)
                {
                    if (item.Value == id)
                    {
                        muzzleFlash = item.Key;
                        _singleton._muzzleFlashPool.Remove(muzzleFlash);
                        muzzleFlash.Reset();
                        muzzleFlash.transform.SetParent(null);
                        muzzleFlash.gameObject.SetActive(true);
                        break;
                    }
                }
            }
            if (muzzleFlash == null)
            {
                ShootersRushMuzzleFlash prefab = ShootersRushLibrary.GetMuzzleFlashPrefab(id);
                muzzleFlash = Instantiate(prefab);
                muzzleFlash.Initialize();
            }
            return muzzleFlash;
        }
        
        public static void SetMuzzleFlashInstance(ShootersRushMuzzleFlash muzzleFlash)
        {
            if (muzzleFlash != null && !Singleton._muzzleFlashPool.ContainsKey(muzzleFlash))
            {
                muzzleFlash.Reset();
                muzzleFlash.gameObject.SetActive(false);
                muzzleFlash.transform.SetParent(_singleton.transform);
                _singleton._muzzleFlashPool.Add(muzzleFlash, muzzleFlash.id);
            }
        }
        
    }
}