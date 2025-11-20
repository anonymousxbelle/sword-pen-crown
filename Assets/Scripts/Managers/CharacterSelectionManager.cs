using UnityEngine;
using Ink.Runtime;
using Managers;
using TMPro;
using UnityEngine.UI;

public class CharacterSelectionManager : MonoBehaviour
{
    public static CharacterSelectionManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private TMP_Text headingText;
    [SerializeField] private Button swordButton;
    [SerializeField] private Button penButton;
    [SerializeField] private Button crownButton;
    [SerializeField] private GameObject characterSelectionPanel;

    private Story _story;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Make it persistent
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    
        gameObject.SetActive(false);
    
        if (swordButton != null)
            swordButton.onClick.AddListener(() => OnCharacterChosen(0));
        if (penButton != null)
            penButton.onClick.AddListener(() => OnCharacterChosen(1));
        if (crownButton != null)
            crownButton.onClick.AddListener(() => OnCharacterChosen(2));
    }

    private void Start()
    {
        if (characterSelectionPanel != null && characterSelectionPanel.activeSelf)
        {
            GameManager.Instance.SetCanSave(false);
        }
    }


    // Called by DialogueManager when Ink reaches a character selection point
    public void ShowCharacterChoices(Story story)
    {
        _story = story;

        if (headingText != null)
            headingText.text = "Choose Your Character";
        
        swordButton.GetComponentInChildren<TMP_Text>().text = _story.currentChoices[0].text.Trim();
        penButton.GetComponentInChildren<TMP_Text>().text = _story.currentChoices[1].text.Trim();
        crownButton.GetComponentInChildren<TMP_Text>().text = _story.currentChoices[2].text.Trim();

    }

    // Called when the player clicks a character button
    public void OnCharacterChosen(int index)
    {
        if (_story == null) return;

        _story.ChooseChoiceIndex(index);

        if (_story.canContinue)
            _story.Continue();
        
        characterSelectionPanel.SetActive(false);
        
        DialogueManager.Instance.isCharacterSelection = false;
        
        GameManager.Instance.SetCanSave(true);
        // Continue the story after a choice is made
        DialogueManager.Instance.DisplayNextInkLine();
    }
}