/// <summary>
/// Structure that holds data related to an encrypted message sent from the client.
/// 
/// Can be serialized to and from JSON using <see cref="System.Text.Json.JsonSerializer" />.
/// </summary>
readonly struct EncryptedMessage {
    public byte[] AesKeyWrap { get; init; }
    public byte[] AESIV { get; init; }
    public byte[] Message { get; init; }
    public byte[] HMACKeyWrap { get; init; }
    public byte[] HMAC { get; init; }

    public EncryptedMessage(byte[] aesKeyWrap, byte[] aesIv, byte[] message, byte[] hmacKeyWrap, byte[] hmac) {
        this.AesKeyWrap = aesKeyWrap;
        this.AESIV = aesIv;
        this.Message = message;
        this.HMACKeyWrap = hmacKeyWrap;
        this.HMAC = hmac;
    }
}

/// <summary>
/// Structure that holds data related to a signed message sent from the server.
/// 
/// Can be serialized to and from JSON using <see cref="System.Text.Json.JsonSerializer" />.
/// </summary>
readonly struct SignedMessage {
    public byte[] Message { get; init; }
    public byte[] Signature { get; init; }

    public SignedMessage(byte[] message, byte[] signature) {
        this.Message = message;
        this.Signature = signature;
    }
}