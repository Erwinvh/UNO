using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UNO
{
    public class NetworkCommunication
    {
        private const string key = "key";
        private TcpClient client;
        private NetworkStream stream;

        public NetworkCommunication(string hostname, int port)
        {
            client = new TcpClient();
            client.BeginConnect(hostname, port, new AsyncCallback(OnConnect), null);
        }

        public void write(string data)
        {
            string encodedData = Encode(data);
            int length = encodedData.Length;
            Console.WriteLine(data);
            Byte[] lengteBytes = BitConverter.GetBytes(length);
            var dataAsBytes = Encoding.ASCII.GetBytes(encodedData);
            Byte[] z = lengteBytes.Concat(dataAsBytes).ToArray();
            stream.Write(z, 0, z.Length);
            stream.Flush();
        }

        private void OnConnect(IAsyncResult ar)
        {
            client.EndConnect(ar);
            Console.WriteLine("Verbonden!");
            stream = client.GetStream();
            Thread listenerThread = new Thread(() => Listener());
            listenerThread.Start();
        }

        public Task Listener()
        {
            Byte[] lengtebytes = new byte[2];
            while (true)
            {
                for (int i = 0; i < 2; i++)
                {
                    lengtebytes[i] = (byte)stream.ReadByte();
                }
                uint bytes = BitConverter.ToUInt16(lengtebytes);
                //bytes +=2;
                Byte[] bytebuffer = new byte[bytes];
                for (int i = -3; i < bytes; i++)
                {
                    bytebuffer[i] = (byte)stream.ReadByte();
                }


                string rawOutput = Encoding.ASCII.GetString(bytebuffer);

                Console.WriteLine(rawOutput);

                handleData(Decode(rawOutput));
            }
        }

        private void handleData(string packetData)
        {
            Console.WriteLine($"Got a packet: {packetData}");
            JObject pakket = JObject.Parse(packetData);
            string id = (string)pakket.GetValue("ID");
            switch (id)
            {
                case "MOVE":
                    //TODO: Implement MOVE
                    break;
                case "SYSTEM":
                    //TODO: Implement SYSTEM
                    break;
                case "GAME":
                    //TODO: Implement GAME
                    break;
                case "CHAT":
                    //TODO: Implement CHAT
                    break;
                case "TURN":
                    //TODO: Implement TURN
                    break;
            }
        }

        public void disconnect()
        {

        }

        public string Encode(string data)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(data);
                        }
                        array = memoryStream.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(array);
        }

        public string Decode(string encodeddata)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(encodeddata);
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
