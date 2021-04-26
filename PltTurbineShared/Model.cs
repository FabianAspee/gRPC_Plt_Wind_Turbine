using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PltTurbineShared
{
    public record Model(int Id, string Value):IInformationDropDrownComponent;
}
