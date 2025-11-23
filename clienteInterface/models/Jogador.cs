using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace cliente
{
    public class Jogador
    {
        public string? NickName { get; set; }
        public int Pontuacao { get; set; }
        public int RepondeuAntes { get; set; }
        public int RespondeuDepois { get; set; }
        public int Acertou { get; set; }
        public int Errou { get; set; }
    }
}