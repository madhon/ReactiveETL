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
            var mongoDbOperation = new MongoDbUpdateOperation<T>(new CommandActivator(), database, collectionName, filter, update, options);
            observed.Subscribe(mongoDbOperation);
            return mongoDbOperation;
        }
    }
}
