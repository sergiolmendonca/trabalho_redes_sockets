using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace clienteInterface;

public partial class TelaInicial : UserControl
{
    private readonly MainWindow _janela;

    public TelaInicial(MainWindow janela)
    {
        InitializeComponent();
        _janela = janela;
    }

    private void Iniciar_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _janela.NavegarPara(new TelaNick(_janela));
    }
}