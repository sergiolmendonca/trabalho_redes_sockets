using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;

namespace cliente
{
    public class RespostaRetornoServidor
    {
        public TiposResposta TiposResposta { get; set; }
        public StatusResposta StatusResposta { get; set; }
        public Questao? Questao { get; set; }
        public Resultado? Resultado { get; set; }

        public string? MensagemTexto { get; set; }
    }
}