using LifeLineApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
