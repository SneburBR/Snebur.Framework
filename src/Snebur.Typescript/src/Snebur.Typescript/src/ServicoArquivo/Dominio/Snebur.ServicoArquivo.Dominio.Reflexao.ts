/*eslint-disable*/
//Data : segunda-feira, 15 de agosto de 2022
//Hora : 06:15:27
//@Namespace: Snebur.ServicoArquivo
//@PrioridadeDominio: 2
//@Globalizar: False
//@Dominios dependentes: []
namespace Snebur.Reflexao
{
	//Enum
	export const __$tipoEnum_Snebur_ServicoArquivo_EnumTipoErroServicoArquivo = new Snebur.Reflexao.TipoEnum("EnumTipoErroServicoArquivo", "Snebur.ServicoArquivo", "Snebur.ServicoArquivo.EnumTipoErroServicoArquivo, Snebur.ServicoArquivo");
	
	//Lista Enum
	export const __$tipoListaEnum_Snebur_ServicoArquivo_EnumTipoErroServicoArquivo = new Snebur.Reflexao.TipoListaEnum("ListaTipoEnum_Snebur.ServicoArquivo.EnumTipoErroServicoArquivo", "Snebur.ServicoArquivo", "Snebur.ServicoArquivo.EnumTipoErroServicoArquivo, Snebur.ServicoArquivo", __$tipoEnum_Snebur_ServicoArquivo_EnumTipoErroServicoArquivo);
	
	//Adicionar Enum
	$Reflexao.Tipos.Adicionar("Snebur.ServicoArquivo.EnumTipoErroServicoArquivo",__$tipoEnum_Snebur_ServicoArquivo_EnumTipoErroServicoArquivo);
	
	$Reflexao.Tipos.Adicionar("ListaTipoEnum_Snebur_ServicoArquivo_EnumTipoErroServicoArquivo",__$tipoListaEnum_Snebur_ServicoArquivo_EnumTipoErroServicoArquivo);
	
	//Associar caminhos Emil
	Snebur.ServicoArquivo.EnumTipoErroServicoArquivo.__CaminhoTipo = "Snebur.ServicoArquivo.EnumTipoErroServicoArquivo";
	
	//BaseDominio
	export const __$tipoBaseDominioSnebur_ServicoArquivo_ResultadoServicoArquivo = new Snebur.Reflexao.TipoBaseDominio(Snebur.ServicoArquivo.ResultadoServicoArquivo, "ResultadoServicoArquivo", "Snebur.ServicoArquivo","Snebur.ServicoArquivo.ResultadoServicoArquivo, Snebur.ServicoArquivo",__$tipoBaseDominioSnebur_Dominio_BaseDominio, false);
	
	//ListaBaseDominio
	export const __$tipoListaBaseDominio_Snebur_ServicoArquivo_ResultadoServicoArquivo = new Snebur.Reflexao.TipoListaBaseDominio("ListaTipoBaseDominio_ResultadoServicoArquivo", "Snebur.ServicoArquivo", "System.Collections.Generic.List`1[[Snebur.ServicoArquivo.ResultadoServicoArquivo, Snebur.ServicoArquivo]], mscorlib",__$tipoBaseDominioSnebur_ServicoArquivo_ResultadoServicoArquivo);
	
	//Adicionar BasesDominio
	$Reflexao.Tipos.Adicionar("Snebur.ServicoArquivo.ResultadoServicoArquivo",__$tipoBaseDominioSnebur_ServicoArquivo_ResultadoServicoArquivo);
	
	$Reflexao.Tipos.Adicionar("ListaTipoBaseDominio_Snebur.ServicoArquivo.ResultadoServicoArquivo",__$tipoListaBaseDominio_Snebur_ServicoArquivo_ResultadoServicoArquivo);
	
	//Associar caminhos BaseDominio
	
	Snebur.ServicoArquivo.ResultadoServicoArquivo.__CaminhoTipo = "Snebur.ServicoArquivo.ResultadoServicoArquivo";
	
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
	
	export const __$propriedade_Snebur_ServicoArquivo_ResultadoServicoArquivo_Id = new Snebur.Reflexao.Propriedade("Id", __$tipoTipoPrimario_Long, __$tipoBaseDominioSnebur_ServicoArquivo_ResultadoServicoArquivo, false);
	
	export const __$propriedade_Snebur_ServicoArquivo_ResultadoServicoArquivo_IsSucesso = new Snebur.Reflexao.Propriedade("IsSucesso", __$tipoTipoPrimario_Boolean, __$tipoBaseDominioSnebur_ServicoArquivo_ResultadoServicoArquivo, false);
	
	export const __$propriedade_Snebur_ServicoArquivo_ResultadoServicoArquivo_MensagemErro = new Snebur.Reflexao.Propriedade("MensagemErro", __$tipoTipoPrimario_String, __$tipoBaseDominioSnebur_ServicoArquivo_ResultadoServicoArquivo, true);
	
	export const __$propriedade_Snebur_ServicoArquivo_ResultadoServicoArquivo_TipoErroServicoArquivo = new Snebur.Reflexao.Propriedade("TipoErroServicoArquivo", __$tipoEnum_Snebur_ServicoArquivo_EnumTipoErroServicoArquivo, __$tipoBaseDominioSnebur_ServicoArquivo_ResultadoServicoArquivo, false);
	
	__$tipoBaseDominioSnebur_ServicoArquivo_ResultadoServicoArquivo.Propriedades.Add(__$propriedade_Snebur_ServicoArquivo_ResultadoServicoArquivo_Id);
	__$tipoBaseDominioSnebur_ServicoArquivo_ResultadoServicoArquivo.Propriedades.Add(__$propriedade_Snebur_ServicoArquivo_ResultadoServicoArquivo_IsSucesso);
	__$tipoBaseDominioSnebur_ServicoArquivo_ResultadoServicoArquivo.Propriedades.Add(__$propriedade_Snebur_ServicoArquivo_ResultadoServicoArquivo_MensagemErro);
	__$tipoBaseDominioSnebur_ServicoArquivo_ResultadoServicoArquivo.Propriedades.Add(__$propriedade_Snebur_ServicoArquivo_ResultadoServicoArquivo_TipoErroServicoArquivo);
	
}