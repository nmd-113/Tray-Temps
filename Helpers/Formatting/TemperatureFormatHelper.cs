namespace TrayTemps
{
    internal static class TemperatureFormatHelper
    {
        internal static bool IsValidTemp(float? value)
        {
            return value.HasValue &&
                   !float.IsNaN(value.Value) &&
                   !float.IsInfinity(value.Value) &&
                   value.Value >= -50 &&
                   value.Value <= 250;
        }

        internal static float GetDisplayTemp(float celsius, bool useFahrenheit)
        {
            return useFahrenheit ? (celsius * 1.8f) + 32 : celsius;
        }

        internal static string GetUnit(bool useFahrenheit)
        {
            return useFahrenheit ? "°F" : "°C";
        }

        internal static string FormatTrayTemperature(float? temp, bool useFahrenheit)
        {
            return temp.HasValue ? $"{GetDisplayTemp(temp.Value, useFahrenheit):F0}" : "NA";
        }
    }
}
