using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Android; 

public class LocationPermission : MonoBehaviour
{
    public static LocationPermission Instance { get; private set; }

    public float latitude;
    public float longitude;
    public bool locationReady = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }
        else
        {
            StartCoroutine(GetLocation());
        }
    }


    void Update()
    {
        // If permission was just granted, start the location service
        if (!locationReady && Permission.HasUserAuthorizedPermission(Permission.FineLocation)
            && Input.location.status == LocationServiceStatus.Stopped)
        {
            StartCoroutine(GetLocation());
        }
    }

    IEnumerator GetLocation()
    {
        if (!Input.location.isEnabledByUser)
        {
            Debug.LogWarning("Location service not enabled by user.");
            yield break;
        }

        Input.location.Start();

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait <= 0)
        {
            Debug.LogWarning("Timed out while initializing location services.");
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogWarning("Unable to determine device location.");
            yield break;
        }
        else
        {
            latitude = Input.location.lastData.latitude;
            longitude = Input.location.lastData.longitude;
            locationReady = true;

            Debug.Log($"Location retrieved: {latitude}, {longitude}");
        }
    }

    public void RefreshLocation()
    {
        StartCoroutine(GetLocation());
    }
}
