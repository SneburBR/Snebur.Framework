/*eslint-disable*/
//Data : segunda-feira, 15 de agosto de 2022
//Hora : 06:15:27
//@Namespace: Snebur.AcessoDados
//@PrioridadeDominio: 2
//@Globalizar: False
//@Dominios dependentes: []
namespace Snebur.Reflexao
{
	//Enum
	export const __$tipoEnum_Snebur_AcessoDados_EnumTipoFuncao = new Snebur.Reflexao.TipoEnum("EnumTipoFuncao", "Snebur.AcessoDados", "Snebur.AcessoDados.EnumTipoFuncao, Snebur.AcessoDados");
	export const __$tipoEnum_Snebur_AcessoDados_EnumPosicaoOrdenacao = new Snebur.Reflexao.TipoEnum("EnumPosicaoOrdenacao", "Snebur.AcessoDados", "Snebur.AcessoDados.EnumPosicaoOrdenacao, Snebur.AcessoDados");
	
	//Lista Enum
	export const __$tipoListaEnum_Snebur_AcessoDados_EnumTipoFuncao = new Snebur.Reflexao.TipoListaEnum("ListaTipoEnum_Snebur.AcessoDados.EnumTipoFuncao", "Snebur.AcessoDados", "Snebur.AcessoDados.EnumTipoFuncao, Snebur.AcessoDados", __$tipoEnum_Snebur_AcessoDados_EnumTipoFuncao);
	export const __$tipoListaEnum_Snebur_AcessoDados_EnumPosicaoOrdenacao = new Snebur.Reflexao.TipoListaEnum("ListaTipoEnum_Snebur.AcessoDados.EnumPosicaoOrdenacao", "Snebur.AcessoDados", "Snebur.AcessoDados.EnumPosicaoOrdenacao, Snebur.AcessoDados", __$tipoEnum_Snebur_AcessoDados_EnumPosicaoOrdenacao);
	
	//Adicionar Enum
	$Reflexao.Tipos.Adicionar("Snebur.AcessoDados.EnumTipoFuncao",__$tipoEnum_Snebur_AcessoDados_EnumTipoFuncao);
	$Reflexao.Tipos.Adicionar("Snebur.AcessoDados.EnumPosicaoOrdenacao",__$tipoEnum_Snebur_AcessoDados_EnumPosicaoOrdenacao);
	
	$Reflexao.Tipos.Adicionar("ListaTipoEnum_Snebur_AcessoDados_EnumTipoFuncao",__$tipoListaEnum_Snebur_AcessoDados_EnumTipoFuncao);
	$Reflexao.Tipos.Adicionar("ListaTipoEnum_Snebur_AcessoDados_EnumPosicaoOrdenacao",__$tipoListaEnum_Snebur_AcessoDados_EnumPosicaoOrdenacao);
	
	//Associar caminhos Emil
	Snebur.AcessoDados.EnumTipoFuncao.__CaminhoTipo = "Snebur.AcessoDados.EnumTipoFuncao";
	Snebur.AcessoDados.EnumPosicaoOrdenacao.__CaminhoTipo = "Snebur.AcessoDados.EnumPosicaoOrdenacao";
	
	//BaseDominio
	export const __$tipoBaseDominioSnebur_AcessoDados_BaseAcessoDados = new Snebur.Reflexao.TipoBaseDominio(Snebur.AcessoDados.BaseAcessoDados, "BaseAcessoDados", "Snebur.AcessoDados","Snebur.AcessoDados.BaseAcessoDados, Snebur.AcessoDados",__$tipoBaseDominioSnebur_Dominio_BaseDominio, true);
	export const __$tipoBaseDominioSnebur_AcessoDados_BaseRelacaoAberta = new Snebur.Reflexao.TipoBaseDominio(Snebur.AcessoDados.BaseRelacaoAberta, "BaseRelacaoAberta", "Snebur.AcessoDados","Snebur.AcessoDados.BaseRelacaoAberta, Snebur.AcessoDados",__$tipoBaseDominioSnebur_AcessoDados_BaseAcessoDados, true);
	export const __$tipoBaseDominioSnebur_AcessoDados_EstruturaConsulta = new Snebur.Reflexao.TipoBaseDominio(Snebur.AcessoDados.EstruturaConsulta, "EstruturaConsulta", "Snebur.AcessoDados","Snebur.AcessoDados.EstruturaConsulta, Snebur.AcessoDados",__$tipoBaseDominioSnebur_AcessoDados_BaseAcessoDados, false);
	export const __$tipoBaseDominioSnebur_AcessoDados_BaseFiltro = new Snebur.Reflexao.TipoBaseDominio(Snebur.AcessoDados.BaseFiltro, "BaseFiltro", "Snebur.AcessoDados","Snebur.AcessoDados.BaseFiltro, Snebur.AcessoDados",__$tipoBaseDominioSnebur_AcessoDados_BaseAcessoDados, true);
	export const __$tipoBaseDominioSnebur_AcessoDados_ConsultaFiltroColecao = new Snebur.Reflexao.TipoBaseDominio(Snebur.AcessoDados.ConsultaFiltroColecao, "ConsultaFiltroColecao", "Snebur.AcessoDados","Snebur.AcessoDados.ConsultaFiltroColecao, Snebur.AcessoDados",__$tipoBaseDominioSnebur_AcessoDados_BaseAcessoDados, false);
	export const __$tipoBaseDominioSnebur_AcessoDados_Ordenacao = new Snebur.Reflexao.TipoBaseDominio(Snebur.AcessoDados.Ordenacao, "Ordenacao", "Snebur.AcessoDados","Snebur.AcessoDados.Ordenacao, Snebur.AcessoDados",__$tipoBaseDominioSnebur_AcessoDados_BaseAcessoDados, false);
	export const __$tipoBaseDominioSnebur_AcessoDados_Resultado = new Snebur.Reflexao.TipoBaseDominio(Snebur.AcessoDados.Resultado, "Resultado", "Snebur.AcessoDados","Snebur.AcessoDados.Resultado, Snebur.AcessoDados",__$tipoBaseDominioSnebur_AcessoDados_BaseAcessoDados, true);
	export const __$tipoBaseDominioSnebur_AcessoDados_EntidadeSalva = new Snebur.Reflexao.TipoBaseDominio(Snebur.AcessoDados.EntidadeSalva, "EntidadeSalva", "Snebur.AcessoDados","Snebur.AcessoDados.EntidadeSalva, Snebur.AcessoDados",__$tipoBaseDominioSnebur_AcessoDados_BaseAcessoDados, false);
	export const __$tipoBaseDominioSnebur_AcessoDados_PropriedadeComputada = new Snebur.Reflexao.TipoBaseDominio(Snebur.AcessoDados.PropriedadeComputada, "PropriedadeComputada", "Snebur.AcessoDados","Snebur.AcessoDados.PropriedadeComputada, Snebur.AcessoDados",__$tipoBaseDominioSnebur_AcessoDados_BaseAcessoDados, false);
	export const __$tipoBaseDominioSnebur_AcessoDados_RelacaoAbertaColecao = new Snebur.Reflexao.TipoBaseDominio(Snebur.AcessoDados.RelacaoAbertaColecao, "RelacaoAbertaColecao", "Snebur.AcessoDados","Snebur.AcessoDados.RelacaoAbertaColecao, Snebur.AcessoDados",__$tipoBaseDominioSnebur_AcessoDados_BaseRelacaoAberta, false);
	export const __$tipoBaseDominioSnebur_AcessoDados_RelacaoAbertaEntidade = new Snebur.Reflexao.TipoBaseDominio(Snebur.AcessoDados.RelacaoAbertaEntidade, "RelacaoAbertaEntidade", "Snebur.AcessoDados","Snebur.AcessoDados.RelacaoAbertaEntidade, Snebur.AcessoDados",__$tipoBaseDominioSnebur_AcessoDados_BaseRelacaoAberta, false);
	export const __$tipoBaseDominioSnebur_AcessoDados_FiltroPropriedadeIn = new Snebur.Reflexao.TipoBaseDominio(Snebur.AcessoDados.FiltroPropriedadeIn, "FiltroPropriedadeIn", "Snebur.AcessoDados","Snebur.AcessoDados.FiltroPropriedadeIn, Snebur.AcessoDados",__$tipoBaseDominioSnebur_AcessoDados_BaseFiltro, false);
	export const __$tipoBaseDominioSnebur_AcessoDados_FiltroIds = new Snebur.Reflexao.TipoBaseDominio(Snebur.AcessoDados.FiltroIds, "FiltroIds", "Snebur.AcessoDados","Snebur.AcessoDados.FiltroIds, Snebur.AcessoDados",__$tipoBaseDominioSnebur_AcessoDados_BaseFiltro, false);
	export const __$tipoBaseDominioSnebur_AcessoDados_BaseFiltroGrupo = new Snebur.Reflexao.TipoBaseDominio(Snebur.AcessoDados.BaseFiltroGrupo, "BaseFiltroGrupo", "Snebur.AcessoDados","Snebur.AcessoDados.BaseFiltroGrupo, Snebur.AcessoDados",__$tipoBaseDominioSnebur_AcessoDados_BaseFiltro, true);
	export const __$tipoBaseDominioSnebur_AcessoDados_FiltroPropriedade = new Snebur.Reflexao.TipoBaseDominio(Snebur.AcessoDados.FiltroPropriedade, "FiltroPropriedade", "Snebur.AcessoDados","Snebur.AcessoDados.FiltroPropriedade, Snebur.AcessoDados",__$tipoBaseDominioSnebur_AcessoDados_BaseFiltro, false);
	export const __$tipoBaseDominioSnebur_AcessoDados_FiltroGrupoE = new Snebur.Reflexao.TipoBaseDominio(Snebur.AcessoDados.FiltroGrupoE, "FiltroGrupoE", "Snebur.AcessoDados","Snebur.AcessoDados.FiltroGrupoE, Snebur.AcessoDados",__$tipoBaseDominioSnebur_AcessoDados_BaseFiltroGrupo, false);
	export const __$tipoBaseDominioSnebur_AcessoDados_FiltroGrupoNAO = new Snebur.Reflexao.TipoBaseDominio(Snebur.AcessoDados.FiltroGrupoNAO, "FiltroGrupoNAO", "Snebur.AcessoDados","Snebur.AcessoDados.FiltroGrupoNAO, Snebur.AcessoDados",__$tipoBaseDominioSnebur_AcessoDados_BaseFiltroGrupo, false);
	export const __$tipoBaseDominioSnebur_AcessoDados_FiltroGrupoOU = new Snebur.Reflexao.TipoBaseDominio(Snebur.AcessoDados.FiltroGrupoOU, "FiltroGrupoOU", "Snebur.AcessoDados","Snebur.AcessoDados.FiltroGrupoOU, Snebur.AcessoDados",__$tipoBaseDominioSnebur_AcessoDados_BaseFiltroGrupo, false);
	export const __$tipoBaseDominioSnebur_AcessoDados_ResultadoExcluir = new Snebur.Reflexao.TipoBaseDominio(Snebur.AcessoDados.ResultadoExcluir, "ResultadoExcluir", "Snebur.AcessoDados","Snebur.AcessoDados.ResultadoExcluir, Snebur.AcessoDados",__$tipoBaseDominioSnebur_AcessoDados_Resultado, false);
	export const __$tipoBaseDominioSnebur_AcessoDados_ResultadoConsulta = new Snebur.Reflexao.TipoBaseDominio(Snebur.AcessoDados.ResultadoConsulta, "ResultadoConsulta", "Snebur.AcessoDados","Snebur.AcessoDados.ResultadoConsulta, Snebur.AcessoDados",__$tipoBaseDominioSnebur_AcessoDados_Resultado, false);
	export const __$tipoBaseDominioSnebur_AcessoDados_ResultadoSalvar = new Snebur.Reflexao.TipoBaseDominio(Snebur.AcessoDados.ResultadoSalvar, "ResultadoSalvar", "Snebur.AcessoDados","Snebur.AcessoDados.ResultadoSalvar, Snebur.AcessoDados",__$tipoBaseDominioSnebur_AcessoDados_Resultado, false);
	
	//ListaBaseDominio
	export const __$tipoListaBaseDominio_Snebur_AcessoDados_BaseAcessoDados = new Snebur.Reflexao.TipoListaBaseDominio("ListaTipoBaseDominio_BaseAcessoDados", "Snebur.AcessoDados", "System.Collections.Generic.List`1[[Snebur.AcessoDados.BaseAcessoDados, Snebur.AcessoDados]], mscorlib",__$tipoBaseDominioSnebur_AcessoDados_BaseAcessoDados);
	export const __$tipoListaBaseDominio_Snebur_AcessoDados_BaseRelacaoAberta = new Snebur.Reflexao.TipoListaBaseDominio("ListaTipoBaseDominio_BaseRelacaoAberta", "Snebur.AcessoDados", "System.Collections.Generic.List`1[[Snebur.AcessoDados.BaseRelacaoAberta, Snebur.AcessoDados]], mscorlib",__$tipoBaseDominioSnebur_AcessoDados_BaseRelacaoAberta);
	export const __$tipoListaBaseDominio_Snebur_AcessoDados_EstruturaConsulta = new Snebur.Reflexao.TipoListaBaseDominio("ListaTipoBaseDominio_EstruturaConsulta", "Snebur.AcessoDados", "System.Collections.Generic.List`1[[Snebur.AcessoDados.EstruturaConsulta, Snebur.AcessoDados]], mscorlib",__$tipoBaseDominioSnebur_AcessoDados_EstruturaConsulta);
	export const __$tipoListaBaseDominio_Snebur_AcessoDados_BaseFiltro = new Snebur.Reflexao.TipoListaBaseDominio("ListaTipoBaseDominio_BaseFiltro", "Snebur.AcessoDados", "System.Collections.Generic.List`1[[Snebur.AcessoDados.BaseFiltro, Snebur.AcessoDados]], mscorlib",__$tipoBaseDominioSnebur_AcessoDados_BaseFiltro);
	export const __$tipoListaBaseDominio_Snebur_AcessoDados_ConsultaFiltroColecao = new Snebur.Reflexao.TipoListaBaseDominio("ListaTipoBaseDominio_ConsultaFiltroColecao", "Snebur.AcessoDados", "System.Collections.Generic.List`1[[Snebur.AcessoDados.ConsultaFiltroColecao, Snebur.AcessoDados]], mscorlib",__$tipoBaseDominioSnebur_AcessoDados_ConsultaFiltroColecao);
	export const __$tipoListaBaseDominio_Snebur_AcessoDados_Ordenacao = new Snebur.Reflexao.TipoListaBaseDominio("ListaTipoBaseDominio_Ordenacao", "Snebur.AcessoDados", "System.Collections.Generic.List`1[[Snebur.AcessoDados.Ordenacao, Snebur.AcessoDados]], mscorlib",__$tipoBaseDominioSnebur_AcessoDados_Ordenacao);
	export const __$tipoListaBaseDominio_Snebur_AcessoDados_Resultado = new Snebur.Reflexao.TipoListaBaseDominio("ListaTipoBaseDominio_Resultado", "Snebur.AcessoDados", "System.Collections.Generic.List`1[[Snebur.AcessoDados.Resultado, Snebur.AcessoDados]], mscorlib",__$tipoBaseDominioSnebur_AcessoDados_Resultado);
	export const __$tipoListaBaseDominio_Snebur_AcessoDados_EntidadeSalva = new Snebur.Reflexao.TipoListaBaseDominio("ListaTipoBaseDominio_EntidadeSalva", "Snebur.AcessoDados", "System.Collections.Generic.List`1[[Snebur.AcessoDados.EntidadeSalva, Snebur.AcessoDados]], mscorlib",__$tipoBaseDominioSnebur_AcessoDados_EntidadeSalva);
	export const __$tipoListaBaseDominio_Snebur_AcessoDados_PropriedadeComputada = new Snebur.Reflexao.TipoListaBaseDominio("ListaTipoBaseDominio_PropriedadeComputada", "Snebur.AcessoDados", "System.Collections.Generic.List`1[[Snebur.AcessoDados.PropriedadeComputada, Snebur.AcessoDados]], mscorlib",__$tipoBaseDominioSnebur_AcessoDados_PropriedadeComputada);
	export const __$tipoListaBaseDominio_Snebur_AcessoDados_RelacaoAbertaColecao = new Snebur.Reflexao.TipoListaBaseDominio("ListaTipoBaseDominio_RelacaoAbertaColecao", "Snebur.AcessoDados", "System.Collections.Generic.List`1[[Snebur.AcessoDados.RelacaoAbertaColecao, Snebur.AcessoDados]], mscorlib",__$tipoBaseDominioSnebur_AcessoDados_RelacaoAbertaColecao);
	export const __$tipoListaBaseDominio_Snebur_AcessoDados_RelacaoAbertaEntidade = new Snebur.Reflexao.TipoListaBaseDominio("ListaTipoBaseDominio_RelacaoAbertaEntidade", "Snebur.AcessoDados", "System.Collections.Generic.List`1[[Snebur.AcessoDados.RelacaoAbertaEntidade, Snebur.AcessoDados]], mscorlib",__$tipoBaseDominioSnebur_AcessoDados_RelacaoAbertaEntidade);
	export const __$tipoListaBaseDominio_Snebur_AcessoDados_FiltroPropriedadeIn = new Snebur.Reflexao.TipoListaBaseDominio("ListaTipoBaseDominio_FiltroPropriedadeIn", "Snebur.AcessoDados", "System.Collections.Generic.List`1[[Snebur.AcessoDados.FiltroPropriedadeIn, Snebur.AcessoDados]], mscorlib",__$tipoBaseDominioSnebur_AcessoDados_FiltroPropriedadeIn);
	export const __$tipoListaBaseDominio_Snebur_AcessoDados_FiltroIds = new Snebur.Reflexao.TipoListaBaseDominio("ListaTipoBaseDominio_FiltroIds", "Snebur.AcessoDados", "System.Collections.Generic.List`1[[Snebur.AcessoDados.FiltroIds, Snebur.AcessoDados]], mscorlib",__$tipoBaseDominioSnebur_AcessoDados_FiltroIds);
	export const __$tipoListaBaseDominio_Snebur_AcessoDados_BaseFiltroGrupo = new Snebur.Reflexao.TipoListaBaseDominio("ListaTipoBaseDominio_BaseFiltroGrupo", "Snebur.AcessoDados", "System.Collections.Generic.List`1[[Snebur.AcessoDados.BaseFiltroGrupo, Snebur.AcessoDados]], mscorlib",__$tipoBaseDominioSnebur_AcessoDados_BaseFiltroGrupo);
	export const __$tipoListaBaseDominio_Snebur_AcessoDados_FiltroPropriedade = new Snebur.Reflexao.TipoListaBaseDominio("ListaTipoBaseDominio_FiltroPropriedade", "Snebur.AcessoDados", "System.Collections.Generic.List`1[[Snebur.AcessoDados.FiltroPropriedade, Snebur.AcessoDados]], mscorlib",__$tipoBaseDominioSnebur_AcessoDados_FiltroPropriedade);
	export const __$tipoListaBaseDominio_Snebur_AcessoDados_FiltroGrupoE = new Snebur.Reflexao.TipoListaBaseDominio("ListaTipoBaseDominio_FiltroGrupoE", "Snebur.AcessoDados", "System.Collections.Generic.List`1[[Snebur.AcessoDados.FiltroGrupoE, Snebur.AcessoDados]], mscorlib",__$tipoBaseDominioSnebur_AcessoDados_FiltroGrupoE);
	export const __$tipoListaBaseDominio_Snebur_AcessoDados_FiltroGrupoNAO = new Snebur.Reflexao.TipoListaBaseDominio("ListaTipoBaseDominio_FiltroGrupoNAO", "Snebur.AcessoDados", "System.Collections.Generic.List`1[[Snebur.AcessoDados.FiltroGrupoNAO, Snebur.AcessoDados]], mscorlib",__$tipoBaseDominioSnebur_AcessoDados_FiltroGrupoNAO);
	export const __$tipoListaBaseDominio_Snebur_AcessoDados_FiltroGrupoOU = new Snebur.Reflexao.TipoListaBaseDominio("ListaTipoBaseDominio_FiltroGrupoOU", "Snebur.AcessoDados", "System.Collections.Generic.List`1[[Snebur.AcessoDados.FiltroGrupoOU, Snebur.AcessoDados]], mscorlib",__$tipoBaseDominioSnebur_AcessoDados_FiltroGrupoOU);
	export const __$tipoListaBaseDominio_Snebur_AcessoDados_ResultadoExcluir = new Snebur.Reflexao.TipoListaBaseDominio("ListaTipoBaseDominio_ResultadoExcluir", "Snebur.AcessoDados", "System.Collections.Generic.List`1[[Snebur.AcessoDados.ResultadoExcluir, Snebur.AcessoDados]], mscorlib",__$tipoBaseDominioSnebur_AcessoDados_ResultadoExcluir);
	export const __$tipoListaBaseDominio_Snebur_AcessoDados_ResultadoConsulta = new Snebur.Reflexao.TipoListaBaseDominio("ListaTipoBaseDominio_ResultadoConsulta", "Snebur.AcessoDados", "System.Collections.Generic.List`1[[Snebur.AcessoDados.ResultadoConsulta, Snebur.AcessoDados]], mscorlib",__$tipoBaseDominioSnebur_AcessoDados_ResultadoConsulta);
	export const __$tipoListaBaseDominio_Snebur_AcessoDados_ResultadoSalvar = new Snebur.Reflexao.TipoListaBaseDominio("ListaTipoBaseDominio_ResultadoSalvar", "Snebur.AcessoDados", "System.Collections.Generic.List`1[[Snebur.AcessoDados.ResultadoSalvar, Snebur.AcessoDados]], mscorlib",__$tipoBaseDominioSnebur_AcessoDados_ResultadoSalvar);
	
	//Adicionar BasesDominio
	$Reflexao.Tipos.Adicionar("Snebur.AcessoDados.BaseAcessoDados",__$tipoBaseDominioSnebur_AcessoDados_BaseAcessoDados);
	$Reflexao.Tipos.Adicionar("Snebur.AcessoDados.BaseRelacaoAberta",__$tipoBaseDominioSnebur_AcessoDados_BaseRelacaoAberta);
	$Reflexao.Tipos.Adicionar("Snebur.AcessoDados.EstruturaConsulta",__$tipoBaseDominioSnebur_AcessoDados_EstruturaConsulta);
	$Reflexao.Tipos.Adicionar("Snebur.AcessoDados.BaseFiltro",__$tipoBaseDominioSnebur_AcessoDados_BaseFiltro);
	$Reflexao.Tipos.Adicionar("Snebur.AcessoDados.ConsultaFiltroColecao",__$tipoBaseDominioSnebur_AcessoDados_ConsultaFiltroColecao);
	$Reflexao.Tipos.Adicionar("Snebur.AcessoDados.Ordenacao",__$tipoBaseDominioSnebur_AcessoDados_Ordenacao);
	$Reflexao.Tipos.Adicionar("Snebur.AcessoDados.Resultado",__$tipoBaseDominioSnebur_AcessoDados_Resultado);
	$Reflexao.Tipos.Adicionar("Snebur.AcessoDados.EntidadeSalva",__$tipoBaseDominioSnebur_AcessoDados_EntidadeSalva);
	$Reflexao.Tipos.Adicionar("Snebur.AcessoDados.PropriedadeComputada",__$tipoBaseDominioSnebur_AcessoDados_PropriedadeComputada);
	$Reflexao.Tipos.Adicionar("Snebur.AcessoDados.RelacaoAbertaColecao",__$tipoBaseDominioSnebur_AcessoDados_RelacaoAbertaColecao);
	$Reflexao.Tipos.Adicionar("Snebur.AcessoDados.RelacaoAbertaEntidade",__$tipoBaseDominioSnebur_AcessoDados_RelacaoAbertaEntidade);
	$Reflexao.Tipos.Adicionar("Snebur.AcessoDados.FiltroPropriedadeIn",__$tipoBaseDominioSnebur_AcessoDados_FiltroPropriedadeIn);
	$Reflexao.Tipos.Adicionar("Snebur.AcessoDados.FiltroIds",__$tipoBaseDominioSnebur_AcessoDados_FiltroIds);
	$Reflexao.Tipos.Adicionar("Snebur.AcessoDados.BaseFiltroGrupo",__$tipoBaseDominioSnebur_AcessoDados_BaseFiltroGrupo);
	$Reflexao.Tipos.Adicionar("Snebur.AcessoDados.FiltroPropriedade",__$tipoBaseDominioSnebur_AcessoDados_FiltroPropriedade);
	$Reflexao.Tipos.Adicionar("Snebur.AcessoDados.FiltroGrupoE",__$tipoBaseDominioSnebur_AcessoDados_FiltroGrupoE);
	$Reflexao.Tipos.Adicionar("Snebur.AcessoDados.FiltroGrupoNAO",__$tipoBaseDominioSnebur_AcessoDados_FiltroGrupoNAO);
	$Reflexao.Tipos.Adicionar("Snebur.AcessoDados.FiltroGrupoOU",__$tipoBaseDominioSnebur_AcessoDados_FiltroGrupoOU);
	$Reflexao.Tipos.Adicionar("Snebur.AcessoDados.ResultadoExcluir",__$tipoBaseDominioSnebur_AcessoDados_ResultadoExcluir);
	$Reflexao.Tipos.Adicionar("Snebur.AcessoDados.ResultadoConsulta",__$tipoBaseDominioSnebur_AcessoDados_ResultadoConsulta);
	$Reflexao.Tipos.Adicionar("Snebur.AcessoDados.ResultadoSalvar",__$tipoBaseDominioSnebur_AcessoDados_ResultadoSalvar);
	
	$Reflexao.Tipos.Adicionar("ListaTipoBaseDominio_Snebur.AcessoDados.BaseAcessoDados",__$tipoListaBaseDominio_Snebur_AcessoDados_BaseAcessoDados);
	$Reflexao.Tipos.Adicionar("ListaTipoBaseDominio_Snebur.AcessoDados.BaseRelacaoAberta",__$tipoListaBaseDominio_Snebur_AcessoDados_BaseRelacaoAberta);
	$Reflexao.Tipos.Adicionar("ListaTipoBaseDominio_Snebur.AcessoDados.EstruturaConsulta",__$tipoListaBaseDominio_Snebur_AcessoDados_EstruturaConsulta);
	$Reflexao.Tipos.Adicionar("ListaTipoBaseDominio_Snebur.AcessoDados.BaseFiltro",__$tipoListaBaseDominio_Snebur_AcessoDados_BaseFiltro);
	$Reflexao.Tipos.Adicionar("ListaTipoBaseDominio_Snebur.AcessoDados.ConsultaFiltroColecao",__$tipoListaBaseDominio_Snebur_AcessoDados_ConsultaFiltroColecao);
	$Reflexao.Tipos.Adicionar("ListaTipoBaseDominio_Snebur.AcessoDados.Ordenacao",__$tipoListaBaseDominio_Snebur_AcessoDados_Ordenacao);
	$Reflexao.Tipos.Adicionar("ListaTipoBaseDominio_Snebur.AcessoDados.Resultado",__$tipoListaBaseDominio_Snebur_AcessoDados_Resultado);
	$Reflexao.Tipos.Adicionar("ListaTipoBaseDominio_Snebur.AcessoDados.EntidadeSalva",__$tipoListaBaseDominio_Snebur_AcessoDados_EntidadeSalva);
	$Reflexao.Tipos.Adicionar("ListaTipoBaseDominio_Snebur.AcessoDados.PropriedadeComputada",__$tipoListaBaseDominio_Snebur_AcessoDados_PropriedadeComputada);
	$Reflexao.Tipos.Adicionar("ListaTipoBaseDominio_Snebur.AcessoDados.RelacaoAbertaColecao",__$tipoListaBaseDominio_Snebur_AcessoDados_RelacaoAbertaColecao);
	$Reflexao.Tipos.Adicionar("ListaTipoBaseDominio_Snebur.AcessoDados.RelacaoAbertaEntidade",__$tipoListaBaseDominio_Snebur_AcessoDados_RelacaoAbertaEntidade);
	$Reflexao.Tipos.Adicionar("ListaTipoBaseDominio_Snebur.AcessoDados.FiltroPropriedadeIn",__$tipoListaBaseDominio_Snebur_AcessoDados_FiltroPropriedadeIn);
	$Reflexao.Tipos.Adicionar("ListaTipoBaseDominio_Snebur.AcessoDados.FiltroIds",__$tipoListaBaseDominio_Snebur_AcessoDados_FiltroIds);
	$Reflexao.Tipos.Adicionar("ListaTipoBaseDominio_Snebur.AcessoDados.BaseFiltroGrupo",__$tipoListaBaseDominio_Snebur_AcessoDados_BaseFiltroGrupo);
	$Reflexao.Tipos.Adicionar("ListaTipoBaseDominio_Snebur.AcessoDados.FiltroPropriedade",__$tipoListaBaseDominio_Snebur_AcessoDados_FiltroPropriedade);
	$Reflexao.Tipos.Adicionar("ListaTipoBaseDominio_Snebur.AcessoDados.FiltroGrupoE",__$tipoListaBaseDominio_Snebur_AcessoDados_FiltroGrupoE);
	$Reflexao.Tipos.Adicionar("ListaTipoBaseDominio_Snebur.AcessoDados.FiltroGrupoNAO",__$tipoListaBaseDominio_Snebur_AcessoDados_FiltroGrupoNAO);
	$Reflexao.Tipos.Adicionar("ListaTipoBaseDominio_Snebur.AcessoDados.FiltroGrupoOU",__$tipoListaBaseDominio_Snebur_AcessoDados_FiltroGrupoOU);
	$Reflexao.Tipos.Adicionar("ListaTipoBaseDominio_Snebur.AcessoDados.ResultadoExcluir",__$tipoListaBaseDominio_Snebur_AcessoDados_ResultadoExcluir);
	$Reflexao.Tipos.Adicionar("ListaTipoBaseDominio_Snebur.AcessoDados.ResultadoConsulta",__$tipoListaBaseDominio_Snebur_AcessoDados_ResultadoConsulta);
	$Reflexao.Tipos.Adicionar("ListaTipoBaseDominio_Snebur.AcessoDados.ResultadoSalvar",__$tipoListaBaseDominio_Snebur_AcessoDados_ResultadoSalvar);
	
	//Associar caminhos BaseDominio
	
	Snebur.AcessoDados.BaseAcessoDados.__CaminhoTipo = "Snebur.AcessoDados.BaseAcessoDados";
	Snebur.AcessoDados.BaseRelacaoAberta.__CaminhoTipo = "Snebur.AcessoDados.BaseRelacaoAberta";
	Snebur.AcessoDados.EstruturaConsulta.__CaminhoTipo = "Snebur.AcessoDados.EstruturaConsulta";
	Snebur.AcessoDados.BaseFiltro.__CaminhoTipo = "Snebur.AcessoDados.BaseFiltro";
	Snebur.AcessoDados.ConsultaFiltroColecao.__CaminhoTipo = "Snebur.AcessoDados.ConsultaFiltroColecao";
	Snebur.AcessoDados.Ordenacao.__CaminhoTipo = "Snebur.AcessoDados.Ordenacao";
	Snebur.AcessoDados.Resultado.__CaminhoTipo = "Snebur.AcessoDados.Resultado";
	Snebur.AcessoDados.EntidadeSalva.__CaminhoTipo = "Snebur.AcessoDados.EntidadeSalva";
	Snebur.AcessoDados.PropriedadeComputada.__CaminhoTipo = "Snebur.AcessoDados.PropriedadeComputada";
	Snebur.AcessoDados.RelacaoAbertaColecao.__CaminhoTipo = "Snebur.AcessoDados.RelacaoAbertaColecao";
	Snebur.AcessoDados.RelacaoAbertaEntidade.__CaminhoTipo = "Snebur.AcessoDados.RelacaoAbertaEntidade";
	Snebur.AcessoDados.FiltroPropriedadeIn.__CaminhoTipo = "Snebur.AcessoDados.FiltroPropriedadeIn";
	Snebur.AcessoDados.FiltroIds.__CaminhoTipo = "Snebur.AcessoDados.FiltroIds";
	Snebur.AcessoDados.BaseFiltroGrupo.__CaminhoTipo = "Snebur.AcessoDados.BaseFiltroGrupo";
	Snebur.AcessoDados.FiltroPropriedade.__CaminhoTipo = "Snebur.AcessoDados.FiltroPropriedade";
	Snebur.AcessoDados.FiltroGrupoE.__CaminhoTipo = "Snebur.AcessoDados.FiltroGrupoE";
	Snebur.AcessoDados.FiltroGrupoNAO.__CaminhoTipo = "Snebur.AcessoDados.FiltroGrupoNAO";
	Snebur.AcessoDados.FiltroGrupoOU.__CaminhoTipo = "Snebur.AcessoDados.FiltroGrupoOU";
	Snebur.AcessoDados.ResultadoExcluir.__CaminhoTipo = "Snebur.AcessoDados.ResultadoExcluir";
	Snebur.AcessoDados.ResultadoConsulta.__CaminhoTipo = "Snebur.AcessoDados.ResultadoConsulta";
	Snebur.AcessoDados.ResultadoSalvar.__CaminhoTipo = "Snebur.AcessoDados.ResultadoSalvar";
	
	//Atributos
	
	//TiposComplexos
	
	//ListaBaseDominio TipoComplexo
	
	//Adicionar TiposComplexos
	
	//Adicionar ListaBaseDominio TiposComplexos
	
	//Associar caminhos TiposComplexos
	
	//Atributos TiposComplexos
	
	//BaseEntidades
	
	//ListaBaseDominio TipoEntidade
	
	//ListaBaseEntidades
	
	//Adicionar BaseEntidades
	
	//Adicionar ListaBaseDominio TipoEntidade
	
	//Adicionar ListaBaseEntidades
	
	//Associar caminhos BaseEntidades
	
	//Atributos TipoEntidade
	
	export const __$propriedade_Snebur_AcessoDados_BaseAcessoDados_MensagemErro = new Snebur.Reflexao.Propriedade("MensagemErro", __$tipoTipoPrimario_String, __$tipoBaseDominioSnebur_AcessoDados_BaseAcessoDados, true);
	
	export const __$propriedade_Snebur_AcessoDados_BaseAcessoDados_FalhaAutenticacao = new Snebur.Reflexao.Propriedade("FalhaAutenticacao", __$tipoTipoPrimario_Boolean, __$tipoBaseDominioSnebur_AcessoDados_BaseAcessoDados, false);
	
	export const __$propriedade_Snebur_AcessoDados_BaseAcessoDados_Comandos = new Snebur.Reflexao.Propriedade("Comandos", __$tipoListaTipoPrimario_String, __$tipoBaseDominioSnebur_AcessoDados_BaseAcessoDados, true);
	
	__$tipoBaseDominioSnebur_AcessoDados_BaseAcessoDados.Propriedades.Add(__$propriedade_Snebur_AcessoDados_BaseAcessoDados_MensagemErro);
	__$tipoBaseDominioSnebur_AcessoDados_BaseAcessoDados.Propriedades.Add(__$propriedade_Snebur_AcessoDados_BaseAcessoDados_FalhaAutenticacao);
	__$tipoBaseDominioSnebur_AcessoDados_BaseAcessoDados.Propriedades.Add(__$propriedade_Snebur_AcessoDados_BaseAcessoDados_Comandos);
	
	export const __$propriedade_Snebur_AcessoDados_BaseRelacaoAberta_CaminhoPropriedade = new Snebur.Reflexao.Propriedade("CaminhoPropriedade", __$tipoTipoPrimario_String, __$tipoBaseDominioSnebur_AcessoDados_BaseRelacaoAberta, true);
	
	export const __$propriedade_Snebur_AcessoDados_BaseRelacaoAberta_NomeTipoEntidade = new Snebur.Reflexao.Propriedade("NomeTipoEntidade", __$tipoTipoPrimario_String, __$tipoBaseDominioSnebur_AcessoDados_BaseRelacaoAberta, true);
	
	export const __$propriedade_Snebur_AcessoDados_BaseRelacaoAberta_TipoEntidadeAssemblyQualifiedName = new Snebur.Reflexao.Propriedade("TipoEntidadeAssemblyQualifiedName", __$tipoTipoPrimario_String, __$tipoBaseDominioSnebur_AcessoDados_BaseRelacaoAberta, true);
	
	export const __$propriedade_Snebur_AcessoDados_BaseRelacaoAberta_NomeTipoDeclarado = new Snebur.Reflexao.Propriedade("NomeTipoDeclarado", __$tipoTipoPrimario_String, __$tipoBaseDominioSnebur_AcessoDados_BaseRelacaoAberta, true);
	
	export const __$propriedade_Snebur_AcessoDados_BaseRelacaoAberta_TipoDeclaradoAssemblyQualifiedName = new Snebur.Reflexao.Propriedade("TipoDeclaradoAssemblyQualifiedName", __$tipoTipoPrimario_String, __$tipoBaseDominioSnebur_AcessoDados_BaseRelacaoAberta, true);
	
	export const __$propriedade_Snebur_AcessoDados_BaseRelacaoAberta_PropriedadesAbertas = new Snebur.Reflexao.Propriedade("PropriedadesAbertas",  new Snebur.Reflexao.TipoHashSet(__$tipoTipoPrimario_String), __$tipoBaseDominioSnebur_AcessoDados_BaseRelacaoAberta, true);
	
	__$tipoBaseDominioSnebur_AcessoDados_BaseRelacaoAberta.Propriedades.Add(__$propriedade_Snebur_AcessoDados_BaseRelacaoAberta_CaminhoPropriedade);
	__$tipoBaseDominioSnebur_AcessoDados_BaseRelacaoAberta.Propriedades.Add(__$propriedade_Snebur_AcessoDados_BaseRelacaoAberta_NomeTipoEntidade);
	__$tipoBaseDominioSnebur_AcessoDados_BaseRelacaoAberta.Propriedades.Add(__$propriedade_Snebur_AcessoDados_BaseRelacaoAberta_TipoEntidadeAssemblyQualifiedName);
	__$tipoBaseDominioSnebur_AcessoDados_BaseRelacaoAberta.Propriedades.Add(__$propriedade_Snebur_AcessoDados_BaseRelacaoAberta_NomeTipoDeclarado);
	__$tipoBaseDominioSnebur_AcessoDados_BaseRelacaoAberta.Propriedades.Add(__$propriedade_Snebur_AcessoDados_BaseRelacaoAberta_TipoDeclaradoAssemblyQualifiedName);
	__$tipoBaseDominioSnebur_AcessoDados_BaseRelacaoAberta.Propriedades.Add(__$propriedade_Snebur_AcessoDados_BaseRelacaoAberta_PropriedadesAbertas);
	
	export const __$propriedade_Snebur_AcessoDados_EstruturaConsulta_NomeTipoEntidade = new Snebur.Reflexao.Propriedade("NomeTipoEntidade", __$tipoTipoPrimario_String, __$tipoBaseDominioSnebur_AcessoDados_EstruturaConsulta, true);
	
	export const __$propriedade_Snebur_AcessoDados_EstruturaConsulta_TipoEntidadeAssemblyQualifiedName = new Snebur.Reflexao.Propriedade("TipoEntidadeAssemblyQualifiedName", __$tipoTipoPrimario_String, __$tipoBaseDominioSnebur_AcessoDados_EstruturaConsulta, true);
	
	export const __$propriedade_Snebur_AcessoDados_EstruturaConsulta_IsMostrarDeletados = new Snebur.Reflexao.Propriedade("IsMostrarDeletados", __$tipoTipoPrimario_Boolean, __$tipoBaseDominioSnebur_AcessoDados_EstruturaConsulta, false);
	
	export const __$propriedade_Snebur_AcessoDados_EstruturaConsulta_IsMostrarInativos = new Snebur.Reflexao.Propriedade("IsMostrarInativos", __$tipoTipoPrimario_Boolean, __$tipoBaseDominioSnebur_AcessoDados_EstruturaConsulta, false);
	
	export const __$propriedade_Snebur_AcessoDados_EstruturaConsulta_IsDesativarOrdenacao = new Snebur.Reflexao.Propriedade("IsDesativarOrdenacao", __$tipoTipoPrimario_Boolean, __$tipoBaseDominioSnebur_AcessoDados_EstruturaConsulta, false);
	
	export const __$propriedade_Snebur_AcessoDados_EstruturaConsulta_Take = new Snebur.Reflexao.Propriedade("Take", __$tipoTipoPrimario_Integer, __$tipoBaseDominioSnebur_AcessoDados_EstruturaConsulta, false);
	
	export const __$propriedade_Snebur_AcessoDados_EstruturaConsulta_Skip = new Snebur.Reflexao.Propriedade("Skip", __$tipoTipoPrimario_Integer, __$tipoBaseDominioSnebur_AcessoDados_EstruturaConsulta, false);
	
	export const __$propriedade_Snebur_AcessoDados_EstruturaConsulta_PaginaAtual = new Snebur.Reflexao.Propriedade("PaginaAtual", __$tipoTipoPrimario_Integer, __$tipoBaseDominioSnebur_AcessoDados_EstruturaConsulta, false);
	
	export const __$propriedade_Snebur_AcessoDados_EstruturaConsulta_CaminhoPropriedadeFuncao = new Snebur.Reflexao.Propriedade("CaminhoPropriedadeFuncao", __$tipoTipoPrimario_String, __$tipoBaseDominioSnebur_AcessoDados_EstruturaConsulta, true);
	
	export const __$propriedade_Snebur_AcessoDados_EstruturaConsulta_TipoFuncaoEnum = new Snebur.Reflexao.Propriedade("TipoFuncaoEnum", __$tipoEnum_Snebur_AcessoDados_EnumTipoFuncao, __$tipoBaseDominioSnebur_AcessoDados_EstruturaConsulta, false);
	
	export const __$propriedade_Snebur_AcessoDados_EstruturaConsulta_FiltroGrupoE = new Snebur.Reflexao.Propriedade("FiltroGrupoE", __$tipoBaseDominioSnebur_AcessoDados_FiltroGrupoE, __$tipoBaseDominioSnebur_AcessoDados_EstruturaConsulta, true);
	
	export const __$propriedade_Snebur_AcessoDados_EstruturaConsulta_FiltroGrupoOU = new Snebur.Reflexao.Propriedade("FiltroGrupoOU", __$tipoBaseDominioSnebur_AcessoDados_FiltroGrupoOU, __$tipoBaseDominioSnebur_AcessoDados_EstruturaConsulta, true);
	
	export const __$propriedade_Snebur_AcessoDados_EstruturaConsulta_Ordenacoes = new Snebur.Reflexao.Propriedade("Ordenacoes",  new Snebur.Reflexao.TipoDicionario(__$tipoBaseDominioSnebur_AcessoDados_Ordenacao), __$tipoBaseDominioSnebur_AcessoDados_EstruturaConsulta, true);
	
	export const __$propriedade_Snebur_AcessoDados_EstruturaConsulta_RelacoesAbertaFiltro = new Snebur.Reflexao.Propriedade("RelacoesAbertaFiltro",  new Snebur.Reflexao.TipoDicionario(__$tipoBaseDominioSnebur_AcessoDados_RelacaoAbertaEntidade), __$tipoBaseDominioSnebur_AcessoDados_EstruturaConsulta, true);
	
	export const __$propriedade_Snebur_AcessoDados_EstruturaConsulta_RelacoesAberta = new Snebur.Reflexao.Propriedade("RelacoesAberta",  new Snebur.Reflexao.TipoDicionario(__$tipoBaseDominioSnebur_AcessoDados_RelacaoAbertaEntidade), __$tipoBaseDominioSnebur_AcessoDados_EstruturaConsulta, true);
	
	export const __$propriedade_Snebur_AcessoDados_EstruturaConsulta_ColecoesAberta = new Snebur.Reflexao.Propriedade("ColecoesAberta",  new Snebur.Reflexao.TipoDicionario(__$tipoBaseDominioSnebur_AcessoDados_RelacaoAbertaColecao), __$tipoBaseDominioSnebur_AcessoDados_EstruturaConsulta, true);
	
	export const __$propriedade_Snebur_AcessoDados_EstruturaConsulta_PropriedadesAbertas = new Snebur.Reflexao.Propriedade("PropriedadesAbertas", __$tipoListaTipoPrimario_String, __$tipoBaseDominioSnebur_AcessoDados_EstruturaConsulta, true);
	
	export const __$propriedade_Snebur_AcessoDados_EstruturaConsulta_ContarRegistros = new Snebur.Reflexao.Propriedade("ContarRegistros", __$tipoTipoPrimario_Boolean, __$tipoBaseDominioSnebur_AcessoDados_EstruturaConsulta, false);
	
	__$tipoBaseDominioSnebur_AcessoDados_EstruturaConsulta.Propriedades.Add(__$propriedade_Snebur_AcessoDados_EstruturaConsulta_NomeTipoEntidade);
	__$tipoBaseDominioSnebur_AcessoDados_EstruturaConsulta.Propriedades.Add(__$propriedade_Snebur_AcessoDados_EstruturaConsulta_TipoEntidadeAssemblyQualifiedName);
	__$tipoBaseDominioSnebur_AcessoDados_EstruturaConsulta.Propriedades.Add(__$propriedade_Snebur_AcessoDados_EstruturaConsulta_IsMostrarDeletados);
	__$tipoBaseDominioSnebur_AcessoDados_EstruturaConsulta.Propriedades.Add(__$propriedade_Snebur_AcessoDados_EstruturaConsulta_IsMostrarInativos);
	__$tipoBaseDominioSnebur_AcessoDados_EstruturaConsulta.Propriedades.Add(__$propriedade_Snebur_AcessoDados_EstruturaConsulta_IsDesativarOrdenacao);
	__$tipoBaseDominioSnebur_AcessoDados_EstruturaConsulta.Propriedades.Add(__$propriedade_Snebur_AcessoDados_EstruturaConsulta_Take);
	__$tipoBaseDominioSnebur_AcessoDados_EstruturaConsulta.Propriedades.Add(__$propriedade_Snebur_AcessoDados_EstruturaConsulta_Skip);
	__$tipoBaseDominioSnebur_AcessoDados_EstruturaConsulta.Propriedades.Add(__$propriedade_Snebur_AcessoDados_EstruturaConsulta_PaginaAtual);
	__$tipoBaseDominioSnebur_AcessoDados_EstruturaConsulta.Propriedades.Add(__$propriedade_Snebur_AcessoDados_EstruturaConsulta_CaminhoPropriedadeFuncao);
	__$tipoBaseDominioSnebur_AcessoDados_EstruturaConsulta.Propriedades.Add(__$propriedade_Snebur_AcessoDados_EstruturaConsulta_TipoFuncaoEnum);
	__$tipoBaseDominioSnebur_AcessoDados_EstruturaConsulta.Propriedades.Add(__$propriedade_Snebur_AcessoDados_EstruturaConsulta_FiltroGrupoE);
	__$tipoBaseDominioSnebur_AcessoDados_EstruturaConsulta.Propriedades.Add(__$propriedade_Snebur_AcessoDados_EstruturaConsulta_FiltroGrupoOU);
	__$tipoBaseDominioSnebur_AcessoDados_EstruturaConsulta.Propriedades.Add(__$propriedade_Snebur_AcessoDados_EstruturaConsulta_Ordenacoes);
	__$tipoBaseDominioSnebur_AcessoDados_EstruturaConsulta.Propriedades.Add(__$propriedade_Snebur_AcessoDados_EstruturaConsulta_RelacoesAbertaFiltro);
	__$tipoBaseDominioSnebur_AcessoDados_EstruturaConsulta.Propriedades.Add(__$propriedade_Snebur_AcessoDados_EstruturaConsulta_RelacoesAberta);
	__$tipoBaseDominioSnebur_AcessoDados_EstruturaConsulta.Propriedades.Add(__$propriedade_Snebur_AcessoDados_EstruturaConsulta_ColecoesAberta);
	__$tipoBaseDominioSnebur_AcessoDados_EstruturaConsulta.Propriedades.Add(__$propriedade_Snebur_AcessoDados_EstruturaConsulta_PropriedadesAbertas);
	__$tipoBaseDominioSnebur_AcessoDados_EstruturaConsulta.Propriedades.Add(__$propriedade_Snebur_AcessoDados_EstruturaConsulta_ContarRegistros);
	
	export const __$propriedade_Snebur_AcessoDados_ConsultaFiltroColecao_Relacao = new Snebur.Reflexao.Propriedade("Relacao", __$tipoTipoPrimario_String, __$tipoBaseDominioSnebur_AcessoDados_ConsultaFiltroColecao, true);
	
	export const __$propriedade_Snebur_AcessoDados_ConsultaFiltroColecao_EstruturaConsulta = new Snebur.Reflexao.Propriedade("EstruturaConsulta", __$tipoBaseDominioSnebur_AcessoDados_EstruturaConsulta, __$tipoBaseDominioSnebur_AcessoDados_ConsultaFiltroColecao, true);
	
	__$tipoBaseDominioSnebur_AcessoDados_ConsultaFiltroColecao.Propriedades.Add(__$propriedade_Snebur_AcessoDados_ConsultaFiltroColecao_Relacao);
	__$tipoBaseDominioSnebur_AcessoDados_ConsultaFiltroColecao.Propriedades.Add(__$propriedade_Snebur_AcessoDados_ConsultaFiltroColecao_EstruturaConsulta);
	
	export const __$propriedade_Snebur_AcessoDados_Ordenacao_CaminhoPropriedade = new Snebur.Reflexao.Propriedade("CaminhoPropriedade", __$tipoTipoPrimario_String, __$tipoBaseDominioSnebur_AcessoDados_Ordenacao, true);
	
	export const __$propriedade_Snebur_AcessoDados_Ordenacao_SentidoOrdenacaoEnum = new Snebur.Reflexao.Propriedade("SentidoOrdenacaoEnum", __$tipoEnum_Snebur_Dominio_EnumSentidoOrdenacao, __$tipoBaseDominioSnebur_AcessoDados_Ordenacao, false);
	
	__$tipoBaseDominioSnebur_AcessoDados_Ordenacao.Propriedades.Add(__$propriedade_Snebur_AcessoDados_Ordenacao_CaminhoPropriedade);
	__$tipoBaseDominioSnebur_AcessoDados_Ordenacao.Propriedades.Add(__$propriedade_Snebur_AcessoDados_Ordenacao_SentidoOrdenacaoEnum);
	
	export const __$propriedade_Snebur_AcessoDados_Resultado_IsSucesso = new Snebur.Reflexao.Propriedade("IsSucesso", __$tipoTipoPrimario_Boolean, __$tipoBaseDominioSnebur_AcessoDados_Resultado, false);
	
	export const __$propriedade_Snebur_AcessoDados_Resultado_Permissao = new Snebur.Reflexao.Propriedade("Permissao", __$tipoEnum_Snebur_AcessoDados_Seguranca_EnumPermissao, __$tipoBaseDominioSnebur_AcessoDados_Resultado, false);
	
	__$tipoBaseDominioSnebur_AcessoDados_Resultado.Propriedades.Add(__$propriedade_Snebur_AcessoDados_Resultado_IsSucesso);
	__$tipoBaseDominioSnebur_AcessoDados_Resultado.Propriedades.Add(__$propriedade_Snebur_AcessoDados_Resultado_Permissao);
	
	export const __$propriedade_Snebur_AcessoDados_EntidadeSalva_Id = new Snebur.Reflexao.Propriedade("Id", __$tipoTipoPrimario_Long, __$tipoBaseDominioSnebur_AcessoDados_EntidadeSalva, false);
	
	export const __$propriedade_Snebur_AcessoDados_EntidadeSalva_IdentificadorUnicoEntidade = new Snebur.Reflexao.Propriedade("IdentificadorUnicoEntidade", __$tipoTipoPrimario_Guid, __$tipoBaseDominioSnebur_AcessoDados_EntidadeSalva, false);
	
	export const __$propriedade_Snebur_AcessoDados_EntidadeSalva_CaminhoTipoEntidadeSalva = new Snebur.Reflexao.Propriedade("CaminhoTipoEntidadeSalva", __$tipoTipoPrimario_String, __$tipoBaseDominioSnebur_AcessoDados_EntidadeSalva, true);
	
	export const __$propriedade_Snebur_AcessoDados_EntidadeSalva_PropriedadesComputada = new Snebur.Reflexao.Propriedade("PropriedadesComputada", __$tipoListaBaseDominio_Snebur_AcessoDados_PropriedadeComputada, __$tipoBaseDominioSnebur_AcessoDados_EntidadeSalva, true);
	
	__$tipoBaseDominioSnebur_AcessoDados_EntidadeSalva.Propriedades.Add(__$propriedade_Snebur_AcessoDados_EntidadeSalva_Id);
	__$tipoBaseDominioSnebur_AcessoDados_EntidadeSalva.Propriedades.Add(__$propriedade_Snebur_AcessoDados_EntidadeSalva_IdentificadorUnicoEntidade);
	__$tipoBaseDominioSnebur_AcessoDados_EntidadeSalva.Propriedades.Add(__$propriedade_Snebur_AcessoDados_EntidadeSalva_CaminhoTipoEntidadeSalva);
	__$tipoBaseDominioSnebur_AcessoDados_EntidadeSalva.Propriedades.Add(__$propriedade_Snebur_AcessoDados_EntidadeSalva_PropriedadesComputada);
	
	export const __$propriedade_Snebur_AcessoDados_PropriedadeComputada_NomePropriedade = new Snebur.Reflexao.Propriedade("NomePropriedade", __$tipoTipoPrimario_String, __$tipoBaseDominioSnebur_AcessoDados_PropriedadeComputada, true);
	
	export const __$propriedade_Snebur_AcessoDados_PropriedadeComputada_Valor = new Snebur.Reflexao.Propriedade("Valor", __$tipoTipoPrimario_Object, __$tipoBaseDominioSnebur_AcessoDados_PropriedadeComputada, true);
	
	__$tipoBaseDominioSnebur_AcessoDados_PropriedadeComputada.Propriedades.Add(__$propriedade_Snebur_AcessoDados_PropriedadeComputada_NomePropriedade);
	__$tipoBaseDominioSnebur_AcessoDados_PropriedadeComputada.Propriedades.Add(__$propriedade_Snebur_AcessoDados_PropriedadeComputada_Valor);
	
	export const __$propriedade_Snebur_AcessoDados_RelacaoAbertaColecao_EstruturaConsulta = new Snebur.Reflexao.Propriedade("EstruturaConsulta", __$tipoBaseDominioSnebur_AcessoDados_EstruturaConsulta, __$tipoBaseDominioSnebur_AcessoDados_RelacaoAbertaColecao, true);
	
	__$tipoBaseDominioSnebur_AcessoDados_RelacaoAbertaColecao.Propriedades.Add(__$propriedade_Snebur_AcessoDados_RelacaoAbertaColecao_EstruturaConsulta);
	
	export const __$propriedade_Snebur_AcessoDados_FiltroPropriedadeIn_CaminhoPropriedade = new Snebur.Reflexao.Propriedade("CaminhoPropriedade", __$tipoTipoPrimario_String, __$tipoBaseDominioSnebur_AcessoDados_FiltroPropriedadeIn, true);
	
	export const __$propriedade_Snebur_AcessoDados_FiltroPropriedadeIn_Lista = new Snebur.Reflexao.Propriedade("Lista", __$tipoListaTipoPrimario_String, __$tipoBaseDominioSnebur_AcessoDados_FiltroPropriedadeIn, true);
	
	__$tipoBaseDominioSnebur_AcessoDados_FiltroPropriedadeIn.Propriedades.Add(__$propriedade_Snebur_AcessoDados_FiltroPropriedadeIn_CaminhoPropriedade);
	__$tipoBaseDominioSnebur_AcessoDados_FiltroPropriedadeIn.Propriedades.Add(__$propriedade_Snebur_AcessoDados_FiltroPropriedadeIn_Lista);
	
	export const __$propriedade_Snebur_AcessoDados_FiltroIds_Ids = new Snebur.Reflexao.Propriedade("Ids", __$tipoListaTipoPrimario_Long, __$tipoBaseDominioSnebur_AcessoDados_FiltroIds, true);
	
	__$tipoBaseDominioSnebur_AcessoDados_FiltroIds.Propriedades.Add(__$propriedade_Snebur_AcessoDados_FiltroIds_Ids);
	
	export const __$propriedade_Snebur_AcessoDados_BaseFiltroGrupo_Filtros = new Snebur.Reflexao.Propriedade("Filtros", __$tipoListaBaseDominio_Snebur_AcessoDados_BaseFiltro, __$tipoBaseDominioSnebur_AcessoDados_BaseFiltroGrupo, true);
	
	__$tipoBaseDominioSnebur_AcessoDados_BaseFiltroGrupo.Propriedades.Add(__$propriedade_Snebur_AcessoDados_BaseFiltroGrupo_Filtros);
	
	export const __$propriedade_Snebur_AcessoDados_FiltroPropriedade_CaminhoPropriedade = new Snebur.Reflexao.Propriedade("CaminhoPropriedade", __$tipoTipoPrimario_String, __$tipoBaseDominioSnebur_AcessoDados_FiltroPropriedade, true);
	
	export const __$propriedade_Snebur_AcessoDados_FiltroPropriedade_TipoPrimarioEnum = new Snebur.Reflexao.Propriedade("TipoPrimarioEnum", __$tipoEnum_Snebur_Reflexao_EnumTipoPrimario, __$tipoBaseDominioSnebur_AcessoDados_FiltroPropriedade, false);
	
	export const __$propriedade_Snebur_AcessoDados_FiltroPropriedade_Operador = new Snebur.Reflexao.Propriedade("Operador", __$tipoEnum_Snebur_AcessoDados_EnumOperadorFiltro, __$tipoBaseDominioSnebur_AcessoDados_FiltroPropriedade, false);
	
	export const __$propriedade_Snebur_AcessoDados_FiltroPropriedade_Valor = new Snebur.Reflexao.Propriedade("Valor", __$tipoTipoPrimario_Object, __$tipoBaseDominioSnebur_AcessoDados_FiltroPropriedade, true);
	
	__$tipoBaseDominioSnebur_AcessoDados_FiltroPropriedade.Propriedades.Add(__$propriedade_Snebur_AcessoDados_FiltroPropriedade_CaminhoPropriedade);
	__$tipoBaseDominioSnebur_AcessoDados_FiltroPropriedade.Propriedades.Add(__$propriedade_Snebur_AcessoDados_FiltroPropriedade_TipoPrimarioEnum);
	__$tipoBaseDominioSnebur_AcessoDados_FiltroPropriedade.Propriedades.Add(__$propriedade_Snebur_AcessoDados_FiltroPropriedade_Operador);
	__$tipoBaseDominioSnebur_AcessoDados_FiltroPropriedade.Propriedades.Add(__$propriedade_Snebur_AcessoDados_FiltroPropriedade_Valor);
	
	export const __$propriedade_Snebur_AcessoDados_ResultadoConsulta_Entidades = new Snebur.Reflexao.Propriedade("Entidades", __$tipoListaEntidade_Snebur_Dominio_Entidade, __$tipoBaseDominioSnebur_AcessoDados_ResultadoConsulta, true);
	
	export const __$propriedade_Snebur_AcessoDados_ResultadoConsulta_TotalRegistros = new Snebur.Reflexao.Propriedade("TotalRegistros", __$tipoTipoPrimario_Integer, __$tipoBaseDominioSnebur_AcessoDados_ResultadoConsulta, false);
	
	__$tipoBaseDominioSnebur_AcessoDados_ResultadoConsulta.Propriedades.Add(__$propriedade_Snebur_AcessoDados_ResultadoConsulta_Entidades);
	__$tipoBaseDominioSnebur_AcessoDados_ResultadoConsulta.Propriedades.Add(__$propriedade_Snebur_AcessoDados_ResultadoConsulta_TotalRegistros);
	
	export const __$propriedade_Snebur_AcessoDados_ResultadoSalvar_EntidadesSalvas = new Snebur.Reflexao.Propriedade("EntidadesSalvas", __$tipoListaBaseDominio_Snebur_AcessoDados_EntidadeSalva, __$tipoBaseDominioSnebur_AcessoDados_ResultadoSalvar, true);
	
	export const __$propriedade_Snebur_AcessoDados_ResultadoSalvar_ErrosValidacao = new Snebur.Reflexao.Propriedade("ErrosValidacao", __$tipoListaBaseDominio_Snebur_Dominio_ErroValidacao, __$tipoBaseDominioSnebur_AcessoDados_ResultadoSalvar, true);
	
	__$tipoBaseDominioSnebur_AcessoDados_ResultadoSalvar.Propriedades.Add(__$propriedade_Snebur_AcessoDados_ResultadoSalvar_EntidadesSalvas);
	__$tipoBaseDominioSnebur_AcessoDados_ResultadoSalvar.Propriedades.Add(__$propriedade_Snebur_AcessoDados_ResultadoSalvar_ErrosValidacao);
	
}