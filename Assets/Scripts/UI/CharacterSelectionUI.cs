using UnityEngine;

namespace UI
{
    public class CharacterSelectionUI : MonoBehaviour
    {
        public static CharacterSelectionUI Instance { get; private set; }

        private void Awake()
        {
            Instance = this;

            // Hide by default
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}