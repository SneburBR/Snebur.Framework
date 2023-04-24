using System;

using Snebur.Imagens;
using Snebur.Dominio;

namespace Snebur.Utilidade
{
    public class ImagemApresentacaoUtil
    {
        public static Dimensao RetornarTamanhoRecipienteImagem(EnumTamanhoImagem tamanhoImagem)
        {
            switch (tamanhoImagem)
            {
                case (EnumTamanhoImagem.Miniatura):

                    return new Dimensao(ConstantesImagemApresentacao.LARGURA_IMAGEM_MINIATURA, ConstantesImagemApresentacao.ALTURA_IMAGEM_MINIATURA);

                case (EnumTamanhoImagem.Pequena):

                    return new Dimensao(ConstantesImagemApresentacao.LARGURA_IMAGEM_PEQUENA, ConstantesImagemApresentacao.ALTURA_IMAGEM_PEQUENA);

                case (EnumTamanhoImagem.Media):

                    return new Dimensao(ConstantesImagemApresentacao.LARGURA_IMAGEM_MEDIA, ConstantesImagemApresentacao.ALTURA_IMAGEM_MEDIA);

                case (EnumTamanhoImagem.Grande):

                    return new Dimensao(ConstantesImagemApresentacao.LARGURA_IMAGEM_GRANDE, ConstantesImagemApresentacao.ALTURA_IMAGEM_GRANDE);

                case (EnumTamanhoImagem.Impressao):

                    return new Dimensao(0, 0);

                default:

                    throw new ErroNaoImplementado("Tamanho da imagem não suportado");
            }
        }

        public static long RetornarQualidade(EnumTamanhoImagem tamanhoImagem)
        {
            switch (tamanhoImagem)
            {
                case (EnumTamanhoImagem.Miniatura):

                    return 40L;

                case (EnumTamanhoImagem.Pequena):
                case (EnumTamanhoImagem.Media):

                    return 60L;

                case (EnumTamanhoImagem.Grande):

                    return 70L;

                case (EnumTamanhoImagem.Impressao):

                    return 100L;

                default:

                    throw new ErroNaoSuportado("Tamanho imagem não suportado");
            }
        }

        private EnumTamanhoImagem RetornarTamanhoImagemAutomatico(IImagem imagem, Dimensao dimensaoRecipiente)
        {
            var largura = dimensaoRecipiente.LarguraPixels;
            var altura = dimensaoRecipiente.AlturaPixels;

            if (imagem.IsExisteGrande && ((largura >= ConstantesImagemApresentacao.LARGURA_IMAGEM_GRANDE) || (altura > ConstantesImagemApresentacao.ALTURA_IMAGEM_GRANDE)))
            {
                return EnumTamanhoImagem.Grande;
            }
            if (imagem.IsExisteMedia && ((largura >= ConstantesImagemApresentacao.LARGURA_IMAGEM_MEDIA) || (altura > ConstantesImagemApresentacao.ALTURA_IMAGEM_MEDIA)))
            {
                return EnumTamanhoImagem.Media;
            }
            if (imagem.IsExistePequena && ((largura >= ConstantesImagemApresentacao.LARGURA_IMAGEM_PEQUENA) || (altura > ConstantesImagemApresentacao.ALTURA_IMAGEM_PEQUENA)))
            {
                return EnumTamanhoImagem.Pequena;
            }
            if (imagem.IsExisteMiniatura && ((largura >= ConstantesImagemApresentacao.LARGURA_IMAGEM_MINIATURA) || (altura > ConstantesImagemApresentacao.ALTURA_IMAGEM_MINIATURA)))
            {
                return EnumTamanhoImagem.Miniatura;
            }
            return EnumTamanhoImagem.Pequena;
        }
    }

}