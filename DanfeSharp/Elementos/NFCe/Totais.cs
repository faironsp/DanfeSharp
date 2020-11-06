using DanfeSharp.Enumeracoes;
using DanfeSharp.Graphics;
using DanfeSharp.Modelo;
using System;

namespace DanfeSharp.Elementos.NFCe
{
    internal class Totais : ElementoBase
    {
        public DanfeViewModel ViewModel { get; private set; }

        public Totais(Estilo estilo, DanfeViewModel viewModel) : base(estilo)
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

            ts.AddLine("QTD. DE ITENS: "+ ViewModel.Produtos.Count.ToString(), f3);
            ts.AddLine("", f2);

            ts.AddLine("VALOR TOTAL", f3);
            ts.AddLine(Convert.ToDecimal(m.ValorTotalProdutos).ToString("c"), f2);
            ts.AddLine("", f2);

            ts.AddLine("DESCONTO", f3);
            ts.AddLine(m.Desconto.ToString("c"), f2);
            ts.AddLine("", f2);

            ts.AddLine("VALOR A PAGAR", f3);
            ts.AddLine(m.ValorTotalNota.ToString("c"), f2);
            ts.AddLine("", f2);

            ts.AlinhamentoHorizontal = AlinhamentoHorizontal.Direita;
            ts.AlinhamentoVertical = AlinhamentoVertical.Centro;
            ts.Draw(gfx);
        }
    }
}
