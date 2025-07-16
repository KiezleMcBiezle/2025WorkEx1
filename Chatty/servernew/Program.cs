using core;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Concurrent;

TcpListener listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 9001);
listener.Start();
Console.WriteLine("success");
Console.ReadLine();
List<TcpClient> clientlist = new List<TcpClient>();

Dictionary < TcpClient,string> usernamelist = new Dictionary<TcpClient,string>();
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
    _ = Task.Run(() => Handle_client(client, usernamelist));

    static async Task Handle_client(TcpClient client,Dictionary<TcpClient, string> usernamelist)
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
                List <string> messageforqueue = new List<string>();
                messageforqueue.Add (opcode.message);
            }
       
    }
});



        List<byte> packet = new List<byte>();
        packet.Add((byte)opcode.message);
        var message = Encoding.ASCII.GetBytes(message_tosend);
        byte message_tosend_length = (byte)message.Length;
        packet.Add((byte)message_tosend_length);
        packet.AddRange(Encoding.ASCII.GetBytes(message_tosend));
        client.GetStream().Write(packet.ToArray(), 0, packet.Count);
        Console.ReadLine();
    }

     else if ((byte)opcode.disconnect == opcode1)
    {
        var userid = client.GetStream().ReadByte();
        Console.WriteLine(usernamelist[client] + "disconnected");
    }
}



