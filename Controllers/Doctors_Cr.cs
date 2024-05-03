
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
        [HttpGet("ByHospitalId/{dhId}")]
        public async Task<ActionResult<IEnumerable<Doctor>>> GetDoctorsByHospitalId(int dhId)
        {
            var doctors = await _dbContext.Doctors
                .Where(doctor => doctor.DHId == dhId)
                .ToListAsync();

            if (doctors == null || doctors.Count == 0)
            {
                return NotFound();
            }

            return doctors;
        }

        [HttpPost]
        public async Task<ActionResult<Doctor>> PostDoctors([FromForm] Doctor s)
        {
            var checkemail = _dbContext.Doctors.Where(x => x.DEmail == s.DEmail).FirstOrDefault();
            if (checkemail == null)
            {
                try
            {
                MailMessage mm = new MailMessage();
                mm.From = new MailAddress("aliyankhan6446@gmail.com");
                mm.To.Add(new MailAddress(s.DEmail));

                Random emailrandomnum = new Random();
                int emailrandomnumber = emailrandomnum.Next(1000, 10000);

                mm.Subject = "Login credentials";
                mm.Body = "Click On the following link to log into LifeLine";
                mm.Body += "Hi," + "<br/><br/>" + "We got a request for your account creation. Please click on the below link to Login an account" +
                "<br/><br/> Password is : " + emailrandomnumber + "<br/><br/>" +
                        "<a href='http://localhost:5000/admin/Login'>Login Link</a>";
                mm.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;

                NetworkCredential nc = new NetworkCredential("aliyankhan6446@gmail.com", "rtnr piax mgbn xzyc");

                smtp.UseDefaultCredentials = false;
                smtp.Credentials = nc;



                if (s.ImageFile != null)
                {
                    string filename = Path.GetFileNameWithoutExtension(s.ImageFile.FileName);
                    string extension = Path.GetExtension(s.ImageFile.FileName);

                    string folder = "images_d";
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + filename + extension;

                    // Ensure the directory exists
                    string serverFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, folder);
                    if (!Directory.Exists(serverFolderPath))
                    {
                        Directory.CreateDirectory(serverFolderPath);
                    }

                    string serverFilePath = Path.Combine(serverFolderPath, uniqueFileName);

                    using (var fileStream = new FileStream(serverFilePath, FileMode.Create))
                    {
                        await s.ImageFile.CopyToAsync(fileStream);
                    }

                    s.DImage = uniqueFileName;
                }
                else
                {
                    // Handle the case where ImageFile is null, perhaps by setting a default image or throwing an error.
                }

                s.DPassword = emailrandomnumber.ToString();
                User t = new User();
                t.Email = s.DEmail;
                t.Password = s.DPassword;

                t.RoleId = 4;

                _dbContext.Users.Add(t);
                _dbContext.Doctors.Add(s);
                await _dbContext.SaveChangesAsync();
                smtp.Send(mm);
                return CreatedAtAction(nameof(GetDoctors), new { id = s.DId }, s);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
            }

            else
            {
                return StatusCode(403, "Doctor  with this Email already exists! ");
            }
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

        [HttpGet("returntime")]
        public async Task<ActionResult<List<DateTime>>> GetAvailableAppointmentSlots([FromQuery] DoctorAppointmentQuery query)
        {
            try
            {
                int doctorId = query.DoctorId;
                DateTime appointmentDate = query.AppointmentDate.Date; 

               
                var doctor = await _dbContext.Doctors.FindAsync(doctorId);

                if (doctor != null)
                {
                    
                    TimeSpan checkinTime = doctor.CheckinTime ?? TimeSpan.Zero;
                    TimeSpan checkoutTime = doctor.CheckoutTime ?? TimeSpan.Zero;
                    TimeSpan averageTimeToSeeOnePatient = doctor.AverageTimeToSeeOnePatient ?? TimeSpan.Zero;

                  
                    var availableTimeSlots = new List<DateTime>();

                    
                    TimeSpan currentTime = checkinTime;
                    while (currentTime.Add(averageTimeToSeeOnePatient) <= checkoutTime)
                    {
                        if (!IsAppointmentTimeTaken(doctorId, appointmentDate, currentTime))
                        {
                        
                            availableTimeSlots.Add(new DateTime(appointmentDate.Year, appointmentDate.Month, appointmentDate.Day, currentTime.Hours, currentTime.Minutes, 0));
                        }

                        currentTime = currentTime.Add(averageTimeToSeeOnePatient);
                    }

                    return Ok(availableTimeSlots);
                }
                else
                {
                    return NotFound("Doctor not found.");
                }
            }
            catch (Exception ex)
            {
              
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }



        bool IsAppointmentTimeTaken(int doctorId, DateTime appointmentDate, TimeSpan appointmentTime)
        {
         
            var existingAppointment = _dbContext.Appointments.Any(a => a.ADId == doctorId && a.ADate == appointmentDate && a.ATime == appointmentTime);

            
            return existingAppointment;
        }


    }
}
