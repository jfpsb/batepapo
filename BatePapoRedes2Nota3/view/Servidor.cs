using BatePapoRedes2Nota3.controller;
using BatePapoRedes2Nota3.view;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace BatePapoRedes2Nota3
{
    public partial class Servidor : Form, IServidorView
    {
        public Servidor()
        {
            InitializeComponent();
        }

        private ServidorController servidorController;

        private void Servidor_Load(object sender, EventArgs e)
        {
            servidorController = new ServidorController(this);
            servidorController.IniciaServidor();
        }

        private void btnEncerrar_Click(object sender, EventArgs e)
        {
            servidorController.Shutdown();
        }

        public void AlteraStatus(String texto, Color cor)
        {
            labelActualStatus.Invoke(new MethodInvoker(() =>
            {
                labelActualStatus.Text = texto;
                labelActualStatus.ForeColor = cor;
            }));
        }

        public void AdicionarComandoDataGridView(string usuario, string mensagem)
        {
            dgvComando.Invoke(new MethodInvoker(() =>
            {
                dgvComando.Rows.Add(usuario, mensagem);
            }));
        }

        public void MensagemAoUsuario(string mensagem)
        {
            MessageBox.Show(mensagem);
        }
    }
    
}
