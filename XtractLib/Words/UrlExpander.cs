using System.Collections.Generic;

namespace XtractLib.Words
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