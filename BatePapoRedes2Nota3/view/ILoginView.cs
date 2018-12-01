using BatePapoRedes2Nota3.modelo;
using System;

namespace BatePapoRedes2Nota3.view
{
    interface ILoginView
    {
        /// <summary>
        /// Mostra mensagem ao usuário quando login foi recusado
        /// </summary>
        /// <param name="resultado">Mensagem</param>
        void LoginRecusado(String resultado);
        /// <summary>
        /// Envia o usuário à tela de bate papo quando o login é aprovado
        /// </summary>
        /// <param name="usuario">Usuário logado</param>
        void LoginAprovado(Usuario usuario);
        /// <summary>
        /// Popula DataGridView com logins e senhas dos usuários
        /// </summary>
        /// <param name="usuarios">Vetor de usuários</param>
        void PopulaDataGridView(Usuario[] usuarios);
        /// <summary>
        /// Mostra ao usuário o resultado da tentativa de conexão ao servidor
        /// </summary>
        /// <param name="mensagem">Mensagem do resultado</param>
        void ResultadoConectar(String mensagem);
    }
}
