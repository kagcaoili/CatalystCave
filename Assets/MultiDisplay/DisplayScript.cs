using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DisplayScript : MonoBehaviour {

	private Camera[] cameras; //array of all the cameras

	// Use this for initialization
	void Start () {

		/*
		//searches all the children of this object for Cameras 
		cameras = GetComponentsInChildren<Camera> ();

		foreach (Camera camera in cameras) {
			CameraRig rig = camera.GetComponent<CameraRig> ();
			if (rig != null) {
				rig.SetupCamera (camera, camera.targetDisplay - 1);
			} else {
				Debug.LogError ("Error: Can't find CameraRig");
			}
		}
		*/



		Debug.Log("displays connected: " + Display.displays.Length);
		//loops through each display and activates it
		foreach (Display display in Display.displays) {
			display.Activate ();
		}
			
	}
	
	// Update is called once per frame
	void Update () {
	
	}

}
