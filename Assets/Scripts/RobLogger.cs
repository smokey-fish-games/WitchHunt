using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

//  
// Copyright (c) Robert Parker 2021. All rights reserved.  
//  
namespace SFG
{
    public class RobLogger
    {
        /* Example code to setup and have this logger in any file */
        // private RobLogger rl;
        // RobLogger RL
        // {
        //     get
        //     {
        //         if (rl != null)
        //         {
        //             return rl;
        //         }
        //         return rl = RobLogger.GetRobLogger();
        //     }
        // }

        enum LogType
        {
            LOG,
            WARNING,
            ERROR
        }

        [Flags]
        public enum LogLevel
        {
            ALWAYS = 0b_0000_0000,  // 0,
            STANDARD = 0b_0000_0001,  // 1
            VERBOSE = 0b_0000_0010,  // 2
            TRACE = 0b_0000_0100   // 4
        }

        public static readonly object _lockObj = new object();
        static int depth = 1;
        string LogFileOutput;
        static RobLogger instance;

        const int MessageColumnStart = 60;

        LogLevel levelsToLog = LogLevel.ALWAYS | LogLevel.STANDARD;
        // LogLevel levelsToLog = LogLevel.ALWAYS | LogLevel.STANDARD | LogLevel.VERBOSE;
        //LogLevel levelsToLog = LogLevel.ALWAYS | LogLevel.STANDARD | LogLevel.VERBOSE | LogLevel.TRACE;
        //LogLevel levelsToLog = LogLevel.TRACE;

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

        public void writeTraceEntry(params object[] list)
        {
            if ((LogLevel.TRACE & levelsToLog) != LogLevel.TRACE)
            {
                return;
            }

            StackTrace stackTrace = new StackTrace();
            string getCallingClass = stackTrace.GetFrame(1).GetType().Name;
            string getCallingMethod = stackTrace.GetFrame(1).GetMethod().Name;

            string message = string.Empty;
            for (int i = 0; i < depth; i++)
            {
                message += ">";
            }

            message += " TRACEENTRY(";
            if (list != null && list.Length == 0)
            {
                for (int i = 0; i < list.Length; i++)
                {
                    message += list[i];
                    if ((i + 1) < list.Length)
                    {
                        message += ",";
                    }
                }
            }
            message += ")";

            if (Application.isEditor)
            {
                doWriteConsole(LogType.LOG, getCallingClass, getCallingMethod, message);
            }
            else
            {
                doWriteToFile(LogType.LOG, getCallingClass, getCallingMethod, message);
            }
            depth++;
        }

        public void writeTraceExit(object returned)
        {
            if ((LogLevel.TRACE & levelsToLog) != LogLevel.TRACE)
            {
                return;
            }

            StackTrace stackTrace = new StackTrace();
            string getCallingClass = stackTrace.GetFrame(1).GetType().Name;
            string getCallingMethod = stackTrace.GetFrame(1).GetMethod().Name;

            string message = string.Empty;
            for (int i = 1; i < depth; i++)
            {
                message += "<";
            }

            message += " TRACEEXIT(";
            if (returned != null)
            {
                message += returned;
            }
            message += ")";

            if (Application.isEditor)
            {
                doWriteConsole(LogType.LOG, getCallingClass, getCallingMethod, message);
            }
            else
            {
                doWriteToFile(LogType.LOG, getCallingClass, getCallingMethod, message);
            }

            depth--;
            if (depth < 1)
            {
                UnityEngine.Debug.LogError("Depth dropped below 1 so you have a mismatch of entry-exit points!");
                depth = 1;
            }
        }

        public void writeError(string message)
        {
            StackTrace stackTrace = new StackTrace();
            string getCallingClass = stackTrace.GetFrame(1).GetType().Name;
            string getCallingMethod = stackTrace.GetFrame(1).GetMethod().Name;

            if (Application.isEditor)
            {
                doWriteConsole(LogType.ERROR, getCallingClass, getCallingMethod, message);
            }
            else
            {
                doWriteToFile(LogType.ERROR, getCallingClass, getCallingMethod, message);
            }
        }

        public void writeWarning(string message)
        {
            StackTrace stackTrace = new StackTrace();
            string getCallingClass = stackTrace.GetFrame(1).GetType().Name;
            string getCallingMethod = stackTrace.GetFrame(1).GetMethod().Name;

            if (Application.isEditor)
            {
                doWriteConsole(LogType.WARNING, getCallingClass, getCallingMethod, message);
            }
            else
            {
                doWriteToFile(LogType.WARNING, getCallingClass, getCallingMethod, message);
            }
        }

        public void writeInfo(LogLevel level, string message)
        {
            if ((level & levelsToLog) != level)
            {
                return;
            }

            StackTrace stackTrace = new StackTrace();
            string getCallingClass = stackTrace.GetFrame(1).GetMethod().DeclaringType.Name;
            string getCallingMethod = stackTrace.GetFrame(1).GetMethod().Name;

            if (Application.isEditor)
            {
                doWriteConsole(LogType.LOG, getCallingClass, getCallingMethod, message);
            }
            else
            {
                doWriteToFile(LogType.LOG, getCallingClass, getCallingMethod, message);
            }
        }

        private void doWriteConsole(LogType type, string cla, string meth, string message)
        {
            string output = string.Empty;
            output += "[" + cla + ":" + meth + "] ";

            if (output.Length > MessageColumnStart)
            {
                UnityEngine.Debug.LogWarning("Rob Column start is too short! " + output.Length + " v " + MessageColumnStart);
            }
            while (output.Length < MessageColumnStart)
            {
                output += " ";
            }
            output += ": ";
            output += message;

            switch (type)
            {
                case LogType.LOG:
                    UnityEngine.Debug.Log(output);
                    break;
                case LogType.WARNING:
                    UnityEngine.Debug.LogWarning(output);
                    break;
                case LogType.ERROR:
                    UnityEngine.Debug.LogError(output);
                    break;
                default:
                    UnityEngine.Debug.Log(output);
                    break;
            }
        }

        private void doWriteToFile(LogType type, string cla, string meth, string message)
        {
            if (!UnityEngine.Debug.isDebugBuild)
            {
                return;
            }
            string prefixes = string.Empty;
            prefixes += "[" + cla + ":" + meth + "] ";
            switch (type)
            {
                case LogType.LOG:
                    prefixes += "LOG  ";
                    break;
                case LogType.WARNING:
                    prefixes += "WARN ";
                    break;
                case LogType.ERROR:
                    prefixes += "ERROR";
                    break;
                default:
                    prefixes += "UNK  ";
                    break;
            }

            while (prefixes.Length < MessageColumnStart)
            {
                prefixes += " ";
            }
            prefixes += ": ";
            prefixes += message;

            // write to the file
            lock (_lockObj)
            {
                using (StreamWriter w = File.AppendText(LogFileOutput))
                {
                    prefixes = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff ") + prefixes;
                    w.WriteLine(prefixes);
                }
            }
        }
    }
}
