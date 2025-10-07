using UnityEngine;
using UnityEngine.SceneManagement;


public class Bootstrapper : MonoBehaviour
{
    [SerializeField] private string gameSystemsSceneName = "GameSystems";
    
    private void Awake()
    {
    // Make sure this object persists across scenes
        DontDestroyOnLoad(gameObject);
        
    // Check if GameSystems scene is already loaded
        if (!SceneManager.GetSceneByName(gameSystemsSceneName).isLoaded)
        {
        // Load GameSystems scene additively so it doesn't unload the bootstrapper
            SceneManager.LoadScene(gameSystemsSceneName, LoadSceneMode.Additive);
        }
    }
}