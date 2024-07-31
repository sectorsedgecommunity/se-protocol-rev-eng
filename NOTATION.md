# Notation
UDP streams are written down in the usual fashion:
```
8 digit (hex) number  16 bytes in hex and double spaces between each 8 bytes
00000000  00 00 00 00 00 00 00 00  ff ff ff ff ff ff ff ff
00000010  ...
```

Every analyzed part is notated with `type...closing tag` where `...` represents an arbitrary amount of spaces, and both `type` and `closing tag` are 1-char to allow notating one byte in a way that takes the same amount of space.

The list of types is different for every file and is specified in the file.

The list of closing tags is the same for each each file:
```
.   static
/   static but different for client->server and server->client packets
>   changing with a rule
?   dynamic (mostly used as unknown rule >)o
*   static, specific to either client->server or server->client, depending on the indentation
--  continuation of the previous tag(for multibyte elements)
```
