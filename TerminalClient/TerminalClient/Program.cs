using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


namespace TerminalClient
{
    class Program
    {
        public static int port = 6321;
        static Thread clientThread;
        static Client thisClient;

        static void Main(string[] args)
        {
            AskToConnect();
            clientThread = new Thread (CreateClient);
            clientThread.Start();

        }

        static void CreateClient() {
            thisClient = new Client();
        }

        static void AskToConnect()
        {
            bool answered = false;
            bool connect = false;
            while (!answered && !connect)
            {
                Console.WriteLine("Welcome to Alec's Chat service.\n" +
                "Would you like to be the connect? [Y]es [N]o");
                string input = Console.ReadLine();
                //ConsoleKeyInfo result = Console.ReadKey();
                if ((input == "Y") || input == "y")
                {
                    answered = true;
                    connect = true;
                }
                else if ((input == "N") || input == "n")
                {
                    answered = true;
                    connect = false;
                }
                else
                {
                    Console.WriteLine("Press Y or N on the keyboard.");
                }
            }
        }

    }

    public class Client
    {
        private bool socketReady;
        private TcpClient socket;
        private NetworkStream stream;
        private StreamWriter writer;
        private StreamReader reader;

        string clientName = "Alec";
        string messageToSend;

        private Thread T;
        private Thread inputThread;

        public Client()
        {
            T = new Thread(ConnectToServer);
            T.Start();
        }

        public void ConnectToServer()
        {
            bool connectedYet = false;
            while (!connectedYet)
            {
                //if connected ignore
                if (socketReady)
                    return;

                string host = "127.0.0.1";
                int port = Program.port;

                try
                {
                    socket = new TcpClient(host, port);
                    stream = socket.GetStream();
                    writer = new StreamWriter(stream);
                    reader = new StreamReader(stream);
                    socketReady = true;
                    connectedYet = true;

                }
                catch (Exception e)
                {
                    Console.WriteLine("Socket Error : " + e.Message);
                }
            }
            T = new Thread(Update);
            T.Start();
            inputThread = new Thread(SendMessage);
            inputThread.Start();
        }

        void Update()
        {
            while (true)
            {
                if (socketReady)
                {
                    if (stream.DataAvailable)
                    {
                        string data = reader.ReadLine();
                        if (data != null)
                            OnIncomingData(data);

                    }
                }
            }

        }
        void OnIncomingData(string data)
        {
            if (data == "%NAME")
            {
                SendMessage("&NAME|" + clientName);
                return;
            }
            Console.WriteLine(data);
        }

        void SendMessage()
        {
            while (true)
            {
                if (!socketReady)
                    return;
                messageToSend = Console.ReadLine();
                writer.WriteLine(messageToSend);
                writer.Flush();
            }
        }
        void SendMessage(string data)
        {
            writer.WriteLine(data);
            writer.Flush();
        }


    }

}
