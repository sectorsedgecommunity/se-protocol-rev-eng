# Data Types
The messages SE sends is made up various fixed and variable sized data types. All possible types are listed in the following. Everything else are just composites. Some numeric vaules may represent an enum.

# Fixed sized types
self explanatory

- byte
- u16 u32 u64
- i16 i32 i64
- float double

- bool          a single byte; non zero represents true
- String [512]  a string with a fixed sizo of 512 bytes

# Variable sized types
- byte[] u64[]  first u16 is the length of the list
- String        first u16 is the length of the string
- longString    first int is the length of the string
- dictionaries  currently undocumented