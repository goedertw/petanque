using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petanque.Contracts.Responses
{
    public class SpelerScoresResponseContract
    {
        public int SpelerVolgNr { get; set; }
        public int ScoreA { get; set; }
        public int ScoreB { get; set; }
    }
}
