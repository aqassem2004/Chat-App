using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Server
{
    private static TcpListener listener;
    private static Dictionary<string, TcpClient> clients = new Dictionary<string, TcpClient>();

    public static void StartServer()
    {
        listener = new TcpListener(IPAddress.Any, 5000);
        listener.Start();
        Console.WriteLine("Server is Running");

        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();
            Console.WriteLine("New client connected.");

            Thread clientThread = new Thread(() => HandleClient(client));
            clientThread.Start();
        }
    }

    private static void HandleClient(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];
        string clientPhone = "0";
        try
        {
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            clientPhone = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
            Console.WriteLine($"Client registered with phone: {clientPhone}");

            lock (clients)
            {
                clients[clientPhone] = client;
            }

            while (true)
            {
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0) break;

                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                Console.WriteLine($"{clientPhone}: {message}");

                string[] parts = message.Split(":", 2);
                if (parts.Length == 2)
                {
                    string targetPhone = parts[0].Trim();
                    string actualMessage = parts[1].Trim();

                    SendMessageToClient(targetPhone, $"{clientPhone}: {actualMessage}");
                }
            }
        }
        catch
        {
            Console.WriteLine("Client disconnected.");
        }
        finally
        {
            client.Close();
            lock (clients)
            {
                clients.Remove(clientPhone);
            }
        }
    }

    private static void SendMessageToClient(string targetPhone, string message)
    {
        lock (clients)
        {
            if (clients.ContainsKey(targetPhone))
            {
                try
                {
                    TcpClient targetClient = clients[targetPhone];
                    NetworkStream stream = targetClient.GetStream();
                    byte[] buffer = Encoding.UTF8.GetBytes(message);
                    stream.Write(buffer, 0, buffer.Length);
                }
                catch
                {
                    Console.WriteLine($"Failed to send message to {targetPhone}.");
                }
            }
            else
            {
                Console.WriteLine($"Client with phone {targetPhone} not found.");
            }
        }
    }
}

class Program
{
    static void Main()
    {
        Server.StartServer();
    }
}
