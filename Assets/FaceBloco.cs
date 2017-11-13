using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceBloco : MonoBehaviour
{
	public Animator ac;
	public MeshRenderer mesh;
	public BoxCollider col;
	public Material textura1;
	public Material textura2;

	public void Start ()
	{
	}

	public void SetTipo (bool esconder)
	{
		Material textura;
		if (esconder) {
			textura = textura2;
		} else {
			textura = textura1;
		}
		mesh.material = textura;
	}

	public void SetEsconderObstaculos (bool esconderObstaculos)
	{
		col.enabled = !esconderObstaculos;
		SetTipo (esconderObstaculos);
	}

}
