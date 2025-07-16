using UnityEngine;
using UnityEngine.Android;

public class RequestLocationPermission : MonoBehaviour
{
    void Start()
    {
        // Request location permission when the scene starts
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }
    }
}
