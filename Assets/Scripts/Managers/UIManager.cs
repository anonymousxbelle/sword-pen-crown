using System;
using Managers;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIManager : MonoBehaviour
{   
    public static UIManager Instance { get; private set; }

    private GameObject activeScreen;
    
    [Header("Title Screen")] 
    [SerializeField] GameObject titleScreen;
    
    [Header("Main Menu")] 
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private Button loadButton;
    
    
    [Header("Pause Menu")] 
    [SerializeField] private CanvasGroup pauseMenuCanvas;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button showPlaytimeButton;
    [SerializeField] private TMP_Text playtimeText;
    
    [Header("Load Panel")] 
    [SerializeField] private GameObject loadCanvas;
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
    [SerializeField] private GameObject dialogueCanvas;
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

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    void Start()
    {
        SwitchScreen(titleScreen);
        if (!SaveLoadManager.Instance.AnyLoadGameExists(slotButtons.Length))
        {
            loadButton.interactable = false;
        }
    }

    public void SwitchScreen(GameObject newScreen)
    {
        if (activeScreen != null)
        {
            activeScreen.SetActive(false);
        }
        newScreen.SetActive(true);
        activeScreen = newScreen;
    }

    void GoToMainMenu()
    {
        SwitchScreen(mainMenuCanvas);
    }

    public void GoToDialogueScreen()
    {
       SwitchScreen(dialogueCanvas);
    }

    public void GoToLoadScreen()
    {
        SwitchScreen(loadCanvas);
    }

    private void Update()
    {
        if (activeScreen == titleScreen)
        {
            if (Input.anyKeyDown || Input.GetMouseButtonUp(0))
            {
                GoToMainMenu();
            }
        }
    }
}