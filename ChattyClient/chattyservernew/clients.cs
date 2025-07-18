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
        Disconnect = 3,
        SystemMessage = 4  // Added new opcode for system messages
    }

    class Client
    {
        public string Username { get; set; } = string.Empty;
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
                // Read username length
                int length = Stream.ReadByte();
                if (length == -1) return;

                // Read user ID
                UserID = (byte)Stream.ReadByte();
                if (UserID == 0) return; // Invalid user ID

                // Read username
                byte[] usernameBytes = new byte[length];
                int bytesRead = Stream.Read(usernameBytes, 0, length);
                if (bytesRead == length)
                {
                    Username = Encoding.ASCII.GetString(usernameBytes);
                    Console.WriteLine($"{DateTime.Now}: Client connected with username: {Username} (ID: {UserID})");

                    // Broadcast to other clients that this user joined
                    Program.BroadcastSystemMessage($"{Username} joined the chat", this);
                }
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
                // Read message length
                int length = Stream.ReadByte();
                if (length == -1) return;

                // Read user ID
                byte userID = (byte)Stream.ReadByte();
                if (userID == 0) return;

                // Read message
                byte[] messageBytes = new byte[length];
                int bytesRead = Stream.Read(messageBytes, 0, length);
                if (bytesRead == length)
                {
                    string message = Encoding.ASCII.GetString(messageBytes);
                    Console.WriteLine($"{DateTime.Now}: {Username} (ID: {userID}): {message}");

                    // Broadcast to all other clients with username
                    Program.BroadcastMessage(this, $"({Username}) {message}", userID);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now}: Error handling message: {ex.Message}");
            }
        }

        private void HandleDisconnect()
        {
            Console.WriteLine($"{DateTime.Now}: Client {Username} disconnected");
            Program.BroadcastSystemMessage($"{Username} left the chat", this);
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
                Stream.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now}: Error sending message to {Username}: {ex.Message}");
            }
        }

        public void SendSystemMessage(string message)
        {
            try
            {
                if (!ClientSocket.Connected) return;

                var messageBytes = Encoding.ASCII.GetBytes(message);
                var packet = new List<byte>
                {
                    (byte)Opcode.SystemMessage,  // Use SystemMessage opcode
                    (byte)messageBytes.Length,
                    0 // System message (User ID 0)
                };
                packet.AddRange(messageBytes);

                Stream.Write(packet.ToArray(), 0, packet.Count);
                Stream.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now}: Error sending system message to {Username}: {ex.Message}");
            }
        }
    }
}

