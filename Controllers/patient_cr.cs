using LifeLineApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks.Dataflow;
using System.Web;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.TwiML.Messaging;
using Twilio.TwiML.Voice;
using Twilio.Types;


namespace LifeLineApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class patient_cr : ControllerBase
    {
        private readonly LifeLinedbContext _dbContext;
        private readonly TwilioSmsService _smsService;

        public patient_cr(LifeLinedbContext dbContext, TwilioSmsService smsService)
        {
            _dbContext = dbContext;
            _smsService = smsService;
        }
        //Contact Start
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Patient>>> Getpatients()
        {
            if (_dbContext.Patients == null)
            {
                return NotFound();
            }
            var stud = await _dbContext.Patients.ToListAsync();

            return stud;
        }

        [HttpPost("Realtimewaitingupdate")]
        public IActionResult Realtimewaitingupdate()
        {
            var currentDate = DateTime.Now.Date;
            var email = _dbContext.Realtimewaitingtbs.Where(x => x.PDate == currentDate && x.Status == "enquee").ToList();


            foreach (var job in email)
            {
                try
                {
                    MailMessage mm = new MailMessage();
                    mm.From = new MailAddress("aliyankhan6446@gmail.com");
                    mm.To.Add(new MailAddress(job.PEmail));
                    mm.Subject = "Appointment Reminder";
                    string dateString = job.PDate?.ToString("yyyy-MM-dd");



                    string timeString = job.PTime?.ToString();


                    string timeAmPm = DateTime.ParseExact(timeString, "HH:mm:ss", CultureInfo.InvariantCulture).ToString("hh:mm tt");


                    string body = @"
            <html>
            <head>
                <style>
                    /* Define styles for the email body */
                    body {
                        font-family: Arial, sans-serif;
                        background-color: #f2f2f2;
                        color: #333333;
                    }
                    .container {
                        margin: 20px auto;
                        padding: 20px;
                        background-color: #ffffff;
                        border-radius: 10px;
                        box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.1);
                    }
                    .content {
                        text-align: center;
                    }
                    .button {
                        display: inline-block;
                        padding: 10px 20px;
                        background-color: #007bff;
                        color: white;
                        text-decoration: none;
                        border-radius: 5px;
                    }
                    .highlight {
                        font-weight: bold;
                        color: #ff5733; /* orange color */
                    }
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='content'>
                        <h2>Appointment Reminder</h2>
                        <p>
                            Hi,<br/><br/>
                            We wanted to remind you of your appointment with LifeLine scheduled for <span class='highlight'>" + dateString + @"</span> at <span class='highlight'>" + timeAmPm + @"</span>.<br/><br/>
                        </p>
                        <p>
                            Please either visit our hospital or log in to our website to proceed with your appointment.
                        </p>
                        <p>
                            We wish you a pleasant and healthy visit!
                        </p>
                        <a href='http://localhost:3000/' class='button'>Login to LifeLine</a>
                    </div>
                </div>
            </body>
            </html>
        ";

                    mm.Body = body;
                    mm.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    NetworkCredential nc = new NetworkCredential("aliyankhan6446@gmail.com", "rtnr piax mgbn xzyc");
                    smtp.Credentials = nc;
                    try
                    {
                        smtp.Send(mm);
                        job.Status = "sent";
                        _dbContext.Update(job);
                        _dbContext.SaveChanges();


                        //work sms

                        try
                        {
                            string sms = "Your Appointment at " + timeAmPm;
                            string n = "+92" + job.PNumber;
                            _smsService.SendSmsAsync(n, sms);
                            return Ok("SMS Sent!");
                        }
                        catch (Exception ex)
                        {
                            // Log the exception or handle it appropriately
                            return StatusCode(500, $"Failed to send SMS: {ex.Message}");
                        }


                        //end sms
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error sending email: " + ex.Message);
                        job.Status = "failed";
                    }

                    _dbContext.Update(job);
                }
                catch (Exception ex)
                {
                    return BadRequest($"Error processing job : {ex.Message}");
                }
            }


            _dbContext.SaveChanges();

            return Ok("Jobs processed successfully");

        }
        [HttpPost]

        public async Task<ActionResult<Patient>> PostPatient(Patient s)
        {
            var hospital = _dbContext.Doctors
.Where(x => x.DId == s.PDId)
.Select(x => x.DH)
.FirstOrDefault();

            var existingPatient = _dbContext.Patients
                .Where(x => x.PEmail == s.PEmail && x.PD.DH.HId == hospital.HId)
                .FirstOrDefault();

            if (existingPatient == null)
            {
                User t = new User();
                t.Email = s.PEmail;
                t.Password = s.PPassword;

                t.RoleId = 5;

                _dbContext.Users.Add(t);


                MailMessage mm = new MailMessage();
                mm.From = new MailAddress("aliyankhan6446@gmail.com");
                mm.To.Add(new MailAddress(s.PEmail));

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

                smtp.Send(mm);
                s.PPassword = emailrandomnumber.ToString();



                _dbContext.Patients.Add(s);

                _dbContext.SaveChanges();

                await _dbContext.SaveChangesAsync();

                return CreatedAtAction(nameof(Getpatients), new { id = s.PId }, s);
            }
            else
            {
                return StatusCode(403, "Patient with this Email already exists! ");
            }
        }

        [HttpPut]
        public async Task<ActionResult> PutPatient(int id, Patient s)
        {
            try
            {
                var existingPatient = await _dbContext.Patients.FindAsync(id);
                if (existingPatient == null)
                {
                    return BadRequest();
                }


                existingPatient.PName = s.PName;
                existingPatient.PDob = s.PDob;
                existingPatient.PMobile = s.PMobile;
                existingPatient.PDate = s.PDate;
                existingPatient.PTime = s.PTime;
                existingPatient.PAStatus = s.PAStatus;
                existingPatient.PReason = s.PReason;

                if (s.PEmail != null)
                {
                    return BadRequest("Email field cannot be changed");
                }

                await _dbContext.SaveChangesAsync();


                var user = _dbContext.Users.FirstOrDefault(x => x.Email == s.PEmail);
                if (user != null)
                {
                    user.Password = s.PPassword;
                    await _dbContext.SaveChangesAsync();
                }

                return Ok("Updated");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HPatientAvailable(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }


        private bool HPatientAvailable(int id)
        {
            return (_dbContext.Patients?.Any(x => x.PId == id)).GetValueOrDefault();
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeletePatient(int id)
        {
            if (_dbContext.Patients == null)
            {
                return NotFound();
            }
            var stud = await _dbContext.Patients.FindAsync(id);

            if (stud == null)
            {
                return NotFound();
            }
            _dbContext.Patients.Remove(stud);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
        [HttpGet("patients")]
        public async Task<ActionResult<IEnumerable<Patient>>> GetPatients([FromQuery] int hospitalId)
        {
            try
            {
                // Fetch patients and include associated doctor and hospital information
                var patients = await _dbContext.Patients
                    .Where(p => p.PD != null && p.PD.DHId == hospitalId)
                    .Include(p => p.PD) // Include the doctor information
                        .ThenInclude(d => d.DH) // Include the hospital information for the doctor
                    .Select(p => new Patient
                    {
                        PId = p.PId,
                        PName = p.PName,
                        PDob = p.PDob,
                        PDate = p.PDate,
                        PMobile = p.PMobile,
                        PTime = p.PTime,
                        PReason = p.PReason,
                        PEmail = p.PEmail,
                        PDId = p.PDId,
                        PD = new Doctor
                        {
                            DId = p.PD.DId,
                            DName = p.PD.DName,
                            DField = p.PD.DField,
                            DHId = p.PD.DHId, // Assuming this property exists in your Doctor entity
                            DH = new Hospital
                            {
                                // Include the properties you want from the Hospital entity
                                HId = p.PD.DH.HId,
                                HName = p.PD.DH.HName,
                                HAddress = p.PD.DH.HAddress,
                                // Include other properties as needed
                            }
                        }
                    })
                    .ToListAsync();

                return Ok(patients);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
        [HttpGet("Docpre")]

        public ActionResult GetPrescriptionById(int id)
        {
            if (_dbContext.DoctorPrescriptions == null)
            {
                return NotFound();
            }
            var stud = _dbContext.DoctorPrescriptions.Where(x => x.DpPId == id).SingleOrDefault();

            if (stud == null)
            {
                return NotFound();
            }
            return Ok(stud);
        }

        [HttpPost("Accept(\"{id}\")")]
        public IActionResult AcceptAppointment(Patient p, int id)
        {
            try
            {
                var appointment = _dbContext.Appointments.Find(id);

                // Retrieve hospital information from the doctor associated with the appointment
                var hospital = _dbContext.Doctors
                    .Where(x => x.DId == appointment.ADId)
                    .Select(x => x.DH)
                    .FirstOrDefault();



                // Check if a patient with the same email exists in the same hospital
                var existingPatient = _dbContext.Patients
                    .Where(x => x.PEmail == appointment.AEmail && x.PD.DH.HId == hospital.HId)
                    .FirstOrDefault();

                if (existingPatient != null)
                {


                    // Patient with the same email exists in the same hospital
                    // Return the existing patient's data with a message
                    return StatusCode(403, new { Message = "Patient with this Email already exists in the same hospital.", ExistingPatient = existingPatient });
                }



                p.PName = appointment.APatientName;
                p.PDId = appointment.ADId;
                p.PDob = appointment.APatientDob;
                p.PMobile = appointment.AMobile;
                p.PDate = appointment.ADate;
                p.PTime = appointment.ATime;
               
                if (appointment.AType == "online")
                {
                    p.PAStatus = "Accepted online";
                }
                else
                {
                    p.PAStatus = "Accepted";
                }
                p.PReason = appointment.AReason;
                p.PEmail = appointment.AEmail;                
                MailMessage mm = new MailMessage();
                mm.From = new MailAddress("aliyankhan6446@gmail.com");
                mm.To.Add(new MailAddress(p.PEmail));

                Random emailrandomnum = new Random();
                int emailrandomnumber = emailrandomnum.Next(1000, 10000);

                mm.Subject = "Appointment Succesfull - LifeLine";
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
                p.PPassword = emailrandomnumber.ToString();

                User t = new User();
                t.Email = p.PEmail;
                t.Password = p.PPassword;

                t.RoleId = 5;

                _dbContext.Users.Add(t);



                _dbContext.Patients.Add(p);
                _dbContext.Appointments.Remove(appointment);
                _dbContext.SaveChanges();

                return Ok("Appointment accepted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error accepting appointment: {ex.Message}");
            }
        }

        [HttpGet("doctorpatient")]
        public async Task<ActionResult<IEnumerable<Patient>>> GetAcceptedPatients([FromQuery] int pdId)
        {
            try
            {

                DateTime today = DateTime.Today;

                var patients = await _dbContext.Patients
                    .Where(p => p.PD != null &&
                           (p.PAStatus != "Reject" || p.PAStatus.Contains("online")) && p.PDate >= today  &&
                           p.PDId == pdId &&
                           !p.PD.DoctorPrescriptions.Any(dp => dp.DpDate == today)) // Exclude records with doctor prescriptions for the same doctor on today's date
                    .Include(p => p.PD) // Include the doctor information
                        .ThenInclude(d => d.DH) // Include the hospital information for the doctor
                    .Select(p => new Patient
                    {
                        PId = p.PId,
                        PName = p.PName,
                        PDob = p.PDob,
                        PDate = p.PDate,
                        PMobile = p.PMobile,
                        PTime = p.PTime,
                        PReason = p.PReason,
                        PEmail = p.PEmail,
                        PDId = p.PDId,
                        PAStatus = p.PAStatus,
                        PD = new Doctor
                        {
                            DId = p.PD.DId,
                            DName = p.PD.DName,
                            DField = p.PD.DField,
                            DHId = p.PD.DHId, // Assuming this property exists in your Doctor entity
                            DH = new Hospital
                            {
                                // Include the properties you want from the Hospital entity
                                HId = p.PD.DH.HId,
                                HName = p.PD.DH.HName,
                                HAddress = p.PD.DH.HAddress,
                                // Include other properties as needed
                            }
                        }
                    })
                    .ToListAsync();

                return Ok(patients);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }





        //send video email


        [HttpPost("Sendvideoemail")]
        public IActionResult Sendvideolinkemail(sendemail em)
        {
            try
            {
               



                
                MailMessage mm = new MailMessage();
                mm.From = new MailAddress("aliyankhan6446@gmail.com");
                mm.To.Add(new MailAddress(em.email));



                mm.Subject = "Video Consultation Meeting Link - LifeLine";
                mm.Body = "Hi,<br/><br/>" +
                    "Click on the following link to join the video consultation from LifeLine:<br/><br/>" +
                    "<a href='" + em.link + "'> JoinBlock Meeting </a><br/><br/>" +
                    "Doctor's Email: <br/><br/>" + em.doctor;

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
                
               

                return Ok("Sent link successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error sending link: {ex.Message}");
            }
        }


    }
}

