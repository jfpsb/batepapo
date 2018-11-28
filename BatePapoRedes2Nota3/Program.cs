using System;
using System.Threading;
using System.Windows.Forms;

namespace BatePapoRedes2Nota3
{
    static class Program
    {
        private static Thread threadLogin;
        private static Thread threadServidor;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Thread.CurrentThread.SetApartmentState(ApartmentState.STA);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            threadLogin = new Thread(AbreTelaLogin);
            threadServidor = new Thread(AbreTelaServidor);

            threadServidor.Start();
            threadLogin.Start();
        }

        [STAThread]
        private static void AbreTelaLogin()
        {
            try
            {
                Login telaLogin = new Login();
                DialogResult dialogResult = telaLogin.ShowDialog();

                if (dialogResult == DialogResult.OK)
                {
                    BatePapo batePapo = new BatePapo();
                    batePapo.SetUsuario(telaLogin.Usuario);

                    Application.Run(batePapo);
                }
                else
                {
                    Application.ExitThread();
                    Application.Exit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        [STAThread]
        private static void AbreTelaServidor()
        {
            new Servidor().ShowDialog();
        }
    }
}
