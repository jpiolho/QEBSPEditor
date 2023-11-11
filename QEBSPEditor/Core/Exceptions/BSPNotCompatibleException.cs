using System.Runtime.Serialization;

namespace QEBSPEditor.Core.Exceptions;

public class BSPNotCompatibleException : Exception
{
    public BSPNotCompatibleException() : base("The BSP is not compatible with this operation")
    {
    }

    public BSPNotCompatibleException(string? message) : base(message)
    {
    }

    public BSPNotCompatibleException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected BSPNotCompatibleException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
