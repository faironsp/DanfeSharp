using DanfeSharp.Elementos;
using DanfeSharp.Elementos.NFCe;
using DanfeSharp.Enumeracoes;
using DanfeSharp.Modelo;
using org.pdfclown.documents.contents.xObjects;

namespace DanfeSharp.Blocos.NFCe
{
    internal class BlocoIdentificacaoEmitente : BlocoBase
    {
        public const float LarguraCampoChaveNFe = 93F;
        public const float AlturaLinha1 = 30;

        IdentificacaoEmitente idEmitente;

        public BlocoIdentificacaoEmitente(DanfeViewModel viewModel, Estilo estilo) : base(viewModel, estilo)
        {
            idEmitente = new IdentificacaoEmitente(Estilo, ViewModel);

            FlexibleLine fl = new FlexibleLine() { Height = 40 }
            .ComElemento(idEmitente)
            .ComLargurasIguais();

            MainVerticalStack.Add(fl);
        }

        public XObject Logo
        {
            get => idEmitente.Logo;
            set => idEmitente.Logo = value;
        }

        public override PosicaoBloco Posicao => PosicaoBloco.Topo;
        public override bool VisivelSomentePrimeiraPagina => false;
    }
}
