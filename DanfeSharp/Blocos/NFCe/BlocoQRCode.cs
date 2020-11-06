using DanfeSharp.Elementos;
using DanfeSharp.Elementos.NFCe;
using DanfeSharp.Enumeracoes;
using DanfeSharp.Modelo;

namespace DanfeSharp.Blocos.NFCe
{
    internal class BlocoQRCode : BlocoBase
    {
        BlocoQRCodeDesenho blocoQRCodeDesenho;

        public BlocoQRCode(DanfeViewModel viewModel, Estilo estilo) : base(viewModel, estilo)
        {
            var height = 54.5f;

            blocoQRCodeDesenho = new BlocoQRCodeDesenho(Estilo, ViewModel);

            FlexibleLine fl = new FlexibleLine() { Height = height }
            .ComElemento(blocoQRCodeDesenho)
            .ComLargurasIguais();

            MainVerticalStack.Add(fl);
        }

        public org.pdfclown.documents.contents.xObjects.XObject QrCode
        {
            get => blocoQRCodeDesenho.QrCode;
            set => blocoQRCodeDesenho.QrCode = value;
        }

        public override string Cabecalho => "";
        public override PosicaoBloco Posicao => PosicaoBloco.Base;
    }
}
