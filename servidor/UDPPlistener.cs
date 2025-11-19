using System.Net;
using System.Net.Sockets;
using System.Text;

namespace servidor
{
    public class UDPPlistener
    {
        private readonly int listenPort = 12345;
        private readonly byte[] buffer = new byte[1024];
        private List<Jogador> ListaJogadores = new List<Jogador>();
        private Socket Listener;

        public async Task StartListener()
        {
            
            AbreConexao();

            while (true)
            {
                var receivedBytes = await RecebeMensagem();

                if (ListaJogadores.Count < 2) 
                    IniciaJogador(receivedBytes);
                else 
                    EnviaMensagem("Acabou!", receivedBytes.RemoteEndPoint);
            }
        }

        private async void AbreConexao()
        {
            Listener = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            
            IPEndPoint local = new(IPAddress.Any, listenPort);

            Listener.Bind(local);
            
            Console.WriteLine($"Conexão aberta! \n");
            Console.WriteLine($"Esperando participantes... \n");
        }

        private async Task<SocketReceiveFromResult> RecebeMensagem()
        {
            

            EndPoint endPointRemoto = new IPEndPoint(IPAddress.Any, 0);

            var receive = await Listener.ReceiveFromAsync(buffer, SocketFlags.None, endPointRemoto);

            string mensagemRecebida = Encoding.UTF8.GetString(buffer, 0, receive.ReceivedBytes);

            Console.WriteLine($"Mensagem recebida: {mensagemRecebida}");

            return receive;
        }

        private async void EnviaMensagem(string mensagem, EndPoint destino)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(mensagem);
            await Listener.SendToAsync(bytes, SocketFlags.None, destino);
        }

        private void IniciaJogador(SocketReceiveFromResult socketReceived)
        {
            EndPoint endPointRemoto = socketReceived.RemoteEndPoint;

            string mensagemRecebida = Encoding.UTF8.GetString(buffer, 0, socketReceived.ReceivedBytes);

            if (!mensagemRecebida.Contains("NICKNAME:"))
                EnviaMensagem("Mensagem inválida, Jogador não iniciado!", endPointRemoto);
            else
            {
                ListaJogadores.Add(new Jogador(mensagemRecebida.Replace("NICKNAME:", ""), endPointRemoto));
                EnviaMensagem("[OK] \n Jogador cadastrado! \n Aguardando início do Jogo... ", endPointRemoto);
            }
        }
    }
}