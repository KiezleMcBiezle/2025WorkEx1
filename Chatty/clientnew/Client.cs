using core;
using System.Net.Sockets;
using System.Text;

namespace clientnew;
public class Client
{
    private readonly TcpClient _client;

    public EventHandler<string> MessageReceived;

    public Client()
    {
        _client = new();
    }

    public void Connect(string ipAddress, int port)
    {
        _client.Connect(System.Net.IPAddress.Parse(ipAddress), port);

        SendFakeUsername(); // This has no proper implementation across the GUIs

        _ = Task.Run(() => PollForMessages());
    }

    private void SendFakeUsername()
    {
        var packet = new List<byte>
        {
            (byte)opcode.username,
        };
        var messageBytes = Encoding.ASCII.GetBytes("SomeUsername");
        var length = (byte)messageBytes.Length;
        packet.Add(length);
        packet.AddRange(messageBytes);
        _client.GetStream().Write(packet.ToArray(), 0, packet.Count);
    }

    public void SendMessage(string message)
    {
        var packet = new List<byte>
        {
            (byte)opcode.message
        };
        var messageBytes = Encoding.ASCII.GetBytes(message);
        var length = (byte)messageBytes.Length;
        packet.Add(length);
        packet.AddRange(messageBytes);
        _client.GetStream().Write(packet.ToArray(), 0, packet.Count);
    }

    private void PollForMessages()
    {
        while (true)
        {
            var opcode1 = _client.GetStream().ReadByte();
            if ((byte)opcode.message == opcode1)
            {
                var lengthofrecieved = _client.GetStream().ReadByte();
                var data = new byte[lengthofrecieved];
                _ = _client.GetStream().Read(data, 0, lengthofrecieved);
                var complete_message = Encoding.ASCII.GetString(data);
                MessageReceived.Invoke(this, complete_message);
            }
        }
    }
}
