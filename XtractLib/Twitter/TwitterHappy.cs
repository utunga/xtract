using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XtractLib.Twitter
{
    public static class TwitterHappy
    {
        public const int MAX_FAILED_ATTEMPTS = 400;
        public const int BASE_WAIT = 300;
        public const int MIN_WAIT = 10; //100th of a second
        public const int MAX_WAIT = 900000; //15 minutes

        static readonly object[] _syncLock;
        static int _failedAttempts;
        
        static TwitterHappy()
        {
            _syncLock = new object[0];
            _failedAttempts = 0;
        }

        // number of milliseconds we currently recommend you wait 
        public static int RecommendedWait()
        {
            lock (_syncLock)
            {
                if (_failedAttempts == 0) return MIN_WAIT;
                if (_failedAttempts > 13) return MAX_WAIT; //15 minutes
                return (int)Math.Pow(2, _failedAttempts) * 100 + BASE_WAIT;
            }
        }

        public static void SeemsHappy()
        {
            lock (_syncLock)
            {
                _failedAttempts = 0;
            }
        }

        public static void SeemsGrumpy()
        {
            lock (_syncLock)
            {
                _failedAttempts++;
                if (_failedAttempts > MAX_FAILED_ATTEMPTS)
                {
                    throw new ApplicationException("Exceeded MAX_FAILED_ATTEMPTS (" + MAX_FAILED_ATTEMPTS + ") to reach service. Giving up completely" );
                }
            }
        }

        public static void PrintStatus()
        {
            Console.Out.WriteLine("'failedAttempts'=" + _failedAttempts + " recommendedWait:" + RecommendedWait());
        }


    }
}
