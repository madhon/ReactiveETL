using ReactiveETL.Activators;

namespace ReactiveETL.Operations.File
{
    /// <summary>
    /// Operation to write to a file
    /// </summary>
    public class FileWriteOperation<T> : AbstractOperation
    {
        private FileWriteActivator<T> _activator;

        /// <summary>
        /// File Write constructor
        /// </summary>
        /// <param name="activator">file write parameters</param>
        public FileWriteOperation(FileWriteActivator<T> activator)
        {
            _activator = activator;
        }

        /// <summary>
        /// Method called by OnNext > Dispatch to process the notified value. This method just return the value and could be overriden in subclasses
        /// </summary>
        /// <param name="value">pipelined value</param>
        /// <returns>treated row</returns>
        protected override Row TreatRow(Row value)
        {
            _activator.InitializeEngine();
            _activator.Engine.Write(value.ToObject<T>());

            return base.TreatRow(value);
        }

        /// <summary>
        /// Notifies the observer of the end of the sequence.
        /// </summary>
        public override void OnCompleted()
        {
            _activator.Release();
            base.OnCompleted();
        }
    }
}