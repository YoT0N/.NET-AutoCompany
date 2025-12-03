namespace PersonnelService.Infrastructure.Exceptions
{
    public class MongoDbWriteException : Exception
    {
        public MongoDbWriteException(string message, Exception? inner = null)
            : base(message, inner) { }
    }
}   