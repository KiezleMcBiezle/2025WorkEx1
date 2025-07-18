using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Collections.Concurrent;
using core;
using System.Security.Cryptography.X509Certificates;

namespace servernew
{
    internal class server_class
    {
        private TcpListener listener;
        private List<TcpClient> clientlist;
        ConcurrentQueue<message_packet> queue;



        public server_class()
        {
            clientlist = new List<TcpClient>();
            listener = new TcpListener(IPAddress.Parse("192.168.55.3"), 9001);
            queue = new ConcurrentQueue<message_packet>();
        }

        public void start_listener()
        {
            listener.Start();
            Console.WriteLine("success");

        }

        public void accept_clients()
        {
            while (true)
            {
                var client = listener.AcceptTcpClient();
                clientlist.Add(client);
                _ = Task.Run(() => Handle_client(client));
            }



        }

        public void Handle_client(TcpClient client)
        {
            while (true)
            {
                var opcode1 = client.GetStream().ReadByte();
                if ((byte)opcode.message == opcode1)
                {
                    var length = client.GetStream().ReadByte();
                    byte[] data = new byte[length];
                    _ = client.GetStream().Read(data, 0, length);
                    var complete_message = Encoding.ASCII.GetString(data);
                    message_packet queuemessage = new message_packet(complete_message, opcode1, client);
                    queue.Enqueue(queuemessage);
                }
            }
        }

        public void sort_queue()
        {
                if (queue.TryDequeue(out message_packet result))
                {
                    if (result.Opcode == (int)opcode.message)
                    {
                        List<byte> packet = new List<byte>();
                        packet.Add((byte)opcode.message);
                        var message = Encoding.ASCII.GetBytes(result.Message);
                        byte message_tosend_length = (byte)result.Message.Length;
                        packet.Add((byte)message_tosend_length);
                        packet.AddRange(Encoding.ASCII.GetBytes(result.Message));
                        foreach (TcpClient client in clientlist)
                        {
                        if (client == result.Clientsender)
                        {
                            continue;
                        }
                        packet.AddRange(Encoding.ASCII.GetBytes("Unknown"));
                        client.GetStream().Write(packet.ToArray(), 0, packet.Count);
                        }
                    }
                }
        }
    }

}

