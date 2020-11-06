using DanfeSharp.Elementos;
using DanfeSharp.Elementos.CFeSat;
using DanfeSharp.Enumeracoes;
using DanfeSharp.Modelo;

namespace DanfeSharp.Blocos.CFeSat
{
    internal class BlocoResumo : BlocoBase
    {
        Totais totais;
        FormasDePagamentos formasDePagamentos;

        public BlocoResumo(DanfeViewModel viewModel, Estilo estilo) : base(viewModel, estilo)
        {
            var height = 30;

            totais = new Totais(Estilo, ViewModel);
            formasDePagamentos = new FormasDePagamentos(Estilo, ViewModel);

            FlexibleLine fl = new FlexibleLine() { Height = height }
            .ComElemento(totais)
            .ComElemento(formasDePagamentos)
            .ComLargurasIguais();

            MainVerticalStack.Add(fl);
        }

        public override string Cabecalho => "";
        public override PosicaoBloco Posicao => PosicaoBloco.Base;
    }
}
