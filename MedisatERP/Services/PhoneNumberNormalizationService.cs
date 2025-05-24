using PhoneNumbers;
using System;

namespace MedisatERP.Services
{

    public interface IPhoneNumberNormalizationService
    {
        string NormalizePhoneNumber(string phoneNumber);
    }

    public class PhoneNumberNormalizationService : IPhoneNumberNormalizationService
    {
        private readonly PhoneNumberUtil _phoneNumberUtil;

        public PhoneNumberNormalizationService()
        {
            _phoneNumberUtil = PhoneNumberUtil.GetInstance();
        }

        public string NormalizePhoneNumber(string phoneNumber)
        {
            try
            {
                var number = _phoneNumberUtil.Parse(phoneNumber, null); // null means we don’t assume a specific country.
                if (!_phoneNumberUtil.IsValidNumber(number))
                {
                    throw new ArgumentException("Invalid phone number format.");
                }

                // Return the phone number in international format with country code (E.164 format)
                return _phoneNumberUtil.Format(number, PhoneNumberFormat.E164);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error normalizing phone number: {ex.Message}");
            }
        }

    }


}
