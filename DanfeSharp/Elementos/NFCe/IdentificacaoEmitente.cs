﻿using DanfeSharp.Enumeracoes;
using DanfeSharp.Graphics;
using DanfeSharp.Modelo;
using org.pdfclown.documents.contents.xObjects;
using System;
using System.Drawing;

namespace DanfeSharp.Elementos.NFCe
{
    internal class IdentificacaoEmitente : ElementoBase
    {
        public DanfeViewModel ViewModel { get; private set; }
        public XObject Logo { get; set; }

        public IdentificacaoEmitente(Estilo estilo, DanfeViewModel viewModel) : base(estilo)
        {
            ViewModel = viewModel;
            Logo = null;
        }

        public override void Draw(Gfx gfx)
        {
            base.Draw(gfx);

            // 7.7.6 Conteúdo do Quadro Dados do Emitente
            // Deverá estar impresso em negrito.A razão social e/ ou nome fantasia deverá ter tamanho
            // mínimo de doze(12) pontos, ou 17 CPP e os demais dados do emitente, endereço,
            // município, CEP, fone / fax deverão ter tamanho mínimo de oito(8) pontos, ou 17 CPP.

            var rp = BoundingBox.InflatedRetangle(0.75F);
            float alturaMaximaLogoHorizontal = 14F;

            Fonte f2 = Estilo.CriarFonteNegrito(12);
            Fonte f3 = Estilo.CriarFonteRegular(8);

            if (Logo == null)
            {
                var f1 = Estilo.CriarFonteRegular(6);
                gfx.DrawString("IDENTIFICAÇÃO DO EMITENTE", rp, f1, AlinhamentoHorizontal.Centro);
                rp = rp.CutTop(f1.AlturaLinha);
            }
            else
            {
                RectangleF rLogo;

                if (Logo.Size.Width >= Logo.Size.Height)
                {
                    rLogo = new RectangleF(rp.X, rp.Y, rp.Width, alturaMaximaLogoHorizontal);
                    rp = rp.CutTop(alturaMaximaLogoHorizontal);
                }
                else
                {
                    float lw = rp.Height * Logo.Size.Width / Logo.Size.Height;
                    rLogo = new RectangleF(rp.X, rp.Y, lw, rp.Height);
                    rp = rp.CutLeft(lw);
                }

                gfx.ShowXObject(Logo, rLogo);
            }

            var emitente = ViewModel.Emitente;
            var nome = !String.IsNullOrWhiteSpace(emitente.NomeFantasia) ? emitente.NomeFantasia : emitente.RazaoSocial;
            var ts = new TextStack(rp) { LineHeightScale = 1 }
                .AddLine(nome, f2)
                .AddLine("CNPJ: " + Utils.FormatarCpfCnpj(emitente.CnpjCpf), f3)
                .AddLine("I.E.: " + emitente.Ie, f3)
                .AddLine(emitente.EnderecoLinha1.Trim(), f3)
                .AddLine(emitente.EnderecoLinha2.Trim(), f3)
                .AddLine(emitente.EnderecoLinha3.Trim(), f3);

            ts.AlinhamentoHorizontal = AlinhamentoHorizontal.Centro;
            ts.AlinhamentoVertical = AlinhamentoVertical.Centro;
            ts.Draw(gfx);
        }
    }
}
