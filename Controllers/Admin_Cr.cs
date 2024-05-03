using LifeLineApi.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace LifeLineApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Admin_Cr : ControllerBase
    {
        private readonly LifeLinedbContext _dbContext;

        public Admin_Cr(LifeLinedbContext dbContext)
        {
            _dbContext = dbContext;
        }
        //Contact Start
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApplicationAdmin>>> GetAdminAvailability()
        {
            if (_dbContext.ApplicationAdmins == null)
            {
                return NotFound();
            }
            var stud = await _dbContext.ApplicationAdmins.ToListAsync();

            return stud;
        }

        [HttpPost]

        public async Task<ActionResult<ApplicationAdmin>> PostAdmin(ApplicationAdmin s)
        {

            User t = new User();
            t.Email = s.AaEmail;
            t.Password = s.AaPassword;

            t.RoleId = 1;

            _dbContext.Users.Add(t);
            _dbContext.ApplicationAdmins.Add(s);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAdminAvailability), new { id = s.AaId }, s);

        }

        [HttpPut]
        public async Task<ActionResult> PutAdminAvailability(int id, ApplicationAdmin s)
        {
            if (id != s.AaId)
            {
                return BadRequest();
            }
            _dbContext.Entry(s).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {


                if (!HAdminAvailabilityAvailable(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok();

        }

        private bool HAdminAvailabilityAvailable(int id)
        {
            return (_dbContext.ApplicationAdmins?.Any(x => x.AaId == id)).GetValueOrDefault();
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteAdminAvailability(int id)
        {
            if (_dbContext.ApplicationAdmins == null)
            {
                return NotFound();
            }
            var stud = await _dbContext.ApplicationAdmins.FindAsync(id);

            if (stud == null)
            {
                return NotFound();
            }
            _dbContext.ApplicationAdmins.Remove(stud);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("ALog")]
        public IActionResult LogAdminAvailability([FromBody] ApplicationAdmin a)
        {
            try
            {
                if (_dbContext.ApplicationAdmins == null)
                {
                    return NotFound();
                }
                var stud = _dbContext.ApplicationAdmins
                  .Where(x => x.AaUserName == a.AaUserName && x.AaPassword == a.AaPassword)
                  .SingleOrDefault();

                if (stud == null)
                {
                    return NotFound();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing request: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }
        

        [HttpPost("Login")]
        public IActionResult LogAdminAvailability([FromBody] User a)
        {
            try
            {
                if (_dbContext.Users == null)
                {
                    return NotFound();
                }
                var stud = _dbContext.Users
                  .Where(x => x.Email == a.Email && x.Password == a.Password)
                  .SingleOrDefault();

                if (stud == null)
                {
                    return NotFound();
                }
                var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, a.Email) },
                        CookieAuthenticationDefaults.AuthenticationScheme);

                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                HttpContext.Session.SetString("useremail", a.Email);

                object userProfile = null;
                switch (stud.RoleId)
                {
                    case 1:
                        userProfile = _dbContext.ApplicationAdmins
                            .Where(admin => admin.AaEmail == a.Email)
                            .SingleOrDefault();
                        break;
                    case 2:
                        userProfile = _dbContext.Hospitals
                            .Where(hospital => hospital.HEmail == a.Email)
                            .SingleOrDefault();
                        break;
                    case 3:
                        userProfile = _dbContext.HEmployees
                            .Where(hEmployee => hEmployee.HeEmail == a.Email)
                            .SingleOrDefault();
                        break;
                    case 4:
                        userProfile = _dbContext.Doctors
                            .Where(doctor => doctor.DEmail == a.Email)
                            .SingleOrDefault();
                        break;
                    case 5:
                        userProfile = _dbContext.Patients
                            .Where(patient => patient.PEmail == a.Email)
                            .SingleOrDefault();
                        break;
                    default:
                        return BadRequest("Invalid RoleId");
                }
                var userDto = new
                {
                    
                    role_id = stud.RoleId,
                    email = stud.Email,
                    
                    userProfile,
                };

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing request: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }
        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            // Clear cookies
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Add Cache-Control header to prevent caching
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate"; 
            Response.Headers["Pragma"] = "no-cache"; // HTTP 1.0
            Response.Headers["Expires"] = "0"; // Proxies

            return Ok();
        }
        [HttpGet("LoggedInHospital")]
        //[Authorize]
        public IActionResult GetLoggedInHospital()
        {
            var userEmail = HttpContext.User.Identity.Name; // Get user email from claims

            // Fetch hospital admin information based on userEmail from the database
            var hospitalAdmin = _dbContext.Users
                .FirstOrDefault(x => x.Email == userEmail);

            if (hospitalAdmin == null)
            {
                return NotFound();
            }

            // Return the hospital admin information
            var hospitalAdminDto = new
            {
                role_id = hospitalAdmin.RoleId,
                email = hospitalAdmin.Email,
                // Add other properties you want to expose
            };

            return Ok(hospitalAdminDto);
        }


    }
}