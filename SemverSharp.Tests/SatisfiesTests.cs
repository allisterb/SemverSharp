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
            SemanticVersion v090 = new SemanticVersion(0, 9, 0);
            ComparatorSet axr = Grammar.XRange.Parse("*");
            Assert.True(SemanticVersion.Satisfies(v130, axr));
            ComparatorSet majorxr = Grammar.XRange.Parse("1.x");
            ComparatorSet majorminorxr = Grammar.XRange.Parse("0.9.x");
            Assert.True(SemanticVersion.Satisfies(v130, majorxr));
            Assert.False(SemanticVersion.Satisfies(v090, majorxr));
            Assert.True(SemanticVersion.Satisfies(v090, majorminorxr));
        }
    }
}
