using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
	float angle = 0;
	public float radius = 50;


	public void Rodar (float add)
	{
		angle += add;
		transform.position = new Vector3 (radius * Mathf.Sin (angle), transform.position.y, radius * Mathf.Cos (angle));
		transform.LookAt (Vector3.zero);
	}

}
