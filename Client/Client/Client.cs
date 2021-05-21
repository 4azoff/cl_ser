using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{   

    class Client
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


        const int port = 8888;
        const string address = "127.0.0.1";

        static void Main(string[] args)
        {           
           
            try
            {
                Console.WriteLine("Укажите путь к папке с проверяемыми файлами, либо напишите <Stop>, чтобы выйти");
                while (true)
                {
                    
                    string checkPath = Console.ReadLine();
                    if (checkPath.Trim().ToLower() == "stop")
                        break;
                    TcpClient client = new TcpClient(address, port);                   
                    var stream = client.GetStream();   
                   
                    string answer = ReceiveMessage(stream);
                    if(answer == "Start") {
                        var request = new RequestForProcessing(client, checkPath);
                        Thread thread = new Thread(new ParameterizedThreadStart(request.Treatment));
                        thread.Start(request);
                        Console.WriteLine("Укажите путь к папке с проверяемыми файлами, либо дождитесь обработки предыдущего запроса.");
                    }
                    else
                    {
                        Console.WriteLine("Нет свободных потоков для обработки запроса, подождите завершения обработки других запросов");
                        client.Close();
                    }
                    
                }
                    
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
            
            
        }
    }
}
