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
//  run with -o "output" to specifiy output for a binary recording without the .client.rec/.server.rec suffix
//  run with -Qto disable all info/error/warning logs
//  run with -q to disable all logging of packet dumps

using ENet;

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

  static char filterChar(byte b) => (b < 32 || b > 126) ? '.' : (char)b;

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
  
  static void binDump(BinaryWriter writer, Event ev, int num) {
    long time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    if (ev.Type == EventType.Receive) {
      // 8    4   4   len  len%4   8
      // time len num data padding "0xcafebabe"
      writer.Write((Int64) time);
      writer.Write((Int32) ev.Packet.Length);
      writer.Write((Int32) num);
      // add padding for alignment
      byte[] bytes = new byte[ev.Packet.Length + (~ev.Packet.Length & 3)];
      ev.Packet.CopyTo(bytes);
      writer.Write(bytes);
      writer.Write((Int64) 0xcafebabe);
    } else {
      // 8    4   4    4      8
      // time typ data peerId padding
      writer.Write((Int64) time);
      writer.Write((Int32) ev.Type);
      writer.Write((Int32) ev.Data);
      writer.Write((UInt32) ev.Peer.ID);
      writer.Write((Int64) 0xdeadbeef);
    }
  }

  static void logPacket(string log_prefix, Event ev) {
    if (log_packets) {
      Console.WriteLine("{0}: {1}{{data: {2}, ipAddr: \"{3}\"}}", log_prefix, ev.Type, ev.Data, ev.Peer.IP);
    }
  }

  static void logDataPacket(string log_prefix, int num, Event ev) {
    if (log_packets) {
      byte[] bytes = new byte[ev.Packet.Length];
      ev.Packet.CopyTo(bytes);
      Console.WriteLine("{0}: Receive{{num: {1}, channel: {2}, dataLength: {3}, data: \n\"{4}\"\n\t}}", log_prefix, num, ev.ChannelID, ev.Packet.Length, hexdump(16, bytes));
    }
  }
  
  static void log(string fmt, params object[] arg) {
    if (should_log) {
       Console.Error.WriteLine(fmt, arg);
    }
  }

  struct Option<T> {
    public static Option<T> None => default(Option<T>);
    public static Option<T> Some(T value) => new Option<T>(value);

    public readonly bool isSome;
    public readonly T value;

    public Option(T value) {
      this.value = value;
      isSome = true;
    }

    public void Act(Action<T> func) {
      if (this.isSome)
        func(this.value);
    }
  }

  // also accesses the static fields of this class state(volatile uint) and data(volative uint)
  static void evloop(Host host, string log_prefix, ref ConcurrentQueue<ChannelPacket> inQueue, ref ConcurrentQueue<ChannelPacket> outQueue, bool leader, Option<BinaryWriter> writer) {
    Event ev;
    Peer peer;
    int num = 0;
    // wait for leader
    while (true) { 
      // wait for leader to connect
      while (!leader && state == 0)
        Thread.Yield();

      log("I: {0}: Waiting for connection", log_prefix);
      // wait for connection first
      while (true) {
        if (host.Service(0, out ev) == 1 && ev.Type == EventType.Connect) {
          writer.Act((_writer) => binDump(_writer, ev, 0));
          logPacket(log_prefix, ev);
          log("I: {0}: Connected to {1}", log_prefix, ev.Peer.IP);
          peer = ev.Peer;
          if (leader)
            state = 1;
          break;
        }
      }
      while (state == 1) {
        // check packet queue first and then incoming pachets
        // incoming packets might close the connection
        ChannelPacket packet;
        if (inQueue.TryDequeue(out packet)) {
          if(!packet.sendTo(peer))
            log("E: {0}: Failed to send {1}", log_prefix, packet.num);
        }
        host.Flush();
        if (host.Service(0, out ev) == 1) {
          writer.Act((_writer) => binDump(_writer, ev, num));
          switch (ev.Type) {
            case EventType.Connect:
                  logPacket(log_prefix, ev);
                  log("I: {0}: Connected to {2}", log_prefix, ev.Peer.IP);
                  if (ev.Peer.ID != peer.ID) {
                    log("W: {0}: multiple peers are connected this might cause unexpected behaviour", log_prefix);
                  }
                  break;
            case EventType.Disconnect:
                  logPacket(log_prefix, ev);
                  log("I: {0}: Disconnected from {1}", log_prefix, ev.Peer.IP);
                  if (ev.Peer.ID == peer.ID) {
                    // disconnect all
                    data = ev.Data;
                    state = 2;
                  }
                  break;
            case EventType.Receive:
                  logDataPacket(log_prefix, num, ev);
                  outQueue.Enqueue(new ChannelPacket(ev.ChannelID, ev.Packet, num++));
                  break;
          }
        }
      }
      
      if (peer.State == PeerState.Connected) {
        peer.DisconnectNow(data);
        log("I: {0}: Disconnected from peer", log_prefix);
        data = 0;
      }

      // sync at this point
      while (leader && state == 2)
        Thread.Yield();
      if (!leader)
        state = 0;  
    }
  }
  
  // mom look i have written a garbage collector
  static void garbageCollector(ConcurrentQueue<ChannelPacket> inQueue) {
    ChannelPacket packet;
    log("I: Garbage collector running");
    while (true) {
      if (inQueue.TryDequeue(out packet))
        packet.Dispose();
      if (state == 0)
        state = 1;
      else if (state == 2)
        state = 3;
    }
  }

  static Host createServer(ushort port) {
    Host server = new Host();
    Address addr = new Address();
    addr.Port = port;
    server.Create(addr, 1, 255);
    log("I: Created server at address 127.0.0.1 on port {0}", port);
    return server;
  }
  
  static Host createClient(string hostname, ushort port, out Peer peer) {
    Host client = new Host();
    Address addr = new Address();
    addr.SetHost(hostname);
    addr.Port = port;
    client.Create();
    peer = client.Connect(addr, 255);
    log("I: Created client for address {0} on port {1}", hostname, port);
    return client;
  }

  // valid values:
  // 0 wait for connection on leader
  // 1 connect all & run
  // 2 leader wait for all to disconnect
  // 3.. wait for any connection
  static volatile uint state;
  // contains the data sent in a disconnect message
  static volatile uint data = 0;
  static bool should_log = true;
  static bool log_packets = true;

  static void Main(string[] args) {
    int mode = 1; // bitflag 0b01 = client, 0b10 = server, default = client(1)
    Host client;
    Host server;
    ConcurrentQueue<ChannelPacket> serverQueue = new ConcurrentQueue<ChannelPacket>(); // client feeds, server eats
    ConcurrentQueue<ChannelPacket> clientQueue = new ConcurrentQueue<ChannelPacket>(); // server feeds, client eats
    Action serverEvLoop = () => garbageCollector(serverQueue);
    Action clientEvLoop = () => garbageCollector(clientQueue);
    Option<BinaryWriter> clientOutput = Option<BinaryWriter>.None; 
    Option<BinaryWriter> serverOutput = Option<BinaryWriter>.None; 
    Peer serverPeer;
    ushort listenPort = 11810;
    string hostname = "127.0.0.1";
    ushort connectPort = 11810;

    int sArgIdx = Array.IndexOf(args, "-S");
    if (sArgIdx != -1) {
      if (sArgIdx + 1 < args.Length && !UInt16.TryParse(args[sArgIdx + 1], out listenPort)) {
        log("E: {0} after argument -S contains an invalid port", args[sArgIdx + 1]);
        return;
      }
      mode = 2;
    } else if (args.Contains("-s")) {
      mode = 2;
    }

    int cArgIdx = Array.IndexOf(args, "-C");
    if (cArgIdx != -1) {
      if (cArgIdx + 1 < args.Length) {
        var arr = args[sArgIdx + 1].Split(":");
        hostname = arr[0];
        if (arr.Length > 1 && !UInt16.TryParse(arr[1], out connectPort)) {
          log("E: {0} after argument -C contains an invalid port", args[cArgIdx + 1]);
          return;
        }
      }
      mode = mode | 1;
    } else if (args.Contains("-c") || mode == 1) {
      mode = mode | 1;
    }

    int oArgIdx = Array.IndexOf(args, "-o");
    if (oArgIdx != -1) {
      int nameIdx = oArgIdx + 1;
      if (nameIdx < args.Length) {
        string name = args[nameIdx];
        clientOutput = Option<BinaryWriter>.Some(new BinaryWriter(File.Open($"{name}.client.rec", FileMode.Create, FileAccess.Write)));
        serverOutput = Option<BinaryWriter>.Some(new BinaryWriter(File.Open($"{name}.server.rec", FileMode.Create, FileAccess.Write)));
      } else {
        should_log = true;
        log("E: expected path to output after -o");
        return;
      }
    } 

    if (Array.IndexOf(args, "-Q") != -1)
      should_log = false;
    if (Array.IndexOf(args, "-q") != -1)
      log_packets = false;


    if (mode == 3 && hostname == "127.0.0.1" && listenPort == connectPort)  {
      log("W: client and server need different port when connecting to localhost changing server port to 11811");
      listenPort = 11811;
    }
        
    if ((mode & 1) != 0) {
      client = createClient(hostname, connectPort, out serverPeer);
      clientEvLoop = () => evloop(client, "C", ref clientQueue, ref serverQueue, false, clientOutput);
      state = 1;
    }
    if ((mode & 2) != 0) {
      server = createServer(listenPort);
      serverEvLoop = () => evloop(server, "S", ref serverQueue, ref clientQueue, true, serverOutput);
      state = 0;
    }
    Parallel.Invoke(serverEvLoop, clientEvLoop);
  }
}
