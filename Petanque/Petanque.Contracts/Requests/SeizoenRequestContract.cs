using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petanque.Contracts.Requests
{
    public class SeizoenRequestContract
    {

        public DateOnly Startdatum { get; set; }

        public DateOnly Einddatum { get; set; }

        
    }
}
