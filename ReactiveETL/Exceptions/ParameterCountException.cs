namespace ReactiveETL.Exceptions
{
    /// <summary>
    /// Thrown when an access to a quacking dictionary is made with more than a single
    /// parameter
    /// </summary>
    [global::System.Serializable]
    public class ParameterCountException : System.Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterCountException"/> class.
        /// </summary>
        public ParameterCountException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterCountException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ParameterCountException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterCountException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public ParameterCountException(string message, System.Exception inner) : base(message, inner)
        {
        }
    }
}