using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Collections;
namespace Managers
{
    [Serializable]
    public class GameSave
    {
        public string sceneName;
        public int dialogueIndex;
        public float playTimeSeconds;
        public string savedAt;
    }
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public bool IsNewGame { get; private set; } 
        public string PlayerChoice;
        public float playTimeSeconds;
        public bool IsPaused { get; private set; }
        public GameSave currentSave;
        public int LastUsedSlotIndex { get; private set; } = -1;
// Flags for Save/Load Scene Source
        public bool OpenedFromPauseMenu { get; private set; }
        public bool OpenedFromMainMenu { get; private set; }
        public bool IsResetForNewGameRequired { get; set; } = false;

        public GameObject MainMenuCanvas { get; set; }
        public GameObject PauseMenuCanvas { get; set; }
// NEW: Data to pass to the SaveLoadScene
        public bool ShouldOpenSaveLoad { get; private set; } = false;
        public bool IsSaveModeForSaveLoad { get; private set; } = false;
        
        public bool ShouldAutoSaveNewGameAfterLoad { get; set; } = false;
        public int AutoSaveSlotIndex { get; set; } = -1;
        private void Awake()
        { //make the object persistent across scenes
            if (Instance == null) //checks if instance already exists
            {
                Instance = this; // if it doesn't assign this as the instance
                DontDestroyOnLoad(gameObject);
                SceneManager.sceneLoaded += OnSceneLoaded;
                //Subscribes the OnSceneLoaded method to Unity's sceneLoaded event so GameManager can react after scenes finish loading
            }
            else
            {
                Destroy(gameObject); // if instance exists(another game manager exists) then destroy
                return;
            }
        }
        public void SetSaveLoadSource(bool fromPauseMenu, bool fromMainMenu, bool isSaveMode)
        { /* checks whether the game is started from the pause menu or the main menu 
        it also tells it whether to render in save mode or load mode*/
            OpenedFromPauseMenu = fromPauseMenu;
            OpenedFromMainMenu = fromMainMenu;
            ShouldOpenSaveLoad = true;
            IsSaveModeForSaveLoad = isSaveMode;
        }
        
        public void ClearSaveLoadSource()
        { //resets saveload related flags 
            OpenedFromPauseMenu = false;
            OpenedFromMainMenu = false;
            ShouldOpenSaveLoad = false;
            IsSaveModeForSaveLoad = false;
            
            IsResetForNewGameRequired = false;
            ShouldAutoSaveNewGameAfterLoad = false;
            AutoSaveSlotIndex = -1;
        }
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log($"OnSceneLoaded: {scene.name}, ShouldAutoSaveNewGameAfterLoad={ShouldAutoSaveNewGameAfterLoad}, AutoSaveSlotIndex={AutoSaveSlotIndex}");
    
            // 1: Restore dialogue line after loading a saved game
            if (currentSave != null && !IsNewGame)
            {
                // If the saved scene matches the one just loaded, restore dialogue position
                if (scene.name == currentSave.sceneName)
                {
                    StartCoroutine(RestoreDialogueLineWhenReady(currentSave.dialogueIndex));
                }
            }
            // 2. Auto-save new game on CharacterSelectionScene after overwrite flow
            if (scene.name == "CharacterSelectionScene" && ShouldAutoSaveNewGameAfterLoad)
            {
                SaveGame(AutoSaveSlotIndex, 0); // save at dialogue index 0 or start of game
                ClearSaveLoadSource(); // reset necessary flags
            }

            // 3. Force close any popup
            PopupManager.Instance?.ForceClosePopup();

            // 4. Initialize SaveLoadManager if SaveLoadScene loaded additively and ShouldOpenSaveLoad flag set
            if (scene.name == "SaveLoadScene" && mode == LoadSceneMode.Additive && ShouldOpenSaveLoad)
            {
                Debug.Log("GameManager: SaveLoadScene loaded. Attempting to initialize SaveLoadManager.");
                SaveLoadManager manager = FindAnyObjectByType<SaveLoadManager>(); // find saveload manager instance
                if (manager != null)// if found
                {
                    Debug.Log("GameManager: Found SaveLoadManager! Initializing...");
                    manager.Initialize(IsSaveModeForSaveLoad); //initialize it in the correct mode
                }
                else
                {
                    Debug.LogError("GameManager: Failed to find SaveLoadManager in the loaded scene!");
                }
            }
        }

        private IEnumerator RestoreDialogueLineWhenReady(int lineIndex)
        {
            // Wait for DialogueManager to exist and be ready
            while (DialogueManager.Instance == null || DialogueManager.Instance.InstanceIsNotReady())
                yield return null; //yields a frame and tries again(basically waits a period of time then checks again until condition is false)

            DialogueManager.Instance.SetLine(lineIndex);//set dialogue manager to right line
            DialogueManager.Instance.RefreshUI();// refreshes dialogue UI to display right line
        }
        public void SetNewGame()
        {
            currentSave = null; //clear any currently held game save in memory
            IsNewGame = true;
        }

        public void ClearNewGameFlag()
        {
            IsNewGame = false;
        }
        
        public void SetPaused(bool paused) => IsPaused = paused;
        public void SetLastUsedSlot(int slotIndex) => LastUsedSlotIndex = slotIndex;
        public int GetFirstEmptySlot()
        {
            for (int i = 0; i < 3; i++)// checks all slots
                if (!SaveExists(i))
                    return i; // if the saves do not exist(slot is empty) return that
            return -1; // Returns -1 if all are full
        }
        public int GetOverwriteSlot() => LastUsedSlotIndex >= 0 ? LastUsedSlotIndex : 0; /*returns last used slot if available
        if not returns 0*/
        public bool SaveExists(int slotIndex) => File.Exists(GetSavePath(slotIndex)); /*Checks file system for the save file corresponding to slotIndex.
         Uses GetSavePath to compute the path.*/
        public void SaveGame(int slotIndex, int dialogueIndex) //Method to create and write a GameSave JSON file to disk for the specified slot.
        {
            GameSave save = new GameSave
            {
                sceneName = SceneManager.GetActiveScene().name, //Save the name of the currently active scene so it can be restored later.
                dialogueIndex = dialogueIndex,
                playTimeSeconds = playTimeSeconds,
                savedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            string path = GetSavePath(slotIndex);//Compute the final save path for the slot (e.g., persistentDataPath/SaveSlot_0.json).
            string tmp = path + ".tmp";//Temporary path used for safe-write behavior: write to .tmp first then atomically move into place.
            try
            {
                File.WriteAllText(tmp, JsonUtility.ToJson(save, true));//Serialize save to pretty-printed JSON and write it to the temporary file
                if (File.Exists(path)) File.Delete(path);//If an old save file exists, delete it to allow the move.
                File.Move(tmp, path);//Move the temp file to the final path.
                PopupManager.Instance.ShowMessage($"Saved to slot {slotIndex + 1}");
            }
            catch (Exception e)
            {
                PopupManager.Instance.ShowMessage($"Save failed: {e.Message}");
            }
            SetLastUsedSlot(slotIndex);
            currentSave = save; //Keep the saved data in memory so other systems can access it without reloading from disk.
        }
        
        public GameSave LoadGame(int slotIndex)
        {
            string path = GetSavePath(slotIndex);
            if (!File.Exists(path))
            {
                PopupManager.Instance.ShowMessage("This slot is empty");
                return null;
            }
            string json = File.ReadAllText(path);//Read the entire JSON text from disk.(ready everything we saved)
            GameSave save = JsonUtility.FromJson<GameSave>(json); //Deserialize JSON into a GameSave instance
            currentSave = save; //Store the loaded save in memory.
            playTimeSeconds = save.playTimeSeconds;
            SetLastUsedSlot(slotIndex);
            return save;
        }
        public void ResetSlot(int slotIndex)
        { //Method to delete the save file at the given slot (resetting it).
            string path = GetSavePath(slotIndex);
            if (File.Exists(path))
            {
                File.Delete(path);
                PopupManager.Instance.ShowMessage($"Slot {slotIndex + 1} reset");

            }
        }
        public string GetSlotLabel(int slotIndex)
        { //Returns a human-readable label for a save slot
            string path = GetSavePath(slotIndex);
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                GameSave save = JsonUtility.FromJson<GameSave>(json);
                return $"Slot {slotIndex + 1} - {save.sceneName} ({save.savedAt})";
            }
            return $"Slot {slotIndex + 1} - Empty";
        }
        
        private string GetSavePath(int slotIndex) => //returns the filesystem path for the given slot.
            Path.Combine(Application.persistentDataPath, $"SaveSlot_{slotIndex}.json"); /*Application.persistentDataPath in Unity provides a directory path where you can store data that you want to retain between runs.
             This path is platform-specific and ensures that the data is not erased by app updates.*/
        private void Update()
        {
            if (!IsPaused && SceneManager.GetActiveScene().name != "MainMenuScene")
                playTimeSeconds += Time.unscaledDeltaTime;//Only increment play time if the game isn't paused and the current scene isn't the main menu.
        }
        public string GetFormattedPlaytime()
        {
            int hours = Mathf.FloorToInt(playTimeSeconds / 3600f);
            int minutes = Mathf.FloorToInt((playTimeSeconds % 3600f) / 60f);
            return $"{hours}h {minutes}m";
        }
        public void StartNewGameTransition(int slotIndex)
        {
            //triggers the new-game transition.
            StartCoroutine(NewGameTransitionSequence(slotIndex));
        }

        private IEnumerator NewGameTransitionSequence(int slotIndex)
        { //Coroutine that handles the flow for starting a new game after the overwrite flow
            PopupManager.Instance?.ForceClosePopup();

            if (OpenedFromMainMenu && MainMenuCanvas != null)
            {
                Debug.Log("GameManager: Forcefully destroying Main Menu canvas before new scene load.");
                Destroy(MainMenuCanvas); //forcefully close main menu to load game
            }
    
            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync("SaveLoadScene");/*synchronous unload of the SaveLoadScene
            basically runs in the background*/
            yield return new WaitUntil(() => unloadOp.isDone);/*Yield until the unload operation finishes before progressing.
            This ensures the SaveLoad scene is gone before loading the next one.*/
    
            // 1. Set flags for auto-save and new game flow 
            SetLastUsedSlot(slotIndex);
            SetNewGame();
            ShouldAutoSaveNewGameAfterLoad = true;
            AutoSaveSlotIndex = slotIndex;
            IsResetForNewGameRequired = false; //Turn on the flag that will cause an auto-save when the CharacterSelectionScene loads.
    
            Debug.Log("Loading CharacterSelectionScene now with LoadSceneMode.Single...");
            SceneManager.LoadScene("CharacterSelectionScene", LoadSceneMode.Single);
        }
        private void OnDestroy() {
            if (Instance == this) SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        //once the GameManager is destroyed, it’s also detached from scene load notifications.

    }

}