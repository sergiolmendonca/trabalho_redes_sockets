using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using System;
using System.Threading.Tasks;
using cliente;
using Avalonia.Threading;
using System.Linq;

namespace clienteInterface;

public partial class TelaPergunta : UserControl
{
    private readonly MainWindow _janela;
    private readonly ClienteUDP clienteUDP;
    public int PontuacaoAtual = 0;

    public TelaPergunta(MainWindow janela)
    {
        InitializeComponent();
        _janela = janela;
        this.clienteUDP = janela.Cliente;
        EsperaResposta();
    }

    private async void EnviarResposta(string respostaSelecionada)
    {
        await clienteUDP.EnviaResposta(respostaSelecionada);
        EsperaResposta();
    }

    private void Responder_Click(object? sender, RoutedEventArgs e)
    {
        string resposta = "";

        if (OpA.IsChecked == true) resposta = "a";
        if (OpB.IsChecked == true) resposta = "b";
        if (OpC.IsChecked == true) resposta = "c";
        if (OpD.IsChecked == true) resposta = "d";

        if (!string.IsNullOrEmpty(resposta))
        {
            MostrarMensagem("Esperando o resultado...");
            EnviarResposta(resposta);
        }
    }

    private void NovoJogo_Click(object? sender, RoutedEventArgs e)
    {
        PontuacaoAtual = 0;
        this.clienteUDP?.EnviaNickName(_janela.nickName);
        MostrarMensagem("Aguardando oponente...");
        EsperaResposta();
    }

    private async void EsperaResposta()
    {
        bool continua = true;

        while (continua)
        {
            RespostaRetornoServidor? resposta = await clienteUDP.RecebeAsync();
            Console.WriteLine($"Resposta Retorno Servidor: {resposta?.TiposResposta}");

            switch (resposta?.TiposResposta)
            {
                case TiposResposta.PERGUNTA:
                    MontaPergunta(resposta?.Questao);
                    continua = false;
                    break;
                case TiposResposta.MENSAGEM:
                    MostrarMensagem(resposta.MensagemTexto);
                    break;
                case TiposResposta.RESULTADO:
                    MostrarResultado(resposta?.Resultado);
                    if (resposta?.Resultado?.TipoResultado == TipoResultado.FINAL) 
                        continua = false;
                    break;
            }
        }

        
    }

    private void MostrarResultado(Resultado? resultado)
    {
        if (resultado?.TipoResultado == TipoResultado.PARCIAL)
            MostrarResultadoParcial(resultado);
        else
            MostrarResultadoFinal(resultado);
    }

    private void MostrarResultadoParcial(Resultado resultado)
    {
        MensagensDeResultado msgmResultado = new();
        string mensagem = msgmResultado.GetMensagemResultado(resultado);

        PontuacaoAtual += resultado.Pontuacao;
        
        MostrarMensagem(mensagem);
    }

    private void MostrarResultadoFinal(Resultado? resultado)
    {
        if (resultado?.Pontuacao >= 30)
            MostrarMensagem("Fim de Jogo! \nVocê Venceu!");
        else
            MostrarMensagem("Fim de Jogo! \nVocê Perdeu!");
        
        MostrarStatisticasJogador(resultado?.Jogador, resultado?.Oponente);
    }

    private void MostrarStatisticasJogador(Jogador? jogador, Jogador? oponente)
    {
        Dispatcher.UIThread.Post(() =>
        {
            Estatisticas.IsVisible = true;
            NovoJogoBtn.IsVisible = true;
            Estatisticas.Text = @$"Suas estatísticas foram:
            Respostas Corretas: {jogador?.Acertou}
            Respostas Erradas: {jogador?.Errou}
            Respondeu Primeiro: {jogador?.RepondeuAntes}
            Respondeu Por Último: {jogador?.RespondeuDepois}

            Seu oponente {oponente?.NickName} fez {oponente?.Pontuacao} pontos.";
        });
    }

    private void MostrarMensagem(string? mensagemTexto)
    {
        Dispatcher.UIThread.Post(() =>
        {
            MensagemEspera.Text = mensagemTexto;
            MensagemEspera.IsVisible = true;

            Pontuacao.Text = $"Pontuação Atual: {PontuacaoAtual}";

            NovoJogoBtn.IsVisible = false;
            Estatisticas.IsVisible = false;
            TextoPergunta.IsVisible = false;
            OpcoesPanel.IsVisible = false;
            ResponderBtn.IsVisible = false;
        });
    }

    private void MontaPergunta(Questao? questao)
    {
        Dispatcher.UIThread.Post(() =>
        {
            MensagemEspera.IsVisible = false;

            TextoPergunta.IsVisible = true;
            OpcoesPanel.IsVisible = true;
            ResponderBtn.IsVisible = true;

            TextoPergunta.Text = questao?.Enunciado;

            OpA.Content = "A) " + questao?.Opcoes["a"];
            OpB.Content = "B) " + questao?.Opcoes["b"];
            OpC.Content = "C) " + questao?.Opcoes["c"];
            OpD.Content = "D) " + questao?.Opcoes["d"];
        });
    }
}