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
            if "FlagSpawns" in mapjson and len(mapjson["FlagSpawns"]) > 1: # read NOTE 3 for the script in README.md (1)
                if check_teamspawns(mapjson, "ctf"):
                    add_path(mapfile, "ctf")
                if check_teamspawns(mapjson, "esc"):
                    add_path(mapfile, "esc")
            if "RushStages" in mapjson: # (1)
                add_path(mapfile, "rush")
            if "WeaponSpawns" in mapjson: # (1)
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