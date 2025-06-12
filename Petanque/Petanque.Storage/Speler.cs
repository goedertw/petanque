using System;
using System.Collections.Generic;

namespace Petanque.Storage;

public partial class Speler
{
    public int SpelerId { get; set; }

    public string Voornaam { get; set; } = null!;

    public string Naam { get; set; } = null!;

    public virtual ICollection<Aanwezigheid> Aanwezigheids { get; set; } = new List<Aanwezigheid>();

    public virtual ICollection<Dagklassement> Dagklassements { get; set; } = new List<Dagklassement>();

    public virtual ICollection<Seizoensklassement> Seizoensklassements { get; set; } = new List<Seizoensklassement>();

    public virtual ICollection<Spelverdeling> Spelverdelings { get; set; } = new List<Spelverdeling>();
}
