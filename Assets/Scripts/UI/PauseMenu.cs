using UnityEngine;
using TMPro;
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
            PopupManager.Instance.ShowConfirmation(
                "Return to Main Menu? Unsaved progress will be lost.",
                () =>
                {
                    UIManager.Instance.HidePopUp();
                    UIManager.Instance.HidePauseMenu();
                    UIManager.Instance.GoToMainMenu();
                },
                null 
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