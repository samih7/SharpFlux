namespace SharpFlux
{
    //Payload: The actual information or message in transmitted data
    public class Payload<T>
    {
        public T ActionType { get; }
        public object Data { get; }

        public Payload(T actionType, object data)
        {
            ActionType = actionType;
            Data = data;
        }
    }
}
