using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace Managers
{
   public class PopupManager : MonoBehaviour
   {
      public static PopupManager Instance { get; private set; }
      // public bool IsPopupActive => popupRoot != null && popupRoot.activeInHierarchy;

      [Header("UI References")]
      //[SerializeField] private GameObject popupRoot;
      [SerializeField]
      private TMP_Text messageText;

      [SerializeField] private Button confirmButton;
      [SerializeField] private Button cancelButton;

      private Action confirmAction;
      private Action cancelAction;

      void Awake()
      {
         if (Instance != null && Instance != this)
         {
            Destroy(gameObject);
            return;
         }

         Instance = this;
      }

      public void ShowMessage(string message)
      {
         messageText.text = message;
         UIManager.Instance.HidePopUpCancelButton();
         UIManager.Instance.ShowPopUp();
      }

      public void ShowConfirmation(string message, Action onConfirm, Action onCancel)
      {
         messageText.text = message;
         confirmAction = onConfirm;
         cancelAction = onCancel;
         UIManager.Instance.ShowPopUpCancelButton();
         UIManager.Instance.ShowPopUp();

      }
      
      public void OnConfirm()
      {
         // Execute action first
         confirmAction?.Invoke();
         // Then clear and hide
         ClearActions();
      }
      
      public void OnCancel()
      {
         cancelAction?.Invoke();
         ClearActions();
      }

      private void ClearActions()
      {
         confirmAction = null;
         cancelAction = null;

         UIManager.Instance.HidePopUp();


      }

   }

}