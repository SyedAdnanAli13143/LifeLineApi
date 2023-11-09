﻿using LifeLineApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LifeLineApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BloodAvailability_Cr : ControllerBase
    {
        private readonly LifeLinedbContext _dbContext;

        public BloodAvailability_Cr(LifeLinedbContext dbContext)
        {
            _dbContext = dbContext;
        }
        //Contact Start
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BloodAvailability>>> GetBloodAvailability()
        {
            if (_dbContext.BloodAvailabilities == null)
            {
                return NotFound();
            }
            var stud = await _dbContext.BloodAvailabilities.ToListAsync();

            return stud;
        }

        [HttpPost]

        public async Task<ActionResult<BloodAvailability>> PostAppointment(BloodAvailability s)
        {


            _dbContext.BloodAvailabilities.Add(s);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBloodAvailability), new { id = s.BaId }, s);

        }

        [HttpPut]
        public async Task<ActionResult> PutBloodAvailability(int id, BloodAvailability s)
        {
            if (id != s.BaId)
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


                if (!HBloodAvailabilityAvailable(id))
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

        private bool HBloodAvailabilityAvailable(int id)
        {
            return (_dbContext.BloodAvailabilities?.Any(x => x.BaId == id)).GetValueOrDefault();
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteBloodAvailability(int id)
        {
            if (_dbContext.BloodAvailabilities == null)
            {
                return NotFound();
            }
            var stud = await _dbContext.BloodAvailabilities.FindAsync(id);

            if (stud == null)
            {
                return NotFound();
            }
            _dbContext.BloodAvailabilities.Remove(stud);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
