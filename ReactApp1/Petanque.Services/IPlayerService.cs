using Petanque.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petanque.Services
{
    public interface IPlayerService
    {
        PlayerResponseContract? GetById(int id);
        PlayerResponseContract Create(PlayerRequestContract request);
    }
}
