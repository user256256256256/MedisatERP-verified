﻿using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.Design;
using Newtonsoft.Json.Linq;
using MedisatERP.Areas.CoreSystem.Models;
using MedisatERP.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace MedisatERP.Controllers
{
    [Route("api/[controller]/[action]")]
    public class AspNetUsersAPIController : Controller
    {
        private MedisatErpDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AspNetUsersAPIController(MedisatErpDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Retrieves a list of User Accounts with their associated address  data basing on the selected company.
        /// Supports filtering, sorting, and pagination.
        /// </summary>
        /// <param name="loadOptions">The options for filtering, sorting, and paging data.</param>
        /// <returns>Returns the processed User Accounts data.</returns>
        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions, Guid? companyId)
        {
            var usersQuery = _context.AspNetUsers.AsQueryable();

            // Filter by CompanyId if provided, otherwise fetch users with a NULL CompanyId
            if (companyId.HasValue)
            {
                usersQuery = usersQuery.Where(u => u.CompanyId == companyId.Value);
            }
            else
            {
                usersQuery = usersQuery.Where(u => u.CompanyId == null);
            }

            // Select the necessary user fields, including roles
            var users = usersQuery
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.Email,
                    u.PhoneNumber,
                    u.EmailConfirmed,
                    u.PhoneNumberConfirmed,
                    u.LockoutEnabled,
                    u.LockoutEnd,
                    u.AccessFailedCount,
                    u.BioData,
                    u.ProfileImagePath,
                    // Generate a comma-separated list of roles
                    CurrentRoles = string.Join(", ", u.Roles.Select(r => r.Name)), // Join roles as a comma-separated string
                })
                .OrderBy(u => u.Id); // Sorting users by Id, adjust if necessary

            // Apply filtering, sorting, and paging using DataSourceLoader
            var transformedData = await DataSourceLoader.LoadAsync(users, loadOptions);

            return Json(transformedData); // Return the processed data
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AspNetUser userInput)
        {
            try
            {
                // Log the content of the incoming request for debugging
                if (userInput == null)
                {
                    Console.WriteLine("Received user input: null");
                    return BadRequest(new { message = "User input cannot be empty." });
                }

                var userInputJson = JsonConvert.SerializeObject(userInput, Formatting.Indented);
                Console.WriteLine($"Received user input: {userInputJson}");

                // List of restricted roles (e.g., "System Administrator")
                var restrictedRoles = new List<string> { "System Administrator" };

                // ******To do ********** //
                // if account is just being created and role is restricted roles add account lockup enable

                // Handle roles if provided in the user input
                if (userInput.Roles != null && userInput.Roles.Any())
                {
                    foreach (var roleObject in userInput.Roles)
                    {
                        // Assuming each role is an object with an Id property (of type Guid)
                        if (roleObject == null || roleObject.Id == null)
                        {
                            Console.WriteLine($"Invalid Role object: {roleObject}");
                            return BadRequest(new { message = "Invalid Role object." });
                        }

                        var roleId = roleObject.Id;

                        // Find the role in the database by role ID (Guid)
                        var roleFromDb = await _context.AspNetRoles
                            .FirstOrDefaultAsync(r => r.Id == roleId);

                        if (roleFromDb == null)
                        {
                            // If the role doesn't exist, return an error
                            Console.WriteLine($"Role with ID '{roleId}' not found.");
                            return NotFound(new { message = $"Role with ID '{roleId}' not found." });
                        }

                        // Check if the role is restricted and if the user has a CompanyId
                        if (userInput.CompanyId.HasValue && restrictedRoles.Contains(roleFromDb.Name))
                        {
                            // If the role is restricted and user has a CompanyId, terminate and return an error
                            Console.WriteLine($"User has a CompanyId and role '{roleFromDb.Name}' is restricted. Terminating.");
                            return BadRequest(new { message = "Role NOT applicable for user!" });
                        }
                    }
                }

                // Create an IdentityUser instance for UserManager only after role checks
                var identityUser = new IdentityUser
                {
                    UserName = userInput.UserName,
                    Email = userInput.Email,
                    PhoneNumber = userInput.PhoneNumber,
                    NormalizedUserName = userInput.NormalizedUserName,
                    NormalizedEmail = userInput.NormalizedEmail
                };

                // Password hashing
                var passwordHasher = new PasswordHasher<IdentityUser>();
                identityUser.PasswordHash = passwordHasher.HashPassword(identityUser, userInput.PasswordHash);

                // Create the user using _userManager
                var createResult = await _userManager.CreateAsync(identityUser);
                if (!createResult.Succeeded)
                {
                    return BadRequest(new { message = "Failed to create user.", errors = createResult.Errors });
                }

                // Handle roles after user is created
                if (userInput.Roles != null && userInput.Roles.Any())
                {
                    foreach (var roleObject in userInput.Roles)
                    {
                        var roleId = roleObject.Id;

                        // Find the role in the database by role ID (Guid)
                        var roleFromDb = await _context.AspNetRoles
                            .FirstOrDefaultAsync(r => r.Id == roleId);

                        if (roleFromDb != null)
                        {
                            var roleName = roleFromDb.Name;

                            // Check if the role is restricted and if the user has a CompanyId
                            if (userInput.CompanyId.HasValue && restrictedRoles.Contains(roleName))
                            {
                                // Skip assigning restricted roles
                                Console.WriteLine($"User has a CompanyId and role '{roleName}' is restricted. Skipping assignment.");
                                continue;
                            }

                            var addRoleResult = await _userManager.AddToRoleAsync(identityUser, roleName);

                            if (!addRoleResult.Succeeded)
                            {
                                // If role assignment fails, delete the created user and return an error
                                await _userManager.DeleteAsync(identityUser);
                                Console.WriteLine("Role assignment failed. User deleted.");
                                return BadRequest(new { message = "Failed to assign role." });
                            }

                            Console.WriteLine($"Role '{roleName}' assigned successfully.");
                        }
                    }
                }

                // Handle the company ID if provided
                if (userInput.CompanyId.HasValue)
                {
                    // Update the CompanyId in the user record if it's provided
                    var userFromDb = await _context.AspNetUsers
                        .FirstOrDefaultAsync(u => u.UserName == identityUser.UserName);

                    if (userFromDb != null)
                    {
                        userFromDb.CompanyId = userInput.CompanyId; // Assign the CompanyId from userInput
                        await _context.SaveChangesAsync(); // Save changes to the database
                        Console.WriteLine($"Company Id '{userFromDb.CompanyId}' assigned successfully.");
                    }
                }

                return Json(new { Id = identityUser.Id });
            }
            catch (Exception ex)
            {
                // Log the exception message and stack trace for debugging purposes
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");

                // Return a standardized error response
                return StatusCode(500, new { message = "An internal server error occurred.", error = ex.Message });
            }
        }


        /// <summary>
        /// Updates an existing User and its Roles Data.
        /// </summary>
        /// <param name="key">The unique identifier of the user to update.</param>
        /// <param name="values">The incoming updated values as a JSON string.</param>
        /// <returns>Returns a success status if update is successful.</returns>
        [HttpPut]
        public async Task<IActionResult> Put(string key, string values)
        {
            try
            {
                var rawData = await new StreamReader(Request.Body).ReadToEndAsync();
                Console.WriteLine($"Raw Request Data: {rawData}");

                Console.WriteLine($"Attempting to update user with key: {key}");

                // Retrieve the user by unique identifier without including roles
                var model = await _context.AspNetUsers
                    .FirstOrDefaultAsync(u => u.Id == key);

                if (model == null)
                {
                    Console.WriteLine("User not found.");
                    return StatusCode(409, "User not found");
                }

                Console.WriteLine("User found, proceeding with updates.");

                // Deserialize the incoming updated values
                var valuesDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(values);

                // Log the deserialized dictionary for easier inspection
                Console.WriteLine("Deserialized values:");
                Console.WriteLine(JsonConvert.SerializeObject(valuesDict, Formatting.Indented)); // Pretty-print the JSON

                // Update other user information based on the provided values
                PopulateModel(model, valuesDict);

                // Validate the updated model
                if (!TryValidateModel(model))
                {
                    Console.WriteLine("Model validation failed.");
                    return BadRequest(GetFullErrorMessage(ModelState));
                }
                else
                {
                    Console.WriteLine("Model validated successfully.");
                }

                // Save the changes to the database
                await _context.SaveChangesAsync();
                Console.WriteLine("User updated successfully in the database.");

                return Ok();
            }
            catch (Exception ex)
            {
                // Return an internal server error if an exception occurs
                Console.WriteLine($"Exception occurred: {ex.Message}");
                return StatusCode(500, $"Internal Server error: {ex.Message}");
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string key)
        {
            try
            {
                // Log the entry point with the key being used for deletion
                Console.WriteLine($"Delete request received for user with ID: {key}");

                // Retrieve the user to delete
                var model = await _context.AspNetUsers.FirstOrDefaultAsync(item => item.Id == key);

                // Check if the user exists
                if (model == null)
                {
                    // Log that the user was not found
                    Console.WriteLine($"No user found with ID: {key}");
                    // Return not found if the user does not exist
                    return NotFound($"User with ID {key} not found.");
                }

                // Log that the user was found and is about to be deleted
                Console.WriteLine($"Found user with ID: {key}. Preparing to delete.");

                // Remove the user record
                _context.AspNetUsers.Remove(model);

                // Log the removal of the user
                Console.WriteLine($"Removing user with ID: {key}");

                // Save the changes to the database
                await _context.SaveChangesAsync();

                // Log successful deletion
                Console.WriteLine($"Successfully deleted user with ID: {key}");

                // Return No Content status after successful deletion
                return NoContent();
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error occurred while deleting user with ID: {key}. Error: {ex.Message}");

                // Return an internal server error if an exception occurs
                return StatusCode(500, $"An internal server error occurred: {ex.Message}");
            }
        }


        private void PopulateModel(AspNetUser model, IDictionary values)
        {
            string ID = nameof(AspNetUser.Id);
            string USER_NAME = nameof(AspNetUser.UserName);
            string NORMALIZED_USER_NAME = nameof(AspNetUser.NormalizedUserName);
            string EMAIL = nameof(AspNetUser.Email);
            string NORMALIZED_EMAIL = nameof(AspNetUser.NormalizedEmail);
            string EMAIL_CONFIRMED = nameof(AspNetUser.EmailConfirmed);
            string PASSWORD_HASH = nameof(AspNetUser.PasswordHash);
            string SECURITY_STAMP = nameof(AspNetUser.SecurityStamp);
            string CONCURRENCY_STAMP = nameof(AspNetUser.ConcurrencyStamp);
            string PHONE_NUMBER = nameof(AspNetUser.PhoneNumber);
            string PHONE_NUMBER_CONFIRMED = nameof(AspNetUser.PhoneNumberConfirmed);
            string TWO_FACTOR_ENABLED = nameof(AspNetUser.TwoFactorEnabled);
            string LOCKOUT_END = nameof(AspNetUser.LockoutEnd);
            string LOCKOUT_ENABLED = nameof(AspNetUser.LockoutEnabled);
            string ACCESS_FAILED_COUNT = nameof(AspNetUser.AccessFailedCount);
            string BIO_DATA = nameof(AspNetUser.BioData);  // Add BioData to the list

            // Existing property mappings
            if (values.Contains(ID))
            {
                model.Id = Convert.ToString(values[ID]);
            }

            if (values.Contains(USER_NAME))
            {
                model.UserName = Convert.ToString(values[USER_NAME]);
            }

            if (values.Contains(NORMALIZED_USER_NAME))
            {
                model.NormalizedUserName = Convert.ToString(values[NORMALIZED_USER_NAME]);
            }

            if (values.Contains(EMAIL))
            {
                model.Email = Convert.ToString(values[EMAIL]);
            }

            if (values.Contains(NORMALIZED_EMAIL))
            {
                model.NormalizedEmail = Convert.ToString(values[NORMALIZED_EMAIL]);
            }

            if (values.Contains(EMAIL_CONFIRMED))
            {
                model.EmailConfirmed = Convert.ToBoolean(values[EMAIL_CONFIRMED]);
            }

            if (values.Contains(PASSWORD_HASH))
            {
                model.PasswordHash = Convert.ToString(values[PASSWORD_HASH]);
            }

            if (values.Contains(SECURITY_STAMP))
            {
                model.SecurityStamp = Convert.ToString(values[SECURITY_STAMP]);
            }

            if (values.Contains(CONCURRENCY_STAMP))
            {
                model.ConcurrencyStamp = Convert.ToString(values[CONCURRENCY_STAMP]);
            }

            if (values.Contains(PHONE_NUMBER))
            {
                model.PhoneNumber = Convert.ToString(values[PHONE_NUMBER]);
            }

            if (values.Contains(PHONE_NUMBER_CONFIRMED))
            {
                model.PhoneNumberConfirmed = Convert.ToBoolean(values[PHONE_NUMBER_CONFIRMED]);
            }

            if (values.Contains(TWO_FACTOR_ENABLED))
            {
                model.TwoFactorEnabled = Convert.ToBoolean(values[TWO_FACTOR_ENABLED]);
            }

            if (values.Contains(LOCKOUT_END))
            {
                model.LockoutEnd = values[LOCKOUT_END] != null ? ConvertTo<System.DateTimeOffset>(values[LOCKOUT_END]) : (DateTimeOffset?)null;
            }

            if (values.Contains(LOCKOUT_ENABLED))
            {
                model.LockoutEnabled = Convert.ToBoolean(values[LOCKOUT_ENABLED]);
            }

            if (values.Contains(ACCESS_FAILED_COUNT))
            {
                model.AccessFailedCount = Convert.ToInt32(values[ACCESS_FAILED_COUNT]);
            }

            // New addition for BioData
            if (values.Contains(BIO_DATA))
            {
                model.BioData = Convert.ToString(values[BIO_DATA]);  // Update BioData field
            }
        }


        private T ConvertTo<T>(object value)
        {
            var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(T));
            if (converter != null)
            {
                return (T)converter.ConvertFrom(null, CultureInfo.InvariantCulture, value);
            }
            else
            {
                // If necessary, implement a type conversion here
                throw new NotImplementedException();
            }
        }

        private string GetFullErrorMessage(ModelStateDictionary modelState)
        {
            var messages = new List<string>();

            foreach (var entry in modelState)
            {
                foreach (var error in entry.Value.Errors)
                    messages.Add(error.ErrorMessage);
            }

            return String.Join(" ", messages);
        }
    }
}