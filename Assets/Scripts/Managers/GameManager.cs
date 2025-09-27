using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;

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

        public float playTimeSeconds = 0f;
        public bool IsPaused { get; private set; }
        public GameSave currentSave;
        public int LastUsedSlotIndex { get; private set; } = -1;

        // Flags for Save/Load Scene Source
        public bool OpenedFromPauseMenu { get; private set; }
        public bool OpenedFromMainMenu { get; private set; }
        public GameObject MainMenuCanvas { get; set; }

        // NEW: Data to pass to the SaveLoadScene
        public bool ShouldOpenSaveLoad { get; private set; } = false;
        public bool IsSaveModeForSaveLoad { get; private set; } = false;

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
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // 1. Logic for a loaded game (Set Dialogue Line)
            if (currentSave != null && currentSave.sceneName == scene.name && scene.name != "MainMenuScene")
            {
                // Only set the line if the DialogueManager exists in the newly loaded scene
                DialogueManager.Instance?.SetLine(currentSave.dialogueIndex);
                currentSave = null; // Clear the save data after applying it to prevent re-applying
            }
            
            PopupManager.Instance?.ForceClosePopup();

            // 2. Logic for initializing the SaveLoadManager (THE FIX)
            if (scene.name == "SaveLoadScene" && mode == LoadSceneMode.Additive && ShouldOpenSaveLoad)
            {
                Debug.Log("GameManager: SaveLoadScene loaded. Attempting to initialize SaveLoadManager.");

                // Use FindAnyObjectByType to ensure we find the newly created manager
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

        public void SetNewGame()
        {
            playTimeSeconds = 0f;
            currentSave = null;
        }

        public void SetPaused(bool paused) => IsPaused = paused;
        public void SetLastUsedSlot(int slotIndex) => LastUsedSlotIndex = slotIndex;

        public int GetFirstEmptySlot()
        {
            for (int i = 0; i < 3; i++)
                if (!SaveExists(i)) return i;
            return -1; // Returns -1 if all are full
        }

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
    }
}