
public class InvalidSignatureException : SystemException {

    public InvalidSignatureException() { }

    public InvalidSignatureException(string message) : base(message) { }

    public InvalidSignatureException(string message, Exception inner) : base(message, inner) { }
}