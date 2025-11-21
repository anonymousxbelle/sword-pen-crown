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
    [SerializeField] private GameObject pauseMenuCanvas;
    [SerializeField] private Button saveButton;
    
    [Header("Load Panel")] 
    [SerializeField] private GameObject loadCanvas;
    [SerializeField] private TMP_Text[] loadLabels;
    
    [Header("Save Panel")]
    [SerializeField] private GameObject saveCanvas;
    [SerializeField] private TMP_Text[] saveLabels;
    
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
        if (!SaveLoadManager.Instance.AnyLoadGameExists(loadLabels.Length))
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

    public void GoToMainMenu()
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
        PopulateLabels();
    }

    public void GoToSaveScreen()
    {
        SwitchScreen(saveCanvas);
        PopulateLabels();
    }

    public void ShowPauseMenu()
    {
        if (activeScreen == dialogueCanvas)
        {
            pauseMenuCanvas.SetActive(true);
            saveButton.interactable = GameManager.Instance.CanSave;
        }
    }

    public void HidePauseMenu()
    {
        pauseMenuCanvas.SetActive(false);
    }

    public bool CanPause()
    {
        return activeScreen == dialogueCanvas;
    }

    public void PopulateLabels()
    {
        for (int i = 0; i < loadLabels.Length; i++)
        {
            loadLabels[i].text = GameManager.Instance.GetSlotLabel(i);
            saveLabels[i].text = loadLabels[i].text;
        }
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