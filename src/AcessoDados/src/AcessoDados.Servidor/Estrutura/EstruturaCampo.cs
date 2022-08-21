using System;
using System.Data;
using System.Reflection;
using Snebur.Dominio.Atributos;
using Snebur.Reflexao;
using Snebur.Utilidade;

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

        internal object ValorPadrao { get; set; }

        internal IValorPadrao AtributoValorPadrao { get; }

        internal EnumTipoPrimario TipoPrimarioEnum { get; }

        internal SqlDbType TipoSql { get; }

        internal EstruturaRelacaoChaveEstrangeira EstruturaRelacaoChaveEstrangeira { get; set; } = null;

        internal PropertyInfo PropriedadeTipoComplexo { get; } = null;

        internal bool IsTipoComplexo { get; } = false;

        internal bool IsSomenteLeitura { get;  }

        internal bool IsValorFuncaoServidor { get; private set; }

        internal bool IsFormatarSomenteNumero { get; private set; }

        internal string ValorFuncaoServidor { get; }

        internal bool IsRelacaoChaveEstrangeira
        {
            get
            {
                return this.EstruturaRelacaoChaveEstrangeira != null;
            }
        }

        internal bool IsPossuiIndiceTextoCompleto { get; private set; }

        internal bool IsNotificarAlteracaoPropriedade { get; private set; }

        public bool IsAutorizarAlteracaoPropriedade { get; private set; }

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
            this.ValorPadrao = ReflexaoUtil.RetornarValorPadraoPropriedade(this.Propriedade);

            this.TipoPrimarioEnum = ReflexaoUtil.RetornarTipoPrimarioEnum(this.Propriedade.PropertyType);
            this.TipoSql = this.RetornarTipoBanco();
            this.IsSomenteLeitura = this.RetornarIsSomenteLeitura();



            this.NomeParametro = String.Format("@{0}", this.NomeCampo);



            this.NomeCampoSensivel = this.RetornarNomeCampoSensivel();

            this.AtributoValorPadrao = this.RetornarAtributoValorPadrao();

            if (this.AtributoValorPadrao?.TipoNullableRequerido ?? false)
            {
                if (this.Tipo.IsValueType && !this.IsTipoNullable)
                {
                    this.Alertas.Add($"O propriedade {this.Propriedade.Name} da entidade {this.Propriedade.DeclaringType.Name} por valor padrao, altere o tipo da propriedade para Nullble<{this.Tipo.Name}> para o valor padrao seja inserido quando valor nullo");
                }
            }
            this.IsPossuiIndiceTextoCompleto = this.RetornarIsPossuiIndiceTextoCompleto();
            this.IsNotificarAlteracaoPropriedade = this.RetornarIsNotificarAlteracaoPropriedade();
            this.IsAutorizarAlteracaoPropriedade = this.RetornarIsAutorizarAlteracaoPropriedade();
            this.IsFormatarSomenteNumero = this.RetornarIsFormatarSomenteNumero();
            this.ValorFuncaoServidor = this.RetornarValorParametroServidor();

            if(this.IsFormatarSomenteNumero && this.Propriedade.PropertyType != typeof(string))
            {
                throw new Erro($"O atributo {nameof(FormatarSomenteNumerosAttribute)} é suportado apenas para propriedade do tipo {nameof(String)}");
            }
        }

        private bool RetornarIsSomenteLeitura()
        {
            var atributo = this.Propriedade.GetCustomAttribute<SomenteLeituraAttribute>(true);
            if (atributo != null)
            {
                if (atributo is ValorPadraoDataHoraServidorAttribute atributoDataHoraServidor)
                {
                    return !atributoDataHoraServidor.PermitirAtualizacao;
                }
                return true;
            }
            return false;
        }

        private string RetornarValorParametroServidor()
        {
            if (this.TipoSql == SqlDbType.DateTime)
            {

                var atributoDataHoraServidor = this.Propriedade.GetCustomAttribute<ValorPadraoDataHoraServidorAttribute>();
                if (atributoDataHoraServidor != null)
                {
                    var isDataHoraUTC = atributoDataHoraServidor.DataHoraUTC;
                    this.IsValorFuncaoServidor = true;
                    return isDataHoraUTC ? " GETUTCDATE() " : " GETUTCDATE() ";
                }
            }
            return null;
        }

        internal IValorPadrao RetornarAtributoValorPadrao()
        {
            return EntidadeUtil.RetornarAtributoImplementaIValorPradao(this.Propriedade);
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

                    return SqlDbType.Bit;


                default:

                    throw new Erro("O tipo primario não é suportado");
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
        #endregion
    }
}