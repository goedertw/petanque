﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petanque.Contracts.Requests
{
    public class SeizoensKlassementRequestContract
    {
        public int? SpelerId { get; set; }

        public int? SeizoensId { get; set; }

        public int Hoofdpunten { get; set; }

        public int PlusMinPunten { get; set; }

        
    }
}
