using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;

class Client
{
    private static TcpClient client;
    private static NetworkStream stream;
    private static List<string> UnreadMessages = new List<string>();
    private static bool isRunning = true;
    private static readonly object lockObj = new object();

    public static void StartClient()
    {
        try
        {
            client = new TcpClient("127.0.0.1", 5000);
            stream = client.GetStream();

            Console.Write("Enter your phone number: ");
            string phoneNumber = Console.ReadLine();
            SendMessage(phoneNumber);

            Thread receiveThread = new Thread(ReceiveMessages);
            receiveThread.Start();

            Thread uiThread = new Thread(UpdateUI); // تحديث الواجهة في خيط منفصل
            uiThread.Start();

            while (isRunning)
            {
                Thread.Sleep(100); // الحفاظ على البرنامج يعمل
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            isRunning = false;
            client?.Close();
        }
    }

    private static void UpdateUI()
    {
        while (isRunning)
        {
            Console.Clear(); // مسح الشاشة
            Console.WriteLine("Welcome!");
            Console.WriteLine("\n1. Open chat");
            Console.WriteLine("2. Show unread messages");
            Console.WriteLine("3. Exit");
            Console.Write("Choose an option: ");

            // انتظار المدخلات من المستخدم بشكل غير متزامن
            string option = Console.ReadLine();

            if (option == "1")
            {
                OpenChat();
            }
            else if (option == "2")
            {
                ShowUnreadMessages();
            }
            else if (option == "3")
            {
                isRunning = false;
                break;
            }
            else
            {
                Console.WriteLine("Invalid option. Try again.");
            }

            Thread.Sleep(500); // الانتظار نصف ثانية قبل التحديث التالي
        }
    }

    private static void OpenChat()
    {
        Console.Clear();
        Console.WriteLine("Who do you want to talk to?");
        Console.Write("Phone: ");
        string phone = Console.ReadLine();
        Console.WriteLine("If you want to stop sending messages, send an empty message.");
        while (true)
        {
            Console.Write("Message: ");
            string message = Console.ReadLine();
            if (string.IsNullOrEmpty(message)) break;
            SendMessage(phone + ":" + message);
        }
    }

    private static void ShowUnreadMessages()
    {
        Console.Clear();
        lock (lockObj)
        {
            if (UnreadMessages.Count == 0)
            {
                Console.WriteLine("No unread messages.");
            }
            else
            {
                Console.WriteLine("Unread messages:");
                foreach (var message in UnreadMessages)
                {
                    Console.WriteLine(message);
                }
                UnreadMessages.Clear();
            }
        }
        Console.WriteLine("\nPress any key to return to the main menu...");
        Console.ReadKey();
    }

    private static void SendMessage(string message)
    {
        try
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            stream.Write(buffer, 0, buffer.Length);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending message: {ex.Message}");
        }
    }

    private static void ReceiveMessages()
    {
        byte[] buffer = new byte[1024];

        while (isRunning)
        {
            try
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0) break;

                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                lock (lockObj)
                {
                    UnreadMessages.Add(message);
                }
            }
            catch
            {
                Console.WriteLine("Disconnected from the server.");
                break;
            }
        }

        client?.Close();
    }
}

class Program
{
    static void Main()
    {
        Client.StartClient();
    }
}
