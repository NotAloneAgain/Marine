using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marine.Redux.API.Interfaces
{
    public interface IHasHandlers
    {
        void Subscribe();

        void Unsubscribe();
    }
}
