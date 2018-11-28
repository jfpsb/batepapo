using BatePapoRedes2Nota3.controller.util;
using BatePapoRedes2Nota3.modelo;
using BatePapoRedes2Nota3.view;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace BatePapoRedes2Nota3.controller
{
    class BatePapoController
    {
        private Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private IBatePapoView batePapoView;
        private Usuario usuarioModel = new Usuario();
        private Thread threadLogin;
        private byte[] recebido_do_servidor = new byte[1024];

        public BatePapoController(IBatePapoView batePapoView)
        {
            this.batePapoView = batePapoView;
        }

        public void Conectar()
        {
            socket.Connect(IPAddress.Loopback, 3500);

            String requisicao_do_cliente = "logou " + usuarioModel.login;

            socket.BeginSend(Util.RetornaEmByteArray(requisicao_do_cliente), 0, Util.RetornaEmByteArray(requisicao_do_cliente).Length, SocketFlags.None, SendCallback, socket);
            socket.BeginReceive(recebido_do_servidor, 0, recebido_do_servidor.Length, SocketFlags.None, ReceiveCallback, socket);
        }

        private void SendCallback(IAsyncResult asyncResult)
        {
            Socket socket = (Socket)asyncResult.AsyncState;

            if(socket.Connected)
            {
                socket.EndSend(asyncResult);
            }
        }

        private void ReceiveCallback(IAsyncResult asyncResult)
        {
            Socket socket = (Socket)asyncResult.AsyncState;

            if (socket.Connected)
            {
                int index = socket.EndReceive(asyncResult);

                byte[] recebido_do_servidor_tamanho_exato = new byte[index];

                Array.Copy(recebido_do_servidor, recebido_do_servidor_tamanho_exato, index);

                String comando = Util.RetornaEmString(recebido_do_servidor_tamanho_exato).Split(new char[] { ' ' })[0];

                switch (comando)
                {
                    case "logou":
                        Logou(Util.RetornaEmString(recebido_do_servidor_tamanho_exato));
                        break;
                    case "msg":
                        Mensagem(Util.RetornaEmString(recebido_do_servidor_tamanho_exato));
                        break;
                    case "exit":
                        Exit(Util.RetornaEmString(recebido_do_servidor_tamanho_exato));
                        break;
                    case "shutdown":
                        Shutdown();
                        break;
                    default:
                        break;
                }
            }
        }

        private void Shutdown()
        {
            Thread t = new Thread(new ParameterizedThreadStart(contador));
            t.Start(DateTime.Now);
        }

        public void EnviarMensagem(String mensagem)
        {
            String msg = mensagem.Trim(new char[] { ' ', '\r', '\n' });
            String requisicao_do_cliente = null;

            batePapoView.LimparMensagem();

            if (msg != String.Empty && socket.Connected)
            {
                switch (msg)
                {
                    case "exit":
                        requisicao_do_cliente = "exit " + usuarioModel.login;
                        batePapoView.Fechar();
                        break;
                    default:
                        requisicao_do_cliente = "msg " + usuarioModel.login + " " + msg;
                        break;
                }

                socket.BeginSend(Util.RetornaEmByteArray(requisicao_do_cliente), 0, Util.RetornaEmByteArray(requisicao_do_cliente).Length, SocketFlags.None, SendCallback, socket);

                if(socket.Connected)
                    socket.BeginReceive(recebido_do_servidor, 0, recebido_do_servidor.Length, SocketFlags.None, ReceiveCallback, socket);
            }
        }

        private void contador(object data)
        {
            DateTime horaComando = (DateTime)data;

            double segundos = 11;
            double intervalo = 0;

            while (intervalo < 10)
            {
                intervalo = (DateTime.Now - horaComando).TotalSeconds;

                batePapoView.AlterarTextoTxtAviso("O bate papo será fechado em " + ((int)(segundos - intervalo)).ToString() + " segundos");
            }

            batePapoView.AlterarTextoTxtAviso("A sessão do bate papo foi encerrada pelo administrador");
        }

        private void Mensagem(String requisicao)
        {
            String mensagem = requisicao.Remove(0, 4);

            String usuario = mensagem.Split(new char[] { ' ' })[0];
            mensagem = mensagem.Remove(0, usuario.Length + 1);

            batePapoView.AdicionarComandoDataGridView(usuario, mensagem);

            socket.BeginReceive(recebido_do_servidor, 0, recebido_do_servidor.Length, SocketFlags.None, ReceiveCallback, socket);
        }

        private void Exit(String requisicao)
        {
            String usuario = requisicao.Split(new char[] { ' ' })[1];

            String exit_msg = "Usuário " + usuario + " saiu da sala de bate papo!";

            batePapoView.AlterarTextoTxtAviso(exit_msg);
        }

        private void Logou(String requisicao)
        {
            String usuario = requisicao.Split(new char[] { ' ' })[1];

            String logou_msg = "Usuário " + usuario + " entrou na sala de bate papo!";

            batePapoView.AlterarTextoTxtAviso(logou_msg);

            socket.BeginReceive(recebido_do_servidor, 0, recebido_do_servidor.Length, SocketFlags.None, ReceiveCallback, socket);
        }

        public void FecharTelaBatePapo()
        {
            if (socket.Connected)
            {
                socket.BeginSend(Util.RetornaEmByteArray("exit " + usuarioModel.login), 0, Util.RetornaEmByteArray("exit " + usuarioModel.login).Length, SocketFlags.None, SendCallback, socket);
            }
        }

        [STAThread]
        private void AbrirOutroBatePapo()
        {
            Login telaLogin = new Login();
            DialogResult dialogResult = telaLogin.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                BatePapo batePapo = new BatePapo();
                batePapo.SetUsuario(telaLogin.Usuario);
                batePapo.ShowDialog();
            }
            else
            {
                telaLogin.Close();
            }
        }

        public void SetUsuario(Usuario usuario)
        {
            usuarioModel = usuario;
            batePapoView.ConfiguraTituloBatePapo(usuarioModel.login);
        }

        public void MenuStripAbrirOutroBatePapo()
        {
            threadLogin = new Thread(AbrirOutroBatePapo);
            threadLogin.Start();
        }

        public String Usuario_Login
        {
            get
            {
                return usuarioModel.login;
            }
        }
    }
}
