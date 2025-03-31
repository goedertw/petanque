using System;
using System.Collections.Generic;

namespace Petanque.Storage;

public partial class Seizoensklassement
{
    public int SeizoensklassementId { get; set; }

    public int? SpelerId { get; set; }

    public int? SeizoensId { get; set; }

    public int Hoofdpunten { get; set; }

    public int PlusMinPunten { get; set; }

    public virtual Seizoen? Seizoens { get; set; }

    public virtual Speler? Speler { get; set; }
}
