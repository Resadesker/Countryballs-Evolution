// using System.Collections;
// using System.Collections.Generic;
// // using UnityEditor;
// using UnityEngine;
//
//
// public class Grid : MonoBehaviour
// {
//     public Building prefab;
//     public RectTransform gridCanvas;
//     public int amountToSide = 0;
//     public int amountToTop = 0;
//     public int amountToBottom = 0;
//
//     void OnEnable()
//     {
//         SceneView.duringSceneGui += OnSceneGUI;
//     }
//
//     void OnDisable()
//     {
//         SceneView.duringSceneGui -= OnSceneGUI;
//     }
//
//     private void Start()
//     {
//         // InstantiateGrid(GetComponent<RectTransform>().position);
//     }
//     
//     public void InstantiateGrid()
//     {
//         // Clear existing objects
//         Transform[] children = gridCanvas.GetComponentsInChildren<Transform>();
//         foreach (Transform child in children)
//         {
//             if (child != gridCanvas.transform)
//             {
//                 DestroyImmediate(child.gameObject);
//             }
//         }
//
//         // Instantiate new grid
//         for (int x = -amountToSide; x <= amountToSide; x++)
//         {
//             for (int z = -amountToBottom; z <= amountToTop; z++)
//             {
//                 Vector3 position = gridCanvas.position + new Vector3(x * prefab.GetComponent<RectTransform>().rect.width, z * prefab.GetComponent<RectTransform>().rect.height, 0f);
//
//                 GameObject building = PrefabUtility.InstantiatePrefab(prefab, gridCanvas.transform.parent) as GameObject;
//                 RectTransform buildingRect = building.GetComponent<RectTransform>();
//
//                 // Set the position of the building relative to the gridCanvas
//                 buildingRect.anchoredPosition = position;
//
//                 // Optionally set other properties like size, rotation, etc., based on your requirements.
//
//                 // Add the new object to the BuildingManager's slots list (if needed)
//                 // buildingManager.slots.Add(building);
//             }
//         }
//     }
//
//     void OnSceneGUI(SceneView sceneView)
//     {
//         InstantiateGrid();
//     }
// }
