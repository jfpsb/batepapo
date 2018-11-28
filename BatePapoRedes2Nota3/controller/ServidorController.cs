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

        public void IniciaServidor()
        {
            ouvinte = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3500);

            ouvinte.Bind(endPoint);
            ouvinte.Listen(10);

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

        private void SendCallback(IAsyncResult asyncResult)
        {
            Socket socket_cliente = (Socket)asyncResult.AsyncState;

            if (socket_cliente.Connected)
            {
                socket_cliente.EndSend(asyncResult);
            }
        }

        private void ReceiveCallback(IAsyncResult asyncResult)
        {
            //Socket que enviou algo ao servidor
            Socket socket_cliente = (Socket)asyncResult.AsyncState;

            if (socket_cliente.Connected)
            {
                int tamanho_receive = socket_cliente.EndReceive(asyncResult);

                byte[] recebe_do_cliente_tamanho_exato = new byte[tamanho_receive];

                //Copia a mensagem do cliente e coloca em um vetor de bytes de tamanho exato ao da mensagem
                //para eliminar os campos vazios do vetor
                Array.Copy(recebe_do_cliente, recebe_do_cliente_tamanho_exato, tamanho_receive);

                //String do comando enviado pelo cliente que será tratado pelo servidor
                String requisicao_do_cliente = Util.RetornaEmString(recebe_do_cliente_tamanho_exato);

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

        private void ShutdownThread()
        {
            try
            {
                byte[] resposta_ao_cliente = new byte[8];

                resposta_ao_cliente = Util.RetornaEmByteArray("shutdown");

                foreach (KeyValuePair<String, Socket> usuario_em_sessao in usuarios_em_sessao)
                {
                    Socket socket_cliente = usuario_em_sessao.Value;
                    socket_cliente.BeginSend(resposta_ao_cliente, 0, resposta_ao_cliente.Length, SocketFlags.None, SendCallback, socket_cliente);
                }

                Thread.Sleep(10000);

                usuarios_em_sessao.Clear();
                servidorView.AlteraStatus("DESCONECTADO", Color.Red);
                servidorView.AdicionarComandoDataGridView("Admin", "shutdown");

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

        private void Login(String requisicao, Socket socket_cliente)
        {
            byte[] resposta_ao_cliente = new byte[100];

            String[] chaves_requisicao = requisicao.Split(new char[] { ' ' });

            servidorView.AdicionarComandoDataGridView("Admin", requisicao);

            if (chaves_requisicao.Length == 3)
            {
                String login = chaves_requisicao[1];
                String senha = chaves_requisicao[2];

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
            socket_cliente.BeginReceive(recebe_do_cliente, 0, recebe_do_cliente.Length, SocketFlags.None, ReceiveCallback, socket_cliente);
        }

        private void Logou(String requisicao, Socket socket_cliente)
        {
            byte[] resposta_ao_cliente = Util.RetornaEmByteArray(requisicao);

            String usuario = requisicao.Split(new char[] { ' ' })[1];

            usuarios_em_sessao.Add(usuario, socket_cliente);

            servidorView.AdicionarComandoDataGridView(usuario, requisicao);

            foreach (KeyValuePair<String, Socket> usuario_em_sessao in usuarios_em_sessao)
            {
                Socket socket_usuario = usuario_em_sessao.Value;
                socket_usuario.BeginSend(resposta_ao_cliente, 0, resposta_ao_cliente.Length, SocketFlags.None, SendCallback, socket_usuario);
                socket_cliente.BeginReceive(recebe_do_cliente, 0, recebe_do_cliente.Length, SocketFlags.None, ReceiveCallback, socket_cliente);
            }
        }

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
