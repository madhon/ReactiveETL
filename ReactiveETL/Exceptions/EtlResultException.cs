using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ReactiveETL.Exceptions
{
    
    /// <summary>
    /// Result of ETL operation error
    /// </summary>
    public class EtlResultException : ReactiveETLException
    {
        /// <summary>
        /// Operation Result
        /// </summary>
        public EtlResult EtlResult { get; private set; }

        /// <summary>
        /// Throw exception from result
        /// </summary>
        /// <param name="result"></param>
        public static void From(EtlResult result)
        {
            if (result.CountExceptions > 0)
            {
                if (result.CountExceptions == 1)
                {
                    throw new EtlResultException(result, "Unexpected operation exception", result.Exceptions.First());
                }
                else
                {
                    throw new EtlResultException(result);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveETLException"/> class.
        /// </summary>
        /// <param name="result">The operation result.</param>
        public EtlResultException(EtlResult result)
            : base() =>
            EtlResult = result;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveETLException"/> class.
        /// </summary>
        /// <param name="result">The operation result.</param>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public EtlResultException(EtlResult result, string message, Exception inner) : base(message, inner) => EtlResult = result;

        /// <summary>
        /// Creates and returns a string representation of the current exception.
        /// </summary>
        /// <returns>
        /// A string representation of the current exception.
        /// </returns>
        /// <filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" PathDiscovery="*AllFiles*"/></PermissionSet>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Operation result has error(s) :");
            foreach (var exception in EtlResult.Exceptions)
            {
                sb.AppendLine(exception.ToString());
            }
            return sb.ToString();
        }
    }
}
