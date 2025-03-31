using System;
using System.Collections.Generic;

namespace Petanque.Storage;

public partial class Aanwezigheid
{
    public int AanwezigheidId { get; set; }

    public int? SpeeldagId { get; set; }

    public int? SpelerId { get; set; }

    public int SpelerVolgnr { get; set; }

    public virtual Speeldag? Speeldag { get; set; }

    public virtual Speler? Speler { get; set; }
}
