using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using XtractLib.Twitter;

internal class Program
{
    private const int NUM_STATUSES_TO_PULL = 100000;
    private const int WRITE_OUT_USERS_EVERY = 1000;

    // Methods
    private static void Main(string[] args)
    {
        FileStream statusFile = new FileStream("status_dump.txt", FileMode.OpenOrCreate, FileAccess.Write);
        StreamWriter statusWriter = new StreamWriter(statusFile);
        int count = 0;
        try
        {
            TwitterStatusProvider provider = new TwitterStatusProvider();
            string twitter_api_username = ConfigurationManager.AppSettings["twitter_user"];
            string twitter_api_password = ConfigurationManager.AppSettings["twitter_pass"];
            provider.UseCGICredentials(twitter_api_username, twitter_api_password);
            provider.YieldThisMany = NUM_STATUSES_TO_PULL;

            Console.Out.WriteLine("About to start reading from twitter - up to " + NUM_STATUSES_TO_PULL + " statuses.");

            EnglishStatusProvider englishProvider = new EnglishStatusProvider(provider);
            SortedDictionary<long, int> userMsgCounts = new SortedDictionary<long, int>();
            SortedDictionary<long, string> usernameLookup = new SortedDictionary<long, string>();
            foreach (TwitterStatus status in englishProvider.GetMessages())
            {
                string username = status.user.screen_name;
                long user_id = status.user.id;
                if (!usernameLookup.ContainsKey(user_id))
                {
                    usernameLookup.Add(user_id, username);
                    userMsgCounts.Add(user_id, 1);
                }
                else
                {
                    userMsgCounts[user_id] += 1;
                }
                string text = status.text.Replace("|", " ");
                statusWriter.WriteLine(string.Concat(new object[] { user_id, "|", username, "|", text }));
                //Console.Out.WriteLine(string.Concat(new object[] { user_id, "|", username, "|", text }));
                if (count++ > WRITE_OUT_USERS_EVERY)
                {
                    count = 0;
                    FileStream userFile = new FileStream("users.txt", FileMode.Create, FileAccess.Write);
                    using (StreamWriter userWriter = new StreamWriter(userFile))
                    {
                        foreach (KeyValuePair<long, int> userMsgCount in userMsgCounts.OrderBy(key => key.Value))
                        {
                            user_id = userMsgCount.Key;
                            username = usernameLookup[user_id];
                            int msgCount = userMsgCount.Value;
                            userWriter.WriteLine(string.Concat(new object[] { msgCount, " message|", user_id, "|", username }));
                        }
                        userWriter.Flush();
                        userWriter.Close();
                        Console.Out.WriteLine("Done writing out " + userMsgCounts.Count + " matching users");
                    }
                }
                statusWriter.Flush();
            }
        }
        finally
        {
            statusWriter.Flush();
            statusWriter.Close();
        }
    }
}
