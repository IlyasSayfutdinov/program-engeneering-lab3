using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

class FileServer
{
    static void Main()
    {
        IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        int port = 8888;

        TcpListener listener = new TcpListener(ipAddress, port);
        listener.Start();
        Console.WriteLine("Server started!");

        while (true)
        {
            Console.WriteLine("Waiting for connections...");

            TcpClient client = listener.AcceptTcpClient();
            Console.WriteLine("Connected!");

            NetworkStream stream = client.GetStream();

            byte[] data = new byte[256];
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = stream.Read(data, 0, data.Length);
                builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
            }
            while (stream.DataAvailable);

            string request = builder.ToString().Trim();

            string[] parts = request.Split(' ');
            string action = parts[0];
            string filename = parts[1];
            string response = "";

            switch (action)
            {
                case "PUT":
                    string fileContent = parts[2];
                    response = CreateFile(filename, fileContent);
                    break;
                case "GET":
                    response = GetFile(filename);
                    break;
                case "DELETE":
                    response = DeleteFile(filename);
                    break;
                default:
                    response = "Invalid action!";
                    break;
            }

            byte[] responseBytes = Encoding.UTF8.GetBytes(response);
            stream.Write(responseBytes, 0, responseBytes.Length);

            stream.Close();
            client.Close();

            if (action == "exit")
                break;
        }

        listener.Stop();
    }

    static string CreateFile(string filename, string fileContent)
    {
        string directoryPath = Path.Combine("server", "data");
        string path = Path.Combine(directoryPath, filename);

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        if (File.Exists(path))
        {
            return "403";
        }
        else
        {
            File.WriteAllText(path, fileContent);
            return "200";
        }
    }


    static string GetFile(string filename)
    {
        string path = Path.Combine("server", "data", filename);
        if (File.Exists(path))
        {
            string fileContent = File.ReadAllText(path);
            return $"200 {fileContent}";
        }
        else
        {
            return "404";
        }
    }

    static string DeleteFile(string filename)
    {
        string path = Path.Combine("server", "data", filename);
        if (File.Exists(path))
        {
            File.Delete(path);
            return "200";
        }
        else
        {
            return "404";
        }
    }
}
