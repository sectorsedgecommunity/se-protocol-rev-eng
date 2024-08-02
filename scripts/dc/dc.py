"""
IMPORTANT:
    THIS SCRIPT WILL MAKE YOUR COMPUTER UNUSABLE FOR 72.5 MINUTES!
USAGE:
    make sure your display is 1920x1080 and sector's edge is in fullscreen
    1. install the requirements placed alongside the script:
           pip install -r requirements.txt
    2. put the script in the same folder as your server executable
    3. open sector's edge
    3. run it!
    4. switch into the game window when it tells you to
    4? if you wish to restart the script from scratch, delete all the 
       lobby(x).json files it generated in your se folder
TROUBLESHOOTING:
    1. the script will break on crossing breakthrough (cr-brk)
       and reactor breakthrough (r-brk)
       when this happens, simply restart the script - it doesnt collect data 
       again if the lobby(x).json for the combination is present
       (presuming they combinations stay in the same order! as they are
       regenerated every run)
    2. the script requires the jsons for the maps to be perfect!
       this includes:
          1. no comments, python doesnt know how to handle them
          2. no useless rushspawns, flagspawns, weaponspawns, teamspawns
       if there are such issues, the script will assume some impossible 
       gamemodes for maps are actually deemed possible due to simplified checks
"""

import json
import os
import pyshark
from itertools import chain
import multiprocessing
import time
import pyautogui
import psutil

def start_capture(fname):
    capture = pyshark.LiveCapture(interface = '\\Device\\NPF_Loopback', output_file = fname)
    capture.sniff(timeout=30)
    capture
def get_path(f):
    return f.split("\\")[-1][:-5]
def check_teamspawns(i, k):
    if "TeamSpawns" in i:
        c = 0
        for j in i["TeamSpawns"]:
            if "GameMode" in j and (j["GameMode"] == k or j["GameMode"] == "all"):
                c += 1
            if c > 1:
                return True
def add_path(f, m):
    gmdict.append((get_path(f),m))
    print(m + " " + f)
if __name__ == '__main__':
    serverloc = os.path.dirname(os.path.realpath(__file__))

    files = [os.path.join(dp, f) 
            for dp, dn, filenames in os.walk(f"{serverloc}\\map") 
            for f in filenames if os.path.splitext(f)[1] == '.json']

    gmdict = list(chain.from_iterable(((get_path(f),"gg"),(get_path(f),"ffa"),(get_path(f),"tdm")) for f in files))
    
    for mapfile in files:
        with open(mapfile) as g:
            mapjson = json.loads(g.read())
            if mapjson["MapSize"][0] == 96 and mapjson["MapSize"][2] == 160:
                add_path(mapfile, "sta")
            if "LinearZones" in mapjson and len(mapjson["LinearZones"]) > 1:
                add_path(mapfile, "cs")
                if check_teamspawns(mapjson, "brk"):
                    add_path(mapfile, "brk")
            if "FlagSpawns" in mapjson and len(mapjson["FlagSpawns"]) > 1: # TROUBLESHOOTING 2.2
                if check_teamspawns(mapjson, "ctf"):
                    add_path(mapfile, "ctf")
                if check_teamspawns(mapjson, "esc"):
                    add_path(mapfile, "esc")
            if "RushStages" in mapjson: # TROUBLESHOOTING 2.2
                add_path(mapfile, "rush")
            if "WeaponSpawns" in mapjson: # TROUBLESHOOTING 2.2
                add_path(mapfile, "hh")

    print("OPEN SE WINDOW")
    time.sleep(10)
    print(gmdict)
    with open(f'{serverloc}\\lobby.json') as l:
        default_json = json.loads(l.read())
        default_json["FillWithBots"] = False
        for i, game_pair in enumerate(gmdict):
            if os.path.isfile(f"{serverloc}\\lobby{i+1}.json"):
                continue
            with open(f"{serverloc}\\lobby{i+1}.json", "w") as w:
                default_json["GameModes"]=[game_pair[1]]
                default_json["Maps"]=[game_pair[0]]
                w.write(json.dumps(default_json))

            server_process = psutil.Popen([f"{serverloc}\\sectorsedgeserver.exe", f"{i+1}"])

            capture_thread = multiprocessing.Process(target = start_capture, args = (f"{game_pair[0]}-{game_pair[1]}.pcapng",))
            capture_thread.start()
            time.sleep(5)
            
            pyautogui.click(700, 20)
            pyautogui.click(760, 220)
            pyautogui.press('backspace', presses = 20)
            pyautogui.write(f'127.0.0.1:{11811 + i}')
            pyautogui.click(760, 370)
            time.sleep(10)
            pyautogui.click(1,1)
            time.sleep(5)
            pyautogui.press('space')
            time.sleep(5)
            pyautogui.press('esc')
            pyautogui.click(1000, 660)
            pyautogui.click(1000, 660)

            capture_thread.join()
            server_process.terminate()