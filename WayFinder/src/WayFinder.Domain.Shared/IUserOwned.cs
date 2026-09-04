using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WayFinder
{
    public interface IUserOwned
    {
        Guid UserId { get; set; }
    }
}
