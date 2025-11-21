using System;
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
        public static SaveLoadManager Instance{private set; get;}
        [Header("UI References")] 
        [SerializeField] private Button closeButton;
        [SerializeField] private TMP_Text[] slotLabels;
        [SerializeField] private TextAsset inkFile;

        private bool isSaveMode;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        
        private IEnumerator LoadAndRestore(int slotIndex)
        {
            var save = GameManager.Instance.LoadGame(slotIndex);
            if (save == null)
            {
                PopupManager.Instance.ShowMessage("Failed to load save data.");
                yield break;
            }
            
            GameManager.Instance.PlayerChoice = save.playerCharacter;
            GameManager.Instance.SetCanSave(true); // Enable saving since character is already chosen
            
            
            UIManager.Instance.GoToDialogueScreen();
            
            // Restore Ink state
            if (!string.IsNullOrEmpty(save.inkState))
            {
                DialogueManager.Instance.LoadInkStory();
                yield return null; // Wait one frame for story to initialize

                DialogueManager.Instance.LoadInkState(save.inkState);

                // Restore current line for visual continuity
                if (!string.IsNullOrEmpty(save.currentLine))
                    DialogueManager.Instance.SetCurrentLine(save.currentLine);
            }
            
            PopupManager.Instance.ShowMessage($"Loaded Slot {slotIndex + 1}");
        }

        public void SaveGame(int slotIndex)
        {
            if (GameManager.Instance.SaveExists(slotIndex))
            {
                Debug.Log("Regular manual save");
                PopupManager.Instance.ShowConfirmation(
                    $"Slot {slotIndex + 1} already has a save. Overwrite?",
                    () =>
                    {
                        GameManager.Instance.SaveGame(slotIndex);
                        UIManager.Instance.PopulateLabels();
                        PauseMenu.Instance.Resume();
                        UIManager.Instance.GoToDialogueScreen();
                        PopupManager.Instance.ShowMessage("Save Successful");
                    },
                    () =>
                    {
                        
                    }
                );
            }
            else
            {
                GameManager.Instance.SaveGame(slotIndex);
                UIManager.Instance.PopulateLabels();
                PauseMenu.Instance.Resume();
                UIManager.Instance.GoToDialogueScreen();
                PopupManager.Instance.ShowMessage("Save Successful");
            }
        }

        public void LoadGame(int slotIndex)
        {
            if (!GameManager.Instance.SaveExists(slotIndex))
            {
                PopupManager.Instance.ShowMessage($"Slot {slotIndex + 1} is empty.");
                return;
            }

            StartCoroutine(LoadAndRestore(slotIndex)); 
            
        }
        
        public bool AnyLoadGameExists(int numberOfSlots)
        {
            for (int i = 0; i < numberOfSlots; i++)
            {
                if (GameManager.Instance.SaveExists(i))
                {
                    return true;
                }
            }
            return false;
        }
    }
}