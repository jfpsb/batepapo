using System;
using System.Text;

namespace BatePapoRedes2Nota3.controller.util
{
    class Util
    {
        public static byte[] RetornaEmByteArray(String mensagem)
        {
            return Encoding.ASCII.GetBytes(mensagem);
        }

        public static String RetornaEmString(byte[] vetor)
        {
            return Encoding.ASCII.GetString(vetor);
        }
    }
}
