using System.Collections.Generic;

namespace XtractLib.Words
{
    public interface IUrlExpander
    {
        IEnumerable<string> ProcessUrls(IEnumerable<string> enumerable);
    }
}