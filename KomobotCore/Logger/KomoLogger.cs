using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Easy.Logger.Interfaces;

namespace KomobotV2.Logger
{
    public class KomoLogger : IEasyLogger
    {
        #region properties
        public string Name { get; set; }

        public bool IsTraceEnabled {
            get { return false; }
        }

        public bool IsDebugEnabled { get; set; }

        public bool IsInfoEnabled { get; set; }

        public bool IsWarnEnabled { get; set; }

        public bool IsErrorEnabled { get; set; }

        public bool IsFatalEnabled { get; set; }
        #endregion

        #region constructors
        public KomoLogger()
        {
            Name = "KomoLogger";
            EnableAllProperties();
        }

        public KomoLogger(string name)
        {
            Name = name;
            EnableAllProperties();
        }

        public KomoLogger(bool isWarnEnabled, bool isErrorEnabled, bool isDebugEnabled, bool isFatalEnabled, bool isInfoEnabled)
        {
            Name = "KomoLogger";
            IsWarnEnabled = isWarnEnabled;
            IsErrorEnabled = isErrorEnabled;
            IsDebugEnabled = isDebugEnabled;
            IsFatalEnabled = isFatalEnabled;
            IsInfoEnabled = isInfoEnabled;
        }

        public KomoLogger(string name, bool isWarnEnabled, bool isErrorEnabled, bool isDebugEnabled, bool isFatalEnabled, bool isInfoEnabled)
        {
            Name = name;
            IsWarnEnabled = isWarnEnabled;
            IsErrorEnabled = isErrorEnabled;
            IsDebugEnabled = isDebugEnabled;
            IsFatalEnabled = isFatalEnabled;
            IsInfoEnabled = isInfoEnabled;
        }
        #endregion

        #region public methods
        public void Debug(object message)
        {
            string msg = DateTime.UtcNow.ToString() + " [Debug]: " + message + "\n";

            ApplyDebugForeGroundColor();
            Console.Write(msg);
            File.AppendAllText(logPath, msg);
            ResetForeGroundColor();
        }

        public void Debug(object message, Exception exception)
        {
            string msg = DateTime.UtcNow.ToString() + " [Debug]: Exception occured: " + exception.ToString() + ".\nAt: " + message + "\n";

            ApplyDebugForeGroundColor();
            Console.Write(msg);
            File.AppendAllText(logPath, msg);
            ResetForeGroundColor();
        }

        public void DebugFormat(string format, object arg)
        {
            throw new NotImplementedException();
        }

        public void DebugFormat(string format, object arg1, object arg2)
        {
            throw new NotImplementedException();
        }

        public void DebugFormat(string format, object arg1, object arg2, object arg3)
        {
            throw new NotImplementedException();
        }

        public void DebugFormat(string format, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void Error(object message)
        {
            string msg = DateTime.UtcNow.ToString() + " [Error]: " + message + "\n";

            ApplyErrorForeGroundColor();
            Console.Write(msg);
            File.AppendAllText(logPath, msg);
            ResetForeGroundColor();
        }

        public void Error(object message, Exception exception)
        {
            string msg = DateTime.UtcNow.ToString() + " [Error]: Exception occured: " + exception.ToString() + ".\nAt: " + message + "\n";

            ApplyErrorForeGroundColor(); 
            Console.Write(msg);
            File.AppendAllText(logPath, msg);
            ResetForeGroundColor();
        }

        public void ErrorFormat(string format, object arg)
        {
            throw new NotImplementedException();
        }

        public void ErrorFormat(string format, object arg1, object arg2)
        {
            throw new NotImplementedException();
        }

        public void ErrorFormat(string format, object arg1, object arg2, object arg3)
        {
            throw new NotImplementedException();
        }

        public void ErrorFormat(string format, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void Fatal(object message)
        {
            string msg = DateTime.UtcNow.ToString() + " [Fatal]: " + message+"\n";
            
            ApplyFatalForeGroundColor();
            Console.Write(msg);
            File.AppendAllText(logPath, msg);
            ResetForeGroundColor();
        }

        public void Fatal(object message, Exception exception)
        {
            string msg = DateTime.UtcNow.ToString() + " [Fatal]: Exception occured: " + exception.ToString() + ".\nAt: " + message + "\n";

            ApplyFatalForeGroundColor();
            Console.Write(msg);
            File.AppendAllText(logPath, msg);
            ResetForeGroundColor();
        }

        public void FatalFormat(string format, object arg)
        {
            throw new NotImplementedException();
        }

        public void FatalFormat(string format, object arg1, object arg2)
        {
            throw new NotImplementedException();
        }

        public void FatalFormat(string format, object arg1, object arg2, object arg3)
        {
            throw new NotImplementedException();
        }

        public void FatalFormat(string format, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
            throw new NotImplementedException();
        }

        public IDisposable GetScopedLogger(string name)
        {
            throw new NotImplementedException();
        }

        public void Info(object message)
        {
            string msg = DateTime.UtcNow.ToString() + " [Info]: " + message + "\n";

            ApplyInfoForeGroundColor();
            Console.Write(msg);
            File.AppendAllText(logPath, msg);
            ResetForeGroundColor();
        }

        public void Info(object message, Exception exception)
        {
            string msg = DateTime.UtcNow.ToString() + " [Info]: Exception occured: " + exception.ToString() + ".\nAt: " + message + "\n";

            ApplyInfoForeGroundColor();
            Console.Write(msg);
            File.AppendAllText(logPath, msg);
            ResetForeGroundColor();
        }

        public void InfoFormat(string format, object arg)
        {
            throw new NotImplementedException();
        }

        public void InfoFormat(string format, object arg1, object arg2)
        {
            throw new NotImplementedException();
        }

        public void InfoFormat(string format, object arg1, object arg2, object arg3)
        {
            throw new NotImplementedException();
        }

        public void InfoFormat(string format, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void Trace(object message)
        {
            throw new NotImplementedException();
        }

        public void Trace(object message, Exception exception)
        {
            throw new NotImplementedException();
        }

        public void TraceFormat(string format, object arg)
        {
            throw new NotImplementedException();
        }

        public void TraceFormat(string format, object arg1, object arg2)
        {
            throw new NotImplementedException();
        }

        public void TraceFormat(string format, object arg1, object arg2, object arg3)
        {
            throw new NotImplementedException();
        }

        public void TraceFormat(string format, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void TraceFormat(IFormatProvider provider, string format, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void Warn(object message)
        {
            string msg = DateTime.UtcNow.ToString() + " [Warn]: " + message + "\n";

            ApplyWarnForeGroundColor();
            Console.Write(msg);
            File.AppendAllText(logPath, msg);
            ResetForeGroundColor();
        }

        public void Warn(object message, Exception exception)
        {
            string msg = DateTime.UtcNow.ToString() + " [Warn]: Exception occured: " + exception.ToString() + ".\nAt: " + message + "\n";

            ApplyWarnForeGroundColor();
            Console.Write(msg);
            File.AppendAllText(logPath, msg);
            ResetForeGroundColor();
        }

        public void WarnFormat(string format, object arg)
        {
            throw new NotImplementedException();
        }

        public void WarnFormat(string format, object arg1, object arg2)
        {
            throw new NotImplementedException();
        }

        public void WarnFormat(string format, object arg1, object arg2, object arg3)
        {
            throw new NotImplementedException();
        }

        public void WarnFormat(string format, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region private methods
        private void ResetForeGroundColor()
        {
            SetConsoleColor(ConsoleColor.White);
        }

        private void ApplyDebugForeGroundColor()
        {
            SetConsoleColor(ConsoleColor.Green);
        }

        private void ApplyWarnForeGroundColor()
        {
            SetConsoleColor(ConsoleColor.Yellow);
        }

        private void ApplyErrorForeGroundColor()
        {
            SetConsoleColor(ConsoleColor.Red);
        }

        private void ApplyInfoForeGroundColor()
        {
            SetConsoleColor(ConsoleColor.Gray);
        }

        private void ApplyFatalForeGroundColor()
        {
            SetConsoleColor(ConsoleColor.Red);
        }

        private void SetConsoleColor(ConsoleColor color)
        {
            Console.ForegroundColor = color;
        }

        private void EnableAllProperties()
        {
            IsFatalEnabled = true;
            IsErrorEnabled = true;
            IsWarnEnabled = true;
            IsInfoEnabled = true;
            IsDebugEnabled = true;
        }

        #endregion

        #region fields
        string logPath = @"C:\Users\Peter\Documents\Log\KomoLog.txt";
        #endregion
    }
}
