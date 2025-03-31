using System;
using System.Collections.Generic;

namespace Petanque.Storage;

public partial class Seizoen
{
    public int SeizoensId { get; set; }

    public DateOnly Startdatum { get; set; }

    public DateOnly Einddatum { get; set; }

    public virtual ICollection<Seizoensklassement> Seizoensklassements { get; set; } = new List<Seizoensklassement>();

    public virtual ICollection<Speeldag> Speeldags { get; set; } = new List<Speeldag>();
}
