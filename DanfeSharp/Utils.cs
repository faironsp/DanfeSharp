using System;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace DanfeSharp
{
    internal static class Utils
    {
        /// <summary>
        /// Verifica se uma string contém outra string no formato chave: valor.
        /// </summary>
        public static Boolean StringContemChaveValor(String str, String chave, String valor)
        {
            if (String.IsNullOrWhiteSpace(chave)) throw new ArgumentException(nameof(chave));
            if (String.IsNullOrWhiteSpace(str)) return false;

            return Regex.IsMatch(str, $@"({chave}):?\s*{valor}", RegexOptions.IgnoreCase);
        }

        public static String TipoDFeDeChaveAcesso(String chaveAcesso)
        {
            if (String.IsNullOrWhiteSpace(chaveAcesso)) throw new ArgumentException(nameof(chaveAcesso));

            if (chaveAcesso.Length == 44)
            {
                String f = chaveAcesso.Substring(20, 2);

                if (f == "55") return "NF-e";
                else if (f == "57") return "CT-e";
                else if (f == "65") return "NFC-e";
            }

            return "DF-e Desconhecido";
        }

        public static string FormatarCpfCnpj(string strCpfCnpj)
        {
            if (strCpfCnpj.Length <= 11)
            {
                MaskedTextProvider mtpCpf = new MaskedTextProvider(@"000\.000\.000-00");

                mtpCpf.Set(ZerosEsquerda(strCpfCnpj, 11));

                return mtpCpf.ToString();
            }
            else
            {
                MaskedTextProvider mtpCnpj = new MaskedTextProvider(@"00\.000\.000/0000-00");

                mtpCnpj.Set(ZerosEsquerda(strCpfCnpj, 11));

                return mtpCnpj.ToString();
            }
        }

        public static string ZerosEsquerda(string strString, int intTamanho)
        {
            string strResult = "";

            for (int intCont = 1; intCont <= (intTamanho - strString.Length); intCont++)
            {
                strResult += "0";
            }

            return strResult + strString;
        }
    }
}
