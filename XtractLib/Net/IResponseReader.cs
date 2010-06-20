using System;

namespace XtractLib.Net
{
    public interface IResponseReader : IDisposable
    {
        string ReadLine();
        string ReadToEnd();
    }
}