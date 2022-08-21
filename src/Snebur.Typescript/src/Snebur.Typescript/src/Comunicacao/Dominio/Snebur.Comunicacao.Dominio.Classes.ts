/*eslint-disable*/
//Data : segunda-feira, 15 de agosto de 2022
//Hora : 06:15:23
//@Namespace: Snebur.Comunicacao
//@PrioridadeDominio: 1
//@Globalizar: False
//@Dominios dependentes: []

namespace Snebur.Comunicacao
{
    export class BaseComunicao extends Snebur.Dominio.BaseDominio
    {
        //#region Construtor
    
        public constructor(inicializador?: Partial<BaseComunicao>) 
        {
            super(inicializador);
            this.Inicializar();
        }
        //#endregion
    }
    export class Cabecalho extends Snebur.Comunicacao.BaseComunicao
    {
        //#region Propriedades
    
        private _credencialServico : Snebur.Seguranca.CredencialServico = null;
        private _credencialUsuario : Snebur.Seguranca.CredencialUsuario = null;
        private _credencialAvalista : Snebur.Seguranca.CredencialUsuario = null;
        private _urlOrigem : string = null;
    
        public get CredencialServico(): Snebur.Seguranca.CredencialServico 
        {
            return this._credencialServico;
        }

        public set CredencialServico(value: Snebur.Seguranca.CredencialServico)  
        {
            this.NotificarValorPropriedadeAlterada("CredencialServico", this._credencialServico, this._credencialServico = value);
        }
    
        public get CredencialUsuario(): Snebur.Seguranca.CredencialUsuario 
        {
            return this._credencialUsuario;
        }

        public set CredencialUsuario(value: Snebur.Seguranca.CredencialUsuario)  
        {
            this.NotificarValorPropriedadeAlterada("CredencialUsuario", this._credencialUsuario, this._credencialUsuario = value);
        }
    
        public get CredencialAvalista(): Snebur.Seguranca.CredencialUsuario 
        {
            return this._credencialAvalista;
        }

        public set CredencialAvalista(value: Snebur.Seguranca.CredencialUsuario)  
        {
            this.NotificarValorPropriedadeAlterada("CredencialAvalista", this._credencialAvalista, this._credencialAvalista = value);
        }
    
        public get UrlOrigem(): string 
        {
            return this._urlOrigem;
        }

        public set UrlOrigem(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("UrlOrigem", this._urlOrigem, this._urlOrigem = value);
        }
        //#endregion
    
        //#region Construtor
    
        public constructor() 
        {
            super();
        }
        //#endregion
    }
    export class ContratoChamada extends Snebur.Comunicacao.BaseComunicao
    {
        //#region Propriedades
    
        private _cabecalho : Snebur.Comunicacao.Cabecalho = null;
        private _informacaoSessaoUsuario : Snebur.Dominio.InformacaoSessaoUsuario = null;
        private _operacao : string = null;
        private _dataHora : Date =  new Date();
        private _async : boolean = false;
        private _parametros : Array<Snebur.Comunicacao.ParametroChamada> =  new Array<Snebur.Comunicacao.ParametroChamada>();
    
        public get Cabecalho(): Snebur.Comunicacao.Cabecalho 
        {
            return this._cabecalho;
        }

        public set Cabecalho(value: Snebur.Comunicacao.Cabecalho)  
        {
            this.NotificarValorPropriedadeAlterada("Cabecalho", this._cabecalho, this._cabecalho = value);
        }
    
        public get InformacaoSessaoUsuario(): Snebur.Dominio.InformacaoSessaoUsuario 
        {
            return this._informacaoSessaoUsuario;
        }

        public set InformacaoSessaoUsuario(value: Snebur.Dominio.InformacaoSessaoUsuario)  
        {
            this.NotificarValorPropriedadeAlterada("InformacaoSessaoUsuario", this._informacaoSessaoUsuario, this._informacaoSessaoUsuario = value);
        }
    
        public get Operacao(): string 
        {
            return this._operacao;
        }

        public set Operacao(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("Operacao", this._operacao, this._operacao = value);
        }
    
        public get DataHora(): Date 
        {
            return this._dataHora;
        }

        public set DataHora(value: Date)  
        {
            this.NotificarValorPropriedadeAlterada("DataHora", this._dataHora, this._dataHora = value);
        }
    
        public get Async(): boolean 
        {
            return this._async;
        }

        public set Async(value: boolean)  
        {
            this.NotificarValorPropriedadeAlterada("Async", this._async, this._async = value);
        }
    
        public get Parametros(): Array<Snebur.Comunicacao.ParametroChamada> 
        {
            return this._parametros;
        }

        public set Parametros(value: Array<Snebur.Comunicacao.ParametroChamada>)  
        {
            this.NotificarValorPropriedadeAlterada("Parametros", this._parametros, this._parametros = value);
        }
        //#endregion
    
        //#region Construtor
    
        public constructor() 
        {
            super();
        }
        //#endregion
    }
    export abstract class ParametroChamada extends Snebur.Comunicacao.BaseComunicao
    {
        //#region Propriedades
    
        private _nome : string = null;
        private _nomeTipoParametro : string = null;
        private _assemblyQualifiedName : string = null;
    
        public get Nome(): string 
        {
            return this._nome;
        }

        public set Nome(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("Nome", this._nome, this._nome = value);
        }
    
        public get NomeTipoParametro(): string 
        {
            return this._nomeTipoParametro;
        }

        public set NomeTipoParametro(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("NomeTipoParametro", this._nomeTipoParametro, this._nomeTipoParametro = value);
        }
    
        public get AssemblyQualifiedName(): string 
        {
            return this._assemblyQualifiedName;
        }

        public set AssemblyQualifiedName(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("AssemblyQualifiedName", this._assemblyQualifiedName, this._assemblyQualifiedName = value);
        }
        //#endregion
    
        //#region Construtor
    
        public constructor(inicializador?: Partial<ParametroChamada>) 
        {
            super(inicializador);
        }
        //#endregion
    }
    export class ChamadaRegraNegocio extends Snebur.Comunicacao.BaseComunicao
    {
        //#region Propriedades
    
        private _assemblyQualifiedName : string = null;
        private _nomeMetodo : string = null;
    
        public get AssemblyQualifiedName(): string 
        {
            return this._assemblyQualifiedName;
        }

        public set AssemblyQualifiedName(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("AssemblyQualifiedName", this._assemblyQualifiedName, this._assemblyQualifiedName = value);
        }
    
        public get NomeMetodo(): string 
        {
            return this._nomeMetodo;
        }

        public set NomeMetodo(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("NomeMetodo", this._nomeMetodo, this._nomeMetodo = value);
        }
        //#endregion
    
        //#region Construtor
    
        public constructor() 
        {
            super();
        }
        //#endregion
    }
    export abstract class ResultadoChamada extends Snebur.Comunicacao.BaseComunicao
    {
        //#region Propriedades
    
        private _nomeServico : string = null;
        private _dataHora : Date =  new Date();
        private _operacao : string = null;
        private _tempoOperacao : number = 0;
    
        public get NomeServico(): string 
        {
            return this._nomeServico;
        }

        public set NomeServico(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("NomeServico", this._nomeServico, this._nomeServico = value);
        }
    
        public get DataHora(): Date 
        {
            return this._dataHora;
        }

        public set DataHora(value: Date)  
        {
            this.NotificarValorPropriedadeAlterada("DataHora", this._dataHora, this._dataHora = value);
        }
    
        public get Operacao(): string 
        {
            return this._operacao;
        }

        public set Operacao(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("Operacao", this._operacao, this._operacao = value);
        }
    
        public get TempoOperacao(): number 
        {
            return this._tempoOperacao;
        }

        public set TempoOperacao(value: number)  
        {
            this.NotificarValorPropriedadeAlterada("TempoOperacao", this._tempoOperacao, this._tempoOperacao = value);
        }
        //#endregion
    
        //#region Construtor
    
        public constructor(inicializador?: Partial<ResultadoChamada>) 
        {
            super(inicializador);
        }
        //#endregion
    }
    export class ConfiguracaoServicoImagem extends Snebur.Comunicacao.BaseComunicao
    {
        //#region Propriedades
    
        private _urlVisualizarImagem : string = null;
    
        public get UrlVisualizarImagem(): string 
        {
            return this._urlVisualizarImagem;
        }

        public set UrlVisualizarImagem(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("UrlVisualizarImagem", this._urlVisualizarImagem, this._urlVisualizarImagem = value);
        }
        //#endregion
    
        //#region Construtor
    
        public constructor() 
        {
            super();
        }
        //#endregion
    }
    export class ResultadoExcluir extends Snebur.Comunicacao.BaseComunicao
    {
        //#region Propriedades
    
        private _isSucesso : boolean = false;
        private _mensagemErro : string = null;
    
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
        //#endregion
    
        //#region Construtor
    
        public constructor() 
        {
            super();
        }
        //#endregion
    }
    export class ParametroChamadaBaseDominio extends Snebur.Comunicacao.ParametroChamada
    {
        //#region Propriedades
    
        private _baseDominio : Snebur.Dominio.BaseDominio = null;
    
        public get BaseDominio(): Snebur.Dominio.BaseDominio 
        {
            return this._baseDominio;
        }

        public set BaseDominio(value: Snebur.Dominio.BaseDominio)  
        {
            this.NotificarValorPropriedadeAlterada("BaseDominio", this._baseDominio, this._baseDominio = value);
        }
        //#endregion
    
        //#region Construtor
    
        public constructor(inicializador?: Partial<ParametroChamadaBaseDominio>) 
        {
            super(inicializador);
            this.Inicializar();
        }
        //#endregion
    }
    export class ParametroChamadaNulo extends Snebur.Comunicacao.ParametroChamada
    {
        //#region Construtor
    
        public constructor(inicializador?: Partial<ParametroChamadaNulo>) 
        {
            super(inicializador);
            this.Inicializar();
        }
        //#endregion
    }
    export class ParametroChamadaEnum extends Snebur.Comunicacao.ParametroChamada
    {
        //#region Propriedades
    
        private _nomeTipoEnum : string = null;
        private _namespaceEnum : string = null;
        private _valor : number = 0;
    
        public get NomeTipoEnum(): string 
        {
            return this._nomeTipoEnum;
        }

        public set NomeTipoEnum(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("NomeTipoEnum", this._nomeTipoEnum, this._nomeTipoEnum = value);
        }
    
        public get NamespaceEnum(): string 
        {
            return this._namespaceEnum;
        }

        public set NamespaceEnum(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("NamespaceEnum", this._namespaceEnum, this._namespaceEnum = value);
        }
    
        public get Valor(): number 
        {
            return this._valor;
        }

        public set Valor(value: number)  
        {
            this.NotificarValorPropriedadeAlterada("Valor", this._valor, this._valor = value);
        }
        //#endregion
    
        //#region Construtor
    
        public constructor(inicializador?: Partial<ParametroChamadaEnum>) 
        {
            super(inicializador);
            this.Inicializar();
        }
        //#endregion
    }
    export abstract class ParametroChamadaLista extends Snebur.Comunicacao.ParametroChamada
    {
        //#region Construtor
    
        public constructor(inicializador?: Partial<ParametroChamadaLista>) 
        {
            super(inicializador);
        }
        //#endregion
    }
    export class ParametroChamadaTipoPrimario extends Snebur.Comunicacao.ParametroChamada
    {
        //#region Propriedades
    
        private _valor : any = null;
        private _tipoPrimarioEnum : Snebur.Reflexao.EnumTipoPrimario = 0;
    
        public get Valor(): any 
        {
            return this._valor;
        }

        public set Valor(value: any)  
        {
            this.NotificarValorPropriedadeAlterada("Valor", this._valor, this._valor = value);
        }
    
        public get TipoPrimarioEnum(): Snebur.Reflexao.EnumTipoPrimario 
        {
            return this._tipoPrimarioEnum;
        }

        public set TipoPrimarioEnum(value: Snebur.Reflexao.EnumTipoPrimario)  
        {
            this.NotificarValorPropriedadeAlterada("TipoPrimarioEnum", this._tipoPrimarioEnum, this._tipoPrimarioEnum = value);
        }
        //#endregion
    
        //#region Construtor
    
        public constructor(inicializador?: Partial<ParametroChamadaTipoPrimario>) 
        {
            super(inicializador);
            this.Inicializar();
        }
        //#endregion
    }
    export class ParametroChamadaListaBaseDominio extends Snebur.Comunicacao.ParametroChamadaLista
    {
        //#region Propriedades
    
        private _nomeTipoBaseDominio : string = null;
        private _nomeNamespaceTipoBaseDominio : string = null;
        private _basesDominio : Array<Snebur.Dominio.BaseDominio> =  new Array<Snebur.Dominio.BaseDominio>();
    
        public get NomeTipoBaseDominio(): string 
        {
            return this._nomeTipoBaseDominio;
        }

        public set NomeTipoBaseDominio(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("NomeTipoBaseDominio", this._nomeTipoBaseDominio, this._nomeTipoBaseDominio = value);
        }
    
        public get NomeNamespaceTipoBaseDominio(): string 
        {
            return this._nomeNamespaceTipoBaseDominio;
        }

        public set NomeNamespaceTipoBaseDominio(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("NomeNamespaceTipoBaseDominio", this._nomeNamespaceTipoBaseDominio, this._nomeNamespaceTipoBaseDominio = value);
        }
    
        public get BasesDominio(): Array<Snebur.Dominio.BaseDominio> 
        {
            return this._basesDominio;
        }

        public set BasesDominio(value: Array<Snebur.Dominio.BaseDominio>)  
        {
            this.NotificarValorPropriedadeAlterada("BasesDominio", this._basesDominio, this._basesDominio = value);
        }
        //#endregion
    
        //#region Construtor
    
        public constructor(inicializador?: Partial<ParametroChamadaListaBaseDominio>) 
        {
            super(inicializador);
            this.Inicializar();
        }
        //#endregion
    }
    export class ParametroChamadaListaEnum extends Snebur.Comunicacao.ParametroChamadaLista
    {
        //#region Propriedades
    
        private _nomeTipoEnum : string = null;
        private _namespaceEnum : string = null;
        private _valores : Array<number> =  new Array<number>();
    
        public get NomeTipoEnum(): string 
        {
            return this._nomeTipoEnum;
        }

        public set NomeTipoEnum(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("NomeTipoEnum", this._nomeTipoEnum, this._nomeTipoEnum = value);
        }
    
        public get NamespaceEnum(): string 
        {
            return this._namespaceEnum;
        }

        public set NamespaceEnum(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("NamespaceEnum", this._namespaceEnum, this._namespaceEnum = value);
        }
    
        public get Valores(): Array<number> 
        {
            return this._valores;
        }

        public set Valores(value: Array<number>)  
        {
            this.NotificarValorPropriedadeAlterada("Valores", this._valores, this._valores = value);
        }
        //#endregion
    
        //#region Construtor
    
        public constructor(inicializador?: Partial<ParametroChamadaListaEnum>) 
        {
            super(inicializador);
            this.Inicializar();
        }
        //#endregion
    }
    export class ParametroChamadaListaTipoPrimario extends Snebur.Comunicacao.ParametroChamadaLista
    {
        //#region Propriedades
    
        private _lista : Array<any> =  new Array<any>();
        private _tipoPrimarioEnum : Snebur.Reflexao.EnumTipoPrimario = 0;
    
        public get Lista(): Array<any> 
        {
            return this._lista;
        }

        public set Lista(value: Array<any>)  
        {
            this.NotificarValorPropriedadeAlterada("Lista", this._lista, this._lista = value);
        }
    
        public get TipoPrimarioEnum(): Snebur.Reflexao.EnumTipoPrimario 
        {
            return this._tipoPrimarioEnum;
        }

        public set TipoPrimarioEnum(value: Snebur.Reflexao.EnumTipoPrimario)  
        {
            this.NotificarValorPropriedadeAlterada("TipoPrimarioEnum", this._tipoPrimarioEnum, this._tipoPrimarioEnum = value);
        }
        //#endregion
    
        //#region Construtor
    
        public constructor(inicializador?: Partial<ParametroChamadaListaTipoPrimario>) 
        {
            super(inicializador);
            this.Inicializar();
        }
        //#endregion
    }
    export class ResultadoSessaoUsuarioInvalida extends Snebur.Comunicacao.ResultadoChamada
    {
        //#region Propriedades
    
        private _estadoSessaoUsuario : Snebur.Dominio.EnumEstadoSessaoUsuario = 0;
        private _identificadorSessaoUsuario : string = "00000000-0000-0000-0000-000000000000";
    
        public get EstadoSessaoUsuario(): Snebur.Dominio.EnumEstadoSessaoUsuario 
        {
            return this._estadoSessaoUsuario;
        }

        public set EstadoSessaoUsuario(value: Snebur.Dominio.EnumEstadoSessaoUsuario)  
        {
            this.NotificarValorPropriedadeAlterada("EstadoSessaoUsuario", this._estadoSessaoUsuario, this._estadoSessaoUsuario = value);
        }
    
        public get IdentificadorSessaoUsuario(): string 
        {
            return this._identificadorSessaoUsuario;
        }

        public set IdentificadorSessaoUsuario(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("IdentificadorSessaoUsuario", this._identificadorSessaoUsuario, this._identificadorSessaoUsuario = value);
        }
        //#endregion
    
        //#region Construtor
    
        public constructor( estadoSessaoUsuario : Snebur.Dominio.EnumEstadoSessaoUsuario ,  identificadorSessaoUsuario : string ) 
        {
            super();
            this.Inicializar();
            this._estadoSessaoUsuario = estadoSessaoUsuario;
            this._identificadorSessaoUsuario = identificadorSessaoUsuario;
        }
        //#endregion
    }
    export class ResultadoChamadaBaseDominio extends Snebur.Comunicacao.ResultadoChamada
    {
        //#region Propriedades
    
        private _baseDominio : Snebur.Dominio.BaseDominio = null;
    
        public get BaseDominio(): Snebur.Dominio.BaseDominio 
        {
            return this._baseDominio;
        }

        public set BaseDominio(value: Snebur.Dominio.BaseDominio)  
        {
            this.NotificarValorPropriedadeAlterada("BaseDominio", this._baseDominio, this._baseDominio = value);
        }
        //#endregion
    
        //#region Construtor
    
        public constructor(inicializador?: Partial<ResultadoChamadaBaseDominio>) 
        {
            super(inicializador);
            this.Inicializar();
        }
        //#endregion
    }
    export class ResultadoChamadaEnum extends Snebur.Comunicacao.ResultadoChamada
    {
        //#region Propriedades
    
        private _nomeTipoEnum : string = null;
        private _namespaceEnum : string = null;
        private _valor : number = 0;
    
        public get NomeTipoEnum(): string 
        {
            return this._nomeTipoEnum;
        }

        public set NomeTipoEnum(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("NomeTipoEnum", this._nomeTipoEnum, this._nomeTipoEnum = value);
        }
    
        public get NamespaceEnum(): string 
        {
            return this._namespaceEnum;
        }

        public set NamespaceEnum(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("NamespaceEnum", this._namespaceEnum, this._namespaceEnum = value);
        }
    
        public get Valor(): number 
        {
            return this._valor;
        }

        public set Valor(value: number)  
        {
            this.NotificarValorPropriedadeAlterada("Valor", this._valor, this._valor = value);
        }
        //#endregion
    
        //#region Construtor
    
        public constructor(inicializador?: Partial<ResultadoChamadaEnum>) 
        {
            super(inicializador);
            this.Inicializar();
        }
        //#endregion
    }
    export abstract class ResultadoChamadaErro extends Snebur.Comunicacao.ResultadoChamada
    {
        //#region Propriedades
    
        private _mensagemErro : string = null;
        private _erro : any = null;
    
        public get MensagemErro(): string 
        {
            return this._mensagemErro;
        }

        public set MensagemErro(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("MensagemErro", this._mensagemErro, this._mensagemErro = value);
        }
    
        public get Erro(): any 
        {
            return this._erro;
        }

        public set Erro(value: any)  
        {
            this.NotificarValorPropriedadeAlterada("Erro", this._erro, this._erro = value);
        }
        //#endregion
    
        //#region Construtor
    
        public constructor(inicializador?: Partial<ResultadoChamadaErro>) 
        {
            super(inicializador);
        }
        //#endregion
    }
    export abstract class ResultadoChamadaLista extends Snebur.Comunicacao.ResultadoChamada
    {
        //#region Propriedades
    
        private _assemblyQualifiedName : string = null;
    
        public get AssemblyQualifiedName(): string 
        {
            return this._assemblyQualifiedName;
        }

        public set AssemblyQualifiedName(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("AssemblyQualifiedName", this._assemblyQualifiedName, this._assemblyQualifiedName = value);
        }
        //#endregion
    
        //#region Construtor
    
        public constructor(inicializador?: Partial<ResultadoChamadaLista>) 
        {
            super(inicializador);
        }
        //#endregion
    }
    export class ResultadoChamadaTipoPrimario extends Snebur.Comunicacao.ResultadoChamada
    {
        //#region Propriedades
    
        private _valor : any = null;
        private _tipoPrimarioEnum : Snebur.Reflexao.EnumTipoPrimario = 0;
    
        public get Valor(): any 
        {
            return this._valor;
        }

        public set Valor(value: any)  
        {
            this.NotificarValorPropriedadeAlterada("Valor", this._valor, this._valor = value);
        }
    
        public get TipoPrimarioEnum(): Snebur.Reflexao.EnumTipoPrimario 
        {
            return this._tipoPrimarioEnum;
        }

        public set TipoPrimarioEnum(value: Snebur.Reflexao.EnumTipoPrimario)  
        {
            this.NotificarValorPropriedadeAlterada("TipoPrimarioEnum", this._tipoPrimarioEnum, this._tipoPrimarioEnum = value);
        }
        //#endregion
    
        //#region Construtor
    
        public constructor(inicializador?: Partial<ResultadoChamadaTipoPrimario>) 
        {
            super(inicializador);
            this.Inicializar();
        }
        //#endregion
    }
    export class ResultadoChamadaVazio extends Snebur.Comunicacao.ResultadoChamada
    {
        //#region Construtor
    
        public constructor(inicializador?: Partial<ResultadoChamadaVazio>) 
        {
            super(inicializador);
            this.Inicializar();
        }
        //#endregion
    }
    export class ResultadoChamadaErroInternoServidor extends Snebur.Comunicacao.ResultadoChamadaErro
    {
        //#region Construtor
    
        public constructor(inicializador?: Partial<ResultadoChamadaErroInternoServidor>) 
        {
            super(inicializador);
            this.Inicializar();
        }
        //#endregion
    }
    export class ResultadoChamadaListaBaseDominio extends Snebur.Comunicacao.ResultadoChamadaLista
    {
        //#region Propriedades
    
        private _nomeTipoBaseDominio : string = null;
        private _nomeNamespaceTipoBaseDominio : string = null;
        private _basesDominio : Array<Snebur.Dominio.BaseDominio> =  new Array<Snebur.Dominio.BaseDominio>();
    
        public get NomeTipoBaseDominio(): string 
        {
            return this._nomeTipoBaseDominio;
        }

        public set NomeTipoBaseDominio(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("NomeTipoBaseDominio", this._nomeTipoBaseDominio, this._nomeTipoBaseDominio = value);
        }
    
        public get NomeNamespaceTipoBaseDominio(): string 
        {
            return this._nomeNamespaceTipoBaseDominio;
        }

        public set NomeNamespaceTipoBaseDominio(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("NomeNamespaceTipoBaseDominio", this._nomeNamespaceTipoBaseDominio, this._nomeNamespaceTipoBaseDominio = value);
        }
    
        public get BasesDominio(): Array<Snebur.Dominio.BaseDominio> 
        {
            return this._basesDominio;
        }

        public set BasesDominio(value: Array<Snebur.Dominio.BaseDominio>)  
        {
            this.NotificarValorPropriedadeAlterada("BasesDominio", this._basesDominio, this._basesDominio = value);
        }
        //#endregion
    
        //#region Construtor
    
        public constructor(inicializador?: Partial<ResultadoChamadaListaBaseDominio>) 
        {
            super(inicializador);
            this.Inicializar();
        }
        //#endregion
    }
    export class ResultadoChamadaListaEnum extends Snebur.Comunicacao.ResultadoChamadaLista
    {
        //#region Propriedades
    
        private _nomeTipoEnum : string = null;
        private _namespaceEnum : string = null;
        private _valores : Array<number> =  new Array<number>();
    
        public get NomeTipoEnum(): string 
        {
            return this._nomeTipoEnum;
        }

        public set NomeTipoEnum(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("NomeTipoEnum", this._nomeTipoEnum, this._nomeTipoEnum = value);
        }
    
        public get NamespaceEnum(): string 
        {
            return this._namespaceEnum;
        }

        public set NamespaceEnum(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("NamespaceEnum", this._namespaceEnum, this._namespaceEnum = value);
        }
    
        public get Valores(): Array<number> 
        {
            return this._valores;
        }

        public set Valores(value: Array<number>)  
        {
            this.NotificarValorPropriedadeAlterada("Valores", this._valores, this._valores = value);
        }
        //#endregion
    
        //#region Construtor
    
        public constructor(inicializador?: Partial<ResultadoChamadaListaEnum>) 
        {
            super(inicializador);
            this.Inicializar();
        }
        //#endregion
    }
    export class ResultadoChamadaListaTipoPrimario extends Snebur.Comunicacao.ResultadoChamadaLista
    {
        //#region Propriedades
    
        private _valores : Array<any> =  new Array<any>();
        private _tipoPrimarioEnum : Snebur.Reflexao.EnumTipoPrimario = 0;
    
        public get Valores(): Array<any> 
        {
            return this._valores;
        }

        public set Valores(value: Array<any>)  
        {
            this.NotificarValorPropriedadeAlterada("Valores", this._valores, this._valores = value);
        }
    
        public get TipoPrimarioEnum(): Snebur.Reflexao.EnumTipoPrimario 
        {
            return this._tipoPrimarioEnum;
        }

        public set TipoPrimarioEnum(value: Snebur.Reflexao.EnumTipoPrimario)  
        {
            this.NotificarValorPropriedadeAlterada("TipoPrimarioEnum", this._tipoPrimarioEnum, this._tipoPrimarioEnum = value);
        }
        //#endregion
    
        //#region Construtor
    
        public constructor(inicializador?: Partial<ResultadoChamadaListaTipoPrimario>) 
        {
            super(inicializador);
            this.Inicializar();
        }
        //#endregion
    }
    export class DominioGlobalizacao extends Snebur.Dominio.BaseViewModel
    {
        //#region Propriedades
    
        private _namespaceGlobalizacao : string = null;
        private _jsonBase54 : string = null;
    
        public get NamespaceGlobalizacao(): string 
        {
            return this._namespaceGlobalizacao;
        }

        public set NamespaceGlobalizacao(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("NamespaceGlobalizacao", this._namespaceGlobalizacao, this._namespaceGlobalizacao = value);
        }
    
        public get JsonBase54(): string 
        {
            return this._jsonBase54;
        }

        public set JsonBase54(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("JsonBase54", this._jsonBase54, this._jsonBase54 = value);
        }
        //#endregion
    
        //#region Construtor
    
        public constructor(inicializador?: Partial<DominioGlobalizacao>) 
        {
            super(inicializador);
            this.Inicializar();
        }
        //#endregion
    }
    export class TelaGlobalizacao extends Snebur.Dominio.BaseViewModel
    {
        //#region Propriedades
    
        private _namespaceGlobalizacao : string = null;
        private _jsonBase54 : string = null;
    
        public get NamespaceGlobalizacao(): string 
        {
            return this._namespaceGlobalizacao;
        }

        public set NamespaceGlobalizacao(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("NamespaceGlobalizacao", this._namespaceGlobalizacao, this._namespaceGlobalizacao = value);
        }
    
        public get JsonBase54(): string 
        {
            return this._jsonBase54;
        }

        public set JsonBase54(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("JsonBase54", this._jsonBase54, this._jsonBase54 = value);
        }
        //#endregion
    
        //#region Construtor
    
        public constructor(inicializador?: Partial<TelaGlobalizacao>) 
        {
            super(inicializador);
            this.Inicializar();
        }
        //#endregion
    }
    export class ResultadoGlobalizacao extends Snebur.Dominio.BaseViewModel
    {
        //#region Propriedades
    
        private _jsonIdiomaBase64 : string = null;
        private _jsonCulturaBase64 : string = null;
        private _dominios : Array<Snebur.Comunicacao.DominioGlobalizacao> =  new Array<Snebur.Comunicacao.DominioGlobalizacao>();
        private _telas : Array<Snebur.Comunicacao.TelaGlobalizacao> =  new Array<Snebur.Comunicacao.TelaGlobalizacao>();
    
        public get JsonIdiomaBase64(): string 
        {
            return this._jsonIdiomaBase64;
        }

        public set JsonIdiomaBase64(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("JsonIdiomaBase64", this._jsonIdiomaBase64, this._jsonIdiomaBase64 = value);
        }
    
        public get JsonCulturaBase64(): string 
        {
            return this._jsonCulturaBase64;
        }

        public set JsonCulturaBase64(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("JsonCulturaBase64", this._jsonCulturaBase64, this._jsonCulturaBase64 = value);
        }
    
        public get Dominios(): Array<Snebur.Comunicacao.DominioGlobalizacao> 
        {
            return this._dominios;
        }

        public set Dominios(value: Array<Snebur.Comunicacao.DominioGlobalizacao>)  
        {
            this.NotificarValorPropriedadeAlterada("Dominios", this._dominios, this._dominios = value);
        }
    
        public get Telas(): Array<Snebur.Comunicacao.TelaGlobalizacao> 
        {
            return this._telas;
        }

        public set Telas(value: Array<Snebur.Comunicacao.TelaGlobalizacao>)  
        {
            this.NotificarValorPropriedadeAlterada("Telas", this._telas, this._telas = value);
        }
        //#endregion
    
        //#region Construtor
    
        public constructor(inicializador?: Partial<ResultadoGlobalizacao>) 
        {
            super(inicializador);
            this.Inicializar();
        }
        //#endregion
    }
}
namespace Snebur.Comunicacao.Mensageiro
{
    export class ContratoMensageiro extends Snebur.Dominio.BaseDominio
    {
        //#region Propriedades
    
        private _remetente : Snebur.Dominio.BaseDominio = null;
        private _destinatario : Snebur.Dominio.BaseDominio = null;
        private _nomeRecurso : string = null;
        private _valorParametro : Snebur.Dominio.BaseDominio = null;
    
        public get Remetente(): Snebur.Dominio.BaseDominio 
        {
            return this._remetente;
        }

        public set Remetente(value: Snebur.Dominio.BaseDominio)  
        {
            this.NotificarValorPropriedadeAlterada("Remetente", this._remetente, this._remetente = value);
        }
    
        public get Destinatario(): Snebur.Dominio.BaseDominio 
        {
            return this._destinatario;
        }

        public set Destinatario(value: Snebur.Dominio.BaseDominio)  
        {
            this.NotificarValorPropriedadeAlterada("Destinatario", this._destinatario, this._destinatario = value);
        }
    
        public get NomeRecurso(): string 
        {
            return this._nomeRecurso;
        }

        public set NomeRecurso(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("NomeRecurso", this._nomeRecurso, this._nomeRecurso = value);
        }
    
        public get ValorParametro(): Snebur.Dominio.BaseDominio 
        {
            return this._valorParametro;
        }

        public set ValorParametro(value: Snebur.Dominio.BaseDominio)  
        {
            this.NotificarValorPropriedadeAlterada("ValorParametro", this._valorParametro, this._valorParametro = value);
        }
        //#endregion
    
        //#region Construtor
    
        public constructor(inicializador?: Partial<ContratoMensageiro>) 
        {
            super(inicializador);
            this.Inicializar();
        }
        //#endregion
    }
    export class Remetente extends Snebur.Dominio.BaseDominio
    {
        //#region Construtor
    
        public constructor(inicializador?: Partial<Remetente>) 
        {
            super(inicializador);
            this.Inicializar();
        }
        //#endregion
    }
}