using core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace servernew
{
    internal class message_packet
    {
        public string Message { get;}
        public int Opcode {get;}
        public TcpClient Clientsender { get;}
       public message_packet(string message,int opcode1,TcpClient client)
        {
            Message = message;
            Opcode = opcode1;
            Clientsender = client;
        }

    }
}
