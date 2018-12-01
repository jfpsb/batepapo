using System;
using System.Drawing;

namespace BatePapoRedes2Nota3.view
{
    public interface IServidorView
    {
        /// <summary>
        /// Alterar status do servidor para conectado ou desconectado
        /// </summary>
        /// <param name="texto">Texto a ser mostrado</param>
        /// <param name="cor">Cor do texto</param>
        void AlteraStatus(String texto, Color cor);
        /// <summary>
        /// Adiciona os comandos recebidos pelo servidor em DataGridView
        /// </summary>
        /// <param name="usuario">Usuário que mandou comando</param>
        /// <param name="mensagem">Mensagem recebida pelo servidor</param>
        void AdicionarComandoDataGridView(String usuario, String mensagem);
        /// <summary>
        /// Mostra qualquer mensagem ao usuário necessária
        /// </summary>
        /// <param name="mensagem">Texto da mensagem</param>
        void MensagemAoUsuario(String mensagem);
    }
}
