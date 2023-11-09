using LifeLineApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LifeLineApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class patient_cr : ControllerBase { 
    private readonly LifeLinedbContext _dbContext;

    public patient_cr(LifeLinedbContext dbContext)
    {
        _dbContext = dbContext;
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

    [HttpPost]

    public async Task<ActionResult<Patient>> PostPatient(Patient s)
    {


        _dbContext.Patients.Add(s);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(Getpatients), new { id = s.PId }, s);

    }

    [HttpPut]
    public async Task<ActionResult> PutPatient(int id, Patient s)
    {
        if (id != s.PId)
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


            if (!HPatientAvailable(id))
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
}
}

