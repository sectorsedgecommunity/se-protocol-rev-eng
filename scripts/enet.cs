// USAGE:
//  download and unpack in the same folder:
//   https://github.com/nxrighthere/ENet-CSharp/releases/download/2.4.7/ENet-CSharp-2.4.7-x64.zip
//  please note that that is not the latest version ^, that is 2.4.7
//  this specific library is used because it is also used by Sector's Edge
//  find netstandard.dll of your dotnet/mono installation(on linux for mono: /usr/lib/mono/{version here}/Facades/netstandard.dll)
//  compile with csc -r:ENet-CSharp.dll -r{path to netstandard.dll} enet.cs
//  (alternatively csc -> mono-csc)
//  run with -s to execute the server
//  run without arguments to execute the client
//  run both -s and -c to run a proxy
//  run with -S "port" to specify the port for the server
//  run with -C "address:[port]" to specify address and optionally the port

using ENet;

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Program {
  class ChannelPacket {
    public byte chan;
    public Packet packet;
    public int num;
    
    public ChannelPacket(byte c, Packet p, int n) {
      chan = c;
      packet = p;
      num = n;
    }
    
    public bool sendTo(Peer peer) => peer.Send(chan, ref packet);
    
    public void Dispose() => packet.Dispose();
  }  

  static char filterChar(byte b) => (b <= 32 || b > 127) ? '.' : (char)b;

  static string hexdump(int columns, byte[] bytes) {
    int rows = bytes.Length / columns + (bytes.Length % columns > 0 ? 1 : 0);
    string[] lines = new string[rows];
    string format = String.Format("\t {{0:X8}}  {{1,-{0}}} {{2}}", columns * 2 + columns);
    int i = 0; 
    int j = 0;
    while(i < bytes.Length) {
      StringBuilder hex = new StringBuilder();
      StringBuilder asc = new StringBuilder();
      int columnLen = Math.Min(columns, bytes.Length - i);
      foreach (byte b in new Span<byte>(bytes, i, columnLen)) {
        hex.AppendFormat("{0:X2} ", b);
        asc.Append(filterChar(b));
      }
      lines[j] = String.Format(format, i, hex, asc);
      i += columns;
      j++;
    }
    return string.Join("\n" , lines);
  }

  static void evloop(Host host, string log_prefix, ref ConcurrentQueue<ChannelPacket> inQueue, ref ConcurrentQueue<ChannelPacket> outQueue, int timeout) {
    Peer peer;
    Event ev;
    int num = 0;
    Console.WriteLine("{0}: Waiting for connection", log_prefix);
    // wait for connection before anything else
    while (true) {
      if (host.Service(timeout, out ev) == 1 && ev.Type == EventType.Connect) {
        peer = ev.Peer;
        Console.WriteLine("{0}: Connect{{data: {1}, ipAddr: \"{2}\"}}", log_prefix, ev.Data, ev.Peer.IP);
        break;
      }
    }
    while (true) {
      if (host.Service(0, out ev) == 1) {
        switch (ev.Type) {
          case EventType.Connect:
                Console.WriteLine("{0}: Connect{{data: {1}, ipAddr: \"{2}\"}}", log_prefix, ev.Data, ev.Peer.IP);
                break;
          case EventType.Disconnect:
                Console.WriteLine("{0}: Disconnect{{data: {1}, ipAddr: \"{2}\"}}", log_prefix, ev.Data, ev.Peer.IP);
                break;
          case EventType.Receive:
                byte[] bytes = new byte[ev.Packet.Length];
                ev.Packet.CopyTo(bytes);
                Console.WriteLine("{0}: Receive{{num: {1} channel: {2}, dataLength: {3}, data: \n\"{4}\"\n\t}}", log_prefix, num, ev.ChannelID, ev.Packet.Length, hexdump(16, bytes));
                outQueue.Enqueue(new ChannelPacket(ev.ChannelID, ev.Packet, num++));
                break;
        }
      }
      ChannelPacket packet;
      while (inQueue.TryDequeue(out packet)) {
        if (packet.sendTo(peer))
          Console.WriteLine("{0}: Sent {1}", log_prefix, packet.num);
        else 
          Console.WriteLine("{0}: E: Failed to send {1}", log_prefix, packet.num);
      }
      host.Flush();
    }
  }
  
  // mom look i have written a garbage collector
  static void garbageCollector(ConcurrentQueue<ChannelPacket> inQueue) {
    ChannelPacket packet;
    Console.WriteLine("Garbage collector running");
    while (inQueue.TryDequeue(out packet))
      packet.Dispose();
  }

  static Host createServer(ushort port) {
    Host server = new Host();
    Address addr = new Address();
    addr.Port = port;
    server.Create(addr, 1);
    Console.WriteLine("I: Created server");
    return server;
  }
  
  static Host createClient(string hostname, ushort port, out Peer peer) {
    Host client = new Host();
    Address addr = new Address();
    addr.SetHost(hostname);
    addr.Port = port;
    client.Create();
    peer = client.Connect(addr);
    Console.WriteLine("I: Created client");
    return client;
  }

  static void Main(string[] args) {
    int mode = 1; // bitflag 0b01 = client, 0b10 = server, default = client(1)
    Host client;
    Host server;
    ConcurrentQueue<ChannelPacket> serverQueue = new ConcurrentQueue<ChannelPacket>(); // client feeds, server eats
    ConcurrentQueue<ChannelPacket> clientQueue = new ConcurrentQueue<ChannelPacket>(); // server feeds, client eats
    Action serverEvLoop = () => garbageCollector(serverQueue);
    Action clientEvLoop = () => garbageCollector(clientQueue); 
    Peer serverPeer;
    ushort listenPort = 11810;
    string hostname = "127.0.0.1";
    ushort connectPort = 11810;

    int sArgIdx = Array.IndexOf(args, "-S");
    if (sArgIdx != -1) {
      if (sArgIdx + 1 < args.Length && !UInt16.TryParse(args[sArgIdx + 1], out listenPort)) {
        Console.WriteLine("E: {args[sArgIdx + 1]} after argument -S contains an invalid port");
        return;
      }
      mode = 2;
    } else if (args.Contains("-s")) {
      mode = 2;
    }

    int cArgIdx =  Array.IndexOf(args, "-C");
    if (cArgIdx != -1) {
      if (cArgIdx + 1 < args.Length) {
        var arr = args[sArgIdx + 1].Split(":");
        hostname = arr[0];
        if (arr.Length > 1 && !UInt16.TryParse(arr[1], out connectPort)) {
          Console.WriteLine("E: {args[cArgIdx + 1]} after argument -C contains an invalid port");
          return;
        }
      }
      mode = mode | 1;
    } else if (args.Contains("-c") || mode == 1) {
      mode = mode | 1;
    }


    if (mode == 3 && hostname == "127.0.0.1" && listenPort == connectPort)  {
      Console.WriteLine("W: client and server need different port when connecting to localhost changing server port to 11811");
      listenPort = 11811;
    }
        
    // start server first
    if ((mode & 2) != 0) {
      Console.WriteLine($"I: Creating server on localhost at port {listenPort}");
      server = createServer(listenPort);
      serverEvLoop = () => evloop(server, "S", ref serverQueue, ref clientQueue, 0);
    }
    if ((mode & 1) != 0) {
      Console.WriteLine($"I: Creating client for address {hostname} at port {connectPort}");
      client = createClient(hostname, connectPort, out serverPeer);
      clientEvLoop = () => evloop(client, "C", ref clientQueue, ref serverQueue, 0);
    }
   Parallel.Invoke(serverEvLoop, clientEvLoop);
  }
}
