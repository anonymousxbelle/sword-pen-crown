using UnityEngine;
using System;
using System.IO;

namespace Managers
{
    [Serializable]
    public class GameSave
    {
        public string savedAt;
        public string inkState;
        public string playerCharacter;
        public string currentKnot; 
        public string currentLine;
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public string playerChoice;
        public float playTimeSeconds;
        private bool IsPaused;
        public GameSave currentSave;
        private bool _canSave;
        public void SetCanSave(bool value) => _canSave = value;

        public bool CanSave() => _canSave;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        public void SetPaused(bool paused) => IsPaused = paused;

        public bool SaveExists(int slotIndex) => File.Exists(GetSavePath(slotIndex));

        public void SaveGame(int slotIndex)
        {
            string path = GetSavePath(slotIndex);
            Directory.CreateDirectory(Path.GetDirectoryName(path)); 
            string tmp = path + ".tmp";

            string inkStateJson = DialogueManager.Instance.GetInkSaveData();
            string currentKnot = DialogueManager.Instance.GetCurrentKnot();
            string currentLine = DialogueManager.Instance.GetCurrentLine();

            GameSave save = new GameSave
            {
                savedAt = DateTime.Now.ToString("MMM dd, yyyy HH:mm"),
                inkState = inkStateJson,
                playerCharacter = playerChoice,
                currentKnot = currentKnot,
                currentLine = currentLine 
            };
            
            File.WriteAllText(tmp, JsonUtility.ToJson(save, true));
            if (File.Exists(path)) File.Delete(path);
            File.Move(tmp, path);
            
            PopupManager.Instance.ShowMessage($"Saved to slot {slotIndex + 1}");



            currentSave = save;
        }


        public GameSave LoadGame(int slotIndex)
        {
            string path = GetSavePath(slotIndex);

            if (!File.Exists(path))
            {
                return null;
            }

            string json = File.ReadAllText(path);
            GameSave save = JsonUtility.FromJson<GameSave>(json);
            currentSave = save;
            return save;
        }

        public string GetSlotLabel(int slotIndex)
        {
            string path = GetSavePath(slotIndex);
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                GameSave save = JsonUtility.FromJson<GameSave>(json);

                string info = "";
                if (!string.IsNullOrEmpty(save.playerCharacter))
                    info += $"{save.playerCharacter} - ";

                if (!string.IsNullOrEmpty(save.currentKnot))
                {
                    string readableKnot = FormatKnotName(save.currentKnot);
                    info += $"{readableKnot}";
                }

                return info;

            }

            return "Empty";
        }

        public string GetLastPlayed(int slotIndex)
        {
            string path = GetSavePath(slotIndex);
            if (File.Exists(path))
            {

                string json = File.ReadAllText(path);
                GameSave save = JsonUtility.FromJson<GameSave>(json);

                string lastPlayed = "";
                lastPlayed += $"\n{save.savedAt}";
                return lastPlayed;

            }

            return "";
        }

        private string FormatKnotName(string knotName)
        {
            if (string.IsNullOrEmpty(knotName)) return "";
            string formatted = knotName.Replace("_", " ");
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
            if (!IsPaused && UIManager.Instance.GetActiveScreen() != "TitleScreenCanvas" &&
                UIManager.Instance.GetActiveScreen() != "MainMenuCanvas")
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