using ClientPltTurbine.Shared.Chart3DComponent.ConfigGeneral;
using ClientPltTurbine.Shared.Chart3DComponent.DrawCalendarChart.Contract;

namespace ClientPltTurbine.Shared.Chart3DComponent.DrawCalendarChart.Implementation
{
    public class CalendarChart : ConfigChart3D, ICalendarChart
    {
        public override string GetNameSetup() => "setupCalendarChart";
    }
}
