using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
//using UnityEngine.CoreModule;
[InitializeOnLoad]
public class MyScenePostprocessor
{
    static MyScenePostprocessor()
    {
        
         EditorApplication.hierarchyChanged += ExampleCallback;
        //EditorSceneManager.newSceneCreated += ExampleCallback;
       // EditorSceneManager.newSceneCreated += new EditorSceneManager.NewSceneCreatedCallback(ExampleCallback);


    }

    static void ExampleCallback(/*UnityEngine.SceneManagement.Scene scene, NewSceneSetup sceneSetups, NewSceneMode sceneMode*/)
    {
        
        
        //EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Single);
        
        List<GameObject> sceneObjects = new List<GameObject>(GameObject.FindObjectsOfType<GameObject>());

        // Does it match the default scene exactly?
        if (sceneObjects.Count == 2 && sceneObjects.Find(go => go.name == "Main Camera") && sceneObjects.Find(go => go.name == "Directional Light"))
        {
            string defaultScenePath = System.IO.Path.Combine(Application.dataPath, "!Global/Scenes/NewWaveDefaultScene.unity");
            if (System.IO.File.Exists(defaultScenePath))
            {
                //Debug.Log("Detected default scene - replacing with DefaultSceneOverride");
                //EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
                EditorApplication.OpenSceneAdditive(defaultScenePath);
                //EditorSceneManager.OpenScene(defaultScenePath, OpenSceneMode.Additive);
            }
        }
    }
}
#endif