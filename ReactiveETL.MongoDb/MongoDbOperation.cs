namespace ReactiveETL
{
    using System;
    using Activators;
    using Microsoft.Extensions.Logging;
    using MongoDB.Driver;

    public partial class MongoDbUpdateOperation<T> : AbstractOperation
    {
        private readonly ILogger log;

        private CommandActivator _activator;

        private IMongoDatabase database;
        private string collectionName;
        private Func<Row, FilterDefinition<T>> filter;
        private Func<Row, UpdateDefinition<T>> update;
        private UpdateOptions options;

        public MongoDbUpdateOperation(CommandActivator activator,
            IMongoDatabase database,
            string collectionName,
            Func<Row, FilterDefinition<T>> filter,
            Func<Row, UpdateDefinition<T>> update,
            ILogger log,
            UpdateOptions options = null
            )
        {
            _activator = activator;
            this.database = database;
            this.collectionName = collectionName;
            this.filter = filter;
            this.update = update;
            this.options = options;
            this.log = log;
        }

        /// <summary>
        /// Notifies the observer of a new value in the sequence. It's best to override Dispatch or TreatRow than this method because this method contains pipeline logic.
        /// </summary>
        public override void OnNext(Row value)
        {
            CountTreated++;

            using (var session = this.database.Client.StartSession())
            {
                try
                {
                    var _filter = filter(value);
                    var _update = update(value);

                    var targetCollection = this.database.GetCollection<T>(collectionName);

                    session.StartTransaction();

                    var result = targetCollection.UpdateOne(session, _filter, _update, options);

                    session.CommitTransaction();
                }
                catch (MongoCommandException duplicateKeyException)
                {
                    LogDuplicateKeyException(duplicateKeyException, duplicateKeyException.Code, duplicateKeyException.CodeName, duplicateKeyException.ErrorMessage, duplicateKeyException.Message, duplicateKeyException.Data.ToString(), value);

                    if (session.IsInTransaction)
                    {
                        session.AbortTransaction();
                    }
                }
                catch (Exception exc)
                {
                    LogUnHandledRowException(value, exc);

                    if (session.IsInTransaction)
                    {
                        session.AbortTransaction();
                    }
                }
            }

            base.OnNext(value);
        }

        /// <summary>
        /// Notifies the observers of the end of the sequence.
        /// </summary>
        public override void OnCompleted()
        {
            _activator.Release();
            base.OnCompleted();
        }

        [LoggerMessage(
            EventId = 1001, 
            Level = LogLevel.Error, 
            Message = "Unhandled exception while processing row {value}")]
        partial void LogUnHandledRowException(Row value, Exception exc);

        [LoggerMessage(
            EventId = 1002, 
            Level = LogLevel.Error, 
            Message = "Duplicate Key Exception: Code:{code}, CodeName:{codeName}, ErrorMessage:{errorMessage}, Message:{message}, Data:{data}, {value}")]
        partial void LogDuplicateKeyException(MongoCommandException ex,  int code, string codeName, string errorMessage, string message, string data, Row value);
    }
}
