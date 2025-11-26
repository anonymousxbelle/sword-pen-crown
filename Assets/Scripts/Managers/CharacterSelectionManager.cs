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

    private Story _story;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void ShowCharacterChoices(Story story)
    {
        _story = story;
        
        if (headingText != null)
            headingText.text = "Choose Your Character";
        
        swordButton.GetComponentInChildren<TMP_Text>().text = _story.currentChoices[0].text;
        penButton.GetComponentInChildren<TMP_Text>().text = _story.currentChoices[1].text;
        penButton.interactable = false;
        crownButton.GetComponentInChildren<TMP_Text>().text = _story.currentChoices[2].text;
        crownButton.interactable = false;

    }
    
    public void OnCharacterChosen(int index)
    {
        if (_story == null) return;

        _story.ChooseChoiceIndex(index);

        if (_story.canContinue)
            _story.Continue();
        
        UIManager.Instance.GoToDialogueScreen();
        
        DialogueManager.Instance.isCharacterSelection = false;
        
        GameManager.Instance.SetCanSave(true);
        DialogueManager.Instance.DisplayNextInkLine();
    }
}