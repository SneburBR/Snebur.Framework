//using System;
//using System.Linq;
//using System.Text;
//using System.Collections.Generic;
//using Snebur.Dominio.Atributos;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace Snebur.Comunicacao
//{
//    [IgnorarGlobalizacao]
//     public class ConfiguracaoServico : BaseComunicao
//    {

//		#region Campos Privados

//    	public long _configuracaoAplicacao_Id;
//    	public string _nomeServico;
//    	public string _urlServico;

//		#endregion

//    	[OcultarColuna]
//    	public long ConfiguracaoAplicacao_Id {  get => this.RetornarValorPropriedade(this._configuracaoAplicacao_Id); set => this.NotificarValorPropriedadeAlterada(this._configuracaoAplicacao_Id, this._configuracaoAplicacao_Id = value); }

//        [OcultarColuna]
//        [ChaveEstrangeira("ConfiguracaoAplicacao_Id")]
//    	[ValidacaoRequerido]
//    	public ConfiguracaoAplicacao ConfiguracaoAplicacao { get; set; }

//    	[Rotulo("Nome do serviço")]
//    	[ValidacaoRequerido]
//    	[ValidacaoTextoTamanho(100)]
//    	public string NomeServico {  get => this.RetornarValorPropriedade(this._nomeServico); set => this.NotificarValorPropriedadeAlterada(this._nomeServico, this._nomeServico = value); }

//    	[Rotulo("Url do serviço")]
//    	[ValidacaoRequerido]
//    	[ValidacaoTextoTamanho(500)]
//    	public string UrlServico {  get => this.RetornarValorPropriedade(this._urlServico); set => this.NotificarValorPropriedadeAlterada(this._urlServico, this._urlServico = value); }

//    }
//}