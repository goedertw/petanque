using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petanque.Contracts.Responses
{
    public class SeizoenResponseContract
    {
        public int SeizoensId { get; set; }

        public DateOnly Startdatum { get; set; }

        public DateOnly Einddatum { get; set; }

        //public virtual ICollection<SeizoensKlassementResponseContract> Seizoensklassements { get; set; } = new List<SeizoensKlassementResponseContract>();

        public virtual ICollection<SpeeldagResponseContract> Speeldags { get; set; } = new List<SpeeldagResponseContract>();
    }
}
