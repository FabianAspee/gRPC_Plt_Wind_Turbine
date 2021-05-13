using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PltTurbineShared
{
    [Table("Errors")]
    public record ErrorTurbine(int Id, string Value, string CustomId) : IInformationDropDrownComponent;
    
}
