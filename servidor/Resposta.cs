using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace servidor
{
    public class Resposta
    {
        public bool PrimeiraResposta { get; set; }
        public Jogador Jogador { get; set; }
        public string RespostaTexto { get; set; }
        public Questao Questao { get; set; }
    }
}