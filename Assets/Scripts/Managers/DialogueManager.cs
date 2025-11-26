using UnityEngine;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine.UI;
using Ink.Runtime;


namespace Managers
{
    public class DialogueManager : MonoBehaviour

    {
        public static DialogueManager Instance { get; private set; }
        public bool isCharacterSelection;


        [Header("UI References")] [SerializeField]
        private TMP_Text speakerText;

        [SerializeField] public TMP_Text dialogueText;
        [SerializeField] private Image characterImage;

        private Story _inkStory;
        [SerializeField] private TextAsset inkJsonAsset;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void LoadInkStory()
        {

            _inkStory = new Story(inkJsonAsset.text);
            UIManager.Instance.GoToDialogueScreen();
            DisplayNextInkLine();
        }


        public void DisplayNextInkLine()
        {
            if (UIManager.Instance.GetActiveScreen() != "DialogueUI")
            {
                UIManager.Instance.GoToDialogueScreen();
            }

            if (_inkStory.variablesState["show_character_selection"] != null &&
                (bool)_inkStory.variablesState["show_character_selection"])
            {
                isCharacterSelection = true;
            }
            else
            {
                isCharacterSelection = false;
            }

            if (_inkStory.currentChoices.Count > 0)
            {
                if (isCharacterSelection)
                {
                    UIManager.Instance.GoToCharacterSelectionScreen();
                    CharacterSelectionManager.Instance.ShowCharacterChoices(_inkStory);
                }
                else
                {
                    ShowInkChoices();
                }
            }

            else if (_inkStory.canContinue)
            {
                string line = _inkStory.Continue();
                line = FormatLine(line);

                if (dialogueText != null)
                    dialogueText.text = line;

                UpdateInkTags();
            }
            else
            {
                Debug.Log("Story has ended");
                // Story has ended
                EndInkDialogue();
            }
        }

        public void DisplayCurrentInkLine()
        {
            if (UIManager.Instance.GetActiveScreen() != "DialogueUI")
            {
                UIManager.Instance.GoToDialogueScreen();
            }

            string line = _inkStory.currentText;
            line = FormatLine(line);

            if (dialogueText != null)
                dialogueText.text = line;

            UpdateInkTags();


        }

        public string FormatLine(string line)
        {
            line = Regex.Replace(line, @"\*\*(.*?)\*\*", "<b><color=#5A0F14>$1</color></b>");
            line = Regex.Replace(line, @"\*(.*?)\*", "<i><color=#B48A2E>$1</color></i>");

            return line;
        }

        private void UpdateInkTags()
        {
            if (_inkStory.currentTags.Count > 0)
            {
                foreach (string tag in _inkStory.currentTags)
                {
                    string[] splitTag = tag.Split(':');
                    if (splitTag.Length != 2) continue;

                    string tagKey = splitTag[0];
                    string tagValue = splitTag[1];

                    switch (tagKey)
                    {
                        case "speaker":
                            UIManager.Instance.SetSpeaker(tagValue);
                            UIManager.Instance.ShowSpeaker();

                            break;

                        case "image":
                            UIManager.Instance.SetCharacterImage(tagValue);
                            UIManager.Instance.ShowCharacterImage();

                            break;
                    }
                }
            }
            else
            {
                // No tags - hide speaker and image
                UIManager.Instance.HideSpeaker();
                UIManager.Instance.HideCharacterImage();
            }
        }

        private void ShowInkChoices()
        {
            // Safety check: make sure there are actually choices
            if (_inkStory.currentChoices.Count == 0)
            {
                return;
            }

            ChoiceManager.Instance.DisplayChoices(_inkStory);
            ChoiceManager.Instance.OnChoiceSelected += OnInkChoiceSelected;

        }

        private void OnInkChoiceSelected(int choiceIndex)
        {
            ChoiceManager.Instance.OnChoiceSelected -= OnInkChoiceSelected;
            UIManager.Instance.GoToDialogueScreen();
            DisplayNextInkLine();
        }

        private void EndInkDialogue()
        {
            UIManager.Instance.GoToEndScreen();
        }

        public string GetInkSaveData()
        {
            if (_inkStory != null)
            {
                return _inkStory.state.ToJson();
            }

            return null;
        }
        public void LoadInkState(string stateJson)
        {
            if (_inkStory != null && !string.IsNullOrEmpty(stateJson))
            {
                _inkStory.state.LoadJson(stateJson);
                UIManager.Instance.GoToDialogueScreen();
                DisplayCurrentInkLine();
            }
        }

        public string GetCurrentKnot()
        {
            if (_inkStory != null)
            {
                string currentPath = _inkStory.state.currentPathString;
                if (!string.IsNullOrEmpty(currentPath))
                {
                    // Path format is usually "knotName" or "knotName.stitchName"
                    int dotIndex = currentPath.IndexOf('.');
                    if (dotIndex > 0)
                    {
                        string knot = currentPath.Substring(0, dotIndex);
                        return knot;
                    }
                    
                    return currentPath;
                }
            }
            return null;
        }

        public string GetCurrentLine()
        {
            return dialogueText != null ? dialogueText.text : null;
        }

        public void SetCurrentLine(string line)
        {
            if (dialogueText != null)
                dialogueText.text = line;
        }


    }
}
        
    


