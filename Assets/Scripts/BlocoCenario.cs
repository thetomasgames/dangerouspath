using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BlocoCenario : MonoBehaviour
{
	[Serializable]
	public struct TransformadaPorFace
	{
		public FaceDado face;
		public FaceBloco script;
	}

	public List<TransformadaPorFace> obstaculosPorFace;

	Quaternion rotationObjetivo;

	void Start ()
	{
		foreach (var obstaculo in obstaculosPorFace) {
			obstaculo.script.Start ();
		}
	}

	public void SetFacesLimpas (HashSet<FaceDado> faces)
	{
		foreach (var face in obstaculosPorFace) {
			bool contem = faces.Contains (face.face);
			face.script.SetEsconderObstaculos (contem);
		}
	}

	public void SetDicas (HashSet<FaceDado> faces)
	{
		foreach (var face in faces) {
			//obstaculosPorFace.Find (o => o.face == face).script.SetCor (Color.blue);
		}
	}

	public void Gira (Vector3 angle)
	{
		if (rotationObjetivo != null) {
			transform.rotation = rotationObjetivo;
		}
		StopAllCoroutines ();
		Quaternion aux = transform.rotation;
		transform.Rotate (angle, Space.World);
		rotationObjetivo = transform.rotation;
		transform.rotation = aux;
		StartCoroutine (girar (angle));
	}


	private IEnumerator girar (Vector3 angle)
	{
		float duracao = 0.3f;
		float precisao = 0.02f;
		float step = precisao / duracao;
		float somatorio = 0;
		for (float i = 0; i < duracao; i += step) {
			yield return new WaitForSeconds (step);
			float giro = step / duracao;
			somatorio += giro;
			transform.Rotate (angle * giro, Space.World);
		}
		float arredondamento = 1 - somatorio;
		transform.Rotate (angle * arredondamento, Space.World);
	}
}
