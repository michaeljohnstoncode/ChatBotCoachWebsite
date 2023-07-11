using System.Runtime.Serialization;

namespace ChatBotCoach_logic.Clients
{
    /// <summary>
    /// Allows the user to access the security excpetion class to specify instances of specific security violations as errors.
    /// </summary>
    [Serializable]
    internal class SecurityException : Exception
    {
        public SecurityException()
        {
        }

        public SecurityException(string? message) : base(message)
        {
        }

        public SecurityException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected SecurityException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}