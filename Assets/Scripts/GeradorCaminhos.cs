using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GeradorCaminhos:MonoBehaviour
{
	private List<List<TipoMovimentacao>> caminhosAlternativos;
	private List<TipoMovimentacao> caminhoValido;
	public Dictionary<Tuple<int,int>,HashSet<FaceDado>> restricoesPorCasas;
	public Dictionary<Tuple<int,int>,HashSet<FaceDado>> dicasPorCasas;


	public void Gerar (int largura, int altura)
	{
		caminhoValido =	gerarCaminho (largura, altura, new List<TipoMovimentacao> (), largura * altura * 10);
		geraCaminhosAlternativos (largura, altura);
		restricoesPorCasas = new Dictionary<Tuple<int, int>, HashSet<FaceDado>> ();
		dicasPorCasas = new Dictionary<Tuple<int, int>, HashSet<FaceDado>> ();

		aplicaDadosEmLista (caminhoValido, true, largura, altura);
		foreach (List<TipoMovimentacao> lista in caminhosAlternativos) {
			aplicaDadosEmLista (lista, false, largura, altura);
		}
	}

	private void geraCaminhosAlternativos (int largura, int altura)
	{
		caminhosAlternativos = new List<List<TipoMovimentacao>> ();
		for (int i = 0; i < caminhoValido.Count; i += Mathf.Min (5, largura, altura)) {
			caminhosAlternativos.Add (gerarCaminho (largura, altura, caminhoValido.GetRange (0, i), largura * altura));
		}
	}

	private List<TipoMovimentacao> gerarCaminho (int largura, int altura, List<TipoMovimentacao> retorno, int limitePontos)
	{
		System.Random rand = new System.Random (System.Environment.TickCount);

		TipoMovimentacao tipoExclusivo;
		if (rand.NextDouble () > 0.5f) {
			tipoExclusivo = TipoMovimentacao.NORTE;
		} else {
			tipoExclusivo = TipoMovimentacao.OESTE;
		}

		Dictionary<TipoMovimentacao,double> chancesPorTipo = new Dictionary<TipoMovimentacao, double> ();
		chancesPorTipo.Add (TipoMovimentacao.SUL, rand.NextDouble () * 2);
		chancesPorTipo.Add (TipoMovimentacao.LESTE, rand.NextDouble () * 2);
		chancesPorTipo.Add (tipoExclusivo, rand.NextDouble ());
		double somatorioTotal = 0;
		foreach (var valor in chancesPorTipo.Values) {
			somatorioTotal += valor;
		}

		int posX = -largura, posZ = -altura;
		retorno.ForEach (a => aplicaMovimentacao (ref posX, ref posZ, a));
		int posXStack = posX, posZStack = posZ;
		TipoMovimentacao? ultimaMovimentacao = null;
		while (posX != largura || posZ != altura) {
			TipoMovimentacao movimentacao = getMovimentacaoAleatoria (chancesPorTipo, somatorioTotal, rand);
			int novoPosX = posX;
			int novoPosZ = posZ;
			switch (movimentacao) {
			case TipoMovimentacao.LESTE:
				novoPosX++;
				break;
			case TipoMovimentacao.OESTE:
				novoPosX--;
				break;
			case TipoMovimentacao.NORTE:
				novoPosZ--;
				break;
			case TipoMovimentacao.SUL:
				novoPosZ++;
				break;
			}
			if (novoPosX >= -largura && novoPosX <= largura
			    && novoPosZ >= -altura && novoPosZ <= altura) {
				if (!(ultimaMovimentacao.HasValue && tipoExclusivo != ultimaMovimentacao && oposto (ultimaMovimentacao.Value, movimentacao))) {
					ultimaMovimentacao = movimentacao;
					retorno.Add (movimentacao);
					posX = novoPosX;
					posZ = novoPosZ;
				}
			}
			if (retorno.Count > limitePontos) {
				break;
			}
		}
		return retorno;
	}

	private bool oposto (TipoMovimentacao tipo1, TipoMovimentacao tipo2)
	{
		switch (tipo1) {
		case TipoMovimentacao.LESTE:
			return tipo2 == TipoMovimentacao.OESTE;
		case TipoMovimentacao.OESTE:
			return tipo2 == TipoMovimentacao.LESTE;
		case TipoMovimentacao.NORTE:
			return tipo2 == TipoMovimentacao.SUL;
		case TipoMovimentacao.SUL:
		default:
			return tipo2 == TipoMovimentacao.NORTE;
		}
	}



	private TipoMovimentacao getMovimentacaoAleatoria (Dictionary<TipoMovimentacao,double> chances, double total, System.Random rand)
	{
		double somatorio = 0;
		double valorSorteado = rand.NextDouble () * total;
		foreach (var kv in chances) {
			somatorio += kv.Value;
			if (somatorio >= valorSorteado) {
				return kv.Key;
			}
		}
		throw new Exception ();
	}

	private void aplicaDadosEmLista (List<TipoMovimentacao> lista, bool isCaminhoValido, int largura, int altura)
	{
		int i = -largura;
		int j = -altura;
		addDadosMovimentacao (i, j, new List<TipoMovimentacao> (), isCaminhoValido);
		List<TipoMovimentacao> movimentoAtual = new List<TipoMovimentacao> ();
		foreach (TipoMovimentacao movimento in lista) {
			aplicaMovimentacao (ref i, ref j, movimento);
			movimentoAtual.Add (movimento);
			addDadosMovimentacao (i, j, movimentoAtual, isCaminhoValido);
		}
		if (isCaminhoValido) {
			preencheCasa (i, j);
		}
	}

	private void addDadosMovimentacao (int i, int j, List<TipoMovimentacao> lista, bool isCaminhoValido)
	{
		Tuple<int,int> tuple = new Tuple<int,int> (i, j);
		FaceDado face = getFace (lista);
		adicionaEmDictionary (restricoesPorCasas, tuple, face);
		if (isCaminhoValido) {
			adicionaEmDictionary (dicasPorCasas, tuple, face);
		}
	}

	private void preencheCasa (int i, int j)
	{
		adicionaEmDictionary (restricoesPorCasas, new Tuple<int,int> (i, j), FaceDado.BASE);
		adicionaEmDictionary (restricoesPorCasas, new Tuple<int,int> (i, j), FaceDado.FRENTE);
		adicionaEmDictionary (restricoesPorCasas, new Tuple<int,int> (i, j), FaceDado.TRAS);
		adicionaEmDictionary (restricoesPorCasas, new Tuple<int,int> (i, j), FaceDado.TOPO);
		adicionaEmDictionary (restricoesPorCasas, new Tuple<int,int> (i, j), FaceDado.LADO_DIREITO);
		adicionaEmDictionary (restricoesPorCasas, new Tuple<int,int> (i, j), FaceDado.LADO_ESQUERDO);
	}

	private  void adicionaEmDictionary (Dictionary<Tuple<int,int>,HashSet<FaceDado>> dict, Tuple<int,int> key, FaceDado value)
	{
		if (!dict.ContainsKey (key)) {
			dict.Add (key, new HashSet<FaceDado> ());
		}
		dict [key].Add (value);
	}


	private FaceDado getFace (List<TipoMovimentacao> lista)
	{
		GameObject m = new GameObject ();
		m.transform.rotation = Quaternion.identity;
		foreach (TipoMovimentacao t in lista) {
			Vector3 rotacao;
			switch (t) {
			case TipoMovimentacao.LESTE:
				rotacao = new Vector3 (90, 0, 0);
				break;
			case TipoMovimentacao.OESTE:
				rotacao = new Vector3 (-90, 0, 0);
				break;
			case TipoMovimentacao.NORTE:
				rotacao = new Vector3 (0, 0, 90);
				break;
			default:
			case TipoMovimentacao.SUL:
				rotacao = new Vector3 (0, 0, -90);
				break;
			}
			m.transform.Rotate (rotacao, Space.World);
		}
		Destroy (m.gameObject);
		return getFaceByTransform (m.transform);

	}

	private FaceDado getFaceByTransform (Transform t)
	{
		FaceDado face = FaceDado.BASE;
		Predicate<Vector3> apontaParaCima = (v) => v == Vector3.up;
		if (apontaParaCima (t.up)) {
			face = FaceDado.TOPO;
		} else if (apontaParaCima (-t.up)) {
			face = FaceDado.BASE;
		} else if (apontaParaCima (t.right)) {
			face = FaceDado.LADO_DIREITO;
		} else if (apontaParaCima (-t.right)) {
			face = FaceDado.LADO_ESQUERDO;
		} else if (apontaParaCima (t.forward)) {
			face = FaceDado.FRENTE;
		} else if (apontaParaCima (-t.forward)) {
			face = FaceDado.TRAS;
		}
		//print (face + ", rotation: " + t.rotation.eulerAngles + "forward: " + t.forward + "right: " + t.right + "up:" + t.up);
		return face;
	}

	private void aplicaMovimentacao (ref int i, ref int j, TipoMovimentacao tipo)
	{
		switch (tipo) {
		case TipoMovimentacao.LESTE:
			i++;
			break;
		case TipoMovimentacao.OESTE:
			i--;
			break;
		case TipoMovimentacao.NORTE:
			j--;
			break;
		case TipoMovimentacao.SUL:
			j++;
			break;
		}
		
	}
}
