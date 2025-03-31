using Petanque.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petanque.Services
{
    public interface IScoreService
    {
        SpelResponseContract? GetById(int id);
        SpelResponseContract Create(SpelRequestContract request);
    }
}
