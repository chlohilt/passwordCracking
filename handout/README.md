# Encryption Project

In this project, you will be implementing an encrypted echo server and client. All of your code will live in `EncryptedEchoClient.cs` and `EncryptedEchoServer.cs`. Feel free to look at the other files to get a sense of how the echo server works.

## Running

* To build, run `dotnet build`.
* To run the server, use `dotnet run server --no-build`
* To run the client, use `dotnet run client --no-build`

## Testing
To aid you in testing your code, I've provided an `echo-test` binary. It contains a working encrypted echo server and client. Run them using `./echo-test server` and `./echo-test client`, respectively. This can be used to test your own implementations. If you can run against these tools without any issue, you should receive full credit on passoff (unless you cut corners around key generation). If you are using an arm device (like the mac M1/M2) use `echo-test-arm`.
