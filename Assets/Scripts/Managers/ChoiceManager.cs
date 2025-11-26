using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using Ink.Runtime;
using System.Collections.Generic;

namespace Managers
{
    public class ChoiceManager : MonoBehaviour
    {
        public static ChoiceManager Instance { get; private set; }

        [Header("UI References")] [SerializeField]
        private GameObject choiceCanvas;

        [SerializeField] private TMP_Text headingText;
        [SerializeField] private Button[] choiceButtons;
        public event Action<int> OnChoiceSelected;
        private Story _currentStory;

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

            // Hide choices until needed
            //if (choiceCanvas != null)
               // choiceCanvas.SetActive(false);
        }
        
        public void DisplayChoices(Story story)
        {
            if (story == null || story.currentChoices.Count == 0)
            {
                return;
            }

            _currentStory = story;
            
            if (headingText != null)
                headingText.text = DialogueManager.Instance.GetCurrentLine();
            
            UIManager.Instance.GoToChoiceScreen();

            List<Choice> choices = story.currentChoices;

            for (int i = 0; i < choiceButtons.Length; i++)
            { 
                if (i < choices.Count)
                {
                    choiceButtons[i].gameObject.SetActive(true);
                    
                    TMP_Text buttonText = choiceButtons[i].GetComponentInChildren<TMP_Text>();
                    
                    if (buttonText != null)
                        buttonText.text = choices[i].text;

                    int choiceIndex = i;
                    choiceButtons[i].onClick.RemoveAllListeners();
                    choiceButtons[i].onClick.AddListener(() => MakeInkChoice(choiceIndex));
                }
                else
                {
                    choiceButtons[i].gameObject.SetActive(false);
                }
            }
        }

        private void MakeInkChoice(int choiceIndex)
        {
            choiceCanvas.SetActive(false);
            
            if (_currentStory != null)
            {
                _currentStory.ChooseChoiceIndex(choiceIndex);
            }
            
            OnChoiceSelected?.Invoke(choiceIndex);
        }

    }

}