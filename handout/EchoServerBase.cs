using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;

/// <summary>
/// Base class for implementing an echo server.
/// </summary>
internal abstract class EchoServerBase {

    /// <summary>
    /// Logger to use in this class.
    /// </summary>
    private ILogger<EchoServerBase> Logger { get; init; } =
        Settings.LoggerFactory.CreateLogger<EchoServerBase>()!;

    /// <summary>
    /// Port from which to accept echo client connections.
    /// </summary>
    private ushort Port { get; set; }

    /// <summary>
    /// Create an echo server.
    /// </summary>
    /// <param name="port">Port from which to accept echo client connections.</param>
    protected EchoServerBase(ushort port) => Port = port;

    /// <summary>
    /// Run the echo server.
    /// </summary>
    public async Task Run() {
        // Create a local server socket
        TcpListener listener = new(IPAddress.Any, Port);
        listener.Start();
        Logger.LogInformation("Server started on port {port}", Port);

        // Close the server if ctrl+c is pressed
        Console.CancelKeyPress += (_, _) => CloseListener(listener);

        // Listen for new connections, spawning new asynchronous handler for each
        try {
            while (true) {
                var client = await listener.AcceptTcpClientAsync();
                _ = HandleClient(client);
            }
        } finally {
            CloseListener(listener);
        }
    }

    /// <summary>
    /// Handle echo clients.
    /// </summary>
    /// <param name="client">Client to handle.</param>
    async Task HandleClient(TcpClient client) {
        var clientName = client.Client.RemoteEndPoint!.ToString()!;
        Logger.LogInformation("[{clientName}]: Connected", clientName);

        var reader = new StreamReader(client.GetStream(), Settings.Encoding);
        var writer = new StreamWriter(client.GetStream(), Settings.Encoding) {
            AutoFlush = true
        };

        // Close the client if ctrl+c is pressed
        void CloseClientHandler(object? sender, ConsoleCancelEventArgs e) => CloseClient(client, clientName);
        Console.CancelKeyPress += CloseClientHandler;

        try {
            await writer.WriteLineAsync(GetServerHello());

            // Read data in and echo it back out
            string? input;
            while ((input = await reader.ReadLineAsync()) is not null) {
                Logger.LogDebug("[{clientName}]: From client: {input}", input);

                string? incomingMessage = null;
                try {
                    incomingMessage = TransformIncomingMessage(input);
                    Logger.LogDebug("[{clientName}]: Message: {message}", incomingMessage);
                } catch (Exception e) {
                    Logger.LogError("[{clientName}]: Error when processing an incoming message: {e}", clientName, e);
                    continue;
                }

                string? outgoingMessage;
                try {
                    outgoingMessage = TransformOutgoingMessage(incomingMessage!);
                } catch (Exception e) {
                    Logger.LogError("[{clientName}]: Error when processing an outgoing message: {e}", clientName, e);
                    continue;
                }

                Logger.LogDebug("To server: {outgoingMessage}", outgoingMessage);
                if (input is not null) await writer.WriteLineAsync(outgoingMessage!);
            }
        } finally {
            Console.CancelKeyPress -= CloseClientHandler;
            CloseClient(client, clientName);
        }
    }

    /// <summary>
    /// Closes the listener.
    /// </summary>
    /// <param name="listener">Listener to close.</param>
    private void CloseListener(TcpListener listener) {
        listener.Server.Close();
        Logger.LogInformation("Server socket closed");
    }

    /// <summary>
    /// Closes the client.
    /// </summary>
    /// <param name="client">Client to close.</param>
    /// <param name="clientName">Name of the client.</param>
    private void CloseClient(TcpClient client, string clientName) {
        client.Close();
        Logger.LogInformation("[{clientName}]: Client socket closed", clientName);
    }

    /// <summary>
    /// Get the message to send to new clients.
    /// </summary>
    /// <returns>Message to send to new clients.</returns>
    public abstract string GetServerHello();

    /// <summary>
    /// Process a message received from an echo client.
    /// </summary>
    /// <param name="input">Message received from an echo client.</param>
    /// <returns>Message to return to the echo client.</returns>
    public abstract string TransformIncomingMessage(string input);

    /// <summary>
    /// Prepare a message to be sent to an echo client.
    /// </summary>
    /// <param name="input">Message to prepare.</param>
    /// <returns>Message to send to the echo client.</returns>
    public abstract string TransformOutgoingMessage(string input);
}