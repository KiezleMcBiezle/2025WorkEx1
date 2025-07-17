using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ChattyServer
{
    class Program
    {
        static List<Client> _users = new List<Client>();
        static TcpListener? _listener;
        static readonly object _lockObject = new object();

        static void Main(string[] args)
        {
            // Use port 9001 to match your client
            _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 9001);
            _listener.Start();

            Console.WriteLine($"{DateTime.Now}: Chatty Server started on 127.0.0.1:9001");
            Console.WriteLine("Waiting for clients to connect...");

            try
            {
                while (true)
                {
                    // Accept new client connection
                    TcpClient tcpClient = _listener.AcceptTcpClient();
                    var client = new Client(tcpClient);

                    lock (_lockObject)
                    {
                        _users.Add(client);
                    }

                    Console.WriteLine($"{DateTime.Now}: New client connected. Total clients: {_users.Count}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now}: Chatty Server error: {ex.Message}");
            }
            finally
            {
                _listener?.Stop();
                Console.WriteLine($"{DateTime.Now}: Chatty Server stopped");
            }
        }

        public static void BroadcastMessage(Client sender, string message, byte senderID)
        {
            lock (_lockObject)
            {
                // Send message to all clients except the sender
                var clientsToRemove = new List<Client>();

                foreach (var client in _users.ToList())
                {
                    if (client != sender && client.ClientSocket.Connected)
                    {
                        try
                        {
                            client.SendMessage(message, senderID);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"{DateTime.Now}: Error broadcasting to {client.Username}: {ex.Message}");
                            clientsToRemove.Add(client);
                        }
                    }
                }

                // Remove disconnected clients
                foreach (var client in clientsToRemove)
                {
                    _users.Remove(client);
                }
            }
        }

        public static void BroadcastSystemMessage(string message, Client excludeClient = null)
        {
            lock (_lockObject)
            {
                var clientsToRemove = new List<Client>();

                foreach (var client in _users.ToList())
                {
                    if (client != excludeClient && client.ClientSocket.Connected)
                    {
                        try
                        {
                            client.SendSystemMessage($"(SYSTEM) {message}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"{DateTime.Now}: Error broadcasting system message to {client.Username}: {ex.Message}");
                            clientsToRemove.Add(client);
                        }
                    }
                }

                // Remove disconnected clients
                foreach (var client in clientsToRemove)
                {
                    _users.Remove(client);
                }
            }
        }

        public static void RemoveClient(Client client)
        {
            lock (_lockObject)
            {
                if (_users.Remove(client))
                {
                    Console.WriteLine($"{DateTime.Now}: Removed client {client.Username}. Total clients: {_users.Count}");
                }
            }
        }
    }
}