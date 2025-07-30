using System;
using System.Collections.Generic;

namespace Petanque.Storage;

public partial class Seizoensklassement
{
    public int? SpelerId { get; set; }

    public int? SeizoensId { get; set; }

    public int Hoofdpunten { get; set; }

    public int PlusMinPunten { get; set; }

    public string SpelerNaam { get; set; }

    public string SpelerVoornaam { get; set; }
}
