namespace DevelopersHub.ShootersRush
{
    using UnityEngine;
    
    public class ShootersRushCharacterAnimationIK : MonoBehaviour
    {

        private Animator _animator = null;
        private bool _enabled = true;
        private Transform _forwardFactor = null;
        public ShootersRushWeapon.Settings _weaponSettings = null;
        private ShootersRushWeapon _weapon = null;
        
        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }
        
        public void Initialize(ShootersRushWeapon.Settings weaponSettings, Transform forwardFactor, ShootersRushWeapon weapon)
        {
            _weapon = weapon;
            _weaponSettings = weaponSettings;
            _forwardFactor = forwardFactor;
        }
        
        private void OnAnimatorIK(int layer)
        {
            if (_enabled && _weaponSettings != null)
            {
                if (_forwardFactor == null)
                {
                    _forwardFactor = transform;
                }
                #if UNITY_EDITOR
                if (Application.isPlaying)
                {
                    HandleHandIK(_animator, _weaponSettings, _weapon);
                }
                #else
                HandleHandIK(_animator, _weaponSettings, _weapon);
                #endif
            }
        }

        public static void HandleHandIK(Animator animator, ShootersRushWeapon.Settings settings, ShootersRushWeapon _weapon)
        {
            #if UNITY_EDITOR
            if (animator == null || _weapon == null) { return; }
            #endif

            Vector3 p = _weapon.rightHandIK.position;
            Quaternion r = _weapon.rightHandIK.rotation;
            
            #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                p = _weapon.rightHandIKPosition;
                r = _weapon.rightHandIKRotation;
            }
            #endif
            
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
            animator.SetIKPosition(AvatarIKGoal.RightHand, p);
            animator.SetIKRotation(AvatarIKGoal.RightHand, r);
            
            if (settings.useLeftHand)
            {
                p = _weapon.leftHandIK.position;
                r = _weapon.leftHandIK.rotation;
            
                #if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    p = _weapon.leftHandIKPosition;
                    r = _weapon.leftHandIKRotation;
                }
                #endif
                
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);
                animator.SetIKPosition(AvatarIKGoal.LeftHand, p);
                animator.SetIKRotation(AvatarIKGoal.LeftHand, r);
            }
        }
        
    }
}