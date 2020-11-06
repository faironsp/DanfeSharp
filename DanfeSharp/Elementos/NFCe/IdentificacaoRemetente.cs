using DanfeSharp.Enumeracoes;
using DanfeSharp.Graphics;
using DanfeSharp.Modelo;
using System;

namespace DanfeSharp.Elementos.NFCe
{
    internal class IdentificacaoRemetente : ElementoBase
    {
        public DanfeViewModel ViewModel { get; private set; }

        public IdentificacaoRemetente(Estilo estilo, DanfeViewModel viewModel) : base(estilo)
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

            if (!string.IsNullOrEmpty(ViewModel.Destinatario.CnpjCpf))
            {
                //Consumidor Identificado
                var destinatario = ViewModel.Destinatario;
                var nome = !String.IsNullOrWhiteSpace(destinatario.NomeFantasia) ? destinatario.NomeFantasia : destinatario.RazaoSocial;
                var ts = new TextStack(rp) { LineHeightScale = 1 }
                    .AddLine("CONSUMIDOR", f1)
                    .AddLine("", f2);

                if (!String.IsNullOrWhiteSpace(nome))
                {
                    ts.AddLine(nome, f2);
                    ts.AddLine("", f2);
                }

                ts.AddLine("CPF/CNPJ", f2);
                ts.AddLine(destinatario.CnpjCpf, f3);

                ts.AlinhamentoHorizontal = AlinhamentoHorizontal.Centro;
                ts.AlinhamentoVertical = AlinhamentoVertical.Centro;
                ts.Draw(gfx);
            }
            else
            {
                //Consumidor Não Identificado

                var ts = new TextStack(rp) { LineHeightScale = 1 }
                    .AddLine("CONSUMIDOR", f1)
                    .AddLine("", f2);

                ts.AddLine("CONSUMIDOR NÃO IDENTIFICADO", f3);

                ts.AlinhamentoHorizontal = AlinhamentoHorizontal.Centro;
                ts.AlinhamentoVertical = AlinhamentoVertical.Centro;
                ts.Draw(gfx);
            }
        }
    }
}
