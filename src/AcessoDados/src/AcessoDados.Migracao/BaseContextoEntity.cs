
using Snebur.Dominio;
using System;
using System.Linq;

#if NET9_0_OR_GREATER
using Microsoft.EntityFrameworkCore;
#endif
#if NET48
using System.Data.Entity;
#endif

namespace Snebur.AcessoDados.Migracao
{
    public partial class BaseContextoEntity : DbContext
    {
        //#region " Propriedades "

        private string ConnectionString;

        private Type[] _tiposEntidade;

        public Type[] TiposEntidade
        {
            get
            {
                if (this._tiposEntidade == null)
                {
                    this._tiposEntidade = BaseContextoUtil.RetornarTiposBaseEntidade(this);
                }
                return this._tiposEntidade;
            }
        }

        public string NomeConnectionString { get; }
        //#endregion

        public BaseContextoEntity(string nomeConnectionString) 
        {
#if !EXTENSAO_VISUALSTUDIO

#endif
            this.NomeConnectionString = nomeConnectionString;
            if (System.Configuration.ConfigurationManager.ConnectionStrings[nomeConnectionString] == null)
            {
                throw new Exception(String.Format("Não foi possível encontrar a string de conexão '{0}'.", nomeConnectionString));
            }

            this.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings[nomeConnectionString].ConnectionString;
            //this.Configuration.ProxyCreationEnabled = false;
            //this.Configuration.AutoDetectChangesEnabled = false;
            //this.Configuration.ValidateOnSaveEnabled = false;
            //this.Configuration.LazyLoadingEnabled = false;
        }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    //modelBuilder.Properties<string>()
        //    //         .Configure(c => c.HasMaxLength(1000));

        //    modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        //    modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        //    modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

        //    modelBuilder.ComplexType<Dimensao>();
        //    modelBuilder.ComplexType<Margem>();
        //    modelBuilder.ComplexType<Cor>();
        //    modelBuilder.ComplexType<Posicao>();
        //    modelBuilder.ComplexType<Localizacao>();
        //    modelBuilder.ComplexType<Area>();
        //    modelBuilder.ComplexType<Regiao>();
        //    modelBuilder.ComplexType<Navegador>();
        //    modelBuilder.ComplexType<RedeSociais>();
        //    modelBuilder.ComplexType<SistemaOperacional>();
        //    modelBuilder.ComplexType<Borda>();
        //    modelBuilder.ComplexType<PrazoTempo>();
        //    //modelBuilder.ComplexType<Pontos>();

        //    modelBuilder.ComplexType<ListaInt32>();
        //    modelBuilder.ComplexType<ListaDouble>();
        //    modelBuilder.ComplexType<ListaString>();

          
        //    //modelBuilder.ComplexType<ListaDouble>();
        //    //MontarContexto.Inicializar(modelBuilder, this);

        //    base.OnModelCreating(modelBuilder);
        //}

        #region " Salvar "

        public override int SaveChanges()
        {
            this.ChangeTracker.DetectChanges();

            //var ctx = ((IObjectContextAdapter)this).ObjectContext;
            //var mudancas = ctx.ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Deleted | EntityState.Modified);

            //if (mudancas.Count() > 0)
            //{
            //    // throw new Exception("Não usaro EntityFramework");
            //}
            return base.SaveChanges();
        }

        #endregion
    }
}