using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using cliente;

namespace clienteInterface;

public partial class MainWindow : Window
{

    public ClienteUDP Cliente = new ClienteUDP();
    public string nickName = "";
    
    public MainWindow()
    {
        InitializeComponent();

        TelaAtual.Content = new TelaInicial(this);
    }

    public void NavegarPara(UserControl tela)
    {
        TelaAtual.Content = tela;
    }

    private void ChangeTheme_Click(object? sender, RoutedEventArgs e)
    {
        var app = Application.Current;

    
        if (ChangeTheme.Content == "Dark Mode")
        {
            ChangeTheme.Content = "Light Mode";
            ChangeTheme.Background = new SolidColorBrush(Colors.White);
            ChangeTheme.Foreground = new SolidColorBrush(Colors.Black);
            app.Resources["TextoCor"] = Colors.White;
            this.Background = new SolidColorBrush(Color.Parse("#28282B"));
        }
        else
        {
            ChangeTheme.Content = "Dark Mode";
            ChangeTheme.Background = new SolidColorBrush(Colors.Black);
            ChangeTheme.Foreground = new SolidColorBrush(Colors.White);
            app.Resources["TextoCor"] = Colors.Black;
            this.Background = new SolidColorBrush(Colors.White);
        }
    }

}