using System;
using System.Collections.Generic;

namespace Petanque.Storage;

public partial class Dagklassement
{
    public int DagklassementId { get; set; }

    public int? SpeeldagId { get; set; }

    public int? SpelerId { get; set; }

    public int Hoofdpunten { get; set; }

    public int PlusMinPunten { get; set; }

    public virtual Speeldag? Speeldag { get; set; }

    public virtual Speler? Speler { get; set; }
}
