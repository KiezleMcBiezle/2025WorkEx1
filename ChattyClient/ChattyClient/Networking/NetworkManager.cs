using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ChattyClient.Networking
{
    public class NetworkManager
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private Thread _receiveThread;
        private byte _userId = 1;
        public event Action<string> MessageReceived;

        public bool IsConnected => _client?.Connected ?? false;

        public void Connect(string username)
        {
            _client = new TcpClient();
            _client.Connect(IPAddress.Parse("127.0.0.1"), 9001);
            _stream = _client.GetStream();

            var userBytes = Encoding.ASCII.GetBytes(username);
            var packet = new List<byte> { (byte)Opcode.Username, (byte)userBytes.Length, _userId };
            packet.AddRange(userBytes);
            _stream.Write(packet.ToArray(), 0, packet.Count);

            _receiveThread = new Thread(Listen);
            _receiveThread.Start();
        }

        public void Disconnect()
        {
            if (!IsConnected) return;
            var packet = new List<byte> { (byte)Opcode.Disconnect, _userId };
            _stream.Write(packet.ToArray(), 0, packet.Count);
            _client.Close();
        }

        public void SendMessage(string message)
        {
            if (!IsConnected) return;
            var msgBytes = Encoding.ASCII.GetBytes(message);
            var packet = new List<byte> { (byte)Opcode.Message, (byte)msgBytes.Length, _userId };
            packet.AddRange(msgBytes);
            _stream.Write(packet.ToArray(), 0, packet.Count);
        }

        private void Listen()
        {
            try
            {
                while (IsConnected)
                {
                    int op = _stream.ReadByte();
                    if (op == -1) break;

                    if (op == (byte)Opcode.Message)
                    {
                        int len = _stream.ReadByte();
                        int uid = _stream.ReadByte();
                        byte[] data = new byte[len];
                        _stream.Read(data, 0, len);
                        string message = Encoding.ASCII.GetString(data);

                        MessageReceived?.Invoke($"(User {uid}) {message}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageReceived?.Invoke($"[ERROR] {ex.Message}");
            }
        }
    }
}
