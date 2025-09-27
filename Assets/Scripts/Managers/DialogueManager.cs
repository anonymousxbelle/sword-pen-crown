using UnityEngine;
using TMPro;

namespace Managers
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private TMP_Text speakerText; 
        [SerializeField] private TMP_Text dialogueText;
        [SerializeField] private GameObject dialogueBox;

        [System.Serializable]
        public struct DialogueLine
        {
            public string speaker; // empty if narration
            public string text;
        }

        private DialogueLine[] dialogueLines;
        private int currentLine = 0;

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

            if (dialogueBox != null)
                dialogueBox.SetActive(false);
        }

        // Method 1: For speaker + text dialogue
        public void SetDialogue(DialogueLine[] lines)
        {
            dialogueLines = lines;
            currentLine = 0;
            if (dialogueBox != null)
                dialogueBox.SetActive(true);
            ShowCurrentLine();
        }

        // Method 2: For narration-only dialogue
        public void SetDialogue(string[] lines)
        {
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

        private void ShowCurrentLine()
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
        }
        private void Update()
        {
            // *** CRITICAL ADDITION: Check if the game is paused ***
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

