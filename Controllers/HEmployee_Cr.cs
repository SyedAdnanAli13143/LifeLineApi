using LifeLineApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;

namespace LifeLineApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HEmployee_Cr : ControllerBase
    {


        private readonly LifeLinedbContext _dbContext;

        public HEmployee_Cr(LifeLinedbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //Employee Start
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HEmployee>>> GetEmployee()
        {
            if (_dbContext.HEmployees == null)
            {
                return NotFound();
            }
            var stud = await _dbContext.HEmployees.ToListAsync();

            return stud;
        }

        [HttpGet("{id}")]

        public async Task<ActionResult<HEmployee>> GetEmployeeById(int id)
        {
            if (_dbContext.HEmployees == null)
            {
                return NotFound();
            }
            var stud = await _dbContext.HEmployees.FindAsync(id);

            if (stud == null)
            {
                return NotFound();
            }
            return stud;
        }

        [HttpPost]

        public async Task<ActionResult<Doctor>> PostEmployee(HEmployee s)
        {
            var checkemail = _dbContext.HEmployees.Where(x => x.HeEmail == s.HeEmail).FirstOrDefault();
            if (checkemail == null)
            {
                MailMessage mm = new MailMessage();
            mm.From = new MailAddress("aliyankhan6446@gmail.com");
            mm.To.Add(new MailAddress(s.HeEmail));

            Random emailrandomnum = new Random();
            int emailrandomnumber = emailrandomnum.Next(1000, 10000);


            mm.Subject = "Login credentials";
            mm.Body = "Click On the following link to log into LifeLine";
            mm.Body = "Hi," + "<br/><br/>" + "We got request for  your account creation. Please click on the below link to Login an account" +
                "<br/><br/> Password  is : " + emailrandomnumber + "<br/><br/>" +
                        "<a href='http://localhost:5000/admin/Login'>Login Link</a>";
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
            s.HePassword = emailrandomnumber.ToString();
            User t = new User();
            t.Email = s.HeEmail;
            t.Password = s.HePassword;

            t.RoleId = 3;
            _dbContext.Users.Add(t);
            _dbContext.HEmployees.Add(s);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEmployee), new { id = s.HeId }, s);
            }
            else
            {
                return StatusCode(403, "Employee with this Email already exists! ");
            }
        }


        [HttpPut]
        public async Task<ActionResult> PutEmployee(int id, HEmployee s)
        {
            if (id != s.HeId)
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
               

                if (!HEmployeeAvailable(id))
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

        private bool HEmployeeAvailable(int id)
        {
            return (_dbContext.HEmployees?.Any(x => x.HeId == id)).GetValueOrDefault();
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteEmployee(int id)
        {
            if (_dbContext.HEmployees == null)
            {
                return NotFound();
            }
            var stud = await _dbContext.HEmployees.FindAsync(id);

            if (stud == null)
            {
                return NotFound();
            }
            _dbContext.HEmployees.Remove(stud);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

    }
}
