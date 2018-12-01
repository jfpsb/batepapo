using BatePapoRedes2Nota3.controller.util;
using BatePapoRedes2Nota3.modelo;
using BatePapoRedes2Nota3.view;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace BatePapoRedes2Nota3.controller
{
    /// <summary>
    /// Controlador da tela de servidor
    /// </summary>
    class ServidorController
    {
        IServidorView servidorView;
        Usuario usuarioModel = new Usuario();

        private Socket ouvinte;
        private byte[] recebe_do_cliente = new byte[1024];
        private Dictionary<String, Socket> usuarios_em_sessao = new Dictionary<string, Socket>();
        private Usuario[] usuarios_cadastrados;

        Thread shutdownThread;

        public ServidorController(IServidorView servidorView)
        {
            this.servidorView = servidorView;
            usuarios_cadastrados = usuarioModel.CarregaUsuarios();
            shutdownThread = new Thread(ShutdownThread);
        }

        /// <summary>
        /// Inicia o servidor do bate papo na porta 3500
        /// </summary>
        public void IniciaServidor()
        {
            //Abre socket ouvinte configurado para IPv4, stream de bytes e protocolo TCP
            ouvinte = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //Representa IP e porta
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3500);

            //Atribui o IP e porta ao socket ouvinte
            ouvinte.Bind(endPoint);

            //Inicia o processo de ouvir no socket e limita o número de requisições ao mesmo tempo para 10
            ouvinte.Listen(10);

            //Inicia processo assíncrono de aceitar conexões solicitadas
            ouvinte.BeginAccept(AcceptCallback, null);

            servidorView.AlteraStatus("CONECTADO", Color.DarkGreen);
        }

        /// <summary>
        /// Lida com as requisições de conexão ao ouvinte
        /// </summary>
        /// <param name="asyncResult">Representa o estado da operação assíncrona</param>
        private void AcceptCallback(IAsyncResult asyncResult)
        {
            try
            {
                //Socket que solicitou a conexão
                Socket socket_cliente = ouvinte.EndAccept(asyncResult);

                //Começa a ouvir este cliente de forma assíncrona
                socket_cliente.BeginReceive(recebe_do_cliente, 0, recebe_do_cliente.Length, SocketFlags.None, ReceiveCallback, socket_cliente);

                //Retoma a operação assíncrona de esperar uma conexão de cliente
                ouvinte.BeginAccept(AcceptCallback, null);
            }
            catch (ObjectDisposedException)
            {
                Console.WriteLine("Ouvinte foi Disposed");
            }
        }

        /// <summary>
        /// Responsável por processar os envios do servidor para clientes
        /// </summary>
        /// <param name="asyncResult">Representa o estado da operação assíncrona</param>
        private void SendCallback(IAsyncResult asyncResult)
        {
            Socket socket_cliente = (Socket)asyncResult.AsyncState;
            socket_cliente.EndSend(asyncResult);
        }

        /// <summary>
        /// Responsável por processar o que é enviado dos clientes para o servidor
        /// </summary>
        /// <param name="asyncResult"></param>
        private void ReceiveCallback(IAsyncResult asyncResult)
        {
            //Socket que enviou algo ao servidor
            Socket socket_cliente = (Socket)asyncResult.AsyncState;

            if (socket_cliente.Connected)
            {
                //Captura tamanho do vetor com requisição do cliente
                int tamanho_receive = socket_cliente.EndReceive(asyncResult);

                //Cria vetor com tamanho exato do que foi enviado ao servidor
                byte[] recebe_do_cliente_tamanho_exato = new byte[tamanho_receive];

                //Copia a mensagem do cliente e coloca em um vetor de bytes de tamanho exato ao da mensagem
                //para eliminar os campos vazios do vetor
                Array.Copy(recebe_do_cliente, recebe_do_cliente_tamanho_exato, tamanho_receive);

                //String do comando enviado pelo cliente que será tratado pelo servidor
                String requisicao_do_cliente = Util.RetornaEmString(recebe_do_cliente_tamanho_exato);

                //Comando é a primeira palavra da requisição
                String comando = requisicao_do_cliente.Split(new char[] { ' ' })[0];

                switch (comando)
                {
                    case "login":
                        Login(requisicao_do_cliente, socket_cliente);
                        break;
                    case "logou":
                        Logou(requisicao_do_cliente, socket_cliente);
                        break;
                    case "msg":
                        Mensagem(requisicao_do_cliente);
                        break;
                    case "exit":
                        Exit(requisicao_do_cliente);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Responsável por iniciar thread que executa o comando shutdown
        /// </summary>
        public void Shutdown()
        {
            try
            {
                shutdownThread.Start();
            }
            catch(ThreadStateException)
            {
                servidorView.MensagemAoUsuario("O servidor já está sendo encerrado!");
            }
        }

        /// <summary>
        /// Responsável por encerrar a sessão do bate papo para todos os clientes
        /// </summary>
        private void ShutdownThread()
        {
            try
            {
                byte[] resposta_ao_cliente = new byte[8];

                resposta_ao_cliente = Util.RetornaEmByteArray("shutdown");

                //Envia o comando shutdown para todos os clientes
                foreach (KeyValuePair<String, Socket> usuario_em_sessao in usuarios_em_sessao)
                {
                    Socket socket_cliente = usuario_em_sessao.Value;
                    socket_cliente.BeginSend(resposta_ao_cliente, 0, resposta_ao_cliente.Length, SocketFlags.None, SendCallback, socket_cliente);
                }

                //Aguarda 11 segundos para encerrar socket ouvinte
                Thread.Sleep(11000);

                //Limpa dicionário de clientes
                usuarios_em_sessao.Clear();
                servidorView.AlteraStatus("DESCONECTADO", Color.Red);
                servidorView.AdicionarComandoDataGridView("Admin", "shutdown");

                //Encerrar comunicação do ouvinte
                ouvinte.Shutdown(SocketShutdown.Both);
            }
            catch (SocketException se)
            {
                Console.WriteLine("Socket Exception: " + se.Message);
            }
            catch (ObjectDisposedException ode)
            {
                Console.WriteLine("Ouvinte foi Disposed: " + ode.Message);
            }
            finally
            {
                ouvinte.Close();
            }
        }

        /// <summary>
        /// Processa a requisição de login
        /// </summary>
        /// <param name="requisicao">Comando enviado da tela de login para servidor</param>
        /// <param name="socket_cliente">Socket conectado ao ouvinte</param>
        private void Login(String requisicao, Socket socket_cliente)
        {
            byte[] resposta_ao_cliente = new byte[100];

            String[] chaves_requisicao = requisicao.Split(new char[] { ' ' });

            servidorView.AdicionarComandoDataGridView("Admin", requisicao);

            if (chaves_requisicao.Length == 3)
            {
                String login = chaves_requisicao[1];
                String senha = chaves_requisicao[2];

                //Testa se usuário já não está logado no bate papo
                if (usuarios_em_sessao.ContainsKey(login))
                {
                    resposta_ao_cliente = Util.RetornaEmByteArray("Já existe uma sessão aberta no bate papo com este usuário.");
                    socket_cliente.BeginSend(resposta_ao_cliente, 0, resposta_ao_cliente.Length, SocketFlags.None, SendCallback, socket_cliente);
                    socket_cliente.BeginReceive(recebe_do_cliente, 0, recebe_do_cliente.Length, SocketFlags.None, ReceiveCallback, socket_cliente);
                    return;
                }

                Boolean resultado = usuarioModel.Autenticar(login, senha);

                if (resultado)
                {
                    resposta_ao_cliente = Util.RetornaEmByteArray("ok");
                }
                else
                {
                    resposta_ao_cliente = Util.RetornaEmByteArray("notok");
                }
            }
            else
            {
                resposta_ao_cliente = Util.RetornaEmByteArray("Erro ao formular comando.");
            }

            socket_cliente.BeginSend(resposta_ao_cliente, 0, resposta_ao_cliente.Length, SocketFlags.None, SendCallback, socket_cliente);
        }

        /// <summary>
        /// Responsável por receber usuário que logou no bate papo, salva o usuário e o seu socket no dicionário
        /// </summary>
        /// <param name="requisicao">Comando enviado da tela de login para servidor</param>
        /// <param name="socket_cliente">Socket conectado ao ouvinte</param>
        private void Logou(String requisicao, Socket socket_cliente)
        {
            byte[] resposta_ao_cliente = Util.RetornaEmByteArray(requisicao);

            String usuario = requisicao.Split(new char[] { ' ' })[1];

            usuarios_em_sessao.Add(usuario, socket_cliente);

            servidorView.AdicionarComandoDataGridView(usuario, requisicao);

            foreach (KeyValuePair<String, Socket> usuario_em_sessao in usuarios_em_sessao)
            {
                Socket socket_usuario = usuario_em_sessao.Value;
                //Envia aos outros clientes que este cliente logou
                socket_usuario.BeginSend(resposta_ao_cliente, 0, resposta_ao_cliente.Length, SocketFlags.None, SendCallback, socket_usuario);
            }

            //Socket que logou começa a esperar receber do servidor
            socket_cliente.BeginReceive(recebe_do_cliente, 0, recebe_do_cliente.Length, SocketFlags.None, ReceiveCallback, socket_cliente);
        }

        /// <summary>
        /// Responsável por processar as mensagens enviadas dos clientes
        /// </summary>
        /// <param name="requisicao">Comando enviado para o servidor</param>
        private void Mensagem(String requisicao)
        {
            byte[] resposta_ao_cliente = new byte[1024];

            resposta_ao_cliente = Util.RetornaEmByteArray(requisicao);

            String usuario = requisicao.Split(new char[] { ' ' })[1];

            servidorView.AdicionarComandoDataGridView(usuario, requisicao);

            foreach (KeyValuePair<String, Socket> usuario_em_sessao in usuarios_em_sessao)
            {
                Socket socket_cliente = usuario_em_sessao.Value;
                socket_cliente.BeginSend(resposta_ao_cliente, 0, resposta_ao_cliente.Length, SocketFlags.None, SendCallback, socket_cliente);
                socket_cliente.BeginReceive(recebe_do_cliente, 0, recebe_do_cliente.Length, SocketFlags.None, ReceiveCallback, socket_cliente);
            }
        }

        private void Exit(String requisicao)
        {
            if (usuarios_em_sessao.Count > 0)
            {
                byte[] resposta_ao_cliente = new byte[1024];

                resposta_ao_cliente = Util.RetornaEmByteArray(requisicao);

                String usuario = requisicao.Split(new char[] { ' ' })[1];

                servidorView.AdicionarComandoDataGridView(usuario, requisicao);

                Socket s = usuarios_em_sessao[usuario];
                s.Shutdown(SocketShutdown.Both);
                s.Close();

                usuarios_em_sessao.Remove(usuario);

                foreach (KeyValuePair<String, Socket> usuario_em_sessao in usuarios_em_sessao)
                {
                    usuario_em_sessao.Value.BeginSend(resposta_ao_cliente, 0, resposta_ao_cliente.Length, SocketFlags.None, SendCallback, usuario_em_sessao.Value);
                }
            }
        }
    }
}
