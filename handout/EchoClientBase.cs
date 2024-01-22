using System.Net.Sockets;
using Microsoft.Extensions.Logging;

/// <summary>
/// Base class for implementing an Echo client.
/// </summary>
internal abstract class EchoClientBase {

    /// <summary>
    /// Logger to use in this class.
    /// </summary>
    private ILogger<EchoClientBase> Logger { get; init; } = 
        Settings.LoggerFactory.CreateLogger<EchoClientBase>()!;

    /// <summary>
    /// Port to which the echo client will connect to.
    /// </summary>
    private ushort Port { get; init; }

    /// <summary>
    /// Address the echo client will connect to.
    /// </summary>
    private string Address { get; init; }

    /// <summary>
    /// Creates an echo client.
    /// </summary>
    /// <param name="port">Port to which the echo client will connect to.</param>
    /// <param name="address">Address of the echo server</param>
    /// <returns></returns>
    protected EchoClientBase(ushort port, string address) => (Port, Address) = (port, address);

    /// <summary>
    /// Run the echo client.
    /// </summary>
    public async Task Run() {
        // Connect to the server
        var client = new TcpClient();
        await client.ConnectAsync(Address, Port);
        Logger.LogInformation("Connected to server {endpont}", client.Client.RemoteEndPoint);

        var reader = new StreamReader(client.GetStream(), Settings.Encoding);
        var writer = new StreamWriter(client.GetStream(), Settings.Encoding) {
            AutoFlush = true
        };

        // Close the client if ctrl+c is pressed
        Console.CancelKeyPress += (_, _) => CloseClient(client);

        try {
            // Connect and get the server hello message
            var helloMessage = await reader.ReadLineAsync();
            if (helloMessage is null) return;
            ProcessServerHello(helloMessage);

            // Take messages from the console and write to the echo server
            var sendMessage = Task.Run(async () => {
                string? input;
                while (!string.IsNullOrEmpty(input = Console.ReadLine())) {
                    Logger.LogDebug("From console: {input}", input);

                    string? message = null;
                    try {
                        message = TransformOutgoingMessage(input);
                    } catch (Exception e) {
                        Logger.LogError("Error while processing outgoing message: {e}", e);
                        continue;
                    }

                    Logger.LogDebug("To server: {outgoingMessage}", message!);
                    if (input is not null) await writer.WriteLineAsync(message!);
                }
            });

            // Receive messages from the echo server and write to the console
            var receiveMessage = Task.Run(async () => {
                string? input;
                while ((input = await reader.ReadLineAsync()) is not null) {
                    Logger.LogDebug("From server: {input}", input);

                    try {
                        var message = TransformIncomingMessage(input);

                        Logger.LogDebug("To console: {outgoingMessage}", message);
                        Console.WriteLine(message);
                    } catch (Exception e) {
                        Logger.LogError("Error when processing incoming message: {e}", e);
                        continue;
                    }
                }
            });

            // Once either task exists, close the connection
            Task.WaitAny(sendMessage, receiveMessage);

        } finally {
            CloseClient(client);
        }
    }

    /// <summary>
    /// Closes the client.
    /// </summary>
    /// <param name="client">Client to close.</param>
    private void CloseClient(TcpClient client) {
        client.Close();
        Logger.LogInformation("Client socket closed");
    }

    /// <summary>
    /// Process the hello message from the server.
    /// </summary>
    /// <param name="message">Message to process.</param>
    public abstract void ProcessServerHello(string message);

    /// <summary>
    /// Prepare a message to be sent to the echo server.
    /// </summary>
    /// <param name="input">Message read from the console.</param>
    /// <returns>Message to send to the server.</returns>
    public abstract string TransformOutgoingMessage(string input);

    /// <summary>
    /// Process a message received from the echo server.
    /// </summary>
    /// <param name="input">Message received from the echo server.</param>
    /// <returns>Message to write to the console.</returns>
    public abstract string TransformIncomingMessage(string input);
}