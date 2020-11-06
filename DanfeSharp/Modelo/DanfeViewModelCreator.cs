using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using DanfeSharp.Enumeracoes;
using DanfeSharp.Esquemas.CFe;
using DanfeSharp.Esquemas.NFe;

namespace DanfeSharp.Modelo
{
    public static class DanfeViewModelCreator
    {
        private static EmpresaViewModel CreateEmpresaFrom(Empresa empresa)
        {
            EmpresaViewModel model = new EmpresaViewModel();

            if (empresa != null)
            {
                model.RazaoSocial = empresa.xNome;
                model.CnpjCpf = !String.IsNullOrWhiteSpace(empresa.CNPJ) ? empresa.CNPJ : empresa.CPF;
                model.Ie = empresa.IE;
                model.IeSt = empresa.IEST;

                var end = empresa.Endereco;

                if (end != null)
                {
                    model.EnderecoLogadrouro = end.xLgr;
                    model.EnderecoNumero = end.nro;
                    model.EnderecoBairro = end.xBairro;
                    model.Municipio = end.xMun;
                    model.EnderecoUf = end.UF;
                    model.EnderecoCep = end.CEP;
                    model.Telefone = end.fone;
                    model.Email = empresa.email;
                }

                if (empresa is Emitente)
                {
                    var emit = empresa as Emitente;
                    model.IM = emit.IM;
                    model.CRT = emit.CRT;
                    model.NomeFantasia = emit.xFant;
                }
            }

            return model;
        }

        /// <summary>
        /// Cria o modelo a partir de um arquivo xml.
        /// </summary>
        /// <param name="caminho"></param>
        /// <returns></returns>
        public static DanfeViewModel CriarDeArquivoXml(string caminho)
        {
            using (StreamReader reader = new StreamReader(caminho, true))
            {
                var str = reader.ReadToEnd();

                return CriarDeArquivoXmlInternal(str);
            }
        }

        /// <summary>
        /// Cria o modelo a partir de um arquivo xml contido num stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>Modelo</returns>
        public static DanfeViewModel CriarDeArquivoXml(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            using (StreamReader reader = new StreamReader(stream, true))
            {
                var str = reader.ReadToEnd();

                return CriarDeArquivoXmlInternal(str);
            }
        }

        /// <summary>
        /// Cria o modelo a partir de uma string xml.
        /// </summary>
        public static DanfeViewModel CriarDeStringXml(string str)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));

            return CriarDeArquivoXmlInternal(str);
        }

        private static DanfeViewModel CriarDeArquivoXmlInternal(string str)
        {
            ProcNFe nfe = null;
            CFe cfe = null;

            XmlSerializer serializerNFe = new XmlSerializer(typeof(ProcNFe));
            XmlSerializer serializerCFe = new XmlSerializer(typeof(CFe));

            try
            {
                using (StringReader reader = new StringReader(str))
                {
                    //Primeiro: tentar ler o XML "achando" que é um ProcNFe
                    nfe = (ProcNFe)serializerNFe.Deserialize(reader);
                    return CreateFromXml(nfe);
                }
            }
            catch (Exception exProcNFe)
            {
                try
                {
                    using (StringReader reader = new StringReader(str))
                    {
                        //Segundo: tentar ler o XML "achando" que é um CFe (SAT)
                        cfe = (CFe)serializerCFe.Deserialize(reader);
                        return CreateFromXml(cfe);
                    }
                }
                catch (Exception exCFe)
                {
                    throw new Exception("Não foi possível ler o arquivo XML");
                }
            }
        }

        internal static void ExtrairDatas(DanfeViewModel model, InfNFe infNfe)
        {
            var ide = infNfe.ide;

            if (infNfe.Versao.Maior >= 3)
            {
                if (ide.dhEmi.HasValue) model.DataHoraEmissao = ide.dhEmi.Value.DateTimeOffsetValue.DateTime;
                if (ide.dhSaiEnt.HasValue) model.DataSaidaEntrada = ide.dhSaiEnt.Value.DateTimeOffsetValue.DateTime;

                if (model.DataSaidaEntrada.HasValue)
                {
                    model.HoraSaidaEntrada = model.DataSaidaEntrada.Value.TimeOfDay;
                }
            }
            else
            {
                model.DataHoraEmissao = ide.dEmi;
                model.DataSaidaEntrada = ide.dSaiEnt;

                if (!String.IsNullOrWhiteSpace(ide.hSaiEnt))
                {
                    model.HoraSaidaEntrada = TimeSpan.Parse(ide.hSaiEnt);
                }

            }
        }

        internal static CalculoImpostoViewModel CriarCalculoImpostoViewModel(ICMSTotal i)
        {
            return new CalculoImpostoViewModel()
            {
                ValorAproximadoTributos = i.vTotTrib,
                BaseCalculoIcms = i.vBC,
                ValorIcms = i.vICMS,
                BaseCalculoIcmsSt = i.vBCST,
                ValorIcmsSt = i.vST,
                ValorTotalProdutos = i.vProd,
                ValorFrete = i.vFrete,
                ValorSeguro = i.vSeg,
                Desconto = i.vDesc,
                ValorII = i.vII,
                ValorIpi = i.vIPI,
                ValorPis = i.vPIS,
                ValorCofins = i.vCOFINS,
                OutrasDespesas = i.vOutro,
                ValorTotalNota = i.vNF,
                vFCPUFDest = i.vFCPUFDest,
                vICMSUFDest = i.vICMSUFDest,
                vICMSUFRemet = i.vICMSUFRemet
            };
        }

        public static DanfeViewModel CreateFromXml(object xmlObj)
        {
            DanfeViewModel model = new DanfeViewModel();

            if (xmlObj is ProcNFe)
            {
                var procNfe = (ProcNFe)xmlObj;

                var nfe = procNfe.NFe;
                var infNfe = nfe.infNFe;
                var ide = infNfe.ide;

                if (ide.tpEmis != FormaEmissao.Normal)
                {
                    throw new Exception("Somente o tpEmis==1 está implementado.");
                }

                if (procNfe.NFe.infNFe.ide.mod == 55)
                {
                    model.TipoLeituraDeXML = DanfeViewModel.enumTipoLeituraDeXML.NFe;
                }
                else if (procNfe.NFe.infNFe.ide.mod == 65)
                {
                    model.TipoLeituraDeXML = DanfeViewModel.enumTipoLeituraDeXML.NFCe;
                }

                if (ide.tpImp == 1)
                    model.Orientacao = Orientacao.Retrato;
                else if (ide.tpImp == 2)
                    model.Orientacao = Orientacao.Paisagem;

                var infProt = procNfe.protNFe.infProt;
                model.CodigoStatusReposta = infProt.cStat;
                model.DescricaoStatusReposta = infProt.xMotivo;

                model.TipoAmbiente = (int)ide.tpAmb;
                model.NfNumero = ide.nNF;
                model.NfSerie = ide.serie;
                model.NaturezaOperacao = ide.natOp;
                model.ChaveAcesso = procNfe.NFe.infNFe.Id.Substring(3);
                model.TipoNF = (int)ide.tpNF;

                model.Emitente = CreateEmpresaFrom(infNfe.emit);
                model.Destinatario = CreateEmpresaFrom(infNfe.dest);

                model.NotasFiscaisReferenciadas = ide.NFref.Select(x => x.ToString()).ToList();

                // Informações adicionais de compra
                if (infNfe.compra != null)
                {
                    model.Contrato = infNfe.compra.xCont;
                    model.NotaEmpenho = infNfe.compra.xNEmp;
                    model.Pedido = infNfe.compra.xPed;
                }

                foreach (var det in infNfe.det)
                {
                    ProdutoViewModel produto = new ProdutoViewModel();
                    produto.Codigo = det.prod.cProd;
                    produto.Descricao = det.prod.xProd;
                    produto.Ncm = det.prod.NCM;
                    produto.Cfop = det.prod.CFOP;
                    produto.Unidade = det.prod.uCom;
                    produto.Quantidade = det.prod.qCom;
                    produto.ValorUnitario = det.prod.vUnCom;
                    produto.ValorTotal = det.prod.vProd;
                    produto.InformacoesAdicionais = det.infAdProd;

                    var imposto = det.imposto;

                    if (imposto != null)
                    {
                        if (imposto.ICMS != null)
                        {
                            var icms = imposto.ICMS.ICMS;

                            if (icms != null)
                            {
                                produto.ValorIcms = icms.vICMS;
                                produto.BaseIcms = icms.vBC;
                                produto.AliquotaIcms = icms.pICMS;
                                produto.OCst = icms.orig + icms.CST + icms.CSOSN;
                            }
                        }

                        if (imposto.IPI != null)
                        {
                            var ipi = imposto.IPI.IPITrib;

                            if (ipi != null)
                            {
                                produto.ValorIpi = ipi.vIPI;
                                produto.AliquotaIpi = ipi.pIPI;
                            }
                        }
                    }

                    model.Produtos.Add(produto);
                }

                if (infNfe.cobr != null)
                {
                    if (infNfe.cobr.dup != null)
                    {
                        foreach (var item in infNfe.cobr.dup)
                        {
                            DuplicataViewModel duplicata = new DuplicataViewModel();

                            duplicata.Numero = item.nDup;
                            duplicata.Valor = item.vDup;
                            duplicata.Vecimento = item.dVenc;

                            model.Duplicatas.Add(duplicata);
                        }
                    }
                }

                if (infNfe.pag != null)
                {
                    if (infNfe.pag.detPag != null)
                    {
                        foreach (var item in infNfe.pag.detPag)
                        {
                            DetalhePagamentoViewModel detpag = new DetalhePagamentoViewModel();

                            detpag.indPag = item.indPag;
                            detpag.tPag = item.tPag;
                            detpag.vPag = item.vPag;

                            model.Pagamentos.Add(detpag);
                        }
                    }
                }

                model.CalculoImposto = CriarCalculoImpostoViewModel(infNfe.total.ICMSTot);

                var issqnTotal = infNfe.total.ISSQNtot;

                if (issqnTotal != null)
                {
                    var c = model.CalculoIssqn;
                    c.InscricaoMunicipal = infNfe.emit.IM;
                    c.BaseIssqn = issqnTotal.vBC;
                    c.ValorTotalServicos = issqnTotal.vServ;
                    c.ValorIssqn = issqnTotal.vISS;
                    c.Mostrar = true;
                }

                var transp = infNfe.transp;
                var transportadora = transp.transporta;
                var transportadoraModel = model.Transportadora;

                transportadoraModel.ModalidadeFrete = (int)transp.modFrete;

                if (transp.veicTransp != null)
                {
                    transportadoraModel.VeiculoUf = transp.veicTransp.UF;
                    transportadoraModel.CodigoAntt = transp.veicTransp.RNTC;
                    transportadoraModel.Placa = transp.veicTransp.placa;
                }

                if (transportadora != null)
                {
                    transportadoraModel.RazaoSocial = transportadora.xNome;
                    transportadoraModel.EnderecoUf = transportadora.UF;
                    transportadoraModel.CnpjCpf = !String.IsNullOrWhiteSpace(transportadora.CNPJ) ? transportadora.CNPJ : transportadora.CPF;
                    transportadoraModel.EnderecoLogadrouro = transportadora.xEnder;
                    transportadoraModel.Municipio = transportadora.xMun;
                    transportadoraModel.Ie = transportadora.IE;
                }

                var vol = transp.vol.FirstOrDefault();

                if (vol != null)
                {
                    transportadoraModel.QuantidadeVolumes = vol.qVol;
                    transportadoraModel.Especie = vol.esp;
                    transportadoraModel.Marca = vol.marca;
                    transportadoraModel.Numeracao = vol.nVol;
                    transportadoraModel.PesoBruto = vol.pesoB;
                    transportadoraModel.PesoLiquido = vol.pesoL;
                }

                var infAdic = infNfe.infAdic;
                if (infAdic != null)
                {
                    model.InformacoesComplementares = procNfe.NFe.infNFe.infAdic.infCpl;
                    model.InformacoesAdicionaisFisco = procNfe.NFe.infNFe.infAdic.infAdFisco;
                }

                var infoProto = procNfe.protNFe.infProt;

                model.ProtocoloAutorizacao = String.Format(Formatador.Cultura, "{0} - {1}", infoProto.nProt, infoProto.dhRecbto.DateTimeOffsetValue.DateTime);

                ExtrairDatas(model, infNfe);
            }
            else if (xmlObj is CFe)
            {
                var cfe = (CFe)xmlObj;

                //var nfe = cfe.NFe;
                var infCfe = cfe.infCFe;
                var ide = infCfe.ide;

                model.TipoLeituraDeXML = DanfeViewModel.enumTipoLeituraDeXML.CFeSat;

                model.Orientacao = Orientacao.Retrato;

                model.TipoAmbiente = Convert.ToInt16(ide.tpAmb);
                model.NfNumero = Convert.ToInt16(ide.nCFe);

                model.ChaveAcesso = cfe.infCFe.Id.Substring(3);
                model.TipoNF = 1; //Saída

                if (infCfe.emit != null)
                {
                    EmpresaViewModel emitViewModel = new EmpresaViewModel();

                    emitViewModel.RazaoSocial = infCfe.emit.xNome;
                    emitViewModel.CnpjCpf = infCfe.emit.CNPJ;
                    emitViewModel.Ie = infCfe.emit.IE;

                    var end = infCfe.emit.enderEmit;

                    if (end != null)
                    {
                        emitViewModel.EnderecoLogadrouro = end.xLgr;
                        emitViewModel.EnderecoNumero = end.nro;
                        emitViewModel.EnderecoBairro = end.xBairro;
                        emitViewModel.Municipio = end.xMun;
                        emitViewModel.EnderecoCep = end.CEP;
                    }

                    emitViewModel.IM = infCfe.emit.IM;
                    emitViewModel.CRT = infCfe.emit.cRegTrib;
                    emitViewModel.NomeFantasia = infCfe.emit.xFant;

                    model.Emitente = emitViewModel;
                }

                if (infCfe.dest != null)
                {
                    EmpresaViewModel destViewModel = new EmpresaViewModel();

                    destViewModel.NomeFantasia = infCfe.dest.xNome;
                    destViewModel.RazaoSocial = infCfe.dest.xNome;
                    destViewModel.CnpjCpf = infCfe.dest.Item;

                    model.Destinatario = destViewModel;
                }

                foreach (var det in infCfe.det)
                {
                    ProdutoViewModel produto = new ProdutoViewModel();

                    produto.Codigo = det.prod.cProd;
                    produto.Descricao = det.prod.xProd;
                    produto.Ncm = det.prod.NCM;
                    produto.Cfop = det.prod.CFOP;
                    produto.Unidade = det.prod.uCom;
                    produto.Quantidade = det.prod.qCom;
                    produto.ValorUnitario = det.prod.vUnCom;

                    if (det.prod.vProd == 0)
                        produto.ValorTotal = det.prod.qCom * det.prod.vUnCom;
                    else
                        produto.ValorTotal = det.prod.vProd;

                    produto.InformacoesAdicionais = det.infAdProd;

                    var imposto = det.imposto;

                    if (imposto != null)
                    {
                        if (imposto.Item is CFeInfCFeDetImpostoICMS)
                        {
                            var icms = (((CFeInfCFeDetImpostoICMS)imposto.Item).Item);

                            if (icms != null)
                            {
                                if (icms is CFeInfCFeDetImpostoICMSICMS00)
                                {
                                    var ICMS00 = (CFeInfCFeDetImpostoICMSICMS00)icms;
                                    if (ICMS00 != null)
                                    {
                                        produto.ValorIcms = ICMS00.vICMS;
                                        //produto.BaseIcms = ICMS00.vBC;
                                        produto.AliquotaIcms = ICMS00.pICMS;
                                        produto.OCst = ICMS00.Orig + ICMS00.CST;
                                    }
                                }

                                if (icms is CFeInfCFeDetImpostoICMSICMS40)
                                {
                                    var ICMS40 = (CFeInfCFeDetImpostoICMSICMS40)icms;
                                    if (ICMS40 != null)
                                    {
                                        //produto.ValorIcms = ICMS40.vICMS;
                                        //produto.BaseIcms = ICMS40.vBC;
                                        //produto.AliquotaIcms = ICMS40.pICMS;
                                        //produto.OCst = ICMS40.Orig + ICMS40.CST + ICMS40.CSOSN;
                                    }
                                }

                                if (icms is CFeInfCFeDetImpostoICMSICMSSN102)
                                {
                                    var ICMSSN102 = (CFeInfCFeDetImpostoICMSICMSSN102)icms;
                                    if (ICMSSN102 != null)
                                    {

                                    }
                                }

                                if (icms is CFeInfCFeDetImpostoICMSICMSSN900)
                                {
                                    var ICMSSN900 = (CFeInfCFeDetImpostoICMSICMSSN900)icms;
                                    if (ICMSSN900 != null)
                                    {
                                        produto.ValorIcms = ICMSSN900.vICMS;
                                        //produto.BaseIcms = ICMSSN900.vBC;
                                        produto.AliquotaIcms = ICMSSN900.pICMS;
                                        produto.OCst = ICMSSN900.Orig + ICMSSN900.CSOSN;
                                    }
                                }
                            }
                        }
                        else if (imposto.Item is CFeInfCFeDetImpostoISSQN)
                        {
                            var issqn = (CFeInfCFeDetImpostoISSQN)imposto.Item;
                        }
                    }

                    model.Produtos.Add(produto);
                }

                //if (infCfe.cobr != null)
                //{
                //    foreach (var item in infCfe.cobr.dup)
                //    {
                //        DuplicataViewModel duplicata = new DuplicataViewModel();
                //        duplicata.Numero = item.nDup;
                //        duplicata.Valor = item.vDup;
                //        duplicata.Vecimento = item.dVenc;

                //        model.Duplicatas.Add(duplicata);
                //    }
                //}

                //model.CalculoImposto = CriarCalculoImpostoViewModel(infCfe.total.ICMSTot);

                //var issqnTotal = infCfe.total.ISSQNtot;

                //if (issqnTotal != null)
                //{
                //    var c = model.CalculoIssqn;
                //    c.InscricaoMunicipal = infCfe.emit.IM;
                //    c.BaseIssqn = issqnTotal.vBC;
                //    c.ValorTotalServicos = issqnTotal.vServ;
                //    c.ValorIssqn = issqnTotal.vISS;
                //    c.Mostrar = true;
                //}

                //var infAdic = infCfe.infAdic;
                //if (infAdic != null)
                //{
                //    model.InformacoesComplementares = cfe.NFe.infNFe.infAdic.infCpl;
                //    model.InformacoesAdicionaisFisco = cfe.NFe.infNFe.infAdic.infAdFisco;
                //}

                //var infoProto = cfe.protNFe.infProt;

                //model.ProtocoloAutorizacao = String.Format(Formatador.Cultura, "{0} - {1}", infoProto.nProt, infoProto.dhRecbto.DateTimeOffsetValue.DateTime);

                //ExtrairDatas(model, infCfe);
            }

            return model;
        }
    }
}
