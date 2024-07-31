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
1. [dc.py](scripts/dc.py)
collects information from 140 - 2(?) bugged + 5 custom combinations of map-gamemode.  
If you want to use this script, it needs to be put in the same folder as your server executable because otherwise the server will fail to run - the script will also fail to run because it detects the location of your server folder by the location of the file itself.  
This script produces a `{mapcode}-{gamemodecode}.pcapng` for each of the possible maps -- that is, maps that are located in the map folder alongside your server executable. Possible gamemodes for each map are determined by the script itself.  
**NOTE 0!** The script will make your computer unusable for 72.5 minutes. There are some (admittedly, probably useless) timeouts in the script to make sure the automation works properly.  
**NOTE 1!** The script will break when running Breakthrough on Crossing(cr-brk) and Breakthrough on Reactor - this is a problem with the official SE server. When you reach these, just restart the script. The script checks if the lobby json files for the mode+gamemode combination has already been generated and skips it if it has.  
**NOTE 2!** Run SE before the script and then switch to the window when it says to, because the script only gives you 10 seconds to do so.  
**NOTE 3:** The script requires the JSON files describing the maps to be perfect, as some checks were simplified and may give false positives if some things are not technically possible/correct