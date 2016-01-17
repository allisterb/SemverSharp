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
            Tuple<ExpressionType, SemanticVersion> c = Grammar.Comparator.Parse("<=1.5.4");
            Assert.Equal(c.Item1, ExpressionType.LessThan);
            Assert.Equal(Grammar.Comparator.Parse("<=1.5.4").Item1, ExpressionType.LessThanOrEqual);
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
