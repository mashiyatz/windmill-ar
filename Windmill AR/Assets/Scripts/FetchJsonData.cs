using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using SimpleJSON;
using TMPro;
using ARLocation;
using Siccity.GLTFUtility;
using System.IO;

public class FetchJsonData : MonoBehaviour
{
    private const string URL = "https://igbhggdacj.reearth.io/data.json";

    public TextMeshProUGUI textBox;
    public GameObject markerPrefab;
    public string filePath;

    private void Start()
    {
        filePath = $"{Application.dataPath}/Files/test.gltf";
    }

    public void GenerateRequest()
    {
        StartCoroutine(ProcessRequest(URL));
    }

    private IEnumerator ProcessRequest(string uri)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(uri)) // allow user to input their own Re:Earth URL, just need the name, other can be autofill
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(request.error);
            } else
            {
                // Debug.Log(request.downloadHandler.text);

                JSONNode reearthData = JSON.Parse(request.downloadHandler.text);

                for (int i = 0; i < reearthData["layers"].Count; i++)
                {
                    if (reearthData["layers"][i]["extensionId"] == "model")
                    {
                        float latitude = reearthData["layers"][i]["property"]["default"]["location"]["lat"];
                        float longitude = reearthData["layers"][i]["property"]["default"]["location"]["lng"];
                        string modelURL = reearthData["layers"][i]["property"]["default"]["model"];
                        // string name = reearthData["layers"][i]["property"]["default"]["title"];
                        GenerateLandmark(latitude, longitude, modelURL);

                        textBox.text = "Lat: " + latitude.ToString() + "\nLon: " + longitude.ToString() + "\nURL: " + modelURL;
                    }
                }
                
            }

        }
    }

    private void GenerateLandmark(float lat, float lon, string url)
    {
        GameObject newLandmark;
        newLandmark = Instantiate(markerPrefab);

        newLandmark.GetComponent<PlaceAtLocation>().Location = new Location()
        {
            Latitude = lat,
            Longitude = lon,
            Altitude = 0,
            AltitudeMode = AltitudeMode.DeviceRelative,
            // Label = lab,
        };

        // newLandmark.gameObject.name = lab;

        newLandmark.GetComponent<PlaceAtLocation>().PlacementOptions = new PlaceAtLocation.PlaceAtOptions()
        {
            HideObjectUntilItIsPlaced = true,
            MaxNumberOfLocationUpdates = 2,
            MovementSmoothing = 0.1f,
            UseMovingAverage = false
        };

        DownloadModel(url, newLandmark);

    }


    private void DownloadModel(string url, GameObject landmark)
    {
        if (File.Exists(filePath))
        {
            Debug.Log("Found file locally, loading...");
            LoadModel(landmark);
            return;
        }

        StartCoroutine(GetFileRequest(url, (UnityWebRequest req) =>
        { if (req.result == UnityWebRequest.Result.ConnectionError || req.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log($"{req.error} : {req.downloadHandler.text}");
            } else
            {
                LoadModel(landmark);
            } }
    ));
    }

    void LoadModel(GameObject landmark)
    {
        GameObject model = Importer.LoadFromFile(filePath);
        Debug.Log("Loaded file!");
        model.transform.SetParent(landmark.transform);
    }

    IEnumerator GetFileRequest(string url, System.Action<UnityWebRequest> callback)
    {
        using(UnityWebRequest req = UnityWebRequest.Get(url))
        {
            yield return req.SendWebRequest();
            req.downloadHandler = new DownloadHandlerFile(filePath);
            callback(req);
        }
    }


}
