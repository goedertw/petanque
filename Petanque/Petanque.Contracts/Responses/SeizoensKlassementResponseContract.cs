﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petanque.Contracts.Responses
{
    public class SeizoensKlassementResponseContract
    {
        public int SeizoensklassementId { get; set; }

        public int? SpelerId { get; set; }

        public int? SeizoensId { get; set; }

        public int Hoofdpunten { get; set; }

        public int PlusMinPunten { get; set; }

        public virtual SeizoenResponseContract? Seizoens { get; set; }

        public virtual PlayerResponseContract? Speler { get; set; }
    }
}
