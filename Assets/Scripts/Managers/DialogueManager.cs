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

        
        [System.Serializable]
        public struct DialogueLine
        {
            public string speaker;
            public string text;
        }
        
        private DialogueLine[] dialogueLines;
        private int currentLine = 0;
// --- NEW ---
        [SerializeField] private GameObject choiceButtonPrefab;
        [SerializeField] private GameObject choicePanel; // stores scene-specific choice buttons
        private void Awake()
        {
// Singleton
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



// --- NEW METHOD ---
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
// Skip Gamesystems scene
            if (scene.name == "GameSystems") return;
            dialogueBox = GameObject.FindWithTag("DialogueBox");
            GameObject textObj = GameObject.FindWithTag("DialogueText");
            GameObject speakerObj = GameObject.FindWithTag("SpeakerText");
            
            if (dialogueBox != null)
                dialogueBox.SetActive(false);
            else
                Debug.LogWarning($"DialogueBox not found in {scene.name}");

            
            if (textObj != null)
                dialogueText = textObj.GetComponent<TMP_Text>();
            else
                Debug.LogWarning($"DialogueText not found in {scene.name}");

            
            if (speakerObj != null)
                speakerText = speakerObj.GetComponent<TMP_Text>();
            else
                Debug.LogWarning($"SpeakerText not found in {scene.name}");
            
            if (dialogueLines != null && dialogueLines.Length > 0)
                RefreshUI();
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
// Method 1: For speaker + text dialogue
        public void SetDialogue(DialogueLine[] lines)

        {
            isDialogueActive = true;
            dialogueLines = lines;
            currentLine = 0;
            if (dialogueBox != null)
                dialogueBox.SetActive(true);
            ShowCurrentLine();
        }

// Method 2: For narration-only dialogue
        public void SetDialogue(string[] lines)

        {
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
            }
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
        {
            if (dialogueLines == null || lineIndex < 0 || lineIndex >= dialogueLines.Length)
                return;
            isDialogueActive = true;
            currentLine = lineIndex;
            ShowCurrentLine();
        }
        
        public int GetCurrentLine()
        {
            return currentLine;
        }
        
        private void EndDialogue()
        {
            dialogueLines = null;
            currentLine = 0;
            if (dialogueBox != null)
                dialogueBox.SetActive(false);
            isDialogueActive = false;
        }

        public void ShowChoices(string[] choices, System.Action<int> onChoiceSelected)
        {
            if (choicePanel == null || choiceButtonPrefab == null) return;
// Clear old buttons
            foreach (Transform child in choicePanel.transform)
                Destroy(child.gameObject);
// Create new buttons
            for (int i = 0; i < choices.Length; i++)
            {
                int index = i; // capture for lambda
                GameObject buttonObj = Instantiate(choiceButtonPrefab, choicePanel.transform);
                TMP_Text text = buttonObj.GetComponentInChildren<TMP_Text>();
                text.text = choices[i];
                Button button = buttonObj.GetComponent<Button>();
                button.onClick.AddListener(() =>
                {
                    onChoiceSelected?.Invoke(index);
                    choicePanel.SetActive(false);
                });
            }
            choicePanel.SetActive(true);
        }


        public void RefreshUI()
        {
            if (dialogueBox != null) dialogueBox.SetActive(true);
            ShowCurrentLine();
        }





        public bool InstanceIsNotReady()
        {

// Returns true if not ready yet (so callers can wait while this returns true)
// We check the UI refs — OnSceneLoaded populates them.
            return (dialogueText == null || speakerText == null || dialogueBox == null);
        }
        private void Update()
        {
// Check if the game is paused ***
            if (GameManager.Instance.IsPaused || (PopupManager.Instance != null && PopupManager.Instance.IsPopupActive))
            {
                return; // Do NOT process dialogue input if paused or a popup is open
            }
// Original input processing
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return))

            {
                NextLine();
            }
        }
        
    }

}
