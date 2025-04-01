using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petanque.Contracts.Requests
{
    public class PlayerRequestContract
    {
        public string Voornaam { get; set; } = null!;

        public string Naam { get; set; } = null!;
    }
}
