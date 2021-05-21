using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Server
    {

        public static string ReceiveMessage(NetworkStream stream)
        {
            byte[] message = new byte[128];
            int bytes = 0;
            bytes = stream.Read(message, 0, message.Length);
            return Encoding.Unicode.GetString(message, 0, bytes).ToString();
        }

        public static void SendMessage(NetworkStream stream, string text)
        {
            byte[] message = Encoding.Unicode.GetBytes(text);
            if (message.Length == 0)
            {
                message = Encoding.Unicode.GetBytes(" ");
            }
            stream.Write(message, 0, message.Length);
        }

        static TcpListener listener;
        const int port = 8888;
        const string address = "127.0.0.1";

        public static int CountThread { get; set; } = 0;

        public static bool AreThereFreeThread
        {
            get { return CountThread < N; }
        }
        static int N = 0;

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Введите максимальное количество возможных запросов(От 1 до 10)");
                int.TryParse(Console.ReadLine(), out N);
                listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
                listener.Start();                
                Console.WriteLine("Ожидание запроса для обработки....");
                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    var stream = client.GetStream();

                    if (AreThereFreeThread)
                    {
                        SendMessage(stream, "Start");
                        CountThread++;
                        ClientObject clientObject = new ClientObject(client);
                        Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                        clientThread.Start();
                        Console.WriteLine("Запрос получен.");
                    }
                    else
                    {
                        SendMessage(stream, "Error");
                        client.Close();
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (listener != null)
                    listener.Stop();
            }

        }
    }
}
