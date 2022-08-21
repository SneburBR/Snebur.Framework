using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Snebur.Dominio;

namespace Snebur.AcessoDados.Admin
{
    public abstract class BaseJanelaLista : BaseJanela
    {
        public static readonly DependencyProperty EntidadeSelecionadoProperty = DependencyProperty.Register("EntidadeSelecionada", typeof(Entidade), typeof(BaseJanelaLista));

        public Entidade EntidadeSelecionada
        {
            get
            {
                return this.GetValue(BaseJanelaLista.EntidadeSelecionadoProperty) as Entidade;
            }
            set
            {
                this.SetValue(BaseJanelaLista.EntidadeSelecionadoProperty, value);
            }
        }

        public ObservableCollection<IEntidade> Lista { get; set; } = new ObservableCollection<IEntidade>();
        

        public BaseJanelaLista()
        {
            this.DataContext = this;
            this.Loaded += this.JanelaLista_Loaded;
        }

        private void JanelaLista_Loaded(object sender, RoutedEventArgs e)
        {
            this.AtualizarLista();
        }

        public void AtualizarLista()
        {
            this.Lista.Clear();

            using (var contexto = ContextoDadosUtil.RetornarContextoDados())
            {
                var consulta = this.RetornarConsulta(contexto);
                var entidades = consulta.ToList();
                this.Lista.AddRange(entidades);
            }
        }

        public void Novo()
        {
            var janela = this.RetornarJanelaCadastro(null);
            janela.Owner = this;

            var resultado = janela.ShowDialog();
            if (resultado.HasValue && resultado.Value)
            {
                this.AtualizarLista();
            }
        }

        public void Excluir()
        {
            var entidade = this.EntidadeSelecionada;
            if (entidade != null)
            {
                var mensagem = "Você tem certezq que deseja excluir este registro?";
                if (MessageBox.Show(this, mensagem, "Atenção", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    using (var contexto = ContextoDadosUtil.RetornarContextoDados())
                    {
                        var resultado = contexto.ExcluirSeguranca(entidade);
                        if (resultado.IsSucesso)
                        {
                            this.AtualizarLista();
                        }
                    }
                }
            }
        }

        public void Editar()
        {
            var entidade = this.EntidadeSelecionada;
            if (entidade != null)
            {
                var janela = this.RetornarJanelaCadastro(entidade);
                janela.Owner = this;


                var resultado = janela.ShowDialog();
                if (resultado.HasValue && resultado.Value)
                {
                    this.AtualizarLista();
                }
            }
        }

        public abstract IConsultaEntidade<IEntidade> RetornarConsulta(IContextoDadosSeguranca contexto);

        public abstract BaseJanelaCadastro RetornarJanelaCadastro(Entidade entidade);

    }
}
