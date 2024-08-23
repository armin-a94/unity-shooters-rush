namespace DevelopersHub.ShootersRush
{
    using UnityEngine;

    [ExecuteInEditMode] public class ShootersRushEditorAnimation : MonoBehaviour
    {
   
        private Animator _animator = null;
        private Transform _forwardFactor = null;
        public ShootersRushWeapon.Settings _weaponSettings = null;
        private ShootersRushWeapon _weapon = null;
        
        private void Awake()
        {
            Initialize();
            if (Application.isPlaying)
            {
                Destroy(this);
            }
        }

        private void OnEnable()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.update += EditorUpdate;
            #endif
        }
        
        private void OnDisable()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.update -= EditorUpdate;
            #endif
        }
        
        private void Initialize()
        {
            _animator = GetComponent<Animator>();
        }
        
        public void Initialize(ShootersRushWeapon.Settings weaponSettings, Transform forwardFactor, ShootersRushWeapon weapon)
        {
            _weapon = weapon;
            _weaponSettings = weaponSettings;
            _forwardFactor = forwardFactor;
        }
        
        private void EditorUpdate()
        {
            #if UNITY_EDITOR
            if (_animator == null && !Application.isPlaying)
            {
                Initialize();
            }
            #endif
            if (_animator != null && !Application.isPlaying)
            {
                _animator.Update(Time.deltaTime);
            }
            #if UNITY_EDITOR
            UnityEditor.SceneView.RepaintAll();
            #endif
        }
        
        private void LateUpdate()
        {
            if (_animator != null && _weaponSettings != null)
            {
                #if UNITY_EDITOR
                if (!Application.isPlaying && _weapon != null)
                {
                    _weapon.LateUpdate(_forwardFactor);
                }
                #endif
            }
        }
        
        private void OnAnimatorIK(int layer)
        {
            #if UNITY_EDITOR
            if (!Application.isPlaying && _weaponSettings != null)
            {
                if (_forwardFactor == null)
                {
                    _forwardFactor = transform;
                }
                HandleIK(layer, _forwardFactor.forward, _forwardFactor.right);
            }
            #endif
        }

        private void HandleIK(int layer, Vector3 forward, Vector3 right)
        {
            ShootersRushCharacterAnimationIK.HandleHandIK(_animator, _weaponSettings, _weapon);
        }
        
    }
}