using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Managers;

namespace UI
{
    public class PauseMenu : MonoBehaviour
    {
        public static PauseMenu Instance { get; private set; }
        [SerializeField] private TMP_Text playtimeText;

        private bool isPaused;
        private bool isShowingPlaytime;

        private void Awake()
        {
            //ensures only one popup manager exists for the entire game.
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape) && UIManager.Instance.CanPause())
            {
               // if (PopupManager.Instance != null && PopupManager.Instance.IsPopupActive) return;
                if (isPaused) Resume();
                else Pause();
            }
            
        }
        

        public void Pause()
        {
            UIManager.Instance.ShowPauseMenu();
            Time.timeScale = 0f;
            isPaused = true;
            GameManager.Instance.SetPaused(true);
        }

        public void Resume()
        {
           UIManager.Instance.HidePauseMenu();
            Time.timeScale = 1f;
            isPaused = false;
            GameManager.Instance.SetPaused(false);
        }
        
        public void ReturnToMainMenu()
        {
            Debug.Log("PauseMenu: Main Menu button clicked. Showing confirmation popup.");
            PopupManager.Instance.ShowConfirmation(
                "Return to Main Menu? Unsaved progress will be lost.",
                () =>
                {
                    UIManager.Instance.HidePopUp();
                    SceneManager.LoadScene("StoryScene");//reloads scene
                },
                null // Cancel does nothing
            );
        
        }
        
        
        public void ShowPlaytime()
        {
            if (playtimeText == null) return;
            
            if (isShowingPlaytime)
            {
                playtimeText.text = "Playtime";
                isShowingPlaytime = false;
            }
            else
            {
                playtimeText.text = $"{GameManager.Instance.GetFormattedPlaytime()}";
                isShowingPlaytime = true;
            }
        }
        
    }
}