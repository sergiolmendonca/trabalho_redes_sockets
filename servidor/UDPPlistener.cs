using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace servidor
{
    public class UDPPlistener
    {
        private readonly int listenPort = 10000;
        private readonly byte[] buffer = new byte[1024];
        private List<Jogador> ListaJogadores = new List<Jogador>();
        private List<Resposta> ListaRespostas = new List<Resposta>();
        private List<Questao> ListaQuestoes = new List<Questao>();
        private Socket Listener;
        private bool jogoIniciado = false;
        private Questao? QuestaoAtual;

        public async Task StartListener()
        {
            
            AbreConexao();

            while (true)
            {
                Mensagem mensagem = new Mensagem();
                

                var recebido = await RecebeMensagem();
                mensagem.ConverterMensagemRecebida(recebido, buffer);

                await ProcessaRetorno(mensagem.RespostaRetornoCliente);

                if (jogoIniciado)
                    await LoopJogo();
            }
        }

        private async Task LoopJogo()
        {
            if (ListaRespostas.Count == 0 && QuestaoAtual == null)
                await EnviarProximaQuestão();
            
            if (ListaRespostas.Count == 2) 
            {
                if (QuestaoAtual.Equals(ListaRespostas.FirstOrDefault()))
                    await ValidaEnviaRespostas();
                else 
                    ListaRespostas = new List<Resposta>();
            }
        }

        private async Task ValidaEnviaRespostas()
        {
            var primeiraResposta = ListaRespostas.Where(x => x.PrimeiraResposta).FirstOrDefault();
            var segundaResposta = ListaRespostas.Where(x => !x.PrimeiraResposta).FirstOrDefault();

            if (QuestaoAtual.Correta.Equals(primeiraResposta.RespostaTexto))
            {
                await ComputaPontosEnviaResultado(primeiraResposta.Jogador, StatusResultado.ACERTOU_ANTES);
                if (QuestaoAtual.Correta.Equals(segundaResposta.RespostaTexto))
                    await ComputaPontosEnviaResultado(segundaResposta.Jogador, StatusResultado.ACERTOU_SEM_PONTUACAO);
            }
            else if (QuestaoAtual.Correta.Equals(segundaResposta.RespostaTexto))
            {
                await ComputaPontosEnviaResultado(primeiraResposta.Jogador, StatusResultado.ERROU);
                await ComputaPontosEnviaResultado(segundaResposta.Jogador, StatusResultado.ACERTOU_DEPOIS);
            }
            else
            {
                await ComputaPontosEnviaResultado(primeiraResposta.Jogador, StatusResultado.ERROU);
                await ComputaPontosEnviaResultado(segundaResposta.Jogador, StatusResultado.ERROU);
            }
        }

        private async Task ComputaPontosEnviaResultado(Jogador? jogador, StatusResultado statusResultado)
        {
            int pontuacaoRodada = 0;
            bool acertou = false;
            bool primeiro = false;
            switch (statusResultado)
            {
                case StatusResultado.ACERTOU_ANTES:
                    pontuacaoRodada = 5;
                    acertou = true;
                    primeiro = true;
                    break;
                case StatusResultado.ACERTOU_DEPOIS:
                    pontuacaoRodada = 3;
                    acertou = true;
                    break;
                case StatusResultado.ACERTOU_SEM_PONTUACAO:
                    acertou = true;
                    break;
                case StatusResultado.ERROU:
                    jogador.Errou++;
                    break;
                default:
                    break;
            }
            
            jogador.Pontuacao += pontuacaoRodada;
            jogador.Acertou += acertou ? 1 : 0;
            jogador.RepondeuAntes += primeiro ? 1 : 0;
            jogador.RespondeuDepois += primeiro ? 0 : 1;

            Resultado resultado = new Resultado()
            {
                TipoResultado = TipoResultado.PARCIAL,
                Acertou = acertou,
                Primeiro = primeiro,
                Pontuacao = pontuacaoRodada
            };

            await EnviaResultado(resultado, jogador);
        }

        private async Task EnviaResultado(Resultado resultado, Jogador? jogador)
        {
            RespostaEnvioCliente respostaEnvio = new RespostaEnvioCliente()
            {
                TiposResposta = TiposResposta.RESULTADO,
                Resultado = resultado  
            };

            await Enviar(respostaEnvio, jogador.EndPoint);
        }

        private void ComputaPontosPrimeiroJogadorAcertou(Jogador? jogador)
        {
            jogador.Pontuacao = jogador.Pontuacao + 5;
            jogador.Acertou++;
            jogador.RepondeuAntes++;
        }

        private void ComputaPontosSegundoJogadorAcertou(Jogador? jogador)
        {
            jogador.Pontuacao = jogador.Pontuacao + 3;
            jogador.Acertou++;
            jogador.RespondeuDepois++;
        }

        private async Task EnviarProximaQuestão()
        {
            CarregarProximaQuestao();

            RespostaEnvioCliente respostaEnvio = new RespostaEnvioCliente()
            {
                TiposResposta = TiposResposta.PERGUNTA,
                Questao = QuestaoAtual  
            };

            await EnviarParaTodos(respostaEnvio);
        }

        private void CarregarProximaQuestao()
        {
            if (ListaQuestoes.Count == 0)
            {
                ListaQuestoes = new QuestoesService().GetQuestoes();
                QuestaoAtual = ListaQuestoes[0];
            }
            else 
            {
                int proximoIndex = ListaQuestoes.IndexOf(QuestaoAtual) + 1;
                if (proximoIndex < ListaQuestoes.Count)
                    QuestaoAtual = ListaQuestoes[proximoIndex];
            }
        }

        private async void AbreConexao()
        {
            Listener = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            
            IPEndPoint local = new(IPAddress.Any, listenPort);

            Listener.Bind(local);
            
            Console.WriteLine($"Conexão aberta! \n");
            Console.WriteLine($"Esperando participantes... \n");
        }

        private async Task<SocketReceiveFromResult> RecebeMensagem()
        {
            var endPointRemoto = new IPEndPoint(IPAddress.Any, 0);

            return await Listener.ReceiveFromAsync(buffer, SocketFlags.None, endPointRemoto);
        }

        private async Task EnviaMensagem(string mensagem, EndPoint? endPointRemoto)
        {
            RespostaEnvioCliente resposta = new RespostaEnvioCliente()
            {
                TiposResposta = TiposResposta.MENSAGEM,
                MensagemTexto = mensagem  
            };

            await Enviar(resposta, endPointRemoto);
        }

        private async Task Enviar(RespostaEnvioCliente respostaEnvio, EndPoint? endPointRemoto)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(respostaEnvio));
            await Listener.SendToAsync(bytes, SocketFlags.None, endPointRemoto);
        }

        private async Task EnviarParaTodos(RespostaEnvioCliente respostaEnvio)
        {
            foreach(var jogador in ListaJogadores)
                await Enviar(respostaEnvio, jogador.EndPoint);
        }

        private async Task ProcessaRetorno(RespostaRetornoCliente? retornoCliente)
        {
            if (retornoCliente != null)
            {
                switch (retornoCliente.TiposResposta)
                {
                    case TiposResposta.NICKNAME:
                        await IniciaJogador(retornoCliente.NickName, retornoCliente.EndPointRemoto);
                        break;
                    case TiposResposta.RESPOSTA:
                        ProcessaResposta(retornoCliente.RespostaQuestao, retornoCliente.EndPointRemoto);
                        break;
                    case TiposResposta.MENSAGEM:
                        Console.WriteLine(retornoCliente.MensagemTexto);
                        break;
                    default:
                        Console.WriteLine("nada");
                        break;
                }
            }
        }

        private void ProcessaResposta(string? respostaQuestao, EndPoint? endPointRemoto)
        {
            bool primeiraResposta = false;
            if (ListaRespostas.Count == 0) primeiraResposta = true;

            ListaRespostas.Add(new Resposta()
            {
                RespostaTexto = respostaQuestao,
                PrimeiraResposta = primeiraResposta,
                Jogador = ListaJogadores.Where(x => x.EndPoint.Equals(endPointRemoto)).FirstOrDefault(),
                Questao = QuestaoAtual
            });
            
        }

        private async Task IniciaJogador(string? nickName, EndPoint? endPointRemoto)
        {
            if (ListaJogadores.Count < 2)
            {
                Jogador jogador = new Jogador(nickName, endPointRemoto);
                ListaJogadores.Add(jogador);

                if (ListaJogadores.Count == 2)
                    await ComecarJogo();
                else
                    await EnviaMensagem("Aguardando oponente...", endPointRemoto);
                
            }
        }

        private async Task ComecarJogo()
        {
            jogoIniciado = true;
            foreach (var jogador in ListaJogadores)
            {
                await EnviaMensagem("Todos participantes estão prontos! prepare-se!", jogador.EndPoint);
            }

            await Task.Delay(3000);
        }
    }
}