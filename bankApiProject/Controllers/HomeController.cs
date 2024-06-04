using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using bankApiProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Text.Json.Nodes;
using System.Security.Claims;
using System.Text.RegularExpressions;



namespace bankApiProject.Controllers
{



    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BankSystemContext _context; // Inject your DbContext here
        private readonly TokenizationUtility _tokenizationUtility;

        public HomeController(ILogger<HomeController> logger, BankSystemContext context)
        {
            _logger = logger;
            _context = context;
            _tokenizationUtility = new TokenizationUtility();

        }
        [HttpGet("login")]
        public async Task Login()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("GoogleResponse")
                }
                ) ; 
        }
        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!result.Succeeded)
            {
                return BadRequest(); // Handle failure
            }
            var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
            {
                claim.Issuer,
                claim.OriginalIssuer,
                claim.Type,
                claim.Value
            });
            _logger.LogInformation("User logged in with claims: {Claims}", claims);
            var emailClaim = claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email);
            if (emailClaim == null)
            {
                return BadRequest("No email claim found.");
            }

            var email = emailClaim.Value;
            _logger.LogInformation("User logged in with email: {Email}", email);

            // Check if the email is already registered in the Customer table
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerEmail == email);
           // var hashedCustomerName = BCrypt.Net.BCrypt.HashPassword(model.CustomerName);
            if (customer == null)
            {
                // If the customer does not exist, create a new record
                customer = new Customer
                {
                    CustomerEmail = email,
                    // Add any other necessary fields from claims
                    CustomerName = claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value,
                    
                //LastName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value
                CustomerPassword = null
                };
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
            }

            //return Ok(new { message = "Customer Login Sucessfully.", CustomerId = customer.CustomerId });

            // Redirect to the main page
            var redirectUrl = $"http://localhost:3000/dashboard/{customer.CustomerId}";
            return Redirect(redirectUrl);
        }


        [HttpPost("/usersignup")]
        public async Task<IActionResult> SignUp(Customer model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return UnprocessableEntity(ModelState);
                }

                if (string.IsNullOrEmpty(model.CustomerName) || string.IsNullOrEmpty(model.CustomerEmail) || string.IsNullOrEmpty(model.CustomerPassword))
                {
                    return UnprocessableEntity(new { message = "All fields are required" });
                }

                var existingCustomer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerEmail == model.CustomerEmail);
                if (existingCustomer != null)
                {
                    return Conflict(new { message = "An account with this email already exists." });
                }

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.CustomerPassword);
                var tokenizedCustomerName = _tokenizationUtility.EncryptString(model.CustomerName);

                var customer = new Customer
                {
                    CustomerName = tokenizedCustomerName,
                    CustomerEmail = model.CustomerEmail,
                    CustomerPassword = hashedPassword
                };

                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Customer created successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request." });
            }
        }








        [HttpPost("/usersignin")] // Route for user signup
        public async Task<IActionResult> SignIn(Customer model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return UnprocessableEntity(ModelState); // 422 response with validation errors
                }


                if (string.IsNullOrEmpty(model.CustomerEmail) || string.IsNullOrEmpty(model.CustomerPassword))
                {
                    return UnprocessableEntity(new { message = "All fields are required" });
                }
                var existingCustomer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerEmail == model.CustomerEmail);
                Console.WriteLine("Existing customer is : " + existingCustomer);
                if (existingCustomer != null)
                {
                    bool isPasswordValid = BCrypt.Net.BCrypt.Verify(model.CustomerPassword, existingCustomer.CustomerPassword);
                    if (isPasswordValid)
                    {
                        return Ok(new { message = "Customer Login Successfully.", CustomerId = existingCustomer.CustomerId });
                    }
                    else
                    {
                        return Conflict(new { message = "Invalid Password." });
                    }

                }
                else
                {
                    return Conflict(new { message = "Invalid Email." });

                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request." }); // 500 Internal Server Error
            }
        }

        [HttpPost("/bankcreation")]
        public async Task<IActionResult> BankCreation([FromBody] Bank bankModel)
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            if (string.IsNullOrEmpty(bankModel.BankName))
            {
                return UnprocessableEntity(new { message = "All fields are required" });
            }
            var existingBankAccount = await _context.Banks.FirstOrDefaultAsync(b => b.CustomerId == bankModel.CustomerId );
            //if (existingBankAccount != null)
            //{
             //   return Conflict(new { message = "Customer already has already registered for that Bank" });
            //}


            var bankdetail = new Bank
            {
                BankName = bankModel.BankName,
                BankAccountNumber = bankModel.BankAccountNumber,
                CustomerId = bankModel.CustomerId,
                BankBalance = bankModel.BankBalance,
                IsBankCreated = bankModel.IsBankCreated
                // Assign other properties as needed
            };

            _context.Banks.Add(bankdetail);
            await _context.SaveChangesAsync();

            return Ok(bankdetail);
        }
        [HttpGet("/viewbankaccount/{CustomerId}")]
        public IActionResult ViewBankAccount(int CustomerId)
        {

            var bankAccounts = _context.Banks
                             .Where(b => b.CustomerId == CustomerId)
                             .ToList();
            if(bankAccounts.Count() ==0)
            {
                return UnprocessableEntity(new { error = "You have not Registered for any Bank right now" });
            }
            

            return Ok(bankAccounts);
        }

        [HttpPost("/depositamount")]
        public IActionResult DepositAmount([FromBody] Bank bankModel)
        {

            if (bankModel.DepositAmount < 0)
            {

                return BadRequest(new { error = "Invalid Amount entered" });
            }

            else if (bankModel.DepositAmount < 1000)
            {
                return StatusCode(400, new { error = "Invalid amount: Deposit amount cannot be less than 1000." });



            }


            try
            {
                var bank = _context.Banks.FirstOrDefault(b => b.BankAccountNumber == bankModel.BankAccountNumber);
                if (bank == null)
                {
                   
                    return NotFound(new { error = "Bank account not found." });
                }
                if (bank.IsBankCreated.Equals(null) )
                {
                    return Unauthorized(new { error = "Your account is not authorized for deposits." });
                }

                // Update BankBalance
                bank.BankBalance += bankModel.DepositAmount;
                bank.DepositAmount = bankModel.DepositAmount;

                _context.SaveChanges();

                return Ok(new { message = "Deposit Sucessfully..", bankBalance=bank.BankBalance });
            }
            catch (Exception ex)
            {

                return StatusCode(500, "An error occurred while processing the deposit.");
            }
        }

        [HttpPost("/withdrawamount")]
        public IActionResult WithdrawAmount([FromBody] Bank bankModel)
        {

          
            try
            {
                var bank = _context.Banks.FirstOrDefault(b => b.BankAccountNumber == bankModel.BankAccountNumber);
                if (bank == null)
                {

                    return NotFound(new { error = "Bank account not found." });
                }
                if (bankModel.WithdrawalAmount < 0)
                {

                    return BadRequest(new { error = "Invalid Amount entered" });
                }

                else if (bankModel.WithdrawalAmount > bank.BankBalance)
                {
                    return StatusCode(400, new { error = "InSufficient Amount" });
                }
                if (bank.IsBankCreated.Equals(null))
                {
                    return Unauthorized(new { error = "Your account is not authorized for deposits." });
                }


                bank.BankBalance -= bankModel.WithdrawalAmount;
                bank.WithdrawalAmount = bankModel.WithdrawalAmount;

                _context.SaveChanges();

                return Ok(new { message = "Withdraw Amount Sucessfully..", bankBalance = bank.BankBalance });
            }
            catch (Exception ex)
            {

                return StatusCode(500, "An error occurred while processing the deposit.");
            }
        }

        [HttpPost("/adminsignin")] // Route for user signup
        public async Task<IActionResult> AdminLogin(Admin model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return UnprocessableEntity(ModelState); // 422 response with validation errors
                }


                if (string.IsNullOrEmpty(model.AdminUsername) || string.IsNullOrEmpty(model.AdminPassword))
                {
                    return UnprocessableEntity(new { message = "All fields are required" });
                }
                var existingCustomer = await _context.Admins.FirstOrDefaultAsync(c => c.AdminUsername == model.AdminUsername);
                Console.WriteLine("Existing customer is : " + existingCustomer);
                if (existingCustomer != null)
                {
                    var existingPassword = await _context.Admins.FirstOrDefaultAsync(c => c.AdminPassword == model.AdminPassword);
                    Console.WriteLine("Existing password is : " + existingPassword);
                    if (existingPassword != null)
                    {
                        return Ok(new { message = "Admin Login Sucessfully." });
                        // 200 OK response with a success message

                    }
                    else
                    {
                        return Conflict(new { message = "Invalid Password." });

                    }

                }
                else
                {
                    return Conflict(new { message = "Invalid Email." });

                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request." }); // 500 Internal Server Error
            }
        }

        [HttpPost("/getcustomernames")]
        public async Task<IActionResult> GetCustomers([FromBody] Customer customerModel, [FromServices] ILogger<HomeController> logger)
        {
            try
            {
                if (string.IsNullOrEmpty(customerModel.CustomerName))
                {
                    return BadRequest(new { message = "Search term is required." });
                }

                // Retrieve all customers from the database
                var allCustomers = await _context.Customers.ToListAsync();

                // Decrypt customer names and filter using the search term
                var filteredCustomers = allCustomers
                    .Select(c => new
                    {
                        c.CustomerId,
                        DecryptedCustomerName = _tokenizationUtility.DecryptString(c.CustomerName)
                    })
                    .Where(c => c.DecryptedCustomerName != null && c.DecryptedCustomerName.Contains(customerModel.CustomerName, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (filteredCustomers.Any())
                {
                    return Ok(filteredCustomers.Select(c => new { c.CustomerId, c.DecryptedCustomerName }));
                }
                else
                {
                    return NotFound(new { message = "No customers found matching the search term." });
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while processing the request."); // Log the exception
                return StatusCode(500, new { message = "An error occurred while processing your request." });
            }
        }


        // DecryptString Method













        [HttpPost("/getrecord")]
        public async Task<IActionResult> GetSpecificRecord([FromBody] Bank bankModel)
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState); // 422 response with validation errors
            }

            if (bankModel.CustomerId == null)
            {
                return UnprocessableEntity(new { message = "CustomerId is required." });
            }

            var matchingRecords = await _context.Banks
         .Where(b => b.CustomerId == bankModel.CustomerId)
         .Join(_context.Customers,
               bank => bank.CustomerId,
               customer => customer.CustomerId,
               (bank, customer) => new
               {
                   CustomerId = customer.CustomerId,
                   CustomerEmail = customer.CustomerEmail,
                   CustomerName = customer.CustomerName,
                   BankName=bank.BankName,
                   BankAccountNumber = bank.BankAccountNumber,
                   BankBalance = bank.BankBalance,
                   IsBankCreated = bank.IsBankCreated
                  
               })
         .ToListAsync();

            if (matchingRecords.Count == 0)
            {
                return NotFound(new { message = "No records found for the provided CustomerId." });
            }

            return Ok( matchingRecords);
        }


        [HttpPost("/deleterecord")]
        public async Task<IActionResult> DeleteSpecificRecord([FromBody] Bank bankModel)
        {
            try
            {

            
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState); // 422 response with validation errors
            }

            // Step 1: Find the Bank record based on the provided BankAccountNumber
            var bankRecord = await _context.Banks.SingleOrDefaultAsync(b => b.BankAccountNumber == bankModel.BankAccountNumber);

            if (bankRecord == null)
            {
                return NotFound(); // Return 404 if record not found
            }

            // Step 2: Retrieve the CustomerId associated with the Bank record
            var customerId = bankRecord.CustomerId;

            // Step 3: Use the CustomerId to find and delete the corresponding Customer record
            var customerRecord = await _context.Customers.FindAsync(customerId);
            if (customerRecord != null)
            {
                    var associatedBankRecords = _context.Banks.Where(b => b.CustomerId == customerId);
                    _context.Banks.RemoveRange(associatedBankRecords);

                    // Delete the customer record
                    _context.Customers.Remove(customerRecord);
                    await _context.SaveChangesAsync();

                }
            return Ok();

            }
            catch (Exception error)
            {
                return StatusCode(501);
            }

            // Step 4: Delete the Bank record itself

        }

        [HttpPost("/approverecord")]
        public async Task<IActionResult> ApproveAccount([FromBody] Bank bankModel)
        {
            try
            {
                if (bankModel == null || bankModel.BankAccountNumber == null)
                {
                    return BadRequest("Invalid bank account number.");
                }
                var bankAccount = await _context.Banks
              .FirstOrDefaultAsync(b => b.BankAccountNumber == bankModel.BankAccountNumber);

                if (bankAccount == null)
                {
                    return NotFound("Bank account not found.");
                }
                bankAccount.IsBankCreated = true;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Bank account approved successfully." });
            }
            catch(Exception error)
            {
                return StatusCode(501);
            }
            

        }










       }

}