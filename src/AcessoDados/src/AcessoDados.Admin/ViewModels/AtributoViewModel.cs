using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur.Dominio;

namespace Snebur.AcessoDados.Admin.ViewModels
{
    public class AtributoViewModel : BaseViewModel
    {
        public string Descricao { get;  }

        public PermissaoCampoViewModel PermissaoCampoViewModel { get; }

        public Attribute Atributo { get; }

        public  AtributoViewModel(PermissaoCampoViewModel permissaoCampoViewModel,Attribute atributo)
        {
            this.PermissaoCampoViewModel = permissaoCampoViewModel;
            this.Atributo = atributo;
            this.Descricao = this.RetornarDescricao();
        }

        private string RetornarDescricao()
        {
            var descricao = this.Atributo.ToString();
            descricao = descricao.Replace(this.Atributo.GetType().Namespace + ".", String.Empty);
            descricao = descricao.Replace("Attribute", String.Empty);

            return descricao;
        }

    
    }
}
