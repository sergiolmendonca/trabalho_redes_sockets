using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace cliente
{
    public class ClienteUDP
    {
        private readonly Socket _socket;
        private readonly EndPoint _endpoint;
        private readonly string _ipServidor = "127.0.0.1";
        private readonly int _porta = 10000;
        private readonly byte[] _buffer = new byte[1024];

        public ClienteUDP()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _endpoint = new IPEndPoint(IPAddress.Parse(_ipServidor), _porta);
        }

        public async Task EnviaNickName(string? nickname)
        {
            RespostaEnvioServidor respostaEnvio = new RespostaEnvioServidor()
            {
                TiposResposta = TiposResposta.NICKNAME,
                NickName = nickname  
            };

            await Enviar(respostaEnvio);
        }

        public async Task Enviar(RespostaEnvioServidor respostaEnvio)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(respostaEnvio));
            await _socket.SendToAsync(bytes, SocketFlags.None, _endpoint);
        }

        public async Task<RespostaRetornoServidor?> RecebeAsync()
        {
            EndPoint remoto = new IPEndPoint(IPAddress.Any, 0);

            var recebido = await _socket.ReceiveFromAsync(_buffer, SocketFlags.None, remoto);
            string recebidoTexto = Encoding.UTF8.GetString(_buffer, 0, recebido.ReceivedBytes);

            return JsonSerializer.Deserialize<RespostaRetornoServidor>(recebidoTexto);
        }
    }
}