using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Map : MonoBehaviour
{
    [Header("API Settings")]
    public string apiKey;

    [Header("Map Coordinates")]
    public Vector2 gpsCoordinate = new Vector2(1.5147767587376162f, 110.34245382171568f);
    public int zoom = 12;

    public enum Resolution { Low = 1, High = 2 };
    public Resolution mapResolution = Resolution.Low;

    public enum MapType { Roadmap, Satellite, Hybrid, Terrain };
    public MapType mapType = MapType.Roadmap;

    private string url = "";
    private int mapWidth;
    private int mapHeight;
    private bool mapIsLoading = false;
    private Rect rect;

    // Track last settings to optimize map updates
    private string apiKeyLast;
    private Vector2 gpsCoordinateLast;
    private int zoomLast;
    private Resolution mapResolutionLast;
    private MapType mapTypeLast;

    void Start()
    {
        rect = GetComponent<RawImage>().rectTransform.rect;
        mapWidth = Mathf.RoundToInt(rect.width);
        mapHeight = Mathf.RoundToInt(rect.height);

        // Load the map for the first time
        StartCoroutine(GetGoogleMap());
    }

    void Update()
    {
        if (IsMapUpdateRequired())
        {
            rect = GetComponent<RawImage>().rectTransform.rect;
            mapWidth = Mathf.RoundToInt(rect.width);
            mapHeight = Mathf.RoundToInt(rect.height);

            StartCoroutine(GetGoogleMap());
        }
    }

    private bool IsMapUpdateRequired()
    {
        return apiKeyLast != apiKey ||
               gpsCoordinate != gpsCoordinateLast ||
               zoomLast != zoom ||
               mapResolutionLast != mapResolution ||
               mapTypeLast != mapType;
    }

    IEnumerator GetGoogleMap()
    {
        mapIsLoading = true;

        url = $"https://maps.googleapis.com/maps/api/staticmap?center={gpsCoordinate.x},{gpsCoordinate.y}&zoom={zoom}&size={mapWidth}x{mapHeight}&scale={(int)mapResolution}&maptype={mapType.ToString().ToLower()}&key={apiKey}";

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Map Load Error: {www.error}");
        }
        else
        {
            mapIsLoading = false;
            var texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            GetComponent<RawImage>().texture = texture;

            // Cache the last settings
            apiKeyLast = apiKey;
            gpsCoordinateLast = gpsCoordinate;
            zoomLast = zoom;
            mapResolutionLast = mapResolution;
            mapTypeLast = mapType;

            Debug.Log("Map loaded successfully.");
        }
    }
}
