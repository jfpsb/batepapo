using System;

namespace BatePapoRedes2Nota3.view
{
    interface IBatePapoView
    {
        void AdicionarComandoDataGridView(String usuario, String mensagem);
        void LimparMensagem();
        void AlterarTextoTxtAviso(String texto);
        void ConfiguraTituloBatePapo(String usuario);
        void Fechar();
    }
}
