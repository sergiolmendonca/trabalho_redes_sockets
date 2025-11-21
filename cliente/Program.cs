using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace cliente
{
    public class Program
    {
        public static async Task Main()
        {
            ClienteUDP udpCliente = new ClienteUDP();

            Console.WriteLine("Para iniciar o jogo informe seu nickname:");
            string nickname = Console.ReadLine() ?? $"player{new Random().ToString()}";

            await udpCliente.EnviaNickName(nickname);

            while (true)
            {
                RespostaRetornoServidor retornoServidor = await udpCliente.RecebeAsync();
                Console.WriteLine(retornoServidor.MensagemTexto);
            }
        }
    }
}


