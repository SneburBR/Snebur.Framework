//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;
//using Snebur.AcessoDados.Estrutura;
//using Snebur.Dominio.Atributos;

//namespace Snebur.AcessoDados.Utilidade
//{
//    public class DepedenciaOrdenadas
//    {
//        private HashSet<EstruturaEntidade> TodasEstruturasEntidade { get; }
//        private HashSet<EstruturaEntidade> EstruturasEncontradas { get; } = new HashSet<EstruturaEntidade>();
//        private List<EstruturaEntidade> Ordenado { get; } = new List<EstruturaEntidade>();

//        private DepedenciaOrdenadas(Type tipoContexto)
//        {
//            EstruturaBancoDados.RetornarEstruturaBancoDados(tipoContexto);
//            this.TodasEstruturasEntidade = EstruturaBancoDados.Atual.EstruturasEntidade.Values.ToHashSet();
//        }

//        private List<Type> RetornarDepedenciasOrdernadasInterno()
//        {
//            while (this.TodasEstruturasEntidade.Count > 0)
//            {
//                var proximoEstruturaEntidade = this.RetornarProximaEstruturaEntidade();
//                if (proximoEstruturaEntidade == null)
//                {
//                    throw new Erro("Não foi possivel retornar a proxima estrutura entidade");
//                }
//                this.TodasEstruturasEntidade.Remove(proximoEstruturaEntidade);
//                this.EstruturasEncontradas.Add(proximoEstruturaEntidade);
//                this.Ordenado.Add(proximoEstruturaEntidade);

//            }
//            return this.Ordenado.Select(x => x.TipoEntidade).ToList().ToList();

//        }

//        private EstruturaEntidade RetornarProximaEstruturaEntidade()
//        {
//            return this.TodasEstruturasEntidade.Where(x => !this.IsExisteDepdencia(x)).FirstOrDefault();
//        }

//        private bool IsExisteDepdencia(EstruturaEntidade estruturaEntidade)
//        {
//            var estruturaRelacoes = estruturaEntidade.EstruturasRelacoes.Values.OfType<EstruturaRelacaoChaveEstrangeira>().ToList();
//            estruturaRelacoes.Except(estruturaRelacoes.Where(x => x.Propriedade.GetCustomAttribute<ChaveEstrangeiraRelacaoUmUmAttribute>() != null)).ToList();

//            var estruturaChaveEstrangeirasDepedente = estruturaRelacoes.Where(x => !this.EstruturasEncontradas.Contains(x.EstruturaEntidadeChaveEstrangeiraDeclarada)).ToList();
//            var isRelacaoDepedenteChaveEstrangeira = (estruturaRelacoes.Count > 0 && estruturaChaveEstrangeirasDepedente.Count > 0);
//            var isExisteDepedenciaTipoBase = estruturaEntidade.EstruturasEntidadeBase.Count > 0 && estruturaEntidade.EstruturasEntidadeBase.Values.Any(x => !this.EstruturasEncontradas.Contains(x));

//            var isExisteDepedencia = (isRelacaoDepedenteChaveEstrangeira || isExisteDepedenciaTipoBase); 

//            if (isExisteDepedencia)
//            {
//                if(!estruturaChaveEstrangeirasDepedente.Select(x=> x.EstruturaEntidadeChaveEstrangeiraDeclarada).All(x=> this.TodasEstruturasEntidade.Contains(x) || this.EstruturasEncontradas.Contains(x)))
//                {
//                    throw new Erro("aaa");
//                }

//                if (!estruturaEntidade.EstruturasEntidadeBase.Values.All(x => this.TodasEstruturasEntidade.Contains(x) || this.EstruturasEncontradas.Contains(x)))
//                {
//                    throw new Erro("aaa");
//                }
//            }
//            return isExisteDepedencia;

//        }

//        public static List<Type> RetornarDepedenciasOrdernadas(Type tipoContexto)
//        {
//            var novo = new DepedenciaOrdenadas(tipoContexto);
//            return novo.RetornarDepedenciasOrdernadasInterno();
//        }
//    }
//}
