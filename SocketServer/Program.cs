using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace SocketServer
{
    class Program
    {
        static Encoding EncodingIso = Encoding.GetEncoding("ISO-8859-1");

        static void Main(string[] args)
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse("192.168.0.197"), 8081);

            Socket socket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(ipEndPoint);
            socket.Listen(120);

            while (true) {
                Socket socketClient = socket.Accept();

                if (socketClient.Connected)
                {
                    new Thread(() =>
                    {
                        SocketClientThread(socketClient);
                    }).Start();
                }
            }
        }

        

        static void SocketClientThread(Socket socket)
        {
            byte[] bytesSocket = new byte[10000];
            
            try
            {
                int tamanhoBytesRecebidos = socket.Receive(bytesSocket, bytesSocket.Length, 0);

                var objetoRecebido = EncodingIso.GetString(bytesSocket, 0, tamanhoBytesRecebidos);
                var jsonObjectEnvio = JsonConvert.DeserializeObject<ObjetoEnvio>(objetoRecebido);

                var objetoRetorno = new ObjetoRetorno()
                {
                    Id = jsonObjectEnvio.Id,
                    Name = jsonObjectEnvio.Name,
                    Retorno = "mensagem de retorno"
                };

                var jsonRetorno = JsonConvert.SerializeObject(objetoRetorno);

                socket.Send(EncodingIso.GetBytes(jsonRetorno));
              

                Console.WriteLine(objetoRecebido);
                socket.Close();
            }
            catch (Exception ex)
            {
                if (socket.Connected)
                    socket.Send(BitConverter.GetBytes(-1));

                socket.Close();
            }
        }
    }
}
