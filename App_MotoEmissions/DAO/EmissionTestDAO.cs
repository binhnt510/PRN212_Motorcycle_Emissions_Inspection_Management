using System;
using System.Collections.Generic;
using System.Linq;
using App_MotoEmissions.Models;

namespace App_MotoEmissions.DAO
{
    class EmissionTestDAO
    {
        public static List<EmissionReport> GetEmissionStatistics(DateTime startDate, DateTime endDate, string area = null)
        {
            using (var db = new PVehicleContext())
            {
                var query = from test in db.EmissionTests
                            join inspection in db.Inspections on test.InspectionId equals inspection.InspectionId
                            join center in db.InspectionCenters on inspection.CenterId equals center.CenterId
                            where test.TestDate >= startDate && test.TestDate <= endDate
                            select new
                            {
                                center.Name,
                                center.Address,
                                test.Result
                            };

                if (!string.IsNullOrEmpty(area))
                {
                    query = query.Where(q => q.Address == area);
                }

                var result = query.GroupBy(q => new { q.Name, q.Address })
                                  .Select(g => new EmissionReport
                                  {
                                      InspectionCenter = g.Key.Name,
                                      Address = g.Key.Address,
                                      Passed = g.Count(q => q.Result == "Pass"),
                                      Failed = g.Count(q => q.Result == "Fail")
                                  })
                                  .ToList();

                return result;
            }
        }

        public static List<string> GetAllAreas()
        {
            using (var db = new PVehicleContext())
            {
                return db.InspectionCenters.Select(c => c.Address).Distinct().ToList();
            }
        }
    }

    public class EmissionReport
    {
        public string InspectionCenter { get; set; }
        public string Address { get; set; }
        public int Passed { get; set; }
        public int Failed { get; set; }
    }
}
