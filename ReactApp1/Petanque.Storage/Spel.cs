using System;
using System.Collections.Generic;

namespace Petanque.Storage;

public partial class Spel
{
    public int SpelId { get; set; }

    public int? SpeeldagId { get; set; }

    public string Terrein { get; set; } = null!;

    public int SpelerVolgnr { get; set; }

    public int ScoreA { get; set; }

    public int ScoreB { get; set; }

    public virtual Speeldag? Speeldag { get; set; }
}
