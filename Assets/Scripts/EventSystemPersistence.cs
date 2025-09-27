// EventSystemPersistence.cs
using UnityEngine;
using UnityEngine.EventSystems;
// Namespace for FindObjectsByType and FindObjectsSortMode
using Object = UnityEngine.Object;

public class EventSystemPersistence : MonoBehaviour
{
    void Awake()
    {
        EventSystem[] allEventSystems = Object.FindObjectsByType<EventSystem>(FindObjectsSortMode.None);

        // 2. Check if more than one exists and destroy the duplicates
        if (allEventSystems.Length > 1)
        {
            // Iterate and destroy any other EventSystem GameObjects
            foreach (EventSystem es in allEventSystems)
            {
                // We use "es != this" to ensure we don't destroy the one this script is attached to.
                if (es != GetComponent<EventSystem>()) 
                {
                    // Destroy the GameObject holding the duplicate EventSystem component
                    Destroy(es.gameObject);
                }
            }
        }
        
        DontDestroyOnLoad(this.gameObject);
    }
}