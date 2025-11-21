using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace servidor
{
    public class Resultado
    {
        public TipoResultado TipoResultado { get; set; }
        public bool Acertou { get; set; }
        public bool Primeiro { get; set; }
        public int Pontuacao { get; set; }
    }
}