using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Leatha.WarOfTheElements.Server.Utilities
{
    public static class MongoExtensions
    {
        public static IMongoCollection<TEntity> GetMongoCollection<TEntity>(this IMongoDatabase database)
            where TEntity : class
        {
            return database.GetCollection<TEntity>(typeof(TEntity).Name);
        }


        //public static object DeserializeData(this IDataEntity action)
        //{
        //    if (action.DataType == null)
        //        throw new InvalidOperationException("Null value provided for DataType.");

        //    var type = Type.GetType(action.DataType);
        //    if (type == null)
        //        throw new InvalidOperationException($"Cannot resolve type {action.DataType}.");

        //    return BsonSerializer.Deserialize(action.Data, type);
        //}

        //public static T DeserializeData<T>(this IDataEntity action)
        //    => BsonSerializer.Deserialize<T>(action.Data);
    }
}
