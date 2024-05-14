namespace MRSTWEb.BusinessLogic.Infrastructure
{
    public class OperationDetails
    {
        public bool Succeeded { get; private set; }
        public string Message { get; private set; }
        public string Property { get; private set; }
        public OperationDetails(bool succeded, string message, string prop)
        {
            Succeeded = succeded;
            Message = message;
            Property = prop;
        }
    }
}
