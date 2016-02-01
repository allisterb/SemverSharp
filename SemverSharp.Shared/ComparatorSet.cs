using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SemverSharp
{
    public class ComparatorSet : List<Tuple<ExpressionType, SemanticVersion>>
    {
        public ComparatorSet() : base() {}                           
    }
}
