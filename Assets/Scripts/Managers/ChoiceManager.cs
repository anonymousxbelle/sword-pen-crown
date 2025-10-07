using UnityEngine;
using UnityEngine.UI;
using System;

public class ChoiceManager : MonoBehaviour
{
    [SerializeField] private Button[] choiceButtons;

    public event Action<int> OnChoiceSelected;

    public void ShowChoices(string[] choices)
    {
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            choiceButtons[i].gameObject.SetActive(i < choices.Length);/*Checks if the current index i is within the number of available choices.
            If it is, the button’s GameObject is made active (visible and clickable).
            If it’s not, the button is deactivated, hiding unused buttons (for instance, if there are only 2 choices but 4 buttons exist).*/
            if (i < choices.Length)
                choiceButtons[i].GetComponentInChildren<TMPro.TMP_Text>().text = choices[i];
        }
    }

    public void SelectChoice(int index) => OnChoiceSelected?.Invoke(index);//f there are any subscribers to OnChoiceSelected, call them and pass along index
}