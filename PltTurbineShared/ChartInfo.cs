using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PltTurbineShared
{
    [Table("ChartInfos")]
    public record ChartInfo(int Id, string Value) : IInformationDropDrownComponent;
    
}
