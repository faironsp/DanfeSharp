using DanfeSharp.Elementos;
using DanfeSharp.Elementos.CFeSat;
using DanfeSharp.Enumeracoes;
using DanfeSharp.Modelo;

namespace DanfeSharp.Blocos.CFeSat
{
    internal class BlocoInfo : BlocoBase
    {
        Info info;

        public BlocoInfo(DanfeViewModel viewModel, Estilo estilo) : base(viewModel, estilo)
        {
            var height = 26;

            info = new Info(Estilo, ViewModel);

            FlexibleLine fl = new FlexibleLine() { Height = height }
            .ComElemento(info)
            .ComLargurasIguais();

            MainVerticalStack.Add(fl);
        }

        public override string Cabecalho => "";
        public override PosicaoBloco Posicao => PosicaoBloco.Base;
    }
}
