using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace BatePapoRedes2Nota3
{
    public partial class BatePapo : Form
    {
        public BatePapo()
        {
            InitializeComponent();
        }

        private Thread threadLogin;
        public String Login_Usuario;

        private byte[] recebido_do_servidor = new byte[1024];

        private Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private void BatePapo_Load(object sender, EventArgs e)
        {
            Text += Login_Usuario;

            threadLogin = new Thread(AbrirOutroBatePapo);

            socket.Connect(IPAddress.Loopback, 3500);

            String requisicao_do_cliente = "logou " + Login_Usuario;

            socket.BeginSend(Servidor.RetornaEmByteArray(requisicao_do_cliente), 0, Servidor.RetornaEmByteArray(requisicao_do_cliente).Length, SocketFlags.None, SendCallback, socket);
        }

        private void SendCallback(IAsyncResult asyncResult)
        {
            Socket socket = (Socket)asyncResult.AsyncState;
            socket.EndSend(asyncResult);

            socket.BeginReceive(recebido_do_servidor, 0, recebido_do_servidor.Length, SocketFlags.None, ReceiveCallback, socket);
        }

        private void ReceiveCallback(IAsyncResult asyncResult)
        {
            Socket socket = (Socket)asyncResult.AsyncState;
            int index = socket.EndReceive(asyncResult);

            byte[] recebido_do_servidor_tamanho_exato = new byte[index];

            Array.Copy(recebido_do_servidor, recebido_do_servidor_tamanho_exato, index);

            String comando = Servidor.RetornaEmString(recebido_do_servidor_tamanho_exato).Split(new char[] { ' ' })[0];

            switch(comando)
            {
                case "logou":
                    Logou(Servidor.RetornaEmString(recebido_do_servidor_tamanho_exato));
                    break;
                case "msg":
                    Mensagem(Servidor.RetornaEmString(recebido_do_servidor_tamanho_exato));
                    break;
                case "exit":
                    Exit(Servidor.RetornaEmString(recebido_do_servidor_tamanho_exato));
                    break;
                default:
                    break;
            }

            socket.BeginReceive(recebido_do_servidor, 0, recebido_do_servidor.Length, SocketFlags.None, ReceiveCallback, socket);
        }

        private void Mensagem(String requisicao)
        {
            String mensagem = requisicao.Remove(0, 4);

            String usuario = mensagem.Split(new char[] { ' ' })[0];
            mensagem = mensagem.Remove(0, usuario.Length + 1);

            AdicionarComandoDataGridView(usuario, mensagem);
        }

        private void Exit(String requisicao)
        {
            String usuario = requisicao.Split(new char[] { ' ' })[1];

            String exit_msg = "Usuário " + usuario + " saiu da sala de bate papo!";

            AlterarTextoTxtAviso(exit_msg);
        }

        private void Logou(String requisicao)
        {
            String usuario = requisicao.Split(new char[] { ' ' })[1];

            String logou_msg = "Usuário " + usuario + " entrou na sala de bate papo!";

            AlterarTextoTxtAviso(logou_msg);
        }

        private void abrirOutroBatePapoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            threadLogin = new Thread(AbrirOutroBatePapo);
            threadLogin.Start();
        }

        private static void AbrirOutroBatePapo()
        {
            Login telaLogin = new Login();
            DialogResult dialogResult = telaLogin.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                BatePapo batePapo = new BatePapo();
                batePapo.Login_Usuario = telaLogin.LoginUsuario();
                batePapo.ShowDialog();
            }
            else
            {
                telaLogin.Close();
            }
        }

        private void AlterarTextoTxtAviso(String texto)
        {
            txtAviso.Invoke(new MethodInvoker(() =>
            {
                txtAviso.Text = texto;
            }));
        }

        private void BatePapo_FormClosing(object sender, FormClosingEventArgs e)
        {
            socket.BeginSend(Servidor.RetornaEmByteArray("exit " + Login_Usuario), 0, Servidor.RetornaEmByteArray("exit " + Login_Usuario).Length, SocketFlags.None, SendCallback, socket);
        }

        private void btnEnviar_Click(object sender, EventArgs e)
        {
            String mensagem = txtMensagem.Text.Trim(new char[] { ' ', '\r', '\n' });
            String requisicao_do_cliente = null;

            txtMensagem.Clear();

            if(mensagem != String.Empty)
            {
                switch(mensagem)
                {
                    case "exit":
                        requisicao_do_cliente = "exit " + Login_Usuario;
                        Close();
                        break;
                    default:
                        requisicao_do_cliente = "msg " + Login_Usuario + " " + mensagem;
                        break;
                }

                socket.BeginSend(Servidor.RetornaEmByteArray(requisicao_do_cliente), 0, Servidor.RetornaEmByteArray(requisicao_do_cliente).Length, SocketFlags.None, SendCallback, socket);
            }
        }

        private void txtMensagem_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                btnEnviar_Click(sender, e);
            }
        }

        private void AdicionarComandoDataGridView(String usuario, String mensagem)
        {
            dgvBatePapo.Invoke(new MethodInvoker(() =>
            {
                dgvBatePapo.Rows.Add(usuario, mensagem);
                dgvBatePapo.FirstDisplayedScrollingRowIndex = dgvBatePapo.Rows.Count - 1;
            }));
        }
    }
}
