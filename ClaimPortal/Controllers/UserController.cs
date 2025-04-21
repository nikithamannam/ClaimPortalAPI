#pragma warning disable CS0618
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using ClaimPortal.Models;
using ClaimPortal.Utils;
using System.Data.SqlClient;

namespace ClaimPortal.Controllers
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

            var message = cmd.ExecuteScalar()?.ToString();

            if (message == "Email   already exists.") // note the extra spaces — you may want to trim
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

            using var reader = cmd.ExecuteReader();

            
            string message = null;
            if (reader.Read())
            {
                message = reader.GetString(0);
            }

          
            if (message == "Login Done Successfully" && reader.NextResult() && reader.Read())
            {
                var user = new
                {
                    FirstName = reader["FirstName"].ToString(),
                    LastName = reader["LastName"].ToString(),
                    Email = reader["Email"].ToString(),
                    PhoneNumber = reader["PhoneNumber"].ToString(),
                    DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"])
                };

                return Ok(new { Message = message, User = user });
            }

            return Unauthorized("Invalid username or password.");
        }


        //[HttpGet("ContactUsTypes")]
        //public IActionResult GetContactUsTypes()
        //{
        //    var contactUsTypes = new List<string>();

        //    using var conn = new SqlConnection(_connectionString);
        //    conn.Open();

        //    using var cmd = new SqlCommand("ContactUsTypeDetails", conn)
        //    {
        //        CommandType = CommandType.StoredProcedure
        //    };
        //    using var reader = cmd.ExecuteReader();

        //    while (reader.Read())
        //    {
        //        contactUsTypes.Add(reader.GetString(0));
        //    }

        //    return Ok(contactUsTypes);
        //}


        //[HttpPost("ContactUs")]
        //public IActionResult ContactUs([FromBody] ContactUs contact)
        //{
        //    if (contact == null || string.IsNullOrWhiteSpace(contact.ContactUsTypeName))
        //        return BadRequest("Invalid request data or ContactUsTypeName not selected.");

        //    try
        //    {
        //        using var conn = new SqlConnection(_connectionString);
        //        conn.Open();

        //        using var cmd = new SqlCommand("InsertContactUsRequest", conn)
        //        {
        //            CommandType = CommandType.StoredProcedure
        //        };

        //        cmd.Parameters.AddWithValue("@ContactUsTypeName", contact.ContactUsTypeName);
        //        cmd.Parameters.AddWithValue("@PolicyNumber", contact.PolicyNumber);
        //        cmd.Parameters.AddWithValue("@FirstName", contact.FirstName);
        //        cmd.Parameters.AddWithValue("@LastName", contact.LastName);
        //        cmd.Parameters.AddWithValue("@Email", contact.Email);
        //        cmd.Parameters.AddWithValue("@PhoneNumber", contact.PhoneNumber);
        //        cmd.Parameters.AddWithValue("@Request", contact.Request);
        //        cmd.Parameters.AddWithValue("@DateOfBirth", contact.DateOfBirth);
        //        cmd.Parameters.AddWithValue("@ClaimNumber", contact.ClaimNumber);

        //        cmd.ExecuteNonQuery();

        //        return StatusCode(201, new { message = "Contact Us request submitted successfully." });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest($"Error: {ex.Message}");
        //    }
        //}


        [HttpPost("EmergencyTravelAssistance")]
        public IActionResult CreateEmergencyTravelAssistance([FromBody] ContactUs contact)
        {
            if (contact == null) return BadRequest("Invalid request data.");

            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();

                using var cmd = new SqlCommand("InsertContactUsRequest", conn)
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

                cmd.ExecuteNonQuery();

                return StatusCode(201, new { message = "Emergency Travel Assistance created successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}
//#pragma warning restore CS0618
//[HttpGet("userdetails/{email}")]
//public IActionResult GetEmpByEmail(string email)
//{
//    UserDetails? User = null;
//    try
//    {
//        using SqlConnection connection = new SqlConnection(_connectionString);
//        connection.Open();
//        using SqlCommand command = new SqlCommand("GetUserByEmail", connection);
//        command.CommandType = System.Data.CommandType.StoredProcedure;
//        command.Parameters.AddWithValue("@Email", email);


//        using SqlDataReader reader = command.ExecuteReader();
//        if (reader.Read())
//        {
//            User = new UserDetails
//            {

//                Email = reader["Email"].ToString(),
//                FirstName = reader["FirstName"].ToString(),
//                LastName = reader["LastName"].ToString(),
//                DateOfBirth = DateOnly.FromDateTime(Convert.ToDateTime(reader["DateOfBirth"])),
//                PhoneNumber = reader["PhoneNumber"].ToString(),

//            };
//        }


//        return User != null ? Ok(User) : NotFound("user not found");
//    }
//    catch (Exception ex)
//    {
//        return BadRequest($"Error: {ex.Message}");
//    }
//}
