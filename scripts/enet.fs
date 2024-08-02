// USAGE:
//  download and unpack in the same folder:
//   https://github.com/nxrighthere/ENet-CSharp/releases/download/2.4.7/ENet-CSharp-2.4.7-x64.zip
//  please note that that is not the latest version ^, that is 2.4.7
//  this specific library is used because it is also used by Sector's Edge
//  compile with fsharpc -r:ENet-CSharp.dll enet.fs
//  run with -s to execute the server
//  run without arguments to execute the client

// TROUBLESHOOTING:
//  if the program ignores the -s flag and starts the client anyway
//  comment out the function `client` and rename the server function to something else
//  and replace the if clause in the main function with a call to that name
//  your main should look like this:
//  let main args =
//    printfn "Running"
//      new_server_fun_name
//        0
//  if you have done that, the -s flag is no longer needed

open ENet
open System
open System.Text

let filterChar (b : byte) = if int b <= 32 || int b > 127 then '.' else char b
        
let hexdump w (b : byte[]) =
  let format = sprintf "\t {0:X8}  {1,-%i} {2}" (w * 2 + w)
  let mapj (sb : StringBuilder * StringBuilder) x = 
    (fst sb).AppendFormat("{0:X2} ", x :> obj), 
    (snd sb).Append(filterChar x)
  seq { for i in 0 .. w .. (b.Length - 1) ->
          let (hex, asc) = 
            Array.sub b i (min w (b.Length - i))
            |> Array.fold (mapj) (StringBuilder(), StringBuilder())
          String.Format(format, i, hex, asc)
      }

let dump_received (event: Event) =
  let bytes: byte[] = Array.zeroCreate event.Packet.Length
  System.Runtime.InteropServices.Marshal.Copy(event.Packet.Data, bytes, 0, event.Packet.Length)
  hexdump 16 bytes |> String.concat "\n" |> printfn "Receive{channel: %A, dataLength: %A, data: \n%A\n\t}" event.ChannelID event.Packet.Length
  event.Packet.Dispose()

let rec evloop (host: Host) res (event: Event) =
  match event.Type with
    | EventType.None -> evloop host <|| host.Service(15)
    | EventType.Connect -> printfn "Connect{channel: %u, data: %u}" event.ChannelID event.Data; evloop host <|| host.Service(15)
    | EventType.Disconnect -> printfn "Disconnect{channel: %u, data: %u}" event.ChannelID event.Data
    | EventType.Receive -> dump_received event; evloop host <|| host.Service(15)
    | EventType.Timeout -> evloop host <|| host.Service(15)
    | x -> printfn "Unknown%A{channel: %u}" x event.ChannelID; evloop host <|| host.Service(15)

let client =
  let host = new Host()
  host.Create()
  let mutable addr: Address = new Address()
  addr.SetHost("127.0.0.1") |> ignore
  addr.Port <- 11810us
  printfn "Connecting to server..."
  let peer: Peer = host.Connect(addr)
  printfn "Connected to server!"
  evloop host <|| host.Service(15)

let server =
  let host = new Host()
  let mutable addr = new Address()
  addr.Port <- 11810us
  host.Create(addr, 1);
  printfn "Server listening"
  evloop host <|| host.Service(15)

[<EntryPoint>]
let main args =
  printfn "Running"
  if Array.contains "-s" args then
    server
  else
    client
  0
