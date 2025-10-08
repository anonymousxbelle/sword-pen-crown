using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

namespace Managers
{
    public class ChoiceManager : MonoBehaviour
    {
        public static ChoiceManager Instance { get; private set; }
        
        [Header("UI References")] 
        [SerializeField] private GameObject choiceCanvas;

        [SerializeField] private TMP_Text headingText;
        [SerializeField] private Button[] choiceButtons;
        public event Action<int> OnChoiceSelected;

        private void Awake()
        {
            if (Instance == null){
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else{
                Destroy(gameObject);
            }
            // Hide choices until needed
            if (choiceCanvas != null)
                choiceCanvas.SetActive(false);
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
        }

        private void SelectChoice(int index)
        {
            choiceCanvas.SetActive(false);
            OnChoiceSelected?.Invoke(index);
        }
    }
}