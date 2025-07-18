using Snebur.Dominio;

namespace Snebur.ServicoArquivo
{
    public class ImagemUtil
    {

        public static Dimensao RetornaDimencaoMaximaProporcional(int larguraImagem, int alturaImagem, int larguraMaxima, int alturaMaxima)
        {
            int novaLargura = 0;
            int novaAltura = 0;

            if (larguraImagem > alturaImagem)
            {
                //IMAGEM NA HORIZONTAL
                novaLargura = larguraMaxima;
                novaAltura = alturaImagem * novaLargura / larguraImagem;

                if (novaAltura > alturaMaxima)
                {
                    novaAltura = alturaMaxima;
                    novaLargura = larguraImagem * novaAltura / alturaImagem;
                }
            }
            else if (alturaImagem > larguraImagem)
            {
                //IMAGEM NA VERTICAL
                novaAltura = alturaMaxima;
                novaLargura = larguraImagem * novaAltura / alturaImagem;

                if (novaLargura > larguraMaxima)
                {
                    novaLargura = larguraMaxima;
                    novaAltura = alturaImagem * novaLargura / larguraImagem;
                }
            }
            else if (larguraImagem == alturaImagem)
            {
                //IMAGEM QUADRADA ' SELECIONAR O MENOR LADO
                if (alturaMaxima > larguraMaxima)
                {
                    novaLargura = larguraMaxima;
                    novaAltura = larguraMaxima;
                }
                else
                {
                    novaLargura = alturaMaxima;
                    novaAltura = alturaMaxima;
                }
            }
            return new Dimensao(novaLargura, novaAltura);
        }

    }
}