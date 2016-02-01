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
            Assert.True(SemanticVersion.Satisfies(Grammar.SemanticVersion.Parse("0.0.0"), Grammar.XRange.Parse("*")));
            Assert.True(SemanticVersion.Satisfies(Grammar.SemanticVersion.Parse("1.4"), Grammar.XRange.Parse("1.x")));
            Assert.False(SemanticVersion.Satisfies(Grammar.SemanticVersion.Parse("2.0"), Grammar.XRange.Parse("1.x")));
            Assert.True(SemanticVersion.Satisfies(Grammar.SemanticVersion.Parse("4.4.3"), Grammar.XRange.Parse("4.4.x")));
            Assert.False(SemanticVersion.Satisfies(Grammar.SemanticVersion.Parse("4"), Grammar.XRange.Parse("4.4.x")));
        }

        [Fact]
        public void CanSatisfyTildeRange()
        {
            Assert.True(SemanticVersion.Satisfies(new SemanticVersion(1, 2, 4), Grammar.TildeRange.Parse("~1.2.3")));
            Assert.True(SemanticVersion.Satisfies(new SemanticVersion(1, 2, 1), Grammar.TildeRange.Parse("~1.2")));
            Assert.False(SemanticVersion.Satisfies(new SemanticVersion(1, 3), Grammar.TildeRange.Parse("~1.2")));
            //Assert.False(SemanticVersion.Satisfies(new SemanticVersion(1, 2), Grammar.CaretRange.Parse("^1.2.3")));
            //Assert.True(SemanticVersion.Satisfies(new SemanticVersion(0, 2, 5), Grammar.CaretRange.Parse("^0.2.3")));
        }

        [Fact]
        public void CanSatisfyCaretRange()
        {
            Assert.True(SemanticVersion.Satisfies(new SemanticVersion(1, 3), Grammar.CaretRange.Parse("^1.2.3")));
            Assert.True(SemanticVersion.Satisfies(new SemanticVersion(1, 4, 5), Grammar.CaretRange.Parse("^1.2.3")));
            Assert.False(SemanticVersion.Satisfies(new SemanticVersion(1, 2), Grammar.CaretRange.Parse("^1.2.3")));
            Assert.True(SemanticVersion.Satisfies(new SemanticVersion(0, 2, 5), Grammar.CaretRange.Parse("^0.2.3")));
        }
    }
}
