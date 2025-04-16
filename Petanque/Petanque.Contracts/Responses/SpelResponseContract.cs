using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petanque.Contracts.Responses
{
    public class SpelResponseContract
    {
        public int SpelId { get; set; }

        public int? SpeeldagId { get; set; }

        public string Terrein { get; set; } = null!;

        public virtual ICollection<SpelerScoresResponseContract> SpelerScores { get; set; } = new List<SpelerScoresResponseContract>();

    }
}
