using LifeLineApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace LifeLineApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForgotPasswordController : ControllerBase
    {
        private readonly LifeLinedbContext _dbContext;

        public ForgotPasswordController(LifeLinedbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        [Route("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] User model)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user != null)
            {



                // Send the reset link to the user's email
                SendResetLinkEmail(model.Email);

                return Ok("Reset link sent successfully.");
            }

            return NotFound("User with this email does not exist.");
        }


        [HttpPost]
        [Route("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] User model)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user != null)
            {
                // Update the password
                user.Password = model.Password;


                await _dbContext.SaveChangesAsync();

                return Ok("Password updated successfully.");
            }

            return BadRequest("Invalid email or reset token.");
        }

        private void SendResetLinkEmail(string email)
        {
            string resetLink = $"http://localhost:5000/admin/reset?email={email}";

            MailMessage mm = new MailMessage();
            mm.From = new MailAddress("aliyankhan6446@gmail.com");
            mm.To.Add(new MailAddress(email));

            mm.Subject = "Password Reset Link";

            // Use anchor tag with styling
            mm.Body = $@"<p>Click on the following link to reset your password:</p>
                 <a href=""{resetLink}"" style=""color:white;background-color:blue;padding:14px 30px;border-radius:12px;text-decoration:none;display:inline-block;"">Reset Password</a>";

            mm.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;

            NetworkCredential nc = new NetworkCredential("aliyankhan6446@gmail.com", "rtnr piax mgbn xzyc");

            smtp.UseDefaultCredentials = true;
            smtp.Credentials = nc;
            smtp.UseDefaultCredentials = false;

            smtp.Send(mm);

            ;
        }
    }
}