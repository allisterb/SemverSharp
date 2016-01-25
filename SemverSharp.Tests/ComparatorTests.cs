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
        SemanticVersion v1 = new SemanticVersion(1);
        SemanticVersion v11 = new SemanticVersion(1, 1);
        SemanticVersion v2 = new SemanticVersion(2);
        SemanticVersion v202 = new SemanticVersion(2, 0, 2);
        SemanticVersion v202a = new SemanticVersion(2, 0, 2, "alpha");
        SemanticVersion v310ab = new SemanticVersion(3, 1, 0, "alpha.beta");
        SemanticVersion v000a1 = new SemanticVersion(0, 0, 0, "alpha.1");
        SemanticVersion v000b1 = new SemanticVersion(0, 0, 0, "beta.1");
        SemanticVersion v000a2 = new SemanticVersion(0, 0, 0, "alpha.2");
        SemanticVersion v000a0 = new SemanticVersion(0, 0, 0, "alpha.0");
        SemanticVersion v090a1 = new SemanticVersion(0, 9, 0, "alpha.1");
        SemanticVersion v010a1 = new SemanticVersion(0, 10, 0, "alpha.1");
        SemanticVersion v090a2 = new SemanticVersion(0, 9, 0, "alpha.2");
        SemanticVersion v090b1 = new SemanticVersion(0, 9, 0, "beta.1");
        SemanticVersion v090b2 = new SemanticVersion(0, 9, 0, "beta.2");

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

            BinaryExpression e = SemanticVersion.GetComparator(ExpressionType.LessThan, v1, v2);
            Assert.NotNull(e);          
            BinaryExpression e2 = SemanticVersion.GetComparator(ExpressionType.LessThan, v000a2, v090a1);            
            Assert.True(SemanticVersion.InvokeComparator(e2));                        
            e2 = SemanticVersion.GetComparator(ExpressionType.LessThan, v000a1, v010a1);
            Assert.True(SemanticVersion.InvokeComparator(e2)); //should not compare on prerelease            
            e2 = SemanticVersion.GetComparator(ExpressionType.LessThan, v000a1, v010a1);            
            Assert.True(SemanticVersion.InvokeComparator(e2));
            Assert.True(SemanticVersion.InvokeComparator(SemanticVersion.GetComparator(ExpressionType.LessThan, v202a, v202)));
            Assert.True(SemanticVersion.InvokeComparator(SemanticVersion.GetComparator(ExpressionType.LessThan, v090a1, v090b2)));            
            Assert.False(SemanticVersion.InvokeComparator(SemanticVersion.GetComparator(ExpressionType.LessThan, v000a1, v000a0)));
            Assert.True(SemanticVersion.InvokeComparator(SemanticVersion.GetComparator(ExpressionType.LessThan, v090b1, v090b2)));
            Assert.True(SemanticVersion.InvokeComparator(SemanticVersion.GetComparator(ExpressionType.LessThan, v090a1, v090b2)));
            Assert.True(SemanticVersion.InvokeComparator(SemanticVersion.GetComparator(ExpressionType.LessThan, v090a2, v090b1)));
        }

        [Fact]
        public void CanSatisfyLessThanOrEqual()
        {            
            BinaryExpression e = SemanticVersion.GetComparator(ExpressionType.LessThanOrEqual, v1, v1);            
            Assert.NotNull(e);
            Assert.True(SemanticVersion.InvokeComparator(e));
            BinaryExpression e2 = SemanticVersion.GetComparator(ExpressionType.LessThanOrEqual, v000a2, v090a1);            
            Assert.True(SemanticVersion.InvokeComparator(e2));            
            e2 = SemanticVersion.GetComparator(ExpressionType.LessThanOrEqual, v000a1, v010a1);
            Assert.True(SemanticVersion.InvokeComparator(e2)); //should compare on prerelease
            e2 = SemanticVersion.GetComparator(ExpressionType.LessThanOrEqual, v000a1, v010a1);
            Assert.True(SemanticVersion.InvokeComparator(e2));            
            Assert.False(SemanticVersion.InvokeComparator(SemanticVersion.GetComparator(ExpressionType.LessThanOrEqual, v090a1, v000a0)));
        }


        [Fact]
        public void CanSatisfyRangeIntersect()
        {           
            Assert.True(SemanticVersion.RangeIntersect(ExpressionType.LessThan, v1, ExpressionType.LessThan, v11));
            Assert.False(SemanticVersion.RangeIntersect(ExpressionType.LessThan, v1, ExpressionType.GreaterThan, v11));
            Assert.False(SemanticVersion.RangeIntersect(ExpressionType.GreaterThan, v11, ExpressionType.GreaterThan, v11));
            Assert.False(SemanticVersion.RangeIntersect(ExpressionType.LessThan, v090b1, ExpressionType.GreaterThan, v11));
            Assert.True(SemanticVersion.RangeIntersect(ExpressionType.LessThan, v010a1, ExpressionType.GreaterThan, v090a2));
            Assert.True(SemanticVersion.RangeIntersect(ExpressionType.GreaterThan, v202a, ExpressionType.LessThan, v310ab));
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
