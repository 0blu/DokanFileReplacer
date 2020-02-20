# Dokan File Replacer

This is just a simple project to create a virtual filesystem with [Dokan](https://github.com/dokan-dev/dokan-dotnet)
which is able to replace files on the fly (even if there are already opened from another process).

Works best with access behaviors that open a file just once (like `File.ReadAllText(...)`).

## Example

The example shows how to use this library to intercept every second request that is made.

Output:
```
--- Reading files with File.ReadAllText("R:\hello world.txt") ---
[PROXY] File '\hello world.txt' was requested
Output 1: >> 'Just hello world' <<
[PROXY] File '\hello world.txt' was requested
[PROXY] Replace file!
Output 2: >> 'Definitely not the real file!' <<
...
```

---

The example mirror driver from Dokan was used and modified for this project.

License: [Apache License 2.0](./LICENSE.txt)
