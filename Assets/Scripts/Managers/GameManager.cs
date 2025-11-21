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
        public float playTimeSeconds;
        public string savedAt;
        public string inkState; // Store Ink story state
        public string playerCharacter; // Store selected character
        public string currentKnot; // Store current knot name
        public string currentLine;
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public bool IsNewGame { get; private set; }
        public string PlayerChoice;
        public float playTimeSeconds;
        public bool IsPaused { get; private set; }
        public int LastUsedSlotIndex { get; private set; } = -1;

        // Flags for Save/Load Scene Source
        public bool OpenedFromPauseMenu { get; private set; }
        public bool OpenedFromMainMenu { get; private set; }
        public bool IsResetForNewGameRequired { get; set; } = false;

        public GameObject MainMenuCanvas { get; set; }
        public GameObject PauseMenuCanvas { get; set; }

        // Data to pass to the SaveLoadScene
        public bool ShouldOpenSaveLoad { get; private set; } = false;
        public bool IsSaveModeForSaveLoad { get; private set; } = false;

        public bool ShouldAutoSaveNewGameAfterLoad { get; set; } = false;
        public int AutoSaveSlotIndex { get; set; } = -1; 
        
        private bool _canSave = false;
        public void SetCanSave(bool value) => _canSave = value;

        public bool CanSave => _canSave;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }
        
    
        /*private IEnumerator RestoreGameStateWhenReady()
        {
            // Wait for DialogueManager to exist and be ready
            while (DialogueManager.Instance == null || DialogueManager.Instance.InstanceIsNotReady())
                yield return null;

            // ✅ Restore Ink story if saved
            if (!string.IsNullOrEmpty(currentSave.inkState))
            {
                // Reopen the same Ink file if needed
                DialogueManager.Instance.LoadInkStory(Resources.Load<TextAsset>("Ink/" + currentSave.sceneName));

                // Restore Ink internal state
                DialogueManager.Instance.LoadInkState(currentSave.inkState);

                // Optionally restore last line text (for smooth visual continuity)
                if (!string.IsNullOrEmpty(currentSave.currentLine))
                {
                    DialogueManager.Instance.SetCurrentLine(currentSave.currentLine);
                }
            }

            // Restore player character
            if (!string.IsNullOrEmpty(currentSave.playerCharacter))
                PlayerChoice = currentSave.playerCharacter;
        }*/


        /*private IEnumerator AutoSaveNewGame()
        {
            // Wait a frame to ensure everything is initialized
            yield return null;

            // Wait for DialogueManager to be ready
            while (DialogueManager.Instance == null || DialogueManager.Instance.InstanceIsNotReady())
                yield return null;

            // Save at the start of the story without showing popup
            SaveGame(AutoSaveSlotIndex, false);
            ClearSaveLoadSource();
        }*/

        public void SetNewGame()
        {
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
            return -1;
        }

        public int GetOverwriteSlot() => LastUsedSlotIndex >= 0 ? LastUsedSlotIndex : 0;

        public bool SaveExists(int slotIndex) => File.Exists(GetSavePath(slotIndex));

        public void SaveGame(int slotIndex, bool showPopup = true)
        {
            if (!_canSave)
            {
                Debug.LogWarning("Save blocked: Character not yet selected.");
                if (showPopup && PopupManager.Instance != null)
                    PopupManager.Instance.ShowMessage("Cannot save until a character is selected.");
                return;
            }
            
            string path = GetSavePath(slotIndex);
            Directory.CreateDirectory(Path.GetDirectoryName(path)); // Ensure directory exists
            string tmp = path + ".tmp";

            // Capture Ink state and current knot if available
            string inkStateJson = null;
            string currentKnot = null;
            string currentLine = null; // ✅ NEW

            if (DialogueManager.Instance != null)
            {
                inkStateJson = DialogueManager.Instance.GetInkState();
                currentKnot = DialogueManager.Instance.GetCurrentKnot();
                currentLine = DialogueManager.Instance.GetCurrentLine(); // ✅ NEW: capture currently shown line
            }
            else
            {
                Debug.LogWarning("DialogueManager is null during save!");
            }

            // ✅ Warn if Ink data missing (helpful if saving outside dialogue)
            if (string.IsNullOrEmpty(inkStateJson))
                Debug.LogWarning("Ink state not captured; continuing with partial save.");

            // Build save data
            GameSave save = new GameSave
            {
                sceneName = SceneManager.GetActiveScene().name,
                playTimeSeconds = playTimeSeconds,
                savedAt = DateTime.Now.ToString("MMM dd, yyyy HH:mm"), // ✅ More readable timestamp
                inkState = inkStateJson,
                playerCharacter = PlayerChoice,
                currentKnot = currentKnot,
                currentLine = currentLine // ✅ include new line field
            };

            try
            {
                // Write JSON safely
                File.WriteAllText(tmp, JsonUtility.ToJson(save, true));
                if (File.Exists(path)) File.Delete(path);
                File.Move(tmp, path);

                if (showPopup && PopupManager.Instance != null)
                    PopupManager.Instance.ShowMessage($"Saved to slot {slotIndex + 1}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Save failed: {e.Message}");
                if (showPopup && PopupManager.Instance != null)
                    PopupManager.Instance.ShowMessage($"Save failed: {e.Message}");
            }

            SetLastUsedSlot(slotIndex);
        }



        public GameSave LoadGame(int slotIndex)
        {
            string path = GetSavePath(slotIndex);

            if (!File.Exists(path))
            {
                Debug.LogWarning($"No save found at slot {slotIndex}");
                return null;
            }

            try
            {
                string json = File.ReadAllText(path);
                GameSave save = JsonUtility.FromJson<GameSave>(json);
                SetLastUsedSlot(slotIndex);
                return save;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Load failed: {e.Message}");
                return null;
            }
        }

        
        public void ResetSlot(int slotIndex)
        {
            string path = GetSavePath(slotIndex);
            if (File.Exists(path))
            {
                File.Delete(path);
                if (PopupManager.Instance != null)
                    PopupManager.Instance.ShowMessage($"Slot {slotIndex + 1} reset");
            }
        }
        
        public string GetSlotLabel(int slotIndex)
        {
            string path = GetSavePath(slotIndex);
            if (File.Exists(path))
            {
                try
                {
                    string json = File.ReadAllText(path);
                    GameSave save = JsonUtility.FromJson<GameSave>(json);
                    
                    // Build info string
                    string info = $"Slot {slotIndex + 1}";
                    
                    // Add character if available
                    if (!string.IsNullOrEmpty(save.playerCharacter))
                        info += $" - {save.playerCharacter}";
                    
                    // Add knot name if available (make it readable)
                    if (!string.IsNullOrEmpty(save.currentKnot))
                    {
                        string readableKnot = FormatKnotName(save.currentKnot);
                        info += $" - {readableKnot}";
                    }
                    
                    // Add date on new line
                    info += $"\n{save.savedAt}";
                    
                    return info;
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error reading save slot {slotIndex}: {e.Message}");
                    return $"Slot {slotIndex + 1} - Corrupted Save";
                }
            }
            return $"Slot {slotIndex + 1} - Empty";
        }
        
        private string FormatKnotName(string knotName)
        {
            // Convert "soldier_chapter_1" to "Soldier Chapter 1"
            if (string.IsNullOrEmpty(knotName)) return "";
            
            // Replace underscores with spaces
            string formatted = knotName.Replace("_", " ");
            
            // Capitalize first letter of each word
            string[] words = formatted.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length > 0)
                {
                    words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1);
                }
            }
            
            return string.Join(" ", words);
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
        
        public void StartNewGameTransition(int slotIndex)
        {
            StartCoroutine(NewGameTransitionSequence(slotIndex));
        }

        private IEnumerator NewGameTransitionSequence(int slotIndex)
        {
            if (PopupManager.Instance != null)
                PopupManager.Instance.ForceClosePopup();

            if (OpenedFromMainMenu && MainMenuCanvas != null)
            {
                Debug.Log("GameManager: Forcefully destroying Main Menu canvas before new scene load.");
                Destroy(MainMenuCanvas);
            }
    
            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync("SaveLoadScene");
            yield return new WaitUntil(() => unloadOp.isDone);
    
            SetLastUsedSlot(slotIndex);
            SetNewGame();
            ShouldAutoSaveNewGameAfterLoad = true;
            AutoSaveSlotIndex = slotIndex;
            IsResetForNewGameRequired = false;
    
            Debug.Log("Loading StoryScene now with LoadSceneMode.Single...");
            SceneManager.LoadScene("StoryScene", LoadSceneMode.Single);
        }
        
        
        /*private IEnumerator RestoreGameStateWhenReady()
        {
            // Wait for DialogueManager to exist and be ready
            while (DialogueManager.Instance == null || DialogueManager.Instance.InstanceIsNotReady())
                yield return null;

            // Restore Ink state if it exists
            if (!string.IsNullOrEmpty(currentSave.inkState))
            {
                DialogueManager.Instance.LoadInkState(currentSave.inkState);
            }
            // Fallback to legacy dialogue system
            else if (currentSave.dialogueIndex >= 0)
            {
                DialogueManager.Instance.SetLine(currentSave.dialogueIndex);
                DialogueManager.Instance.RefreshUI();
            }

            // Restore player character choice
            if (!string.IsNullOrEmpty(currentSave.playerCharacter))
            {
                PlayerChoice = currentSave.playerCharacter;
            }
        }*/
    }
}