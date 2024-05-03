using LifeLineApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LifeLineApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class appointment_cr : ControllerBase
    {
        private readonly LifeLinedbContext _dbContext;

        public appointment_cr(LifeLinedbContext dbContext)
        {
            _dbContext = dbContext;
        }
        //Contact Start
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments()
        {
            if (_dbContext.Appointments == null)
            {
                return NotFound();
            }
            var stud = await _dbContext.Appointments.ToListAsync();

            return stud;
        }

        [HttpPost]

        public async Task<ActionResult<Appointment>> PostAppointment(Appointment s)
            {


            _dbContext.Appointments.Add(s);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAppointments), new { id = s.AId }, s);

        }

        [HttpPut]
        public async Task<ActionResult> PutAppointment(int id, Appointment s)
        {
            if (id != s.AId)
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


                if (!HAppointmentAvailable(id))
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

        private bool HAppointmentAvailable(int id)
        {
            return (_dbContext.Appointments?.Any(x => x.AId == id)).GetValueOrDefault();
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteAppointment(int id)
        {
            if (_dbContext.Appointments == null)
            {
                return NotFound();
            }
            var stud = await _dbContext.Appointments.FindAsync(id);

            if (stud == null)
            {
                return NotFound();
            }
            _dbContext.Appointments.Remove(stud);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("appointments")]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments([FromQuery] int hospitalId)
        {
            try
            {
                // Fetch doctors associated with the hospital
                var doctors = await _dbContext.Doctors
                    .Where(d => d.DHId == hospitalId)
                    .ToListAsync();

                // Extract doctor IDs from the list of doctors
                var doctorIds = doctors.Select(d => d.DId).ToList(); // Use DId which is int?

                // Fetch appointments associated with the identified doctors
                var appointments = await _dbContext.Appointments
                    .Where(a => doctorIds.Contains(a.ADId.Value))
                    .Select(a => new Appointment
                    {
                        AId = a.AId,
                        AHId = a.AHId,
                        ADId = a.ADId,
                        APatientDob = a.APatientDob,
                        APatientName = a.APatientName,
                        ADate = a.ADate,
                        ATime = a.ATime,
                        AMobile = a.AMobile,
                        AType = a.AType,
                        AReason = a.AReason,
                        AD = a.AD,
                        
                        
                        
                    })
                    .ToListAsync();

                return Ok(appointments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }




    }
}
