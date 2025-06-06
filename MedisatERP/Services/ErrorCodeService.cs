﻿namespace MedisatERP.Services
{

    public interface IErrorCodeService
    {
        (string ErrorCode, string ErrorMessage) GetErrorDetails(string errorCode);
    }

    public class ErrorCodeService : IErrorCodeService
    {
        private readonly Dictionary<string, string> _errorDetails;

        public ErrorCodeService()
        {
            // Initialize a dictionary of error codes and their corresponding messages
            _errorDetails = new Dictionary<string, string>
                {
                // Authentication and Authorization Errors
                { "MISSING_CREDENTIALS", "Missing required credentials." },
                { "INVALID_EMAIL", "Please enter a valid email address." },
                { "USER_NOT_FOUND", "No user found with those credentials." },
                { "INVALID_PASSWORD", "Incorrect password." },
                { "ACCOUNT_LOCKED", "Your account has been locked due to multiple failed login attempts. Please try again later." },
                { "ACCESS_DENIED", "You do not have permission to access this resource." },
                { "TWO_FACTOR_NOT_ENABLED", "Two-factor authentication is not enabled for this user." },
                { "NO_VALID_2FA_PROVIDERS", "No valid 2FA providers available." },
                { "INVALID_2FA_PROVIDER", "Invalid 2FA provider." },
                { "USER_LOCKED_OUT", "User is locked out." },
                { "SIGN_IN_NOT_ALLOWED", "Sign-in not allowed." },
                { "AUTHENTICATION_FAILED", "Authentication failed." },
                { "INVALID_USER_ID", "Invalid user ID." },
                { "INVALID_USER_ID_AFTER_DECODING", "Invalid user ID after decoding." },
                { "LOGOUT_FAILED", "Logout failed. Please try again." },
                { "EMAIL_NOT_CONFIRMED", "Your email exists but it is not confirmed, a confirmation link has been sent to your email address." },
                { "FAILED_TO_GENERATE_CODE", "Failed to generate code try using another provider or contact support." },
                { "INCORRECT_PASSWORD_FORMAT", "Password doesn’t meet security requirements. Please use a mix of letters and numbers." },
                { "SESSION_EXPIRED", "Your session has expired. Please log in again." },
                { "DISABLED_ACCOUNT", "Your account has been disabled. Please contact support." },

                // Validation Errors
                { "INVALID_INPUT", "The input provided is invalid." },
                { "REQUIRED_FIELD", "This field is required." },
                { "INVALID_DATE_FORMAT", "Please enter a valid date format." },
                { "OUT_OF_RANGE", "The value entered is out of the allowed range." },
                { "INVALID_ROLE_OBJECT", "Invalid Role object." },
                { "INVALID_EMAIL_FORMAT", "The email address is not in a valid format." },
                { "INVALID_PHONE_NUMBER_FORMAT","The phone number is not in a valid format. Please use the standard format: +<CountryCode><PhoneNumber>, e.g., +256 for the UG or +44 for the UK."},
                {"WEAK_PASSWORD","Password must be at least 5 characters long, contain one non-alphanumeric character, one uppercase letter, and one lowercase letter."},
                { "USERNAME_ALREADY_EXISTS", "The username is already taken by another user." },
                { "EMAIL_ALREADY_REGISTERED", "The email address is already registered with another user." },
                { "PHONE_NUMBER_ALREADY_REGISTERED", "The phone number is already registered with another user." },
                { "INVALID_COUNTRY_SELECTION", "The selected country is not valid or not recognized." },
                { "INVALID_GENDER_SELECTION", "The selected gender is not valid or not recognized." },
                { "BIODATA_TOO_LONG", "Bio data should be less than 250 words." },
                { "INVALID_USERNAME", " Username must not contain spaces and should only have alphanumeric characters and underscores." },
                { "USERNAME_TOO_LONG", " Username must not exceed 15 letters." },
                { "ADDRESS_FIELD_TOO_LONG", "Address fields should not exceed 10 letters." },
                { "INVALID_COMPANY_NAME", "Company name should only have alphanumeric characters and underscores." },
                { "COMPANY_NAME_TOO_LONG", " Company name must not exceed 15 letters."},
                { "COMPANY_NAME_ALREADY_EXISTS", "Company name already registered." },
                { "COMPANY_INITIALS_TOO_LONG", "Company initial must not exceed 3 letters. e.g 'LA' for a company name 'Lyfex Africa'"},
                { "COMPANY_MOTTO_TOO_LONG", "Company motto should not exceed 15 words."},
                { "INVALID_NAME", "Name should only have alphanumeric characters and underscores and should not exceed 15 letters." },
                { "COMPANY_EMAIL_ALREADY_EXISTS",  "The email address is already registered with another company." },
                { "INVALID_COUNTRY_NAME", "The country name provided is not valid. Please check the spelling or use a valid country name."},
                { "INVALID_WEBSITE_NAME_FORMAT", "The provided website URL format is invalid. Please ensure it follows the standard URL format (e.g., http://www.example.com)." },


                // Database Errors
                { "DB_CONNECTION_FAILED", "Failed to connect to the database." },
                { "DB_QUERY_FAILED", "An error occurred while executing the database query." },
                { "RECORD_NOT_FOUND", "The requested record was not found." },
                { "DUPLICATE_RECORD", "A record with the same key already exists." },
                { "ROLE_NOT_FOUND", "Role not found." },
                { "USER_CREATION_FAILED", "Failed to create user." },
                { "ROLE_ASSIGNMENT_FAILED", "Failed to assign role." },
                { "CONCURRENCY_CONFLICT", "The record you attempted to edit was modified by another user." },

                // File Handling Errors
                { "FILE_NOT_FOUND", "The requested file was not found." },
                { "FILE_UPLOAD_FAILED", "Failed to upload the file." },
                { "FILE_READ_ERROR", "An error occurred while reading the file." },
                { "FILE_DELETE_FAILED", "Error while deleting the old file." },
                { "FILE_SAVE_FAILED", "Error while saving the new file." },
                { "DB_UPDATE_FAILED", "Error while updating the user profile picture path in the database." },
                { "NO_FILE_UPLOADED", "No file uploaded or file is empty." },

                // Network Errors
                { "NETWORK_ERROR", "A network error occurred. Please try again later." },
                { "TIMEOUT", "The operation timed out. Please try again." },

                // Miscellaneous Errors
                { "UNKNOWN_ERROR", "An unknown error occurred." },
                { "NOT_IMPLEMENTED", "This feature has not been implemented yet." },
                { "SERVICE_UNAVAILABLE", "The service is currently unavailable. Please try again later." },
                { "ROLE_NOT_APPLICABLE", "Role NOT applicable for user!" },
                { "INTERNAL_SERVER_ERROR", "An internal server error occurred." },
                { "EMAIL_SEND_ERROR", "Error sending email." },
                { "INVALID_REQUEST", "Invalid password recovery request." },
                { "PASSWORD_RESET_FAILED", "Password reset failed." }
            };
        }

        public (string ErrorCode, string ErrorMessage) GetErrorDetails(string errorCode)
        {
            if (_errorDetails.ContainsKey(errorCode))
            {
                return (errorCode, _errorDetails[errorCode]);
            }

            // Default error message if errorCode is not found
            return ("UNKNOWN_ERROR", "An unknown error occurred.");
        }
    }


}