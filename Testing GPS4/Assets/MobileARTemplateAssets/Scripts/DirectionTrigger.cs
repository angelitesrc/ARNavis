using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DirectionTrigger : MonoBehaviour
{
    public GameObject straightPanel;
    public GameObject leftPanel;
    public GameObject rightPanel;
    public GameObject parkPanel;

    public AudioSource turnRight;
    public AudioSource turnLeft;
    public AudioSource goStraight;
    public AudioSource reachedPark;

    // Taman Hui Sing
    //public Vector2 straightLocation = new Vector2(1.513542832686142f, 110.34103451507545f);
    //public Vector2 leftLocation = new Vector2(1.5140093741668086f, 110.34227906006397f);
    //public Vector2 rightLocation = new Vector2(1.514073724708003f, 110.34122763412606f);
    //public Vector2 parkLocation = new Vector2(1.5145434101913227f, 110.34236552732055f);

    // Kolej Rafflesia
    public Vector2 straightLocation = new Vector2(1.513542832686142f, 110.34103451507545f);
    public Vector2 leftLocation = new Vector2(1.4461728787636612f, 110.45518127197636f);
    public Vector2 rightLocation = new Vector2(1.514073724708003f, 110.34122763412606f);
    public Vector2 parkLocation = new Vector2(1.4468378463664888f, 110.45462270654556f);

    public float triggerDistance = 10f;
    public float voiceDelay = 2f;

    private string lastTriggered = "";
    private Coroutine currentAudioCoroutine;

    void Start()
    {
        StartCoroutine(StartLocationService());
    }

    IEnumerator StartLocationService()
    {
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Location services not enabled.");
            yield break;
        }

        Input.location.Start();

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (Input.location.status != LocationServiceStatus.Running)
        {
            Debug.Log("Unable to start location service.");
            yield break;
        }

        while (true)
        {
            LocationInfo location = Input.location.lastData;
            Vector2 currentPos = new Vector2(location.latitude, location.longitude);

            if (LocationPermission.Instance != null && LocationPermission.Instance.locationReady)
            {
                float lat = LocationPermission.Instance.latitude;
                float lon = LocationPermission.Instance.longitude;
                Debug.Log($"Using GPS: {lat}, {lon}");
            }

            CheckProximity(currentPos);
            yield return new WaitForSeconds(2f);
        }
    }

    void CheckProximity(Vector2 currentPos)
    {
        // Reset panels
        straightPanel.SetActive(false);
        leftPanel.SetActive(false);
        rightPanel.SetActive(false);
        parkPanel.SetActive(true); // false

        // Compare distance
        if (GetDistance(currentPos, straightLocation) < triggerDistance)
        {
            straightPanel.SetActive(true);
            if (lastTriggered != "straight")
            {
                PlayNewVoice(goStraight);
                lastTriggered = "straight";
            }
        }
        else if (GetDistance(currentPos, leftLocation) < triggerDistance)
        {
            leftPanel.SetActive(true);
            if (lastTriggered != "left")
            {
                PlayNewVoice(turnLeft);
                lastTriggered = "left";
            }
        }
        else if (GetDistance(currentPos, rightLocation) < triggerDistance)
        {
            rightPanel.SetActive(true);
            if (lastTriggered != "right")
            {
                PlayNewVoice(turnRight);
                lastTriggered = "right";
            }
        }
        else if (GetDistance(currentPos, parkLocation) < triggerDistance)
        {
            parkPanel.SetActive(true);
            if (lastTriggered != "park")
            {
                PlayNewVoice(reachedPark);
                lastTriggered = "park";
            }
        }
        else
        {
            lastTriggered = "";
        }
    }

    void PlayNewVoice(AudioSource clipToPlay)
    {
        if (currentAudioCoroutine != null)
        {
            StopCoroutine(currentAudioCoroutine);
        }
        currentAudioCoroutine = StartCoroutine(PlayDelayedAudio(clipToPlay));
    }

    IEnumerator PlayDelayedAudio(AudioSource audio)
    {
        yield return new WaitForSeconds(voiceDelay);
        audio.Play();
    }

    float GetDistance(Vector2 pos1, Vector2 pos2)
    {
        float R = 6371000;
        float dLat = Mathf.Deg2Rad * (pos2.x - pos1.x);
        float dLon = Mathf.Deg2Rad * (pos2.y - pos1.y);

        float a = Mathf.Sin(dLat / 2) * Mathf.Sin(dLat / 2) +
                  Mathf.Cos(Mathf.Deg2Rad * pos1.x) * Mathf.Cos(Mathf.Deg2Rad * pos2.x) *
                  Mathf.Sin(dLon / 2) * Mathf.Sin(dLon / 2);

        float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
        return R * c;
    }
}