using Petanque.Contracts.Requests;
using Petanque.Contracts.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petanque.Services.Interfaces
{
    public interface IScoreService
    {
        SpelResponseContract? GetById(int id);
        SpelResponseContract Create(SpelRequestContract request);
    }
}
