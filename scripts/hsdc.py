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
    
    for f in files:
        with open(f) as g:
            i = json.loads(g.read())
            if i["MapSize"][0] == 96 and i["MapSize"][2] == 160:
                add_path(f, "sta")
            if "LinearZones" in i and len(i["LinearZones"]) > 1:
                add_path(f, "cs")
                if check_teamspawns(i, "brk"):
                    add_path(f, "brk")
            if "FlagSpawns" in i and len(i["FlagSpawns"]) > 1: # read NOTE 1 for the script in README.md (1)
                if check_teamspawns(i, "ctf"):
                    add_path(f, "ctf")
                if check_teamspawns(i, "esc"):
                    add_path(f, "esc")
            if "RushStages" in i: # (1)
                add_path(f, "rush")
            if "WeaponSpawns" in i: # (1)
                add_path(f, "hh")

    print("OPEN SE WINDOW")
    time.sleep(10)
    print(gmdict)
    with open('{serverloc}\\lobby.json') as l:
        d = json.loads(l.read())
        for z, e in enumerate(gmdict):
            if os.path.isfile(f"{server_loc}\\lobby{z+1}.json"):
                continue
            with open(f"{server_loc}\\lobby{z+1}.json", "w") as w:
                d["GameModes"]=[e[1]]
                d["Maps"]=[e[0]]
                w.write(json.dumps(d))
            p = psutil.Popen([f"{server_loc}\\sectorsedgeserver.exe", f"{z+1}"])
            capture_thread = multiprocessing.Process(target = start_capture, args = (f"{e[0]}-{e[1]}.pcapng",))
            capture_thread.start()
            time.sleep(5)
            pyautogui.click(700, 20)
            pyautogui.click(760, 220)
            pyautogui.press('backspace', presses = 20)
            pyautogui.write(f'127.0.0.1:{11811 + z}')
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
            p.terminate()