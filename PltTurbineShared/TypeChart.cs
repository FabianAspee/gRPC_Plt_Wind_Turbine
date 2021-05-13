using System.ComponentModel.DataAnnotations.Schema;

namespace PltTurbineShared
{
    [Table("TypeCharts")]
    public record TypeChart(int Id, string Value, string CustomId) : IInformationDropDrownComponent;
    
}
