namespace DevelopersHub.ShootersRush.Editor
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    
    public class EditorShootersRushTools
    {
   
        public static void AddTag(string tag)
        {
            ShootersRushTools.AddTag(tag);
        }
        
        public static bool IsInPrefabEditMode()
        {
            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            return prefabStage != null;
        }
        
    }
}