using System.ComponentModel.DataAnnotations.Schema;

namespace PltTurbineShared
{
    [Table("Sensors")]
    public record Sensor(int Id, string Value,string CustomId, bool IsOwn=false): IInformationDropDrownComponent;
     
}
