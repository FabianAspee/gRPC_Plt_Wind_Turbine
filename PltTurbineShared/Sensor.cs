using System.ComponentModel.DataAnnotations.Schema;

namespace PltTurbineShared
{
    [Table("Sensors")]
    public record Sensor(int Id, string Value): IInformationDropDrownComponent;
     
}
