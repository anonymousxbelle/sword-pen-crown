using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UI;
using System.Collections;

namespace Managers
{
    public class SaveLoadManager : MonoBehaviour

    {
        [Header("UI References")] [SerializeField]
        private Button[] slotButtons;
        [SerializeField] private Button[] resetButtons;
        [SerializeField] private Button closeButton;
        [SerializeField] private TMP_Text[] slotLabels;
        
        private bool isSaveMode;
        public void Initialize(bool saveMode)
        {
            Debug.Log($"SaveLoadManager: Initialized in {(saveMode ? "SAVE" : "LOAD")} mode.");
            isSaveMode = saveMode;
            SetupButtons();

            // Disable slot buttons and enable only reset buttons if opened from main menu and new game with full slots
            if (GameManager.Instance.OpenedFromMainMenu && isSaveMode)
            {
                int firstEmpty = GameManager.Instance.GetFirstEmptySlot();
                // **FIX 1: Use IsResetForNewGameRequired flag to determine reset-only mode**
                bool resetRequired = GameManager.Instance.IsResetForNewGameRequired;

                if (resetRequired && firstEmpty == -1)
                {
                    for (int i = 0; i < slotButtons.Length; i++)
                    {
                        slotButtons[i].interactable = false;  // disable save/load slots selection
                        if (resetButtons.Length > i && resetButtons[i] != null)
                        {
                            resetButtons[i].interactable = true; // ensure reset buttons are enabled
                        }
                    }
                } 
                // Restore normal save/load interactivity if we were in the save/load scene 
                // but not for a required new game reset. This might need more logic 
                // depending on when IsNewGame is set, but the above block covers the specific bug.
                else if (GameManager.Instance.IsNewGame && firstEmpty != -1)
                {
                    // Ensure empty slots are clickable if starting a new game (normal flow)
                    for (int i = 0; i < slotButtons.Length; i++)
                    {
                        // Only empty slots should be interactable for a new game save
                        slotButtons[i].interactable = !GameManager.Instance.SaveExists(i);
                    }
                }
            }
            // Ensure slot buttons are interactable for non-new game save/load actions
            else
            {
                for (int i = 0; i < slotButtons.Length; i++)
                {
                    // In Load mode, only saved slots are interactable
                    // In Save mode (not new game), all slots are interactable
                    slotButtons[i].interactable = isSaveMode || GameManager.Instance.SaveExists(i);
                }
            }

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
                Debug.Log($"SaveLoadManager: Added listener to Slot {i + 1}.");
                
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
                
// Update reset button visibility
                if (resetButtons.Length > i && resetButtons[i] != null)
                {
                    resetButtons[i].gameObject.SetActive(GameManager.Instance.SaveExists(i));
                }

            }

        }
        private IEnumerator LoadAndRestore(int slotIndex)
        {
            GameSave save = GameManager.Instance.LoadGame(slotIndex);
            if (save == null || string.IsNullOrEmpty(save.sceneName)) yield break;

            PopupManager.Instance?.ForceClosePopup();

            AsyncOperation op = SceneManager.LoadSceneAsync(save.sceneName);
            yield return new WaitUntil(() => op.isDone);

            GameManager.Instance.SetPaused(false);

            while (DialogueManager.Instance == null || DialogueManager.Instance.InstanceIsNotReady())
                yield return null;

            DialogueManager.Instance.SetLine(save.dialogueIndex);
            DialogueManager.Instance.RefreshUI();

            GameManager.Instance.SetPaused(false);
            Time.timeScale = 1f;

            // Reset PauseMenu to ensure it's hidden and unpaused
            PauseMenu pauseMenu = FindAnyObjectByType<PauseMenu>();
            if (pauseMenu != null)
                pauseMenu.HidePauseMenuOnSceneStart();

            if (SceneManager.GetSceneByName("SaveLoadScene").isLoaded)
                SceneManager.UnloadSceneAsync("SaveLoadScene");

            if (GameManager.Instance.OpenedFromPauseMenu)
                FindAnyObjectByType<PauseMenu>()?.SetMenuVisible(false);
            else if (GameManager.Instance.OpenedFromMainMenu)
                FindAnyObjectByType<MainMenu>()?.SetMenuVisible(true);

            GameManager.Instance.ClearSaveLoadSource();
        }

        
     private void OnSlotClicked(int slotIndex)
            {
                if (isSaveMode)
                {
                    bool allFull = GameManager.Instance.GetFirstEmptySlot() == -1;
                    bool startingNewGame = GameManager.Instance.IsNewGame;

                    if (startingNewGame && allFull)
                    {
                        PopupManager.Instance.ShowConfirmation(
                            $"All save slots are full! Overwrite Slot {slotIndex + 1} to start a new game?",
                            () =>
                            {
                                Debug.Log($"Overwrite confirmed for slot {slotIndex + 1}");
                                GameManager.Instance.SetLastUsedSlot(slotIndex);
                                GameManager.Instance.SetNewGame();

                                // Set flags to auto-save after character selection scene loads
                                GameManager.Instance.ShouldAutoSaveNewGameAfterLoad = true;
                                GameManager.Instance.AutoSaveSlotIndex = slotIndex;

                                GameManager.Instance.StartNewGameTransition(slotIndex);
                            },
                            null
                        );
                        return; // Skip normal save flow
                    }

                    if (startingNewGame)
                    {
                        int firstEmpty = GameManager.Instance.GetFirstEmptySlot();
                        GameManager.Instance.SetLastUsedSlot(firstEmpty);
                        GameManager.Instance.SetNewGame();

                        GameManager.Instance.ShouldAutoSaveNewGameAfterLoad = true;
                        GameManager.Instance.AutoSaveSlotIndex = firstEmpty;

                        GameManager.Instance.StartNewGameTransition(slotIndex);
                        return;
                    }

                    // Normal save flow (not a new game)
                    int dialogueIndex = DialogueManager.Instance != null ? DialogueManager.Instance.GetCurrentLine() : 0;

                    if (GameManager.Instance.SaveExists(slotIndex))
                    {
                        PopupManager.Instance.ShowConfirmation(
                            $"Slot {slotIndex + 1} already has a save. Overwrite?",
                            () =>
                            {
                                GameManager.Instance.SaveGame(slotIndex, dialogueIndex);
                                RefreshUI();
                            },
                            () => { RefreshUI(); }
                        );
                    }
                    else
                    {
                        GameManager.Instance.SaveGame(slotIndex, dialogueIndex);
                        RefreshUI();
                    }
                }
                else
                {
                    // Load mode
                    if (!GameManager.Instance.SaveExists(slotIndex))
                    {
                        PopupManager.Instance.ShowMessage($"Slot {slotIndex + 1} is empty.");
                        return;
                    }
                    PopupManager.Instance.ShowConfirmation(
                        $"Load from slot {slotIndex + 1}? Current progress will be lost.",
                        () => { StartCoroutine(LoadAndRestore(slotIndex)); },
                        () => { }
                    );
                }
            }
            // SaveLoadManager.cs -> OnResetClicked method

            // SaveLoadManager.cs -> OnResetClicked method

            private void OnResetClicked(int slotIndex)
            {
                // Capture the state of the flags before the slot is reset
                bool wasOpenedFromMainMenu = GameManager.Instance.OpenedFromMainMenu;
                bool wasResetRequired = GameManager.Instance.IsResetForNewGameRequired;

                PopupManager.Instance.ShowConfirmation(
                    $"Reset slot {slotIndex + 1}? This cannot be undone.",
                    () => // THIS lambda runs on confirmation
                    {
                        GameManager.Instance.ResetSlot(slotIndex);
                        RefreshUI();

                        // The 'if' check is still necessary to prevent accidental new game starts
                        if (wasOpenedFromMainMenu && wasResetRequired)
                        {
                            // **THE FIX: Delegate the persistent transition to the GameManager**
                            GameManager.Instance.StartNewGameTransition(slotIndex);
                            return; 
                        }
                    },
                    () => { RefreshUI(); }
                );
            }

// **DELETE OR COMMENT OUT THIS COROUTINE in SaveLoadManager.cs**
/*
private IEnumerator UnloadSaveLoadAndLoadCharacterSelection()
{
    // ... no longer needed ...
}
*/
            
            /*private IEnumerator UnloadSaveLoadAndLoadCharacterSelection()
            {
                // Close any lingering popups before scene transition
                PopupManager.Instance?.ForceClosePopup();

                // 1. Unload the SaveLoadScene
                AsyncOperation unloadOp = SceneManager.UnloadSceneAsync("SaveLoadScene");
                yield return new WaitUntil(() => unloadOp.isDone);
    
                // **ULTIMATE FIX: Forcefully destroy the Main Menu Canvas/GameObject**
                // This is necessary because LoadSceneMode.Single isn't always instant/guaranteed 
                // to clean up complex scenes like the Main Menu immediately.
                if (GameManager.Instance.OpenedFromMainMenu && GameManager.Instance.MainMenuCanvas != null)
                {
                    Debug.Log("Forcing destruction of MainMenuCanvas before new scene load.");
                    Destroy(GameManager.Instance.MainMenuCanvas);
                }
    
                // Clear the source flags now that we know we are transitioning
                GameManager.Instance.ClearSaveLoadSource();
    
                // 2. Load the new scene, replacing everything.
                Debug.Log("Loading CharacterSelectionScene now with LoadSceneMode.Single...");
                SceneManager.LoadScene("CharacterSelectionScene", LoadSceneMode.Single); 
            }*/
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
                    mainMenu.SetMenuVisible(true); // Sets alpha back to 
                }
            }
            GameManager.Instance.ClearSaveLoadSource();
            SceneManager.UnloadSceneAsync("SaveLoadScene");
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.RefreshUI();
            }
        }
        
        public void HighlightSlotForOverwrite(int index)
        {
            GameManager.Instance.SetLastUsedSlot(index);
        }
    }
}