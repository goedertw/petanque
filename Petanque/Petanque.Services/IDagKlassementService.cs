using Petanque.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petanque.Services {
    public interface IDagKlassementService {
        DagKlassementResponseContract? GetById(int id);
        DagKlassementResponseContract Create(DagKlassementRequestContract request);
    }
}
