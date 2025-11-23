using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using cliente;

namespace clienteInterface;

public partial class TelaNick : UserControl
{
    private readonly MainWindow _janela;
    private readonly ClienteUDP clienteUDP;

    public TelaNick(MainWindow janela)
    {
        InitializeComponent();
        _janela = janela;
        this.clienteUDP = janela.Cliente;
    }

    private void Confirmar_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var nick = NickBox.Text ?? "";
        
        EnviarNick(nick);

        _janela.nickName = nick;
        _janela.NavegarPara(new TelaPergunta(_janela));
    }

    private async void EnviarNick(string nick)
    {
        await clienteUDP.EnviaNickName(nick);
    }
}