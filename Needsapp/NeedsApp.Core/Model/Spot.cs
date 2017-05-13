using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeedsApp.Core.Model
{
    [PropertyChanged.ImplementPropertyChanged]
    public class Spot: BaseModel
    {

        public string Name { get; set; }
        public Position SpotLocation { get; set; }

    }
}
