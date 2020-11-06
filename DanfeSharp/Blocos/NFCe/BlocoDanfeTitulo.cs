using DanfeSharp.Elementos;
using DanfeSharp.Elementos.NFCe;
using DanfeSharp.Enumeracoes;
using DanfeSharp.Modelo;

namespace DanfeSharp.Blocos.NFCe
{
    internal class BlocoDanfeTitulo : BlocoBase
    {
        DanfeTitulo danfeTitulo;

        public BlocoDanfeTitulo(DanfeViewModel viewModel, Estilo estilo) : base(viewModel, estilo)
        {
            var height = 7;

            danfeTitulo = new DanfeTitulo(Estilo, ViewModel);

            FlexibleLine fl = new FlexibleLine() { Height = height }
            .ComElemento(danfeTitulo)
            .ComLargurasIguais();

            MainVerticalStack.Add(fl);
        }

        public override string Cabecalho => "";
        public override PosicaoBloco Posicao => PosicaoBloco.Topo;
    }
}
