using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UI;

namespace Managers
{
    public class SaveLoadManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button[] slotButtons; 
        [SerializeField] private Button[] resetButtons;
        [SerializeField] private Button closeButton;
        [SerializeField] private TMP_Text[] slotLabels; 

        private bool isSaveMode;

        public void Initialize(bool saveMode)
        {
            Debug.Log($"SaveLoadManager: Initialized in {(saveMode ? "SAVE" : "LOAD")} mode.");
            isSaveMode = saveMode;
            SetupButtons();
            RefreshUI();
        }

        private void SetupButtons()
        {
            Debug.Log($"SaveLoadManager: SetupButtons called. Slot count: {slotButtons.Length}");
            for (int i = 0; i < slotButtons.Length; i++)
            {
                int index = i;
                slotButtons[i].onClick.RemoveAllListeners();
                slotButtons[i].onClick.AddListener(() => OnSlotClicked(index));
                Debug.Log($"SaveLoadManager: Added listener to Slot {i+1}.");

                if (resetButtons.Length > i && resetButtons[i] != null)
                {
                    resetButtons[i].onClick.RemoveAllListeners();
                    resetButtons[i].onClick.AddListener(() => OnResetClicked(index));
                    // Only show reset buttons in load mode or when save exists
                   // resetButtons[i].gameObject.SetActive(!isSaveMode || GameManager.Instance.SaveExists(index));
                }
            }

            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(CloseScene);
            }
        }

        private void RefreshUI()
        {
            for (int i = 0; i < slotButtons.Length; i++)
            {
                if (slotLabels != null && slotLabels.Length > i && slotLabels[i] != null)
                {
                    slotLabels[i].text = GameManager.Instance.GetSlotLabel(i);
                }
                
                // Update reset button visibility (optional but good UI)
                if (resetButtons.Length > i && resetButtons[i] != null)
                {
                    resetButtons[i].gameObject.SetActive(GameManager.Instance.SaveExists(i));
                }
            }
        }

        private void OnSlotClicked(int slotIndex)
        {
            if (isSaveMode)
            {
                int dialogueIndex = DialogueManager.Instance != null ? DialogueManager.Instance.GetCurrentLine() : 0;
                PopupManager.Instance.ShowConfirmation(
                    $"Save to slot {slotIndex + 1}?",
                    () =>
                    {
                        GameManager.Instance.SaveGame(slotIndex, dialogueIndex);
                        RefreshUI();
                    },
                    () => { RefreshUI(); } // just refresh on cancel
                );
            }
            else // Load Mode
            {
                if (!GameManager.Instance.SaveExists(slotIndex))
                {
                    PopupManager.Instance.ShowMessage($"Slot {slotIndex + 1} is empty.");
                    return;
                }

                PopupManager.Instance.ShowConfirmation(
                    $"Load from slot {slotIndex + 1}? Current progress will be lost.",
                    () =>
                    {
                        // 1. Load the data into GameManager
                        GameManager.Instance.LoadGame(slotIndex);

                        // 2. CRITICAL FIX: Close the popup immediately after load, 
                        // but before the scene change.
                        PopupManager.Instance.ForceClosePopup();

                        // 3. Load the actual scene saved in the data
                        if (GameManager.Instance.currentSave != null)
                        {
                            CloseScene(); // Close the SaveLoad Scene first
                            SceneManager.LoadScene(GameManager.Instance.currentSave.sceneName);
                        }
                    },
                    () => { } // cancel does nothing
                );
            }
        }

        private void OnResetClicked(int slotIndex)
        {
            PopupManager.Instance.ShowConfirmation(
                $"Reset slot {slotIndex + 1}? This cannot be undone.",
                () =>
                {
                    GameManager.Instance.ResetSlot(slotIndex);
                    RefreshUI();
                },
                () => { RefreshUI(); }
            );
        }
        private void CloseScene()
        {
            if (GameManager.Instance.OpenedFromPauseMenu)
            {
                // Assuming the PauseMenu script is active and can be found:
                PauseMenu pauseMenu = FindAnyObjectByType<PauseMenu>();
                if (pauseMenu != null)
                {
                    pauseMenu.Pause(); // Re-activate the pause menu UI without changing Time.timeScale (which is already 0)
                }

            }
            else if (GameManager.Instance.OpenedFromMainMenu)
            {
                MainMenu mainMenu = FindAnyObjectByType<MainMenu>();
                if (mainMenu != null)
                {
                    mainMenu.SetMenuVisible(true); // Sets alpha back to 1
                }
            }

            GameManager.Instance.ClearSaveLoadSource();
            SceneManager.UnloadSceneAsync("SaveLoadScene");
        }
    }
}
