using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.Video;
using System.Collections.Generic;

public class ImageRecognitionHandler : MonoBehaviour
{
    private ARTrackedImageManager trackedImageManager;

    public GameObject ellipticalPanel;
    public GameObject ponyPanel;
    public GameObject surferPanel;

    public VideoPlayer ellipticalVideo;
    public VideoPlayer ponyVideo;
    public VideoPlayer surferVideo;

    void Awake()
    {
        trackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    void OnEnable()
    {
        if (trackedImageManager != null)
        {
            trackedImageManager.trackablesChanged.AddListener(OnTrackablesChanged);
        }
    }

    void OnDisable()
    {
        if (trackedImageManager != null)
        {
            trackedImageManager.trackablesChanged.RemoveListener(OnTrackablesChanged);
        }
    }

    public void ShowEllipticalPanel()
    {
        ShowOnlyOnePanel(ellipticalPanel, ellipticalVideo);
    }

    public void ShowPonyPanel()
    {
        ShowOnlyOnePanel(ponyPanel, ponyVideo);
    }

    public void ShowSurferPanel()
    {
        ShowOnlyOnePanel(surferPanel, surferVideo);
    }

    void OnTrackablesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            HandleImage(trackedImage);
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            HandleImage(trackedImage);
        }

        foreach (var trackedImage in eventArgs.removed)
        {
            DeactivateAllPanels();
        }
    }

    void HandleImage(ARTrackedImage trackedImage)
    {
        if (trackedImage.trackingState != TrackingState.Tracking)
        {
            DeactivateAllPanels();
            return;
        }

        string imageName = trackedImage.referenceImage.name;

        switch (imageName)
        {
            case "gumball": // Elliptical
                ShowOnlyOnePanel(ellipticalPanel, ellipticalVideo);
                break;
            case "A":    //Pony
                ShowOnlyOnePanel(ponyPanel, ponyVideo);
                break;
            case "Surfer":
                ShowOnlyOnePanel(surferPanel, surferVideo);
                break;
            default:
                break;
        }
    }

    void ShowOnlyOnePanel(GameObject panelToShow, VideoPlayer videoToPlay)
    {
        DeactivateAllPanels(); // deactivate others

        panelToShow.SetActive(true);
        videoToPlay.Play();
    }

    void DeactivateAllPanels()
    {
        ellipticalPanel.SetActive(false);
        ponyPanel.SetActive(false);
        surferPanel.SetActive(false);

        ellipticalVideo.Stop();
        ponyVideo.Stop();
        surferVideo.Stop();
    }
}
