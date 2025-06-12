using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petanque.Contracts.Requests
{
    public class SpeeldagRequestContract
    {
        public DateTime Datum { get; set; }

        public int? SeizoensId { get; set; }
    }
}
 