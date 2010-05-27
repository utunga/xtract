using System.Collections.Generic;

namespace XtractLib.Common
{
    public interface IUrlExpander
    {
        IEnumerable<string> ProcessUrls(IEnumerable<string> enumerable);
    }
}