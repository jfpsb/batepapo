using BatePapoRedes2Nota3.data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace BatePapoRedes2Nota3
{
    public partial class Servidor : Form
    {
        public Servidor()
        {
            InitializeComponent();
        }

        private Socket ouvinte = null;
        private Dictionary<String, Socket> usuarios_em_sessao = new Dictionary<string, Socket>();
        private byte[] recebe_do_cliente = new byte[1024];

        private Usuario[] usuarios_cadastrados = new Usuario[10];

        private void Servidor_Load(object sender, EventArgs e)
        {
            IniciarServidor();

            // Popula a lista de usuários com logins e senha no servidor
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            var usuariosDicionario = serializer.Deserialize<Dictionary<String, Usuario[]>>(File.ReadAllText("../../data/usuarios.json"));

            foreach (KeyValuePair<string, Usuario[]> entrada in usuariosDicionario)
            {
                usuarios_cadastrados = entrada.Value;
            }
        }

        /// <summary>
        /// Cria socket do servidor
        /// </summary>
        private void IniciarServidor()
        {
            ouvinte = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3500);

            ouvinte.Bind(endPoint);
            ouvinte.Listen(10);

            ouvinte.BeginAccept(AcceptCallback, null);

            labelActualStatus.Text = "CONECTADO";
            labelActualStatus.ForeColor = Color.DarkGreen;
        }

        private void AcceptCallback(IAsyncResult asyncResult)
        {
            Socket socket_cliente = ouvinte.EndAccept(asyncResult);

            //Começa a ouvir este cliente de forma assíncrona
            socket_cliente.BeginReceive(recebe_do_cliente, 0, recebe_do_cliente.Length, SocketFlags.None, ReceiveCallback, socket_cliente);

            //Retoma a operação assíncrona de esperar uma conexão de cliente
            ouvinte.BeginAccept(AcceptCallback, null);
        }

        private void ReceiveCallback(IAsyncResult asyncResult)
        {
            Socket socket_cliente = (Socket)asyncResult.AsyncState;

            int tamanho_receive = socket_cliente.EndReceive(asyncResult);

            byte[] recebe_do_cliente_tamanho_exato = new byte[tamanho_receive];

            //Copia a mensagem do cliente e coloca em um vetor de bytes de tamanho exato ao da mensagem
            //para eliminar os campos vazios do vetor
            Array.Copy(recebe_do_cliente, recebe_do_cliente_tamanho_exato, tamanho_receive);

            //String do comando enviado pelo cliente que será tratado pelo servidor
            String requisicao_do_cliente = RetornaEmString(recebe_do_cliente_tamanho_exato);

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
                case "escrevendo":
                    break;
                case "exit":
                    Exit(requisicao_do_cliente);
                    break;
                default:
                    break;
            }

        }

        private void SendCallback(IAsyncResult asyncResult)
        {
            Socket socket_cliente = (Socket)asyncResult.AsyncState;
            socket_cliente.EndSend(asyncResult);

            socket_cliente.BeginReceive(recebe_do_cliente, 0, recebe_do_cliente.Length, SocketFlags.None, ReceiveCallback, socket_cliente);
        }

        private void Login(String requisicao, Socket socket_cliente)
        {
            byte[] resposta_ao_cliente = new byte[100];

            String[] chaves_requisicao = requisicao.Split(new char[] { ' ' });

            AdicionarComandoDataGridView("Admin", requisicao);

            if (chaves_requisicao.Length == 3)
            {
                //Primeiro valor é o comando login
                String login = chaves_requisicao[1];
                String senha = chaves_requisicao[2];

                if(usuarios_em_sessao.ContainsKey(login))
                {
                    resposta_ao_cliente = RetornaEmByteArray("Já existe uma sessão aberta no bate papo com este usuário.");
                    socket_cliente.BeginSend(resposta_ao_cliente, 0, resposta_ao_cliente.Length, SocketFlags.None, SendCallback, socket_cliente);
                    return;
                }

                //Resposta padrão
                resposta_ao_cliente = RetornaEmByteArray("usuarionaoexiste");

                foreach (Usuario usuario in usuarios_cadastrados)
                {
                    if (usuario.login.Equals(login))
                    {
                        if (usuario.senha.Equals(senha))
                        {
                            resposta_ao_cliente = RetornaEmByteArray("ok");
                        }
                        else
                        {
                            resposta_ao_cliente = RetornaEmByteArray("senhaerrada");
                        }

                        break;
                    }
                }
            }
            else
            {
                resposta_ao_cliente = RetornaEmByteArray("Erro ao formular comando.");
            }

            socket_cliente.BeginSend(resposta_ao_cliente, 0, resposta_ao_cliente.Length, SocketFlags.None, SendCallback, socket_cliente);
        }

        private void Logou(String requisicao, Socket socket_cliente)
        {
            byte[] resposta_ao_cliente = RetornaEmByteArray(requisicao);

            String usuario = requisicao.Split(new char[] { ' ' })[1];

            usuarios_em_sessao.Add(usuario, socket_cliente);

            AdicionarComandoDataGridView(usuario, requisicao);

            foreach (KeyValuePair<String, Socket> usuario_em_sessao in usuarios_em_sessao)
            {
                usuario_em_sessao.Value.BeginSend(resposta_ao_cliente, 0, resposta_ao_cliente.Length, SocketFlags.None, SendCallback, usuario_em_sessao.Value);
            }
        }

        private void Mensagem(String requisicao) 
        {
            byte[] resposta_ao_cliente = new byte[1024];

            resposta_ao_cliente = RetornaEmByteArray(requisicao);

            String usuario = requisicao.Split(new char[] { ' ' })[1];

            AdicionarComandoDataGridView(usuario, requisicao);

            foreach (KeyValuePair<String, Socket> usuario_em_sessao in usuarios_em_sessao)
            {
                usuario_em_sessao.Value.BeginSend(resposta_ao_cliente, 0, resposta_ao_cliente.Length, SocketFlags.None, SendCallback, usuario_em_sessao.Value);
            }
        }

        private void Exit(String requisicao)
        {
            byte[] resposta_ao_cliente = new byte[1024];

            resposta_ao_cliente = RetornaEmByteArray(requisicao);

            String usuario = requisicao.Split(new char[] { ' ' })[1];

            AdicionarComandoDataGridView(usuario, requisicao);

            usuarios_em_sessao.Remove(usuario);

            foreach (KeyValuePair<String, Socket> usuario_em_sessao in usuarios_em_sessao)
            {
                usuario_em_sessao.Value.BeginSend(resposta_ao_cliente, 0, resposta_ao_cliente.Length, SocketFlags.None, SendCallback, usuario_em_sessao.Value);
            }
        }

        public static byte[] RetornaEmByteArray(String mensagem)
        {
            return Encoding.ASCII.GetBytes(mensagem);
        }

        public static String RetornaEmString(byte[] vetor)
        {
            return Encoding.ASCII.GetString(vetor);
        }

        private void AdicionarComandoDataGridView(String usuario, String mensagem)
        {
            dgvComando.Invoke(new MethodInvoker(() =>
            {
                dgvComando.Rows.Add(usuario, mensagem);
            }));
        }

        private void btnEncerrar_Click(object sender, EventArgs e)
        {
            byte[] resposta_ao_cliente = new byte[8];

            resposta_ao_cliente = RetornaEmByteArray("shutdown");

            foreach (KeyValuePair<String, Socket> usuario_em_sessao in usuarios_em_sessao)
            {
                usuario_em_sessao.Value.BeginSend(resposta_ao_cliente, 0, resposta_ao_cliente.Length, SocketFlags.None, SendCallback, usuario_em_sessao.Value);
            }

            usuarios_em_sessao.Clear();
        }
    }
    
}
