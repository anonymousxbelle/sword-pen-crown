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
        {
            startNewGameButton.onClick.RemoveAllListeners();
            startNewGameButton.onClick.AddListener(StartNewGame);

            loadGameButton.onClick.RemoveAllListeners();
            loadGameButton.onClick.AddListener(OpenLoadScene);

            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(QuitGame);

            // Ensure GameManager knows about the Main Menu Canvas for closing the Save/Load screen
            GameManager.Instance.MainMenuCanvas = mainMenuCanvas.gameObject;
        }

        public void StartNewGame()
        {
            // The slot to start a new game in (first empty, or 0 if full)
            int firstEmpty = GameManager.Instance.GetFirstEmptySlot();
            bool allFull = firstEmpty == -1; // -1 if all are full, as per GameManager update

            if (allFull)
            {
                PopupManager.Instance.ShowConfirmation(
                    "All save slots are full! Overwrite a slot?",
                    () =>
                    {
                        UnhookButtonListeners();
                        GameManager.Instance.SetSaveLoadSource(false, true, true); // (fromMainMenu, isSaveMode=true)
                        SceneManager.LoadScene("SaveLoadScene", LoadSceneMode.Additive);
                        SetMenuVisible(false);
                    },
                    null
                );
                return;
            }
            
            GameManager.Instance.MainMenuCanvas = null;
            UnhookButtonListeners();
            
            GameManager.Instance.SetLastUsedSlot(firstEmpty);
            GameManager.Instance.SetNewGame(); 
            SceneManager.LoadScene("CharacterSelectionScene"); 
        }
        private void UnhookButtonListeners()
        {
            startNewGameButton.onClick.RemoveAllListeners();
            loadGameButton.onClick.RemoveAllListeners();
            quitButton.onClick.RemoveAllListeners();
        }   
        public void OpenLoadScene()
        {
            // Set the source and mode for the SaveLoadManager to use
            GameManager.Instance.SetSaveLoadSource(false, true, false); // (fromMainMenu, isSaveMode=false)
            SceneManager.LoadScene("SaveLoadScene", LoadSceneMode.Additive);
            SetMenuVisible(false);
        }

        public void SetMenuVisible(bool active)
        {
            mainMenuCanvas.alpha = active ? 1f : 0f;
            mainMenuCanvas.interactable = active;
            mainMenuCanvas.blocksRaycasts = active;
        }

        public void QuitGame()
        {
            PopupManager.Instance.ShowConfirmation(
                "Are you sure you want to quit?",
                () =>
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
                },
                null
            );
        }
    }
}
