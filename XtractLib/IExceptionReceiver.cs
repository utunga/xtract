using System;

namespace XtractLib
{
    public interface IExceptionReceiver
    {
        // Methods
        void NotifyException(Exception ex);

        // Properties
        Exception LastException { get; }
    }
}