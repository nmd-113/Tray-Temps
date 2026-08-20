using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;

namespace TrayTemps
{
    internal static class WmiQueryHelper
    {
        internal static List<ManagementObject> WmiQuery(string query)
        {
            TryWmiQuery(query, out List<ManagementObject> list);
            return list;
        }

        internal static bool TryWmiQuery(string query, out List<ManagementObject> list)
        {
            list = new List<ManagementObject>();

            try
            {
                using (var searcher = new ManagementObjectSearcher(query))
                using (var results = searcher.Get())
                {
                    foreach (ManagementObject obj in results.Cast<ManagementObject>())
                        list.Add(obj);
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("WmiQuery failed: " + ex);
                return false;
            }
        }

        internal static List<ManagementObject> WmiQuery(string scopePath, string query)
        {
            var list = new List<ManagementObject>();

            try
            {
                var scope = new ManagementScope(scopePath);
                using (var searcher = new ManagementObjectSearcher(scope, new ObjectQuery(query)))
                using (var results = searcher.Get())
                {
                    foreach (ManagementObject obj in results.Cast<ManagementObject>())
                        list.Add(obj);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("WmiQuery (scoped) failed: " + ex);
            }

            return list;
        }

        internal static void DisposeAll(IEnumerable<ManagementObject> objects)
        {
            if (objects == null)
                return;

            foreach (ManagementObject obj in objects)
            {
                try { obj?.Dispose(); }
                catch (Exception ex) { Debug.WriteLine("Disposing WMI object failed: " + ex); }
            }
        }
    }
}
