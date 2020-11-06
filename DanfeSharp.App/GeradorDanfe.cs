using System;
using System.Windows;
using DanfeSharp.Enumeracoes;
using DanfeSharp.Modelo;

namespace DanfeSharp.App
{
    public static class GeradorDanfe
    {
        public static void GerarDanfe(String xmlPath, String logoPath, Orientacao orientacao = Orientacao.Retrato, TipoFolha tipoFolha = TipoFolha.A4)
        {
            try
            {
                DanfeViewModel model = DanfeViewModel.CreateFromXmlFile(xmlPath);
                
                //model.Margem = 2;

                using (Danfe danfe = new Danfe(model))
                {
                    model.Orientacao = orientacao;

                    if (!string.IsNullOrEmpty(logoPath))
                        danfe.AdicionarLogoImagem(logoPath);

                    danfe.Folha = tipoFolha;

                    danfe.Gerar();

                    String outFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(xmlPath), model.ChaveAcesso + ".pdf");
                    danfe.Salvar(outFile);

                    //Abre o PDF
                    var process = System.Diagnostics.Process.Start(outFile);
                    if (process == null)
                    {
                        MessageBox.Show(String.Format("Não foi possível abrir o DANFE gerado.\nEle foi gravado em: {0}", outFile), "DanfeSharp", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Erro ao gerar o DANFE", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
