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
    public class EmergencyContact_cr : ControllerBase
    {
        private readonly LifeLinedbContext _dbContext;

        public EmergencyContact_cr(LifeLinedbContext dbContext)
        {
            _dbContext = dbContext;
        }
        //Contact Start
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmergencyContact>>> GetContact()
        {
            if (_dbContext.EmergencyContacts == null)
            {
                return NotFound();
            }
            var stud = await _dbContext.EmergencyContacts.ToListAsync();

            return stud;
        }

        [HttpPost]

        public async Task<ActionResult<EmergencyContact>> PostContact(EmergencyContact s)
        {
            

            _dbContext.EmergencyContacts.Add(s);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetContact), new { id = s.EcHId }, s);

        }

        [HttpPut]
        public async Task<ActionResult> PutContact(int id, EmergencyContact s)
        {
            if (id != s.EcHId)
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


                if (!HContactAvailable(id))
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

        private bool HContactAvailable(int id)
        {
            return (_dbContext.EmergencyContacts?.Any(x => x.EcHId == id)).GetValueOrDefault();
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteContact(int id)
        {
            if (_dbContext.EmergencyContacts == null)
            {
                return NotFound();
            }
            var stud = await _dbContext.EmergencyContacts.FindAsync(id);

            if (stud == null)
            {
                return NotFound();
            }
            _dbContext.EmergencyContacts.Remove(stud);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
