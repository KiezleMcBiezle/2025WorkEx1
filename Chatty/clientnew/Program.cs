using core;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Emit;
using System.Text;

var client =  new TcpClient();
client.Connect(IPAddress.Parse("127.0.0.1"),9001);

Console.WriteLine("what is your username");
var username = Console.ReadLine();
List<byte> usernamepacket = new List<byte>();
usernamepacket.Add((byte)opcode.username);
var user = Encoding.ASCII.GetBytes(username);
byte len = (byte)user.Length;
usernamepacket.Add((byte)len);
usernamepacket.Add((byte)1);
usernamepacket.AddRange(Encoding.ASCII.GetBytes(username));
client.GetStream().Write(usernamepacket.ToArray(), 0, usernamepacket.Count);
Console.ReadLine();

Console.WriteLine("can you enter a message please");
while (1 > 0)
{
    var input = Console.ReadLine();
    List<byte> packet = new List<byte>();
    packet.Add((byte)opcode.message);
    var message = Encoding.ASCII.GetBytes(input);
    byte length = (byte)message.Length;
    packet.Add((byte)length);
    packet.Add((byte)1);   
    packet.AddRange(Encoding.ASCII.GetBytes(input));
    client.GetStream().Write(packet.ToArray(), 0, packet.Count);
    Console.ReadLine();
    var opcode1 = client.GetStream().ReadByte();
    if ((byte)opcode.message == opcode1)
        {
        var lengthofrecieved = client.GetStream().ReadByte();
        byte[] data = new byte[lengthofrecieved];
         _ = client.GetStream().Read(data, 0, lengthofrecieved);
         var complete_message = Encoding.ASCII.GetString(data);
        Console.WriteLine(complete_message);
        Console.ReadLine() ;
        }
   
}





