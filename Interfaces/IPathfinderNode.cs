using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Interfaces
{
    public interface IPathfinderNode<T> : IEquatable<T>, IEqualityComparer<T>
    {
        IEnumerable<T> Adjacent();
    }
}
