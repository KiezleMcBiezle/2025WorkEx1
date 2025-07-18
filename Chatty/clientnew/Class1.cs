using core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace clientnew
{
    public class Class1
    {
        TcpClient Tcpclient { get; }
         public EventHandler<(string, string)> messagerecieved;

        public Class1(TcpClient client)
        {
            Tcpclient = client;
        }
        public void send_message(string message)
        {
            List<byte> packet = new List<byte>();
            packet.Add((byte)opcode.message);
            var messages = Encoding.ASCII.GetBytes(message);
            byte length = (byte)message.Length;
            packet.Add((byte)length);
            packet.AddRange(Encoding.ASCII.GetBytes(message));
            Tcpclient.GetStream().Write(packet.ToArray(), 0, packet.Count);
        }
        public void start()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    var opcode1 = Tcpclient.GetStream().ReadByte();
                    if ((byte)opcode.message == opcode1)
                    {
                        var lengthofrecieved = Tcpclient.GetStream().ReadByte();
                        byte[] data = new byte[lengthofrecieved];
                        _ = Tcpclient.GetStream().Read(data, 0, lengthofrecieved);
                        var complete_message = Encoding.ASCII.GetString(data);
                        messagerecieved.Invoke(this, ("", complete_message));
                    }
                    
                }
            });
            
        }
    }
}
