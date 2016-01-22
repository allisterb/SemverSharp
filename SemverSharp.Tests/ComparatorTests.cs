using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using SemverSharp;
using Sprache;
using Xunit;


namespace SemverSharp.Tests
{
    public class ComparatorTests
    {
        [Fact]
        public void CanParseLessThan ()
        {
            Tuple<ExpressionType, SemanticVersion> c = Grammar.Comparator.Parse("<1.5.4");
            Assert.Equal(c.Item1, ExpressionType.LessThan);
            Assert.Equal(c.Item2.Major, 1);
            Assert.Equal(c.Item2.Minor, 5);
            c = Grammar.Comparator.Parse("<1.0");
            Assert.Equal(c.Item1, ExpressionType.LessThan);
            Assert.Equal(c.Item2.Major, 1);
            Assert.Equal(c.Item2.Minor, 0);
            c = Grammar.Comparator.Parse("<1.0.0-alpha.1.0");
            Assert.Equal(c.Item1, ExpressionType.LessThan);
            Assert.Equal(c.Item2.Major, 1);
            Assert.Equal(c.Item2.Minor, 0);
            Assert.Equal(c.Item2.PreRelease.ToString(), "alpha.1.0");
        }

        [Fact]
        public void CanSatisfyLessThan()
        {
            Expression e = Grammar.ComparatorSet.Parse("1.5<=1.5.4");
            Assert.NotNull(e);
            Assert.Equal(Grammar.Comparator.Parse("<=1.5.4").Item1, ExpressionType.LessThanOrEqual);
        }

        [Fact]
        public void CanParseTilde()
        {
            Tuple<ExpressionType, SemanticVersion> c = Grammar.Comparator.Parse("~1.5.4");
            Assert.Equal(c.Item1, ExpressionType.OnesComplement);
            //Assert.Equal(Grammar.Comparator.Parse("<=1.5.4").Item1, ExpressionType.LessThanOrEqual);
        }

        [Fact]
        public void CanSatisfyTilde()
        {
            Expression e = Grammar.ComparatorSet.Parse("1.5~1");
            Assert.NotNull(e);
            
        }

    }
}
