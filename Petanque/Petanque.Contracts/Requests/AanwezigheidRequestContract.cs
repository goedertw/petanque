﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petanque.Contracts.Requests
{
    public class AanwezigheidRequestContract
    {
        public int? SpeeldagId { get; set; }

        public int? SpelerId { get; set; }

        public int SpelerVolgnr { get; set; }
    }
}
