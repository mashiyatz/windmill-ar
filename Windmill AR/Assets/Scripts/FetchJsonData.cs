using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using SimpleJSON;
using TMPro;

public class FetchJsonData : MonoBehaviour
{
    private const string URL = "https://igbhggdacj.reearth.io/data.json";
    public float latitude;
    public float longitude;
    public string modelURL;

    public TextMeshProUGUI textBox;

    public void GenerateRequest()
    {
        StartCoroutine(ProcessRequest(URL));
    }

    private IEnumerator ProcessRequest(string uri)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(request.error);
            } else
            {
                // Debug.Log(request.downloadHandler.text);

                JSONNode reearthData = JSON.Parse(request.downloadHandler.text);
                latitude = reearthData["layers"][5]["property"]["default"]["location"]["lat"];
                longitude = reearthData["layers"][5]["property"]["default"]["location"]["lng"];
                modelURL = reearthData["layers"][5]["property"]["default"]["model"];
                textBox.text = "Lat: " + latitude.ToString() + "\nLon: " + longitude.ToString() + "\nURL: " + modelURL;
            }

        }
    }



}
