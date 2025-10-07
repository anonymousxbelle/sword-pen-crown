using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Managers;

namespace UI
{
    public class MainMenu : MonoBehaviour

    {
        [SerializeField] private CanvasGroup mainMenuCanvas;
        [SerializeField] private Button startNewGameButton;
        [SerializeField] private Button loadGameButton;
        [SerializeField] private Button quitButton;
        private void Start()
        { /*Sets up button click listeners to call the correct methods.
        Removes any previous listeners to prevent duplicates.
        Stores a reference to the mainMenuCanvas in the GameManager so other systems can control its visibility.*/
            startNewGameButton.onClick.RemoveAllListeners();
            startNewGameButton.onClick.AddListener(StartNewGame);
            loadGameButton.onClick.RemoveAllListeners();
            loadGameButton.onClick.AddListener(OpenLoadScene);
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(QuitGame);
            GameManager.Instance.MainMenuCanvas = mainMenuCanvas.gameObject;
        }
        
        public void StartNewGame()
        {
            int firstEmpty = GameManager.Instance.GetFirstEmptySlot();//checks for empty slot
            if (firstEmpty == -1)
            { //if none
               
                PopupManager.Instance.ShowConfirmation(
                    "All slots are full! You must reset a slot to continue.",
                    () =>
                    {
                        GameManager.Instance.SetSaveLoadSource(false, true, true);
                        GameManager.Instance.IsResetForNewGameRequired = true;
                        SceneManager.LoadScene("SaveLoadScene", LoadSceneMode.Additive); //loads saveload scene
                        SetMenuVisible(false);
                    },
                    null
                );
                return;
            }
            
            //if there are empty slots
    
            GameManager.Instance.SetLastUsedSlot(firstEmpty); //selects empty slot for new game
            GameManager.Instance.SetNewGame();
            
            GameManager.Instance.ShouldAutoSaveNewGameAfterLoad = true;
            GameManager.Instance.AutoSaveSlotIndex = firstEmpty;

            SceneManager.LoadScene("CharacterSelectionScene");
            SetMenuVisible(false);
        }



        public void SetMenuVisible(bool active)
        {//Controls the visibility and interactivity of the main menu.
            mainMenuCanvas.alpha = active ? 1f : 0f;
            mainMenuCanvas.interactable = active;
            mainMenuCanvas.blocksRaycasts = active;
        }
        public void OpenLoadScene()
        {
            GameManager.Instance.SetSaveLoadSource(false, true, false); // fromMainMenu, load mode
            SceneManager.LoadScene("SaveLoadScene", LoadSceneMode.Additive); //loads saveload scene
            SetMenuVisible(false); //hides main menu
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
