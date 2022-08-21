/*eslint-disable*/
//Data : segunda-feira, 15 de agosto de 2022
//Hora : 06:15:27
//@Namespace: Snebur.AcessoDados
//@PrioridadeDominio: 2
//@Globalizar: False
//@Dominios dependentes: []

namespace Snebur.AcessoDados
{
    export abstract class BaseAcessoDados extends Snebur.Dominio.BaseDominio
    {
        //#region Propriedades
    
        private _mensagemErro : string = null;
        private _falhaAutenticacao : boolean = false;
        private _comandos : Array<string> =  new Array<string>();
    
        public get MensagemErro(): string 
        {
            return this._mensagemErro;
        }

        public set MensagemErro(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("MensagemErro", this._mensagemErro, this._mensagemErro = value);
        }
    
        public get FalhaAutenticacao(): boolean 
        {
            return this._falhaAutenticacao;
        }

        public set FalhaAutenticacao(value: boolean)  
        {
            this.NotificarValorPropriedadeAlterada("FalhaAutenticacao", this._falhaAutenticacao, this._falhaAutenticacao = value);
        }
    
        public get Comandos(): Array<string> 
        {
            return this._comandos;
        }

        public set Comandos(value: Array<string>)  
        {
            this.NotificarValorPropriedadeAlterada("Comandos", this._comandos, this._comandos = value);
        }
        //#endregion
    
        //#region Construtor
    
        public constructor(inicializador?: Partial<BaseAcessoDados>) 
        {
            super(inicializador);
        }
        //#endregion
    }
    export abstract class BaseRelacaoAberta extends Snebur.AcessoDados.BaseAcessoDados
    {
        //#region Propriedades
    
        private _caminhoPropriedade : string = null;
        private _nomeTipoEntidade : string = null;
        private _tipoEntidadeAssemblyQualifiedName : string = null;
        private _nomeTipoDeclarado : string = null;
        private _tipoDeclaradoAssemblyQualifiedName : string = null;
        private _propriedadesAbertas : HashSet<string> =  new HashSet<string>();
    
        public get CaminhoPropriedade(): string 
        {
            return this._caminhoPropriedade;
        }

        public set CaminhoPropriedade(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("CaminhoPropriedade", this._caminhoPropriedade, this._caminhoPropriedade = value);
        }
    
        public get NomeTipoEntidade(): string 
        {
            return this._nomeTipoEntidade;
        }

        public set NomeTipoEntidade(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("NomeTipoEntidade", this._nomeTipoEntidade, this._nomeTipoEntidade = value);
        }
    
        public get TipoEntidadeAssemblyQualifiedName(): string 
        {
            return this._tipoEntidadeAssemblyQualifiedName;
        }

        public set TipoEntidadeAssemblyQualifiedName(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("TipoEntidadeAssemblyQualifiedName", this._tipoEntidadeAssemblyQualifiedName, this._tipoEntidadeAssemblyQualifiedName = value);
        }
    
        public get NomeTipoDeclarado(): string 
        {
            return this._nomeTipoDeclarado;
        }

        public set NomeTipoDeclarado(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("NomeTipoDeclarado", this._nomeTipoDeclarado, this._nomeTipoDeclarado = value);
        }
    
        public get TipoDeclaradoAssemblyQualifiedName(): string 
        {
            return this._tipoDeclaradoAssemblyQualifiedName;
        }

        public set TipoDeclaradoAssemblyQualifiedName(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("TipoDeclaradoAssemblyQualifiedName", this._tipoDeclaradoAssemblyQualifiedName, this._tipoDeclaradoAssemblyQualifiedName = value);
        }
    
        public get PropriedadesAbertas(): HashSet<string> 
        {
            return this._propriedadesAbertas;
        }

        public set PropriedadesAbertas(value: HashSet<string>)  
        {
            this.NotificarValorPropriedadeAlterada("PropriedadesAbertas", this._propriedadesAbertas, this._propriedadesAbertas = value);
        }
        //#endregion
    
        //#region Construtor
    
        public constructor(inicializador?: Partial<BaseRelacaoAberta>) 
        {
            super(inicializador);
        }
        //#endregion
    }
    export class EstruturaConsulta extends Snebur.AcessoDados.BaseAcessoDados implements Snebur.AcessoDados.Seguranca.IEstruturaConsultaSeguranca 
    {
        //#region Propriedades
    
        private _nomeTipoEntidade : string = null;
        private _tipoEntidadeAssemblyQualifiedName : string = null;
        private _isMostrarDeletados : boolean = false;
        private _isMostrarInativos : boolean = false;
        private _isDesativarOrdenacao : boolean = false;
        private _take : number = 0;
        private _skip : number = 0;
        private _paginaAtual : number = 0;
        private _caminhoPropriedadeFuncao : string = null;
        private _tipoFuncaoEnum : Snebur.AcessoDados.EnumTipoFuncao = 0;
        private _filtroGrupoE : Snebur.AcessoDados.FiltroGrupoE =  new Snebur.AcessoDados.FiltroGrupoE();
        private _filtroGrupoOU : Snebur.AcessoDados.FiltroGrupoOU =  new Snebur.AcessoDados.FiltroGrupoOU();
        private _ordenacoes : DicionarioSimples<Snebur.AcessoDados.Ordenacao> =  new DicionarioSimples<Snebur.AcessoDados.Ordenacao>();
        private _relacoesAbertaFiltro : DicionarioSimples<Snebur.AcessoDados.RelacaoAbertaEntidade> =  new DicionarioSimples<Snebur.AcessoDados.RelacaoAbertaEntidade>();
        private _relacoesAberta : DicionarioSimples<Snebur.AcessoDados.RelacaoAbertaEntidade> =  new DicionarioSimples<Snebur.AcessoDados.RelacaoAbertaEntidade>();
        private _colecoesAberta : DicionarioSimples<Snebur.AcessoDados.RelacaoAbertaColecao> =  new DicionarioSimples<Snebur.AcessoDados.RelacaoAbertaColecao>();
        private _propriedadesAbertas : Array<string> =  new Array<string>();
        private _contarRegistros : boolean = false;
    
        public get NomeTipoEntidade(): string 
        {
            return this._nomeTipoEntidade;
        }

        public set NomeTipoEntidade(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("NomeTipoEntidade", this._nomeTipoEntidade, this._nomeTipoEntidade = value);
        }
    
        public get TipoEntidadeAssemblyQualifiedName(): string 
        {
            return this._tipoEntidadeAssemblyQualifiedName;
        }

        public set TipoEntidadeAssemblyQualifiedName(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("TipoEntidadeAssemblyQualifiedName", this._tipoEntidadeAssemblyQualifiedName, this._tipoEntidadeAssemblyQualifiedName = value);
        }
    
        public get IsMostrarDeletados(): boolean 
        {
            return this._isMostrarDeletados;
        }

        public set IsMostrarDeletados(value: boolean)  
        {
            this.NotificarValorPropriedadeAlterada("IsMostrarDeletados", this._isMostrarDeletados, this._isMostrarDeletados = value);
        }
    
        public get IsMostrarInativos(): boolean 
        {
            return this._isMostrarInativos;
        }

        public set IsMostrarInativos(value: boolean)  
        {
            this.NotificarValorPropriedadeAlterada("IsMostrarInativos", this._isMostrarInativos, this._isMostrarInativos = value);
        }
    
        public get IsDesativarOrdenacao(): boolean 
        {
            return this._isDesativarOrdenacao;
        }

        public set IsDesativarOrdenacao(value: boolean)  
        {
            this.NotificarValorPropriedadeAlterada("IsDesativarOrdenacao", this._isDesativarOrdenacao, this._isDesativarOrdenacao = value);
        }
    
        public get Take(): number 
        {
            return this._take;
        }

        public set Take(value: number)  
        {
            this.NotificarValorPropriedadeAlterada("Take", this._take, this._take = value);
        }
    
        public get Skip(): number 
        {
            return this._skip;
        }

        public set Skip(value: number)  
        {
            this.NotificarValorPropriedadeAlterada("Skip", this._skip, this._skip = value);
        }
    
        public get PaginaAtual(): number 
        {
            return this._paginaAtual;
        }

        public set PaginaAtual(value: number)  
        {
            this.NotificarValorPropriedadeAlterada("PaginaAtual", this._paginaAtual, this._paginaAtual = value);
        }
    
        public get CaminhoPropriedadeFuncao(): string 
        {
            return this._caminhoPropriedadeFuncao;
        }

        public set CaminhoPropriedadeFuncao(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("CaminhoPropriedadeFuncao", this._caminhoPropriedadeFuncao, this._caminhoPropriedadeFuncao = value);
        }
    
        public get TipoFuncaoEnum(): Snebur.AcessoDados.EnumTipoFuncao 
        {
            return this._tipoFuncaoEnum;
        }

        public set TipoFuncaoEnum(value: Snebur.AcessoDados.EnumTipoFuncao)  
        {
            this.NotificarValorPropriedadeAlterada("TipoFuncaoEnum", this._tipoFuncaoEnum, this._tipoFuncaoEnum = value);
        }
    
        public get FiltroGrupoE(): Snebur.AcessoDados.FiltroGrupoE 
        {
            return this._filtroGrupoE;
        }

        public set FiltroGrupoE(value: Snebur.AcessoDados.FiltroGrupoE)  
        {
            this.NotificarValorPropriedadeAlterada("FiltroGrupoE", this._filtroGrupoE, this._filtroGrupoE = value);
        }
    
        public get FiltroGrupoOU(): Snebur.AcessoDados.FiltroGrupoOU 
        {
            return this._filtroGrupoOU;
        }

        public set FiltroGrupoOU(value: Snebur.AcessoDados.FiltroGrupoOU)  
        {
            this.NotificarValorPropriedadeAlterada("FiltroGrupoOU", this._filtroGrupoOU, this._filtroGrupoOU = value);
        }
    
        public get Ordenacoes(): DicionarioSimples<Snebur.AcessoDados.Ordenacao> 
        {
            return this._ordenacoes;
        }

        public set Ordenacoes(value: DicionarioSimples<Snebur.AcessoDados.Ordenacao>)  
        {
            this.NotificarValorPropriedadeAlterada("Ordenacoes", this._ordenacoes, this._ordenacoes = value);
        }
    
        public get RelacoesAbertaFiltro(): DicionarioSimples<Snebur.AcessoDados.RelacaoAbertaEntidade> 
        {
            return this._relacoesAbertaFiltro;
        }

        public set RelacoesAbertaFiltro(value: DicionarioSimples<Snebur.AcessoDados.RelacaoAbertaEntidade>)  
        {
            this.NotificarValorPropriedadeAlterada("RelacoesAbertaFiltro", this._relacoesAbertaFiltro, this._relacoesAbertaFiltro = value);
        }
    
        public get RelacoesAberta(): DicionarioSimples<Snebur.AcessoDados.RelacaoAbertaEntidade> 
        {
            return this._relacoesAberta;
        }

        public set RelacoesAberta(value: DicionarioSimples<Snebur.AcessoDados.RelacaoAbertaEntidade>)  
        {
            this.NotificarValorPropriedadeAlterada("RelacoesAberta", this._relacoesAberta, this._relacoesAberta = value);
        }
    
        public get ColecoesAberta(): DicionarioSimples<Snebur.AcessoDados.RelacaoAbertaColecao> 
        {
            return this._colecoesAberta;
        }

        public set ColecoesAberta(value: DicionarioSimples<Snebur.AcessoDados.RelacaoAbertaColecao>)  
        {
            this.NotificarValorPropriedadeAlterada("ColecoesAberta", this._colecoesAberta, this._colecoesAberta = value);
        }
    
        public get PropriedadesAbertas(): Array<string> 
        {
            return this._propriedadesAbertas;
        }

        public set PropriedadesAbertas(value: Array<string>)  
        {
            this.NotificarValorPropriedadeAlterada("PropriedadesAbertas", this._propriedadesAbertas, this._propriedadesAbertas = value);
        }
    
        public get ContarRegistros(): boolean 
        {
            return this._contarRegistros;
        }

        public set ContarRegistros(value: boolean)  
        {
            this.NotificarValorPropriedadeAlterada("ContarRegistros", this._contarRegistros, this._contarRegistros = value);
        }
        //#endregion
    
        //#region Construtor
    
        public constructor(inicializador?: Partial<EstruturaConsulta>) 
        {
            super(inicializador);
            this.Inicializar();
        }
        //#endregion
    }
    export abstract class BaseFiltro extends Snebur.AcessoDados.BaseAcessoDados
    {
        //#region Construtor
    
        public constructor(inicializador?: Partial<BaseFiltro>) 
        {
            super(inicializador);
        }
        //#endregion
    }
    export class ConsultaFiltroColecao extends Snebur.AcessoDados.BaseAcessoDados
    {
        //#region Propriedades
    
        private _relacao : string = null;
        private _estruturaConsulta : Snebur.AcessoDados.EstruturaConsulta = null;
    
        public get Relacao(): string 
        {
            return this._relacao;
        }

        public set Relacao(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("Relacao", this._relacao, this._relacao = value);
        }
    
        public get EstruturaConsulta(): Snebur.AcessoDados.EstruturaConsulta 
        {
            return this._estruturaConsulta;
        }

        public set EstruturaConsulta(value: Snebur.AcessoDados.EstruturaConsulta)  
        {
            this.NotificarValorPropriedadeAlterada("EstruturaConsulta", this._estruturaConsulta, this._estruturaConsulta = value);
        }
        //#endregion
    
        //#region Construtor
    
        public constructor(inicializador?: Partial<ConsultaFiltroColecao>) 
        {
            super(inicializador);
            this.Inicializar();
        }
        //#endregion
    }
    export class Ordenacao extends Snebur.AcessoDados.BaseAcessoDados
    {
        //#region Propriedades
    
        private _caminhoPropriedade : string = null;
        private _sentidoOrdenacaoEnum : Snebur.Dominio.EnumSentidoOrdenacao = 0;
    
        public get CaminhoPropriedade(): string 
        {
            return this._caminhoPropriedade;
        }

        public set CaminhoPropriedade(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("CaminhoPropriedade", this._caminhoPropriedade, this._caminhoPropriedade = value);
        }
    
        public get SentidoOrdenacaoEnum(): Snebur.Dominio.EnumSentidoOrdenacao 
        {
            return this._sentidoOrdenacaoEnum;
        }

        public set SentidoOrdenacaoEnum(value: Snebur.Dominio.EnumSentidoOrdenacao)  
        {
            this.NotificarValorPropriedadeAlterada("SentidoOrdenacaoEnum", this._sentidoOrdenacaoEnum, this._sentidoOrdenacaoEnum = value);
        }
        //#endregion
    
        //#region Construtor
    
        public constructor(inicializador?: Partial<Ordenacao>) 
        {
            super(inicializador);
            this.Inicializar();
        }
        //#endregion
    }
    export abstract class Resultado extends Snebur.AcessoDados.BaseAcessoDados
    {
        //#region Propriedades
    
        private _isSucesso : boolean = false;
        private _permissao : Snebur.AcessoDados.Seguranca.EnumPermissao = 0;
    
        public get IsSucesso(): boolean 
        {
            return this._isSucesso;
        }

        public set IsSucesso(value: boolean)  
        {
            this.NotificarValorPropriedadeAlterada("IsSucesso", this._isSucesso, this._isSucesso = value);
        }
    
        public get Permissao(): Snebur.AcessoDados.Seguranca.EnumPermissao 
        {
            return this._permissao;
        }

        public set Permissao(value: Snebur.AcessoDados.Seguranca.EnumPermissao)  
        {
            this.NotificarValorPropriedadeAlterada("Permissao", this._permissao, this._permissao = value);
        }
        //#endregion
    
        //#region Construtor
    
        public constructor(inicializador?: Partial<Resultado>) 
        {
            super(inicializador);
        }
        //#endregion
    }
    export class EntidadeSalva extends Snebur.AcessoDados.BaseAcessoDados
    {
        //#region Propriedades
    
        private _id : number = 0;
        private _identificadorUnicoEntidade : string = "00000000-0000-0000-0000-000000000000";
        private _caminhoTipoEntidadeSalva : string = null;
        private _propriedadesComputada : Array<Snebur.AcessoDados.PropriedadeComputada> =  new Array<Snebur.AcessoDados.PropriedadeComputada>();
    
        public get Id(): number 
        {
            return this._id;
        }

        public set Id(value: number)  
        {
            this.NotificarValorPropriedadeAlterada("Id", this._id, this._id = value);
        }
    
        public get IdentificadorUnicoEntidade(): string 
        {
            return this._identificadorUnicoEntidade;
        }

        public set IdentificadorUnicoEntidade(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("IdentificadorUnicoEntidade", this._identificadorUnicoEntidade, this._identificadorUnicoEntidade = value);
        }
    
        public get CaminhoTipoEntidadeSalva(): string 
        {
            return this._caminhoTipoEntidadeSalva;
        }

        public set CaminhoTipoEntidadeSalva(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("CaminhoTipoEntidadeSalva", this._caminhoTipoEntidadeSalva, this._caminhoTipoEntidadeSalva = value);
        }
    
        public get PropriedadesComputada(): Array<Snebur.AcessoDados.PropriedadeComputada> 
        {
            return this._propriedadesComputada;
        }

        public set PropriedadesComputada(value: Array<Snebur.AcessoDados.PropriedadeComputada>)  
        {
            this.NotificarValorPropriedadeAlterada("PropriedadesComputada", this._propriedadesComputada, this._propriedadesComputada = value);
        }
        //#endregion
    
        //#region Construtor
    
        public constructor(inicializador?: Partial<EntidadeSalva>) 
        {
            super(inicializador);
            this.Inicializar();
        }
        //#endregion
    }
    export class PropriedadeComputada extends Snebur.AcessoDados.BaseAcessoDados
    {
        //#region Propriedades
    
        private _nomePropriedade : string = null;
        private _valor : any = null;
    
        public get NomePropriedade(): string 
        {
            return this._nomePropriedade;
        }

        public set NomePropriedade(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("NomePropriedade", this._nomePropriedade, this._nomePropriedade = value);
        }
    
        public get Valor(): any 
        {
            return this._valor;
        }

        public set Valor(value: any)  
        {
            this.NotificarValorPropriedadeAlterada("Valor", this._valor, this._valor = value);
        }
        //#endregion
    
        //#region Construtor
    
        public constructor(inicializador?: Partial<PropriedadeComputada>) 
        {
            super(inicializador);
            this.Inicializar();
        }
        //#endregion
    }
    export class RelacaoAbertaColecao extends Snebur.AcessoDados.BaseRelacaoAberta
    {
        //#region Propriedades
    
        private _estruturaConsulta : Snebur.AcessoDados.EstruturaConsulta = null;
    
        public get EstruturaConsulta(): Snebur.AcessoDados.EstruturaConsulta 
        {
            return this._estruturaConsulta;
        }

        public set EstruturaConsulta(value: Snebur.AcessoDados.EstruturaConsulta)  
        {
            this.NotificarValorPropriedadeAlterada("EstruturaConsulta", this._estruturaConsulta, this._estruturaConsulta = value);
        }
        //#endregion
    
        //#region Construtor
    
        public constructor(inicializador?: Partial<RelacaoAbertaColecao>) 
        {
            super(inicializador);
            this.Inicializar();
        }
        //#endregion
    }
    export class RelacaoAbertaEntidade extends Snebur.AcessoDados.BaseRelacaoAberta
    {
        //#region Construtor
    
        public constructor(inicializador?: Partial<RelacaoAbertaEntidade>) 
        {
            super(inicializador);
            this.Inicializar();
        }
        //#endregion
    }
    export class FiltroPropriedadeIn extends Snebur.AcessoDados.BaseFiltro
    {
        //#region Propriedades
    
        private _caminhoPropriedade : string = null;
        private _lista : Array<string> =  new Array<string>();
    
        public get CaminhoPropriedade(): string 
        {
            return this._caminhoPropriedade;
        }

        public set CaminhoPropriedade(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("CaminhoPropriedade", this._caminhoPropriedade, this._caminhoPropriedade = value);
        }
    
        public get Lista(): Array<string> 
        {
            return this._lista;
        }

        public set Lista(value: Array<string>)  
        {
            this.NotificarValorPropriedadeAlterada("Lista", this._lista, this._lista = value);
        }
        //#endregion
    
        //#region Construtor
    
        public constructor( caminhoPropriedade : string ,  lista : Array<string> ) 
        {
            super();
            this.Inicializar();
            this._caminhoPropriedade = caminhoPropriedade;
            this._lista = lista;
        }
        //#endregion
    }
    export class FiltroIds extends Snebur.AcessoDados.BaseFiltro
    {
        //#region Propriedades
    
        private _ids : Array<number> =  new Array<number>();
    
        public get Ids(): Array<number> 
        {
            return this._ids;
        }

        public set Ids(value: Array<number>)  
        {
            this.NotificarValorPropriedadeAlterada("Ids", this._ids, this._ids = value);
        }
        //#endregion
    
        //#region Construtor
    
        public constructor( ids : Array<number> ) 
        {
            super();
            this.Inicializar();
            this._ids = ids;
        }
        //#endregion
    }
    export abstract class BaseFiltroGrupo extends Snebur.AcessoDados.BaseFiltro
    {
        //#region Propriedades
    
        private _filtros : Array<Snebur.AcessoDados.BaseFiltro> =  new Array<Snebur.AcessoDados.BaseFiltro>();
    
        public get Filtros(): Array<Snebur.AcessoDados.BaseFiltro> 
        {
            return this._filtros;
        }

        public set Filtros(value: Array<Snebur.AcessoDados.BaseFiltro>)  
        {
            this.NotificarValorPropriedadeAlterada("Filtros", this._filtros, this._filtros = value);
        }
        //#endregion
    
        //#region Construtor
    
        public constructor(inicializador?: Partial<BaseFiltroGrupo>) 
        {
            super(inicializador);
        }
        //#endregion
    }
    export class FiltroPropriedade extends Snebur.AcessoDados.BaseFiltro
    {
        //#region Propriedades
    
        private _caminhoPropriedade : string = null;
        private _tipoPrimarioEnum : Snebur.Reflexao.EnumTipoPrimario = 0;
        private _operador : Snebur.AcessoDados.EnumOperadorFiltro = 0;
        private _valor : any = null;
    
        public get CaminhoPropriedade(): string 
        {
            return this._caminhoPropriedade;
        }

        public set CaminhoPropriedade(value: string)  
        {
            this.NotificarValorPropriedadeAlterada("CaminhoPropriedade", this._caminhoPropriedade, this._caminhoPropriedade = value);
        }
    
        public get TipoPrimarioEnum(): Snebur.Reflexao.EnumTipoPrimario 
        {
            return this._tipoPrimarioEnum;
        }

        public set TipoPrimarioEnum(value: Snebur.Reflexao.EnumTipoPrimario)  
        {
            this.NotificarValorPropriedadeAlterada("TipoPrimarioEnum", this._tipoPrimarioEnum, this._tipoPrimarioEnum = value);
        }
    
        public get Operador(): Snebur.AcessoDados.EnumOperadorFiltro 
        {
            return this._operador;
        }

        public set Operador(value: Snebur.AcessoDados.EnumOperadorFiltro)  
        {
            this.NotificarValorPropriedadeAlterada("Operador", this._operador, this._operador = value);
        }
    
        public get Valor(): any 
        {
            return this._valor;
        }

        public set Valor(value: any)  
        {
            this.NotificarValorPropriedadeAlterada("Valor", this._valor, this._valor = value);
        }
        //#endregion
    
        //#region Construtor
    
        public constructor(inicializador?: Partial<FiltroPropriedade>) 
        {
            super(inicializador);
            this.Inicializar();
        }
        //#endregion
    }
    export class FiltroGrupoE extends Snebur.AcessoDados.BaseFiltroGrupo
    {
        //#region Construtor
    
        public constructor(inicializador?: Partial<FiltroGrupoE>) 
        {
            super(inicializador);
            this.Inicializar();
        }
        //#endregion
    }
    export class FiltroGrupoNAO extends Snebur.AcessoDados.BaseFiltroGrupo
    {
        //#region Construtor
    
        public constructor(inicializador?: Partial<FiltroGrupoNAO>) 
        {
            super(inicializador);
            this.Inicializar();
        }
        //#endregion
    }
    export class FiltroGrupoOU extends Snebur.AcessoDados.BaseFiltroGrupo
    {
        //#region Construtor
    
        public constructor(inicializador?: Partial<FiltroGrupoOU>) 
        {
            super(inicializador);
            this.Inicializar();
        }
        //#endregion
    }
    export class ResultadoExcluir extends Snebur.AcessoDados.Resultado
    {
        //#region Construtor
    
        public constructor(inicializador?: Partial<ResultadoExcluir>) 
        {
            super(inicializador);
            this.Inicializar();
        }
        //#endregion
    }
    export class ResultadoConsulta extends Snebur.AcessoDados.Resultado
    {
        //#region Propriedades
    
        private _entidades : ListaEntidades<Snebur.Dominio.IEntidade> =  new ListaEntidades<Snebur.Dominio.IEntidade>();
        private _totalRegistros : number = 0;
    
        public get Entidades(): ListaEntidades<Snebur.Dominio.IEntidade> 
        {
            return this._entidades;
        }

        public set Entidades(value: ListaEntidades<Snebur.Dominio.IEntidade>)  
        {
            this.NotificarValorPropriedadeAlterada("Entidades", this._entidades, this._entidades = value);
        }
    
        public get TotalRegistros(): number 
        {
            return this._totalRegistros;
        }

        public set TotalRegistros(value: number)  
        {
            this.NotificarValorPropriedadeAlterada("TotalRegistros", this._totalRegistros, this._totalRegistros = value);
        }
        //#endregion
    
        //#region Construtor
    
        public constructor(inicializador?: Partial<ResultadoConsulta>) 
        {
            super(inicializador);
            this.Inicializar();
        }
        //#endregion
    }
    export class ResultadoSalvar extends Snebur.AcessoDados.Resultado
    {
        //#region Propriedades
    
        private _entidadesSalvas : Array<Snebur.AcessoDados.EntidadeSalva> =  new Array<Snebur.AcessoDados.EntidadeSalva>();
        private _errosValidacao : Array<Snebur.Dominio.ErroValidacao> =  new Array<Snebur.Dominio.ErroValidacao>();
    
        public get EntidadesSalvas(): Array<Snebur.AcessoDados.EntidadeSalva> 
        {
            return this._entidadesSalvas;
        }

        public set EntidadesSalvas(value: Array<Snebur.AcessoDados.EntidadeSalva>)  
        {
            this.NotificarValorPropriedadeAlterada("EntidadesSalvas", this._entidadesSalvas, this._entidadesSalvas = value);
        }
    
        public get ErrosValidacao(): Array<Snebur.Dominio.ErroValidacao> 
        {
            return this._errosValidacao;
        }

        public set ErrosValidacao(value: Array<Snebur.Dominio.ErroValidacao>)  
        {
            this.NotificarValorPropriedadeAlterada("ErrosValidacao", this._errosValidacao, this._errosValidacao = value);
        }
        //#endregion
    
        //#region Construtor
    
        public constructor(inicializador?: Partial<ResultadoSalvar>) 
        {
            super(inicializador);
            this.Inicializar();
        }
        //#endregion
    }
}