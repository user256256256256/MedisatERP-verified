using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MedisatERP.Areas.CoreSystem.Models;
using MedisatERP.Data;
using Microsoft.AspNetCore.Http;

namespace MedisatERP.Controllers
{
    [Route("api/[controller]/[action]")]
    public class CompaniesAPIController : Controller
    {
        private readonly  MedisatErpDbContext _context;

        // Constructor to initialize the context and HttpClient
        public CompaniesAPIController(MedisatErpDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a list of companies with their associated address data.
        /// Supports filtering, sorting, and pagination.
        /// </summary>
        /// <param name="loadOptions">The options for filtering, sorting, and paging data.</param>
        /// <returns>Returns the processed company data.</returns>
        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions)
        {
            var companies = _context.Companies
                                    .Include(c => c.Address)  // Ensure Address data is eagerly loaded
                                    .Select(i => new {
                                        i.CompanyId,
                                        i.CompanyName,
                                        i.ContactPerson,
                                        i.CompanyEmail,
                                        i.CompanyPhone,
                                        i.ExpDate,
                                        i.ApiCode,
                                        i.CompanyStatus,
                                        i.SubscriptionAmount,
                                        i.CompanyInitials,
                                        i.SmsAccount,
                                        i.PayAccount,
                                        i.PayBank,
                                        i.PayAccountName,
                                        i.Motto,
                                        i.CompanyType,
                                        i.CreatedAt,
                                        i.CompanyLogoFilePath,
                                        Address = new
                                        {
                                            i.Address.AddressId,
                                            i.Address.Street,
                                            i.Address.City,
                                            i.Address.State,
                                            i.Address.PostalCode,
                                            i.Address.Country
                                        }
                                    });

            // Apply filtering, sorting, and paging using DataSourceLoader
            var transformedData = await DataSourceLoader.LoadAsync(companies, loadOptions);

            return Json(transformedData); // Return the processed data
        }

        /// <summary>
        /// Adds a new company with its associated address data.
        /// </summary>
        /// <param name="values">The incoming values as a JSON string.</param>
        /// <returns>Returns the Status OK  of the newly created company.</returns>
        [HttpPost]
        public async Task<IActionResult> Post(string values)
        {
            try
            {
                // Deserialize the incoming request values
                var valuesDict = JsonConvert.DeserializeObject<IDictionary<string, object>>(values);

                // Extract the address data if available
                var addressData = valuesDict.ContainsKey("Address") ? valuesDict["Address"] as JObject : null;

                // Create a new Company instance and populate it with the provided data
                var model = new Company();
                var companyData = valuesDict.Where(kv => kv.Key != "Address")
                                            .ToDictionary(kv => kv.Key, kv => kv.Value);
                PopulateModel(model, companyData); // Populate the company model

                // Initialize Address if not provided
                if (model.Address == null)
                {
                    model.Address = new CompanyAddress();
                }

                // Generate a new AddressId if no AddressId is provided
                if (model.Address.AddressId == Guid.Empty)
                {
                    model.Address.AddressId = Guid.NewGuid();
                }

                // If address data is provided, populate the address model
                if (addressData != null)
                {
                    model.Address.Street = addressData["Street"]?.ToString();
                    model.Address.City = addressData["City"]?.ToString();
                    model.Address.State = addressData["State"]?.ToString();
                    model.Address.PostalCode = addressData["PostalCode"]?.ToString();
                    model.Address.Country = addressData["Country"]?.ToString();
                }

                // Set CreatedAt field if not already set
                if (model.CreatedAt == null)
                {
                    model.CreatedAt = DateTime.Now;
                }

                // Set the CompanyId if not already set
                if (model.CompanyId == Guid.Empty)
                {
                    model.CompanyId = Guid.NewGuid();
                }


                // Save the new company record to the database
                _context.Companies.Add(model);
                await _context.SaveChangesAsync();

                // Return the status ok (200) of the newly created company
                return Ok();
            }
            catch (Exception ex)
            {
                // Return an internal server error if an exception occurs
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

		/// <summary>
		/// Updates an existing company and its address data.
		/// </summary>
		/// <param name="key">The unique identifier of the company to update.</param>
		/// <param name="values">The incoming updated values as a JSON string.</param>
		/// <returns>Returns a success status if the company is updated.</returns>
		[HttpPut]
		public async Task<IActionResult> Put(Guid key, string values)
		{
			try
			{
				// Retrieve the company by its unique identifier
				var model = await _context.Companies
										  .Include(c => c.Address)
										  .FirstOrDefaultAsync(item => item.CompanyId == key);

				if (model == null)
				{
					Console.WriteLine($"Company not found with key: {key}");
					return StatusCode(404, "Company not found");
				}

				Console.WriteLine($"Company found with key: {key}, proceeding with updates.");

				// Deserialize the incoming updated values
				var valuesDict = JsonConvert.DeserializeObject<IDictionary<string, object>>(values);

				// Extract address data from the request, if provided
				var addressData = valuesDict.ContainsKey("Address") ? valuesDict["Address"] as JObject : null;

				// If address data is provided, update the address
				if (addressData != null)
				{
					if (model.Address == null)
					{
						model.Address = new CompanyAddress();
					}

					model.Address.Street = addressData["Street"]?.ToString();
					model.Address.City = addressData["City"]?.ToString();
					model.Address.State = addressData["State"]?.ToString();
					model.Address.PostalCode = addressData["PostalCode"]?.ToString();
					model.Address.Country = addressData["Country"]?.ToString();
				}

				// Extract company data and update the company fields
				var companyData = valuesDict.Where(kv => kv.Key != "Address")
											.ToDictionary(kv => kv.Key, kv => kv.Value);
				PopulateModel(model, companyData); // Update company model

				// Validate the updated model before saving
				if (!TryValidateModel(model))
				{
					Console.WriteLine("Model validation failed.");
					return BadRequest(GetFullErrorMessage(ModelState));
				}

				Console.WriteLine("Model validated successfully.");

				try
				{
					// Save the changes to the database
					await _context.SaveChangesAsync();
					Console.WriteLine("Company updated successfully in the database.");
					return Ok();
				}
				catch (DbUpdateConcurrencyException ex)
				{
					// Handle the concurrency exception
					Console.WriteLine("Concurrency exception occurred while updating company.");
					var entry = ex.Entries.Single();
					var databaseValues = entry.GetDatabaseValues();
					if (databaseValues == null)
					{
						Console.WriteLine("The record you attempted to edit was deleted by another user.");
						return NotFound(new { success = false, message = "The record you attempted to edit was deleted by another user." });
					}
					else
					{
						var dbValues = (Company)databaseValues.ToObject();
						Console.WriteLine("The record you attempted to edit was modified by another user.");

						// Optionally, reload the entity with current database values
						await entry.ReloadAsync();
						return Conflict(new { success = false, message = "The record you attempted to edit was modified by another user.", currentValues = dbValues });
					}
				}
			}
			catch (Exception ex)
			{
				// Return an internal server error if an exception occurs
				Console.WriteLine($"Exception occurred: {ex.Message}");
				return StatusCode(500, $"Internal Server error: {ex.Message}");
			}
		}


		/// <summary>
		/// Deletes a company and its associated address data by its unique identifier.
		/// </summary>
		/// <param name="key">The unique identifier of the company to delete.</param>
		/// <returns>Returns a status code indicating the result of the operation.</returns>
		[HttpDelete]
        public async Task<IActionResult> Delete(Guid key)
        {
            try
            {
                // Retrieve the company to delete
                var model = await _context.Companies
                                          .Include(c => c.Address) // Ensure Address data is eagerly loaded
                                          .FirstOrDefaultAsync(item => item.CompanyId == key);

                if (model == null)
                {
                    // Return not found if the company does not exist
                    return NotFound($"Company with ID {key} not found.");
                }

                // Step 1: Retrieve the current logo file path from the database
                string currentLogoFilePath = model.CompanyLogoFilePath;  // Fetch the current logo file name from DB (e.g., "oldLogo.jpg")

                // Step 2: Check if the logo exists and delete it if necessary
                if (!string.IsNullOrEmpty(currentLogoFilePath))
                {
                    // Ensure the folder path where logos are stored
                    string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "companyLogoImages");

                    // Step 3: Construct the full path for the current logo file
                    string currentLogoFullPath = Path.Combine(folderPath, currentLogoFilePath);

                    // Step 4: Delete the existing logo file if it exists
                    if (System.IO.File.Exists(currentLogoFullPath))
                    {
                        try
                        {
                            Console.WriteLine($"Deleting old logo file: {currentLogoFullPath}");
                            System.IO.File.Delete(currentLogoFullPath);  // Delete the old logo file
                            Console.WriteLine("Old logo file deleted successfully.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error while deleting the old logo file: {ex.Message}");
                            // Optionally, log the error and continue with the company deletion
                        }
                    }
                }

                // Step 5: Remove the associated address, if it exists
                if (model.Address != null)
                {
                    _context.CompanyAddresses.Remove(model.Address);
                }

                // Step 6: Remove the company record itself
                _context.Companies.Remove(model);

                // Step 7: Save the changes to the database
                await _context.SaveChangesAsync();

                return NoContent(); // Return No Content status after successful deletion
            }
            catch (Exception ex)
            {
                // Return an internal server error if an exception occurs
                return StatusCode(500, $"An internal server error occurred: {ex.Message}");
            }
        }


        [HttpPut]
        public async Task<ActionResult> UploadLogo(string companyId, IFormFile companyLogo)
        {
            // Log the start of the method
            Console.WriteLine("UploadLogo method started for CompanyId: " + companyId);

            if (companyLogo != null && companyLogo.Length > 0)
            {
                // Step 1: Retrieve the current logo file path from the database using companyId
                string currentLogoFilePath = GetCurrentLogoFilePath(companyId);  // This should fetch the current file name from DB (e.g., "oldLogo.jpg")

                // Step 2: Ensure the folder path where logos are stored
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "companyLogoImages");

                // Step 3: Check if there is a valid current logo file path, and delete the existing logo if it exists
                if (!string.IsNullOrEmpty(currentLogoFilePath))
                {
                    string currentLogoFullPath = Path.Combine(folderPath, currentLogoFilePath);

                    // Step 4: Delete the existing logo file if it exists
                    if (System.IO.File.Exists(currentLogoFullPath))
                    {
                        try
                        {
                            Console.WriteLine("Deleting old logo file: " + currentLogoFullPath);
                            System.IO.File.Delete(currentLogoFullPath);  // Delete the old logo
                            Console.WriteLine("Old logo file deleted.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error while deleting old file: " + ex.Message);
                            return StatusCode(500, new { error = "Error while deleting the old file: " + ex.Message });
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No current logo file path found. Skipping logo deletion.");
                }

                // Step 5: Generate a new unique file name for the logo to avoid conflicts
                string newFileName = $"{companyId}_{Path.GetFileName(companyLogo.FileName)}";  // You can change the naming logic as per your needs

                // Step 6: Construct the full path for the new logo
                string newLogoFilePath = Path.Combine(folderPath, newFileName);

                // Step 7: Save the new logo file to the server
                try
                {
                    using (var fileStream = new FileStream(newLogoFilePath, FileMode.Create))
                    {
                        Console.WriteLine("Saving new logo file to the server...");
                        await companyLogo.CopyToAsync(fileStream);
                        Console.WriteLine("New logo file saved successfully.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error while saving the new file: " + ex.Message);
                    return StatusCode(500, new { error = "Error while saving the new file: " + ex.Message });
                }

                // Step 8: Update the company's logo file path in the database with the new file name
                bool updateSuccess = UpdateCompanyLogoFilePath(companyId, newFileName);
                if (!updateSuccess)
                {
                    Console.WriteLine("Error while updating the company logo path in the database.");
                    return StatusCode(500, new { error = "Error while updating the company logo path in the database." });
                }

                // Step 9: Return the relative file path 
                string relativeFilePath = $"../../img/companyLogoImages/{newFileName}";
                Console.WriteLine("New logo file saved successfully. Returning relative path: " + relativeFilePath);

                // Return success response with the relative file path
                return Json(new { filePath = relativeFilePath });
            }
            else
            {
                Console.WriteLine("No file uploaded or file is empty.");
            }

            return Json(new { error = "No file uploaded" });
        }


        // Method to retrieve the current logo file path from the database 
        private string GetCurrentLogoFilePath(string companyId)
        {
            // Convert the companyId from string to Guid
            if (Guid.TryParse(companyId, out Guid companyIdGuid))
            {
                // Find the company by CompanyId (Guid)
                var company = _context.Companies.FirstOrDefault(c => c.CompanyId == companyIdGuid);

                // Check if the company exists
                if (company != null)
                {
                    Console.WriteLine($"Retrieved company logo path: {company.CompanyLogoFilePath}");
                    // Return the CompanyLogoFilePath if found
                    return company.CompanyLogoFilePath;
                }
                else
                {
                    // Handle the case when the company is not found
                    return null;
                }
            }
            else
            {
                // If the companyId is not valid, return null or handle error as needed
                Console.WriteLine("Invalid companyId format.");
                return null;
            }
        }

        // Method to update the company's logo path in the database
        private bool UpdateCompanyLogoFilePath(string companyId, string newFileName)
        {
            // Convert the companyId from string to Guid
            if (Guid.TryParse(companyId, out Guid companyIdGuid))
            {
                // Find the company by CompanyId (Guid)
                var company = _context.Companies.FirstOrDefault(c => c.CompanyId == companyIdGuid);

                // Check if the company exists
                if (company != null)
                {
                    try
                    {
                        // Update the CompanyLogoFilePath with the new file name
                        company.CompanyLogoFilePath = newFileName;

                        // Save changes to the database
                        _context.SaveChanges();

                        // Log success
                        Console.WriteLine("Successfully updated the company logo file path in the database.");

                        // Return true indicating the update was successful
                        return true;
                    }
                    catch (Exception ex)
                    {
                        // Log the error and return false if there's an issue
                        Console.WriteLine("Error while updating the company logo file path: " + ex.Message);
                        return false;
                    }
                }
                else
                {
                    // If the company doesn't exist, log the error
                    Console.WriteLine("Company not found in the database.");
                    return false;
                }
            }
            else
            {
                // If the companyId is invalid, log the error
                Console.WriteLine("Invalid companyId format.");
                return false;
            }
        }


        // Update the UploadLogo method to use IFormFile
        //[HttpPost]
        //public async Task<ActionResult> UploadLogo(IFormFile companyLogo)
        //{
        //    // Log the start of the method
        //    Console.WriteLine("UploadLogo method started.");

        //    if (companyLogo != null && companyLogo.Length > 0)
        //    {
        //        Console.WriteLine("File received: " + companyLogo.FileName);

        //        // Generate a unique name for the image file to avoid overwriting
        //        string fileName = Path.GetFileName(companyLogo.FileName);
        //        Console.WriteLine("Generated file name: " + fileName);

        //        // Determine the folder path to store the image
        //        string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "companyLogoImages");
        //        Console.WriteLine("Folder path: " + folderPath);

        //        // Ensure the directory exists
        //        if (!Directory.Exists(folderPath))
        //        {
        //            Console.WriteLine("Directory does not exist. Creating directory...");
        //            Directory.CreateDirectory(folderPath);
        //        }
        //        else
        //        {
        //            Console.WriteLine("Directory already exists.");
        //        }

        //        // Combine the folder path and file name to create the full file path
        //        string filePath = Path.Combine(folderPath, fileName);
        //        Console.WriteLine("Full file path: " + filePath);

        //        // Save the file to the server
        //        try
        //        {
        //            using (var fileStream = new FileStream(filePath, FileMode.Create))
        //            {
        //                Console.WriteLine("Saving file to the server...");
        //                await companyLogo.CopyToAsync(fileStream);
        //                Console.WriteLine("File saved successfully.");
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine("Error while saving the file: " + ex.Message);
        //            return Json(new { error = "Error while saving the file: " + ex.Message });
        //        }

        //        // Return the relative path to be stored in the database
        //        string relativeFilePath = "../../img/companyLogoImages/" + fileName;
        //        Console.WriteLine("File saved successfully. Returning relative path: " + relativeFilePath);

        //        return Json(new { filePath = relativeFilePath });
        //    }
        //    else
        //    {
        //        Console.WriteLine("No file uploaded or file is empty.");
        //    }

        //    return Json(new { error = "No file uploaded" });
        //}


        /// <summary>
        /// Populates the company model with the given values.
        /// </summary>
        private void PopulateModel(Company model, IDictionary<string, object> values)
        {
            // Define the field names for clarity and ease of use
            string COMPANY_ID = nameof(Company.CompanyId);
            string COMPANY_NAME = nameof(Company.CompanyName);
            string CONTACT_PERSON = nameof(Company.ContactPerson);
            string COMPANY_EMAIL = nameof(Company.CompanyEmail);
            string COMPANY_PHONE = nameof(Company.CompanyPhone);
            string EXP_DATE = nameof(Company.ExpDate);
            string API_CODE = nameof(Company.ApiCode);
            string COMPANY_STATUS = nameof(Company.CompanyStatus);
            string SUBSCRIPTION_AMOUNT = nameof(Company.SubscriptionAmount);
            string COMPANY_INITIALS = nameof(Company.CompanyInitials);
            string SMS_ACCOUNT = nameof(Company.SmsAccount);
            string PAY_ACCOUNT = nameof(Company.PayAccount);
            string PAY_BANK = nameof(Company.PayBank);
            string PAY_ACCOUNT_NAME = nameof(Company.PayAccountName);
            string MOTTO = nameof(Company.Motto);
            string COMPANY_TYPE = nameof(Company.CompanyType);
            string ADDRESS_ID = nameof(Company.AddressId);
            string CREATED_AT = nameof(Company.CreatedAt);

            if (values.ContainsKey(COMPANY_ID))
            {
                model.CompanyId = ConvertTo<Guid>(values[COMPANY_ID]);
            }

            if (values.ContainsKey(COMPANY_NAME))
            {
                model.CompanyName = Convert.ToString(values[COMPANY_NAME]);
            }

            if (values.ContainsKey(CONTACT_PERSON))
            {
                model.ContactPerson = Convert.ToString(values[CONTACT_PERSON]);
            }

            if (values.ContainsKey(COMPANY_EMAIL))
            {
                model.CompanyEmail = Convert.ToString(values[COMPANY_EMAIL]);
            }

            if (values.ContainsKey(COMPANY_PHONE))
            {
                model.CompanyPhone = Convert.ToString(values[COMPANY_PHONE]);
            }

            if (values.ContainsKey(EXP_DATE))
            {
                model.ExpDate = values[EXP_DATE] != null ? Convert.ToDateTime(values[EXP_DATE]) : (DateTime?)null;
            }

            if (values.ContainsKey(API_CODE))
            {
                model.ApiCode = Convert.ToString(values[API_CODE]);
            }

            if (values.ContainsKey(COMPANY_STATUS))
            {
                model.CompanyStatus = Convert.ToString(values[COMPANY_STATUS]);
            }

            if (values.ContainsKey(SUBSCRIPTION_AMOUNT))
            {
                model.SubscriptionAmount = values[SUBSCRIPTION_AMOUNT] != null ? Convert.ToDecimal(values[SUBSCRIPTION_AMOUNT], CultureInfo.InvariantCulture) : (decimal?)null;
            }

            if (values.ContainsKey(COMPANY_INITIALS))
            {
                model.CompanyInitials = Convert.ToString(values[COMPANY_INITIALS]);
            }

            if (values.ContainsKey(SMS_ACCOUNT))
            {
                model.SmsAccount = Convert.ToString(values[SMS_ACCOUNT]);
            }

            if (values.ContainsKey(PAY_ACCOUNT))
            {
                model.PayAccount = Convert.ToString(values[PAY_ACCOUNT]);
            }

            if (values.ContainsKey(PAY_BANK))
            {
                model.PayBank = Convert.ToString(values[PAY_BANK]);
            }

            if (values.ContainsKey(PAY_ACCOUNT_NAME))
            {
                model.PayAccountName = Convert.ToString(values[PAY_ACCOUNT_NAME]);
            }

            if (values.ContainsKey(MOTTO))
            {
                model.Motto = Convert.ToString(values[MOTTO]);
            }

            if (values.ContainsKey(COMPANY_TYPE))
            {
                model.CompanyType = Convert.ToString(values[COMPANY_TYPE]);
            }

            if (values.ContainsKey(ADDRESS_ID))
            {
                model.AddressId = ConvertTo<Guid>(values[ADDRESS_ID]);
            }

            if (values.ContainsKey(CREATED_AT))
            {
                model.CreatedAt = values[CREATED_AT] != null ? Convert.ToDateTime(values[CREATED_AT]) : (DateTime?)null;
            }
        }


        /// <summary>
        /// Converts a value to the specified type.
        /// </summary>
        private T ConvertTo<T>(object value)
        {
            var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(T));
            if (converter != null)
            {
                return (T)converter.ConvertFrom(null, CultureInfo.InvariantCulture, value);
            }
            else
            {
                throw new NotImplementedException("Conversion not implemented");
            }
        }

        /// <summary>
        /// Retrieves a full error message from the ModelState.
        /// </summary>
        private string GetFullErrorMessage(ModelStateDictionary modelState)
        {
            return string.Join(" ", modelState.SelectMany(entry => entry.Value.Errors).Select(error => error.ErrorMessage));
        }

        [HttpGet]
        public async Task<IActionResult> GetCompanyLogo(Guid companyId)
        {
            // Log the companyId to the console to track if it's null or invalid
            Console.WriteLine($"Received CompanyId: {companyId}");

            // If CompanyId is null or invalid, we return an error response
            if (companyId == Guid.Empty)
            {
                Console.WriteLine("CompanyId is empty or null");
                return BadRequest("Unauthorized access: Invalid Company Id!");
            }

            // Fetch the company logo file path
            var companyLogo = await _context.Companies
                .Where(c => c.CompanyId == companyId)
                .Select(c => c.CompanyLogoFilePath)
                .FirstOrDefaultAsync();

            // Log the logo path or default fallback
            if (string.IsNullOrEmpty(companyLogo))
            {
                companyLogo = "defaultCompanyLogo.jpeg";  // Default logo if not found
                Console.WriteLine("No logo found, using default: " + companyLogo);
            }
            else
            {
                Console.WriteLine($"Found company logo: {companyLogo}");
            }

            // Log the final data being returned
            Console.WriteLine($"Returning logo file path: {companyLogo}");

            // Return the data in the expected format for DevExtreme (including CompanyId)
            return Json(new
            {
                data = new[] {
            new {
                CompanyId = companyId,  // Include the CompanyId as the key field
                CompanyLogoFilePath = companyLogo
            }
        }
            });
        }
    }
}
