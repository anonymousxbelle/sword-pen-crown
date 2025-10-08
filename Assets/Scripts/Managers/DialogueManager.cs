using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

namespace Managers
{
    public class DialogueManager : MonoBehaviour

    {
        public static DialogueManager Instance { get; private set; }
        private bool isDialogueActive = false;
        public bool IsDialogueActive => isDialogueActive; // read-only property for other scripts
        
        [Header("UI References")] [SerializeField]
        private TMP_Text speakerText;
        [SerializeField] private TMP_Text dialogueText;
        [SerializeField] private GameObject dialogueBox;
        [SerializeField] private Image characterImage;
        

        
        [System.Serializable]
        public struct DialogueLine
        {
            public string speaker;
            public string text;
            public Sprite characterSprite; // new field for character image
        }
        
        private DialogueLine[] dialogueLines;
        private int currentLine = 0;

        [SerializeField] private GameObject choiceButtonPrefab;
        [SerializeField] private GameObject choicePanel; // stores scene-specific choice buttons
        private void Awake()
        { //Ensures one consistent dialogue manager across all scenes and prevents UI from lingering on start.
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; // listen for new scene
            if (dialogueBox != null)
                dialogueBox.SetActive(false);
        }
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "GameSystems") return;  //Skips setup for your backend “GameSystems” scene where all managers live
            dialogueBox = GameObject.FindWithTag("DialogueBox");
            GameObject textObj = GameObject.FindWithTag("DialogueText");
            GameObject speakerObj = GameObject.FindWithTag("SpeakerText");
            //Finds the UI elements in the new scene by tag.
            if (dialogueBox != null)
                dialogueBox.SetActive(false);//Hides the dialogue UI
            else
                Debug.LogWarning($"DialogueBox not found in {scene.name}");

            
            if (textObj != null)
                dialogueText = textObj.GetComponent<TMP_Text>(); //Assigns references dynamically.
            else
                Debug.LogWarning($"DialogueText not found in {scene.name}");

            
            if (speakerObj != null) //Assigns references dynamically.
                speakerText = speakerObj.GetComponent<TMP_Text>();
            else
                Debug.LogWarning($"SpeakerText not found in {scene.name}");
            
            if (dialogueLines != null && dialogueLines.Length > 0)
                RefreshUI();//If a dialogue was already in progress (e.g., after loading a save), it reinitializes the UI.
        }
        
        public void AssignDialogueUI(GameObject box, TMP_Text dialogue, TMP_Text speaker)
        {
            dialogueBox = box;
            dialogueText = dialogue;
            speakerText = speaker;
        }
        
        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        public void SetDialogue(DialogueLine[] lines)
        { /*Starts a dialogue with both text and speakers.
            Sets active state, resets to first line, shows UI, and displays the first line.*/
            isDialogueActive = true;
            dialogueLines = lines;
            currentLine = 0;
            if (dialogueBox != null)
                dialogueBox.SetActive(true);
            ShowCurrentLine();
        }
        
        public void SetDialogue(string[] lines)
        { //Same as above, but automatically wraps plain strings into DialogueLine structs with empty speaker fields.
            isDialogueActive = true;
            dialogueLines = new DialogueLine[lines.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                dialogueLines[i] = new DialogueLine { speaker = "", text = lines[i] };
            }

            currentLine = 0;
            if (dialogueBox != null)
                dialogueBox.SetActive(true);
            ShowCurrentLine();

        }

        public void ShowCurrentLine()
        {
            if (dialogueText == null || dialogueLines == null || currentLine >= dialogueLines.Length)
                return;
            
            dialogueText.text = dialogueLines[currentLine].text;
            
            if (speakerText != null)
            {
                if (!string.IsNullOrEmpty(dialogueLines[currentLine].speaker))
                {
                    speakerText.gameObject.SetActive(true);
                    speakerText.text = dialogueLines[currentLine].speaker;
                }
                else
                {
                    speakerText.gameObject.SetActive(false);
                }
            }//Shows or hides the speaker name box depending on whether a speaker exists.

            if (characterImage != null)
            {
                if (dialogueLines[currentLine].characterSprite != null)
                {
                    characterImage.gameObject.SetActive(true);
                    characterImage.sprite = dialogueLines[currentLine].characterSprite;
                }
                else
                {
                    characterImage.gameObject.SetActive(false);
                }
            } //shows/changes character image depending on who's speaking
        }

        public void NextLine()
        {
            if (dialogueLines == null) return;
            
            currentLine++;
            
            if (currentLine >= dialogueLines.Length)
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
            if (dialogueLines == null || lineIndex < 0 || lineIndex >= dialogueLines.Length)
                return;
            
            isDialogueActive = true;
            currentLine = lineIndex;
            ShowCurrentLine();
        }
        
        public int GetCurrentLine()
        { //Returns the current line index (for saving game progress).
            return currentLine;
        }
        
        private void EndDialogue()
        { //Resets everything when dialogue ends and hides the dialogue box.
            dialogueLines = null;
            currentLine = 0;
            if (dialogueBox != null)
                dialogueBox.SetActive(false);
            isDialogueActive = false;
        }
        public void ShowChoices(string[] choices, System.Action<int> onChoiceSelected)
        {
            if (choicePanel == null || choiceButtonPrefab == null) return;
      
            foreach (Transform child in choicePanel.transform)//  // Clear old buttons
                Destroy(child.gameObject);
        
            for (int i = 0; i < choices.Length; i++)// Create new buttons
            {
                int index = i; // capture for lambda
                GameObject buttonObj = Instantiate(choiceButtonPrefab, choicePanel.transform);
                TMP_Text text = buttonObj.GetComponentInChildren<TMP_Text>();
                text.text = choices[i];
                Button button = buttonObj.GetComponent<Button>();
                button.onClick.AddListener(() =>//adds a function to run when the button is clicked.
                {
                    onChoiceSelected?.Invoke(index);//calls the callback passed in from outside, giving it the index of the choice the player made.
                    choicePanel.SetActive(false);
                });
            }
            choicePanel.SetActive(true);
        }


        public void RefreshUI()
        { //Re-shows dialogue box and restores current line
            if (dialogueBox != null) dialogueBox.SetActive(true);
            ShowCurrentLine();
        }
        public bool InstanceIsNotReady()
        {
            //Used by GameManager.RestoreDialogLineWhenReady() to wait until this manager finishes reconnecting to the new scene’s UI
            return (dialogueText == null || speakerText == null || dialogueBox == null);
        }
        
        private void Update()
        {
        // Check if the game is paused
            if (GameManager.Instance.IsPaused || (PopupManager.Instance != null && PopupManager.Instance.IsPopupActive))
            {
                return; // Do NOT process dialogue input if paused or a popup is open
            }
        //input processing
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return))
            {
                NextLine();
            }
        }
        
    }

}
