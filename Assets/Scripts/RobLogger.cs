using System;
using System.IO;
using UnityEngine;

namespace SFG.WitchHunt
{
    public class RobLogger
    {
        enum LogAlert
        {
            LOG,
            WARNING,
            ERROR
        }

        public static readonly object _lockObj = new object();
        string LogFileOutput;
        static RobLogger instance;
        public static RobLogger GetRobLogger()
        {
            if (instance == null)
            {
                /* Grab a lock but we may be in a race with someone else so check again after */
                lock (_lockObj)
                {
                    if (instance == null)
                    {
                        instance = new RobLogger();
                    }
                }
            }
            return instance;
        }

        RobLogger()
        {
            LogFileOutput = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "WitchHunt-" + System.Guid.NewGuid().ToString() + ".log");
        }

        public void writeError(string message)
        {
            if (Application.isEditor)
            {
                Debug.LogError(message);
            }
            else
            {
                writeToFile(LogAlert.ERROR, message);
            }
        }

        public void writeWarning(string message)
        {
            if (Application.isEditor)
            {
                Debug.LogWarning(message);
            }
            else
            {
                writeToFile(LogAlert.WARNING, message);
            }
        }

        public void writeInfo(string message)
        {
            if (Application.isEditor)
            {
                Debug.Log(message);
            }
            else
            {
                writeToFile(LogAlert.LOG, message);
            }
        }

        private void writeToFile(LogAlert level, string message)
        {
            if (!Debug.isDebugBuild)
            {
                return;
            }
            // write to the file
            lock (_lockObj)
            {
                using (StreamWriter w = File.AppendText(LogFileOutput))
                {
                    string fullmessage = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff - ");
                    switch (level)
                    {
                        case LogAlert.LOG:
                            fullmessage += "LOG   : ";
                            break;
                        case LogAlert.WARNING:
                            fullmessage += "WARN  : ";
                            break;
                        case LogAlert.ERROR:
                            fullmessage += "ERROR : ";
                            break;
                        default:
                            fullmessage += "UNK   : ";
                            break;
                    }
                    fullmessage += message;
                    w.WriteLine(fullmessage);
                }
            }
        }
    }
}
