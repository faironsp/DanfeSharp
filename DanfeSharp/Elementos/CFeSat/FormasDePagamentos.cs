using DanfeSharp.Enumeracoes;
using DanfeSharp.Graphics;
using DanfeSharp.Modelo;

namespace DanfeSharp.Elementos.CFeSat
{
    internal class FormasDePagamentos : ElementoBase
    {
        public DanfeViewModel ViewModel { get; private set; }

        public FormasDePagamentos(Estilo estilo, DanfeViewModel viewModel) : base(estilo)
        {
            ViewModel = viewModel;
        }

        public override void Draw(Gfx gfx)
        {
            base.Draw(gfx);

            var rp = BoundingBox.InflatedRetangle(0.75F);

            Fonte f1 = Estilo.CriarFonteNegrito(10);
            Fonte f2 = Estilo.CriarFonteNegrito(8);
            Fonte f3 = Estilo.CriarFonteRegular(8);

            var ts = new TextStack(rp) { LineHeightScale = 1 };

            var m = ViewModel.CalculoImposto;

            ts.AddLine("FORMAS DE PAGAMENTOS", f1);
            ts.AddLine(m.ValorTotalNota.ToString("c"), f2);

            ts.AlinhamentoHorizontal = AlinhamentoHorizontal.Esquerda;
            ts.AlinhamentoVertical = AlinhamentoVertical.Topo;
            ts.Draw(gfx);
        }
    }
}
