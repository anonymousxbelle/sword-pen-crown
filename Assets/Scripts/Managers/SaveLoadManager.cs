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
        { //Handles loading a save and restoring the player’s state.
            GameSave save = GameManager.Instance.LoadGame(slotIndex);//Loads saved data:
            if (save == null || string.IsNullOrEmpty(save.sceneName)) yield break;

            PopupManager.Instance?.ForceClosePopup();

            AsyncOperation op = SceneManager.LoadSceneAsync(save.sceneName); //Loads the saved scene asynchronously:
            yield return new WaitUntil(() => op.isDone);

            GameManager.Instance.SetPaused(false); //unpauses the game
            Time.timeScale = 1f;

            while (DialogueManager.Instance == null || DialogueManager.Instance.InstanceIsNotReady())
                yield return null; //Wait till dialogue manager is initialized

            DialogueManager.Instance.SetLine(save.dialogueIndex);//restores dialogue state
            DialogueManager.Instance.RefreshUI(); //updates UI
            
            //Finds the PauseMenu object in the scene (if it exists) and hides it, ensuring it does not appear after loading.
            PauseMenu pauseMenu = FindAnyObjectByType<PauseMenu>();
            if (pauseMenu != null)
                pauseMenu.HidePauseMenuOnSceneStart();
            /*Checks if the temporary “Save/Load” UI scene is still loaded.
            If it is, unload it so the player only sees the actual gameplay scene.*/
            if (SceneManager.GetSceneByName("SaveLoadScene").isLoaded)
                SceneManager.UnloadSceneAsync("SaveLoadScene");

            if (GameManager.Instance.OpenedFromPauseMenu)
                FindAnyObjectByType<PauseMenu>()?.SetMenuVisible(false);
            else if (GameManager.Instance.OpenedFromMainMenu)
                FindAnyObjectByType<MainMenu>()?.SetMenuVisible(false);

            GameManager.Instance.ClearSaveLoadSource();//final clean up
        }

        
     private void OnSlotClicked(int slotIndex)
            {
                if (isSaveMode)
                {//save mode
                    bool allFull = GameManager.Instance.GetFirstEmptySlot() == -1;//checks if all slots are full
                    bool startingNewGame = GameManager.Instance.IsNewGame;// checks if loading a new game

                    if (startingNewGame && allFull)
                    {//If all slots are full, we can’t auto-save a new game without overwriting.
                        PopupManager.Instance.ShowConfirmation(
                            $"All save slots are full! Overwrite Slot {slotIndex + 1} to start a new game?",
                            () =>
                            {
                                Debug.Log($"Overwrite confirmed for slot {slotIndex + 1}");
                                GameManager.Instance.SetLastUsedSlot(slotIndex);//Sets this slot as the last used.
                                GameManager.Instance.SetNewGame();//Flags that we’re starting a new game.

                                // Set flags to auto-save after character selection scene loads
                                GameManager.Instance.ShouldAutoSaveNewGameAfterLoad = true;
                                GameManager.Instance.AutoSaveSlotIndex = slotIndex;

                                GameManager.Instance.StartNewGameTransition(slotIndex);//Starts the transition to the new game scene.
                            },
                            null
                        );
                        return; // Skip normal save flow since we're overwriting
                    }

                    if (startingNewGame)
                    {//Starting a new game with at least one empty slot
                        int firstEmpty = GameManager.Instance.GetFirstEmptySlot();//chooses the first empty slot.
                        
                        //Sets flags to auto-save and starts the new game
                        GameManager.Instance.SetLastUsedSlot(firstEmpty);
                        GameManager.Instance.SetNewGame();

                        GameManager.Instance.ShouldAutoSaveNewGameAfterLoad = true;
                        GameManager.Instance.AutoSaveSlotIndex = firstEmpty;

                        GameManager.Instance.StartNewGameTransition(slotIndex);
                        return;
                    }

                    // Normal save flow (not a new game)
                    int dialogueIndex = DialogueManager.Instance != null ? DialogueManager.Instance.GetCurrentLine() : 0;
                    /*Gets the current dialogue line index so that the save can restore dialogue progress later.
                     Defaults to 0 if no dialogue manager exists.*/
                    if (GameManager.Instance.SaveExists(slotIndex))
                    {//If the slot is occupied:
                        PopupManager.Instance.ShowConfirmation(
                            $"Slot {slotIndex + 1} already has a save. Overwrite?",
                            () =>
                            {
                                GameManager.Instance.SaveGame(slotIndex, dialogueIndex);
                                RefreshUI();//On confirm, save the game and refresh the UI.
                            },
                            () => { RefreshUI(); }//On cancel, just refresh the UI to reflect no changes
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
                        () => { StartCoroutine(LoadAndRestore(slotIndex)); },//calls LoadAndRestore(slotIndex) to load the game and restore state
                        () => { }
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