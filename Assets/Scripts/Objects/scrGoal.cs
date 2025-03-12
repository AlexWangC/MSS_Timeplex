using System.Collections.Generic;
using System.Threading;
using Fries;
# if UNITY_EDITOR
using System.IO;
using UnityEditor.SceneManagement;
using UnityEditor;
# endif
//using UnityEditor.SearchService;
using UnityEngine;

public class scrGoal : MonoBehaviour
{
    //public bool MovementLock; // set whether after reaching this goal locks player movement in this scene. NOT YET IMPLEMENTED
    public bool Reached;
    public bool Locked = false;
    public int DoorType = -1; // whether it's corresponding to key 1, 2, or 3. Default -1 means the door's not locked.

    // [HideInInspector]
    public string nextSceneName;
    [Tooltip("Set connected scene")]
    # if UNITY_EDITOR
    [SerializeField] public SceneAsset nextScene; // Drag a scene here in the Inspector
    # endif
    // private void OnValidate()
    // {
    //     # if UNITY_EDITOR
    //     if (nextScene != null)
    //     {
    //         nextSceneName = nextScene.name;
    //     }
    //     # endif
    // }

    # if UNITY_EDITOR
    [MenuItem("Tools/Fries/Bat")]
    public static void item() {
        List<string> scenePaths = new List<string>();

        // 获取当前 Project 窗口中选中的所有资源 GUID，并转换为路径
        foreach (string guid in Selection.assetGUIDs) {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            // 递归遍历路径，收集所有场景文件 (.unity)
            CollectSceneFiles(path, scenePaths);
        }

        if (scenePaths.Count > 0) {
            foreach (string scenePath in scenePaths) {
                // 加载场景（注意：加载场景时会替换当前打开的场景）
                EditorSceneManager.OpenScene(scenePath);
                Debug.Log("Loading scene: " + scenePath);

                // 执行方法 a()
                scrGoal[] goals = FindObjectsByType<scrGoal>(FindObjectsSortMode.None);
                goals.ForEach(goal => {
                    if (goal.nextScene == null) {
                        Debug.Log($"Scene {scenePath}, goal {goal.name} has no nextScene attached");
                        return;
                    }

                    if (goal.nextScene.name == goal.nextSceneName) {
                        Debug.Log("Exact same name is already set");
                        return;
                    }
                    
                    Debug.Log($"Changed scene name from '{goal.nextSceneName}' to '{goal.nextScene.name}'");
                    goal.nextSceneName = goal.nextScene.name;
                });

                // 等待3秒（阻塞主线程，期间编辑器可能会短暂无响应）
                Thread.Sleep(3000);
            }

            Debug.Log("All of the selected scenes are processed");
        }
        else {
            Debug.Log("No scene is found");
        }
    }

    private static void CollectSceneFiles(string path, List<string> scenePaths) {
        if (Directory.Exists(path)) {
            // 遍历当前文件夹中的所有文件
            string[] files = Directory.GetFiles(path);
            foreach (string file in files) {
                if (Path.GetExtension(file).ToLower() == ".unity") {
                    scenePaths.Add(file);
                }
            }

            // 递归遍历所有子文件夹
            string[] directories = Directory.GetDirectories(path);
            foreach (string dir in directories) {
                CollectSceneFiles(dir, scenePaths);
            }
        }
        else if (File.Exists(path)) {
            // 如果是单个文件，则判断是否为场景文件
            if (Path.GetExtension(path).ToLower() == ".unity") {
                scenePaths.Add(path);
            }
        }
    }
# endif

    private void Start()
    {
        Reached = false; // uwu 
        // if (nextScene.name == scrSceneSequenceManager.lastScene.name)
        if (nextSceneName == scrSceneSequenceManager.lastScene.name)
        {
            //move player position
            Transform parent = transform.parent;
            if (parent != null)
            {
                // Search for a child with the "Player" tag
                foreach (Transform child in parent)
                {
                    if (child.CompareTag("Player")) // Check tag
                    {
                        GridObject gridObject = child.GetComponent<GridObject>();
                        if (gridObject != null)
                        {
                            gridObject.gridPosition = GetComponent<GridObject>().gridPosition;
                        }
                    }
                }
            }
        }
    }

    private void Update()
    {
        checkIfReached();
    }

    // i think this is the problem. Refactor it first?
    public bool checkIfReached()
    {
        scrGridManager grid_manager = transform.parent.gameObject.GetComponentInChildren<scrGridManager>();
        
        GridObject[] objects_at_this_position = grid_manager.GetGridObjectsAtPosition(toVector2Int(GetComponent<GridObject>().gridPosition)); // getting all objects at this scrGoal's location
        if (objects_at_this_position.Length >= 1)
        {
            foreach (GridObject obj in objects_at_this_position)
            {
                if (obj.CompareTag("player"))
                {
                    //print(obj.name + " Set reached to true");
                    Reached = true;
                    return true;
                }
            }
        }

        //print(gameObject.name + " Set reached to false");
        Reached = false;
        return false;
    }

    private Vector2Int toVector2Int(Vector2 vector2) // helper from scr player. Local access for efficiency
    {
        return new Vector2Int((int)vector2.x, (int)vector2.y);
    }
}
