using System.IO;
using ReactiveETL.Operations.File;
using ReactiveETL.Activators;
using System;
using ReactiveETL.Files;

namespace ReactiveETL
{
    /// <summary>
    /// Extension Methods for files
    /// </summary>
    public static class FileExtensions
    {
        /// <summary>
        /// Write the result to a file
        /// </summary>
        /// <typeparam name="T">The poco type used to define file format</typeparam>
        /// <param name="observed">observed operation</param>
        /// <param name="filename">full path to the file</param>
        /// <returns>file write operation</returns>
        public static FileWriteOperation<T> WriteFile<T>(this IObservableOperation observed, string filename) => observed.WriteFile<T>(filename, null);

        /// <summary>
        /// Write the result to a file
        /// </summary>
        /// <typeparam name="T">The poco type used to define file format</typeparam>
        /// <param name="observed">observed operation</param>
        /// <param name="filename">full path to the file</param>
        /// <param name="prepare">Callback to prepare the file engine</param>
        /// <returns>file write operation</returns>
        public static FileWriteOperation<T> WriteFile<T>(this IObservableOperation observed, string filename, Action<FluentFile> prepare)
        {
            var activator = new FileWriteActivator<T>() { FileName = filename, PrepareFluentFile = prepare };
            var resoperation = new FileWriteOperation<T>(activator);
            observed.Subscribe(resoperation);
            return resoperation;
        }

        /// <summary>
        /// Write the result to a stream
        /// </summary>
        /// <typeparam name="T">The poco type used to define file format</typeparam>
        /// <param name="observed">observed operation</param>
        /// <param name="strm">stream to the file</param>
        /// <returns>file write operation</returns>
        public static FileWriteOperation<T> WriteFile<T>(this IObservableOperation observed, Stream strm) => observed.WriteFile<T>(strm, null);

        /// <summary>
        /// Write the result to a stream
        /// </summary>
        /// <typeparam name="T">The poco type used to define file format</typeparam>
        /// <param name="observed">observed operation</param>
        /// <param name="strm">stream to the file</param>
        /// <param name="prepare">Callback to prepare the file engine</param>
        /// <returns>file write operation</returns>
        public static FileWriteOperation<T> WriteFile<T>(this IObservableOperation observed, Stream strm, Action<FluentFile> prepare)
        {
            var activator = new FileWriteActivator<T>() { Stream = strm, PrepareFluentFile = prepare };
            var resoperation = new FileWriteOperation<T>(activator);
            observed.Subscribe(resoperation);
            return resoperation;
        }

        /// <summary>
        /// Write the result to a stream
        /// </summary>
        /// <typeparam name="T">The poco type used to define file format</typeparam>
        /// <param name="observed">observed operation</param>
        /// <param name="writer">stream to the file</param>
        /// <returns>file write operation</returns>
        public static FileWriteOperation<T> WriteFile<T>(this IObservableOperation observed, TextWriter writer) => observed.WriteFile<T>(writer, null);

        /// <summary>
        /// Write the result to a stream
        /// </summary>
        /// <typeparam name="T">The poco type used to define file format</typeparam>
        /// <param name="observed">observed operation</param>
        /// <param name="writer">stream to the file</param>
        /// <param name="prepare">Callback to prepare the file engine</param>
        /// <returns>file write operation</returns>
        public static FileWriteOperation<T> WriteFile<T>(this IObservableOperation observed, TextWriter writer, Action<FluentFile> prepare)
        {
            var activator = new FileWriteActivator<T>() { Writer = writer, PrepareFluentFile = prepare };
            var resoperation = new FileWriteOperation<T>(activator);
            observed.Subscribe(resoperation);
            return resoperation;
        }
    }
}
