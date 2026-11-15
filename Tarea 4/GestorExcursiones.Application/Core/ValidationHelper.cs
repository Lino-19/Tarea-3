using System.Net.Mail;
using System.Text.RegularExpressions;

namespace GestorExcursiones.Application.Core;

public static class ValidationHelper
{
    private static readonly Regex NameRegex = new(@"^[\p{L}][\p{L}\s'.\-]*$", RegexOptions.Compiled);
    private static readonly Regex PhoneRegex = new(@"^\+?[0-9\s\-()]{7,20}$", RegexOptions.Compiled);

    public static bool IsValidName(string value)
    {
        return NameRegex.IsMatch(value);
    }

    public static bool IsValidPhone(string value)
    {
        return PhoneRegex.IsMatch(value);
    }

    public static bool IsValidEmail(string value)
    {
        try
        {
            var address = new MailAddress(value);
            return address.Address == value;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    public static string OnlyDigits(string value)
    {
        return new string(value.Where(char.IsDigit).ToArray());
    }
}
