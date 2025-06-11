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
        IEnumerable<AanwezigheidResponseContract> GetAanwezighedenOpSpeeldag(int id);
        void Delete(int id);
        IEnumerable<AanwezigheidResponseContract> GetAanwezighedenOpSpeler(int spelerId);

    }
}
