using core;
using System.Net;
using System.Net.Sockets;
using System.Text;

var client = new TcpClient();
client.Connect(IPAddress.Parse("192.168.55.3"), 9001);

Console.WriteLine("what is your username");
var username = Console.ReadLine();
var usernamepacket = new List<byte>
{
    (byte)opcode.username
};
var user = Encoding.ASCII.GetBytes(username);
var len = (byte)user.Length;
usernamepacket.Add(len);
usernamepacket.AddRange(Encoding.ASCII.GetBytes(username));
client.GetStream().Write(usernamepacket.ToArray(), 0, usernamepacket.Count);

Console.WriteLine("can you enter a message please");
while (1 > 0)
{
    var input = Console.ReadLine();



}

