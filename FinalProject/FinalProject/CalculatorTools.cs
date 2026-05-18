using System.Data;
using System.Globalization;

namespace AIChat
{
    public class CalculatorTools
    {
        private static readonly HashSet<char> AllowedCharacters = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '+', '-', '*', '/', '(', ')', '.', ' ', '%'];

        public string Calculate(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                return "Calculation error: expression is required.";
            }

            var normalizedExpression = expression.Replace('×', '*').Replace('x', '*').Replace('X', '*').Replace('÷', '/').Trim();
            if (normalizedExpression.Any(character => !AllowedCharacters.Contains(character)))
            {
                return "Calculation error: only numbers and arithmetic operators are supported.";
            }

            try
            {
                var result = new DataTable { Locale = CultureInfo.InvariantCulture }.Compute(normalizedExpression, string.Empty);
                return Convert.ToString(result, CultureInfo.InvariantCulture) ?? "Calculation error: no result.";
            }
            catch (Exception ex)
            {
                return $"Calculation error: {ex.Message}";
            }
        }
    }
}
