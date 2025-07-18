using core;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Emit;
using System.Text;

var client =  new TcpClient();
client.Connect(IPAddress.Parse("192.168.55.3"),9001);

Console.WriteLine("what is your username");
var username = Console.ReadLine();
List<byte> usernamepacket = new List<byte>();
usernamepacket.Add((byte)opcode.username);
var user = Encoding.ASCII.GetBytes(username);
byte len = (byte)user.Length;
usernamepacket.Add((byte)len);
usernamepacket.AddRange(Encoding.ASCII.GetBytes(username));
client.GetStream().Write(usernamepacket.ToArray(), 0, usernamepacket.Count);

Console.WriteLine("can you enter a message please");
while (1 > 0)
{
    var input = Console.ReadLine();
    var opcode1 = client.GetStream().ReadByte();
    if ((byte)opcode.message == opcode1)
        {
        var lengthofrecieved = client.GetStream().ReadByte();
        byte[] data = new byte[lengthofrecieved];
         _ = client.GetStream().Read(data, 0, lengthofrecieved);
         var complete_message = Encoding.ASCII.GetString(data);
        }
   
}





