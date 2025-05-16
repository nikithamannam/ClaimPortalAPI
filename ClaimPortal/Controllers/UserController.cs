using ClaimantPortal.Models;

using ClaimPortal.Utils;

using Microsoft.AspNetCore.Mvc;

using System.Data.SqlClient;

using System.Data;

using Microsoft.Extensions.Configuration;

using System.Reflection;
using ClaimantPortal.Models;
using ClaimPortal.Utils;

namespace ClaimPortal.Controllers

//using Microsoft.AspNetCore.Authorization;

//using System.Reflection.PortableExecutable;

{

    [Route("api/[controller]")]

    [ApiController]

    public class UserController : ControllerBase

    {

        private readonly string _connectionString;

        private readonly DbHelper _dbHelper;

        public UserController(IConfiguration configuration, DbHelper dbHelper)

        {

            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException("Connection string not found.");

            _dbHelper = dbHelper ?? throw new ArgumentNullException(nameof(dbHelper));

        }

        [HttpPost("Register")]
        public IActionResult Register([FromBody] Registration registration)
        {
            var hashedPassword = HashHelper.ComputeSha256Hash(registration.Password);

            using var conn = _dbHelper.GetConnection();
            conn.Open();

            using var cmd = new SqlCommand("RegisterUser", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@FirstName", registration.FirstName);
            cmd.Parameters.AddWithValue("@LastName", registration.LastName);
            cmd.Parameters.AddWithValue("@Email", registration.Email);
            cmd.Parameters.AddWithValue("@PhoneNumber", registration.PhoneNumber);
            cmd.Parameters.AddWithValue("@DateOfBirth", registration.DateOfBirth);
            cmd.Parameters.AddWithValue("@Password", hashedPassword);

            string? message = null;
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    message = reader["Message"].ToString();
                }
            }

            if (message != null && message.Trim().Contains("already exists", StringComparison.OrdinalIgnoreCase))
            {
                return Conflict(new { message = "User already exists." });
            }

            return Ok(new { message = message ?? "User registered." });
        }


        [HttpPost("Login")]
        public IActionResult Login([FromBody] Login login)
        {
            var hashedPassword = HashHelper.ComputeSha256Hash(login.Password);

            using var conn = _dbHelper.GetConnection();
            conn.Open();

            using var cmd = new SqlCommand("LoginUser", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Email", login.Email);
            cmd.Parameters.AddWithValue("@Password", hashedPassword);

            // Add return value parameter
            var returnValue = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
            returnValue.Direction = ParameterDirection.ReturnValue;

            object? user = null;

            using var reader = cmd.ExecuteReader();

            // Read user data from the first result set
            if (reader.HasRows && reader.Read())
            {
                user = new
                {
                    UserId = reader["UserId"] != DBNull.Value ? Convert.ToInt32(reader["UserId"]) : 0,
                    FirstName = reader["FirstName"]?.ToString() ?? "",
                    LastName = reader["LastName"]?.ToString() ?? "",
                    Email = reader["Email"]?.ToString() ?? "",
                    PhoneNumber = reader["PhoneNumber"]?.ToString() ?? "",
                    DateOfBirth = reader["DateOfBirth"] != DBNull.Value ? Convert.ToDateTime(reader["DateOfBirth"]) : (DateTime?)null,
                    Address = reader["Address"]?.ToString() ?? ""
                };
            }

            // Advance reader to completion to safely access return value
            while (reader.NextResult()) { }
            reader.Close();

            int result = (int)returnValue.Value;

            if (result == 1 && user != null)
            {
                return Ok(new
                {
                    Message = "Login successful",
                    User = user
                });
            }

            return Unauthorized("Invalid username or password.");
        }



        [HttpPost("ContactUs")]

        public IActionResult ContactUs([FromBody] ContactUs contact)

        {

            if (contact == null) return BadRequest("Invalid request data.");

            try

            {

                using var conn = new SqlConnection(_connectionString);

                conn.Open();

                using var cmd = new SqlCommand("InsertIntoContactUsDetails", conn)

                {

                    CommandType = CommandType.StoredProcedure

                };

                cmd.Parameters.AddWithValue("@ContactUsTypeName", contact.ContactUsTypeName);

                cmd.Parameters.AddWithValue("@PolicyNumber", contact.PolicyNumber);

                cmd.Parameters.AddWithValue("@FirstName", contact.FirstName);

                cmd.Parameters.AddWithValue("@LastName", contact.LastName);

                cmd.Parameters.AddWithValue("@Email", contact.Email);

                cmd.Parameters.AddWithValue("@PhoneNumber", contact.PhoneNumber);

                cmd.Parameters.AddWithValue("@Request", contact.Request);

                cmd.Parameters.AddWithValue("@DateOfBirth", contact.DateOfBirth);

                cmd.Parameters.AddWithValue("@ClaimNumber", contact.ClaimNumber);

                cmd.Parameters.AddWithValue("@ClaimType", contact.ClaimType);

                cmd.Parameters.AddWithValue("@IsAtTravel", contact.IsAtTravel ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@IsSeekingMedicalCare", contact.IsSeekingMedicalCare ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@IsTripRelated", contact.IsTripRelated ?? (object)DBNull.Value);

                cmd.ExecuteNonQuery();

                return StatusCode(201, new { message = " Your message has been sent successfully." });

            }

            catch (Exception ex)

            {

                return BadRequest($"Error: {ex.Message}");

            }

        }

        [HttpPut("UpdateUser/{id}")]
        public IActionResult UpdateUser(int id, UserDetails userDetails)
        {
            try
            {
                using var conn = _dbHelper.GetConnection();
                conn.Open();

                using var cmd = new SqlCommand("EditUserDetails", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@UserId", id);
                cmd.Parameters.AddWithValue("@FirstName", userDetails.FirstName);
                cmd.Parameters.AddWithValue("@LastName", userDetails.LastName);
                cmd.Parameters.AddWithValue("@PhoneNumber", userDetails.PhoneNumber ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DateOfBirth", userDetails.DateOfBirth);
                cmd.Parameters.AddWithValue("@Email", userDetails.Email);
                cmd.Parameters.AddWithValue("@Address", userDetails.Address ?? string.Empty);
                cmd.Parameters.AddWithValue("@Other_Phone", userDetails.OtherPhone ?? string.Empty);

                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                    return Ok(new { message = "Updated successfully" });
                else
                    return NotFound(new { message = "No user found with the given ID" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating member", error = ex.Message });
            }
        }


        [HttpGet("GetUserDetailsByEmail")]

        public IActionResult GetUserDetailsByEmail([FromQuery] string email)

        {

            try

            {

                using var conn = _dbHelper.GetConnection();

                conn.Open();

                using var cmd = new SqlCommand("GetUserByEmail", conn)

                {

                    CommandType = CommandType.StoredProcedure

                };

                cmd.Parameters.AddWithValue("@Email", email);

                using var reader = cmd.ExecuteReader();

                if (reader.Read())

                {

                    var user = new UserDetails

                    {

                        //UserId = Convert.ToInt32(reader["UserId"]),

                        FirstName = reader["FirstName"].ToString() ?? "",

                        LastName = reader["LastName"].ToString() ?? "",

                        Email = reader["Email"].ToString() ?? "",

                        PhoneNumber = reader["PhoneNumber"].ToString() ?? "",

                        DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"])

                    };

                    return Ok(user);

                }

                return NotFound(new { message = "User not found" });

            }

            catch (Exception ex)

            {

                return StatusCode(500, new { message = "Error retrieving user details", error = ex.Message });

            }

        }




        [HttpPost("UploadDocument")]

        public async Task<IActionResult> UploadDocument(IFormFile file)

        {

            if (file == null || file.Length == 0)

                return BadRequest(new { message = "No file uploaded." });

            using var ms = new MemoryStream();

            await file.CopyToAsync(ms);

            var fileBytes = ms.ToArray();

            using var conn = _dbHelper.GetConnection();

            conn.Open();

            using var cmd = new SqlCommand("InsertDocument", conn)

            {

                CommandType = CommandType.StoredProcedure

            };

            cmd.Parameters.AddWithValue("@FileName", file.FileName);

            cmd.Parameters.AddWithValue("@ContentType", file.ContentType);

            cmd.Parameters.AddWithValue("@FileData", fileBytes);

            await cmd.ExecuteNonQueryAsync();

            return Ok(new { message = "Document uploaded successfully." });

        }

    }

}
