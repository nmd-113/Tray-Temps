using LibreHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace TrayTemps
{
    internal static class ComponentDisplayNameHelper
    {
        internal static List<string> GetLhmDisplayNames(IEnumerable<IHardware> hardwares)
        {
            return hardwares?
                .Where(hardware => hardware != null && !string.IsNullOrWhiteSpace(hardware.Name))
                .GroupBy(hardware => hardware.Identifier.ToString(), StringComparer.OrdinalIgnoreCase)
                .Select(group => HardwareReportFormatHelper.Safe(group.First().Name))
                .Where(name => name != "N/A")
                .ToList() ?? new List<string>();
        }

        internal static List<string> MergeStorageDisplayNames(
            IEnumerable<string> wmiNames,
            IEnumerable<string> lhmNames)
        {
            List<string> reliableWmiNames = wmiNames?
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Select(HardwareReportFormatHelper.SanitizeSingleLineText)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .ToList() ?? new List<string>();

            var mergedNames = new List<string>(reliableWmiNames);

            if (lhmNames == null)
                return mergedNames;

            foreach (string lhmName in lhmNames.Where(name => !string.IsNullOrWhiteSpace(name)))
            {
                string cleanLhmName = HardwareReportFormatHelper.SanitizeSingleLineText(lhmName);
                string normalizedLhmName = HardwareReportFormatHelper.NormalizeHardwareText(cleanLhmName);
                bool alreadyRepresentedByWmi = reliableWmiNames.Any(wmiName =>
                    string.Equals(
                        HardwareReportFormatHelper.NormalizeHardwareText(wmiName),
                        normalizedLhmName,
                        StringComparison.OrdinalIgnoreCase));

                if (!alreadyRepresentedByWmi)
                    mergedNames.Add(cleanLhmName);
            }

            return mergedNames;
        }

        internal static List<string> MergeComponentDisplayNames(
            IEnumerable<string> wmiNames,
            IEnumerable<string> lhmNames,
            bool isCpu)
        {
            List<string> cleanWmiNames = CleanComponentDisplayNames(wmiNames);
            List<string> cleanLhmNames = CleanComponentDisplayNames(lhmNames);
            var mergedNames = new List<string>(cleanWmiNames);
            var matchedWmiNames = new bool[cleanWmiNames.Count];

            foreach (string lhmName in cleanLhmNames)
            {
                int matchingWmiIndex = -1;

                for (int index = 0; index < cleanWmiNames.Count; index++)
                {
                    if (!matchedWmiNames[index] &&
                        AreSameComponentDisplayName(cleanWmiNames[index], lhmName, isCpu))
                    {
                        matchingWmiIndex = index;
                        break;
                    }
                }

                if (matchingWmiIndex >= 0)
                    matchedWmiNames[matchingWmiIndex] = true;
                else
                    mergedNames.Add(lhmName);
            }

            return mergedNames;
        }

        internal static string FormatDisplayNames(IReadOnlyList<string> names, string emptyText)
        {
            if (names == null || names.Count == 0)
                return emptyText;

            return names.Count == 1
                ? names[0]
                : string.Join(" | ", names.Select((name, index) => $"{index + 1}.{name}"));
        }

        private static bool AreSameComponentDisplayName(string firstName, string secondName, bool isCpu)
        {
            string first = NormalizeComponentDisplayName(firstName, isCpu);
            string second = NormalizeComponentDisplayName(secondName, isCpu);

            return !string.IsNullOrEmpty(first) &&
                string.Equals(first, second, StringComparison.OrdinalIgnoreCase);
        }

        private static string NormalizeComponentDisplayName(string name, bool isCpu)
        {
            string text = HardwareReportFormatHelper.SanitizeSingleLineText(name).ToUpperInvariant();
            text = text.Replace("(R)", string.Empty)
                .Replace("(TM)", string.Empty);

            int clockSuffixIndex = text.LastIndexOf(" @ ", StringComparison.Ordinal);
            bool hasWmiCpuClockSuffix = isCpu && clockSuffixIndex > 0 &&
                (" " + text.Substring(0, clockSuffixIndex) + " ")
                    .IndexOf(" CPU ", StringComparison.Ordinal) >= 0 &&
                IsClockFrequencySuffix(text.Substring(clockSuffixIndex + " @ ".Length));

            if (hasWmiCpuClockSuffix)
                text = text.Substring(0, clockSuffixIndex);

            string tokenText = new string(text
                .Select(character => char.IsLetterOrDigit(character) ? character : ' ')
                .ToArray());
            var tokens = tokenText
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            string generationSuffix = isCpu && tokens.Count > 0 && tokens[0].Length > 2
                ? tokens[0].Substring(tokens[0].Length - 2)
                : string.Empty;
            bool hasGenerationPrefix = isCpu && tokens.Count >= 2 && tokens[1] == "GEN" &&
                (generationSuffix == "ST" || generationSuffix == "ND" ||
                 generationSuffix == "RD" || generationSuffix == "TH") &&
                int.TryParse(tokens[0].Substring(0, tokens[0].Length - 2), out _);

            if (hasGenerationPrefix)
                tokens.RemoveRange(0, 2);

            if (isCpu)
            {
                tokens.RemoveAll(token => token == "CPU" || token == "PROCESSOR");
                RemoveAmdIntegratedGraphicsSuffix(tokens);
            }

            if (!isCpu && tokens.Count > 0 &&
                (tokens[tokens.Count - 1] == "PROCESSOR" || tokens[tokens.Count - 1] == "GPU"))
            {
                tokens.RemoveAt(tokens.Count - 1);
            }

            if (isCpu && tokens.Count >= 2 && tokens[tokens.Count - 1] == "CORE" &&
                IsCoreCountToken(tokens[tokens.Count - 2]))
            {
                tokens.RemoveRange(tokens.Count - 2, 2);
            }

            return string.Concat(tokens);
        }

        private static void RemoveAmdIntegratedGraphicsSuffix(List<string> tokens)
        {
            if (tokens.Count < 4 || tokens[0] != "AMD" ||
                (tokens[tokens.Count - 1] != "GRAPHICS" && tokens[tokens.Count - 1] != "GFX"))
            {
                return;
            }

            for (int index = 2; index + 1 < tokens.Count; index++)
            {
                if ((tokens[index] == "WITH" || tokens[index] == "W") &&
                    tokens[index + 1] == "RADEON")
                {
                    tokens.RemoveRange(index, tokens.Count - index);
                    return;
                }
            }
        }

        private static bool IsCoreCountToken(string token)
        {
            if (int.TryParse(token, out _))
                return true;

            switch (token)
            {
                case "SINGLE":
                case "DUAL":
                case "TRIPLE":
                case "QUAD":
                case "SIX":
                case "EIGHT":
                case "TEN":
                case "TWELVE":
                case "SIXTEEN":
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsClockFrequencySuffix(string suffix)
        {
            string compactSuffix = suffix.Replace(" ", string.Empty);
            string unit = compactSuffix.EndsWith("GHZ", StringComparison.Ordinal)
                ? "GHZ"
                : compactSuffix.EndsWith("MHZ", StringComparison.Ordinal) ? "MHZ" : null;

            return unit != null &&
                double.TryParse(
                    compactSuffix.Substring(0, compactSuffix.Length - unit.Length),
                    NumberStyles.AllowDecimalPoint,
                    CultureInfo.InvariantCulture,
                    out double clock) &&
                clock > 0;
        }

        private static List<string> CleanComponentDisplayNames(IEnumerable<string> names)
        {
            return names?
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Select(HardwareReportFormatHelper.SanitizeSingleLineText)
                .Where(name => !string.IsNullOrWhiteSpace(name) && name != "N/A")
                .ToList() ?? new List<string>();
        }
    }
}
