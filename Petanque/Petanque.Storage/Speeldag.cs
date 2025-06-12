using System;
using System.Collections.Generic;

namespace Petanque.Storage;

public partial class Speeldag
{
    public int SpeeldagId { get; set; }

    public DateTime Datum { get; set; }

    public int? SeizoensId { get; set; }

    public virtual ICollection<Aanwezigheid> Aanwezigheids { get; set; } = new List<Aanwezigheid>();

    public virtual ICollection<Dagklassement> Dagklassements { get; set; } = new List<Dagklassement>();

    public virtual Seizoen? Seizoens { get; set; }

    public virtual ICollection<Spel> Spels { get; set; } = new List<Spel>();
}
