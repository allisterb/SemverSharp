using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SemverSharp
{
    public class Comparator : Tuple<ExpressionType, SemanticVersion>
    {
        public Comparator(ExpressionType op, SemanticVersion version) : base(op, version) { }
    }
}
