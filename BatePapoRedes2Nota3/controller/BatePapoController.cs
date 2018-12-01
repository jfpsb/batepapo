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
    /// <summary>
    /// Controlador da tela de bate papo
    /// </summary>
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

        /// <summary>
        /// Realiza a conexão com o servidor e envia o comando "logou" para o servidor. O BeginSend envia a requisição de forma assíncrona
        /// e logo depois é acionado o estado de receber do servidor pelo BeginReceive
        /// </summary>
        public void Conectar()
        {
            //Como estou rodando localmente, uso o Loopback (127.0.0.1)
            socket.Connect(IPAddress.Loopback, 3500);

            String requisicao_do_cliente = "logou " + usuarioModel.login;

            //Envia ao servidor comando logou
            socket.BeginSend(Util.RetornaEmByteArray(requisicao_do_cliente), 0, Util.RetornaEmByteArray(requisicao_do_cliente).Length, SocketFlags.None, SendCallback, socket);
            //Começa a ouvir servidor
            socket.BeginReceive(recebido_do_servidor, 0, recebido_do_servidor.Length, SocketFlags.None, ReceiveCallback, socket);
        }

        /// <summary>
        /// Método callback responsável por tratar a operação de Send do socket
        /// </summary>
        /// <param name="asyncResult">Guarda estado da operação assíncrona, no caso o socket</param>
        private void SendCallback(IAsyncResult asyncResult)
        {
            Socket socket = (Socket)asyncResult.AsyncState;
            socket.EndSend(asyncResult);
        }

        /// <summary>
        /// Método callback responsável por tratar a operação Receive do socket. Quando o servidor envia algo é aqui que é tratado.
        /// </summary>
        /// <param name="asyncResult">Guarda estado da operação assíncrona, no caso o socket</param>
        private void ReceiveCallback(IAsyncResult asyncResult)
        {
            Socket socket = (Socket)asyncResult.AsyncState;

            // Tamanho do vetor byte com os dados do servidor
            int index = socket.EndReceive(asyncResult);

            // Crio vetor de byte com tamanho exato da mensagem
            byte[] recebido_do_servidor_tamanho_exato = new byte[index];

            // Copio do vetor maior para o com tamanho exato para eliminar os campos vazios do vetor maior
            Array.Copy(recebido_do_servidor, recebido_do_servidor_tamanho_exato, index);

            // A primeira palavra da mensagem é o comando
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

        /// <summary>
        /// Inicia a thread do comando shutdown para encerrar a sessão
        /// </summary>
        private void Shutdown()
        {
            Thread shutdownThread = new Thread(new ParameterizedThreadStart(ShutdownThread));
            shutdownThread.Start(DateTime.Now);
        }

        /// <summary>
        /// Responsável por mostrar ao usuário a contagem de 10 segundos para fechamento do bate papo
        /// </summary>
        /// <param name="data">Momento em que o comando foi enviado pelo servidor</param>
        private void ShutdownThread(object data)
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

        /// <summary>
        /// Método que trata o envio das mensagens do bate papo para servidor
        /// </summary>
        /// <param name="mensagem">Conteúdo da mensagem</param>
        public void EnviarMensagem(String mensagem)
        {
            String msg = mensagem.Trim(new char[] { ' ', '\r', '\n' });
            String requisicao_do_cliente = null;

            batePapoView.LimparMensagem();

            if (msg != String.Empty && socket.Connected)
            {
                switch (msg)
                {
                    //Se o usuário digitar exit vai acionar o comando de fechar bate papo
                    case "exit":
                        //O envio do comando exit é feito no evento de fechamento da tela
                        batePapoView.Fechar();
                        break;
                    default:
                        //Qualquer outra mensagem é tratada como mensagem do bate papo
                        requisicao_do_cliente = "msg " + usuarioModel.login + " " + msg;
                        socket.BeginSend(Util.RetornaEmByteArray(requisicao_do_cliente), 0, Util.RetornaEmByteArray(requisicao_do_cliente).Length, SocketFlags.None, SendCallback, socket);
                        //socket.BeginReceive(recebido_do_servidor, 0, recebido_do_servidor.Length, SocketFlags.None, ReceiveCallback, socket);
                        break;
                }
            }
        }

        /// <summary>
        /// Responsável por tratar as mensagens sendo replicadas dos outros usuários pelo servidor
        /// </summary>
        /// <param name="requisicao">Comando enviado pelo servidor</param>
        private void Mensagem(String requisicao)
        {
            //remove comando msg
            String mensagem = requisicao.Remove(0, 4);

            //Extrai usuário que mandou mensagem
            String usuario = mensagem.Split(new char[] { ' ' })[0];

            //Mensagem de fato
            mensagem = mensagem.Remove(0, usuario.Length + 1);            

            batePapoView.AdicionarComandoDataGridView(usuario, mensagem);

            socket.BeginReceive(recebido_do_servidor, 0, recebido_do_servidor.Length, SocketFlags.None, ReceiveCallback, socket);
        }

        /// <summary>
        /// Responsável por informar aos usuários no bate papo se alguém saiu da sessão
        /// </summary>
        /// <param name="requisicao">Comando enviado pelo servidor</param>
        private void Exit(String requisicao)
        {
            String usuario = requisicao.Split(new char[] { ' ' })[1];

            String exit_msg = "Usuário " + usuario + " saiu da sala de bate papo!";

            batePapoView.AlterarTextoTxtAviso(exit_msg);

            socket.BeginReceive(recebido_do_servidor, 0, recebido_do_servidor.Length, SocketFlags.None, ReceiveCallback, socket);
        }

        /// <summary>
        /// Responsável por informar aos usuários no bate papo quem entrou na sessão
        /// </summary>
        /// <param name="requisicao">Comando enviado pelo servidor</param>
        private void Logou(String requisicao)
        {
            String usuario = requisicao.Split(new char[] { ' ' })[1];

            String logou_msg = "Usuário " + usuario + " entrou na sala de bate papo!";

            batePapoView.AlterarTextoTxtAviso(logou_msg);

            socket.BeginReceive(recebido_do_servidor, 0, recebido_do_servidor.Length, SocketFlags.None, ReceiveCallback, socket);
        }

        /// <summary>
        /// Método chamado na view que envia o comando exit
        /// </summary>
        public void FecharTelaBatePapo()
        {
            if (socket.Connected)
            {
                socket.BeginSend(Util.RetornaEmByteArray("exit " + usuarioModel.login), 0, Util.RetornaEmByteArray("exit " + usuarioModel.login).Length, SocketFlags.None, SendCallback, socket);
            }
        }

        /// <summary>
        /// Método que abre nova tela de bate papo
        /// </summary>
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

        /// <summary>
        /// Configura usuário proprietário da sessão no bate papo
        /// </summary>
        /// <param name="usuario">Usuário logado</param>
        public void SetUsuario(Usuario usuario)
        {
            usuarioModel = usuario;
            batePapoView.ConfiguraTituloBatePapo(usuarioModel.login);
        }

        /// <summary>
        /// Inicia thread para abrir outro bate papo
        /// </summary>
        public void MenuStripAbrirOutroBatePapo()
        {
            threadLogin = new Thread(AbrirOutroBatePapo);
            threadLogin.Start();
        }
    }
}
