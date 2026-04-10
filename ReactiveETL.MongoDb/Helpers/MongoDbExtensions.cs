namespace ReactiveETL.Helpers
{
    using System;
    using Activators;
    using MongoDB.Driver;

    public static class MongoDbExtensions
    {
       public static MongoDbUpdateOperation<T> MongoDbUpsert<T>(this IObservableOperation observed,
            IMongoDatabase database,
            string collectionName,
            Func<Row, FilterDefinition<T>> filter,
            Func<Row, UpdateDefinition<T>> update,
            UpdateOptions options = null)
        {
#pragma warning disable CA1510
            if (observed == null)
            {
                throw new ArgumentNullException(nameof(observed));
            }
#pragma warning restore CA1510
            
            var mongoDbOperation = new MongoDbUpdateOperation<T>(new CommandActivator(), database, collectionName, filter, update, LogProvider.GetLogger(typeof(MongoDbUpdateOperation<>).ToString()), options);
            observed.Subscribe(mongoDbOperation);
            return mongoDbOperation;
        }
    }
}
