/*eslint-disable*/
//Data : segunda-feira, 15 de agosto de 2022
//Hora : 06:15:27
//@Namespace: Snebur.ServicoArquivo
//@PrioridadeDominio: 2
//@Globalizar: False
//@Dominios dependentes: []

namespace Snebur.ServicoArquivo
{
    export class ResultadoServicoArquivo extends Snebur.Dominio.BaseDominio
    {
        //#region Propriedades
    
        private _id : number = 0;
        private _isSucesso : boolean = false;
        private _mensagemErro : string = null;
        private _tipoErroServicoArquivo : Snebur.ServicoArquivo.EnumTipoErroServicoArquivo = 0;
    
        public get Id(): number 
        {
            return this._id;
        }

        public set Id(value: number)  
        {
            this.NotificarValorPropriedadeAlterada("Id", this._id, this._id = value);
        }
    
        public get IsSucesso(): boolean 
        {
            return this._isSucesso;
        }

        public set IsSucesso(value: boolean)  
        {
            this.NotificarValorPropriedadeAlterada("IsSucesso", this._isSucesso, this._isSucesso = value);
        }
    
        public get MensagemErro(): string 
        {
            return this._mensagemErro;
        }

        public set MensagemErro(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("MensagemErro", this._mensagemErro, this._mensagemErro = value);
        }
    
        public get TipoErroServicoArquivo(): Snebur.ServicoArquivo.EnumTipoErroServicoArquivo 
        {
            return this._tipoErroServicoArquivo;
        }

        public set TipoErroServicoArquivo(value: Snebur.ServicoArquivo.EnumTipoErroServicoArquivo)  
        {
            this.NotificarValorPropriedadeAlterada("TipoErroServicoArquivo", this._tipoErroServicoArquivo, this._tipoErroServicoArquivo = value);
        }
        //#endregion
    
        //#region Construtor
    
        public constructor(inicializador?: Partial<ResultadoServicoArquivo>) 
        {
            super(inicializador);
            this.Inicializar();
        }
        //#endregion
    }
}