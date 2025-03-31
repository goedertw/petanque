using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petanque.Contracts
{
    class DagKlassementResponseContract
    {
        public int DagklassementId { get; set; }

        public int? SpeeldagId { get; set; }

        public int? SpelerId { get; set; }

        public int Hoofdpunten { get; set; }

        public int PlusMinPunten { get; set; }

        public virtual SpeeldagResponseContract? Speeldag { get; set; }

        public virtual PlayerResponseContract? Speler { get; set; }
    }
}
