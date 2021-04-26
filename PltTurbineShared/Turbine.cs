using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PltTurbineShared
{
    [Table("Turbines")]
    public record Turbine(int Id, string Value): IInformationDropDrownComponent;
     
}
