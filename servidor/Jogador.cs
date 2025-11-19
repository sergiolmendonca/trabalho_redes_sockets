using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace servidor
{
    public class Jogador
    {
        public Jogador(string nickName, EndPoint endPoint)
        {
            this.NickName = nickName;
            this.EndPoint = endPoint;
        }

        public string NickName { get; set; }
        public EndPoint EndPoint { get; set; }
        public int Pontuacao { get; set; }
        public int RepondeuAntes { get; set; }
        public int RespondeuDepois { get; set; }
        public int Acertou { get; set; }
        public int Errou { get; set; }
    }
}