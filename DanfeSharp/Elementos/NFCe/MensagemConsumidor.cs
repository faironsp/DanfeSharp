using DanfeSharp.Enumeracoes;
using DanfeSharp.Graphics;
using DanfeSharp.Modelo;

namespace DanfeSharp.Elementos.NFCe
{
    internal class MensagemConsumidor : ElementoBase
    {
        public DanfeViewModel ViewModel { get; private set; }

        public MensagemConsumidor(Estilo estilo, DanfeViewModel viewModel) : base(estilo)
        {
            ViewModel = viewModel;
        }

        public override void Draw(Gfx gfx)
        {
            base.Draw(gfx);

            var rp = BoundingBox.InflatedRetangle(0.75F);

            var f1 = Estilo.CriarFonteRegular(6);
            gfx.DrawString("ÁREA DE MENSAGENS DE INTERESSE DO CONTRIBUINTE", rp, f1, AlinhamentoHorizontal.Centro);
            rp = rp.CutTop(f1.AlturaLinha);
        }
    }
}
