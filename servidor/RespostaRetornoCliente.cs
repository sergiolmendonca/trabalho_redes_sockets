using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace servidor
{
    public class RespostaRetornoCliente
    {
        public TiposResposta TiposResposta { get; set; }
        public StatusResposta StatusResposta { get; set; }
        public string? RespostaQuestao { get; set; }
        public string? NickName { get; set; }
        public string? MensagemTexto { get; set; }
        public EndPoint? EndPointRemoto { get; set; }
    }
}