﻿namespace ApplicationInsightsTracer.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class OperationAlreadyStartedException : Exception
    {
        // Constructor form 1 ()
        /// <summary>
        /// Creates a new instance of the <see cref="OperationAlreadyStartedException"/> class with no arguments.
        /// </summary>
        public OperationAlreadyStartedException()
        {
        }

        // Constructor form 2 (message)
        /// <summary>
        /// Creates a new instance of the <see cref="OperationAlreadyStartedException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name='message'>The message that explains the reason for the exception.</param>
        public OperationAlreadyStartedException(string message)
            : base(message)
        {
        }

        // Constructor form 3 (message, exception)
        /// <summary>
        /// Creates a new instance of the <see cref="OperationAlreadyStartedException"/> class
        /// with a specified error message and a reference to an inner exception.
        /// </summary>
        /// <param name='message'>The message that explains the reason for the exception.</param>
        /// <param name='innerException'>The exception that is the cause of the current exception.
        /// If it is not a null reference, the current exception is raised in a catch block
        /// that handles the inner exception.</param>
        public OperationAlreadyStartedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        // Constructor form 4 (info, context)
        /// <summary>
        /// Creates a new instance of the <see cref="OperationAlreadyStartedException"/> class
        /// with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SeraizliationInfo"/> that holds the serialized
        /// object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual
        /// information about the source or destination.</param>
        public OperationAlreadyStartedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}