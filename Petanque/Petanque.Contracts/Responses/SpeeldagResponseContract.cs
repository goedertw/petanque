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

        public DateOnly Datum { get; set; }

        public virtual SeizoenResponseContract? Seizoenen { get; set; }

        public virtual ICollection<SpelResponseContract> Spellen { get; set; } = new List<SpelResponseContract>();
    }
}
