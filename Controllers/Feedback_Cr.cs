using LifeLineApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace LifeLineApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Feedback_Cr : ControllerBase
    {
        private readonly LifeLinedbContext _dbContext;

        public Feedback_Cr(LifeLinedbContext dbContext)
        {
            _dbContext = dbContext;
        }
        //Contact Start
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Feedback>>> GetFeedback()
        {
            if (_dbContext.Feedbacks == null)
            {
                return NotFound();
            }
            var stud = await _dbContext.Feedbacks.ToListAsync();

            return stud;
        }

        [HttpGet("GetReviewDetails")]
        public async Task<ActionResult<IEnumerable<object>>> GetFeedbackWithDetails()
        {
            var feedbacksWithDetails = await _dbContext.Feedbacks
      .Join(
          _dbContext.Doctors,
          feedback => feedback.FDId,
          doctor => doctor.DId,
          (feedback, doctor) => new { Feedback = feedback, Doctor = doctor }
      )
      .Join(
          _dbContext.Patients,
          joinResult => joinResult.Feedback.FPId,
          patient => patient.PId,
          (joinResult, patient) => new { joinResult.Feedback, joinResult.Doctor, Patient = patient }
      )
      .Join(
          _dbContext.Hospitals,
          joinResult => joinResult.Doctor.DHId,
          hospital => hospital.HId,
          (joinResult, hospital) => new { joinResult.Feedback, joinResult.Doctor, joinResult.Patient, Hospital = hospital }
      )
      .GroupBy(joinResult => joinResult.Doctor.DId) 
      .Select(group => new
      {
          name = group.First().Doctor.DName,
          rating = (int)Math.Round(group.Average(joinResult => (decimal)joinResult.Feedback.FRating)),
          comments = string.Join(", ", group.Select(joinResult => joinResult.Feedback.FComments)),
          image = group.First().Doctor.DImage,
          Doctorid = group.Key,
          Hospitalid = group.First().Doctor.DHId,
          HospitalName = group.First().Hospital.HName // Assuming Hospital name property is HName
      })
      .ToListAsync();






            return feedbacksWithDetails;
        }


        [HttpGet("Getreview")]
        public ActionResult<IEnumerable<Feedback>> GetFeedbackByHospitalId(int Hid)
        {
            try
            {
                                    var feedback = _dbContext.Feedbacks
                        .Join(
                            _dbContext.Doctors,
                            feedback => feedback.FDId,
                            doctor => doctor.DId,
                            (feedback, doctor) => new { Feedback = feedback, Doctor = doctor }
                        )
                        .Join(
                            _dbContext.Patients,
                            joinResult => joinResult.Feedback.FPId,
                            patient => patient.PId,
                            (joinResult, patient) => new { joinResult.Feedback, joinResult.Doctor, Patient = patient }
                        )
                        .Where(joinResult => joinResult.Doctor.DHId == Hid)
                        .GroupBy(
                            joinResult => new { joinResult.Doctor.DId, joinResult.Doctor.DName, joinResult.Doctor.DEmail },
                            (key, group) => new
                            {
                                DoctorName = key.DName,
                                DoctorEmail = key.DEmail,
                                PatientName = group.First().Patient.PName,
                                PatientEmail = group.First().Patient.PEmail,
                                Ratings = (int)Math.Round(group.Average(item => item.Feedback.FRating ?? 0)), // Round the average and cast to integer
                                Comments = group.FirstOrDefault() != null ? group.FirstOrDefault().Feedback.FComments : null
                            }
                        )
                        .ToList();

                return Ok(feedback);

            }
            catch (Exception ex)
            {
              
                return StatusCode(500, "Internal server error");
            }
        }



        [HttpPost]

        public async Task<ActionResult<Feedback>> PostAppointment(Feedback s)
        {


            _dbContext.Feedbacks.Add(s);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFeedback), new { id = s.FId }, s);

        }

        [HttpPut]
        public async Task<ActionResult> PutFeedback(int id, Feedback s)
        {
            if (id != s.FId)
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


                if (!HFeedbackAvailable(id))
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

        private bool HFeedbackAvailable(int id)
        {
            return (_dbContext.Feedbacks?.Any(x => x.FId == id)).GetValueOrDefault();
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteFeedback(int id)
        {
            if (_dbContext.Feedbacks == null)
            {
                return NotFound();
            }
            var stud = await _dbContext.Feedbacks.FindAsync(id);

            if (stud == null)
            {
                return NotFound();
            }
            _dbContext.Feedbacks.Remove(stud);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
