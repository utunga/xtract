namespace XtractLib
{
    public interface IMessageReceiver<T>
    {
        // Methods
        void Notify(T message);
    }
}


