using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petanque.Contracts.Responses
{
    public class SpelverdelingResponseContract
    {
        public int SpelverdelingsId { get; set; }

        public int? SpelId { get; set; }

        public string Team { get; set; } = null!;

        public string SpelerPositie { get; set; } = null!;

        public int SpelerVolgnr { get; set; }

        public PlayerResponseContract Speler { get; set; }

        public SpelResponseContract Spel { get; set; }

    }
}
