using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ChattyClient.Networking
{
    public enum Opcode : byte
    {
        Username = 1,
        Message = 2,
        Disconnect = 3,
        SystemMessage = 4  // Added new opcode for system messages
    }

    public class NetworkManager
    {
        private TcpClient? _client;
        private NetworkStream? _stream;
        private Thread? _receiveThread;
        private byte _userId = 1;

        public event Action<string> MessageReceived;
        public bool IsConnected => _client?.Connected ?? false;

        public void Connect(string username)
        {
            _client = new TcpClient();
            _client.Connect(IPAddress.Parse("127.0.0.1"), 9001);
            _stream = _client.GetStream(); // Fixed: removed asterisks

            // Send username packet
            var userBytes = Encoding.ASCII.GetBytes(username);
            var packet = new List<byte>
            {
                (byte)Opcode.Username,
                (byte)userBytes.Length,
                _userId
            };
            packet.AddRange(userBytes);
            _stream.Write(packet.ToArray(), 0, packet.Count);
            _stream.Flush();

            // Start listening for messages
            _receiveThread = new Thread(Listen);
            _receiveThread.Start();
        }

        public void Disconnect()
        {
            if (!IsConnected) return;

            var packet = new List<byte> { (byte)Opcode.Disconnect };
            _stream?.Write(packet.ToArray(), 0, packet.Count);
            _stream?.Flush();
            _client?.Close();
        }

        public void SendMessage(string message)
        {
            if (!IsConnected || _stream == null) return;

            var msgBytes = Encoding.ASCII.GetBytes(message);
            var packet = new List<byte>
            {
                (byte)Opcode.Message,
                (byte)msgBytes.Length,
                _userId
            };
            packet.AddRange(msgBytes);
            _stream.Write(packet.ToArray(), 0, packet.Count);
            _stream.Flush();
        }

        private void Listen()
        {
            if (_stream == null) return;

            try
            {
                while (IsConnected)
                {
                    int op = _stream.ReadByte();
                    if (op == -1) break;

                    if (op == (byte)Opcode.Message)
                    {
                        int len = _stream.ReadByte();
                        if (len == -1) break;

                        int uid = _stream.ReadByte();
                        if (uid == -1) break;

                        byte[] data = new byte[len];
                        int bytesRead = _stream.Read(data, 0, len);
                        if (bytesRead == len)
                        {
                            string message = Encoding.ASCII.GetString(data);
                            MessageReceived?.Invoke(message); // Server now includes username in message
                        }
                    }
                    else if (op == (byte)Opcode.SystemMessage)
                    {
                        int len = _stream.ReadByte();
                        if (len == -1) break;

                        int uid = _stream.ReadByte(); // Read but ignore for system messages
                        if (uid == -1) break;

                        byte[] data = new byte[len];
                        int bytesRead = _stream.Read(data, 0, len);
                        if (bytesRead == len)
                        {
                            string message = Encoding.ASCII.GetString(data);
                            MessageReceived?.Invoke($"(SYSTEM) {message}");
                        }
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