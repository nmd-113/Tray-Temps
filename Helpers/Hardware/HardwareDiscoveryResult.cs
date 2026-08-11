using System.Collections.Generic;

namespace TrayTemps
{
    internal sealed class HardwareDiscoveryResult
    {
        internal HardwareDiscoveryResult(string summary, string details, List<string> displayNames, int count)
        {
            Summary = summary;
            Details = details;
            DisplayNames = displayNames ?? new List<string>();
            Count = count;
        }

        internal string Summary { get; }
        internal string Details { get; }
        internal List<string> DisplayNames { get; }
        internal int Count { get; }
    }
}
