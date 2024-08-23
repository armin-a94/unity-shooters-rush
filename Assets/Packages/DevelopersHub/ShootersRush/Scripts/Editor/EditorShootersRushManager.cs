namespace DevelopersHub.ShootersRush.Editor
{
    using UnityEngine;
    using System;
    using UnityEditor;
    using UnityEditor.UIElements;
    using UnityEngine.UI;
    using UnityEngine.UIElements;
    using UnityEditor.SceneManagement;

    [CustomEditor(typeof(ShootersRushManager))] public class EditorShootersRushManager : Editor
    {
  
        private ShootersRushManager _manager = null;
        
        private void OnEnable()
        {
            _manager = (ShootersRushManager)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
        
    }
}