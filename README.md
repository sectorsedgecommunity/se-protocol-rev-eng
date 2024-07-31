# Sector's Edge server protocol
This is a description of Sector's Edge server protocol along with all the scripts that have been used to gather
information for the analysis of the protocol.

# Described and to be described
All descriptions analyze the UDP stream and use a specific [notation](NOTATION.md)
- [x] [Handshake](handshake.md)
- [ ] Player information
- [ ] Map information
- [ ] ???

# Scripts
[dc.py](scripts/dc.py) collects information from 140 - 2(?) bugged + 5 custom combinations of map-gamemode  
If you wish to use this script, it needs to be put in the same folder as your server executable because otherwise the server will fail to run - the script will also fail to run because it detects the location of your server folder by the location of the file itself.  
This script produces a `{mapcode}-{gamemodecode}.pcapng` for each of the possible maps -- that is, maps that are located in the map folder alongside yours server executable. Possible gamemodes for each map are determined by the script itself.  
**NOTE 1:** The script requires the JSON files describing the maps to be perfect, as some checks were simplified and may give false positives if some things are not technically possible/correct  
**NOTE 2:** Run SE before the script and then switch to the window when it says to, because the script only gives you 10 seconds to do so.
