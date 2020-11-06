using DanfeSharp.Elementos;
using DanfeSharp.Elementos.NFCe;
using DanfeSharp.Enumeracoes;
using DanfeSharp.Modelo;
using System;

namespace DanfeSharp.Blocos.NFCe
{
    internal class BlocoDestinatarioRemetente : BlocoBase
    {
        IdentificacaoRemetente idRemetente;

        public BlocoDestinatarioRemetente(DanfeViewModel viewModel, Estilo estilo) : base(viewModel, estilo)
        {
            var destinatario = viewModel.Destinatario;

            var height = 12;
            if (!string.IsNullOrEmpty(destinatario.CnpjCpf))
            {
                height += 3;

                var nome = !String.IsNullOrWhiteSpace(destinatario.NomeFantasia) ? destinatario.NomeFantasia : destinatario.RazaoSocial;

                if (!string.IsNullOrEmpty(nome))
                    height += 3;
            }

            idRemetente = new IdentificacaoRemetente(Estilo, ViewModel);

            FlexibleLine fl = new FlexibleLine() { Height = height }
            .ComElemento(idRemetente)
            .ComLargurasIguais();

            MainVerticalStack.Add(fl);
        }

        public override string Cabecalho => "";
        public override PosicaoBloco Posicao => PosicaoBloco.Base;
    }
}
