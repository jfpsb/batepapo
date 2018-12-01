using System;

namespace BatePapoRedes2Nota3.view
{
    /// <summary>
    /// Interface para view de bate papo
    /// </summary>
    interface IBatePapoView
    {
        /// <summary>
        /// Adiciona as mensagens no datagridview
        /// </summary>
        /// <param name="usuario">Usuário que enviou mensagem</param>
        /// <param name="mensagem">Mensagem</param>
        void AdicionarComandoDataGridView(String usuario, String mensagem);
        /// <summary>
        /// Limpa o campo de mensagem
        /// </summary>
        void LimparMensagem();
        /// <summary>
        /// Altera o texto de avisos ao usuário
        /// </summary>
        /// <param name="texto">Aviso</param>
        void AlterarTextoTxtAviso(String texto);
        /// <summary>
        /// Configura o título da tela de bate papo com nome do usuário
        /// </summary>
        /// <param name="usuario">Nome do usuário</param>
        void ConfiguraTituloBatePapo(String usuario);
        /// <summary>
        /// Fecha tela
        /// </summary>
        void Fechar();
    }
}
