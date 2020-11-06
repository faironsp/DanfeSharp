using DanfeSharp.Enumeracoes;
using DanfeSharp.Graphics;
using DanfeSharp.Modelo;

namespace DanfeSharp.Elementos.CFeSat
{
    internal class DanfeTitulo : ElementoBase
    {
        public DanfeViewModel ViewModel { get; private set; }

        public DanfeTitulo(Estilo estilo, DanfeViewModel viewModel) : base(estilo)
        {
            ViewModel = viewModel;
        }

        public override void Draw(Gfx gfx)
        {
            base.Draw(gfx);

            var rp = BoundingBox.InflatedRetangle(0.75F);

            Fonte f1 = Estilo.CriarFonteNegrito(8);
            Fonte f2 = Estilo.CriarFonteNegrito(8);
            Fonte f3 = Estilo.CriarFonteRegular(8);

            var ts = new TextStack(rp) { LineHeightScale = 1 }
                  .AddLine("EXTRATO SAT", f1)
                  .AddLine("CUPOM FISCAL ELETRÔNICO - SAT", f1);

            ts.AlinhamentoHorizontal = AlinhamentoHorizontal.Centro;
            ts.AlinhamentoVertical = AlinhamentoVertical.Centro;
            ts.Draw(gfx);
        }
    }
}
