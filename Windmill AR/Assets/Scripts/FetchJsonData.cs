using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using SimpleJSON;
using TMPro;
using ARLocation;
using Siccity.GLTFUtility;
using System.IO;
using System.Linq;

public class FetchJsonData : MonoBehaviour
{
    public string inputURL = "igbhggdacj";

    public TextMeshProUGUI textBox;
    public TMP_InputField inputBox;
    public GameObject markerPrefab;
    public string fileDir;
    public Dictionary<string, Vector2> downloadURLs;
    
    private string URL;

    private void Start()
    {
        fileDir = $"{Application.dataPath}/Files/"; 
        downloadURLs = new Dictionary<string, Vector2>();
        
    }

    public void GenerateRequest() 
    {
        inputURL = inputBox.text;
        URL = "https://" + inputURL + ".reearth.io/data.json";
        StartCoroutine(DownloadAssets());
    }

    private IEnumerator RetrieveModelData(string uri)
    {
        using UnityWebRequest request = UnityWebRequest.Get(uri);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
        }
        else
        {
            JSONNode reearthData = JSON.Parse(request.downloadHandler.text);

            for (int i = 0; i < reearthData["layers"].Count; i++)
            {
                if (reearthData["layers"][i]["extensionId"] == "model")
                {
                    float latitude = reearthData["layers"][i]["property"]["default"]["location"]["lat"];
                    float longitude = reearthData["layers"][i]["property"]["default"]["location"]["lng"];
                    string modelURL = reearthData["layers"][i]["property"]["default"]["model"];
                    // string name = reearthData["layers"][i]["property"]["default"]["title"];

                    textBox.text = "Lat: " + latitude.ToString() + "\nLon: " + longitude.ToString() + "\nURL: " + modelURL;
                    downloadURLs.Add(modelURL, new Vector2(latitude, longitude));
                }
            }
            Debug.Log("URL scaping complete!");
        }
    }



    private void GenerateLandmark(float lat, float lon, GameObject model)
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

        model.transform.SetParent(newLandmark.transform);
    }

    GameObject LoadModel(string url) 
    {
        GameObject model = Importer.LoadFromFile(url);
        Debug.Log("Loaded file!");
        return model;
    }

    IEnumerator DownloadAssets()
    {
        yield return StartCoroutine(RetrieveModelData(URL));

        foreach (var item in downloadURLs)
        {
            string filePath = fileDir + item.Key.Split('/').Last(); 

            if (File.Exists(filePath))
            {
                Debug.Log("Found file locally, loading...");
                GenerateLandmark(item.Value[0], item.Value[1], LoadModel(filePath));
            }
            else
            {
                using UnityWebRequest request = UnityWebRequest.Get(item.Key);
                DownloadHandlerFile dlHandler = new DownloadHandlerFile(filePath)
                {
                    removeFileOnAbort = true
                };
                request.downloadHandler = dlHandler;

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log($"{request.error} : {request.downloadHandler.text}");
                }
                else
                {
                    GenerateLandmark(item.Value[0], item.Value[1], LoadModel(filePath));
                }
            }
        }
    }
}
