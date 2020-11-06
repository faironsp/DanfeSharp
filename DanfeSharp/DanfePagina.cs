using System;
using System.Drawing;
using System.Linq;
using DanfeSharp.Blocos;
using DanfeSharp.Elementos;
using DanfeSharp.Enumeracoes;
using DanfeSharp.Graphics;
using org.pdfclown.documents;
using org.pdfclown.documents.contents.composition;

namespace DanfeSharp
{
    internal class DanfePagina
    {
        public Danfe Danfe { get; private set; }
        public Page PdfPage { get; private set; }
        public PrimitiveComposer PrimitiveComposer { get; private set; }
        public Gfx Gfx { get; private set; }
        public RectangleF RetanguloNumeroFolhas { get; set; }
        public RectangleF RetanguloCorpo { get; private set; }
        public RectangleF RetanguloDesenhavel { get; private set; }
        public RectangleF RetanguloCreditos { get; private set; }
        public RectangleF Retangulo { get; private set; }

        public DanfePagina(Danfe danfe)
        {
            Danfe = danfe ?? throw new ArgumentNullException(nameof(danfe));
            PdfPage = new Page(Danfe.PdfDocument);
            Danfe.PdfDocument.Pages.Add(PdfPage);

            PrimitiveComposer = new PrimitiveComposer(PdfPage);
            Gfx = new Gfx(PrimitiveComposer);

            if (Danfe.ViewModel.TipoLeituraDeXML == Modelo.DanfeViewModel.enumTipoLeituraDeXML.NFe)
            {
                if (Danfe.Folha == TipoFolha.A4)
                {
                    if (Danfe.ViewModel.Orientacao == Orientacao.Retrato)
                        Retangulo = new RectangleF(0, 0, Constantes.A4Largura, Constantes.A4Altura);
                    else
                        Retangulo = new RectangleF(0, 0, Constantes.A4Altura, Constantes.A4Largura);
                }
                else if (Danfe.Folha == TipoFolha.A5)
                {
                    throw new Exception("Não é permitida a impressão de NF-e no tipo de folha A5");
                }
                else if (Danfe.Folha == TipoFolha.CupomTermico80mm)
                {
                    throw new Exception("Não é permitida a impressão de NF-e no tipo de cupom térmico de 80mm");
                }
            }
            else if (Danfe.ViewModel.TipoLeituraDeXML == Modelo.DanfeViewModel.enumTipoLeituraDeXML.NFCe)
            {
                if (Danfe.Folha == TipoFolha.A4)
                {
                    if (Danfe.ViewModel.Orientacao == Orientacao.Retrato)
                        Retangulo = new RectangleF(0, 0, Constantes.A4Largura, Constantes.A4Altura);
                    else
                        Retangulo = new RectangleF(0, 0, Constantes.A4Altura, Constantes.A4Largura);
                }
                else if (Danfe.Folha == TipoFolha.A5)
                {
                    if (Danfe.ViewModel.Orientacao == Orientacao.Retrato)
                        Retangulo = new RectangleF(0, 0, Constantes.A5Largura, Constantes.A5Altura);
                    else
                        Retangulo = new RectangleF(0, 0, Constantes.A5Altura, Constantes.A5Largura);
                }
                else if (Danfe.Folha == TipoFolha.CupomTermico80mm)
                {
                    Retangulo = new RectangleF(0, 0, Constantes.Cupom80mmLargura, Constantes.Cupom80mmAltura);
                }
            }
            else if (Danfe.ViewModel.TipoLeituraDeXML == Modelo.DanfeViewModel.enumTipoLeituraDeXML.CFeSat)
            {
                if (Danfe.Folha == TipoFolha.A4)
                {
                    if (Danfe.ViewModel.Orientacao == Orientacao.Retrato)
                        Retangulo = new RectangleF(0, 0, Constantes.A4Largura, Constantes.A4Altura);
                    else
                        Retangulo = new RectangleF(0, 0, Constantes.A4Altura, Constantes.A4Largura);
                }
                else if (Danfe.Folha == TipoFolha.A5)
                {
                    if (Danfe.ViewModel.Orientacao == Orientacao.Retrato)
                        Retangulo = new RectangleF(0, 0, Constantes.A5Largura, Constantes.A5Altura);
                    else
                        Retangulo = new RectangleF(0, 0, Constantes.A5Altura, Constantes.A5Largura);
                }
                else if (Danfe.Folha == TipoFolha.CupomTermico80mm)
                {
                    Retangulo = new RectangleF(0, 0, Constantes.Cupom80mmLargura, Constantes.Cupom80mmAltura);
                }
            }

            RetanguloDesenhavel = Retangulo.InflatedRetangle(Danfe.ViewModel.Margem);
            RetanguloCreditos = new RectangleF(RetanguloDesenhavel.X, RetanguloDesenhavel.Bottom + Danfe.EstiloPadrao.PaddingSuperior, RetanguloDesenhavel.Width, Retangulo.Height - RetanguloDesenhavel.Height - Danfe.EstiloPadrao.PaddingSuperior);
            PdfPage.Size = new SizeF(Retangulo.Width.ToPoint(), Retangulo.Height.ToPoint());
        }

        public void DesenharCreditos()
        {
            Gfx.DrawString("Impresso com DanfeSharp", RetanguloCreditos, Danfe.EstiloPadrao.CriarFonteItalico(6), AlinhamentoHorizontal.Direita);
        }

        private void DesenharCanhoto()
        {
            if (Danfe.ViewModel.QuantidadeCanhotos == 0) return;

            if (Danfe.Canhoto != null)
            {
                var canhoto = Danfe.Canhoto;
                canhoto.SetPosition(RetanguloDesenhavel.Location);

                if (Danfe.ViewModel.Orientacao == Orientacao.Retrato)
                {
                    canhoto.Width = RetanguloDesenhavel.Width;

                    for (int i = 0; i < Danfe.ViewModel.QuantidadeCanhotos; i++)
                    {
                        canhoto.Draw(Gfx);
                        canhoto.Y += canhoto.Height;
                    }

                    RetanguloDesenhavel = RetanguloDesenhavel.CutTop(canhoto.Height * Danfe.ViewModel.QuantidadeCanhotos);
                }
                else
                {
                    canhoto.Width = RetanguloDesenhavel.Height;
                    Gfx.PrimitiveComposer.BeginLocalState();
                    Gfx.PrimitiveComposer.Rotate(90, new PointF(0, canhoto.Width + canhoto.X + canhoto.Y).ToPointMeasure());

                    for (int i = 0; i < Danfe.ViewModel.QuantidadeCanhotos; i++)
                    {
                        canhoto.Draw(Gfx);
                        canhoto.Y += canhoto.Height;
                    }

                    Gfx.PrimitiveComposer.End();
                    RetanguloDesenhavel = RetanguloDesenhavel.CutLeft(canhoto.Height * Danfe.ViewModel.QuantidadeCanhotos);

                }
            }
        }

        public void DesenhaNumeroPaginas(int n, int total)
        {
            if (n <= 0) throw new ArgumentOutOfRangeException(nameof(n));
            if (total <= 0) throw new ArgumentOutOfRangeException(nameof(n));
            if (n > total) throw new ArgumentOutOfRangeException("O número da página atual deve ser menor que o total.");

            Gfx.DrawString($"Folha {n}/{total}", RetanguloNumeroFolhas, Danfe.EstiloPadrao.FonteNumeroFolhas, AlinhamentoHorizontal.Centro);
            Gfx.Flush();
        }

        public void DesenharAvisoHomologacao()
        {
            TextStack ts = new TextStack(RetanguloCorpo) { AlinhamentoVertical = AlinhamentoVertical.Centro, AlinhamentoHorizontal = AlinhamentoHorizontal.Centro, LineHeightScale = 0.9F }
                        .AddLine("SEM VALOR FISCAL", Danfe.EstiloPadrao.CriarFonteRegular(48))
                        .AddLine("AMBIENTE DE HOMOLOGAÇÃO", Danfe.EstiloPadrao.CriarFonteRegular(30));

            Gfx.PrimitiveComposer.BeginLocalState();
            Gfx.PrimitiveComposer.SetFillColor(new org.pdfclown.documents.contents.colorSpaces.DeviceRGBColor(0.35, 0.35, 0.35));
            ts.Draw(Gfx);
            Gfx.PrimitiveComposer.End();
        }

        public void DesenhaBloco(BlocoBase bloco, float x, float y)
        {
            bloco.Width = RetanguloDesenhavel.Width;

            bloco.SetPosition(x, y);

            bloco.Draw(Gfx);

            RetanguloCorpo = RetanguloDesenhavel;
            Gfx.Flush();
        }

        public void DesenharBlocos(bool isPrimeirapagina = false)
        {
            if (isPrimeirapagina && Danfe.ViewModel.QuantidadeCanhotos > 0) DesenharCanhoto();

            var blocos = isPrimeirapagina ? Danfe._Blocos : Danfe._Blocos.Where(x => x.VisivelSomentePrimeiraPagina == false);

            foreach (var bloco in blocos)
            {
                bloco.Width = RetanguloDesenhavel.Width;

                if (bloco.Posicao == PosicaoBloco.Topo)
                {
                    bloco.SetPosition(RetanguloDesenhavel.Location);
                    RetanguloDesenhavel = RetanguloDesenhavel.CutTop(bloco.Height);
                }
                else
                {
                    bloco.SetPosition(RetanguloDesenhavel.X, RetanguloDesenhavel.Bottom - bloco.Height);
                    RetanguloDesenhavel = RetanguloDesenhavel.CutBottom(bloco.Height);
                }

                bloco.Draw(Gfx);

                if (bloco is Blocos.NFe.BlocoIdentificacaoEmitente)
                {
                    var rf = (bloco as Blocos.NFe.BlocoIdentificacaoEmitente).RetanguloNumeroFolhas;
                    RetanguloNumeroFolhas = rf;
                }
            }

            RetanguloCorpo = RetanguloDesenhavel;
            Gfx.Flush();
        }

        public void RedefineRetanguloPagina(float tamanhoTabela)
        {
            float cupom80mmAltura = tamanhoTabela;

            foreach (var item in Danfe._Blocos)
            {
                cupom80mmAltura += item.Size.Height;
            }

            Retangulo = new RectangleF(0, 0, Constantes.Cupom80mmLargura, cupom80mmAltura);

            RetanguloDesenhavel = Retangulo.InflatedRetangle(Danfe.ViewModel.Margem);
            RetanguloCreditos = new RectangleF(RetanguloDesenhavel.X, RetanguloDesenhavel.Bottom + Danfe.EstiloPadrao.PaddingSuperior, RetanguloDesenhavel.Width, Retangulo.Height - RetanguloDesenhavel.Height - Danfe.EstiloPadrao.PaddingSuperior);
            PdfPage.Size = new SizeF(Retangulo.Width.ToPoint(), Retangulo.Height.ToPoint());
        }
    }
}
