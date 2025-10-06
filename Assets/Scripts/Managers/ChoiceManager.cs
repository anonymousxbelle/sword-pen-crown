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
            choiceButtons[i].gameObject.SetActive(i < choices.Length);
            if (i < choices.Length)
                choiceButtons[i].GetComponentInChildren<TMPro.TMP_Text>().text = choices[i];
        }
    }

    public void SelectChoice(int index) => OnChoiceSelected?.Invoke(index);
}