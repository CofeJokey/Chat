using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;

namespace ChatClient
{
    class Program
    {
        static string nickName;
        private const string host = "127.0.0.1";
        private const int port = 8888;
        static TcpClient client;
        static NetworkStream network;

        static void Main(string[] args)
        {
            Console.WriteLine("******** Chat room ********");
            Console.Write("Enter your nickname: ");
            nickName = Console.ReadLine();
            client = new TcpClient();

            try
            {
                client.Connect(host, port);
                network = client.GetStream(); 

                string notice = nickName;
                byte[] data = Encoding.Unicode.GetBytes(notice);
                network.Write(data, 0, data.Length);
                
                Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                receiveThread.Start(); 
                Console.WriteLine("Welcome to chat room, {0}", nickName);
                SendMessage();
            }
            catch (Exception ex)
            { Console.WriteLine(ex.Message); }

            finally
            { Deactivate(); }
        }
        
        static void SendMessage()
        {
            Console.WriteLine("Enter your message: ");

            while (true)
            {
                // Console.Write("You" + ": ");
                string letter = Console.ReadLine();
                byte[] storage = Encoding.Unicode.GetBytes(letter);
                network.Write(storage, 0, storage.Length);
            }
        }
        
        static void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    byte[] storage = new byte[64]; 
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;

                    do
                    {
                        bytes = network.Read(storage, 0, storage.Length);
                        builder.Append(Encoding.Unicode.GetString(storage, 0, bytes));
                    }
                    while (network.DataAvailable);

                    string message = builder.ToString();
                    Console.WriteLine(message);
                }

                catch
                {
                    Console.WriteLine("**** Connection is interrupted! ****"); 
                    Console.ReadLine();
                    Deactivate();
                }
            }
        }

        static void Deactivate()
        {
            if (network != null) network.Close();
            if (client != null)client.Close();
            Environment.Exit(0); 
        }
    }
}