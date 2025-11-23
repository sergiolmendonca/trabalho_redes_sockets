using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace servidor
{
    public class UDPPlistener
    {
        private readonly int _listenPort;
        private readonly byte[] _buffer;
        private readonly Socket _listener;
        private List<Jogador> _listaJogadores;
        private List<Resposta> _listaRespostas;
        private List<Questao> _listaQuestoes;
        private bool _jogoIniciado;
        private Questao? _questaoAtual;


        public UDPPlistener()
        {
            _listener = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _buffer = new byte[1024];
            _listenPort = 10000;
            _listaJogadores = [];
            _listaRespostas = [];
            _listaQuestoes = [];
            _jogoIniciado = false;
        }

        public async Task StartListener()
        {
            
            AbreConexao();

            while (true)
            {
                Mensagem mensagem = new();
                

                var recebido = await RecebeMensagem();
                mensagem.ConverterMensagemRecebida(recebido, _buffer);

                await ProcessaRetorno(mensagem.RespostaRetornoCliente);
                
                if (_jogoIniciado)
                    await LoopJogo();
            }
        }

        private async Task LoopJogo()
        {
            if (_listaRespostas.Count == 0 && _questaoAtual == null)
                await EnviarProximaQuestão();
            
            if (_listaRespostas.Count == 2) 
            {
                Console.WriteLine("Loop, lista com 2 respostas");
                if (_questaoAtual?.Equals(_listaRespostas?.FirstOrDefault()?.Questao) ?? false)
                {
                    await ValidaEnviaRespostas();
                    if (_jogoIniciado)
                    {
                        await EnviarMensagemPreparacao();
                        await EnviarProximaQuestão();
                    }
                }
                else 
                    _listaRespostas = new List<Resposta>();
            }
        }

        private async Task EnviarMensagemPreparacao()
        {
            RespostaEnvioCliente respostaEnvio = new RespostaEnvioCliente()
            {
                TiposResposta = TiposResposta.MENSAGEM,
                MensagemTexto = "Prepare-se para próxima pergunta..."  
            };
            await EnviarParaTodos(respostaEnvio);
            await Task.Delay(3000);
        }

        private async Task ValidaEnviaRespostas()
        {
            var primeiraResposta = _listaRespostas.Where(x => x.PrimeiraResposta).FirstOrDefault();
            var segundaResposta = _listaRespostas.Where(x => !x.PrimeiraResposta).FirstOrDefault();
            StatusResultado statusPrimeiraResposta = StatusResultado.ERROU;
            StatusResultado statusSegundaResposta = StatusResultado.ERROU;

            Console.WriteLine("Veio validar as resposta...");

            if (_questaoAtual?.Correta.ToUpper().Equals(primeiraResposta?.RespostaTexto?.ToUpper()) ?? false)
            {
                statusPrimeiraResposta = StatusResultado.ACERTOU_ANTES;
                if (_questaoAtual.Correta.ToUpper().Equals(segundaResposta?.RespostaTexto?.ToUpper()))
                    statusSegundaResposta = StatusResultado.ACERTOU_SEM_PONTUACAO;
                else
                    statusSegundaResposta = StatusResultado.ERROU;
            }
            else if (_questaoAtual?.Correta.ToUpper().Equals(segundaResposta?.RespostaTexto?.ToUpper()) ?? false)
            {
                statusPrimeiraResposta = StatusResultado.ERROU;
                statusSegundaResposta = StatusResultado.ACERTOU_DEPOIS;
            }

            await ComputaPontosEnviaResultado(primeiraResposta?.Jogador, statusPrimeiraResposta);
            await ComputaPontosEnviaResultado(segundaResposta?.Jogador, statusSegundaResposta);

            await Task.Delay(4000);

            if (_listaJogadores[0].Pontuacao >= 30 || _listaJogadores[1].Pontuacao >= 30)
                await EncerraJogo();
        }

        private async Task EncerraJogo()
        {
            foreach (var jogador in _listaJogadores)
            {
                Resultado resultado = new Resultado()
                {
                    TipoResultado = TipoResultado.FINAL,
                    Pontuacao = jogador.Pontuacao,
                    Jogador = jogador,
                    Oponente = _listaJogadores.Where(x => !x.Equals(jogador)).FirstOrDefault()
                };

                await EnviaResultado(resultado, jogador);
            }

            _questaoAtual = null;
            _listaQuestoes = new List<Questao>();
            _listaRespostas = new List<Resposta>();
            _listaJogadores = new List<Jogador>();
            _jogoIniciado = false;
        }

        private async Task ComputaPontosEnviaResultado(Jogador jogador, StatusResultado statusResultado)
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
                Pontuacao = pontuacaoRodada,
                RespostaCorreta = _questaoAtual?.Correta
            };

            await EnviaResultado(resultado, jogador);
        }

        private async Task EnviaResultado(Resultado resultado, Jogador jogador)
        {
            RespostaEnvioCliente respostaEnvio = new RespostaEnvioCliente()
            {
                TiposResposta = TiposResposta.RESULTADO,
                Resultado = resultado  
            };

            await Enviar(respostaEnvio, jogador?.EndPoint);
        }

        private async Task EnviarProximaQuestão()
        {
            CarregarProximaQuestao();
            _listaRespostas = new List<Resposta>();

            RespostaEnvioCliente respostaEnvio = new RespostaEnvioCliente()
            {
                TiposResposta = TiposResposta.PERGUNTA,
                Questao = _questaoAtual  
            };

            await EnviarParaTodos(respostaEnvio);
        }

        private void CarregarProximaQuestao()
        {
            if (_listaQuestoes.Count == 0)
            {
                _listaQuestoes = new QuestoesService().GetQuestoes();
                _questaoAtual = _listaQuestoes[0];
            }
            else 
            {
                int proximoIndex = _listaQuestoes.IndexOf(_questaoAtual) + 1;
                if (proximoIndex < _listaQuestoes.Count)
                    _questaoAtual = _listaQuestoes[proximoIndex];
            }
        }

        private void AbreConexao()
        {
            IPEndPoint local = new(IPAddress.Any, _listenPort);

            _listener.Bind(local);
            
            Console.WriteLine($"Conexão aberta! \n");
            Console.WriteLine($"Esperando participantes... \n");
        }

        private async Task<SocketReceiveFromResult> RecebeMensagem()
        {
            var endPointRemoto = new IPEndPoint(IPAddress.Any, 0);

            return await _listener.ReceiveFromAsync(_buffer, SocketFlags.None, endPointRemoto);
        }

        private async Task EnviaMensagem(string mensagem, EndPoint endPointRemoto)
        {
            RespostaEnvioCliente resposta = new RespostaEnvioCliente()
            {
                TiposResposta = TiposResposta.MENSAGEM,
                MensagemTexto = mensagem  
            };

            await Enviar(resposta, endPointRemoto);
        }

        private async Task Enviar(RespostaEnvioCliente respostaEnvio, EndPoint endPointRemoto)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(respostaEnvio));
            await _listener.SendToAsync(bytes, SocketFlags.None, endPointRemoto);
        }

        private async Task EnviarParaTodos(RespostaEnvioCliente respostaEnvio)
        {
            foreach(var jogador in _listaJogadores)
                await Enviar(respostaEnvio, jogador.EndPoint);
        }

        private async Task ProcessaRetorno(RespostaRetornoCliente? retornoCliente)
        {
            Console.WriteLine($"Recebi alguma coisa => {retornoCliente?.TiposResposta}");
            if (retornoCliente != null)
            {
                switch (retornoCliente.TiposResposta)
                {
                    case TiposResposta.NICKNAME:
                        await IniciaJogador(retornoCliente.NickName, retornoCliente?.EndPointRemoto);
                        break;
                    case TiposResposta.RESPOSTA:
                        Console.WriteLine($"{retornoCliente.RespostaQuestao} de {retornoCliente.EndPointRemoto}");
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
            if (_listaRespostas.Count == 0) primeiraResposta = true;

            _listaRespostas.Add(new Resposta()
            {
                RespostaTexto = respostaQuestao,
                PrimeiraResposta = primeiraResposta,
                Jogador = _listaJogadores.Where(x => x.EndPoint.Equals(endPointRemoto)).FirstOrDefault(),
                Questao = _questaoAtual
            });
            
            Console.WriteLine($"Agora a lista de resposta está com {_listaRespostas.Count}");
        }

        private async Task IniciaJogador(string? nickName, EndPoint endPointRemoto)
        {
            if (_listaJogadores.Count < 2)
            {
                Jogador jogador = new Jogador(nickName, endPointRemoto);
                _listaJogadores.Add(jogador);

                if (_listaJogadores.Count == 2)
                    await ComecarJogo();
                else
                    await EnviaMensagem("Aguardando oponente...", endPointRemoto);
                
            }
        }

        private async Task ComecarJogo()
        {
            _jogoIniciado = true;
            foreach (var jogador in _listaJogadores)
            {
                await EnviaMensagem("Todos participantes estão prontos! prepare-se!", jogador?.EndPoint);
            }

            await Task.Delay(3000);
        }
    }
}