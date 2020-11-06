using DanfeSharp.Enumeracoes;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DanfeSharp.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Title = String.Format("{0} v{1}", "DanfeSharp", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
        }

        internal void ProcurarArquivo(TextBox tb, String filter, String title)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = filter;
            dlg.Title = title;

            if (dlg.ShowDialog() == true)
            {
                tb.Text = dlg.FileName;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ProcurarArquivo(PathXml, "NFe Processada (*.xml)|*.xml", "Abrir NFe");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ProcurarArquivo(PathLogo, "Logo (*.jpg, *.pdf)|*.jpg;*.pdf", "Abrir Logo");
        }

        private void BGerarDanfe_Click(object sender, RoutedEventArgs e)
        {
            var orientacao = Orientacao.Retrato;

            if (rbOrientacaoDaImpressaoPaisagem.IsChecked.Value) orientacao = Orientacao.Paisagem;


            var tipoFolha = TipoFolha.A4;

            if (rbFolhaA5.IsChecked.Value)
            {
                tipoFolha = TipoFolha.A5;
            }
            else if (rbFolha88mm.IsChecked.Value)
            {
                tipoFolha = TipoFolha.CupomTermico80mm;
            }

            GeradorDanfe.GerarDanfe(PathXml.Text, PathLogo.Text, orientacao, tipoFolha);
        }

        private void SetPathFromFiles(string[] files)
        {
            string pathXml = files.FirstOrDefault(x => x.EndsWith(".xml", StringComparison.InvariantCultureIgnoreCase));
            string pathLogo = files.FirstOrDefault(x => x.EndsWith(".jpg", StringComparison.InvariantCultureIgnoreCase) || x.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase));

            if (!String.IsNullOrWhiteSpace(pathXml))
                PathXml.Text = pathXml;

            if (!String.IsNullOrWhiteSpace(pathLogo))
                PathLogo.Text = pathLogo;
        }

        private void Event_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                SetPathFromFiles(files);
            }
        }
    }
}
