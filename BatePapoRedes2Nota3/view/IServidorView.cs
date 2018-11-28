using System;
using System.Drawing;

namespace BatePapoRedes2Nota3.view
{
    public interface IServidorView
    {
        void AlteraStatus(String texto, Color cor);
        void AdicionarComandoDataGridView(String usuario, String mensagem);
        void MensagemAoUsuario(String mensagem);
    }
}
