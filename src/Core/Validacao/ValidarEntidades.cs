using System.Collections;
using System.Diagnostics;

namespace Snebur.Dominio;

public partial class ValidarEntidades : IDisposable
{
    internal IReadOnlyCollection<Entidade> Entidades { get; set; }

    private ValidarEntidades(IReadOnlyCollection<Entidade> entidades)
    {
        this.Entidades = entidades;
    }

    public List<ErroValidacaoInfo> RetornarErroValidacao(object contextoDados)
    {
        var allErros = new List<ErroValidacaoInfo>();
        foreach (var entidade in this.Entidades)
        {
            var entityErros = new List<ErroValidacaoInfo>();
            if (entidade is IDeletado deletado && deletado.IsDeletado)
            {
                continue;
            }
            //Propriedades
            var propriedades = ReflexaoUtil.RetornarPropriedades(entidade.GetType());
            foreach (var propriedade in propriedades)
            {
                if (entidade.Id == 0 || (entidade.__PropriedadesAlteradas != null &&
                                         entidade.__PropriedadesAlteradas.ContainsKey(propriedade.Name)))
                {
                    var atributosValidacao = ValidacaoUtil.RetornarAtributosValidacao(propriedade);
                    if (atributosValidacao.Count > 0)
                    {
                        var valorPropriedade = propriedade.GetValue(entidade);
                        foreach (var atributoValidacao in atributosValidacao)
                        {
                            if (!atributoValidacao.IsValido(propriedade, entidade, valorPropriedade))
                            {
                                Debugger.Break();

                                var erroValidacao = new ErroValidacaoInfo
                                {
                                    NomeTipoEntidade = entidade.__NomeTipoEntidade,
                                    NomePropriedade = propriedade.Name,
                                    Mensagem = String.Format("Entidade : '{0}' - {1}", entidade.__NomeTipoEntidade,
                                                                                       atributoValidacao.RetornarMensagemValidacao(propriedade,
                                                                                                                                   entidade,
                                                                                                                                   valorPropriedade)),
                                    NomeTipoValidacao = atributoValidacao.GetType().Name,
                                    ValorPropriedade = valorPropriedade,
                                };

                                if (DebugUtil.IsAttached)
                                {
                                    //throw new Erro(erroValidacao.Mensagem);
                                }
                                entityErros.Add(erroValidacao);
                            }
                        }
                    }
                }
            }

            //Entidades
            if (entidade.__IsExisteAlteracao)
            {
                var atributosValidacaoEntidade = ValidacaoUtil.RetornarAtributosValidacaoEntidade(entidade.__TipoEntidade);
                foreach (var atributoEntidade in atributosValidacaoEntidade)
                {
                    if (!atributoEntidade.IsValido(contextoDados, this.Entidades, entidade))
                    {
                        Debugger.Break();

                        var mensagem = String.Format("Entidade : '{0}' - {1}", entidade.__NomeTipoEntidade, atributoEntidade.RetornarMensagemValidacao(entidade));
                        var erroValidacao = new ErroValidacaoInfo
                        {
                            NomeTipoEntidade = entidade.__NomeTipoEntidade,
                            NomePropriedade = "ValidacaoEntidade",
                            Mensagem = mensagem,
                            NomeTipoValidacao = atributoEntidade.GetType().Name,
                        };

                        if (DebugUtil.IsAttached)
                        {
                            throw new Erro(erroValidacao.Mensagem);
                        }
                        entityErros.Add(erroValidacao);
                    }
                }
            }

            if (entityErros.Count > 0)
            {
                var args = new EntityValidationEventArgs(entidade,
                    entityErros,
                    contextoDados);

                entidade.OnValidating(args);
                if (!args.Handled)
                {
                    allErros.AddRange(entityErros);
                }
            }
        }
        return allErros;
    }

    #region IDisposable

    public void Dispose()
    {
    }
    #endregion
}