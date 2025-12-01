using Managers;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;


public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private GameObject activeScreen;

    [Header("Title Screen")] [SerializeField]
    GameObject titleScreen;

    [Header("Main Menu")] [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private Button loadButton;

    [Header("Pause Menu")] [SerializeField]
    private GameObject pauseMenuCanvas;

    [SerializeField] private Button saveButton;

    [Header("Load Panel")] [SerializeField]
    private GameObject loadCanvas;

    [SerializeField] private TMP_Text[] loadLabels;
    [SerializeField] private TMP_Text[] loadSlotLastPlayed;
    [SerializeField] private GameObject[] activeLoadOutline;
    [SerializeField] private GameObject[] inactiveLoadOutline;
    [SerializeField] private GameObject[] loadSlotSceneOutline;

    [Header("Save Panel")] [SerializeField]
    private GameObject saveCanvas;

    [SerializeField] private TMP_Text[] saveLabels;
    [SerializeField] private TMP_Text[] saveSlotLastPlayed;
    [SerializeField] private GameObject[] activeSaveOutline;
    [SerializeField] private GameObject[] inactiveSaveOutline;
    [SerializeField] private GameObject[] saveSlotSceneOutline;

    [Header("Dialogue")] [SerializeField] private GameObject dialogueCanvas;
    [SerializeField] private GameObject speakerBox;
    [SerializeField] private TMP_Text speakerText;
    [SerializeField] private Image characterImage;

    [Header("Character Selection")] [SerializeField]
    private GameObject characterSelectionCanvas;

    [Header("Choices")] [SerializeField] private GameObject choiceCanvas;

    [Header("EndScreen")] [SerializeField] private GameObject endScreen;
    
    [Header("PopUp")]
    [SerializeField] private GameObject popUpCanvas;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button confirmButton;

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

    public void ShowSpeaker()
    {
        speakerBox.SetActive(true);
    }

    public void HideSpeaker()
    {
        speakerBox.SetActive(false);
    }

    public void SetSpeaker(string speakerName)
    {
        speakerText.text = speakerName;
    }

    public void ShowCharacterImage()
    {
        characterImage.gameObject.SetActive(true);
    }

    public void HideCharacterImage()
    {
        characterImage.gameObject.SetActive(false);
    }

    public void SetCharacterImage(string spritePath)
    {
        Sprite sprite = Resources.Load<Sprite>(spritePath);
        characterImage.sprite = sprite;
    }
    
    public void GoToLoadScreen()
    {
        SwitchScreen(loadCanvas);
        PopulateLabels();
        ChooseLoadSlotOutline();
    }

    public void GoToSaveScreen()
    {
        SwitchScreen(saveCanvas);
        PopulateLabels();
        ChooseSaveSlotOutline();

    }

    public void GoToCharacterSelectionScreen()
    {
        SwitchScreen(characterSelectionCanvas);
    }

    public void GoToChoiceScreen()
    {
        SwitchScreen(choiceCanvas);
    }

    public void GoToEndScreen()
    {
        SwitchScreen(endScreen);
    }

    public void ShowPauseMenu()
    {
        if (activeScreen == dialogueCanvas)
        {
            pauseMenuCanvas.SetActive(true);
            saveButton.interactable = GameManager.Instance.CanSave();
        }
    }

    public void HidePauseMenu()
    {
        pauseMenuCanvas.SetActive(false);
    }

    public void ShowPopUp()
    {
        popUpCanvas.SetActive(true);
    }

    public void HidePopUp()
    {
        popUpCanvas.SetActive(false);
    }

    public void HidePopUpCancelButton()
    {
        cancelButton.gameObject.SetActive(false);
    }

    public void ShowPopUpCancelButton()
    {
        cancelButton.gameObject.SetActive(true);
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
            loadSlotLastPlayed[i].text = GameManager.Instance.GetLastPlayed(i);
            saveSlotLastPlayed[i].text = loadSlotLastPlayed[i].text;
        }
    }

    public void ChooseLoadSlotOutline()
    {
        for (int i = 0; i < loadLabels.Length; i++)
            if (loadLabels[i].text == "Empty")
            {
                activeLoadOutline[i].SetActive(false);
                inactiveLoadOutline[i].SetActive(true);
                loadSlotSceneOutline[i].SetActive(false);
            }
            else
            {
                activeLoadOutline[i].SetActive(true);
                inactiveLoadOutline[i].SetActive(false);
                loadSlotSceneOutline[i].SetActive(true);
            }
    }

    public void ChooseSaveSlotOutline()
    {
        for (int i = 0; i < loadLabels.Length; i++)
            if (loadLabels[i].text == "Empty")
            {
                activeSaveOutline[i].SetActive(false);
                inactiveSaveOutline[i].SetActive(true);
                saveSlotSceneOutline[i].SetActive(false);
            }
            else
            {
                activeSaveOutline[i].SetActive(true);
                inactiveSaveOutline[i].SetActive(false);
                saveSlotSceneOutline[i].SetActive(true);
            }
    }

    public string GetActiveScreen()
    {
        return activeScreen.name;
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