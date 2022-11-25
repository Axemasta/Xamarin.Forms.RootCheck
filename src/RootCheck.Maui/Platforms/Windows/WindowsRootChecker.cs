using RootCheck.Core;

namespace RootCheck.Maui
{
    internal sealed class WindowsRootChecker : IRootChecker
    {
        public bool IsDeviceRooted()
        {
            // Probably not worth the hastle!
            return false;
        }
    }
}
