using DanfeSharp.Blocos;
using DanfeSharp.Elementos;
using DanfeSharp.Enumeracoes;
using DanfeSharp.Modelo;
using org.pdfclown.documents;
using org.pdfclown.documents.contents.fonts;
using org.pdfclown.files;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace DanfeSharp
{
    public class Danfe : IDisposable
    {
        public DanfeViewModel ViewModel { get; private set; }
        public File File { get; private set; }
        internal Document PdfDocument { get; private set; }

        internal Blocos.NFe.BlocoCanhoto Canhoto { get; private set; }
        internal Blocos.NFe.BlocoIdentificacaoEmitente IdentificacaoEmitenteNFe { get; private set; }
        internal Blocos.NFCe.BlocoIdentificacaoEmitente IdentificacaoEmitenteNFCe { get; private set; }
        internal Blocos.CFeSat.BlocoIdentificacaoEmitente IdentificacaoEmitenteCFeSat { get; private set; }

        internal List<BlocoBase> _Blocos;
        internal Estilo EstiloPadrao { get; private set; }

        internal List<DanfePagina> Paginas { get; private set; }

        private StandardType1Font _FonteRegular;
        private StandardType1Font _FonteNegrito;
        private StandardType1Font _FonteItalico;
        private StandardType1Font.FamilyEnum _FonteFamilia;

        private Boolean _FoiGerado;

        private org.pdfclown.documents.contents.xObjects.XObject _LogoObject = null;
        private org.pdfclown.documents.contents.xObjects.XObject _QrCodeObject = null;

        public TipoFolha Folha { get; set; } = TipoFolha.A4;

        public Danfe(DanfeViewModel viewModel)
        {
            ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

            _Blocos = new List<BlocoBase>();
            File = new File();
            PdfDocument = File.Document;

            // De acordo com o item 7.7, a fonte deve ser Times New Roman ou Courier New.
            _FonteFamilia = StandardType1Font.FamilyEnum.Times;
            _FonteRegular = new StandardType1Font(PdfDocument, _FonteFamilia, false, false);
            _FonteNegrito = new StandardType1Font(PdfDocument, _FonteFamilia, true, false);
            _FonteItalico = new StandardType1Font(PdfDocument, _FonteFamilia, false, true);

            EstiloPadrao = CriarEstilo();

            Paginas = new List<DanfePagina>();

            if (viewModel.TipoLeituraDeXML == DanfeViewModel.enumTipoLeituraDeXML.NFe)
            {
                Canhoto = CriarBloco<Blocos.NFe.BlocoCanhoto>();
                IdentificacaoEmitenteNFe = AdicionarBloco<Blocos.NFe.BlocoIdentificacaoEmitente>();
                AdicionarBloco<Blocos.NFe.BlocoDestinatarioRemetente>();

                if (ViewModel.Duplicatas.Count > 0)
                    AdicionarBloco<Blocos.NFe.BlocoDuplicataFatura>();

                AdicionarBloco<Blocos.NFe.BlocoCalculoImposto>(ViewModel.Orientacao == Orientacao.Paisagem ? EstiloPadrao : CriarEstilo(4.75F));
                AdicionarBloco<Blocos.NFe.BlocoTransportador>();
                AdicionarBloco<Blocos.NFe.BlocoDadosAdicionais>(CriarEstilo(tFonteCampoConteudo: 8));

                if (ViewModel.CalculoIssqn.Mostrar)
                    AdicionarBloco<Blocos.NFe.BlocoCalculoIssqn>();
            }
            else if (viewModel.TipoLeituraDeXML == DanfeViewModel.enumTipoLeituraDeXML.NFCe)
            {
                IdentificacaoEmitenteNFCe = AdicionarBloco<Blocos.NFCe.BlocoIdentificacaoEmitente>();
                AdicionarBloco<Blocos.NFCe.BlocoDanfeTitulo>();
                AdicionarBloco<Blocos.NFCe.BlocoMensagemConsumidor>();
                AdicionarBloco<Blocos.NFCe.BlocoQRCode>();
                AdicionarBloco<Blocos.NFCe.BlocoDestinatarioRemetente>();
                AdicionarBloco<Blocos.NFCe.BlocoInfo>();
                AdicionarBloco<Blocos.NFCe.BlocoResumo>();
            }
            else if (viewModel.TipoLeituraDeXML == DanfeViewModel.enumTipoLeituraDeXML.CFeSat)
            {
                IdentificacaoEmitenteCFeSat = AdicionarBloco<Blocos.CFeSat.BlocoIdentificacaoEmitente>();
                AdicionarBloco<Blocos.CFeSat.BlocoDanfeTitulo>();
                AdicionarBloco<Blocos.CFeSat.BlocoMensagemConsumidor>();
                AdicionarBloco<Blocos.CFeSat.BlocoQRCode>();
                AdicionarBloco<Blocos.CFeSat.BlocoDestinatarioRemetente>();
                AdicionarBloco<Blocos.CFeSat.BlocoInfo>();
                AdicionarBloco<Blocos.CFeSat.BlocoResumo>();
            }

            GeraQrCOde();

            AdicionarMetadata();

            _FoiGerado = false;
        }

        public void AdicionarLogoImagem(System.IO.Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            var img = org.pdfclown.documents.contents.entities.Image.Get(stream);
            if (img == null) throw new InvalidOperationException("O logotipo não pode ser carregado, certifique-se que a imagem esteja no formato JPEG não progressivo.");
            _LogoObject = img.ToXObject(PdfDocument);
        }

        private void GeraQrCOde()
        {
            if (ViewModel.TipoLeituraDeXML == DanfeViewModel.enumTipoLeituraDeXML.NFCe ||
                 ViewModel.TipoLeituraDeXML == DanfeViewModel.enumTipoLeituraDeXML.CFeSat)
            {
                string strDemilitador = "|";

                StringBuilder textQRCode = new StringBuilder();
                textQRCode.Append(ViewModel.ChaveAcesso);
                textQRCode.Append(strDemilitador);

                if (ViewModel.DataHoraEmissao.HasValue)
                {
                    textQRCode.Append(ViewModel.DataHoraEmissao.Value.ToString("yyyyMMddHHmmss"));
                    textQRCode.Append(strDemilitador);
                }

                textQRCode.Append(ViewModel.CalculoImposto.ValorTotalNota);
                textQRCode.Append(strDemilitador);

                if (!string.IsNullOrEmpty(ViewModel.Destinatario.CnpjCpf))
                {
                    textQRCode.Append(strDemilitador);
                    textQRCode.Append(ViewModel.Destinatario.CnpjCpf);
                }

                //textQRCode.Append(ViewModel.AssinaturaQRCode);
                textQRCode.Append("sdfsdfsdf");

                var bw = new ZXing.BarcodeWriter();
                var encOptions = new ZXing.Common.EncodingOptions() { Width = 300, Height = 300, Margin = 0 };
                bw.Options = encOptions;
                bw.Format = ZXing.BarcodeFormat.QR_CODE;

                var imageQrCode = bw.Write(textQRCode.ToString());

                using (System.IO.MemoryStream m = new System.IO.MemoryStream())
                {
                    imageQrCode.Save(m, System.Drawing.Imaging.ImageFormat.Jpeg);

                    var msData = new System.IO.MemoryStream(m.ToArray(), false);

                    org.pdfclown.documents.contents.entities.Image image = org.pdfclown.documents.contents.entities.Image.Get(msData);

                    _QrCodeObject = image.ToXObject(PdfDocument);
                }

                Type typeClassNFCe = typeof(Blocos.NFCe.BlocoQRCode);
                Type typeClassCFeSat = typeof(Blocos.CFeSat.BlocoQRCode);

                foreach (var item in _Blocos)
                {
                    Type typeItemList = item.GetType();

                    if (typeItemList.FullName == typeClassNFCe.FullName)
                    {
                        ((Blocos.NFCe.BlocoQRCode)item).QrCode = _QrCodeObject;
                    }

                    if (typeItemList.FullName == typeClassCFeSat.FullName)
                    {
                        ((Blocos.CFeSat.BlocoQRCode)item).QrCode = _QrCodeObject;
                    }
                }
            }
        }

        public void AdicionarLogoImagem(byte[] byteArray)
        {
            if (byteArray == null) throw new ArgumentNullException(nameof(byteArray));

            System.IO.Stream stream = new System.IO.MemoryStream(byteArray);

            var img = org.pdfclown.documents.contents.entities.Image.Get(stream);
            if (img == null) throw new InvalidOperationException("O logotipo não pode ser carregado, certifique-se que a imagem esteja no formato JPEG não progressivo.");
            _LogoObject = img.ToXObject(PdfDocument);
        }

        public void AdicionarLogoPdf(System.IO.Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            using (var pdfFile = new org.pdfclown.files.File(new org.pdfclown.bytes.Stream(stream)))
            {
                _LogoObject = pdfFile.Document.Pages[0].ToXObject(PdfDocument);
            }
        }

        public void AdicionarLogoImagem(String path)
        {
            if (String.IsNullOrWhiteSpace(path)) throw new ArgumentException(nameof(path));

            using (var fs = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                AdicionarLogoImagem(fs);
            }
        }

        public void AdicionarLogoPdf(String path)
        {
            if (String.IsNullOrWhiteSpace(path)) throw new ArgumentException(nameof(path));

            using (var fs = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                AdicionarLogoPdf(fs);
            }
        }

        private void AdicionarMetadata()
        {
            var info = PdfDocument.Information;
            info[new org.pdfclown.objects.PdfName("ChaveAcesso")] = ViewModel.ChaveAcesso;
            info[new org.pdfclown.objects.PdfName("TipoDocumento")] = "DANFE";
            info.CreationDate = DateTime.Now;
            info.Creator = String.Format("{0} {1} - {2}", "DanfeSharp", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version, "https://github.com/SilverCard/DanfeSharp");
            info.Title = "DANFE (Documento auxiliar da NFe)";
        }

        private Estilo CriarEstilo(float tFonteCampoCabecalho = 6, float tFonteCampoConteudo = 10)
        {
            return new Estilo(_FonteRegular, _FonteNegrito, _FonteItalico, tFonteCampoCabecalho, tFonteCampoConteudo);
        }

        public void Gerar()
        {
            if (_FoiGerado) throw new InvalidOperationException("O Danfe já foi gerado.");

            if (IdentificacaoEmitenteNFe != null)
                IdentificacaoEmitenteNFe.Logo = _LogoObject;

            if (IdentificacaoEmitenteNFCe != null)
                IdentificacaoEmitenteNFCe.Logo = _LogoObject;

            if (IdentificacaoEmitenteCFeSat != null)
                IdentificacaoEmitenteCFeSat.Logo = _LogoObject;

            var tabela = new TabelaProdutosServicos(this.Folha, ViewModel, EstiloPadrao);

            while (true)
            {
                if (Folha == TipoFolha.CupomTermico80mm)
                {
                    //Cupom Térmico de 80mm

                    DanfePagina p = CriarPaginaCupom80mm(tabela);

                    #region Desenha o Cabeçalho

                    float height = 0;
                    for (int i = 0; i < 2; i++)
                    {
                        p.DesenhaBloco(_Blocos[i], p.RetanguloDesenhavel.Location.X, p.RetanguloDesenhavel.Location.Y + height);
                        height += _Blocos[i].Height;
                    }

                    #endregion

                    #region Desenha a Tabela

                    var alturaTabela = tabela.AlturaTabela(p.RetanguloDesenhavel.Location, p.RetanguloDesenhavel.Size.Width);

                    tabela.SetPosition(p.RetanguloDesenhavel.Location.X, p.RetanguloDesenhavel.Location.Y + height - 3);

                    var size = p.RetanguloDesenhavel.Size;
                    size.Height = alturaTabela;
                    tabela.SetSize(size);
                    tabela.Draw(p.Gfx);

                    height += alturaTabela;

                    #endregion

                    #region Desenha os Demais Blocos

                    for (int i = _Blocos.Count - 1; i >= 2; i--)
                    {
                        p.DesenhaBloco(_Blocos[i], p.RetanguloDesenhavel.Location.X, p.RetanguloDesenhavel.Location.Y + height - 3);
                        height += _Blocos[i].Height;
                    }

                    #endregion

                    p.Gfx.Stroke();
                    p.Gfx.Flush();

                    if (tabela.CompletamenteDesenhada) break;
                }
                else
                {
                    DanfePagina p = CriarPagina();

                    //Demais Tipos de Folhas A4 e A5

                    tabela.SetPosition(p.RetanguloCorpo.Location);
                    tabela.SetSize(p.RetanguloCorpo.Size);
                    tabela.Draw(p.Gfx);

                    p.Gfx.Stroke();
                    p.Gfx.Flush();

                    if (tabela.CompletamenteDesenhada) break;
                }
            }

            PreencherNumeroFolhas();
            _FoiGerado = true;
        }

        private DanfePagina CriarPagina()
        {
            DanfePagina p = new DanfePagina(this);
            Paginas.Add(p);
            p.DesenharBlocos(Paginas.Count == 1);
            p.DesenharCreditos();

            // Ambiente de homologação
            // 7. O DANFE emitido para representar NF-e cujo uso foi autorizado em ambiente de
            // homologação sempre deverá conter a frase “SEM VALOR FISCAL” no quadro “Informações
            // Complementares” ou em marca d’água destacada.
            if (ViewModel.TipoAmbiente == 2)
                p.DesenharAvisoHomologacao();

            return p;
        }

        private DanfePagina CriarPaginaCupom80mm(TabelaProdutosServicos tabela)
        {
            DanfePagina p = new DanfePagina(this);

            var alturaTabela = tabela.AlturaTabela(p.RetanguloDesenhavel.Location, p.RetanguloDesenhavel.Size.Width);
            p.RedefineRetanguloPagina(alturaTabela + 10);

            Paginas.Add(p);
            //p.DesenharBlocos(Paginas.Count == 1); // Não pode chamar esse método agora pois fará com que os blocos saiam de ordem
            p.DesenharCreditos();

            // Ambiente de homologação
            // 7. O DANFE emitido para representar NF-e cujo uso foi autorizado em ambiente de
            // homologação sempre deverá conter a frase “SEM VALOR FISCAL” no quadro “Informações
            // Complementares” ou em marca d’água destacada.
            if (ViewModel.TipoLeituraDeXML == DanfeViewModel.enumTipoLeituraDeXML.NFe && ViewModel.TipoAmbiente == 2)
                p.DesenharAvisoHomologacao();

            return p;
        }

        internal T CriarBloco<T>() where T : BlocoBase
        {
            return (T)Activator.CreateInstance(typeof(T), ViewModel, EstiloPadrao);
        }

        internal T CriarBloco<T>(Estilo estilo) where T : BlocoBase
        {
            return (T)Activator.CreateInstance(typeof(T), ViewModel, estilo);
        }

        internal T AdicionarBloco<T>() where T : BlocoBase
        {
            var bloco = CriarBloco<T>();
            _Blocos.Add(bloco);
            return bloco;
        }

        internal T AdicionarBloco<T>(Estilo estilo) where T : BlocoBase
        {
            var bloco = CriarBloco<T>(estilo);
            _Blocos.Add(bloco);
            return bloco;
        }

        internal void AdicionarBloco(BlocoBase bloco)
        {
            _Blocos.Add(bloco);
        }

        internal void PreencherNumeroFolhas()
        {
            if (this.ViewModel.TipoLeituraDeXML == DanfeViewModel.enumTipoLeituraDeXML.NFe)
            {
                int nFolhas = Paginas.Count;
                for (int i = 0; i < Paginas.Count; i++)
                {
                    Paginas[i].DesenhaNumeroPaginas(i + 1, nFolhas);
                }
            }
        }

        public void Salvar(String path)
        {
            if (String.IsNullOrWhiteSpace(path)) throw new ArgumentException(nameof(path));

            File.Save(path, SerializationModeEnum.Incremental);
        }

        public void Salvar(System.IO.Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            File.Save(new org.pdfclown.bytes.Stream(stream), SerializationModeEnum.Incremental);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    File.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Danfe() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
