/*eslint-disable*/
//Data : segunda-feira, 15 de agosto de 2022
//Hora : 06:15:23
//@Namespace: Snebur.Comunicacao
//@PrioridadeDominio: 1
//@Globalizar: False
//@Dominios dependentes: []

namespace Snebur.AcessoDados
{
    export interface IServicoRegrasNegocio extends Snebur.Comunicacao.IBaseServico 
    {
    }
}
namespace Snebur.Comunicacao
{
    export interface IServicoLogServicoArquivo extends Snebur.Comunicacao.IBaseServico 
    {
        NotificarInicioEnvioAsync(totalArquivos : number,totalBytes : number) : Promise<string>;
        NotificarProgressoEnvioArquivoAsync(identificadorLog : string,progresso : number,bytesEnvidos : number) : Promise<boolean>;
        NotificarFimEnvioAsync(identificadorLog : string,totalBytesEnviado : number) : Promise<boolean>;
    }
}