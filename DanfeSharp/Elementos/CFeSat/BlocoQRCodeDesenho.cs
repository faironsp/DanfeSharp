using DanfeSharp.Enumeracoes;
using DanfeSharp.Graphics;
using DanfeSharp.Modelo;
using System.Drawing;

namespace DanfeSharp.Elementos.CFeSat
{
    internal class BlocoQRCodeDesenho : ElementoBase
    {
        public DanfeViewModel ViewModel { get; private set; }
        public org.pdfclown.documents.contents.xObjects.XObject QrCode { get; set; }

        public BlocoQRCodeDesenho(Estilo estilo, DanfeViewModel viewModel) : base(estilo)
        {
            ViewModel = viewModel;
            QrCode = null;
        }

        public override void Draw(Gfx gfx)
        {
            base.Draw(gfx);

            var rp = BoundingBox.InflatedRetangle(0.75F);

            var f1 = Estilo.CriarFonteRegular(8);
            gfx.DrawString("Consulta via leitor de QR Code", rp, f1, AlinhamentoHorizontal.Centro);

            var sobra = (rp.Width - 50) / 2;

            RectangleF rLogo = new RectangleF(rp.X + sobra, rp.Y + 3, 50, 50);

            gfx.ShowXObject(QrCode, rLogo);
        }
    }
}
