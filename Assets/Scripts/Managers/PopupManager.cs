using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace Managers
{
   public class PopupManager : MonoBehaviour

   {

      public static PopupManager Instance { get; private set; }


// NEW: Public property to check if the popup is active

      public bool IsPopupActive => popupRoot != null && popupRoot.activeInHierarchy;



      [Header("UI References")] [SerializeField]
      private GameObject popupRoot;

      [SerializeField] private TMP_Text messageText;

      [SerializeField] private Button confirmButton;

      [SerializeField] private Button cancelButton;



      private Action confirmAction;

      private Action cancelAction;



      private void Awake()

      {

         if (Instance != null && Instance != this)
         {
            Destroy(gameObject);
            return;
         }

         Instance = this;

         DontDestroyOnLoad(gameObject);



         if (popupRoot != null) popupRoot.SetActive(false);



         confirmButton.onClick.RemoveAllListeners();

         confirmButton.onClick.AddListener(OnConfirm);



         cancelButton.onClick.RemoveAllListeners();

         cancelButton.onClick.AddListener(OnCancel);

      }



      public void ShowMessage(string message)

      {

         if (popupRoot == null) return;



         messageText.text = message;

         confirmButton.gameObject.SetActive(true);

         cancelButton.gameObject.SetActive(false);

         confirmAction = null;

         cancelAction = null;

         popupRoot.SetActive(true);

      }



      public void ShowConfirmation(string message, Action onConfirm, Action onCancel)

      {

         if (popupRoot == null) return;



         messageText.text = message;

         confirmAction = onConfirm;

         cancelAction = onCancel;



         confirmButton.gameObject.SetActive(true);

         cancelButton.gameObject.SetActive(true);



         popupRoot.SetActive(true);

      }



// NEW: Public method to allow other managers to force close the popup

      public void ForceClosePopup()

      {

         ClearActions();

      }



      private void OnConfirm()

      {

// Execute action first

         confirmAction?.Invoke();

// Then clear and hide

         ClearActions();

      }



      private void OnCancel()

      {

// Execute action first

         cancelAction?.Invoke();

// Then clear and hide

         ClearActions();

      }



      private void ClearActions()

      {

         confirmAction = null;

         cancelAction = null;

         if (popupRoot != null)

         {

            popupRoot.SetActive(false);

         }

      }

   }

}