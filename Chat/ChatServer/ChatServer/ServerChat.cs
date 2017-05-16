using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;

namespace ChatServer
{
    public class ServerChat
    {
        static TcpListener listener; 
        List<ClientOptions> new_clients = new List<ClientOptions>(); 

        protected internal void JoinConnection(ClientOptions clientObject)
        {
            new_clients.Add(clientObject);
        }
        protected internal void DeleteConnection(string id)
        {
            ClientOptions client = new_clients.FirstOrDefault(c => c.User_id == id);
            
            if (client != null)
                new_clients.Remove(client);
        }
        
        protected internal void Listen()
        {
            try
            {
                listener = new TcpListener(IPAddress.Any, 8888);
                listener.Start();
                Console.WriteLine("The server is running. Waiting for users...");

                while (true)
                {
                    TcpClient tcpClient = listener.AcceptTcpClient();

                    ClientOptions clientObject = new ClientOptions(tcpClient, this);
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Deactivate();
            }
        }

        
        protected internal void StreamMessage(string letter, string id)
        {
            byte[] storage = Encoding.Unicode.GetBytes(letter);
            for (int i = 0; i < new_clients.Count; i++)
            {
                if (new_clients[i].User_id != id)
                { new_clients[i].Network.Write(storage, 0, storage.Length); }
            }
        }
       
        protected internal void Deactivate()
        {
            listener.Stop(); 

            for (int i = 0; i < new_clients.Count; i++)
            {
                new_clients[i].Close(); 
            }
            Environment.Exit(0); 
        }
    }
}