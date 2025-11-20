using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace servidor
{
    public class Mensagem
    {
        public RespostaEnvioCliente? RespostaEnvioCliente { get; set; }
        public RespostaRetornoCliente? RespostaRetornoCliente { get; set; }

        public RespostaRetornoCliente ConverterMensagemRecebida(SocketReceiveFromResult socketReceived, byte[] buffer)
        {
            string mensagemRecebida = Encoding.UTF8.GetString(buffer, 0, socketReceived.ReceivedBytes);

            this.RespostaRetornoCliente = JsonSerializer
                .Deserialize<RespostaRetornoCliente>(mensagemRecebida) ?? new RespostaRetornoCliente();

            this.RespostaRetornoCliente.EndPointRemoto = socketReceived.RemoteEndPoint;

            return this.RespostaRetornoCliente;
        }

        public void EnviaMensagem()
        {
            
        }
    }
}