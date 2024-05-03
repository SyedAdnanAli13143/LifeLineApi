using LifeLineApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace LifeLineApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicineReminderController : ControllerBase
    {
        private readonly LifeLinedbContext _dbContext;


        public MedicineReminderController(LifeLinedbContext dbContext)
        {
            _dbContext = dbContext;

        }

        [HttpGet("send-reminders")]
        public async Task<IActionResult> SendReminders()
        {
            try
            {
                var remindersFromDb = _dbContext.DoctorPrescriptions
    .Where(dp =>
        dp.DpStartDate <= DateTime.Now.Date &&
        dp.DpEndDate >= DateTime.Now.Date &&
        dp.DpScheduleTime.HasValue &&
        (dp.SentDate.Value.Date != DateTime.Now.Date)
    )
    .ToList();



                foreach (var reminder in remindersFromDb)
                {
                    // Check if it's time to send a reminder
                    if (IsTimeToSendReminder(reminder.DpScheduleTime, reminder.DpStartDate, reminder.DpEndDate) &&
                        (reminder.SentDate == null || (reminder.SentDate >= reminder.DpStartDate && reminder.SentDate <= reminder.DpEndDate)))
                    {
                        // Get patient's email
                        var patientEmail = GetPatientEmail(reminder.DpPId);

                        // Get disease
                        var disease = GetPrescriptionDisease(reminder.DpId);

                        // Send reminder email with additional information
                        await SendReminderEmail(patientEmail, disease, reminder.DpScheduleTime.Value, reminder.DpStartDate.Value, reminder.DpEndDate.Value);

                        // Update SentDate in the database
                        reminder.SentDate = DateTime.Now;

                        // Update database or log that a reminder has been sent
                        UpdateReminderSentStatus(reminder.DpId);
                    }
                }

                // Save changes to the database
                await _dbContext.SaveChangesAsync();

                return Ok("Reminders sent successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error sending reminders: {ex.Message}");
            }
        }

        private bool IsTimeWithinRange(TimeSpan scheduleTime, DateTime referenceDateTime, int minutes)
        {
            var referenceTime = referenceDateTime.TimeOfDay;
            var lowerBound = scheduleTime.Subtract(new TimeSpan(0, minutes, 0));

            // Check if lowerBound becomes negative after subtraction, adjust accordingly
            if (lowerBound.TotalMinutes < 0)
            {
                lowerBound = new TimeSpan(0, 0, 0);
            }

            var upperBound = scheduleTime;

            return referenceTime >= lowerBound && referenceTime <= upperBound;
        }


        private bool IsTimeToSendReminder(TimeSpan? scheduleTime, DateTime? startDate, DateTime? endDate)
        {
            // Log values for debugging
            Console.WriteLine($"scheduleTime: {scheduleTime}, startDate: {startDate}, endDate: {endDate}");

            // Check if scheduleTime, startDate, and endDate are not null before accessing their values
            return scheduleTime.HasValue && startDate.HasValue && endDate.HasValue &&
                   DateTime.Now.TimeOfDay >= scheduleTime.Value && DateTime.Now.Date >= startDate.Value && DateTime.Now.Date <= endDate.Value;
        }


        private string GetPatientEmail(int? patientId)
        {
            // Check if patientId is not null before querying the database
            if (patientId.HasValue)
            {
                // Example: Assume you have a Patients table with a column named "Email"
                var patient = _dbContext.Patients.FirstOrDefault(p => p.PId == patientId.Value);
                return patient?.PEmail ?? "default@example.com"; // Default email if not found
            }

            return "default@example.com"; // Default email if patientId is null
        }

        private async Task SendReminderEmail(string toEmail, string disease, TimeSpan scheduleTime, DateTime startDate, DateTime endDate)
        {
            // Create and send reminder email with additional information
            MailMessage mm = new MailMessage();
            mm.From = new MailAddress("aliyankhan6446@gmail.com");
            mm.To.Add(new MailAddress(toEmail));

            mm.Subject = "Medication Reminder From LifeLine";
            mm.Body = $"Hi,<br/><br/> It's time to take your medicine for {disease}.<br/><br/>" +
                      $"Schedule Time: {scheduleTime}, Start Date: {startDate}, End Date: {endDate}<br/><br/>" +
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

            await smtp.SendMailAsync(mm);
        }
        private void UpdateReminderSentStatus(int dpId)
        {
            // Implement your logic to update the database or log that a reminder has been sent
            // For simplicity, you might update a column in the Doctor_Prescription table indicating the reminder has been sent
        }

        // Add this method to get the disease from the prescription
        private string GetPrescriptionDisease(int dpId)
        {
            var prescription = _dbContext.DoctorPrescriptions.FirstOrDefault(dp => dp.DpId == dpId);
            return prescription?.DpDisease ?? "Unknown Disease";
        }
    }
}