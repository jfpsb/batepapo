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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            threadLogin = new Thread(AbreTelaLogin);
            threadServidor = new Thread(AbreTelaServidor);

            threadServidor.Start();
            threadLogin.Start();
        }

        private static void AbreTelaLogin()
        {
            Login telaLogin = new Login();
            DialogResult dialogResult = telaLogin.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                BatePapo batePapo = new BatePapo();
                batePapo.Login_Usuario = telaLogin.LoginUsuario();

                Application.Run(batePapo);
            }
            else
            {
                Application.ExitThread();
                Application.Exit();
            }
        }

        private static void AbreTelaServidor()
        {
            new Servidor().ShowDialog();
        }
    }
}
