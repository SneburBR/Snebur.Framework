﻿using System;
using System.IO;
using Snebur.Dominio;
using Snebur.Net;

namespace Snebur.ServicoArquivo
{

    public abstract class BaseEnviarImagem : BaseEnviarArquivo<CabecalhoServicoImagem, IInformacaoRepositorioImagem>
    {
        private ComunicacaoServicoImagemCliente ServicoImagemCliente
        {
            get { return (ComunicacaoServicoImagemCliente)this.ServicoArquivoCliente; }
        }

        protected override ComunicacaoServicoArquivoCliente RetornarServicoArquivoCliente(CabecalhoServicoImagem cabecalho)
        {
            var urlServicoArquivo = this.RetornarUrlServicoArquivoCliente();
            return new ComunicacaoServicoImagemCliente(urlServicoArquivo,
                                                      cabecalho.CredencialRequisicao,
                                                      cabecalho.IdentificadorSessaoUsuario,
                                                      this.NormalizarOrigem);
        }

        protected override void NotificarInicioEnvioArquivo(CabecalhoServicoImagem cabecalho, long idArquivo)
        {
            var tamanhoImagem = (cabecalho as CabecalhoServicoImagem).TamanhoImagem;
            if (tamanhoImagem == EnumTamanhoImagem.Impressao)
            {
                base.NotificarInicioEnvioArquivo(cabecalho, idArquivo);
            }
        }

        protected override void NotificarFimEnvioArquivo(CabecalhoServicoImagem cabecalho, long idArquivo, long totalBytes, string checksum)
        {
            if (cabecalho is CabecalhoServicoImagem cabecalhoImagem)
            {
                var tamanhoImagem = (cabecalho as CabecalhoServicoImagem).TamanhoImagem;
                this.ServicoImagemCliente.NotificarFimEnvioImagem(idArquivo, totalBytes, tamanhoImagem, checksum);
            }
            else
            {
                throw new ErroNaoSuportado("o cabecalho da imagem não é suportado");
            }
        }

        protected override void NotificarProgresso(CabecalhoServicoImagem cabecalho, long idArquivo, double progresso)
        {
            if (cabecalho.TamanhoImagem == EnumTamanhoImagem.Impressao)
            {
                base.NotificarProgresso(cabecalho, idArquivo, progresso);
            }
        }

        protected override string RetornarCaminhoArquivoTempoarario(CabecalhoServicoImagem cabecalho, long idArquivo)
        {
            var tamanhoImagem = (cabecalho as CabecalhoServicoImagem).TamanhoImagem;
            var nomeArquivo = String.Format("{0}-{1}.tmp", idArquivo, ServicoImagemUtil.RetornarExtensaoImagem(tamanhoImagem));
            return Path.Combine(this.RetornarCaminhoRepositoTemporario(cabecalho), nomeArquivo);
        }
    
        #region  Métodos sobre-escritos

        protected override CabecalhoServicoImagem RetornarCabecalhoServicoArquivo(ZyonHttpContext httpContext)
        {
            return new CabecalhoServicoImagem(httpContext, true);
        }

        protected override sealed string RetornarDiretorioArquivo(IInformacaoRepositorioImagem informacaoRepositorio)
        {
            return this.RetornarDiretorioImagem(informacaoRepositorio);
        }

        protected override sealed string RetornarRepositoArquivo(IInformacaoRepositorioImagem informacaoRepositorio)
        {
            return this.RetornarRepositorioImagem(informacaoRepositorio);
        }

        public string RetornarDiretorioImagem(IInformacaoRepositorioImagem informacaoRepositorio)
        {
            return ServicoArquivoUtil.RetornarCaminhoDiretorioArquivo(this.RetornarRepositorioImagem(informacaoRepositorio), informacaoRepositorio.IdArquivo);
        }

        protected override string RetornarCaminhoCompletoArquivo(IInformacaoRepositorioImagem informacaoRepositorio)
        {
            var caminhoDiretorioImagem = this.RetornarDiretorioImagem(informacaoRepositorio);
            var nomeArquivo = ServicoArquivoUtil.RetornarNomeArquivo(informacaoRepositorio.IdArquivo, ServicoImagemUtil.RetornarExtensaoImagem(informacaoRepositorio.TamanhoImagem));
            return Path.Combine(caminhoDiretorioImagem, nomeArquivo);
        }
        #endregion

        #region Métodos abstratos 

        protected abstract string RetornarRepositorioImagem(IInformacaoRepositorioImagem informacaoRepositorio);

        #endregion




    }
}