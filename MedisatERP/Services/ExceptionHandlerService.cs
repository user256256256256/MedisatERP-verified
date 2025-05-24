using MedisatERP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Security;

namespace MedisatERP.Services
{

    public class ExceptionHandlerService
    {
        private readonly ILogger<ExceptionHandlerService> _logger;
        public ExceptionHandlerService(ILogger<ExceptionHandlerService> logger)
        {
            _logger = logger;
        }
        public IActionResult HandleException(Exception ex, ControllerBase controller = null)
        {
            // Log the exception message
            _logger.LogError(ex, "An exception occurred");

            // Customize responses for specific exceptions
            switch (ex)
            {
                case SqlException:
                    
                    return controller != null
                        ? controller.StatusCode(500, new { message = "A database error occurred. Please try again later." })
                        : HandleNonControllerException(ex);

                case InvalidOperationException:
                    if (ex.Message.Contains("Multiple constructors accepting all given argument types have been found"))
                    {
                        
                        return controller != null
                            ? controller.StatusCode(500, new { message = "There are multiple constructors with the same arguments found in the controller. Please ensure only one constructor matches the provided arguments." })
                            : HandleNonControllerException(ex);
                    }
                    
                    return controller != null
                        ? controller.StatusCode(500, new { message = "An internal server error occurred. Please try again later." })
                        : HandleNonControllerException(ex);

                case ArgumentNullException:
                    
                    return controller != null
                        ? controller.StatusCode(400, new { message = "A required argument was null. Please check your inputs and try again." })
                        : HandleNonControllerException(ex);

                case ArgumentException:
                    
                    return controller != null
                        ? controller.StatusCode(400, new { message = "An argument provided to a method is not valid. Please check your inputs and try again." })
                        : HandleNonControllerException(ex);

                case UnauthorizedAccessException:
                    
                    return controller != null
                        ? controller.StatusCode(403, new { message = "You do not have permission to access this resource." })
                        : HandleNonControllerException(ex);

                case FileNotFoundException:
                    
                    return controller != null
                        ? controller.StatusCode(404, new { message = "The requested file was not found." })
                        : HandleNonControllerException(ex);

                case DirectoryNotFoundException:
                    
                    return controller != null
                        ? controller.StatusCode(404, new { message = "The requested directory was not found." })
                        : HandleNonControllerException(ex);

                case IOException:
                    
                    return controller != null
                        ? controller.StatusCode(500, new { message = "An error occurred while accessing a file or directory. Please try again later." })
                        : HandleNonControllerException(ex);

                case TimeoutException:
                    
                    return controller != null
                        ? controller.StatusCode(408, new { message = "The operation timed out. Please try again later." })
                        : HandleNonControllerException(ex);

                case TaskCanceledException:
                    
                    return controller != null
                        ? controller.StatusCode(500, new { message = "The operation was canceled. Please try again later." })
                        : HandleNonControllerException(ex);


                case NotImplementedException:
                    
                    return controller != null
                        ? controller.StatusCode(501, new { message = "This feature is not implemented yet." })
                        : HandleNonControllerException(ex);

                case NullReferenceException:
                    
                    return controller != null
                        ? controller.StatusCode(500, new { message = "A null reference was encountered. Please try again later." })
                        : HandleNonControllerException(ex);

                case FormatException:
                    
                    return controller != null
                        ? controller.StatusCode(400, new { message = "The format of the input data is incorrect." })
                        : HandleNonControllerException(ex);

                case InvalidCastException:
                    
                    return controller != null
                        ? controller.StatusCode(400, new { message = "An invalid cast operation occurred." })
                        : HandleNonControllerException(ex);

                case KeyNotFoundException:
                    
                    return controller != null
                        ? controller.StatusCode(404, new { message = "The requested key was not found." })
                        : HandleNonControllerException(ex);

                case OperationCanceledException:
                    
                    return controller != null
                        ? controller.StatusCode(500, new { message = "The operation was canceled unexpectedly." })
                        : HandleNonControllerException(ex);

                case ApplicationException:
                    
                    return controller != null
                        ? controller.StatusCode(500, new { message = "A general application error occurred." })
                        : HandleNonControllerException(ex);

                case HttpRequestException:
                    
                    return controller != null
                        ? controller.StatusCode(502, new { message = "There was an issue with the HTTP request. Please try again later." })
                        : HandleNonControllerException(ex);

                // New exceptions added now
                case NotSupportedException:
                    
                    return controller != null
                        ? controller.StatusCode(400, new { message = "The operation is not supported." })
                        : HandleNonControllerException(ex);

                case InvalidDataException:
                    
                    return controller != null
                        ? controller.StatusCode(400, new { message = "The data provided is invalid." })
                        : HandleNonControllerException(ex);

                case MissingFieldException:
                    
                    return controller != null
                        ? controller.StatusCode(400, new { message = "A required field is missing." })
                        : HandleNonControllerException(ex);

                case SecurityException:
                    
                    return controller != null
                        ? controller.StatusCode(403, new { message = "You do not have the necessary security privileges to perform this operation." })
                        : HandleNonControllerException(ex);

                case AggregateException:
                    
                    return controller != null
                        ? controller.StatusCode(500, new { message = "An error occurred in one or more operations. Please try again later." })
                        : HandleNonControllerException(ex);

                case StackOverflowException:
                    
                    return controller != null
                        ? controller.StatusCode(500, new { message = "A stack overflow error occurred. Please try again later." })
                        : HandleNonControllerException(ex);

                case InvalidProgramException:
                    
                    return controller != null
                        ? controller.StatusCode(500, new { message = "The program encountered an invalid state." })
                        : HandleNonControllerException(ex);

                case JsonSerializationException:
                    
                    return controller != null
                        ? controller.StatusCode(500, new { message = "Invalid input format. Please check your data and try again." })
                        : HandleNonControllerException(ex);

                case SmtpException:
                    Console.WriteLine(ex);
                    return controller != null
                        ? controller.StatusCode(500, new { message = "Please check your internet connection or SMTP settings.." })
                        : HandleNonControllerException(ex);

                case DbUpdateConcurrencyException concurrencyEx:
                    Console.WriteLine("Concurrency exception occurred while updating data.");
                    var entry = concurrencyEx.Entries.Single();
                    var databaseValues = entry.GetDatabaseValues();
                    if (databaseValues == null)
                    {
                        Console.WriteLine("The record you attempted to edit was deleted by another user.");
                        return controller != null
                            ? controller.NotFound(new { success = false, message = "The record you attempted to edit was deleted by another user." })
                            : HandleNonControllerException(concurrencyEx);
                    }
                    else
                    {
                        var dbValues = (DataMigration)databaseValues.ToObject();
                        Console.WriteLine("The record you attempted to edit was modified by another user.");

                        // Optionally, reload the entity with current database values
                        entry.Reload();
                        return controller != null
                            ? controller.Conflict(new { success = false, message = "The record you attempted to edit was modified by another user.", currentValues = dbValues })
                            : HandleNonControllerException(concurrencyEx);
                    }

                case DbUpdateException dbUpdateEx:
                    return controller != null
                        ? controller.StatusCode(500, new { message = "An error occurred while updating the database. Please check for duplicate keys or other database constraints and try again." })
                        : HandleNonControllerException(dbUpdateEx);

                default:
                    // Log the exception message and stack trace for debugging purposes
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                    return controller != null
                        ? controller.StatusCode(500, new { message = "An unexpected error occurred. Please try again later.", error = ex.Message })
                        : HandleNonControllerException(ex);
            }
        }

        private IActionResult HandleNonControllerException(Exception ex)
        {
            // Handle cases where no controller context is available
            // For example, log the exception or send a default error response.
            // Handle non-controller exceptions (e.g., background services)
            _logger.LogError(ex, "An exception occurred in a non-controller context");
            Console.WriteLine($"Non-controller exception: {ex.Message}");
            return new ObjectResult(new { message = "An unexpected error occurred. Please try again later." })
            {
                StatusCode = 500
            };
        }
    }


}
