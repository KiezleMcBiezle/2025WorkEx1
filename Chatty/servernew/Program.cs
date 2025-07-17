using core;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Concurrent;
using servernew;
using System.Runtime.InteropServices;




server_class servers = new server_class();
servers.start_listener();

Task.Run(() => 
{
        servers.accept_clients();

 });

while (true)
{
    servers.sort_queue();
}
    









