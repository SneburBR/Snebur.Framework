using Snebur.Dominio;
using Snebur.Dominio.Atributos;
using Snebur.Reflexao;
using Snebur.Utilidade;
using System;
using System.Data;
using System.Reflection;

namespace Snebur.AcessoDados.Estrutura
{
    internal class EstruturaCampo : EstruturaPropriedade
    {
        internal string NomeCampo { get; }
        internal string NomeCampoSensivel { get; }
        internal string NomeParametro { get; }
        internal string NomeParametroOuValorFuncaoServidor
        {
            get
            {
                if (this.IsValorFuncaoServidor)
                {
                    return this.ValorFuncaoServidor;
                }
                return this.NomeParametro;
            }
        }


        internal EnumTipoPrimario TipoPrimarioEnum { get; }
        internal DateTimeKind? DateTimeKind { get; set; }
        internal SqlDbType TipoSql { get; }
        internal EstruturaRelacaoChaveEstrangeira EstruturaRelacaoChaveEstrangeira { get; set; } = null;
        internal PropertyInfo PropriedadeTipoComplexo { get; } = null;
        internal bool IsTipoComplexo { get; } = false;
        internal OpcoesSomenteLeitura OpcoesSomenteLeitura { get; }
        internal bool IsValorFuncaoServidor { get; private set; }
        internal bool IsFormatarSomenteNumero { get; }
        internal string ValorFuncaoServidor { get; }
        internal bool IsRelacaoChaveEstrangeira => this.EstruturaRelacaoChaveEstrangeira != null;
        internal bool IsPossuiIndiceTextoCompleto { get; }
        internal bool IsNotificarAlteracaoPropriedade { get; }
        public bool IsAutorizarAlteracaoPropriedade { get; }
        public bool IsValorPadrao { get; }

        internal IBaseValorPadrao AtributoValorPadrao { get; }

        public EnumTipoValorPadrao TipoValorPadrao { get; }

        /// <summary>
        /// Construtor campo do TipoComplexo
        /// </summary>
        /// <param name="propriedadeTipoComplexo"></param>
        /// <param name="propriedadeCampo"></param>
        /// <param name="estruturaEntidade"></param>
        internal EstruturaCampo(PropertyInfo propriedadeTipoComplexo, PropertyInfo propriedadeCampo,
                                EstruturaEntidade estruturaEntidade) : this(propriedadeCampo, estruturaEntidade)
        {
            this.IsTipoComplexo = true;
            this.PropriedadeTipoComplexo = propriedadeTipoComplexo;
            this.NomeCampo = this.RetornarCaminhoPropriedade();

            this.NomeParametro = String.Format("@{0}", this.NomeCampo);
            //this.NomeParametroValor = String.Format("@{0}", this.NomeCampo);


            this.NomeCampoSensivel = this.RetornarNomeCampoSensivel();
        }

        internal EstruturaCampo(PropertyInfo propriedade,
                                EstruturaEntidade estruturaEntidade) : base(propriedade, estruturaEntidade)
        {
            this.Propriedade = propriedade;
            this.NomeCampo = this.RetornarNomeCampo();
            //this.ValorPadrao = ReflexaoUtil.RetornarValorPadraoPropriedade(this.Propriedade);

            this.TipoPrimarioEnum = ReflexaoUtil.RetornarTipoPrimarioEnum(this.Propriedade.PropertyType);
            this.TipoSql = this.RetornarTipoBanco();
            this.OpcoesSomenteLeitura = this.RetornarOpcoesSomenteLeitura();

            this.NomeParametro = String.Format("@{0}", this.NomeCampo);

            this.NomeCampoSensivel = this.RetornarNomeCampoSensivel();

            this.AtributoValorPadrao = this.RetornarAtributoValorPadrao();
            this.TipoValorPadrao = this.RetornarTipoValorPadrao();

            this.IsPossuiIndiceTextoCompleto = this.RetornarIsPossuiIndiceTextoCompleto();
            this.IsNotificarAlteracaoPropriedade = this.RetornarIsNotificarAlteracaoPropriedade();
            this.IsAutorizarAlteracaoPropriedade = this.RetornarIsAutorizarAlteracaoPropriedade();
            this.IsFormatarSomenteNumero = this.RetornarIsFormatarSomenteNumero();
            this.ValorFuncaoServidor = this.RetornarValorParametroServidor();

            if (this.IsFormatarSomenteNumero && this.Propriedade.PropertyType != typeof(string))
            {
                throw new Erro($"O atributo {nameof(FormatarSomenteNumerosAttribute)} é suportado apenas para propriedade do tipo {nameof(String)}");
            }

            if (this.AtributoValorPadrao?.IsTipoNullableRequerido == true)
            {
                if (this.Tipo.IsValueType && !this.IsTipoNullable)
                {
                    this.Alertas.Add($"O propriedade {this.Propriedade.Name} da entidade {this.Propriedade.DeclaringType.Name} por valor padrão, altere o tipo da propriedade para Nullble<{this.Tipo.Name}> para o valor padrao seja inserido quando valor nullo");
                }
            }
            if (this.IsPossuiIndiceTextoCompleto)
            {
                this.EstruturaEntidade.EstruturasCamposIndiceTextoCompleto.Add(this);
            }
        }

        private OpcoesSomenteLeitura RetornarOpcoesSomenteLeitura()
        {
            var atributo = this.Propriedade.GetCustomAttribute<SomenteLeituraAttribute>(true);
            if (atributo != null)
            {
                return atributo.OpcoesSomenteLeitura;
            }
            return new OpcoesSomenteLeitura(false);
        }

        private string RetornarValorParametroServidor()
        {
            if (this.TipoSql == SqlDbType.DateTime)
            {

                var atributoDataHoraServidor = this.Propriedade.GetCustomAttribute<ValorPadraoDataHoraServidorAttribute>();
                if (atributoDataHoraServidor != null)
                {
                    var isDataHoraUTC = atributoDataHoraServidor.IsDataHoraUTC;
                    this.IsValorFuncaoServidor = true;
                    return isDataHoraUTC ? " GETUTCDATE() " : " GETUTCDATE() ";
                }
            }
            return null;
        }

        private IBaseValorPadrao RetornarAtributoValorPadrao()
        {
            return EntidadeUtil.RetornarAtributoImplementaInterface<IBaseValorPadrao>(this.Propriedade);
        }

        private EnumTipoValorPadrao RetornarTipoValorPadrao()
        {
            var atributo = this.AtributoValorPadrao;
            if (atributo != null)
            {

                if (atributo is ValorPadraoIDSessaoUsuarioAttribute)
                {
                    return EnumTipoValorPadrao.SessaoUsuario_Id;
                }
                if (atributo is ValorPadraoIDUsuarioLogadoAttribute)
                {
                    return EnumTipoValorPadrao.UsuarioLogado_Id;
                }
                if (atributo is PropriedadeIdentificadorProprietarioAttribute)
                {
                    return EnumTipoValorPadrao.IndentificadorProprietario;
                }
                if(atributo is ValorPadraoAttribute valorPadrao)
                {
                    return valorPadrao.TipoValorPadrao;
                }
                return EnumTipoValorPadrao.Comum;

            }
            return EnumTipoValorPadrao.Nenhum;
        }

        #region Métodos internos

        internal string RetornarCaminhoPropriedade()
        {
            if (this.IsTipoComplexo)
            {
                return String.Format("{0}_{1}", this.PropriedadeTipoComplexo.Name, this.Propriedade.Name);
            }
            else
            {
                return this.Propriedade.Name;
            }
        }
        #endregion

        #region Métodos privados

        private string RetornarNomeCampo()
        {
            //var atributoCampo = (CampoAttribute)this.Propriedade.GetCustomAttribute<CampoAttribute>();
            //if (atributoCampo != null && !String.IsNullOrEmpty(atributoCampo.NomeCampo))
            //{
            //    return atributoCampo.NomeCampo;
            //}
            //return this.Propriedade.Name;
            return EntidadeUtil.RetornarNomeCampo(this.Propriedade);
        }

        private SqlDbType RetornarTipoBanco()
        {
            switch (this.TipoPrimarioEnum)
            {
                case (EnumTipoPrimario.String):

                    return SqlDbType.NVarChar;

                case (EnumTipoPrimario.Boolean):

                    return SqlDbType.Bit;

                case (EnumTipoPrimario.EnumValor):
                case (EnumTipoPrimario.Integer):

                    return SqlDbType.Int;

                case (EnumTipoPrimario.Long):

                    return SqlDbType.BigInt;

                case (EnumTipoPrimario.Decimal):

                    return SqlDbType.Decimal;

                case (EnumTipoPrimario.Double):

                    return SqlDbType.Float;

                case (EnumTipoPrimario.DateTime):

                    return SqlDbType.DateTime;

                case (EnumTipoPrimario.TimeSpan):

                    return SqlDbType.Time;

                case (EnumTipoPrimario.Guid):

                    return SqlDbType.UniqueIdentifier;

                case (EnumTipoPrimario.Byte):

                    return SqlDbType.TinyInt;

                case (EnumTipoPrimario.Char):

                    return SqlDbType.Char;

                default:

                    throw new Erro("O tipo primário não é suportado");
            }
        }

        private string RetornarNomeCampoSensivel()
        {
            var nomeCampo = (this.IsTipoComplexo ? this.RetornarCaminhoPropriedade() : this.NomeCampo);
            switch (ConfiguracaoAcessoDados.TipoBancoDadosEnum)
            {
                case (EnumTipoBancoDados.PostgreSQL):
                case (EnumTipoBancoDados.PostgreSQLImob):

                    return String.Format("\"{0}\"", nomeCampo);

                case (EnumTipoBancoDados.SQL_SERVER):

                    return String.Format("[{0}]", nomeCampo);

                default:

                    throw new ErroNaoSuportado("Tipo do banco de dados não é suportado");
            }
        }

        private bool RetornarIsPossuiIndiceTextoCompleto()
        {
            return this.Propriedade.GetCustomAttribute<IndexarTextoCompletoAttribute>() != null;
        }

        private bool RetornarIsFormatarSomenteNumero()
        {
            return this.Propriedade.GetCustomAttribute<FormatarSomenteNumerosAttribute>() != null;
        }

        private bool RetornarIsNotificarAlteracaoPropriedade()
        {
            return this.Propriedade.GetCustomAttribute<NotificarAlteracaoPropriedadeAttribute>() != null;
        }

        private bool RetornarIsAutorizarAlteracaoPropriedade()
        {
            return this.Propriedade.GetCustomAttribute<AutorizarAlteracaoPropriedadeAttribute>() != null;
        }

        internal T RetornarAtributoValorPadrao<T>()
        {
            if (this.AtributoValorPadrao is T atributo)
            {
                return atributo;
            }
            throw new Exception($"Não foi possível converter o atributo de valor padrão para {typeof(T).Name} ");
        }

        #endregion
    }

   
}