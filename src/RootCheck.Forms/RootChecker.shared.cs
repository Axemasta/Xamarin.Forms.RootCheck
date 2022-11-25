using System;
using System.ComponentModel;
using System.Threading;
using RootCheck.Core;

namespace Xamarin.Forms.RootCheck
{
    /// <summary>
    /// Root Check
    /// </summary>
    public static class RootCheck
    {
        private static IChecker customInstance;

        private static Lazy<IChecker> implementation = new Lazy<IChecker>(CreateChecker, LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Gets if the plugin is supported on the current platform.
        /// </summary>
        public static bool IsSupported => implementation.Value != null;

        /// <summary>
        /// Returns whether the device is rooted
        /// </summary>
        public static bool IsDeviceRooted
        {
            get
            {
                var checker = customInstance ?? implementation.Value;

                if (checker is null)
                {
                    throw NotImplementedInReferenceAssembly();
                }

                return checker.IsDeviceRooted();
            }
        }

        static IChecker CreateChecker()
        {
        #if NETSTANDARD1_0 || NETSTANDARD2_0
                    return null;
        #else
        #pragma warning disable IDE0022 // Use expression body for methods
                return new Xamarin.Forms.RootCheck.RootChecker();
        #pragma warning restore IDE0022 // Use expression body for methods
        #endif
        }

        internal static Exception NotImplementedInReferenceAssembly() =>
            new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void SetInstance(IChecker instance)
        {
            // Allow easier mocking from unit tests.
            customInstance = instance;
        }
    }
}
