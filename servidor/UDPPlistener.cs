using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace servidor
{
    public class UDPPlistener
    {
        private readonly int listenPort = 12345;
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
                    LoopJogo();
            }
        }

        private void LoopJogo()
        {
            if (ListaRespostas.Count == 0 && QuestaoAtual == null)
                EnviarProximaQuestão();
        }

        private void EnviarProximaQuestão()
        {
            if (ListaQuestoes.Count == 0)
                ListaQuestoes = new QuestoesService().GetQuestoes();

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

        private async Task EnviaMensagem(string mensagem, EndPoint endPointRemoto)
        {
            RespostaEnvioCliente resposta = new RespostaEnvioCliente()
            {
                TiposResposta = TiposResposta.MENSAGEM,
                MensagemTexto = mensagem  
            };

            byte[] bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(resposta));
            await Listener.SendToAsync(bytes, SocketFlags.None, endPointRemoto);
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

        private async Task IniciaJogador(string nickName, EndPoint endPointRemoto)
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