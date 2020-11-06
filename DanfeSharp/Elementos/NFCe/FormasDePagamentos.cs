using System;
using DanfeSharp.Enumeracoes;
using DanfeSharp.Graphics;
using DanfeSharp.Modelo;

namespace DanfeSharp.Elementos.NFCe
{
    internal class FormasDePagamentos : ElementoBase
    {
        #region Enum

        public enum Types
        {
            Dinheiro = 1,
            Cheque = 2,
            CartaoCredito = 3,
            CartaoDebito = 4,
            CreditoLoja = 5,
            ValeAlimentacao = 10,
            ValeRefeicao = 11,
            ValePresente = 12,
            ValeCombustivel = 13,
            BoletoBancario = 15,
            SemPagamento = 90,
            Outro = 99
        }

        #endregion

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

            if (ViewModel.Pagamentos?.Count > 0)
            {
                foreach (var item in ViewModel.Pagamentos)
                {
                    string lDescPag = string.Empty;
                    var lType = (Types)Convert.ToInt32(item.tPag);

                    switch (lType)
                    {
                        case Types.Dinheiro:
                            lDescPag = "Dinheiro";
                            break;
                        case Types.Cheque:
                            lDescPag = "Cheque";
                            break;
                        case Types.CartaoCredito:
                            lDescPag = "Cartão de Crédito";
                            break;
                        case Types.CartaoDebito:
                            lDescPag = "Cartão de Débito";
                            break;
                        case Types.CreditoLoja:
                            lDescPag = "Crédito Loja";
                            break;
                        case Types.ValeAlimentacao:
                            lDescPag = "Vale Alimentação";
                            break;
                        case Types.ValeRefeicao:
                            lDescPag = "Vale Receição";
                            break;
                        case Types.ValePresente:
                            lDescPag = "Vale Presente";
                            break;
                        case Types.ValeCombustivel:
                            lDescPag = "Vale Combustível";
                            break;
                        case Types.BoletoBancario:
                            lDescPag = "Boleto";
                            break;
                        case Types.SemPagamento:
                            lDescPag = "Sem Pagamento";
                            break;
                        case Types.Outro:
                            lDescPag = "Outro";
                            break;
                        default:
                            break;
                    }

                    ts.AddLine(item.vPag.Value.ToString("c") + " - " + lDescPag, f2);
                }
            }
            else
            {
                ts.AddLine(m.ValorTotalNota.ToString("c"), f2);
            }

            ts.AlinhamentoHorizontal = AlinhamentoHorizontal.Esquerda;
            ts.AlinhamentoVertical = AlinhamentoVertical.Topo;
            ts.Draw(gfx);
        }
    }
}
