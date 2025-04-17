using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    namespace Petanque.Contracts.Requests
    {
        public class SpelverdelingSpellenRequestContract
        {
            public int SpeeldagId { get; set; }
            public List<string> AanwezigeSpelers { get; set; } = new();
            public int AantalRondes { get; set; }
            public int AantalVelden { get; set; }
        }
    }
