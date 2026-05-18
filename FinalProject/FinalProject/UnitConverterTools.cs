using System.Globalization;

namespace AIChat
{
    public class UnitConverterTools
    {
        public string ConvertUnits(double value, string fromUnit, string toUnit)
        {
            var normalizedFromUnit = NormalizeUnit(fromUnit);
            var normalizedToUnit = NormalizeUnit(toUnit);
            if (string.IsNullOrWhiteSpace(normalizedFromUnit) || string.IsNullOrWhiteSpace(normalizedToUnit))
            {
                return "Conversion error: both units are required.";
            }

            if (TryConvertLength(value, normalizedFromUnit, normalizedToUnit, out var lengthResult))
            {
                return FormatResult(value, fromUnit, lengthResult, toUnit);
            }

            if (TryConvertWeight(value, normalizedFromUnit, normalizedToUnit, out var weightResult))
            {
                return FormatResult(value, fromUnit, weightResult, toUnit);
            }

            if (TryConvertTemperature(value, normalizedFromUnit, normalizedToUnit, out var temperatureResult))
            {
                return FormatResult(value, fromUnit, temperatureResult, toUnit);
            }

            return $"Conversion error: unsupported conversion from {fromUnit} to {toUnit}.";
        }

        private static string FormatResult(double originalValue, string fromUnit, double convertedValue, string toUnit)
        {
            return string.Create(CultureInfo.InvariantCulture, $"{originalValue:0.####} {fromUnit} = {convertedValue:0.####} {toUnit}");
        }

        private static string NormalizeUnit(string unit)
        {
            return string.Concat((unit ?? string.Empty).Trim().ToLowerInvariant().Where(character => character != ' ' && character != '-' && character != '_'));
        }

        private static bool TryConvertLength(double value, string fromUnit, string toUnit, out double result)
        {
            result = 0;
            if (!TryGetLengthFactor(fromUnit, out var fromFactor) || !TryGetLengthFactor(toUnit, out var toFactor))
            {
                return false;
            }

            result = value * fromFactor / toFactor;
            return true;
        }

        private static bool TryConvertWeight(double value, string fromUnit, string toUnit, out double result)
        {
            result = 0;
            if (!TryGetWeightFactor(fromUnit, out var fromFactor) || !TryGetWeightFactor(toUnit, out var toFactor))
            {
                return false;
            }

            result = value * fromFactor / toFactor;
            return true;
        }

        private static bool TryConvertTemperature(double value, string fromUnit, string toUnit, out double result)
        {
            result = 0;
            if (!TryToCelsius(value, fromUnit, out var celsiusValue))
            {
                return false;
            }

            result = toUnit switch
            {
                "c" or "celsius" => celsiusValue,
                "f" or "fahrenheit" => (celsiusValue * 9 / 5) + 32,
                "k" or "kelvin" => celsiusValue + 273.15,
                _ => double.NaN
            };

            return !double.IsNaN(result);
        }

        private static bool TryToCelsius(double value, string fromUnit, out double result)
        {
            result = fromUnit switch
            {
                "c" or "celsius" => value,
                "f" or "fahrenheit" => (value - 32) * 5 / 9,
                "k" or "kelvin" => value - 273.15,
                _ => double.NaN
            };

            return !double.IsNaN(result);
        }

        private static bool TryGetLengthFactor(string unit, out double factor)
        {
            factor = unit switch
            {
                "km" or "kilometer" or "kilometers" => 1000,
                "m" or "meter" or "meters" => 1,
                "cm" or "centimeter" or "centimeters" => 0.01,
                "mm" or "millimeter" or "millimeters" => 0.001,
                "mi" or "mile" or "miles" => 1609.344,
                "ft" or "foot" or "feet" => 0.3048,
                "in" or "inch" or "inches" => 0.0254,
                _ => double.NaN
            };

            return !double.IsNaN(factor);
        }

        private static bool TryGetWeightFactor(string unit, out double factor)
        {
            factor = unit switch
            {
                "kg" or "kilogram" or "kilograms" => 1,
                "g" or "gram" or "grams" => 0.001,
                "lb" or "lbs" or "pound" or "pounds" => 0.45359237,
                "oz" or "ounce" or "ounces" => 0.028349523125,
                _ => double.NaN
            };

            return !double.IsNaN(factor);
        }
    }
}
