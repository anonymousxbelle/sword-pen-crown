using UnityEngine;
using UI;
using System.Collections;

namespace Managers
{
    public class SaveLoadManager : MonoBehaviour

    {
        public static SaveLoadManager Instance{private set; get;}
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
            
            GameManager.Instance.playerChoice = save.playerCharacter;
            GameManager.Instance.SetCanSave(true);
            
            
            UIManager.Instance.GoToDialogueScreen();
            
            if (!string.IsNullOrEmpty(save.inkState))
            {
                DialogueManager.Instance.LoadInkStory();
                yield return null; 

                DialogueManager.Instance.LoadInkState(save.inkState);
            }
            
            PopupManager.Instance.ShowMessage($"Loaded Slot {slotIndex + 1}");
        }
        private IEnumerator SaveWithPopup(int slotIndex)
        {
            GameManager.Instance.SaveGame(slotIndex);
            UIManager.Instance.PopulateLabels();
            PauseMenu.Instance.Resume();
            UIManager.Instance.GoToDialogueScreen();

            yield return null; // wait 1 frame for UI to update

            PopupManager.Instance.ShowMessage($"Save Successful");
        }


        public void SaveGame(int slotIndex)
        {
            if (GameManager.Instance.SaveExists(slotIndex))
            {
                PopupManager.Instance.ShowConfirmation(
                    $"Slot {slotIndex + 1} already has a save. Overwrite?",
                    () =>
                    {
                        StartCoroutine(SaveWithPopup(slotIndex));
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