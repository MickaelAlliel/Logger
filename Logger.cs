///******************************************************************************************************************************************************************///
/// Created by Mickael ALLIEL - 10 Nov 2016
/// 
/// The MIT License (MIT)                                                                                                                                            ///
/// Copyright(c) <2016> <Mickael ALLIEL>                                                                                                                             ///
/// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),               ///
/// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,               ///
/// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:                       ///
/// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.                                   ///
/// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,              ///
/// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,    ///
/// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.       ///
///******************************************************************************************************************************************************************///

using System;
using System.Globalization;
using System.IO;

namespace logger
{
    /// <summary>
    /// Simple logging class - just add and use!
    /// </summary>
    static class Logger
    {

        public enum Severity
        {
            Info,
            Warning,
            Critical,
            Error,
            Exception
        };

        private static FileStream logStream = null;
        
        private static string filePath = null;
        private static string logPrefix = null;
        private static string currentDate = null;

        //Used to check if log needs to be replaced 
        private static string currentLogDay = null;
        private static string currentDay = null;

        /// <summary>
        /// Initialize logger with name prefix, defines the file path and create a new .log
        /// If it is used after logging something already, it will create a new .log file and continue there
        /// </summary>
        /// <param name="prefix">Log prefix - Default: Log + _yyyy-MM-dd.log </param>
        public static void SetPrefix(string prefix = "Log")
        {
            logPrefix = prefix;

            currentDate = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            filePath = AppDomain.CurrentDomain.BaseDirectory + logPrefix + "_" + currentDate + ".log";

            CreateFile();
        }

        /// <summary>
        /// Checks if the log currently used is of the correct day, if not, create a new log for today
        /// </summary>
        private static void CheckDay()
        {
            if (filePath == null)
                SetPrefix();

            currentLogDay = filePath.Substring((filePath.Length - 14), 10);
            currentDay = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

            // If used log is not from today, Create new file for current day
            if (!currentLogDay.Equals(currentDay))
            {
                Console.WriteLine("FILE CHECK: Log file not current - creating new log file");

                //Update file path
                filePath = AppDomain.CurrentDomain.BaseDirectory + logPrefix + "_" + currentDay + ".log";

                //Create new log file
                try
                {
                    CreateFile();
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occured : '{0}'", e);
                }
            }
            else
            {
                Console.WriteLine("FILE CHECK: Log file current - no need to create new file ({0})", currentDate);
            }
        }

        /// <summary>
        /// Checks if prefix was entered manually, if not, use default prefix - set file path and create a new .log file
        /// 
        /// </summary>
        private static void CreateFile()
        {
            if (filePath == null)
                SetPrefix();


            if (!File.Exists(filePath))
            {
                try
                {
                    logStream = File.Create(filePath); // Create new file

                    logStream.Flush(); // Finish writing
                    logStream.Close(); // Free process
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occured : '{0}'", e);

                }
            }
        }

        /// <summary>
        /// Write a line in the currently used .log taking in
        /// text from user and severity of alert
        /// </summary>
        /// <param name="text">Manually entered text for logging</param>
        /// <param name="sev">Severity type of alert</param>
        private static void WriteLog(string text, Severity sev)
        {
            CheckDay(); // Before each call, check if it should be written in current log or in a new file

            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                try {
                    writer.Write(ParseDateTime());
                    writer.Write(ParseSeverity(sev));
                    writer.WriteLine(text);
                }
                catch (Exception e){
                    Console.WriteLine("An error occured : '{0}'", e);
                }
            }
        }

        /// <summary>
        /// Create [INFO] alert in log
        /// </summary>
        /// <param name="text">User entered text for alert</param>
        public static void Info(string text)
        {
            WriteLog(text, Severity.Info);
        }

        /// <summary>
        /// Create [WARNING] alert in log
        /// </summary>
        /// <param name="text">User entered text for alert</param>
        public static void Warning(string text)
        {
            WriteLog(text, Severity.Warning);
        }

        /// <summary>
        /// Create [CRITICAL] alert in log
        /// </summary>
        /// <param name="text">User entered text for alert</param>
        public static void Critical(string text)
        {
            WriteLog(text, Severity.Critical);
        }

        /// <summary>
        /// Create [ERROR] alert in log
        /// </summary>
        /// <param name="text">User entered text for alert</param>
        public static void Error(string text)
        {
            WriteLog(text, Severity.Error);
        }

        /// <summary>
        /// Create [EXCEPTION] alert in log
        /// </summary>
        /// <param name="text">User entered text for alert</param>
        public static void Exception(string text)
        {
            WriteLog(text, Severity.Exception);
        }

        /// <summary>
        /// Returns formatted string defined by type of alert
        /// </summary>
        /// <param name="sev">Severity type of alert</param>
        /// <returns>Returns formatted string defined by type of alert</returns>
        private static string ParseSeverity(Severity sev)
        {
            switch (sev)
            {
                case Severity.Info:
                    return "[INFO] |\t";
                case Severity.Warning:
                    return "[WARNING] |\t";
                case Severity.Critical:
                    return "[CRITICAL] |\t";
                case Severity.Error:
                    return "[ERROR] |\t";
                case Severity.Exception:
                    return "[EXCEPTION] |\t";

                default:
                    return "UNDEFINED |\t";
            }
        }

        /// <summary>
        /// Returns formatted string of current DateTime
        /// </summary>
        /// <returns>Returns formatted string of current DateTime</returns>
        private static string ParseDateTime()
        {
            DateTime currDateTime = DateTime.Now;
            string dateTime = "";

            //Parse DateTime to following format : YYYY-MM-DD -- HH:MM:SS:sss  |   [Text]

            dateTime += currDateTime.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture); // Date
            dateTime += " -- "; // Separator
            dateTime += currDateTime.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture); // Time

            dateTime += "\t| "; // Tab | Tab Separator

            return dateTime;
        }
    }
}