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
    class RequestForProcessing
    {
        TcpClient client;
        string pathToDirectory;

        public TcpClient CurrentClient
        {
            get { return client; }

        }

        public string PathToDirectory
        {
            get { return pathToDirectory; }
        }

        public RequestForProcessing(TcpClient client, string pathToDitectory)
        {
            this.client = client;
            this.pathToDirectory = pathToDitectory;
        }

        public void Treatment(object clientData)
        {
            RequestForProcessing data = (RequestForProcessing)clientData;
            NetworkStream stream = data.CurrentClient.GetStream();
            string checkPath = data.PathToDirectory;
            
            string[] checkDir = Directory.GetFiles(checkPath);
            string answer = "";

            foreach (var file in checkDir)
            {
                var checkFile = new StreamReader(file, Encoding.UTF8);                
                Client.SendMessage(stream, checkFile.ReadToEnd());               
                answer += Path.GetFileName(file) + ") " + Client.ReceiveMessage(stream)+ "\n" ;

            }

            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("Результат обработки папки " + Path.GetFileName(checkPath) + ":");
            Console.WriteLine(answer);
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("Укажите путь к папке с проверяемыми файлами, либо дождитесь обработки предыдущего запроса.");
            Console.WriteLine("Так же вы можете написать Stop чтобы выйти.");
            data.CurrentClient.Close();
        }
    }
}
