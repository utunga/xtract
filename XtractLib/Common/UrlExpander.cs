using System.Collections.Generic;

namespace XtractLib.Common
{
    public class UrlExpander : IUrlExpander
    {   
        //FIXME: actually implement this
        public IEnumerable<string> ProcessUrls(IEnumerable<string> urls)
        {
            foreach (string s in urls)
            {
                yield return s;
            }
        }
    }
}