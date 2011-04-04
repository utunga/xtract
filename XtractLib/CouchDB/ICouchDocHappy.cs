namespace XtractLib.CouchDB
{
    public interface ICouchDocHappy
    {
// ReSharper disable InconsistentNaming
        string _id { get; }
        string doc_type { get; }
// ReSharper restore InconsistentNaming
    }
}