using Petanque.Contracts.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petanque.Contracts.Requests
{
    public class SpelverdelingRequestContract
    {
        public int? SpelId { get; set; }

        public string Team { get; set; } = null!;

        public string SpelerPositie { get; set; } = null!;

        public int SpelerVolgnr { get; set; }
    }
}
