using System.ComponentModel;
using XtractLib.CouchDB;

namespace XtractLib.Linq
{
    public partial class Word : INotifyPropertyChanging, INotifyPropertyChanged
    {
        public bool IsEntity()
        {
            return (text.StartsWith("@") || text.StartsWith("#") || text.StartsWith("http"));
        }
    }
}
