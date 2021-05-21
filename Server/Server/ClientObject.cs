using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class ClientObject
    {
        public TcpClient client;

        public ClientObject(TcpClient tcpClient)
        {
            client = tcpClient;

        }


        static bool СheckPolyndrom(string str)
        {
            str = str.Replace(" ", "").Replace(",", "").Replace(".", "").Replace("!", "").Replace("?", "").Replace("-", "").Trim().ToLower();
            char[] arr = str.ToCharArray();
            Array.Reverse(arr);
            string newStr = new string(arr);
            if (Equals(str, newStr))
                return true;
            else
                return false;
        }

        public void Process()
        {
            NetworkStream stream = null;

            try
            {
                stream = client.GetStream();
                while (true)
                {
                    do
                    {
                        Stopwatch sw = new Stopwatch();
                        sw.Start();

                        bool isPoly = СheckPolyndrom(Server.ReceiveMessage(stream));

                        sw.Stop();
                        if (sw.ElapsedMilliseconds < 1000)
                            Thread.Sleep(1000 - (int)sw.ElapsedMilliseconds);
                        
                        Server.SendMessage(stream, isPoly.ToString());
                    }
                    while (stream.DataAvailable);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Запрос обработан, либо произошла следующая ошибка:");
                Console.WriteLine(ex.Message);
               
            }
            finally
            {
                Server.CountThread--;
                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();
            }
        }
    }
}
