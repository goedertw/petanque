using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petanque.Contracts.Responses
{
    public class SpeeldagResponseContract
    {
        public int SpeeldagId { get; set; }

        public DateTime Datum { get; set; }

        public virtual SeizoenResponseContract? Seizoenen { get; set; }
        public List<SpelResponseContract> Spel { get; set; }
    }
}
