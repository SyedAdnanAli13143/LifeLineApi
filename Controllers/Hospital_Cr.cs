
using LifeLineApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;

namespace LifeLineApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]


    public class Hospital_Cr : ControllerBase
    {
        private readonly LifeLinedbContext _dbContext;

        public Hospital_Cr(LifeLinedbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //Doctors Start

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Hospital>>> GetHospitals()
        {
            if (_dbContext.Hospitals == null)
            {
                return NotFound();
            }
            var stud = await _dbContext.Hospitals.ToListAsync();

            return stud;
        }

        [HttpGet("{id}")]

        public async Task<ActionResult<Hospital>> GetHospitalById(int id)
        {
            if (_dbContext.Hospitals == null)
            {
                return NotFound();
            }
            var stud = await _dbContext.Hospitals.FindAsync(id);

            if (stud == null)
            {
                return NotFound();
            }
            return stud;
        }

        [HttpPost]

        public async Task<ActionResult<Hospital>> PostHospital(Hospital s)
        {
            var checkemail = _dbContext.Hospitals.Where(x => x.HEmail == s.HEmail).FirstOrDefault();
            if (checkemail == null)
            {
                MailMessage mm = new MailMessage();
            mm.From = new MailAddress("aliyankhan6446@gmail.com");
            mm.To.Add(new MailAddress(s.HEmail));

            Random emailrandomnum = new Random();
            int emailrandomnumber = emailrandomnum.Next(1000, 10000);

            mm.Subject = "Login credentials";
            mm.Body = "Click On the following link to log into LifeLine";
            mm.Body = "Hi," + "<br/><br/>" + "We got a request for your account creation. Please click on the below link to log in to your account:" +
                        "<br/><br/> Password is: " + emailrandomnumber + "<br/><br/>" +
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
            s.HPassword = emailrandomnumber.ToString();
            User t = new User();
            t.Email = s.HEmail;
            t.Password = s.HPassword;

            t.RoleId = 2;
            _dbContext.Users.Add(t);
            _dbContext.Hospitals.Add(s);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetHospitals), new { id = s.HId }, s);
            }
            else
            {
                return StatusCode(403, "Hospital with this Email already exists!");
            }
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> PutHospital(int id, Hospital s)
        {
            if (id != s.HId)
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
                /*
                 A DbUpdateConcurrencyException is thrown by SaveChanges when an optimistic concurrency exception is detected while attempting to save an entity that uses foreign key associations.
                 */

                if (!HospitalAvailable(id))
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

        private bool HospitalAvailable(int id)
        {
            return (_dbContext.Hospitals?.Any(x => x.HId == id)).GetValueOrDefault();
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteHospital(int id)
        {
            if (_dbContext.Hospitals == null)
            {
                return NotFound();
            }
            var stud = await _dbContext.Hospitals.FindAsync(id);

            if (stud == null)
            {
                return NotFound();
            }
            _dbContext.Hospitals.Remove(stud);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }


        
    }
}
