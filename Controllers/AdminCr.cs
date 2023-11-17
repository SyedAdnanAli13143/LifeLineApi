using LifeLineApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LifeLineApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Admin_Cr : ControllerBase
    {
        private readonly LifeLinedbContext _dbContext;

        public Admin_Cr(LifeLinedbContext dbContext)
        {
            _dbContext = dbContext;
        }
        //Contact Start
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApplicationAdmin>>> GetAdminAvailability()
        {
            if (_dbContext.ApplicationAdmins == null)
            {
                return NotFound();
            }
            var stud = await _dbContext.ApplicationAdmins.ToListAsync();

            return stud;
        }

        [HttpPost]

        public async Task<ActionResult<ApplicationAdmin>> PostAdmin(ApplicationAdmin s)
        {


            _dbContext.ApplicationAdmins.Add(s);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAdminAvailability), new { id = s.AaId }, s);

        }

        [HttpPut]
        public async Task<ActionResult> PutAdminAvailability(int id, ApplicationAdmin s)
        {
            if (id != s.AaId)
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


                if (!HAdminAvailabilityAvailable(id))
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

        private bool HAdminAvailabilityAvailable(int id)
        {
            return (_dbContext.ApplicationAdmins?.Any(x => x.AaId == id)).GetValueOrDefault();
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteAdminAvailability(int id)
        {
            if (_dbContext.ApplicationAdmins == null)
            {
                return NotFound();
            }
            var stud = await _dbContext.ApplicationAdmins.FindAsync(id);

            if (stud == null)
            {
                return NotFound();
            }
            _dbContext.ApplicationAdmins.Remove(stud);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("ALog")]
        public async Task<IActionResult> LogAdminAvailability([FromBody] ApplicationAdmin a)
        {
            try
            {
                if (_dbContext.ApplicationAdmins == null)
                {
                    return NotFound();
                }

                var stud = _dbContext.ApplicationAdmins
                  .Where(x => x.AaUserName == a.AaUserName && x.AaPassword == a.AaPassword)
                  .SingleOrDefault();

                if (stud == null)
                {
                    return NotFound();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing request: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
