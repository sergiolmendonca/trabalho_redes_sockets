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
        public Questao? Questão { get; set; }
        public string? MensagemTexto { get; set; }
    }
}