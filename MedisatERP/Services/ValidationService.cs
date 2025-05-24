using MedisatERP.Areas.AdministratorSystem.Data.Enum;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace MedisatERP.Services
{

    public interface IValidationService
    {
        (bool IsValid, string ErrorCode) IsValidEmailFormat(string email);
        (bool IsValid, string ErrorCode) IsValidPhoneNumberFormat(string phoneNumber);
        (bool IsValid, string ErrorCode) IsStrongPassword(string password);
        (bool IsValid, string ErrorCode) IsValidUsername(string username);
        (bool IsValid, string ErrorCode) IsDataWithinWordLimit(string biodata, int wordLimit);
        (bool IsValid, string ErrorCode) IsDataWithinLetterLimit(string username, int letterLimit);
        (bool IsValid, string ErrorCode) IsValidCompanyName(string companyName);
        (bool IsValid, string ErrorCode) IsValidName(string name);
        (bool IsValid, string ErrorCode) IsValidCountry(string country);
        (bool IsValid, string ErrorCode) IsValidWebsiteNameFormat(string website); // New method
    }

    public class ValidationService : IValidationService
    {
        private readonly IErrorCodeService _errorCodeService;

        public ValidationService(IErrorCodeService errorCodeService)
        {
            _errorCodeService = errorCodeService;
        }

        public (bool IsValid, string ErrorCode) IsValidEmailFormat(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email ? (true, string.Empty) : (false, "INVALID_EMAIL_FORMAT");
            }
            catch (FormatException)
            {
                return (false, "INVALID_EMAIL_FORMAT");
            }
        }

        public (bool IsValid, string ErrorCode) IsValidPhoneNumberFormat(string phoneNumber)
        {
            var phoneRegex = new Regex(@"^\+[1-9]\d{1,14}$");
            return phoneRegex.IsMatch(phoneNumber) ? (true, string.Empty) : (false, "INVALID_PHONE_NUMBER_FORMAT");
        }

        public (bool IsValid, string ErrorCode) IsStrongPassword(string password)
        {
            return password.Length >= 5 &&
                   password.Any(char.IsUpper) &&
                   password.Any(char.IsLower) &&
                   password.Any(ch => !char.IsLetterOrDigit(ch))
                ? (true, string.Empty)
                : (false, "WEAK_PASSWORD");
        }

        public (bool IsValid, string ErrorCode) IsValidUsername(string username)
        {
            var usernameRegex = new Regex(@"^[a-zA-Z0-9_]+$");
            return usernameRegex.IsMatch(username) && !username.Contains(" ")
                ? (true, string.Empty)
                : (false, "INVALID_USERNAME");
        }

        public (bool IsValid, string ErrorCode) IsValidCompanyName(string companyName)
        {
            // Allow letters (both upper and lower case), digits, underscores, and spaces
            var companyNameRegex = new Regex(@"^[a-zA-Z0-9_ ]+$");
            return companyNameRegex.IsMatch(companyName)
                ? (true, string.Empty)
                : (false, "INVALID_COMPANY_NAME");
        }

        public (bool IsValid, string ErrorCode) IsDataWithinWordLimit(string biodata, int wordLimit)
        {
            var wordCount = biodata.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
            return wordCount <= wordLimit
                ? (true, string.Empty)
                : (false, "BIODATA_TOO_LONG");
        }

        public (bool IsValid, string ErrorCode) IsValidName(string name)
        {
            var nameRegex = new Regex(@"^[a-zA-Z0-9_ ]+$");
            return nameRegex.IsMatch(name)
                ? (true, string.Empty)
                : (false, "INVALID_NAME");
        }

        public (bool IsValid, string ErrorCode) IsDataWithinLetterLimit(string username, int letterLimit)
        {
            return username.Length <= letterLimit
                ? (true, string.Empty)
                : (false, "USERNAME_TOO_LONG");
        }

        public (bool IsValid, string ErrorCode) IsValidCountry(string countryName)
        {
            // Parse the country name to the enum value
            return Enum.IsDefined(typeof(Countries), countryName)
                ? (true, string.Empty)
                : (false, "INVALID_COUNTRY_SELECTION");
        }

        public (bool IsValid, string ErrorCode) IsValidWebsiteNameFormat(string website)
        {
            var websiteRegex = new Regex(@"^(http[s]?:\/\/)?[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}\/?$");
            return websiteRegex.IsMatch(website)
                ? (true, string.Empty)
                : (false, "INVALID_WEBSITE_NAME_FORMAT");
        }
    }

}
