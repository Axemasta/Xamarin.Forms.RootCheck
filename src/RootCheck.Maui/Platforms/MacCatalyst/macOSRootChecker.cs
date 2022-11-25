using RootCheck.Core;

namespace RootCheck.Maui
{
    internal sealed class macOSRootChecker : IRootChecker
    {
        public bool IsDeviceRooted()
        {
            // Check for SIP disabled?

            return false;
        }
    }
}
