using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	private Vector3 objetivo;
	private Rigidbody rb;
	public Transform child;

	void Start ()
	{
		rb = GetComponent<Rigidbody> ();
		objetivo = transform.position;	
	}

	public void Movimenta (Vector3 direcao)
	{
		transform.position = objetivo;
		objetivo += direcao;
		child.LookAt (objetivo + direcao);
		StopAllCoroutines ();
		StartCoroutine (movimentar ());
	}

	private IEnumerator movimentar ()
	{
		float duracao = 1;
		float precisao = 0.02f;
		float step = precisao / duracao;
		for (float i = 0; i < duracao; i += step) {
			yield return new WaitForSeconds (step);
			transform.position = Vector3.Lerp (transform.position, new Vector3 (objetivo.x, transform.position.y, objetivo.z), i / duracao);
		}
	}

	public void OnTriggerEnter (Collider c)
	{
		rb.isKinematic = false;
		rb.useGravity = true;
		Manager.getInstance ().SetEstado (Manager.Estado.GAME_OVER);
		Destroy (this.gameObject, 2.0f);
	}
}
