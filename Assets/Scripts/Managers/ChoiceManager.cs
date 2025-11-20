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
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            // Hide choices until needed
            if (choiceCanvas != null)
                choiceCanvas.SetActive(false);
        }

        //INK METHODS
        public void DisplayChoices(Story story, string heading = "")
        {
            // don't show empty choices
            if (story == null || story.currentChoices.Count == 0)
            {
                choiceCanvas.SetActive(false);
                return;
            }

            _currentStory = story;
            
            if (headingText != null)
                headingText.text = heading;
            
            choiceCanvas.SetActive(true);

            List<Choice> choices = story.currentChoices;

            for (int i = 0; i < choiceButtons.Length; i++)
            { //create choice buttons
                if (i < choices.Count)
                {
                    choiceButtons[i].gameObject.SetActive(true);
                    
                    TMP_Text buttonText = choiceButtons[i].GetComponentInChildren<TMP_Text>();
                    
                    if (buttonText != null)
                        buttonText.text = choices[i].text.Trim();

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
        
        //OLDER METHODS
        
        /*private void SelectChoice(int index)
        {
            choiceCanvas.SetActive(false);
            OnChoiceSelected?.Invoke(index);
        }
        
        public void ShowChoices(string heading, string[] choices, Action<int> callback)
        {
            headingText.text = heading;
            OnChoiceSelected = callback;

            choiceCanvas.SetActive(true);

            for (int i = 0; i < choiceButtons.Length; i++)
            {
                if (i < choices.Length)
                {
                    choiceButtons[i].gameObject.SetActive(true);
                    choiceButtons[i].GetComponentInChildren<TMP_Text>().text = choices[i];
                    int index = i;
                    choiceButtons[i].onClick.RemoveAllListeners();
                    choiceButtons[i].onClick.AddListener(() => SelectChoice(index));
                }
                else
                {
                    choiceButtons[i].gameObject.SetActive(false);
                }
            }
        }*/

    }

}