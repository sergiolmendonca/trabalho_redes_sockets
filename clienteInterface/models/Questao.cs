using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cliente
{
    public class Questao
    {
        public int Id { get; set; }
        public required string Enunciado { get; set; }
        public required Dictionary<string, string> Opcoes { get; set; }
        public required string Correta { get; set; }
    }
}