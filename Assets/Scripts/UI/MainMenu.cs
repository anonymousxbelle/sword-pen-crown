using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Managers;

namespace UI
{
    public class MainMenu : MonoBehaviour

    {
        private void Start()
        { 
         
            
           
        }
        
        public void StartNewGame()
        {
            UIManager.Instance.GoToDialogueScreen();
            DialogueManager.Instance.LoadInkStory();
        }
        
        public void LoadScene()
        {
           UIManager.Instance.GoToLoadScreen();
        }
        public void QuitGame()
        {
            PopupManager.Instance.ShowConfirmation(
                "Are you sure you want to quit?",
                () =>
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false; //Stops play mode in the editor
#else
Application.Quit();//quits the built game
#endif
                },
                null
            );
        }
    }
}
