using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace servidor
{
    public class Program
    {
        public static async Task Main()
        {
            await new UDPPlistener().StartListener();
        }
    }
}