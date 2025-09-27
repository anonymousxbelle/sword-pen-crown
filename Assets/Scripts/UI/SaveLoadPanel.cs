using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace UI
{
    public class SaveLoadPanel : MonoBehaviour
    {
        public static SaveLoadPanel Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private Button[] slotButtons;
        [SerializeField] private TMP_Text[] slotLabels;
        [SerializeField] private Button closeButton;

        // Track which root to show after closing
        private GameObject currentRoot;

        private enum Mode { Save, Load }
        private Mode currentMode;
        private bool openedFromPause;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            panelRoot.SetActive(false);
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(Close);
        }

        public void OpenForSave(GameObject root = null, bool fromPause = false)
        {
            Open(Mode.Save, root, fromPause);
        }

        public void OpenForLoad(GameObject root = null, bool fromPause = false)
        {
            Open(Mode.Load, root, fromPause);
        }

        private void Open(Mode mode, GameObject root, bool fromPause)
        {
            currentMode = mode;
            openedFromPause = fromPause;
            currentRoot = root;

            if (panelRoot != null)
                panelRoot.SetActive(true);

            RefreshSlots();
        }

        public void Close()
        {
            if (panelRoot != null)
                panelRoot.SetActive(false);

            if (currentRoot != null)
                currentRoot.SetActive(true);
        }

        private void RefreshSlots()
        {
            for (int i = 0; i < slotButtons.Length; i++)
            {
                int slotIndex = i;
                string labelText = "Slot " + (slotIndex + 1) + " - Empty";

                if (Managers.GameManager.Instance != null)
                    labelText = Managers.GameManager.Instance.GetSlotLabel(slotIndex);

                slotLabels[slotIndex].text = labelText;
                slotButtons[slotIndex].onClick.RemoveAllListeners();

                if (currentMode == Mode.Save)
                {
                    slotButtons[slotIndex].onClick.AddListener(() =>
                    {
                        if (Managers.GameManager.Instance.SaveExists(slotIndex))
                        {
                            Managers.PopupManager.Instance.ShowConfirmation(
                                $"Overwrite Slot {slotIndex + 1}?",
                                () => SaveToSlot(slotIndex),
                                null
                            );
                        }
                        else
                        {
                            SaveToSlot(slotIndex);
                        }
                    });
                }
                else
                {
                    slotButtons[slotIndex].onClick.AddListener(() =>
                    {
                        var save = Managers.GameManager.Instance.LoadGame(slotIndex);
                        if (save != null)
                        {
                            Managers.GameManager.Instance.SetLastUsedSlot(slotIndex);
                            if (openedFromPause)
                                Managers.GameManager.Instance.SetPaused(false);

                            SceneManager.LoadScene(save.sceneName);
                        }
                    });
                }
            }
        }

        private void SaveToSlot(int slotIndex)
        {
            int dialogueIndex = Managers.DialogueManager.Instance?.GetCurrentLine() ?? 0;
            Managers.GameManager.Instance.SaveGame(slotIndex, dialogueIndex);
            Managers.GameManager.Instance.SetLastUsedSlot(slotIndex);
            RefreshSlots();
        }
    }
}

