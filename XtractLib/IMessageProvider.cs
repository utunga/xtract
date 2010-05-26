using System.Collections.Generic;

namespace XtractLib
{
    public interface IMessageProvider<T>
    {
        // Methods
        IEnumerable<T> GetMessages();
    }
}