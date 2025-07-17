using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChattyServer
{
    public enum Opcode : byte
    {
        Username = 1,
        Message = 2,
        Disconnect = 3
    }

    class Client
    {
        public string Username { get; set; } = "";
        public byte UserID { get; set; }
        public Guid UID { get; set; }
        public TcpClient ClientSocket { get; set; }
        public NetworkStream Stream { get; set; }
        public Thread ClientThread { get; set; }

        public Client(TcpClient client)
        {
            ClientSocket = client;
            Stream = client.GetStream();
            UID = Guid.NewGuid();

            // Start listening for messages from this client
            ClientThread = new Thread(HandleClient);
            ClientThread.Start();
        }

        private void HandleClient()
        {
            try
            {
                while (ClientSocket.Connected)
                {
                    // Read opcode
                    int opcode = Stream.ReadByte();
                    if (opcode == -1) break;

                    switch ((Opcode)opcode)
                    {
                        case Opcode.Username:
                            HandleUsername();
                            break;
                        case Opcode.Message:
                            HandleMessage();
                            break;
                        case Opcode.Disconnect:
                            HandleDisconnect();
                            return;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now}: Client {Username} error: {ex.Message}");
            }
            finally
            {
                Program.RemoveClient(this);
                ClientSocket?.Close();
            }
        }

        private void HandleUsername()
        {
            try
            {
                // Read length
                int length = Stream.ReadByte();
                if (length == -1) return;

                // Read user ID
                UserID = (byte)Stream.ReadByte();

                // Read username
                byte[] usernameBytes = new byte[length];
                Stream.Read(usernameBytes, 0, length);
                Username = Encoding.ASCII.GetString(usernameBytes);

                Console.WriteLine($"{DateTime.Now}: Client connected with username: {Username} (ID: {UserID})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now}: Error handling username: {ex.Message}");
            }
        }

        private void HandleMessage()
        {
            try
            {
                // Read length
                int length = Stream.ReadByte();
                if (length == -1) return;

                // Read user ID
                byte userID = (byte)Stream.ReadByte();

                // Read message
                byte[] messageBytes = new byte[length];
                Stream.Read(messageBytes, 0, length);
                string message = Encoding.ASCII.GetString(messageBytes);

                Console.WriteLine($"{DateTime.Now}: {Username} (ID: {userID}): {message}");

                // Broadcast to all other clients
                Program.BroadcastMessage(this, message, userID);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now}: Error handling message: {ex.Message}");
            }
        }

        private void HandleDisconnect()
        {
            Console.WriteLine($"{DateTime.Now}: Client {Username} disconnected");
            Program.RemoveClient(this);
        }

        public void SendMessage(string message, byte senderID)
        {
            try
            {
                if (!ClientSocket.Connected) return;

                var messageBytes = Encoding.ASCII.GetBytes(message);
                var packet = new List<byte>
                {
                    (byte)Opcode.Message,
                    (byte)messageBytes.Length,
                    senderID
                };
                packet.AddRange(messageBytes);

                Stream.Write(packet.ToArray(), 0, packet.Count);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now}: Error sending message to {Username}: {ex.Message}");
            }
        }
    }
}