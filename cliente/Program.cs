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
            var client = new UdpClient();

            RespostaEnvioServidor respostaEnvioServidor = new RespostaEnvioServidor()
            {
                TiposResposta = TiposResposta.NICKNAME,
                NickName = "nick"
            };

            string mensagem = JsonSerializer.Serialize(respostaEnvioServidor);

            var bytes = Encoding.UTF8.GetBytes(mensagem);

            await client.SendAsync(bytes, bytes.Length, "localhost", 12345);

            var result = await client.ReceiveAsync();

            Console.WriteLine("Resposta: " + Encoding.UTF8.GetString(result.Buffer));

            while (true)
            {
                var leitura = Console.ReadLine();

                bytes = Encoding.UTF8.GetBytes(leitura);

                await client.SendAsync(bytes, bytes.Length, "localhost", 12345);

                result = await client.ReceiveAsync();

                Console.WriteLine("Resposta: " + Encoding.UTF8.GetString(result.Buffer));
            }
        }
    }
}


