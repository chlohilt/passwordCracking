using System.CommandLine;

internal class Program {
    private static async Task<int> Main(string[] args) {
        var rootCommand = new RootCommand("Runs an echo server or client");

        // Command line arguments
        var portOption = new Option<ushort>(
            name: "--port",
            description: "The port to connect to.",
            getDefaultValue: () => 4000
        );

        var addressOption = new Option<string>(
            name: "--address",
            description: "The address to connect to",
            getDefaultValue: () => "localhost"
        );

        // Setup the client command
        var clientCommand = new Command("client", "Start an echo client") {
            portOption,
            addressOption
        };
        rootCommand.AddCommand(clientCommand);

        clientCommand.SetHandler(async (port, ip) => {
            await new EncryptedEchoClient(port, ip).Run();
        }, portOption, addressOption);

        // Setup the server command
        var serverCommand = new Command("server", "Start an echo server") {
            portOption
        };
        rootCommand.AddCommand(serverCommand);

        serverCommand.SetHandler(async (port) => await new EncryptedEchoServer(port).Run(),
            portOption);

        return await rootCommand.InvokeAsync(args);
    }
}