using System.Net;
using System.Net.Sockets;
using System.Text;

var client = new UdpClient();

string mensagem = "Jogador 1 <|EOM|>";
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

