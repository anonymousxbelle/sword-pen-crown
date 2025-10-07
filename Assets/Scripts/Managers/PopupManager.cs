using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace Managers
{
   public class PopupManager : MonoBehaviour
   {
      public static PopupManager Instance { get; private set; }
      public bool IsPopupActive => popupRoot != null && popupRoot.activeInHierarchy;
      
      [Header("UI References")] [SerializeField]
      private GameObject popupRoot;
      [SerializeField] private TMP_Text messageText;
      [SerializeField] private Button confirmButton;
      [SerializeField] private Button cancelButton;
      
      private Action confirmAction;
      private Action cancelAction;
      
      private void Awake()
      { //ensures only one popup manager exists for the entire game.
         if (Instance != null && Instance != this)
         {
            Destroy(gameObject);
            return;
         }
         Instance = this;
         DontDestroyOnLoad(gameObject);
         
         if (popupRoot != null) popupRoot.SetActive(false); //Ensures the popup is hidden at game start.
         
         confirmButton.onClick.RemoveAllListeners(); //Removes any old listeners that may have been added
         confirmButton.onClick.AddListener(OnConfirm); //Adds fresh listeners, linking to methods
         cancelButton.onClick.RemoveAllListeners();
         cancelButton.onClick.AddListener(OnCancel);
      }
      
      public void ShowMessage(string message)
      { //Displays a simple info popup with a single “OK” button
         if (popupRoot == null) return;
         
         messageText.text = message;
         confirmButton.gameObject.SetActive(true);
         cancelButton.gameObject.SetActive(false);//Hides the cancel button since this type doesn’t need one.
         //Clears previous actions to avoid accidental triggers from earlier popups.
         confirmAction = null;
         cancelAction = null;
         popupRoot.SetActive(true);
      }
      public void ShowConfirmation(string message, Action onConfirm, Action onCancel)
      { //Displays a confirmation popup (e.g., “Are you sure you want to overwrite this save?”).
         if (popupRoot == null) return;
         
         messageText.text = message;
         confirmAction = onConfirm;
         cancelAction = onCancel;
         confirmButton.gameObject.SetActive(true);
         cancelButton.gameObject.SetActive(true);
         popupRoot.SetActive(true);

      }
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
         cancelAction?.Invoke();
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