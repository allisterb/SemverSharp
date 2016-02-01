using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sprache;
using Xunit;
using SemverSharp;

namespace SemverSharp.Tests
{
    public class SatisfiesTests
    {
        [Fact]
        public void CanSatisfyXRange()
        {
            SemanticVersion v130 = new SemanticVersion(1, 3);
            ComparatorSet xr = Grammar.MajorXRangeExpression.Parse("1.x");
            Assert.True(SemanticVersion.Satisfies(v130, xr));
        }
    }
}
