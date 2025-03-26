using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static InspectionViewModel;


public class InspectionDetailData : InspectionData
{
    public double CO { get; set; }
    public double HC { get; set; }
    public double NOx { get; set; }
    public string TestResult { get; set; }
    public string Notes { get; set; }
}
