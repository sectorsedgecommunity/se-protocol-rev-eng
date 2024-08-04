# Sector's Edge server protocol
This is a description of Sector's Edge server protocol along with all the scripts that have been used to gather
information for the analysis of the protocol.

# Structure
All descriptions analyze the UDP stream and use a specific [notation](NOTATION.md)

1. [protocol-structure](protocol-structure) describes the general structure and nuances of the protocol
2. [messages](messages) describe every type of message sent between the client and the server

# Scripts
Further notes on the usage and troubleshooting of every script can be found in the comments put in the beginning of these scripts.

1. [dc.py](scripts/dc/dc.py)
   produces `{mapcode}-{gamemodecode}.pcapng` for each of the 140 - 2 bugged combinations of map-gamemode - files consiting of packets sent between a client and a local server over a 30 second period of the client connecting, spawning and disconnecting.  
2. [enet.cs](scripts/enet.cs)
   mimics a basic client, server or proxies traffic and collects and pretty-prints the packets received from the official server or the official client respectively.