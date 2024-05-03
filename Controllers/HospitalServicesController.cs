using LifeLineApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LifeLineApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HospitalServicesController : ControllerBase
    {
        private readonly LifeLinedbContext _dbContext;

        public HospitalServicesController(LifeLinedbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("GetServicesByHospitalId/{hospitalId}")]
        public async Task<ActionResult<IEnumerable<HospitalService>>> GetServicesByHospitalId(int hospitalId)
        {
            var services = await _dbContext.HospitalServices
                .Where(hs => hs.HsHId == hospitalId)
                .ToListAsync();

            if (services == null || services.Count == 0)
            {
                return NotFound($"No services found for the Hospital ID: {hospitalId}");
            }

            return Ok(services);
        }

        // GET: api/HospitalServices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HospitalService>>> GetHospitalServices()
        {
            return await _dbContext.HospitalServices.ToListAsync();
        }

        // GET: api/HospitalServices/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HospitalService>> GetHospitalService(int id)
        {
            var hospitalService = await _dbContext.HospitalServices.FindAsync(id);

            if (hospitalService == null)
            {
                return NotFound();
            }

            return hospitalService;
        }

        // POST: api/HospitalServices
        [HttpPost]
        public async Task<ActionResult<HospitalService>> PostHospitalService(HospitalService hospitalService)
        {
            _dbContext.HospitalServices.Add(hospitalService);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction("GetHospitalService", new { id = hospitalService.HsId }, hospitalService);
        }

        // PUT: api/HospitalServices/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHospitalService(int id, HospitalService hospitalService)
        {
            if (id != hospitalService.HsId)
            {
                return BadRequest();
            }

            _dbContext.Entry(hospitalService).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HospitalServiceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/HospitalServices/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHospitalService(int id)
        {
            var hospitalService = await _dbContext.HospitalServices.FindAsync(id);
            if (hospitalService == null)
            {
                return NotFound();
            }

            _dbContext.HospitalServices.Remove(hospitalService);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        private bool HospitalServiceExists(int id)
        {
            return _dbContext.HospitalServices.Any(e => e.HsId == id);
        }


        [HttpPost("Search")]
        public ActionResult<IEnumerable<Hospital>> SearchNearestHospitals([FromBody] SearchRequestDto searchRequest)
        {
            // Validate searchRequest data
            if (string.IsNullOrEmpty(searchRequest.Service))
            {
                return BadRequest("Service is required.");
            }

            var userLocation = new GeoCoordinate(searchRequest.UserLatitude, searchRequest.UserLongitude);

            var hospitals = _dbContext.HospitalServices
                .Include(hs => hs.HsH)
                .Where(hs => hs.HsServices == searchRequest.Service)
                .Select(hs => new Hospital
                {
                    HId = hs.HsH.HId,
                    HName = hs.HsH.HName,
                    HAddress = hs.HsH.HAddress,
                    HlLatitude = hs.HsH.HlLatitude,
                    HlLongitude = hs.HsH.HlLongitude,
                })
                .AsEnumerable() // Switch to in-memory processing
                .Select(h => new Hospital
                {
                    HId = h.HId,
                    HName = h.HName,
                    HAddress = h.HAddress,
                    HlLatitude = h.HlLatitude,
                    HlLongitude = h.HlLongitude,
                    DistanceInKm = GeoCoordinate.GetDistance(userLocation, new GeoCoordinate(h.HlLatitude, h.HlLongitude))
                })
                .OrderBy(dto => dto.DistanceInKm)
                .Take(3) // Take the top 3 hospitals with the lowest distance
                .ToList();

            return hospitals;
        }



    }

    // SearchRequestDto.cs
    public class SearchRequestDto
    {
        public float UserLatitude { get; set; }
        public float UserLongitude { get; set; }
        public string Service { get; set; }
    }

    // GeoCoordinate.cs
    public class GeoCoordinate
    {
        public double Latitude { get; }
        public double Longitude { get; }

        public GeoCoordinate(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public static double GetDistance(GeoCoordinate coord1, GeoCoordinate coord2)
        {
            // Use the Haversine formula to calculate distance
            const double earthRadius = 6371; // Earth radius in kilometers

            var dLat = DegreesToRadians(coord2.Latitude - coord1.Latitude);
            var dLon = DegreesToRadians(coord2.Longitude - coord1.Longitude);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(DegreesToRadians(coord1.Latitude)) * Math.Cos(DegreesToRadians(coord2.Latitude)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return earthRadius * c;
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }

    }

    


}
