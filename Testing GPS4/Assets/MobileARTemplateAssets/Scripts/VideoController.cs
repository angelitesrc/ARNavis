using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    [Header("Video Players")]
    public VideoPlayer[] videoPlayers;

    [Header("Buttons")]
    public Button[] toggleButtons;
    public Image[] buttonImages;

    [Header("Icons")]
    public Sprite playIcon;
    public Sprite pauseIcon;

    void Start()
    {
        for (int i = 0; i < toggleButtons.Length; i++)
        {
            int index = i; // fix closure issue
            toggleButtons[index].onClick.AddListener(() => ToggleVideo(index));
            UpdateIcon(index);
        }
    }

    void ToggleVideo(int index)
    {
        if (videoPlayers[index].isPlaying)
        {
            videoPlayers[index].Pause();
        }
        else
        {
            videoPlayers[index].Play();
        }

        UpdateIcon(index);
    }

    void UpdateIcon(int index)
    {
        if (videoPlayers[index].isPlaying)
        {
            buttonImages[index].sprite = pauseIcon;
        }
        else
        {
            buttonImages[index].sprite = playIcon;
        }
    }
}
