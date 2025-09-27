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
        [SerializeField] private GameObject pauseMenuUI;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button loadButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button showPlaytimeButton;
        [SerializeField] private TMP_Text playtimeText;

        private bool isPaused = false;

        private void Start()
        {
            pauseMenuUI.SetActive(false);

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
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // Ensure a popup isn't currently open before pausing/resuming
                if (PopupManager.Instance != null && PopupManager.Instance.IsPopupActive) return;

                if (isPaused) Resume();
                else Pause();
            }
        }

        public void Pause()
        {
            pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            isPaused = true;
            GameManager.Instance.SetPaused(true);
        }

        public void Resume()
        {
            pauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
            isPaused = false;
            GameManager.Instance.SetPaused(false);
        }

        private void OpenSaveSceneForSave()
        {
            // Set the source and mode for the SaveLoadManager to use
            GameManager.Instance.SetSaveLoadSource(true, false, true); // (fromPauseMenu, isSaveMode=true)
            SceneManager.LoadScene("SaveLoadScene", LoadSceneMode.Additive);

            pauseMenuUI.SetActive(false);
        }

        private void OpenSaveSceneForLoad()
        {
            // Set the source and mode for the SaveLoadManager to use
            GameManager.Instance.SetSaveLoadSource(true, false, false); // (fromPauseMenu, isSaveMode=false)
            SceneManager.LoadScene("SaveLoadScene", LoadSceneMode.Additive);

            pauseMenuUI.SetActive(false);
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

        private void ShowPlaytime()
        {
            if (playtimeText != null)
            {
                playtimeText.text = $"Playtime: {GameManager.Instance.GetFormattedPlaytime()}";
            }
        }
    }
}