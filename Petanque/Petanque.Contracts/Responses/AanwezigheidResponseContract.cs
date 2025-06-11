using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petanque.Contracts.Responses
{
    public class AanwezigheidResponseContract
    {
        public int AanwezigheidId { get; set; }

        public int? SpeeldagId { get; set; }

        public int? SpelerId { get; set; }

        public int SpelerVolgnr { get; set; }
        public string SpeeldagDatum { get; set; } = string.Empty;
    }
}
