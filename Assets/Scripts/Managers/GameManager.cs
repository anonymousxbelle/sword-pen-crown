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
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                SceneManager.sceneLoaded += OnSceneLoaded;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }
        public void SetSaveLoadSource(bool fromPauseMenu, bool fromMainMenu, bool isSaveMode)
        {
            OpenedFromPauseMenu = fromPauseMenu;
            OpenedFromMainMenu = fromMainMenu;
            ShouldOpenSaveLoad = true;
            IsSaveModeForSaveLoad = isSaveMode;
        }
        
        public void ClearSaveLoadSource()
        {
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
    
            // ... (Section 1: Restore dialogue line after loading a saved game) ...

            // 2. Auto-save new game on CharacterSelectionScene after overwrite flow
            if (scene.name == "CharacterSelectionScene" && ShouldAutoSaveNewGameAfterLoad)
            {
                SaveGame(AutoSaveSlotIndex, 0); // save at dialogue index 0 or start of game
        
                // *** ADD THE FLAG CLEANUP HERE, AFTER THE SAVE IS DONE ***
                ShouldAutoSaveNewGameAfterLoad = false;
                AutoSaveSlotIndex = -1;
                ClearSaveLoadSource(); // <--- FINAL CLEANUP AFTER SUCCESSFUL SAVE
            }

            // 3. Force close any popup
            PopupManager.Instance?.ForceClosePopup();

            // 4. Initialize SaveLoadManager if SaveLoadScene loaded additively and ShouldOpenSaveLoad flag set
            if (scene.name == "SaveLoadScene" && mode == LoadSceneMode.Additive && ShouldOpenSaveLoad)
            {
                Debug.Log("GameManager: SaveLoadScene loaded. Attempting to initialize SaveLoadManager.");
                SaveLoadManager manager = FindAnyObjectByType<SaveLoadManager>();
                if (manager != null)
                {
                    Debug.Log("GameManager: Found SaveLoadManager! Initializing...");
                    manager.Initialize(IsSaveModeForSaveLoad);
                }
                else
                {
                    Debug.LogError("GameManager: Failed to find SaveLoadManager in the loaded scene!");
                }
            }
        }

        private IEnumerator RestoreDialogLineWhenReady(int lineIndex)
        {
            // Wait for DialogueManager to exist and be ready
            while (DialogueManager.Instance == null || DialogueManager.Instance.InstanceIsNotReady())
                yield return null;

            DialogueManager.Instance.SetLine(lineIndex);
            DialogueManager.Instance.RefreshUI();
        }
        public void SetNewGame()
        {
            currentSave = null;
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
            for (int i = 0; i < 3; i++)
                if (!SaveExists(i))
                    return i;
            return -1; // Returns -1 if all are full
        }
        public int GetOverwriteSlot() => LastUsedSlotIndex >= 0 ? LastUsedSlotIndex : 0;
        public bool SaveExists(int slotIndex) => File.Exists(GetSavePath(slotIndex));
        public void SaveGame(int slotIndex, int dialogueIndex)
        {
            GameSave save = new GameSave
            {
                sceneName = SceneManager.GetActiveScene().name,
                dialogueIndex = dialogueIndex,
                playTimeSeconds = playTimeSeconds,
                savedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            string path = GetSavePath(slotIndex);
            string tmp = path + ".tmp";
            try
            {
                File.WriteAllText(tmp, JsonUtility.ToJson(save, true));
                if (File.Exists(path)) File.Delete(path);
                File.Move(tmp, path);
                PopupManager.Instance.ShowMessage($"Saved to slot {slotIndex + 1}");
            }
            catch (Exception e)
            {
                PopupManager.Instance.ShowMessage($"Save failed: {e.Message}");
            }
            SetLastUsedSlot(slotIndex);
            currentSave = save;
        }
        
        public GameSave LoadGame(int slotIndex)
        {
            string path = GetSavePath(slotIndex);
            if (!File.Exists(path))
            {
                PopupManager.Instance.ShowMessage("This slot is empty");
                return null;
            }
            string json = File.ReadAllText(path);
            GameSave save = JsonUtility.FromJson<GameSave>(json);
            currentSave = save;
            playTimeSeconds = save.playTimeSeconds;
            SetLastUsedSlot(slotIndex);
            return save;
        }
        public void ResetSlot(int slotIndex)
        {
            string path = GetSavePath(slotIndex);
            if (File.Exists(path))
            {
                File.Delete(path);
                PopupManager.Instance.ShowMessage($"Slot {slotIndex + 1} reset");

            }
        }
        public string GetSlotLabel(int slotIndex)
        {
            string path = GetSavePath(slotIndex);
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                GameSave save = JsonUtility.FromJson<GameSave>(json);
                return $"Slot {slotIndex + 1} - {save.sceneName} ({save.savedAt})";
            }
            return $"Slot {slotIndex + 1} - Empty";
        }
        
        private string GetSavePath(int slotIndex) =>
            Path.Combine(Application.persistentDataPath, $"SaveSlot_{slotIndex}.json");
        private void Update()
        {
            if (!IsPaused && SceneManager.GetActiveScene().name != "MainMenuScene")
                playTimeSeconds += Time.unscaledDeltaTime;
        }
        public string GetFormattedPlaytime()
        {
            int hours = Mathf.FloorToInt(playTimeSeconds / 3600f);
            int minutes = Mathf.FloorToInt((playTimeSeconds % 3600f) / 60f);
            return $"{hours}h {minutes}m";
        }
        
        // GameManager.cs (Add this to the GameManager script)

        public void StartNewGameTransition(int slotIndex)
        {
            // Use this persistent instance to start the coroutine
            StartCoroutine(NewGameTransitionSequence(slotIndex));
        }

        // GameManager.cs (Modified NewGameTransitionSequence)

        private IEnumerator NewGameTransitionSequence(int slotIndex)
        {
            PopupManager.Instance?.ForceClosePopup();

            if (OpenedFromMainMenu && MainMenuCanvas != null)
            {
                Debug.Log("GameManager: Forcefully destroying Main Menu canvas before new scene load.");
                Destroy(MainMenuCanvas);
            }
    
            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync("SaveLoadScene");
            yield return new WaitUntil(() => unloadOp.isDone);
    
            // 1. Set flags for auto-save and new game flow (KEEP THESE)
            SetLastUsedSlot(slotIndex);
            SetNewGame();
            ShouldAutoSaveNewGameAfterLoad = true;
            AutoSaveSlotIndex = slotIndex;
            IsResetForNewGameRequired = false; // Clear this specific flag.

            // 2. *** DELETE: ClearSaveLoadSource() CALL WAS HERE ***
    
            Debug.Log("Loading CharacterSelectionScene now with LoadSceneMode.Single...");
            SceneManager.LoadScene("CharacterSelectionScene", LoadSceneMode.Single);
        }

    }

}