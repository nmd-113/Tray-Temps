using System.Runtime.InteropServices;

namespace TrayTemps
{
    internal static class SystemMemoryUsageHelper
    {
        private const double BytesPerGigabyte = 1024d * 1024d * 1024d;

        [StructLayout(LayoutKind.Sequential)]
        private struct MemoryStatusEx
        {
            internal uint Length;
            internal uint MemoryLoad;
            internal ulong TotalPhysical;
            internal ulong AvailablePhysical;
            internal ulong TotalPageFile;
            internal ulong AvailablePageFile;
            internal ulong TotalVirtual;
            internal ulong AvailableVirtual;
            internal ulong AvailableExtendedVirtual;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GlobalMemoryStatusEx(ref MemoryStatusEx buffer);

        internal static bool TryGetPhysicalMemoryUsage(out double usedGigabytes, out double totalGigabytes)
        {
            try
            {
                var status = new MemoryStatusEx
                {
                    Length = (uint)Marshal.SizeOf(typeof(MemoryStatusEx))
                };

                if (GlobalMemoryStatusEx(ref status) &&
                    status.TotalPhysical > 0 &&
                    status.AvailablePhysical <= status.TotalPhysical)
                {
                    totalGigabytes = status.TotalPhysical / BytesPerGigabyte;
                    usedGigabytes = (status.TotalPhysical - status.AvailablePhysical) / BytesPerGigabyte;
                    return true;
                }
            }
            catch
            {
                // The app is Windows-only, but a native query failure must not break OSD updates.
            }

            usedGigabytes = 0;
            totalGigabytes = 0;
            return false;
        }
    }
}
