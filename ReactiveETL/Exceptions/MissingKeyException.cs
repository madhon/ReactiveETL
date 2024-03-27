namespace ReactiveETL.Exceptions
{
    /// <summary>
    /// Thrown when a an attempt to retrieve that a value by non existing key and
    /// the quacking dictionary is set to throw
    /// </summary>
    [global::System.Serializable]
    public class MissingKeyException : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingKeyException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public MissingKeyException(string message) : base("Could not find key: " + message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingKeyException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public MissingKeyException(string message, System.Exception inner) : base(message, inner)
        {
        }
    }
}