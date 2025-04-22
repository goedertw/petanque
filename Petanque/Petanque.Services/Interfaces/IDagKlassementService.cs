using Petanque.Contracts.Requests;
using Petanque.Contracts.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petanque.Services.Interfaces {
    public interface IDagKlassementService 
    {
        IEnumerable<DagKlassementResponseContract>? GetById(int id);
        DagKlassementResponseContract Create(DagKlassementRequestContract request);
        IEnumerable<DagKlassementResponseContract> CreateDagKlassementen(SpeeldagResponseContract spelerscores, int id);
    }
}
