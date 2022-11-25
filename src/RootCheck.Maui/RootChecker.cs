using RootCheck.Core;
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
        private static IChecker customInstance;

        private static readonly Lazy<IChecker> platformImplementation = new Lazy<IChecker>(CreateChecker, LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Returns whether the device is rooted
        /// </summary>
        public static bool IsDeviceRooted
        {
            get
            {
                var checker = customInstance ?? platformImplementation.Value;

                if (checker is null)
                    return false;

                return checker.IsDeviceRooted();
            }
        }

        private static IChecker CreateChecker()
        {
            return new PlatformChecker();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void SetInstance(IChecker instance)
        {
            // Allow easier mocking from unit tests.
            customInstance = instance;
        }
    }
}
