using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;

public class Manager : MonoBehaviour
{
	private static Manager instance;

	public static Manager getInstance ()
	{
		return instance;
	}

	public CameraMovement cameraMovement;
	public GameObject playerPrefab;
	public GameObject blocoCenarioPrefab;


	public Animator gameOverAC;
	public Animator venceuAC;
	public Canvas gameOverCanvas;
	public Canvas escolhendoLevelCanvas;
	public Canvas venceuCanvas;
	public Text tempo;

	private int playerX = 0;
	private int playerZ = 0;

	private int largura, altura;
	private float espacamento = 1.5f;

	private Player playerScript;
	private Dictionary<Tuple<int,int>,BlocoCenario> blocos;
	private Estado estado;
	private Stopwatch stopwatch = Stopwatch.StartNew ();

	private int lastSize;

	Manager ()
	{
		instance = this;
	}

	void Start ()
	{
		EscolherLevel ();
	}

	public void Reset ()
	{
		NewGame (lastSize);
	}

	public void NewGame (int size)
	{	
		lastSize = size;
		stopwatch = Stopwatch.StartNew ();
		SetEstado (Estado.JOGANDO);
		largura = (size - 1) / 2;
		altura = (size - 1) / 2;
		playerX = -largura;
		playerZ = -altura;
		if (blocos != null) {
			foreach (var kv in blocos) {
				GameObject.Destroy (kv.Value.gameObject);
			}
		}
		blocos = new Dictionary<Tuple<int,int>,BlocoCenario> ();
		criaPlayer ();
		for (int i = -largura; i <= largura; i++) {
			for (int j = -altura; j <= altura; j++) {
				criaBlocoCenario (new Vector3 (i * espacamento, 0, j * espacamento), i, j);
			}	
		}

		inicializaBlocos ();

	}

	private void inicializaBlocos ()
	{
		GeradorCaminhos gerador = new GeradorCaminhos ();
		gerador.Gerar (largura, altura);
		foreach (var kv in gerador.restricoesPorCasas) {
			if (!blocos.ContainsKey (kv.Key)) {
				print (kv.Key.first + "," + kv.Key.second);
				print (blocos.Count);
			} else {
				blocos [kv.Key].SetFacesLimpas (kv.Value);
			}
		}
	}


	private void criaPlayer ()
	{
		if (playerScript != null) {
			Destroy (playerScript.gameObject);
		}
		GameObject go = GameObject.Instantiate (playerPrefab, new Vector3 (playerX * espacamento, 0.5f, playerZ * espacamento)
			, Quaternion.identity);
		playerScript = go.GetComponent<Player> ();
	}

	private void criaBlocoCenario (Vector3 posicao, int i, int j)
	{
		blocos.Add (new Tuple<int,int> (i, j), GameObject.Instantiate (blocoCenarioPrefab, posicao, Quaternion.identity).GetComponent<BlocoCenario> ());
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetAxis ("Mouse X") != 0) {
			cameraMovement.Rodar (Input.GetAxis ("Mouse X") / 10);
		}
		Vector3 mov = Vector3.zero;

		if (Input.GetKeyDown (KeyCode.W)) {
			mov = cameraMovement.transform.forward;
		} else if (Input.GetKeyDown (KeyCode.S)) {
			mov = -cameraMovement.transform.forward;
		} else if (Input.GetKeyDown (KeyCode.A)) {
			mov = -cameraMovement.transform.right;
		} else if (Input.GetKeyDown (KeyCode.D)) {
			mov = cameraMovement.transform.right;
		}
		if (mov != Vector3.zero && estado == Estado.JOGANDO) {
			giraCenarioEMovimentaPlayer (arredondaDirecao (mov));
		}
		tempo.text = string.Format ("{0:00}:{1:00}", stopwatch.Elapsed.Minutes, stopwatch.Elapsed.Seconds);
		//print (string.Format ("{0:00}:{1:00}", stopwatch.Elapsed.Minutes, stopwatch.Elapsed.Seconds));

	}

	private Vector3 arredondaDirecao (Vector3 cameraDirection)
	{
		if (Mathf.Abs (cameraDirection.x) > Mathf.Abs (cameraDirection.z)) {
			
			return new Vector3 (Mathf.Sign (cameraDirection.x), 0, 0);
		} else {
			return new Vector3 (0, 0, Mathf.Sign (cameraDirection.z));
		}
	}

	private void giraCenarioEMovimentaPlayer (Vector3 input)
	{
		int novoPlayerX = playerX + Mathf.RoundToInt (input.x);
		int novoPlayerZ = playerZ + Mathf.RoundToInt (input.z);
		if (novoPlayerX <= largura && novoPlayerX >= -largura
		    && novoPlayerZ <= altura && novoPlayerZ >= -altura) {
			if (novoPlayerX == largura && novoPlayerZ == altura) {
				SetEstado (Estado.VENCEU);
			}
			playerX = novoPlayerX;
			playerZ = novoPlayerZ;
			movimentaPlayer (input);
			giraCenario (new Vector3 (input.z, 0, -input.x));
		}

	}

	private void movimentaPlayer (Vector3 input)
	{
		playerScript.Movimenta (input * espacamento);
	}

	private void giraCenario (Vector3 angle)
	{
		foreach (var b in blocos.Values) {
			b.Gira (angle * 90);
		}
	}

	public void EscolherLevel ()
	{
		SetEstado (Estado.ESCOLHENDO_LEVEL);
	}

	public void SetEstado (Estado estado)
	{
		this.estado = estado;

		escolhendoLevelCanvas.enabled = false;
		gameOverCanvas.enabled = false;
		venceuCanvas.enabled = true;
		switch (estado) {
		case Estado.ESCOLHENDO_LEVEL:
			escolhendoLevelCanvas.enabled = true;
			break;
		case Estado.GAME_OVER:
			gameOverAC.SetBool ("gameOver", true);
			gameOverCanvas.enabled = true;
			stopwatch.Stop ();
			break;
		case Estado.JOGANDO:
			gameOverAC.SetBool ("gameOver", false);
			venceuAC.SetBool ("venceu", false);
			break;
		case Estado.VENCEU:
			venceuAC.SetBool ("venceu", true);
			stopwatch.Stop ();
			break;
		}
	}

	public enum Estado
	{
		ESCOLHENDO_LEVEL,
		GAME_OVER,
		JOGANDO,
		VENCEU
	}
}
