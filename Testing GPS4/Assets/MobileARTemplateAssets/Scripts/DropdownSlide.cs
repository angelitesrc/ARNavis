using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DropdownSlide : MonoBehaviour
{
    public Animator volumeAnimator;         // The Animator on the VOLUME object
    public TMP_Dropdown fontSizeDropdown;   // The Font Size dropdown
    private bool isDropdownOpen = false;    // Tracks dropdown state

    void Start()
    {
        if (fontSizeDropdown != null)
        {
            // Add event trigger components if they don't exist
            AddEventTriggers();

            // Still keep the value changed listener
            fontSizeDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        }
    }

    void Update()
    {
        // Continuously check if the dropdown is open
        CheckDropdownListVisibility();
    }

    private void AddEventTriggers()
    {
        // Get or add EventTrigger component
        EventTrigger trigger = fontSizeDropdown.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = fontSizeDropdown.gameObject.AddComponent<EventTrigger>();

        // Clear existing entries to avoid duplicates
        trigger.triggers.Clear();

        // Add trigger for dropdown opening
        EventTrigger.Entry openEntry = new EventTrigger.Entry();
        openEntry.eventID = EventTriggerType.PointerClick;
        openEntry.callback.AddListener((data) => { OnDropdownClicked(); });
        trigger.triggers.Add(openEntry);

        // Add trigger for dropdown closing
        EventTrigger.Entry closeEntry = new EventTrigger.Entry();
        closeEntry.eventID = EventTriggerType.Submit;
        closeEntry.callback.AddListener((data) => { OnDropdownClosed(); });
        trigger.triggers.Add(closeEntry);
    }

    // Called when the dropdown is clicked
    private void OnDropdownClicked()
    {
        // Toggle the dropdown state
        isDropdownOpen = !isDropdownOpen;

        // Expand VOLUME when dropdown is opened
        if (isDropdownOpen)
        {
            volumeAnimator.SetBool("isExpanded", true);
        }

        // We need a slight delay to check if the dropdown actually closes
        // because the click could be on an option
        Invoke("CheckDropdownState", 0.1f);
    }

    // Check the actual dropdown state
    private void CheckDropdownState()
    {
        // Schedule continuous checking for the next few frames
        // This ensures we catch when the dropdown closes even without selection
        for (float delay = 0.1f; delay <= 0.3f; delay += 0.1f)
        {
            Invoke("CheckDropdownListVisibility", delay);
        }
    }

    // Check if the dropdown list is visible
    private void CheckDropdownListVisibility()
    {
        // Is there a dropdown list in the scene?
        Transform dropdownList = fontSizeDropdown.gameObject.transform.Find("Dropdown List");

        // If no dropdown list found or it's not active, dropdown is closed
        if (dropdownList == null || !dropdownList.gameObject.activeSelf)
        {
            // Only update if we thought it was still open
            if (isDropdownOpen)
            {
                isDropdownOpen = false;
                volumeAnimator.SetBool("isExpanded", false);
                Debug.Log("Dropdown closed - VOLUME sliding up");
            }
        }
        else
        {
            // Dropdown is still open
            isDropdownOpen = true;
            volumeAnimator.SetBool("isExpanded", true);
        }
    }

    // Called when dropdown value changes
    private void OnDropdownValueChanged(int index)
    {
        // Value selected, collapse VOLUME after a short delay
        // This delay gives time for the dropdown to close first
        Invoke("CloseVolumeAfterSelection", 0.2f);
    }

    // Called when dropdown options are closed
    private void OnDropdownClosed()
    {
        isDropdownOpen = false;
        volumeAnimator.SetBool("isExpanded", false);
    }

    // Called after selecting an option
    private void CloseVolumeAfterSelection()
    {
        isDropdownOpen = false;
        volumeAnimator.SetBool("isExpanded", false);
    }
}