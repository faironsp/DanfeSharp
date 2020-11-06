using System;
using System.Collections.Generic;
using System.Drawing;
using DanfeSharp.Elementos;
using DanfeSharp.Enumeracoes;
using DanfeSharp.Graphics;
using DanfeSharp.Modelo;

namespace DanfeSharp.Blocos
{
    internal class TabelaProdutosServicos : ElementoBase
    {
        public CabecalhoBloco CabecalhoBloco { get; private set; }
        public Tabela Tabela { get; private set; }
        public DanfeViewModel ViewModel { get; private set; }

        public TabelaProdutosServicos(TipoFolha tipoFolha, DanfeViewModel viewModel, Estilo estilo) : base(estilo)
        {
            ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

            string title = string.Empty;
            if (viewModel.TipoLeituraDeXML == DanfeViewModel.enumTipoLeituraDeXML.NFe) title = "DADOS DOS PRODUTOS / SERVIÇOS";
            CabecalhoBloco = new CabecalhoBloco(estilo, title);

            var ad = AlinhamentoHorizontal.Direita;
            var ac = AlinhamentoHorizontal.Centro;
            var ae = AlinhamentoHorizontal.Esquerda;

            Tabela = new Tabela(Estilo);

            if (ViewModel.TipoLeituraDeXML == DanfeViewModel.enumTipoLeituraDeXML.NFe)
            {
                String cabecalho4 = ViewModel.Emitente.CRT == "3" ? "O/CST" : "O/CSOSN";

                if (ViewModel.IsRetrato)
                {
                    Tabela
                    .ComColuna(8.5f, ac, "CÓDIGO", "PRODUTO")
                    .ComColuna(0, ae, "DESCRIÇÃO DO PRODUTO / SERVIÇO")
                    .ComColuna(5.6F, ac, "NCM/SH")
                    .ComColuna(3.9F, ac, cabecalho4)
                    .ComColuna(3.5F, ac, "CFOP")
                    .ComColuna(3.25F, ac, "UN")
                    .ComColuna(6F, ad, "QUANTI.")
                    .ComColuna(6F, ad, "VALOR", "UNIT.")
                    .ComColuna(6F, ad, "VALOR", "TOTAL")
                    .ComColuna(6F, ad, "B CÁLC", "ICMS")
                    .ComColuna(5, ad, "VALOR", "ICMS")
                    .ComColuna(5, ad, "VALOR", "IPI")
                    .ComColuna(3.5F, ad, "ALIQ.", "ICMS")
                    .ComColuna(3.5F, ad, "ALIQ.", "IPI");
                }
                else
                {
                    Tabela
                    .ComColuna(8.1f, ac, "CÓDIGO PRODUTO")
                    .ComColuna(0, ae, "DESCRIÇÃO DO PRODUTO / SERVIÇO")
                    .ComColuna(5.5F, ac, "NCM/SH")
                    .ComColuna(3.1F, ac, cabecalho4)
                    .ComColuna(3.1F, ac, "CFOP")
                    .ComColuna(3F, ac, "UN")
                    .ComColuna(5.25F, ad, "QUANTI.")
                    .ComColuna(5.6F, ad, "VALOR UNIT.")
                    .ComColuna(5.6F, ad, "VALOR TOTAL")
                    .ComColuna(5.6F, ad, "B CÁLC ICMS")
                    .ComColuna(5.6F, ad, "VALOR ICMS")
                    .ComColuna(5.6F, ad, "VALOR IPI")
                    .ComColuna(3F, ad, "ALIQ.", "ICMS")
                    .ComColuna(3F, ad, "ALIQ.", "IPI");
                }

                Tabela.AjustarLarguraColunas();

                foreach (var p in ViewModel.Produtos)
                {
                    var linha = new List<String>
                {
                    p.Codigo,
                    p.DescricaoCompleta,
                    p.Ncm,
                    p.OCst,
                    p.Cfop.Formatar("N0"),
                    p.Unidade,
                    p.Quantidade.Formatar(),
                    p.ValorUnitario.Formatar(),
                    p.ValorTotal.Formatar(),
                    p.BaseIcms.Formatar(),
                    p.ValorIcms.Formatar(),
                    p.ValorIpi.Formatar(),
                    p.AliquotaIcms.Formatar(),
                    p.AliquotaIpi.Formatar()
                };

                    Tabela.AdicionarLinha(linha);
                }
            }
            else if (ViewModel.TipoLeituraDeXML == DanfeViewModel.enumTipoLeituraDeXML.NFCe)
            {
                if (tipoFolha == TipoFolha.CupomTermico80mm)
                {
                    //Cupom Térmico de 80mm

                    Tabela
                    .ComColuna(4.5f, ac, "#")
                    .ComColuna(8.5f, ac, "CÓD")
                    .ComColuna(0, ae, "DESCRIÇÃO")
                    .ComColuna(10F, ac, "UN")
                    .ComColuna(12F, ad, "QTD")
                    .ComColuna(12F, ad, "VALOR", "UNIT.")
                    .ComColuna(12F, ad, "VALOR", "TOTAL");
                }
                else
                {
                    //Demais Tipos de Folhas A4 e A5

                    Tabela
                    .ComColuna(4.5f, ac, "ITEM")
                    .ComColuna(8.5f, ac, "CÓDIGO", "PRODUTO")
                    .ComColuna(0, ae, "DESCRIÇÃO DO PRODUTO / SERVIÇO")
                    .ComColuna(5F, ac, "UN")
                    .ComColuna(6F, ad, "QTD")
                    .ComColuna(6F, ad, "VALOR", "UNIT.")
                    .ComColuna(6F, ad, "VALOR", "TOTAL");
                }

                Tabela.AjustarLarguraColunas();

                int i = 1;
                foreach (var p in ViewModel.Produtos)
                {
                    var linha = new List<String>
                    {
                        i.ToString(),
                        p.Codigo,
                        p.DescricaoCompleta,
                        p.Unidade,
                        p.Quantidade.Formatar(),
                        p.ValorUnitario.Formatar(),
                        p.ValorTotal.Formatar()
                    };

                    Tabela.AdicionarLinha(linha);

                    i++;
                }
            }
            else if (ViewModel.TipoLeituraDeXML == DanfeViewModel.enumTipoLeituraDeXML.CFeSat)
            {
                if (tipoFolha == TipoFolha.CupomTermico80mm)
                {
                    //Cupom Térmico de 80mm

                    Tabela
                    .ComColuna(4.5f, ac, "#")
                    .ComColuna(8.5f, ac, "CÓD")
                    .ComColuna(0, ae, "DESCRIÇÃO")
                    .ComColuna(10F, ac, "UN")
                    .ComColuna(12F, ad, "QTD")
                    .ComColuna(12F, ad, "VALOR", "UNIT.")
                    .ComColuna(12F, ad, "VALOR", "TOTAL");
                }
                else
                {
                    //Demais Tipos de Folhas A4 e A5

                    Tabela
                    .ComColuna(4.5f, ac, "ITEM")
                    .ComColuna(8.5f, ac, "CÓDIGO", "PRODUTO")
                    .ComColuna(0, ae, "DESCRIÇÃO DO PRODUTO / SERVIÇO")
                    .ComColuna(5F, ac, "UN")
                    .ComColuna(6F, ad, "QTD")
                    .ComColuna(6F, ad, "VALOR", "UNIT.")
                    .ComColuna(6F, ad, "VALOR", "TOTAL");
                }

                Tabela.AjustarLarguraColunas();

                int i = 1;
                foreach (var p in ViewModel.Produtos)
                {
                    var linha = new List<String>
                    {
                        i.ToString(),
                        p.Codigo,
                        p.DescricaoCompleta,
                        p.Unidade,
                        p.Quantidade.Formatar(),
                        p.ValorUnitario.Formatar(),
                        p.ValorTotal.Formatar()
                    };

                    Tabela.AdicionarLinha(linha);

                    i++;
                }
            }
        }

        public override void Draw(Gfx gfx)
        {
            base.Draw(gfx);

            Tabela.SetPosition(RetanguloTabela.Location);
            Tabela.SetSize(RetanguloTabela.Size);
            Tabela.Draw(gfx);

            CabecalhoBloco.SetPosition(X, Y);
            CabecalhoBloco.Width = Width;
            CabecalhoBloco.Draw(gfx);
        }

        public float AlturaTabela(PointF p, float width)
        {
            return Tabela.AlturaTabela(p, width);
        }

        public RectangleF RetanguloTabela => BoundingBox.CutTop(CabecalhoBloco.Height);
        public Boolean CompletamenteDesenhada => Tabela.LinhaAtual == ViewModel.Produtos.Count;
        public override bool PossuiContono => false;
    }
}
