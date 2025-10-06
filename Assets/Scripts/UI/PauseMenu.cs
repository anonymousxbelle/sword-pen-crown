using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Managers;

namespace UI
{
    public class PauseMenu : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private CanvasGroup pauseMenuCanvas;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button loadButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button showPlaytimeButton;
        [SerializeField] private TMP_Text playtimeText;

        private bool isPaused = false;
        private void Start()
        {
            // Added RemoveAllListeners for safety
            resumeButton.onClick.RemoveAllListeners();
            resumeButton.onClick.AddListener(Resume);
            
            saveButton.onClick.RemoveAllListeners();
            saveButton.onClick.AddListener(OpenSaveSceneForSave);
            
            loadButton.onClick.RemoveAllListeners();
            loadButton.onClick.AddListener(OpenSaveSceneForLoad);
            
            mainMenuButton.onClick.RemoveAllListeners();
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);
            
            showPlaytimeButton.onClick.RemoveAllListeners();
            showPlaytimeButton.onClick.AddListener(ShowPlaytime);
            
            GameManager.Instance.PauseMenuCanvas = pauseMenuCanvas.gameObject;
            
            HidePauseMenuOnSceneStart();
        }

        // PauseMenu.cs (Add or update the Update method)

        private void Update()
        {
            // Check if the game is paused/resumed (your existing logic)
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (PopupManager.Instance != null && PopupManager.Instance.IsPopupActive) return;
                if (isPaused) Resume();
                else Pause();
            }

            // *** NEW: Control save button visibility/interactability ***
            ControlSaveButton();
        }

// PauseMenu.cs (Add this new private method)

        private void ControlSaveButton()
        {
            // Get the name of the currently loaded scene
            string currentSceneName = SceneManager.GetActiveScene().name;
    
            // Define scenes where saving is NOT allowed
            bool isSetupScene = 
                currentSceneName == "MainMenuScene" || 
                currentSceneName == "CharacterSelectionScene" ||
                currentSceneName == "SaveLoadScene"; // Also block saving if in the Save/Load menu itself (optional, but safer)

            // Save button should be interactable only if we are NOT in a setup scene
            if (saveButton != null)
            {
                saveButton.interactable = !isSetupScene;
                // Optionally, you can visually hide the button if saving is not allowed
                // saveButton.gameObject.SetActive(!isSetupScene);
            }
        }
        public void Pause()
        {
            SetMenuVisible(true);
            Time.timeScale = 0f;
            isPaused = true;
            GameManager.Instance.SetPaused(true);
        }

        public void Resume()
        {
            SetMenuVisible(false);
            Time.timeScale = 1f;
            isPaused = false;
            GameManager.Instance.SetPaused(false);
        }

        private void OpenSaveSceneForSave()
        {
            // Set the source and mode for the SaveLoadManager to use
            GameManager.Instance.SetSaveLoadSource(true, false, true); // (fromPauseMenu, isSaveMode=true)
            SceneManager.LoadScene("SaveLoadScene", LoadSceneMode.Additive);

            SetMenuVisible(false);
        }

        private void OpenSaveSceneForLoad()
        {
            // Set the source and mode for the SaveLoadManager to use
            GameManager.Instance.SetSaveLoadSource(true, false, false); // (fromPauseMenu, isSaveMode=false)
            SceneManager.LoadScene("SaveLoadScene", LoadSceneMode.Additive);

            SetMenuVisible(false);
        }

        private void ReturnToMainMenu()
        {
            Debug.Log("PauseMenu: Main Menu button clicked. Showing confirmation popup.");
            PopupManager.Instance.ShowConfirmation(
                "Return to Main Menu? Unsaved progress will be lost.",
                () =>
                {
                    // CRITICAL FIX: Close the popup NOW, while we are still in the GameScene
                    PopupManager.Instance.ForceClosePopup(); 
            
                    Time.timeScale = 1f;
                    GameManager.Instance.SetPaused(false);
                    GameManager.Instance.ClearSaveLoadSource();
            
                    SceneManager.LoadScene("MainMenuScene");
                },
                null // Cancel does nothing
            );
        
        }
        //Trying to fix dialogue not reacting after load game
        public void SetMenuVisible(bool active)
        {
            pauseMenuCanvas.alpha = active ? 1f : 0f;
            pauseMenuCanvas.interactable = active;
            pauseMenuCanvas.blocksRaycasts = active;
        }
        
        public void HidePauseMenuOnSceneStart()
        {
            SetMenuVisible(false);
            isPaused = false;
            GameManager.Instance.SetPaused(false);
            Time.timeScale = 1f;
        }


        private void ShowPlaytime()
        {
            if (playtimeText != null)
            {
                playtimeText.text = $"Playtime: {GameManager.Instance.GetFormattedPlaytime()}";
            }
        }
    }
}