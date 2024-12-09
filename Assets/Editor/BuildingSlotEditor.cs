// using UnityEngine;
// using UnityEditor;
//
// [CustomEditor(typeof(BuildingShopSlot))]
// public class BuildingEditor : Editor
// {
//     SerializedProperty buildingTypeProp;
//     SerializedProperty timeTypeProp;
//
//     void OnEnable()
//     {
//         // Link the serialized properties
//         buildingTypeProp = serializedObject.FindProperty("buildingType");
//         timeTypeProp = serializedObject.FindProperty("timeType");
//     }
//
//     public override void OnInspectorGUI()
//     {
//         // Load the current values of the serialized properties
//         serializedObject.Update();
//
//         // Draw the BuildingType dropdown
//         EditorGUILayout.PropertyField(buildingTypeProp);
//
//         // If BuildingType is Home, draw the TimeType dropdown
//         if (buildingTypeProp.enumValueIndex == (int)BuildingTypes.Home)
//         {
//             EditorGUILayout.PropertyField(timeTypeProp);
//         }
//
//         // Apply changes to the serialized properties
//         serializedObject.ApplyModifiedProperties();
//     }
// }