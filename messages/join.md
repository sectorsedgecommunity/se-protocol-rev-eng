# join (JoiningMessage)
This is the first message sent by the client to the server and signals joining the game after connecting to the server.

# Definition
```C
struct {
  byte type = 56;
  string version;
  string password;
  bool notSpectating;
  string bearer; // unused(steam play)
  string player_name;
  string _s1;
  u64_list _ul1;
  string _s2;
  byte character_type;
  byte _e1; // weapon skin?
  byte _b1;
  byte _b2;
  byte _b3;
  u16 _il1_len;
  int[5*5*5*_il1_len] _il1;
}
```

# Example
``` 
00000000  56 06 00 32 2E 34 2E 35 62 00 00 00 FC 00 30 38  V..2.4.5b.....08
00000010  30 31 31 30 41 31 46 32 38 37 41 45 30 31 31 38  0110A1F287AE0118
00000020  30 36 32 30 34 30 32 41 37 30 44 43 37 31 30 41  0620402A70DC710A
00000030  30 38 43 42 41 43 33 43 42 45 38 45 36 42 42 37  08CBAC3CBE8E6BB7
00000040  42 33 41 35 45 36 32 32 44 46 38 46 42 31 39 37  B3A5E622DF8FB197
00000050  39 39 46 34 39 34 33 46 31 38 41 37 38 31 39 41  99F4943F18A7819A
00000060  33 41 32 36 45 36 36 38 37 33 42 32 34 34 33 33  3A26E66873B24433
00000070  31 38 38 30 35 33 38 35 33 45 34 43 46 43 33 43  188053853E4CFC3C
00000080  34 33 42 30 41 31 34 35 39 41 43 32 46 30 38 46  43B0A1459AC2F08F
00000090  45 38 42 37 42 41 30 41 42 43 44 34 41 33 39 46  E8B7BA0ABCD4A39F
000000A0  35 41 46 38 34 33 43 33 46 33 31 44 43 44 36 34  5AF843C3F31DCD64
000000B0  39 44 45 31 34 38 36 39 37 43 43 37 39 46 38 41  9DE148697CC79F8A
000000C0  35 31 42 39 42 31 38 34 41 37 39 31 43 42 41 43  51B9B184A791CBAC
000000D0  46 35 31 35 37 37 43 39 36 34 45 38 33 43 35 46  F51577C964E83C5F
000000E0  42 44 35 38 38 44 36 35 45 46 37 33 33 44 32 35  BD588D65EF733D25
000000F0  46 38 30 31 41 43 45 45 36 32 45 31 39 35 36 30  F801ACEE62E19560
00000100  32 38 36 46 36 38 45 32 35 44 06 00 53 75 72 6D  286F68E25D..Surm
00000110  61 6E 00 00 00 20 00 30 30 30 32 31 35 66 39 66  an.....000215f9f
00000120  66 31 66 34 30 33 61 61 32 33 32 62 33 33 61 31  f1f403aa232b33a1
00000130  37 37 31 64 61 65 34 00 05 94 FF FF 03 00 02 00  771dae4.........
00000140  00 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
00000150  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
00000160  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
00000170  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
00000180  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
00000190  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
000001A0  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
000001B0  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
000001C0  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
000001D0  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
000001E0  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
000001F0  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
00000200  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
00000210  20 01 00 00 20 01 00 00 20 01 00 00 00 00 00 00  ................
00000220  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
00000230  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
00000240  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
00000250  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
00000260  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
00000270  20 07 00 00 20 07 00 00 20 07 00 00 20 07 00 00  ................
00000280  20 07 00 00 20 07 00 00 20 07 00 00 20 07 00 00  ................
00000290  20 07 00 00 20 07 00 00 20 07 00 00 20 07 00 00  ................
000002A0  20 07 00 00 20 07 00 00 20 07 00 00 00 00 00 00  ................
000002B0  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
000002C0  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
000002D0  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
000002E0  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
000002F0  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
00000300  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
00000310  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
00000320  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
00000330  00 00 00 00 00 00 00 00 00 00 01 00 02 00 00 00  ................
00000340  00 00 20 0C 00 00 20 0C 00 00 20 0C 00 00 00 00  ................
00000350  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
00000360  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
00000370  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
00000380  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
00000390  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
000003A0  00 00 00 00 00 00 20 0C 00 00 20 0C 00 00 20 0C  ................
000003B0  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
000003C0  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
000003D0  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
000003E0  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
000003F0  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
00000400  00 00 00 00 00 00 00 00 00 00 20 0C 00 00 20 0C  ................
00000410  00 00 20 0C 00 00 00 00 00 00 00 00 00 00 00 00  ................
00000420  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
00000430  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
00000440  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
00000450  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
00000460  00 00 00 00 00 00 00 00 00 00 00 00 00 00 20 0C  ................
00000470  00 00 20 0C 00 00 20 0C 00 00 00 00 00 00 00 00  ................
00000480  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
00000490  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
000004A0  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
000004B0  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
000004C0  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
000004D0  00 00 20 0C 00 00 20 0C 00 00 20 0C 00 00 00 00  ................
000004E0  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
000004F0  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
00000500  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
00000510  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
00000520  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
00000530  00 00 00 00 01 00 02 00 00 00 00 00 20 0C 00 00  ................
00000540  20 0C 00 00 20 0C 00 00 00 00 00 00 00 00 00 00  ................
00000550  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
00000560  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
00000570  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
00000580  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
00000590  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
000005A0  20 0C 00 00 20 0C 00 00 20 0C 00 00 00 00 00 00  ................
000005B0  00 00 00 00 20 0C 00 00 20 0C 00 00 20 0C 00 00  ................
000005C0  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
000005D0  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
000005E0  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
000005F0  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
00000600  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
00000610  00 00 00 00 00 00 00 00 20 0C 00 00 20 0C 00 00  ................
00000620  20 0C 00 00 00 00 00 00 00 00 00 00 20 0C 00 00  ................
00000630  20 0C 00 00 20 0C 00 00 00 00 00 00 00 00 00 00  ................
00000640  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
00000650  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
00000660  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
00000670  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
00000680  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
00000690  20 0C 00 00 20 0C 00 00 20 0C 00 00 00 00 00 00  ................
000006A0  00 00 00 00 20 0C 00 00 20 0C 00 00 20 0C 00 00  ................
000006B0  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
000006C0  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
000006D0  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
000006E0  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
000006F0  00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00  ................
00000700  00 00 00 00 00 00 00 00 20 0C 00 00 20 0C 00 00  ................
00000710  20 0C 00 00 00 00 00 00 00 00 00 00 20 0C 00 00  ................
00000720  20 0C 00 00 20 0C 00 00 00 00 00 00 01 00 01 00  ................
00000730  00 00 FF FF FF FF FF 00 00 00 00 00 00 00 00 00  ................
00000740  00 00 00 20                                      ....
```