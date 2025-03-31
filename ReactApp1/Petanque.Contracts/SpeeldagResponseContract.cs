using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petanque.Contracts
{
    class SpeeldagResponseContract
    {
        public int SpeeldagId { get; set; }

        public DateOnly Datum { get; set; }

        public int? SeizoensId { get; set; }

        public virtual ICollection<AanwezigheidResponseContract> Aanwezigheids { get; set; } = new List<AanwezigheidResponseContract>();

        public virtual ICollection<DagKlassementResponseContract> Dagklassements { get; set; } = new List<DagKlassementResponseContract>();

        public virtual SeizoenResponseContract? Seizoens { get; set; }

        public virtual ICollection<SpelResponseContract> Spels { get; set; } = new List<SpelResponseContract>();
    }
}
