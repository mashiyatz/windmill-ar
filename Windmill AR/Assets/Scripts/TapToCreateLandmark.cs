using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class TapToCreateLandmark : MonoBehaviour
{

    public GameObject markerPrefab;

    private GameObject markerObject;
    private ARRaycastManager arRayCastManager;
    private Vector2 touchPosition;

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private void Awake()
    {
        arRayCastManager = GetComponent<ARRaycastManager>();
    }

    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }
        else
        {
            touchPosition = default;
            return false;
        }
    }

    void Update()
    {
        if (!TryGetTouchPosition(out touchPosition)) { return; }
        if (arRayCastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;
            if (markerObject == null)
            {
                markerObject = Instantiate(markerPrefab, hitPose.position, hitPose.rotation);
            }
            else
            {
                markerObject.transform.position = hitPose.position;
            }

        }
    }
}
