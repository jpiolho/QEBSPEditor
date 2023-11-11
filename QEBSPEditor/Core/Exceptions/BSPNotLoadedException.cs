using System.Runtime.Serialization;

namespace QEBSPEditor.Core.Exceptions
{
    public class BSPNotLoadedException : Exception
    {
        public BSPNotLoadedException() : base("No BSP loaded")
        {
        }

        public BSPNotLoadedException(string? message) : base(message)
        {
        }

        public BSPNotLoadedException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected BSPNotLoadedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
