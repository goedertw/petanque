using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petanque.Contracts
{
    class AanwezigheidRequestContract
    {
        public int? SpeeldagId { get; set; }

        public int? SpelerId { get; set; }

        public int SpelerVolgnr { get; set; }

        public virtual SpeeldagResponseContract? Speeldag { get; set; }

        public virtual PlayerResponseContract? Speler { get; set; }
    }
}
