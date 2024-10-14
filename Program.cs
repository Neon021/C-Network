using System.Net.Sockets;
using System.Net;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        Listen();
    }

    public static async void Listen()
    {
        IPEndPoint iPEndPoint = new(IPAddress.Any, 6379);
        using Socket socket = new(iPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        {
            await socket.ConnectAsync(iPEndPoint);
            byte[] pong = Encoding.UTF8.GetBytes("+PONG\r\n");

            while (true)
            {
                byte[] buffer = new byte[1024];
                _ = socket.ReceiveAsync(buffer, SocketFlags.None);

                await socket.SendAsync(pong);
            }
        }
    }
}
