using System;
using System.Threading;

namespace ChatServer
{
    class Program
    {
        static ServerChat serverObj; 
        static Thread thread; 
        static void Main(string[] args)
        {

            try
            {
                serverObj = new ServerChat();
                thread = new Thread(new ThreadStart(serverObj.Listen));
                thread.Start(); 
            }



            catch (Exception ex)
            {
                serverObj.Deactivate();
                Console.WriteLine(ex.Message);
            }


        }
    }
}