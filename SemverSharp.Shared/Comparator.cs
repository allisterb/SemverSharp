using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SemverSharp
{
    public class Comparator
    {
        private Tuple<ExpressionType, SemanticVersion> _comparator;

        public ExpressionType Operator { get { return _comparator.Item1; } }

        public SemanticVersion Version { get { return _comparator.Item2; } }

        public Comparator(ExpressionType op, SemanticVersion version)
        {
            _comparator = new Tuple<ExpressionType, SemanticVersion>(op, version);
        }
    }
}
