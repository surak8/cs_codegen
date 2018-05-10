using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace NSMisc {
    /// <summary>logging class.</summary>
    public static class Logger {

        #region fields
        /// <summary>controls logging-style.</summary>
        public static bool logDebug = false;
        public static bool isUnique = false;

        /// <summary>messages written.</summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        static readonly List<string> msgs = new List<string>();
        #endregion fields

        #region methods
        #region logging-methods
        /// <summary>log a message</summary>
        /// <param name="msg"/>
        /// <seealso cref="Debug"/>
        /// <seealso cref="Trace"/>
        /// <seealso cref="logDebug"/>
        /// <seealso cref="msgs"/>
        public static void logMsg(string msg) {
            if (isUnique) {
                if (msgs.Contains(msg))
                    return;
                msgs.Add(msg);
            }
            if (logDebug)
#if DEBUG
                Debug.Print("[DEBUG] " + msg);
#endif

#if TRACE
            Trace.WriteLine("[TRACE] " + msg);
#endif
        }

        /// <summary>log a message</summary>
        /// <param name="mb"/>
        /// <seealso cref="makeSig"/>
        /// <seealso cref="log(MethodBase,string)"/>
        public static void log(MethodBase mb) {
            log(mb, string.Empty);
        }

        /// <summary>log a message</summary>
        /// <param name="mb"/>
        /// <param name="msg"/>
        /// <seealso cref="makeSig"/>
        /// <seealso cref="logMsg(string)"/>
        public static void log(MethodBase mb, string msg) {
            logMsg(makeSig(mb) + ":" + msg);
        }

        public static void log(MethodBase mb, Exception ex) {
            logMsg(makeSig(mb) + ":" + ex.Message);
        }
        #endregion logging-methods

        #region misc. methods
        /// <summary>create a method-signature.</summary>
        /// <returns></returns>
        public static string makeSig(MethodBase mb) {
            return mb.ReflectedType.Name + ((
                string.Compare(mb.Name, ConstructorInfo.ConstructorName, true) == 0 ||
                string.Compare(mb.Name, ConstructorInfo.TypeConstructorName, true) == 0
                ) ? string.Empty : ".") + mb.Name;
        }

        public static string extractMessage(Exception ex) {
            Exception ex0 = ex;
            StringBuilder sb = new StringBuilder();

            while (ex0 != null) {
                sb.Append("[" + ex0.GetType().Name + "] " + ex0.Message + Environment.NewLine);
                ex0 = ex0.InnerException;
            }
            return sb.ToString();
        }
        #endregion misc. methods
        #endregion methods
    }
}