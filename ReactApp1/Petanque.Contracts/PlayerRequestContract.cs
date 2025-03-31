using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petanque.Contracts
{
    class PlayerRequestContract
    {
        public string Voornaam { get; set; } = null!;

        public string Naam { get; set; } = null!;

        public virtual ICollection<AanwezigheidResponseContract> Aanwezigheids { get; set; } = new List<AanwezigheidResponseContract>();

        public virtual ICollection<DagKlassementResponseContract> Dagklassements { get; set; } = new List<DagKlassementResponseContract>();

        public virtual ICollection<SeizoensKlassementResponseContract> Seizoensklassements { get; set; } = new List<SeizoensKlassementResponseContract>();
    }
}
