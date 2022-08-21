using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Snebur.Dominio;

namespace Snebur.AcessoDados.Admin
{
    /// <summary>
    /// Interaction logic for JanelaTipoUsuarioAdicionarGrupoUsuario.xaml
    /// </summary>
    public partial class JanelaTipoUsuarioAdicionarGrupoUsuario : BaseJanelaLista
    {
        public JanelaTipoUsuarioAdicionarGrupoUsuario()
        {
            InitializeComponent();
        }

        public override IConsultaEntidade<IEntidade> RetornarConsulta(IContextoDadosSeguranca contexto)
        {
            this.CadastrarTiposUsuario();
            var consulta = contexto.RetornarConsulta<IEntidade>(contexto.TiposSeguranca.TipoUsuarioAdicionarGrupo);
            return consulta;
        }

        private void CadastrarTiposUsuario()
        {
            using (var contexto = ContextoDadosUtil.RetornarContextoDados())
            {
                var tipoUsuario = contexto.TiposSeguranca.TipoUsuario;
                var tiposUsuarioEspecializado = tipoUsuario.Assembly.GetTypes().Where(x => x.IsSubclassOf(tipoUsuario) && !x.IsAbstract).ToList();
                if (tiposUsuarioEspecializado.Count > 0)
                {
                    foreach (var tipoUsuarioEspecializado in tiposUsuarioEspecializado)
                    {

                        var tipoUsuarioAdicionarGrupo = contexto.RetornarConsulta<ITipoUsuarioAdicionarGrupoUsuario>(contexto.TiposSeguranca.TipoUsuarioAdicionarGrupo).
                                                                 Where(x => x.NomeTipoUsuario == tipoUsuarioEspecializado.Name).SingleOrDefault();

                        if (tipoUsuarioAdicionarGrupo == null)
                        {
                            tipoUsuarioAdicionarGrupo = (ITipoUsuarioAdicionarGrupoUsuario)Activator.CreateInstance(contexto.TiposSeguranca.TipoUsuarioAdicionarGrupo);
                            tipoUsuarioAdicionarGrupo.NomeTipoUsuario = tipoUsuarioEspecializado.Name;
                            contexto.SalvarSeguranca(tipoUsuarioAdicionarGrupo);
                        }

                    }

                }
                else
                {
                    if (tipoUsuario.IsAbstract)
                    {
                        throw new Exception("Tipo usuario não suportado");
                    }
                }
            }
        }

        public override BaseJanelaCadastro RetornarJanelaCadastro(Entidade entidade)
        {
            throw new NotImplementedException();
        }
        
        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnEditarGrupos_Click(object sender, RoutedEventArgs e)
        {
            var entidade = this.EntidadeSelecionada;
            if(entidade!= null)
            {
                var janela = new JanelaEditarGruposUsuario(entidade as IMembrosDe);
                janela.Owner = this;
                janela.ShowDialog();
            }
            
        }
    }
}
