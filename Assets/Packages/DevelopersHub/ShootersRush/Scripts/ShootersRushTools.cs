namespace DevelopersHub.ShootersRush
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    
    public class ShootersRushTools : MonoBehaviour
    {

        public enum Axis
        {
            PositiveX = 0, PositiveY = 1, PositiveZ = 2, NegativeX = 3, NegativeY = 4, NegativeZ = 5
        }

        public static Vector3 AxisToVector3(Axis axis)
        {
            switch (axis)
            {
                case Axis.PositiveX: return Vector3.right;
                case Axis.PositiveY: return Vector3.up;
                case Axis.PositiveZ: return Vector3.forward;
                case Axis.NegativeX: return Vector3.left;
                case Axis.NegativeY: return Vector3.down;
                case Axis.NegativeZ: return Vector3.back;
            }
            return Vector3.zero;
        }
        
        /// <summary>
        /// Calculates the world coordinates of the four corners of the camera's far clipping plane.
        /// </summary>
        /// <param name="camera">The Camera for which to calculate the frustum corners.</param>
        /// <returns>An array of Vector3 containing the world coordinates of the corners in the following order: Bottom-left, Top-left, Top-right, Bottom-right.
        /// </returns>
        public static Vector3[] GetCameraFrustumFarCorners(Camera camera)
        {
            // Array to hold the frustum corners
            Vector3[] frustumCorners = new Vector3[4];

            // Get the frustum corners in the camera's local space
            camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), camera.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, frustumCorners);
            
            // Transform the corners to world space
            for (int i = 0; i < 4; i++)
            {
                frustumCorners[i] = camera.transform.TransformPoint(frustumCorners[i]);
            }
            return frustumCorners;
        }
        
        public static float GetTriangleSide(float sideAB, float sideAC, float angleBetweenABAC)
        {
            return Mathf.Sqrt(sideAB * sideAB + sideAC * sideAC - 2f * sideAB * sideAC * Mathf.Cos(angleBetweenABAC * Mathf.Deg2Rad));
        }
        
        public static void AddTag(string tag)
        {
            #if UNITY_EDITOR
            UnityEngine.Object[] asset = UnityEditor.AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
            if (asset != null && asset.Length > 0)
            {
                UnityEditor.SerializedObject serializedObject = new UnityEditor.SerializedObject(asset[0]);
                UnityEditor.SerializedProperty tags = serializedObject.FindProperty("tags");
                for (int i = 0; i < tags.arraySize; ++i)
                {
                    if (tags.GetArrayElementAtIndex(i).stringValue == tag)
                    {
                        return;
                    }
                }
                tags.InsertArrayElementAtIndex(0);
                tags.GetArrayElementAtIndex(0).stringValue = tag;
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
            }
            #endif
        }
        
    }
}