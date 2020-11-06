using DanfeSharp.Enumeracoes;
using DanfeSharp.Graphics;
using DanfeSharp.Modelo;

namespace DanfeSharp.Elementos.NFCe
{
    internal class Info : ElementoBase
    {
        public DanfeViewModel ViewModel { get; private set; }

        public Info(Estilo estilo, DanfeViewModel viewModel) : base(estilo)
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

            var ts = new TextStack(rp) { LineHeightScale = 1 }
                  .AddLine("ÁREA DE MENSAGEM FISCAL", f1)
                  .AddLine("", f2);

            ts.AddLine($"Número: {ViewModel.NfNumero} Série: {ViewModel.NfSerie} Emissão: {ViewModel.DataHoraEmissao}", f3);

            ts.AddLine("", f2);
            
            ts.AddLine("CHAVE DE ACESSO", f1);
            ts.AddLine("", f2);

            ts.AddLine(ViewModel.ChaveAcesso, f2);
            ts.AddLine("", f2);
            ts.AddLine($"Protocolo de Autorização: {ViewModel.ProtocoloAutorizacao}", f3);

            ts.AlinhamentoHorizontal = AlinhamentoHorizontal.Centro;
            ts.AlinhamentoVertical = AlinhamentoVertical.Centro;
            ts.Draw(gfx);
        }
    }
}
