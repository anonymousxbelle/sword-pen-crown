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
        [SerializeField] private TextAsset inkFile;
        
        private bool isSaveMode;
        public void Initialize(bool saveMode)
        { //Sets up everything based on whether it’s Save or Load mode.
            Debug.Log($"SaveLoadManager: Initialized in {(saveMode ? "SAVE" : "LOAD")} mode.");
            isSaveMode = saveMode;
            SetupButtons(); //attaches listeners

           
            if (GameManager.Instance.OpenedFromMainMenu && isSaveMode)
            { //if opened from main menu 
                int firstEmpty = GameManager.Instance.GetFirstEmptySlot();
                bool resetRequired = GameManager.Instance.IsResetForNewGameRequired;
                //Gets the first empty slot and checks if a reset is required
                if (resetRequired && firstEmpty == -1)
                {//Resetrequired:(if we're loading a new game and all slots are full).
                    for (int i = 0; i < slotButtons.Length; i++)
                    {
                        slotButtons[i].interactable = false;  // disable save/load slots selection
                        if (resetButtons.Length > i && resetButtons[i] != null)
                        {
                            resetButtons[i].interactable = true; // ensure reset buttons are enabled
                        }
                    }
                } 
                else if (GameManager.Instance.IsNewGame && firstEmpty != -1)
                {//There’s space to save a new game.
                    for (int i = 0; i < slotButtons.Length; i++)
                    {
                        // Only empty slots should be interactable for a new game save
                        slotButtons[i].interactable = !GameManager.Instance.SaveExists(i);
                    }
                }//UNSURE THIS MATTERS BECAUSE NEW GAME AUTO-SAVES IN FIRST EMPTY SLOT
            }
           
            else
            { //Not from main menu
                for (int i = 0; i < slotButtons.Length; i++)
                {
                    // In Load mode, only saved slots are interactable
                    // In Save mode (not new game), all slots are interactable
                    slotButtons[i].interactable = isSaveMode || GameManager.Instance.SaveExists(i);
                }
            }
            RefreshUI();//Updates text and button visuals.
        }
        
        private void SetupButtons()
        {
            Debug.Log($"SaveLoadManager: SetupButtons called. Slot count: {slotButtons.Length}");
            for (int i = 0; i < slotButtons.Length; i++)
            {//Loops through each slot.
                int index = i;//for lambda
                slotButtons[i].onClick.RemoveAllListeners();//Clears old listeners (avoiding duplicates).
                slotButtons[i].onClick.AddListener(() => OnSlotClicked(index)); //Adds a new listener that calls OnSlotClicked(index).
                Debug.Log($"SaveLoadManager: Added listener to Slot {i + 1}.");
                
                if (resetButtons.Length > i && resetButtons[i] != null)
                {//Attaches reset functionality per slot.
                    resetButtons[i].onClick.RemoveAllListeners();
                    resetButtons[i].onClick.AddListener(() => OnResetClicked(index));
                }

            }
            if (closeButton != null)
            {//Makes the close button exit the Save/Load scene.
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
                    slotLabels[i].text = GameManager.Instance.GetSlotLabel(i);//Updates slot text with current data.
                }
                if (resetButtons.Length > i && resetButtons[i] != null)
                {
                    resetButtons[i].gameObject.SetActive(GameManager.Instance.SaveExists(i));//Only show reset buttons if that slot has data.
                }
            }
        }

        private IEnumerator LoadAndRestore(int slotIndex)
        {
            var save = GameManager.Instance.LoadGame(slotIndex);
            if (save == null)
            {
                PopupManager.Instance.ShowMessage("Failed to load save data.");
                yield break;
            }

            // Restore player character BEFORE scene load
            if (!string.IsNullOrEmpty(save.playerCharacter))
            {
                GameManager.Instance.PlayerChoice = save.playerCharacter;
                GameManager.Instance.SetCanSave(true); // Enable saving since character is already chosen
            }

            GameManager.Instance.ClearSaveLoadSource();
            AsyncOperation unload = SceneManager.UnloadSceneAsync("SaveLoadScene");
            yield return new WaitUntil(() => unload.isDone);

            AsyncOperation load = SceneManager.LoadSceneAsync(save.sceneName, LoadSceneMode.Single);
            yield return new WaitUntil(() => load.isDone);

            // Wait for DialogueManager to initialize
            while (DialogueManager.Instance == null)
                yield return null;

            // Wait for StoryManager to initialize
            StoryManager storyManager = null;
            while (storyManager == null)
            {
                storyManager = FindAnyObjectByType<StoryManager>();
                yield return null;
            }

            // Get the ink asset from StoryManager
            TextAsset inkAsset = storyManager.InkJSONAsset;

            if (inkAsset == null)
            {
                Debug.LogError("StoryManager's InkJSONAsset is null!");
                PopupManager.Instance.ShowMessage("Failed to load story data.");
                yield break;
            }

            // Restore Ink state
            if (!string.IsNullOrEmpty(save.inkState))
            {
                DialogueManager.Instance.LoadInkStory(inkAsset);
                yield return null; // Wait one frame for story to initialize

                DialogueManager.Instance.LoadInkState(save.inkState);

                // Restore current line for visual continuity
                if (!string.IsNullOrEmpty(save.currentLine))
                    DialogueManager.Instance.SetCurrentLine(save.currentLine);
            }

            GameManager.Instance.currentSave = save;
            PopupManager.Instance.ShowMessage($"Loaded Slot {slotIndex + 1}");
        }


        private void OnSlotClicked(int slotIndex)
    {
        if (isSaveMode)
        {
            // --- SAVE MODE ---
            
            //PUT IN NEW GAME LOGIC
            /*bool allFull = GameManager.Instance.GetFirstEmptySlot() == -1;
            bool startingNewGame = GameManager.Instance.IsNewGame;

            // Case 1: All slots full, starting new game (overwrite prompt)
            if (startingNewGame && allFull)
            {
                Debug.Log("A new game and all slots are full");
                PopupManager.Instance.ShowConfirmation(
                    $"All save slots are full! Overwrite Slot {slotIndex + 1} to start a new game?",
                    () =>
                    {
                        GameManager.Instance.SetLastUsedSlot(slotIndex);
                        GameManager.Instance.SetNewGame();
                        GameManager.Instance.ShouldAutoSaveNewGameAfterLoad = true;
                        GameManager.Instance.AutoSaveSlotIndex = slotIndex;
                        GameManager.Instance.StartNewGameTransition(slotIndex);
                    },
                    null
                );
                return;
            }

            // Case 2: Starting new game with at least one empty slot
            if (startingNewGame)
            {
                Debug.Log("Starting new game with at least one empty slot");
                int firstEmpty = GameManager.Instance.GetFirstEmptySlot();
                GameManager.Instance.SetLastUsedSlot(firstEmpty);
                GameManager.Instance.SetNewGame();
                GameManager.Instance.ShouldAutoSaveNewGameAfterLoad = true;
                GameManager.Instance.AutoSaveSlotIndex = firstEmpty;
                GameManager.Instance.StartNewGameTransition(firstEmpty);
                return;
            }*/

            // Case 3: Regular manual save
            if (GameManager.Instance.SaveExists(slotIndex))
            {
                Debug.Log("Regular manual save");
                PopupManager.Instance.ShowConfirmation(
                    $"Slot {slotIndex + 1} already has a save. Overwrite?",
                    () =>
                    {
                        GameManager.Instance.SaveGame(slotIndex);
                        RefreshUI();
                    },
                    () => { RefreshUI(); }
                );
            }
            else
            {
                GameManager.Instance.SaveGame(slotIndex);
                RefreshUI();
            }
        }
        else
        {
            // --- LOAD MODE ---
            if (!GameManager.Instance.SaveExists(slotIndex))
            {
                PopupManager.Instance.ShowMessage($"Slot {slotIndex + 1} is empty.");
                return;
            }

            PopupManager.Instance.ShowConfirmation(
                $"Load from slot {slotIndex + 1}? Current progress will be lost.",
                () => { StartCoroutine(LoadAndRestore(slotIndex)); },
                null
            );
        }
    }

            private void OnResetClicked(int slotIndex)
            {
                // Capture the state of the flags before the slot is reset
                bool wasOpenedFromMainMenu = GameManager.Instance.OpenedFromMainMenu;
                bool wasResetRequired = GameManager.Instance.IsResetForNewGameRequired;

                PopupManager.Instance.ShowConfirmation(
                    $"Reset slot {slotIndex + 1}? This cannot be undone.",
                    () => 
                    {
                        GameManager.Instance.ResetSlot(slotIndex);
                        RefreshUI();
                        
                        if (wasOpenedFromMainMenu && wasResetRequired)
                        {
                            // If we were in main menu new game reset mode, start the new game transition automatically.
                            GameManager.Instance.StartNewGameTransition(slotIndex);
                            return; 
                        }
                    },
                    () => { RefreshUI(); }
                );
            }
            
        private void CloseScene()
        {
            if (GameManager.Instance.OpenedFromPauseMenu)
            { //if pause menu
                PauseMenu pauseMenu = FindAnyObjectByType<PauseMenu>();
                if (pauseMenu != null)
                {
                    pauseMenu.Pause(); //show pause menu
                }
            }
            else if (GameManager.Instance.OpenedFromMainMenu)
            { //if main menu
                MainMenu mainMenu = FindAnyObjectByType<MainMenu>();
                if (mainMenu != null)
                {
                    mainMenu.SetMenuVisible(true);//show main menu
                }
            }
            GameManager.Instance.ClearSaveLoadSource();
            SceneManager.UnloadSceneAsync("SaveLoadScene");//unload scene
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.RefreshUI();// refresh dialogue
            }
        }
        
        public void HighlightSlotForOverwrite(int index)
        {//Marks a slot as the last used. MIGHT IMPLEMENT VISUAL UI HIGHLIGHTING
            GameManager.Instance.SetLastUsedSlot(index);
        }
    }
}