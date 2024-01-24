using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Startup : MonoBehaviour
{

    public List<string> scenes;

    public GameObject playerPrefab;

    private LevelData firstLevelData;

    // Start is called before the first frame update
    void Start()
    {
        SceneManager.sceneLoaded += OnLoadScene;
        foreach (string scene in scenes)
        {
            SceneManager.LoadScene(scene, LoadSceneMode.Additive);
        }
    }

    void OnLoadScene(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "first")
        {
            OnLoadFirstScene(scene);
        }
    }

    void OnLoadFirstScene(UnityEngine.SceneManagement.Scene scene)
    {
        var root = scene.GetRootGameObjects()[0];
        firstLevelData = root.GetComponent<LevelData>();
        Invoke("InstantiatePlayer", 0.1f);
    }

    void InstantiatePlayer()
    {
        var player = Instantiate(playerPrefab, firstLevelData.PlayerSpawn.position, Quaternion.identity);
        var playerMovement = player.GetComponent<PlayerMovement>();
        playerMovement.CurrentLevelData = firstLevelData;
    }
}
