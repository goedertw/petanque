using Petanque.Contracts.Requests;
using Petanque.Contracts.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petanque.Services.Interfaces
{
    public interface IAanwezigheidService
    {
        AanwezigheidResponseContract? GetById(int id);
        AanwezigheidResponseContract Create(AanwezigheidRequestContract request);
        IEnumerable<AanwezigheidResponseContract> GetAll();
    }
}
