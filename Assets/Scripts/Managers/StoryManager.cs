using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace Managers
{
    public class StoryManager : MonoBehaviour
    {
        [Header("Character Buttons")] [SerializeField]
        private Button soldierButton;

        [SerializeField] private Button poetButton;
        [SerializeField] private Button kingButton;
        [SerializeField] private GameObject characterSelectionPanel;

        [Header("Dialogue UI")] [SerializeField]
        private GameObject dialogueBox;

        [SerializeField] private TMP_Text dialogueText;
        [SerializeField] private TMP_Text speakerText;
        [SerializeField] private Image characterImage;
        [SerializeField] private Button clickerButton;

        [Header("Ink Story")] [SerializeField] private TextAsset inkJSONAsset;
        public TextAsset InkJSONAsset => inkJSONAsset;

        // Make this static so it persists across scene reloads
        private static bool hasSelectedCharacter = false;
        private bool hasCheckedForSelection = false;

        private void Start()
        {
            // Hide character selection buttons initially
            //HideCharacterButtons();

            // Assign UI to DialogueManager
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.AssignDialogueUI(
                    dialogueBox,
                    dialogueText,
                    speakerText,
                    characterImage,
                    clickerButton
                );

                // ONLY load the story if there's no active Ink story
                // (i.e., if we're starting fresh, not loading a save)
                if (DialogueManager.Instance.IsUsingInk() && GameManager.Instance.currentSave != null)
                {
                    Debug.Log("Restoring from save - Ink story already loaded");
                    // Re-enable saving if character was already selected
                    if (!string.IsNullOrEmpty(GameManager.Instance.PlayerChoice))
                    {
                        GameManager.Instance.SetCanSave(true);
                    }

                    // Refresh the UI to show current state
                    DialogueManager.Instance.RefreshUI();
                }
                else if (!DialogueManager.Instance.IsUsingInk() && inkJSONAsset != null)
                {
                    Debug.Log("Loading fresh Ink story");
                    DialogueManager.Instance.LoadInkStory(inkJSONAsset);
                }
            }

            // Setup character selection buttons
            /*   if (soldierButton != null)
                   soldierButton.onClick.AddListener(() => SelectCharacter("Soldier"));
               if (poetButton != null)
                   poetButton.onClick.AddListener(() => SelectCharacter("Poet"));
               if (kingButton != null)
                   kingButton.onClick.AddListener(() => SelectCharacter("King"));*/
        }

        /*private void Update()
        {
            // Only check if game is NOT paused
            if (GameManager.Instance != null && !GameManager.Instance.IsPaused &&
                DialogueManager.Instance != null &&
                !DialogueManager.Instance.IsDialogueActive &&
                !hasCheckedForSelection)
            {
                CheckForCharacterSelection();
            }
        }
        private void CheckForCharacterSelection()
        {
            // Don't check if character was already selected
            if (hasSelectedCharacter)
            {
                Debug.Log("Character already selected, skipping check");
                return;
            }

            object showSelection = DialogueManager.Instance.GetInkVariable("show_character_selection");

            Debug.Log($"Checking for character selection: {showSelection}");

            if (showSelection != null && showSelection is bool && (bool)showSelection)
            {
                Debug.Log("Character selection triggered!");
                hasCheckedForSelection = true;
                DialogueManager.Instance.SetInkVariable("show_character_selection", false);
                ShowCharacterSelectionButtons();
            }
        }

        private void HideCharacterButtons()
        {
            if (characterSelectionPanel != null)
                characterSelectionPanel.SetActive(false);
            if (soldierButton != null)
                soldierButton.gameObject.SetActive(false);
            if (poetButton != null)
                poetButton.gameObject.SetActive(false);
            if (kingButton != null)
                kingButton.gameObject.SetActive(false);

            // Show dialogue box when hiding character buttons
            if (dialogueBox != null)
                dialogueBox.SetActive(true);
        }

        private void ShowCharacterSelectionButtons()
        {
            Debug.Log("ShowCharacterSelectionButtons called");

            // Hide the dialogue box
            if (dialogueBox != null)
            {
                dialogueBox.SetActive(false);
                Debug.Log("DialogueBox hidden");
            }
            if (characterSelectionPanel != null) characterSelectionPanel.gameObject.SetActive(true);
            UI.CharacterSelectionUI.Instance.Show();


            // Activate the parent
            if (soldierButton != null && soldierButton.transform.parent != null)
            {
                soldierButton.transform.parent.gameObject.SetActive(true);
                Debug.Log($"Parent '{soldierButton.transform.parent.name}' activated");
            }

            if (soldierButton != null)
            {
                soldierButton.gameObject.SetActive(true);
                Debug.Log($"Soldier button activated. Active: {soldierButton.gameObject.activeSelf}");
            }

            if (poetButton != null)
            {
                poetButton.gameObject.SetActive(true);
                Debug.Log($"Poet button activated. Active: {poetButton.gameObject.activeSelf}");
            }

            if (kingButton != null)
            {
                kingButton.gameObject.SetActive(true);
                Debug.Log($"King button activated. Active: {kingButton.gameObject.activeSelf}");
            }

            Debug.Log("Character selection buttons shown");
        }

        private void SelectCharacter(string character)
        {
            Debug.Log($"SelectCharacter called with: {character}");

            if (GameManager.Instance != null)
            {
                GameManager.Instance.PlayerChoice = character;
                DialogueManager.Instance.SetInkVariable("player_choice", character);
            }

            // Hide the buttons
            HideCharacterButtons();

            // Mark that character has been selected (static, persists across scenes)
            hasSelectedCharacter = true;
            hasCheckedForSelection = true;

            // Jump to the appropriate knot
            switch (character)
            {
                case "Soldier":
                    DialogueManager.Instance.JumpToKnot("soldier_path");
                    break;
                case "Poet":
                    DialogueManager.Instance.JumpToKnot("poet_path");
                    break;
                case "King":
                    DialogueManager.Instance.JumpToKnot("king_path");
                    break;
            }
        }

        private IEnumerator WaitForInkDialogue()
        {
            // Wait until the Ink story finishes
            while (DialogueManager.Instance.IsDialogueActive)
                yield return null;

            // Show character buttons with sprites
            soldierButton.gameObject.SetActive(true);
            poetButton.gameObject.SetActive(true);
            kingButton.gameObject.SetActive(true);

            ShowChoicesAfterDialogue();

        }

        //PREVIOUS METHODS
        private IEnumerator ShowChoicesAfterDialogue()
        {
            // Wait until the dialogue finishes 
            while (DialogueManager.Instance.IsDialogueActive)
            {
                yield return null;
            }

            // Show character choice buttons
            soldierButton.gameObject.SetActive(true);
            poetButton.gameObject.SetActive(true);
            kingButton.gameObject.SetActive(true);
        }

        /*  private void PickCharacter(string character)
          {
              // Save the player’s choice
              GameManager.Instance.PlayerChoice = character;

              // Load the correct scene
              switch (character)
              {
                  case "Soldier":
                      SceneManager.LoadScene("Soldier_Prologue");
                      break;
                  case "Poet":
                      SceneManager.LoadScene("Poet_Prologue");
                      break;
                  case "King":
                      SceneManager.LoadScene("King_Prologue");
                      break;
              }
          }*/

    }
}

