# Data Types
The messages Sector's Edge sends consist of various fixed and variable sized data types. Everything else is a composite of these.  
This document lists all of the types used.

## Fixed sized types
Some numeric values may represent enums
```
byte
u16 u32 u64 
i16 i32 i64
float double
bool          a single byte; non zero represents true
String [512]  a string with a fixed size of 512 bytes
```

## Variable sized types
```
byte[] u64[]  first u16 is the length of the list
String        first u16 is the length of the string
longString    first int is the length of the string
dictionaries  currently undocumented
```