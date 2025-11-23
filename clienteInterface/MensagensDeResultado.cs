using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cliente;

namespace clienteInterface
{
    public class MensagensDeResultado
    {
        private List<string> _acertouPrimeiro = new List<string>() 
            { "Você foi o gatilho mais rápido e certeiro desta vez cowboy", 
            "Parece que temos um relâmpago marquinhos aqui", "Parabéns jovem, você é rápido...", "Um verdadeiro Alpha, não deixou nada pro beta!",
            "Parabéns! Se estivesse valendo dinheiro eim...", "Parabéns! Acertou! E Primeiro!", "Você foi rápido rapaz... Mas acho que essa tava fácil mesmo.",
            "Acho que o Luciano poderia te dar um ponto por essa questão eim..."};
        private List<string> _acertouSegundo = new List<string>()
            { "Parabéns, você acertou! Mas foi meio lento eim", "Sorte a sua que seu oponente não é lá essas coisas", 
            "Você acertou! Mas só pq o oponente é afobado demais pra pensar", "Parabéns! Se você estivesse jogando sozinho(a) seria imbatível.",
            "O gatilho mais lento do sul... Mas acertou...", "Sorte sua eim! Acho que teu adversário(a) merece um obrigado",
            "É... Não são 5 pontos, mas dá pro gasto"};
        private List<string> _acertouSemPonto = new List<string>()
            { "Até acertou, mas teu oponente foi mais rápido(a).", "Pra responder nessa lentidão, seria melhor nem ter acertado", 
            "Infelizmente vai ficar sem ponto, lento demais...", "Pelo menos ta sabendo as respostas...", "É... Dessa vez não deu...",
            "Reflexos de um gato.. morto...", "Não adianta saber e chegar por último né.", "Você acertou!!! Mas não pontuou!!!",
            "Na próxima vai, só ficar atento.", "Desanima não, ele(a) que é rápido(a)..."};
        private List<string> _errou = new List<string>()
            { "Rapaz... Melhor estudar eim...", "Vish! Essa nem tava tão difícil", "Ta sabendo que ainda tem uma prova né?", 
            "Tudo bem amigo(a), umas a gente perde e outras ganham da gente.", "Parabéns!!!!! Você errou!!!!", "Essa tava difícil mesmo, relaxa...",
            "Será que não consegue umas aulinhas particulares com o Luciano?", "Errou... Faz parte...", "Adianta falar alguma coisa?", 
            "A vida não é moranguinho né...", "Quase eim, se fosse uma competição de errar..."};

        private string GetMensagemAcertoPrimeiro()
        {
            var rnd = new Random();
            return _acertouPrimeiro.OrderBy(x => rnd.Next()).FirstOrDefault() ?? "";
        }

        private string GetMensagemAcertoSegundo()
        {
            var rnd = new Random();
            return _acertouSegundo.OrderBy(x => rnd.Next()).FirstOrDefault() ?? "";
        }

        private string GetMensagemAcertouMasNaoPontuou()
        {
            var rnd = new Random();
            return _acertouSemPonto.OrderBy(x => rnd.Next()).FirstOrDefault() ?? "";
        }

        private string GetMensagemErrou()
        {
            var rnd = new Random();
            return _errou.OrderBy(x => rnd.Next()).FirstOrDefault() ?? "";
        }

        public string GetMensagemResultado(Resultado resultado)
        {
            string mensagem;
            if (resultado.Acertou && resultado.Primeiro)
                mensagem = GetMensagemAcertoPrimeiro();
            else if (resultado.Acertou && resultado.Pontuacao > 0)
                mensagem = GetMensagemAcertoSegundo();
            else if (resultado.Acertou && resultado.Pontuacao == 0)
                mensagem = GetMensagemAcertouMasNaoPontuou();
            else 
                mensagem = GetMensagemErrou();

            mensagem = @$"{mensagem}
            Pontuação na rodada {resultado.Pontuacao}
            Alternativa correta: {resultado?.RespostaCorreta?.ToUpper()}";

            return mensagem;
        }
    }
}