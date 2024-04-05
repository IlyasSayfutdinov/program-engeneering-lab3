using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

class FileClient
{
    static void Main()
    {
        string serverIP = "127.0.0.1";
        int serverPort = 8888;

        try
        {
            TcpClient client = new TcpClient(serverIP, serverPort);
            NetworkStream stream = client.GetStream();

            Console.WriteLine("Enter action (1 - get a file, 2 - create a file, 3 - delete a file):");
            string action = Console.ReadLine().Trim();

            string request = "";
            switch (action)
            {
                case "1":
                    Console.WriteLine("Enter filename:");
                    string filenameGet = Console.ReadLine().Trim();
                    request = $"GET {filenameGet}";
                    break;
                case "2":
                    Console.WriteLine("Enter filename:");
                    string filenamePut = Console.ReadLine().Trim();
                    Console.WriteLine("Enter file content:");
                    string fileContent = Console.ReadLine();
                    request = $"PUT {filenamePut} {fileContent}";
                    break;
                case "3":
                    Console.WriteLine("Enter filename:");
                    string filenameDelete = Console.ReadLine().Trim();
                    request = $"DELETE {filenameDelete}";
                    break;
                default:
                    Console.WriteLine("Invalid action!");
                    break;
            }

            byte[] data = Encoding.UTF8.GetBytes(request);
            stream.Write(data, 0, data.Length);

            data = new byte[256];
            StringBuilder responseBuilder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = stream.Read(data, 0, data.Length);
                responseBuilder.Append(Encoding.UTF8.GetString(data, 0, bytes));
            }
            while (stream.DataAvailable);

            string response = responseBuilder.ToString().Trim();

            switch (response)
            {
                case "200":
                    Console.WriteLine("The response says that the file was created!");
                    break;
                case "403":
                    Console.WriteLine("The response says that creating the file was forbidden!");
                    break;
                case "404":
                    Console.WriteLine("The response says that the file was not found!");
                    break;
                case "exit":
                    Console.WriteLine("Server stopped!");
                    break;
                default:
                    Console.WriteLine("Unexpected response from server: " + response);
                    break;
            }

            stream.Close();
            client.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
