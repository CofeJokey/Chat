using System;
using System.Net.Sockets;
using System.Text;

namespace ChatServer
{
    public class ClientOptions
    {
        protected internal string User_id { get; private set; }
        protected internal NetworkStream Network { get; private set; }
        string nickName;
        TcpClient client;
        ServerChat server;

        public ClientOptions(TcpClient tcpCl, ServerChat serverObj)
        {
            User_id = Guid.NewGuid().ToString();
            client = tcpCl;
            server = serverObj;
            serverObj.JoinConnection(this);
        }

        public void Process()
        {
            try
            {
                Network = client.GetStream();
                string message = ReceiveMessage();
                nickName = message;
                message = nickName + " Joined";
                server.StreamMessage(message, this.User_id);
                Console.WriteLine(message);
                
                while (true)
                {
                    try
                    {
                        message = ReceiveMessage();
                        message = String.Format("{0}: {1}", nickName, message);
                        Console.WriteLine(message);
                        server.StreamMessage(message, this.User_id);
                    }
                    catch
                    {
                        message = String.Format("{0}: Left the chat", nickName);
                        Console.WriteLine(message);
                        server.StreamMessage(message, this.User_id);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            finally
            {
                server.DeleteConnection(this.User_id);
                Close();
            }
        }

        
        private string ReceiveMessage()
        {
            byte[] storage = new byte[64]; 
            StringBuilder builders = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Network.Read(storage, 0, storage.Length);
                builders.Append(Encoding.Unicode.GetString(storage, 0, bytes));
            }
            while (Network.DataAvailable);

            return builders.ToString();
        }

        
        protected internal void Close()
        {
            if (Network != null)
                Network.Close();
            if (client != null)
                client.Close();
        }
    }
}