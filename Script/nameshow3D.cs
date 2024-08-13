using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nameshow3D : MonoBehaviour
{
   Camera cam;

	void Update()
	{
		if(cam == null)
			cam = FindObjectOfType<Camera>();

		if(cam == null)
			return;

		transform.LookAt(cam.transform);
		transform.Rotate(Vector3.up * 180);
	}
}
