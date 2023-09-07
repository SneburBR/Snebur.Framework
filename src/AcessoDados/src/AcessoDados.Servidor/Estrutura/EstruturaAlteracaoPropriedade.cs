using Snebur.Dominio;
using Snebur.Dominio.Atributos;
using System;
using System.Reflection;

namespace Snebur.AcessoDados.Estrutura
{

    internal class BaseEstruturaAlteracaoPropriedade<TAtributo> : EstruturaPropriedade, IEstruturaAlteracaoPropriedade where TAtributo : INotificarAlteracaoPropriedade
    {
        public EstruturaCampo EstruturaCampo { get; }
        public EstruturaTipoComplexo EstruturaTipoComplexo { get; }
        public TAtributo Atributo { get; }
        public bool IsTipoComplexo { get; }
        public bool IsNotificarNovoCadastro => (this.Atributo.Opcoes & EnumOpcoesAlterarPropriedade.NotificarNovoCadastro) == EnumOpcoesAlterarPropriedade.NotificarNovoCadastro;
        public bool IsVerificarAlteracaoBanco => (this.Atributo.Opcoes & EnumOpcoesAlterarPropriedade.VerificarAlteracaoNoBanco) == EnumOpcoesAlterarPropriedade.VerificarAlteracaoNoBanco;
        public bool IsSalvarDataHoraFimAlteracao => (this.Atributo.Opcoes & EnumOpcoesAlterarPropriedade.AtualizarDataHoraFimAlteracao) == EnumOpcoesAlterarPropriedade.AtualizarDataHoraFimAlteracao;
        public bool IsIgnorarZeroIgualNull => (this.Atributo.Opcoes & EnumOpcoesAlterarPropriedade.IgnorarZeroIgualNull) == EnumOpcoesAlterarPropriedade.IgnorarZeroIgualNull;
        public bool IsIgnorarValorAntigoNull => (this.Atributo.Opcoes & EnumOpcoesAlterarPropriedade.IgnorarValorAntigoNull) == EnumOpcoesAlterarPropriedade.IgnorarValorAntigoNull;
        public bool IsProteger => (this.Atributo.Opcoes & EnumOpcoesAlterarPropriedade.Proteger) == EnumOpcoesAlterarPropriedade.Proteger;

        internal BaseEstruturaAlteracaoPropriedade(PropertyInfo propriedade,
                                                   EstruturaEntidade estruturaEntidade,
                                                   EstruturaCampo estruturaCampo,
                                                   TAtributo atributo) :
                                                   base(propriedade, estruturaEntidade)
        {
            this.EstruturaCampo = estruturaCampo;
            this.Atributo = atributo;

        }

        internal BaseEstruturaAlteracaoPropriedade(PropertyInfo propriedade,
                                                   EstruturaEntidade estruturaEntidade,
                                                   EstruturaTipoComplexo estrturaTipoComplexo,
                                                   TAtributo atributo) :
                                                   base(propriedade, estruturaEntidade)
        {
            this.Atributo = atributo;
            this.EstruturaTipoComplexo = estrturaTipoComplexo;
            this.IsTipoComplexo = true;
        }

        #region INotificarAlteracaoPropriedade

        INotificarAlteracaoPropriedade IEstruturaAlteracaoPropriedade.Atributo => this.Atributo;
        PropertyInfo IEstruturaAlteracaoPropriedade.Propriedade => this.Propriedade;
        EstruturaEntidade IEstruturaAlteracaoPropriedade.EstruturaEntidade => this.EstruturaEntidade;

        #endregion

    }
    internal class EstruturaAlteracaoPropriedade : BaseEstruturaAlteracaoPropriedade<NotificarAlteracaoPropriedadeAttribute>
    {
        internal EstruturaAlteracaoPropriedade(PropertyInfo propriedade,
                                               EstruturaEntidade estruturaEntidade,
                                               EstruturaCampo estruturaCampo,
                                               NotificarAlteracaoPropriedadeAttribute atributo) :
                                               base(propriedade,
                                                   estruturaEntidade,
                                                   estruturaCampo,
                                                   atributo)
        {


        }

        internal EstruturaAlteracaoPropriedade(PropertyInfo propriedade,
                                               EstruturaEntidade estruturaEntidade,
                                               EstruturaTipoComplexo estrturaTipoComplexo,
                                               NotificarAlteracaoPropriedadeAttribute atributo) :
                                               base(propriedade,
                                                    estruturaEntidade,
                                                    estrturaTipoComplexo,
                                                    atributo)
        {

        }
    }
    internal class EstruturaAlteracaoPropriedadeGenerica : BaseEstruturaAlteracaoPropriedade<INotificarAlteracaoPropriedade>
    {
        internal EstruturaAlteracaoPropriedadeGenerica(PropertyInfo propriedade,
                                                      EstruturaEntidade estruturaEntidade,
                                                      EstruturaCampo estruturaCampo,
                                                      INotificarAlteracaoPropriedade atributo) :
                                                      base(propriedade,
                                                          estruturaEntidade,
                                                          estruturaCampo,
                                                          atributo)
        {


        }

        internal EstruturaAlteracaoPropriedadeGenerica(PropertyInfo propriedade,
                                                       EstruturaEntidade estruturaEntidade,
                                                       EstruturaTipoComplexo estrturaTipoComplexo,
                                                       INotificarAlteracaoPropriedade atributo) :
                                                       base(propriedade,
                                                            estruturaEntidade,
                                                            estrturaTipoComplexo,
                                                            atributo)
        {

        }
    }

    internal interface IEstruturaAlteracaoPropriedade
    {
        EstruturaEntidade EstruturaEntidade { get; }
        PropertyInfo Propriedade { get; }
        EstruturaCampo EstruturaCampo { get; }
        EstruturaTipoComplexo EstruturaTipoComplexo { get; }
        INotificarAlteracaoPropriedade Atributo { get; }
        bool IsTipoComplexo { get; }
        bool IsNotificarNovoCadastro { get; }
        bool IsVerificarAlteracaoBanco { get; }
        bool IsSalvarDataHoraFimAlteracao { get; }
    }
}