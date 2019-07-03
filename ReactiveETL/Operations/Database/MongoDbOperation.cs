using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Driver;
using ReactiveETL.Activators;
using ReactiveETL.Logging;

namespace ReactiveETL.Operations.Database
{
    /// <summary>
    /// opearation of mongodb
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MongoDbOperation<T> : AbstractOperation
    {
        private readonly ILog log = LogProvider.GetCurrentClassLogger();

        private CommandActivator _activator;

        private IMongoDatabase database;
        private string collectionName;
        private Func<Row, MongoDB.Driver.FilterDefinition<T>> filter;
        private Func<Row, MongoDB.Driver.UpdateDefinition<T>> update;
        private MongoDB.Driver.UpdateOptions options;

        public MongoDbOperation(CommandActivator activator,
            IMongoDatabase database,
            string collectionName,
            Func<Row, MongoDB.Driver.FilterDefinition<T>> filter,
            Func<Row, MongoDB.Driver.UpdateDefinition<T>> update,
            MongoDB.Driver.UpdateOptions options = null)
        {
            _activator = activator;
            this.database = database;
            this.collectionName = collectionName;
            this.filter = filter;
            this.update = update;
            this.options = options;
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
                    log.Error($"Code:{duplicateKeyException.Code}, CodeName:{duplicateKeyException.CodeName}", duplicateKeyException);
                    session.AbortTransaction();
                }
                catch (System.Exception exc)
                {
                    log.Error(exc, exc.Message);
                    session.AbortTransaction();
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
    }
}
