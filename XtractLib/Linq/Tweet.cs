using System.Collections.Generic;
using System.ComponentModel;
using XtractLib.CouchDB;

namespace XtractLib.Linq
{
    public partial class Tweet : INotifyPropertyChanging, INotifyPropertyChanged, ICouchDocHappy
    {

        public string _id
        {
            get { return "tweet_" + this.twitter_id; }
        }

        public string doc_type
        {
            get { return "csharp_munged_tweet"; }
        }

        private List<string> _entities;
        public List<string> entities
        {
            get { return _entities; }
        }
        public void AddEntity(string entityText)
        {
            if (_entities==null)
                _entities  = new List<string>();
            _entities.Add(entityText);
        }
    }
}
