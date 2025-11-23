using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cliente
{
    public class Resultado
    {
        public TipoResultado TipoResultado { get; set; }
        public bool Acertou { get; set; }
        public bool Primeiro { get; set; }
        public int Pontuacao { get; set; }
        public string? RespostaCorreta { get; set; }
        public Jogador? Jogador { get; set; }
        public Jogador? Oponente { get; set; }
    }
}