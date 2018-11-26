using System.Net.Sockets;

namespace BatePapoRedes2Nota3.data
{
    class Cliente
    {
        public Usuario usuario { get; set; } = new Usuario();
        public Socket socket { get; set; }
    }
}
