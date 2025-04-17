using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petanque.Contracts.Responses
{
    public class SpelverdelingSpellenResponseContract
    { 
        public class SpelVerdelingRonde
        {
            public int RondeNr { get; set; }
            public List<SpeelveldVerdeling> Speelvelden { get; set; } = new();
        }

        public class SpeelveldVerdeling
        {
            public int SpeelveldNr { get; set; }
            public List<string> Team1 { get; set; } = new();
            public List<string> Team2 { get; set; } = new();
        }
    }
}
