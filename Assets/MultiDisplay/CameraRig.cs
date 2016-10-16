//CameraRig.cs
//Personal Attempt at CameraRig.cs

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

using System.Xml;
using System.Xml.Serialization;
using System.Text;
using System.IO;
using System;


[System.Serializable]
public class CameraRig : MonoBehaviour {

	public struct WallParams {
		public int rows;
		public int cols;
	}

	public WallParams wallParams;
	public float nearClip = 0.3f;
	public float farClip = 1000;
	public float fov = 60;

	public float screenAspectWidth = 9;
	public float screenAspectHeight = 16;

	//the scene should disappear behind the seam
	public float horizontalSeam = 0;
	public float verticalSeam = 0;


	//cluster type will be WALL
	public void SetupCamera(Camera camera, int cameraIndex) {

		wallParams.rows = 1;
		wallParams.cols = 1;

		// reset this incase we were wall once
		camera.ResetProjectionMatrix();

		// this must be reset
		camera.transform.localRotation = Quaternion.identity;

		float left = 0.0f, right = 0.0f, bottom = 0.0f, top = 0.0f;
		CalculateCameraProjectionFrustum(cameraIndex, ref left, ref right, ref bottom, ref top);
		camera.projectionMatrix = PerspectiveOffCenter( left, right, bottom, top, nearClip, farClip);
		camera.nearClipPlane = nearClip;
		camera.farClipPlane = farClip;
	}

	public void CalculateCameraProjectionFrustum (int cameraIndex,
		ref float outLeft,
		ref float outRight,
		ref float outBottom,
		ref float outTop)
	{
		
		float totalScreenAspectWidth = wallParams.cols * (screenAspectWidth + horizontalSeam);
		float totalScreenAspectHeight = wallParams.rows * (screenAspectHeight + verticalSeam);

		// aspect ratio of whole image
		float aspect = totalScreenAspectWidth / totalScreenAspectHeight;
		float halfFov = Mathf.Deg2Rad * (fov * 0.5f);
		// overall top computed from half fov,
		// then overall left computed by multiplying aspect ratio
		float top = Mathf.Tan (halfFov) * nearClip;
		float left = -top * aspect;
		// x and y offsets of current camera based on camera index
		int x = cameraIndex % wallParams.cols;
		int y = (int)(Mathf.Floor (cameraIndex / wallParams.cols));

		float l, t, w, h;

		if (totalScreenAspectWidth > totalScreenAspectHeight)
		{
			float screenAspect = screenAspectHeight / screenAspectWidth;
			// width and height of single screen
			w = (top * aspect * 2) / wallParams.cols;
			h = w * screenAspect;
			float b = (2 * top - h * wallParams.rows) * 0.5f;
			l = left + x * w;
			t = top - b - y * h;

		}
		else
		{
			float screenAspect = screenAspectWidth / screenAspectHeight;
			// height and width of single screen
			h = (top * 2) / wallParams.rows;
			w = h * screenAspect;
			float b = ((2 * top * aspect) - (w * wallParams.cols)) * 0.5f;
			l = left + b + x * w;
			t = top - y * h;
		}

		// use percentage of seam over aspect width or height
		// multipled by width or height to find offset
		float horizontalOffset = (horizontalSeam / screenAspectWidth) * w;
		float verticalOffset = (verticalSeam / screenAspectHeight) * h;

		outLeft = l + horizontalOffset;
		outRight = l + w - horizontalOffset;
		outBottom = t - h + verticalOffset;
		outTop = t - verticalOffset;

	}
  
  
  	private Matrix4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far) {
		float x =  (2.0f * near) / (right - left);
		float y =  (2.0f * near) / (top - bottom);
		float a =  (right + left) / (right - left);
		float b =  (top + bottom) / (top - bottom);
		float c = -(far + near) / (far - near);
		float d = -(2.0f * far * near) / (far - near);
		float e = -1.0f;
		Matrix4x4 m  = new Matrix4x4();
		m[0,0] = x;  m[0,1] = 0;  m[0,2] = a;  m[0,3] = 0;
		m[1,0] = 0;  m[1,1] = y;  m[1,2] = b;  m[1,3] = 0;
		m[2,0] = 0;  m[2,1] = 0;  m[2,2] = c;  m[2,3] = d;
		m[3,0] = 0;  m[3,1] = 0;  m[3,2] = e;  m[3,3] = 0;
		return m;
   }
}
