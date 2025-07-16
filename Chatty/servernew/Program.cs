using core;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Concurrent;
using servernew;
using System.Runtime.InteropServices;

TcpListener listener = new TcpListener(IPAddress.Parse("192.168.55.3"), 9001);
listener.Start();
Console.WriteLine("success");
List<TcpClient> clientlist = new List<TcpClient>();
Dictionary < TcpClient,string> usernamelist = new Dictionary<TcpClient,string>();
ConcurrentQueue<message_packet> queue = new ConcurrentQueue<message_packet>();


Task.Run(() =>
{
    var client = listener.AcceptTcpClient();
    clientlist.Add(client);
    var opcode2 = client.GetStream().ReadByte();
    if ((byte)opcode.username == opcode2)
    {
        var len = client.GetStream().ReadByte();
        byte[] datas = new byte[len];
        var userid = client.GetStream().ReadByte();
        _ = client.GetStream().Read(datas, 0, len);
        var complete_username = Encoding.ASCII.GetString(datas);
        usernamelist[client] = complete_username;
    }
    _ = Task.Run(() => Handle_client(client, usernamelist, queue));

    static async Task Handle_client(TcpClient client, Dictionary<TcpClient, string> usernamelist, ConcurrentQueue<message_packet> queue)
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
});
while (true)
{
    if (queue.TryDequeue(out message_packet result))
    {
        if (result.Opcode ==  (int)opcode.message)
        {
            List<byte> packet = new List<byte>();
            packet.Add((byte)opcode.message);
            var message = Encoding.ASCII.GetBytes(result.Message);
            byte message_tosend_length = (byte)result.Message.Length;
            packet.Add((byte)message_tosend_length);
            packet.AddRange(Encoding.ASCII.GetBytes(result.Message));
            foreach (TcpClient client in clientlist)
            {
                client.GetStream().Write(packet.ToArray(), 0, packet.Count);
            }
        }
    }
}




       

