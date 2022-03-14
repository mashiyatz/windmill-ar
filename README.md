# Re:Earth AR Project

A demonstration of how Re:Earth can be integrated with Unity AR. Retrieve latitude, longitude, and 3D object files of models from any public Re:Earth project, and display them in geographically accurate AR.

## Dependencies
* Unity3D 2020.3.8 (LTS)
* Must include the AR + GPS Location Package from the [Unity Store](https://assetstore.unity.com/packages/tools/integration/ar-gps-location-134882). 
* Included in repository: GLTFUtility, JsonDotNet, SimpleJSON

## Code Summary

### [FetchJsonData.cs](https://github.com/mashiyatz/windmill-ar/blob/main/Windmill%20AR/Assets/Scripts/FetchJsonData.cs)

Takes user input of a Re:Earth subdomain from the UI, retrieves latitude, longitude, and the save-file URL of all 3D model objects from the project's .json, downloads each model (or checks that it already exists in the Unity hierarcy), and recreates the models in AR space.   

## TODO

* Create a new class for the 3D models to accommodate other kinds of data, such as labels, text boxes, etc. 
