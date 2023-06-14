using Snebur.Linq;
using Snebur.Utilidade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Snebur.Dominio.Atributos
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class ValidacaoUnicoCompostaAttribute : BaseAtributoValidacaoAsync
    {
        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        public List<PropriedadeIndexar> Propriedades { get; } = new List<PropriedadeIndexar>();

        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        public List<FiltroPropriedadeIndexar> Filtros { get; } = new List<FiltroPropriedadeIndexar>();

        //[IgnorarPropriedadeTS]
        //[IgnorarPropriedadeTSReflexao]
        public Type TipoEntidade { get; }

        [MensagemValidacao]
        public static string MensagemValidacao { get; set; } = "O {0} '{1}' já existe.";

        [IgnorarPropriedadeTS]
        [IgnorarPropriedadeTSReflexao]
        public List<string> NomesPropriedade { get; } = new List<string>();
        public string[] NomesPropriedadeOuFiltro { get; }

        public bool IsCriarIndicesNomeBanco { get; set; } = true;

        public ValidacaoUnicoCompostaAttribute(Type tipoEntidade,
                                               params string[] nomesPropriedadeOuFiltro)
        {
            this.TipoEntidade = tipoEntidade;
            this.NomesPropriedadeOuFiltro = nomesPropriedadeOuFiltro;
            //this.NomesPropriedade = nomesPropriedadeOuFiltro;

#if EXTENSAO_VISUALSTUDIO
            return;
#endif

            foreach (var nomePropriedadeOuFiltro in nomesPropriedadeOuFiltro)
            {
                var reg = new Regex(@"\s+");
                var partes = reg.Split(nomePropriedadeOuFiltro).Where(x => !String.IsNullOrWhiteSpace(x)).ToArray();
                if (partes.Count() > 1)
                {
                    var propriedadeFiltro = this.RetornarPropriedadeFiltro(partes);
                    if (propriedadeFiltro == null)
                    {
                        throw this.RetornarException(nomePropriedadeOuFiltro);
                    }
                    this.Filtros.Add(propriedadeFiltro);
                }
                else
                {
                    var nomePropriedade = partes.Single().Trim();
                    var propriedadeIndexar = this.RetornarPropriedadeIndexar(nomePropriedade);
                    if (propriedadeIndexar == null)
                    {
                        throw this.RetornarException(nomePropriedadeOuFiltro);
                    }
                    this.Propriedades.Add(propriedadeIndexar);
                    this.NomesPropriedade.Add(nomePropriedade);
                }
            }

            if (ReflexaoUtil.TipoImplementaInterface(tipoEntidade, typeof(IDeletado)))
            {
                var propriedadeIsDeletado = ReflexaoUtil.RetornarPropriedade(tipoEntidade, nameof(IDeletado.IsDeletado), true);
                var propriedadeDataHoraDeletado = ReflexaoUtil.RetornarPropriedade(tipoEntidade, nameof(IDeletado.DataHoraDeletado), true);
                if (propriedadeIsDeletado != null)
                {
                    this.Filtros.Add(new FiltroPropriedadeIndexar(propriedadeIsDeletado,
                                                                  EnumOperadorComparacao.Igual,
                                                                  "0"));
                }

                if (propriedadeDataHoraDeletado != null)
                {
                    this.Filtros.Add(new FiltroPropriedadeIndexar(propriedadeDataHoraDeletado,
                                     EnumOperadorComparacao.Igual,
                                     "null"));
                }
            }

            if (this.Filtros.Count > 0)
            {
                var propriedadesFiltro = this.Filtros.Select(x => x.Propriedade).ToList();
                var propriedadesEmConclito = this.Propriedades.Where(x => propriedadesFiltro.Contains(x.Propriedade)).
                                                                                             Select(x => x.Propriedade);
                if (propriedadesEmConclito.Count() > 0)
                {
                    throw new Erro($"Remover as propriedades  em conflitos {String.Join(",", propriedadesEmConclito.Select(x=> x.Name))}" +
                                   $" no  índice {nameof(ValidacaoUnicoCompostaAttribute)} em {tipoEntidade.Name} ");
                }

                var duplicados = propriedadesFiltro.Duplicados();
                if (duplicados.Count() > 0)
                {
                    throw new Erro($"Remover as propriedades  em conflitos {String.Join(",", duplicados.Select(x => x.Name))}" +
                                   $" no  índice {nameof(ValidacaoUnicoCompostaAttribute)} em {tipoEntidade.Name} ");
                }
            }

        }
        private PropriedadeIndexar RetornarPropriedadeIndexar(string nomePropriedade)
        {
            var tipoEntidade = this.TipoEntidade;
            var isAceitaNulo = nomePropriedade.EndsWith("?");
            if (isAceitaNulo)
            {
                nomePropriedade = nomePropriedade.Substring(0, nomePropriedade.Length - 1);
            }
            var propriedade = ReflexaoUtil.RetornarPropriedade(tipoEntidade, nomePropriedade, true);
            if (propriedade != null)
            {
                return new PropriedadeIndexar(propriedade, isAceitaNulo);
            }
            return null;
        }

        private FiltroPropriedadeIndexar RetornarPropriedadeFiltro(string[] partes)
        {
            if (partes.Count() != 3)
            {
                return null;
            }
            var nomePropriedade = partes[0];
            var operador = partes[1];
            var valor = partes[2];
            var propriedade = ReflexaoUtil.RetornarPropriedade(this.TipoEntidade, nomePropriedade, true);
            if (propriedade == null)
            {
                return null;
            }
            var operadorEnum = this.RetornarOperador(operador, nomePropriedade);
            return new FiltroPropriedadeIndexar(propriedade, operadorEnum, valor);
        }

        private EnumOperadorComparacao RetornarOperador(string operador, string nomePropriedade)
        {
            switch (operador.Trim())
            {
                case "=":
                    return EnumOperadorComparacao.Igual;
                case "<>":
                    return EnumOperadorComparacao.Diferente;
                case ">":
                    return EnumOperadorComparacao.MaiorQue;
                case ">=":
                    return EnumOperadorComparacao.MaiorIgualA;
                case "<":
                    return EnumOperadorComparacao.MenorQue;
                case "<=":
                    return EnumOperadorComparacao.MenorIgualA;
                default:
                    throw this.RetornarException(nomePropriedade, "Operador não suportado");
            }
        }

        private Exception RetornarException(string nomePropriedade, string mensagem = null)
        {
            var tipoEntidade = this.TipoEntidade;
            var nomesPropriedade = this.NomesPropriedade;
            var memsagem = $"{mensagem} ValidacaoUnicoCompostaAttribute(typeof({tipoEntidade.Name}), {String.Join(", ", nomesPropriedade.Select(x => $"\"{x}\""))} - A propriedade ou {nomePropriedade} não foi encontrada  no tipo '{tipoEntidade.Name}'";
            return new Exception(memsagem);
        }
        #region IAtributoValidacao

        public bool IsValido(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
        {
            //validação no lado cliente;
            return true;
        }

        public string RetornarMensagemValidacao(PropertyInfo propriedade, object paiPropriedade, object valorPropriedade)
        {
            var rotulo = ReflexaoUtil.RetornarRotulo(propriedade);
            return String.Format(MensagemValidacao, rotulo);
        }
        #endregion
    }

    public class PropriedadeIndexar
    {
        public PropertyInfo Propriedade { get; }
        public bool IsPermitirNulo { get; }

        public PropriedadeIndexar(PropertyInfo propriedade) : this(propriedade, false)
        {
        }

        public PropriedadeIndexar(PropertyInfo propriedade, bool aceitaNulo)
        {
            this.Propriedade = propriedade;
            this.IsPermitirNulo = aceitaNulo;
        }
    }

    public class FiltroPropriedadeIndexar
    {
        public PropertyInfo Propriedade { get; private set; }
        public EnumOperadorComparacao Operador { get; }
        public string Valor { get; }
        public string OperadoprString
        {
            get
            {
                switch (this.Operador)
                {
                    case EnumOperadorComparacao.Igual:
                        return " = ";
                    case EnumOperadorComparacao.Diferente:
                        return " <> ";
                    case EnumOperadorComparacao.MaiorQue:
                        return " > ";
                    case EnumOperadorComparacao.MenorQue:
                        return " < ";
                    case EnumOperadorComparacao.MaiorIgualA:
                        return " >= ";
                    case EnumOperadorComparacao.MenorIgualA:
                        return " <= ";
                    default:
                        throw new Exception("Operador não suportador");
                }
            }
        }

        public FiltroPropriedadeIndexar(PropertyInfo propriedade,
                                        EnumOperadorComparacao operadopr,
                                        string valor)
        {
            this.Propriedade = propriedade;
            this.Operador = operadopr;
            this.Valor = valor;
        }
    }
}