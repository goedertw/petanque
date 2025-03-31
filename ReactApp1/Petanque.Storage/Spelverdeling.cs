using System;
using System.Collections.Generic;

namespace Petanque.Storage;

public partial class Spelverdeling
{
    public int SpelverdelingsId { get; set; }

    public int? SpelId { get; set; }

    public string Team { get; set; } = null!;

    public string SpelerPositie { get; set; } = null!;

    public int SpelerVolgnr { get; set; }
}
