using core;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

TcpListener listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 9001);
listener.Start();
var client = listener.AcceptTcpClient();
Console.WriteLine("success");
Console.ReadLine();
var opcode2 = client.GetStream().ReadByte();
Dictionary<int,string> usernamelist = new Dictionary<int,string>();
if ((byte)opcode.username == opcode2)
{
    var len = client.GetStream().ReadByte();
    byte[] datas = new byte[len];
    var userid = client.GetStream().ReadByte();
    _ = client.GetStream().Read(datas, 0, len);
    var complete_username = Encoding.ASCII.GetString(datas);
    usernamelist[userid] = complete_username;    
}



while (1 > 0)
{
    var opcode1 = client.GetStream().ReadByte();
    if ((byte)opcode.message == opcode1)
    {
        var length = client.GetStream().ReadByte();
        var userid = client.GetStream().ReadByte();
        byte[] data = new byte[length];
        _ = client.GetStream().Read(data, 0, length);
        var complete_message = Encoding.ASCII.GetString(data);
        Console.WriteLine(complete_message);
        Console.WriteLine("--" + usernamelist[userid]);
        Console.ReadLine();
    }

     else if ((byte)opcode.disconnect == opcode1)
    {
        var userid = client.GetStream().ReadByte();
        Console.WriteLine(usernamelist[userid] + "disconnected");
    }
}



