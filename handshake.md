# Handshake
The handshake consists of 7 packets:
```
1:      client -> server
2-6:    server -> client
7:      client -> server
```

All packets sent from the client are of length 52, from the server - 48.

This is the general look of the UDP stream:
```
client
    server

00000000  4f ff 83 20 82 ff 00 01  00 00 ff ff 00 00 05 00
00000010  00 01 00 00 00 00 00 ff  00 00 00 00 00 00 00 00 
00000020  00 00 13 88 00 00 00 02  00 00 00 02 6d 6e e2 1e   
00000030  00 00 00 00                                        
    00000000  40 00 0e 26 83 ff 00 01  00 00 00 00 00 00 05 00
    00000010  00 01 00 00 00 00 00 ff  00 00 00 00 00 00 00 00 
    00000020  00 00 13 88 00 00 00 02  00 00 00 02 6d 6e e2 1e   
    00000030  40 00 0e 29 83 ff 00 01  00 00 00 00 00 00 05 00
    00000040  00 01 00 00 00 00 00 ff  00 00 00 00 00 00 00 00 
    00000050  00 00 13 88 00 00 00 02  00 00 00 02 6d 6e e2 1e   
    00000060  40 00 0e 2d 83 ff 00 01  00 00 00 00 00 00 05 00
    00000070  00 01 00 00 00 00 00 ff  00 00 00 00 00 00 00 00 
    00000080  00 00 13 88 00 00 00 02  00 00 00 02 6d 6e e2 1e   
    00000090  40 00 0e 31 83 ff 00 01  00 00 00 00 00 00 05 00
    000000A0  00 01 00 00 00 00 00 ff  00 00 00 00 00 00 00 00 
    000000B0  00 00 13 88 00 00 00 02  00 00 00 02 6d 6e e2 1e   
    000000C0  40 00 0e 35 83 ff 00 01  00 00 00 00 00 00 05 00
    000000D0  00 01 00 00 00 00 00 ff  00 00 00 00 00 00 00 00 
    000000E0  00 00 13 88 00 00 00 02  00 00 00 02 6d 6e e2 1e   
00000034  4f ff 83 32 82 ff 00 01  00 00 ff ff 00 00 05 00
00000044  00 01 00 00 00 00 00 ff  00 00 00 00 00 00 00 00 
00000054  00 00 13 88 00 00 00 02  00 00 00 02 6d 6e e2 1e   
00000064  00 00 00 00
```

Analyzing the stream:
```
00000000  t/ s/ 83 20 q/ x_1           . p   / x_2       
00000010   
00000020                                     . cid       ?   
00000030  o         *
---                                        
    00000000  t/ s/ 0e i> q/ x_1           . p   / x_2       
    00000010   
    00000020                                     . cid       ?
    ---   
    00000030  t/ s/ 0e i> q/ x_1           . p   / x_2       
    00000040   
    00000050                                     . cid       ?
    ---
    00000060  t/ s/ 0e i> q/ x_1           . p   / x_2       
    00000070   
    00000080                                     . cid       ?
    ---
    00000090  t/ s/ 0e i> q/ x_1           . p   / x_2       
    000000A0   
    000000B0                                     . cid       ?
    ---   
    000000C0  t/ s/ 0e i> q/ x_1           . p   / x_2       
    000000D0   
    000000E0                                     . cid       ?
---
00000034  t/ s/ 83 32 q/ x_1           . p   / x_2       
00000044   
00000054                                     . cid       ?   
00000064  o         *
```
**Known:**
- `t<` is a reliable ENet packet ID  
  40 from client  
  48 from server

**Partly known:**
- `s<` - always `ff` from client, `00` from server.  
  Purpose unknown.
- `q<` - always `82` from client, `83` from server.  
  Purpose unknown.
- `p<` - always `ff ff` from client, `00 00` from server.  
  Purpose unknown.
- `o<` - always `00 00 00 00`, only present in the client
  Purpose unknown.
- `<x_1>` and `<x_2>` - common information related to the handshake that is the same for every connection
  ```
  x_1 = ff 00 01 00 00
  x_2 = 00 00 05 00 00 01 00 00 00 00 00 ff 00 00 00 00 00 00 00 00 00 00 13 88 00 00 00 02 00 00 00 02
  ```
  The purpose of these is unknown
- `<i>` - increments from packet to packet sent from the server.
  Say, `i = xy` in the first packet sent from the server
  Then, in the last packet sent from the server, i is one of:
    1. `(x+1)y` - all packets increment by 4
    2. `(x+1)(y-1)` - one of the packets increment by 3 and every other by 4
- `<cid>` - unique id for every connection
  (here, `cid = 6d 6e e2 1e` but it does not matter)

**Unknown:** Everything unlabeled in the analysis