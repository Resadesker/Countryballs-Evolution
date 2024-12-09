// using UnityEditor;
// using UnityEngine;
//
// [CustomEditor(typeof(Grid))]
// public class GridInstantiatorEditor : Editor
// {
//     public override void OnInspectorGUI()
//     {
//         DrawDefaultInspector();
//
//         Grid gridInstantiator = (Grid)target;
//
//         if (GUILayout.Button("Instantiate Grid"))
//         {
//             gridInstantiator.InstantiateGrid();
//         }
//     }
// }