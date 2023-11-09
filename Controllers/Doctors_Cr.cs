
using LifeLineApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Hosting;


namespace LifeLineApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

   
    public class Doctors_Cr : ControllerBase
    {
        private readonly LifeLinedbContext _dbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public Doctors_Cr(LifeLinedbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            _dbContext = dbContext;
            _webHostEnvironment = webHostEnvironment;
        }

        //Doctors Start

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Doctor>>> GetDoctors()
        {
            if (_dbContext.Doctors == null)
            {
                return NotFound();
            }
            var stud = await _dbContext.Doctors.ToListAsync();

            return stud;
        }

        [HttpGet("{id}")]

        public async Task<ActionResult<Doctor>> GetDoctorsById(int id)
        {
            if (_dbContext.Doctors == null)
            {
                return NotFound();
            }
            var stud = await _dbContext.Doctors.FindAsync(id);

            if (stud == null)
            {
                return NotFound();
            }
            return stud;
        }

        [HttpPost]

        public async Task<ActionResult<Doctor>> PostDoctors([FromForm]  Doctor s)
        {
            MailMessage mm = new MailMessage();
            mm.From = new MailAddress("aliyankhan6446@gmail.com");
            mm.To.Add(new MailAddress(s.DEmail));

            Random emailrandomnum = new Random();
            int emailrandomnumber = emailrandomnum.Next(1000, 10000);


            mm.Subject = "Login credentials";
            mm.Body = "Click On the following link to log into LifeLine";
            mm.Body = "Hi," + "<br/><br/>" + "We got request for  your account creation. Please click on the below link to Login an account" +
                "<br/><br/> Password  is : " + emailrandomnumber + "<br/><br/>";
            mm.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;

            NetworkCredential nc = new NetworkCredential("aliyankhan6446@gmail.com", "rtnr piax mgbn xzyc");

            smtp.UseDefaultCredentials = true;
            smtp.Credentials = nc;
            smtp.UseDefaultCredentials = false;



           

            string filename = Path.GetFileNameWithoutExtension(s.ImageFile.FileName);
            string extension = Path.GetExtension(s.ImageFile.FileName);

            string folder = "images_d";
            folder += Guid.NewGuid().ToString() + "_" + s.ImageFile.FileName;
            s.DImage = folder;
           
            string serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, folder);

            s.ImageFile.CopyToAsync(new FileStream(serverFolder, FileMode.Create));


            s.DPassword = emailrandomnumber.ToString();           
            _dbContext.Doctors.Add(s);
            await _dbContext.SaveChangesAsync();
            smtp.Send(mm);
            return CreatedAtAction(nameof(GetDoctors), new { id = s.DId }, s);

        }


        [HttpPut]
        public async Task<ActionResult> PutDoctor(int id, Doctor s)
        {
            if (id != s.DId)
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

                if (!DoctorAvailable(id))
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

        private bool DoctorAvailable(int id)
        {
            return (_dbContext.Doctors?.Any(x => x.DId == id)).GetValueOrDefault();
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteDoctor(int id)
        {
            if (_dbContext.Doctors == null)
            {
                return NotFound();
            }
            var stud = await _dbContext.Doctors.FindAsync(id);

            if (stud == null)
            {
                return NotFound();
            }
            _dbContext.Doctors.Remove(stud);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        //Doctors End

        
    }
}
