using System;
using System.Configuration;
using System.Net;
using System.Threading;
using NLog;
using Offr.CouchDB;
using XtractLib.Twitter;

namespace XtractLib.CouchDB
{
    public class PushToCouchDBReceiver 
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        public string CouchServer { get; set; }
        public string CouchDB { get; set; }

        public PushToCouchDBReceiver()
        {
            //set up push end points
            CouchServer = ConfigurationManager.AppSettings["CouchServer"] ?? "http://chchneeds.info"; //actually  won't work because it needs user/pass so you have to specify in localAppSettings.config/app.config
            CouchDB = ConfigurationManager.AppSettings["CouchDB"] ?? "hashmapd"; 
        }

        public void Push(ICouchDocHappy docToPush)
        {
            if (docToPush.doc_type == null)
            {
                throw new ApplicationException("You really 'must' provide a doc_type before you go shoving docs into a couchdb instance");
            }

            string messageAsJSON = JSON.Serialize(docToPush);
            int networkAttempts = 5;
            while (--networkAttempts>0) 
            {
                try
                {
                    new DB().CreateDocument(CouchServer, CouchDB, messageAsJSON);
                    _log.Info("PUSH |" + CouchDB + "| '" + messageAsJSON + "' ->" + CouchServer); 
                    break;
                }
                catch (WebException ex)
                {
                    
                    _log.ErrorException("Failure to push message to " + CouchServer + ":" + CouchDB + " will try again " + networkAttempts + " times.", ex);
                    Thread.Sleep(500); //wait for half a second
                }
            }
            //failed, but hopefully didn't bring the whole service down
        }
    }
}

