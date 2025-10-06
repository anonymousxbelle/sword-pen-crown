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
            GameManager.Instance.MainMenuCanvas = mainMenuCanvas.gameObject;
        }
        
        public void StartNewGame()
        {
            int firstEmpty = GameManager.Instance.GetFirstEmptySlot();
            if (firstEmpty == -1)
            {
                int slotToOverwrite = GameManager.Instance.LastUsedSlotIndex;
                PopupManager.Instance.ShowConfirmation(
                    "All slots are full! You must reset a slot to continue.",
                    () =>
                    {
                        GameManager.Instance.SetSaveLoadSource(false, true, true);
                        GameManager.Instance.SetLastUsedSlot(slotToOverwrite);
                        GameManager.Instance.IsResetForNewGameRequired = true;
                        SceneManager.LoadScene("SaveLoadScene", LoadSceneMode.Additive);
                        SaveLoadManager manager = FindAnyObjectByType<SaveLoadManager>();
                        if (manager != null) manager.HighlightSlotForOverwrite(slotToOverwrite);
                        SetMenuVisible(false);
                    },
                    null
                );
                return;
            }
    
            GameManager.Instance.SetLastUsedSlot(firstEmpty);
            GameManager.Instance.SetNewGame();

            // Set deferred flags - no immediate SaveGame call here!
            GameManager.Instance.ShouldAutoSaveNewGameAfterLoad = true;
            GameManager.Instance.AutoSaveSlotIndex = firstEmpty;

            SceneManager.LoadScene("CharacterSelectionScene");

            SetMenuVisible(false);
        }



        public void SetMenuVisible(bool active)
        {
            mainMenuCanvas.alpha = active ? 1f : 0f;
            mainMenuCanvas.interactable = active;
            mainMenuCanvas.blocksRaycasts = active;
        }
        public void OpenLoadScene()
        {
            GameManager.Instance.SetSaveLoadSource(false, true, false); // fromMainMenu, load mode
            SceneManager.LoadScene("SaveLoadScene", LoadSceneMode.Additive);
            SetMenuVisible(false);
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
