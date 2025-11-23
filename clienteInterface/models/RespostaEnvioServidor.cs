using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cliente
{
    public class RespostaEnvioServidor
    {
        public TiposResposta TiposResposta { get; set; }
        public StatusResposta StatusResposta { get; set; }
        public string? RespostaQuestao { get; set; }
        public string? NickName { get; set; }
    }
}