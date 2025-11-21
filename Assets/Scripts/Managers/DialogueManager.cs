using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using Ink.Runtime;


namespace Managers
{
    public class DialogueManager : MonoBehaviour

    {
        public static DialogueManager Instance { get; private set; }
        private bool _isDialogueActive;
        public bool IsDialogueActive => _isDialogueActive;
        public bool isCharacterSelection;


        [Header("UI References")] [SerializeField]
        private TMP_Text speakerText;

        [SerializeField] public TMP_Text dialogueText;
        [SerializeField] private GameObject dialogueBox;
        [SerializeField] private Image characterImage;
        [SerializeField] private Button clickerButton;
        [SerializeField] private GameObject characterSelectionPanel;

        private Story _inkStory;
        [SerializeField] private TextAsset inkJsonAsset;
        public TextAsset InkJsonAsset => inkJsonAsset;


        [System.Serializable]
        public struct DialogueLine
        {
            public string speaker;
            public string text;
            public Sprite characterSprite;
        }

        private DialogueLine[] _dialogueLines;
        private int _currentLine;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded; // listen for new scene
            if (dialogueBox != null)
                dialogueBox.SetActive(false);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "GameSystems") return;

            // Refresh Ink story state if it was active
            if (_inkStory != null && _isDialogueActive)
                RefreshUI();
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public void AssignDialogueUI(GameObject box, TMP_Text dialogue, TMP_Text speaker, Image character,
            Button clicker)
        {
            dialogueBox = box;
            dialogueText = dialogue;
            speakerText = speaker;
            characterImage = character;
            clickerButton = clicker;
            clickerButton.onClick.RemoveAllListeners();
            clickerButton.onClick.AddListener(DisplayNextInkLine);
            DisplayCurrentInkLine();
        }

        //INK METHODS:

        public void LoadInkStory()
        {
            if (inkJsonAsset == null)
            {
                Debug.LogError("Ink JSON asset not assigned!");
                return;
            }

            _inkStory = new Story(inkJsonAsset.text);
            _isDialogueActive = true;

            if (dialogueBox != null) dialogueBox.SetActive(true);

            // Show first line
            DisplayNextInkLine();
        }


        public void DisplayNextInkLine()
        {
            dialogueBox.SetActive(true);

            Debug.Log("DisplayNextInkLine called");

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
                    Debug.Log("Triggering Character Selection UI!");
                    characterSelectionPanel.SetActive(true);
                    CharacterSelectionManager.Instance.ShowCharacterChoices(_inkStory);
                }
                else
                {
                    Debug.Log("Story has choices, showing choice UI");
                    // Show choices through ChoiceManager
                    ShowInkChoices();
                }
            }

            if (_inkStory.canContinue)
            {
                string line = _inkStory.Continue().Trim();
                Debug.Log($"Continuing story, line: {line}");

                // --- Convert Ink formatting (*, **) to TMP rich text ---
                // Replace bold (**text**) with <b>text</b>
                line = System.Text.RegularExpressions.Regex.Replace(line, @"\*\*(.*?)\*\*", "<b>$1</b>");
                // Replace italics (*text*) with <i>text</i>
                line = System.Text.RegularExpressions.Regex.Replace(line, @"\*(.*?)\*", "<i>$1</i>");

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
            if (_inkStory == null || !_isDialogueActive)
            {
                Debug.Log(
                    $"Cannot display next line - _inkStory null: {_inkStory == null}, isActive: {_isDialogueActive}");
                return;
            }

            Debug.Log("DisplayNextInkLine called");


            string line = _inkStory.currentText.Trim();
            Debug.Log($"Continuing story, line: {line}");

            // --- Convert Ink formatting (*, **) to TMP rich text ---
            // Replace bold (**text**) with <b>text</b>
            line = System.Text.RegularExpressions.Regex.Replace(line, @"\*\*(.*?)\*\*", "<b>$1</b>");
            // Replace italics (*text*) with <i>text</i>
            line = System.Text.RegularExpressions.Regex.Replace(line, @"\*(.*?)\*", "<i>$1</i>");

            if (dialogueText != null)
                dialogueText.text = line;

            UpdateInkTags();


        }

        private void UpdateInkTags()
        {
            if (_inkStory.currentTags.Count > 0)
            {
                foreach (string tag in _inkStory.currentTags)
                {
                    string[] splitTag = tag.Split(':');
                    if (splitTag.Length != 2) continue;

                    string tagKey = splitTag[0].Trim();
                    string tagValue = splitTag[1].Trim();

                    switch (tagKey)
                    {
                        case "speaker":
                            if (speakerText != null)
                            {
                                speakerText.text = tagValue;
                                speakerText.gameObject.SetActive(true);
                            }

                            break;

                        case "image":
                            if (characterImage != null)
                            {
                                Sprite sprite = Resources.Load<Sprite>(tagValue);
                                if (sprite != null)
                                {
                                    characterImage.sprite = sprite;
                                    characterImage.gameObject.SetActive(true);
                                }
                                else
                                {
                                    Debug.LogWarning($"Could not load sprite: {tagValue}");
                                }
                            }

                            break;

                        case "hide_speaker":
                            if (speakerText != null)
                                speakerText.gameObject.SetActive(false);
                            break;

                        case "hide_image":
                            if (characterImage != null)
                                characterImage.gameObject.SetActive(false);
                            break;
                    }
                }
            }
            else
            {
                // No tags - hide speaker and image
                if (speakerText != null)
                    speakerText.gameObject.SetActive(false);
                if (characterImage != null)
                    characterImage.gameObject.SetActive(false);
            }
        }

        private void ShowInkChoices()
        {
            // Safety check: make sure there are actually choices
            if (_inkStory.currentChoices.Count == 0)
            {
                Debug.LogWarning("ShowInkChoices called but no choices available");
                return;
            }

            if (ChoiceManager.Instance == null)
            {
                Debug.LogError("ChoiceManager not found! Cannot display choices.");
                return;
            }

            // Hide dialogue box while showing choices
            if (dialogueBox != null)
                dialogueBox.SetActive(false);

            // Pass the story to ChoiceManager
            ChoiceManager.Instance.DisplayChoices(_inkStory, "");

            // Subscribe to choice selection
            ChoiceManager.Instance.OnChoiceSelected += OnInkChoiceSelected;

        }

        private void OnInkChoiceSelected(int choiceIndex)
        {
            Debug.Log($"OnInkChoiceSelected: Choice {choiceIndex} selected");

            // Unsubscribe from the event
            ChoiceManager.Instance.OnChoiceSelected -= OnInkChoiceSelected;

            // Reactivate dialogue
            _isDialogueActive = true;

            // Show dialogue box again
            if (dialogueBox != null)
                dialogueBox.SetActive(true);

            // Continue story after choice (the choice was already made in ChoiceManager)
            Debug.Log("Continuing story after choice");
            DisplayNextInkLine();
        }

        private void EndInkDialogue()
        {
            Debug.Log("Ink story has ended");

            // Check the variable right when story ends
            if (_inkStory != null)
            {
                try
                {
                    object value = _inkStory.variablesState["show_character_selection"];
                    Debug.Log($"show_character_selection value at story end: {value}");
                }
                catch
                {
                    Debug.LogError("show_character_selection variable doesn't exist in Ink story!");
                }
            }

            // DON'T immediately set _isDialogueActive to false
            // Let other systems check variables first
            _isDialogueActive = false;

            // DON'T hide the dialogue box immediately - let StoryManager handle it
            // if (dialogueBox != null)
            //     dialogueBox.SetActive(false);
        }

        public object GetInkVariable(string variableName)
        {
            if (_inkStory != null)
            {
                try
                {
                    return _inkStory.variablesState[variableName];
                }
                catch
                {
                    // Variable doesn't exist
                    return null;
                }
            }

            return null;
        }

        public void SetInkVariable(string variableName, object value)
        {
            if (_inkStory != null)
            {
                _inkStory.variablesState[variableName] = value;
            }
        }

        public void JumpToKnot(string knotName)
        {
            if (_inkStory != null)
            {
                _inkStory.ChoosePathString(knotName);

                // Reactivate dialogue
                _isDialogueActive = true;

                // Show dialogue box
                if (dialogueBox != null)
                    dialogueBox.SetActive(true);

                DisplayNextInkLine();
            }
        }

        public string GetInkState()
        {
            if (_inkStory != null)
            {
                return _inkStory.state.ToJson();
            }

            return null;
        }

        /// <summary>
        /// Load a saved Ink story state from JSON
        /// </summary>
        public void LoadInkState(string stateJson)
        {
            if (_inkStory != null && !string.IsNullOrEmpty(stateJson))
            {
                try
                {
                    _inkStory.state.LoadJson(stateJson);
                    _isDialogueActive = true;

                    if (dialogueBox != null)
                        dialogueBox.SetActive(true);

                    DisplayCurrentInkLine();
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to load Ink state: {e.Message}");
                }
            }
        }

        public string GetCurrentKnot()
        {
            if (_inkStory != null)
            {
                string currentPath = _inkStory.state.currentPathString;
                Debug.Log($"Current Ink path: {currentPath}");

                if (!string.IsNullOrEmpty(currentPath))
                {
                    // Path format is usually "knotName" or "knotName.stitchName"
                    int dotIndex = currentPath.IndexOf('.');
                    if (dotIndex > 0)
                    {
                        string knot = currentPath.Substring(0, dotIndex);
                        Debug.Log($"Extracted knot: {knot}");
                        return knot;
                    }

                    Debug.Log($"Returning full path as knot: {currentPath}");
                    return currentPath;
                }
            }

            return null;
        }

   

        public void RefreshUI()
        {
            if (dialogueBox != null)
                dialogueBox.SetActive(true);

            // For Ink, the state is already loaded, just ensure UI is visible
            else if (_inkStory != null)
            {
                UpdateInkTags();
            }
        }

        public bool InstanceIsNotReady()
        {
            //Used by GameManager.RestoreDialogLineWhenReady() to wait until this manager finishes reconnecting to the new scene’s UI
            return (dialogueText == null || speakerText == null || dialogueBox == null);
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

        //PREVIOUS DIALOGUE METHODS

        /* public void SetDialogue(DialogueLine[] lines)
         {Starts a dialogue with both text and speakers.
             Sets active state, resets to first line, shows UI, and displays the first line.
             _isDialogueActive = true;
             _dialogueLines = lines;
             _currentLine = 0;
             if (dialogueBox != null)
                 dialogueBox.SetActive(true);
             ShowCurrentLine();
         }

         public void SetDialogue(string[] lines)
       { //Same as above, but automatically wraps plain strings into DialogueLine structs with empty speaker fields.
             _isDialogueActive = true;
             _dialogueLines = new DialogueLine[lines.Length];
             for (int i = 0; i < lines.Length; i++)
             {
                 _dialogueLines[i] = new DialogueLine { speaker = "", text = lines[i] };
             }

             _currentLine = 0;
             if (dialogueBox != null)
                 dialogueBox.SetActive(true);
             ShowCurrentLine();

         }

         public void ShowCurrentLine()
         {
             if (dialogueText == null || _dialogueLines == null || _currentLine >= _dialogueLines.Length)
                 return;

             dialogueText.text = _dialogueLines[_currentLine].text;

             if (speakerText != null)
             {
                 if (!string.IsNullOrEmpty(_dialogueLines[_currentLine].speaker))
                 {
                     speakerText.gameObject.SetActive(true);
                     speakerText.text = _dialogueLines[_currentLine].speaker;
                 }
                 else
                 {
                     speakerText.gameObject.SetActive(false);
                 }
             }//Shows or hides the speaker name box depending on whether a speaker exists.

             if (characterImage != null)
             {
                 if (_dialogueLines[_currentLine].characterSprite != null)
                 {
                     characterImage.gameObject.SetActive(true);
                     characterImage.sprite = _dialogueLines[_currentLine].characterSprite;
                 }
                 else
                 {
                     characterImage.gameObject.SetActive(false);

                 }
             } //shows/changes character image depending on who's speaking
         }

         public void NextLine()
         {
             if (_dialogueLines == null) return;

             _currentLine++;

             if (_currentLine >= _dialogueLines.Length)
             {
                 EndDialogue();
             }
             else
             {
                 ShowCurrentLine();
             }
         }

         public void SetLine(int lineIndex)
         { //Lets you jump to a specific dialogue line (used during save/load restoration).
             if (_dialogueLines == null || lineIndex < 0 || lineIndex >= _dialogueLines.Length)
                 return;

             _isDialogueActive = true;
             _currentLine = lineIndex;
             ShowCurrentLine();
         }


         private void EndDialogue()
         { //Resets everything when dialogue ends and hides the dialogue box.
             _dialogueLines = null;
             _currentLine = 0;
             if (dialogueBox != null)
                 dialogueBox.SetActive(false);
             _isDialogueActive = false;
         }


        /* private void UpdateTags()
         {
             if (inkStory.currentTags.Count > 0)
             {
                 foreach (string tag in inkStory.currentTags)
                 {
                     if (tag.StartsWith("speaker:"))
                     {
                         speakerText.text = tag.Replace("speaker:", "").Trim();
                         speakerText.gameObject.SetActive(true);
                     }
                     else if (tag.StartsWith("image:"))
                     {
                         string imageName = tag.Replace("image:", "").Trim();
                         Sprite sprite = Resources.Load<Sprite>(imageName);
                         characterImage.sprite = sprite;
                         characterImage.gameObject.SetActive(sprite != null);
                     }
                 }
             }
             else
             {
                 speakerText.gameObject.SetActive(false);
                 characterImage.gameObject.SetActive(false);
             }
         } */

    }
}
        
    


