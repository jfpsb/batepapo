using BatePapoRedes2Nota3.modelo;
using System;

namespace BatePapoRedes2Nota3.view
{
    interface ILoginView
    {
        void LoginRecusado(String resultado);
        void LoginAprovado(Usuario usuario);
        void PopulaDataGridView(Usuario[] usuarios);
        void ResultadoConectar(String mensagem);
    }
}
