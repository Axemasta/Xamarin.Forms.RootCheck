using RootCheck.Core;
using RootCheck.Core.Exceptions;
using System;
using System.ComponentModel;
using System.Threading;

namespace RootCheck.Maui
{
    /// <summary>
    /// Root Checker
    /// </summary>
    public static class RootChecker
    {
        private static IRootChecker customInstance;

        private static readonly Lazy<IRootChecker> rootChecker = new Lazy<IRootChecker>(CreateChecker, LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Returns whether the device is rooted
        /// </summary>
        public static IRootChecker Current => customInstance ?? rootChecker.Value;

        public static bool IsDeviceRooted => Current.IsDeviceRooted();

        private static IRootChecker CreateChecker()
        {
#if ANDROID
            return new AndroidRootChecker();
#endif

#if IOS
            return new iOSRootChecker();
#endif

#if MACCATALYST
            return new macOSRootChecker();
#endif

#if WINDOWS
            return new WindowsRootChecker();
#endif

            throw new NotImplementedInReferenceAssemblyException();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void SetInstance(IRootChecker instance)
        {
            // Allow easier mocking from unit tests.
            customInstance = instance;
        }
    }
}
