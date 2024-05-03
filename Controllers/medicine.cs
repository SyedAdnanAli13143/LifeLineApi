using LifeLineApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LifeLineApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class medicine : ControllerBase
    {
        private readonly LifeLinedbContext _dbContext;

        public medicine(LifeLinedbContext dbContext)
        {
            _dbContext = dbContext;
        }


        [HttpGet("prescription/{DId}")]
        public IActionResult GetPrescription(int DId)
        {
            var doctorPrescriptions = _dbContext.DoctorPrescriptions
                .Where(p => p.DpDId == DId)
                .ToList();

            if (doctorPrescriptions == null || doctorPrescriptions.Count == 0)
            {
                return NotFound();
            }

            return Ok(doctorPrescriptions);
        }

        // GET api/doctorprescription/{DpId}
        [HttpGet("{DpId}")]
        public IActionResult Get(int DpId)
        {
            var doctorPrescription = _dbContext.DoctorPrescriptions.Where(x => x.DpPId == DpId).ToList();


            if (doctorPrescription == null)
            {
                return NotFound();
            }

            return Ok(doctorPrescription);
        }

        // POST api/doctorprescription
        [HttpPost]
        public IActionResult Postpre([FromBody] DoctorPrescription doctorPrescription)
        {
            _dbContext.DoctorPrescriptions.Add(doctorPrescription);
            _dbContext.SaveChanges();

            return CreatedAtAction(nameof(Get), new { DpId = doctorPrescription.DpId }, doctorPrescription);
        }

        // PUT api/doctorprescription/{DpId}
        [HttpPut("{DpId}")]
        public IActionResult Put(int DpId, [FromBody] DoctorPrescription updatedDoctorPrescription)
        {
            var existingDoctorPrescription = _dbContext.DoctorPrescriptions
                .FirstOrDefault(dp => dp.DpId == DpId);

            if (existingDoctorPrescription == null)
            {
                return NotFound();
            }


            existingDoctorPrescription.DpDId = updatedDoctorPrescription.DpDId;
            existingDoctorPrescription.DpPId = updatedDoctorPrescription.DpPId;
            existingDoctorPrescription.DpDate = updatedDoctorPrescription.DpDate;
            existingDoctorPrescription.DpDisease = updatedDoctorPrescription.DpDisease;

            _dbContext.SaveChanges();

            return Ok(existingDoctorPrescription);
        }

        // DELETE api/doctorprescription/{DpId}
        [HttpDelete("{DpId}")]
        public IActionResult Delete(int DpId)
        {
            var doctorPrescription = _dbContext.DoctorPrescriptions
                .FirstOrDefault(dp => dp.DpId == DpId);

            if (doctorPrescription == null)
            {
                return NotFound();
            }

            _dbContext.DoctorPrescriptions.Remove(doctorPrescription);
            _dbContext.SaveChanges();

            return NoContent();
        }


    }
}
