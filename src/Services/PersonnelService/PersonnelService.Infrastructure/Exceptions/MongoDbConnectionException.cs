namespace PersonnelService.Infrastructure.Exceptions
{
    public class MongoDbConnectionException : Exception
    {
        public MongoDbConnectionException(string message, Exception? inner = null)
            : base(message, inner) { }
    }
}