using Petanque.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petanque.Services
{
    public interface IAanwezigheidService
    {
        AanwezigheidResponseContract? GetById(int id);
        AanwezigheidResponseContract Create(AanwezigheidRequestContract request);
        IEnumerable<AanwezigheidResponseContract> GetAll();
    }
}
