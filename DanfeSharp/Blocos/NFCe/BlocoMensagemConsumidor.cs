using DanfeSharp.Elementos;
using DanfeSharp.Elementos.NFCe;
using DanfeSharp.Enumeracoes;
using DanfeSharp.Modelo;

namespace DanfeSharp.Blocos.NFCe
{
    internal class BlocoMensagemConsumidor : BlocoBase
    {
        MensagemConsumidor mensagemConsumidor;

        public BlocoMensagemConsumidor(DanfeViewModel viewModel, Estilo estilo) : base(viewModel, estilo)
        {
            var height = 10;

            mensagemConsumidor = new MensagemConsumidor(Estilo, ViewModel);

            FlexibleLine fl = new FlexibleLine() { Height = height }
            .ComElemento(mensagemConsumidor)
            .ComLargurasIguais();

            MainVerticalStack.Add(fl);
        }

        public override string Cabecalho => "";
        public override PosicaoBloco Posicao => PosicaoBloco.Base;
    }
}
