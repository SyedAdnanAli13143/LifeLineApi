using LifeLineApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace LifeLineApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BloodRL : ControllerBase
    {
        private readonly LifeLinedbContext _dbContext;
        private Dictionary<string, Dictionary<string, double>> qValues = new Dictionary<string, Dictionary<string, double>>();

        public BloodRL(LifeLinedbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Endpoint to predict future blood availability based on historical data
        [HttpGet("predict")]
        public ActionResult<IDictionary<string, Dictionary<string, AvailabilityInfo>>> PredictFutureBloodAvailability()
        {
            try
            {
                // Get historical data for all blood groups and hospitals
                var historicalData = _dbContext.BloodAvailabilities
                    .OrderBy(b => b.BaBloodGroup)
                    .ThenBy(b => b.BaDate)
                    .ToList();

                if (historicalData.Count == 0)
                {
                    return NotFound("No historical blood availability data found");
                }

                // Train Q-learning model based on historical data
                TrainQModel(historicalData);

                // Make predictions for the next one or two days
                var predictions = PredictFutureBloodAvailability(historicalData);

                return Ok(predictions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        private void TrainQModel(List<BloodAvailability> historicalData)
        {
            double learningRate = 0.1; 
            double discountFactor = 0.9; 

            foreach (var bloodGroup in historicalData.Select(b => b.BaBloodGroup).Distinct())
            {
                if (!qValues.ContainsKey(bloodGroup))
                {
                    qValues[bloodGroup] = new Dictionary<string, double>();
                }

                // Update Q-values based on historical data
                var bloodGroupData = historicalData.Where(b => b.BaBloodGroup == bloodGroup).ToList();
                for (int i = 0; i < bloodGroupData.Count - 1; i++)
                {
                    double currentStateReward = bloodGroupData[i].BaBottlesAvailable ?? 0;
                    double nextStateReward = bloodGroupData[i + 1].BaBottlesAvailable ?? 0;

                    // Q-learning update formula
                    if (!qValues[bloodGroup].ContainsKey(bloodGroupData[i].BaHId.ToString()))
                    {
                        qValues[bloodGroup][bloodGroupData[i].BaHId.ToString()] = 0.5; // Initialize Q-value for each hospital
                    }

                    qValues[bloodGroup][bloodGroupData[i].BaHId.ToString()] = qValues[bloodGroup][bloodGroupData[i].BaHId.ToString()] +
                                                                             learningRate * (nextStateReward - qValues[bloodGroup][bloodGroupData[i].BaHId.ToString()]);
                }
            }
        }

        private IDictionary<string, Dictionary<string, AvailabilityInfo>> PredictFutureBloodAvailability(List<BloodAvailability> historicalData)
        {
            var predictions = new Dictionary<string, Dictionary<string, AvailabilityInfo>>();

            foreach (var bloodGroup in historicalData.Select(b => b.BaBloodGroup).Distinct())
            {
                predictions[bloodGroup] = new Dictionary<string, AvailabilityInfo>();

                if (qValues.ContainsKey(bloodGroup))
                {
                    var bloodGroupData = historicalData.Where(b => b.BaBloodGroup == bloodGroup).ToList();

                    foreach (var hospitalId in bloodGroupData.Select(b => b.BaHId).Distinct())
                    {
                        if (!predictions[bloodGroup].ContainsKey(hospitalId.ToString()))
                        {
                            predictions[bloodGroup][hospitalId.ToString()] = new AvailabilityInfo();
                        }

                        predictions[bloodGroup][hospitalId.ToString()].HospitalName = GetHospitalName(hospitalId);

                        if (qValues[bloodGroup].ContainsKey(hospitalId.ToString()))
                        {
                            var lastDataPoint = bloodGroupData.LastOrDefault(b => b.BaHId == hospitalId);

                            if (lastDataPoint != null)
                            {
                                predictions[bloodGroup][hospitalId.ToString()].ActualAvailability = lastDataPoint.BaBottlesAvailable ?? 0;

                                // Calculate the predicted availability based on the specified logic
                                predictions[bloodGroup][hospitalId.ToString()].PredictedAvailability = (lastDataPoint.BaBottlesAvailable >= 10) ? 100 : ((double)lastDataPoint.BaBottlesAvailable / 10 * 100);
                            }
                        }
                    }
                }
            }

            return predictions;
        }

        private string GetHospitalName(int? hospitalId)
        {
            var hospitalDetails = _dbContext.Hospitals.FirstOrDefault(h => h.HId == hospitalId);
            return hospitalDetails != null ? hospitalDetails.HName : "Unknown Hospital";
        }





    }

    public class AvailabilityInfo
    {
        public string HospitalName { get; set; }
        public int ActualAvailability { get; set; }
        public double PredictedAvailability { get; set; }
    }
}