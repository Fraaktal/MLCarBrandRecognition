using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ML.Control
{
    class SocketServerController
    {
        const int BUFFER_SIZE = 10240;

        public static void StartServer()
        {
            var listener = new TcpListener(IPAddress.Any, 11000);
            listener.Start();

            Console.WriteLine($"Listening on port 11000...");

            while (true)
            {
                var client = listener.AcceptTcpClient();
                Console.WriteLine($"Accepted client {client.Client.RemoteEndPoint}");
                ThreadPool.QueueUserWorkItem(cb => ClientThread(client));
            }
        }

        static void ClientThread(TcpClient client)
        {
            try
            {
                using (var stream = client.GetStream())
                {
                    // Read filename length
                    int fNameLen = stream.ReadByte();
                    byte[] fNameBytes = new byte[fNameLen];

                    // Read filename
                    stream.Read(fNameBytes, 0, fNameLen);
                    string fName = Encoding.Unicode.GetString(fNameBytes);
                    var path = Path.Combine(MLController.GetInstance()._assetsPath, fName);

                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }

                    using (var fs = File.OpenWrite(path))
                    {
                        byte[] buffer = new byte[BUFFER_SIZE];
                        while (true)
                        {
                            int r = stream.Read(buffer, 0, BUFFER_SIZE);
                            if (r == 0 || Encoding.Unicode.GetString(buffer).Contains("<EOF>"))
                                break;

                            fs.Write(buffer, 0, r);
                        }
                    }

                    Console.WriteLine("Image {fName} received");
                    Console.WriteLine("Starting Processing it");

                    var result = MLController.GetInstance().ClassifySingleImage(path);

                    string res = $"Predicted as {result.PredictedLabelValue} with a score of: {result.Score.Max()}";

                    byte[] msg = Encoding.Unicode.GetBytes(res);

                    stream.WriteByte((byte)msg.Length);
                    stream.Write(msg, 0, msg.Length);
                }
            }

            finally
            {
                client.Close();
            }
        }
    }
}
