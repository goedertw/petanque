using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petanque.Contracts.Requests
{
    public class SpelRequestContract
    {
        public int? SpeeldagId { get; set; }

        public string Terrein { get; set; } = null!;

        public int SpelerVolgnr { get; set; }

        public int ScoreA { get; set; }

        public int ScoreB { get; set; }

    }
}
