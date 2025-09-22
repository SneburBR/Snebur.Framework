namespace Snebur.ServicoArquivo.Comunicacao
{
    public abstract class ComunicacaoServicoImagem : ComunicacaoServicoArquivo, IComunicacaoServicoImagem
    {
        #region IComunicacaoServicoImagem

        public bool ExisteImagem(long idImagem, EnumTamanhoImagem tamanhoImagem)
        {
            using (var contexto = this.RetornarContextoDados())
            {
                var imagem = contexto.RetornarConsulta<IImagem>(contexto.TipoEntidadeArquivoRequired).Where(x => x.Id == idImagem).SingleOrDefault();
                if (imagem != null)
                {
                    switch (tamanhoImagem)
                    {

                        case (EnumTamanhoImagem.Miniatura):

                            return imagem.IsExisteMiniatura;

                        case (EnumTamanhoImagem.Pequena):

                            return imagem.IsExistePequena;

                        case (EnumTamanhoImagem.Media):

                            return imagem.IsExisteMedia;

                        case (EnumTamanhoImagem.Grande):

                            return imagem.IsExisteGrande;

                        case (EnumTamanhoImagem.Impressao):

                            return imagem.IsExisteArquivo;

                        default:

                            throw new ErroNaoSuportado(String.Format("O tamanho da imagem não é suportado {0} ", EnumUtil.RetornarDescricao(tamanhoImagem)));
                    }
                }
                return false;
            }
        }

        public bool NotificarFimEnvioImagem(long idImagem, long totalBytes, EnumTamanhoImagem tamanhoImagem, string checksum)
        {
            using (var contexto = this.RetornarContextoDados())
            {
                var imagem = contexto.RetornarConsulta<IImagem>(contexto.TipoEntidadeArquivoRequired)
                    .Where(x => x.Id == idImagem)
                    .SingleOrDefault();
                if (imagem != null)
                {
                    switch (tamanhoImagem)
                    {
                        case (EnumTamanhoImagem.Miniatura):

                            imagem.IsExisteMiniatura = true;
                            imagem.TotalBytesMiniatura = totalBytes;
                            //imagem.DimensaoImagemMiniatura = dimensao;
                            break;

                        case (EnumTamanhoImagem.Pequena):

                            imagem.IsExistePequena = true;
                            imagem.TotalBytesPequena = totalBytes;
                            //imagem.DimensaoImagemPequena = dimensao;
                            break;

                        case (EnumTamanhoImagem.Media):

                            imagem.IsExisteMedia = true;
                            imagem.TotalBytesMedia = totalBytes;
                            //imagem.DimensaoImagemMedia = dimensao;
                            break;

                        case (EnumTamanhoImagem.Grande):

                            imagem.IsExisteGrande = true;
                            imagem.TotalBytesGrande = totalBytes;
                            //imagem.DimensaoImagemGrande = dimensao;
                            break;

                        case (EnumTamanhoImagem.Impressao):

                            imagem.DataHoraFimEnvio = DateTime.UtcNow;
                            imagem.IsExisteArquivo = true;
                            imagem.TotalBytes = totalBytes;
                            imagem.Checksum = checksum;
                            //imagem.DimensaoImagemOrigem = dimensao;
                            break;
                        default:

                            throw new ErroNaoSuportado(String.Format("O tamanho da imagem não é suportado {0} ", EnumUtil.RetornarDescricao(tamanhoImagem)));
                    }
                    contexto.Salvar(imagem);
                }
                return false;
            }
        }
        #endregion
    }
}