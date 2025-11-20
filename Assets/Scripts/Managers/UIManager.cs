using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIManager : MonoBehaviour
{   [Header("Title Screen")] 
    [SerializeField] GameObject titleScreen;
    
    [Header("Main Menu")] 
    [SerializeField] private CanvasGroup mainMenuCanvas;
    [SerializeField] private Button startNewGameButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button quitButton;
    private bool dismissed = false;
    
    [Header("Pause Menu")] 
    [SerializeField] private CanvasGroup pauseMenuCanvas;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button showPlaytimeButton;
    [SerializeField] private TMP_Text playtimeText;
    
    [Header("Save Load Panel")] 
    [SerializeField] private CanvasGroup saveLoadCanvas;
    [SerializeField] private Button[] slotButtons;
    [SerializeField] private Button[] resetButtons;
    [SerializeField] private Button closeButton;
    [SerializeField] private TMP_Text[] slotLabels;
    [SerializeField] private TextAsset inkFile;
    
    [Header("Pop ups")] 
    [SerializeField] private CanvasGroup popupCanvas;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    
    [Header("Dialogue")] 
    [SerializeField] private CanvasGroup dialogueCanvas;
    [SerializeField] private TMP_Text speakerText;
    [SerializeField] public TMP_Text dialogueText;
    [SerializeField] private Image characterImage;
    [SerializeField] private Button clickerButton;
    
    [Header("Character Selection")] 
    [SerializeField] private GameObject characterSelectionPanel;
    
    [Header("Choices")] 
    [SerializeField] private GameObject choiceCanvas;
    [SerializeField] private TMP_Text headingText;
    [SerializeField] private Button[] choiceButtons;
    
    void Start()
    {
        titleScreen.SetActive(true);
        SetVisible(mainMenuCanvas, false);
        SetVisible(pauseMenuCanvas, false);
        SetVisible(saveLoadCanvas, false);
        SetVisible(dialogueCanvas, false);
        
    }

    void Update()
    {
        if (!dismissed && (Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)))
        {
            HideTitle();
        }
    }

    public void HideTitle()
    {
        dismissed = true;
        titleScreen.SetActive(false);
        SetVisible(mainMenuCanvas, false);

        // Stop checking to save performance
        enabled = false;
    }
    void SetVisible(CanvasGroup obj,  bool active)
    {
        obj.alpha = active ? 1 : 0;
        obj.interactable = active;
        obj.blocksRaycasts = active;
    }

}