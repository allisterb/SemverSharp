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
            SemanticVersion v1 = new SemanticVersion(1);
            SemanticVersion v11 = new SemanticVersion(1,1);
            SemanticVersion v2 = new SemanticVersion(2);
            SemanticVersion v202 = new SemanticVersion(2,0,2);
            SemanticVersion v000a1 = new SemanticVersion(0,0,0,"alpha.1");
            SemanticVersion v000b1 = new SemanticVersion(0, 0, 0, "beta.1");
            SemanticVersion v000a2 = new SemanticVersion(0, 0, 0, "alpha.2");
            SemanticVersion v000a0 = new SemanticVersion(0, 0, 0, "alpha.0");
            SemanticVersion v090a1 = new SemanticVersion(0, 9, 0, "alpha.1");
            SemanticVersion v090a2 = new SemanticVersion(0, 9, 0, "alpha.2");
            SemanticVersion v090b1 = new SemanticVersion(0, 9, 0, "beta.1");
            SemanticVersion v090b2 = new SemanticVersion(0, 9, 0, "beta.2");
            BinaryExpression e = SemanticVersion.GetComparator(ExpressionType.LessThan, v1, v2);
            Assert.NotNull(e);          
            BinaryExpression e2 = SemanticVersion.GetComparator(ExpressionType.LessThan, v090a1, v000a2);            
            Assert.True(SemanticVersion.InvokeComparator(e2));
            v000a1 = new SemanticVersion(0, 9, 0, "alpha.1");
            SemanticVersion v010a1 = new SemanticVersion(0, 10, 0, "alpha.1");
            e2 = SemanticVersion.GetComparator(ExpressionType.LessThan, v000a1, v010a1);
            Assert.False(SemanticVersion.InvokeComparator(e2)); //should compare on prerelease            
            e2 = SemanticVersion.GetComparator(ExpressionType.LessThan, v000a1, v010a1);
            Assert.False(SemanticVersion.InvokeComparator(e2));
            Assert.True(SemanticVersion.InvokeComparator(SemanticVersion.GetComparator(ExpressionType.LessThan, v090a1, v090b2)));            
            Assert.False(SemanticVersion.InvokeComparator(SemanticVersion.GetComparator(ExpressionType.LessThan, v000a1, v000a0)));
            Assert.True(SemanticVersion.InvokeComparator(SemanticVersion.GetComparator(ExpressionType.LessThan, v090b1, v090b2)));
            Assert.True(SemanticVersion.InvokeComparator(SemanticVersion.GetComparator(ExpressionType.LessThan, v090a1, v090b2)));
            Assert.True(SemanticVersion.InvokeComparator(SemanticVersion.GetComparator(ExpressionType.LessThan, v090a2, v090b1)));
        }

        [Fact]
        public void CanSatisfyLessThanOrEqual()
        {
            SemanticVersion v1 = new SemanticVersion(1);
            SemanticVersion v11 = new SemanticVersion(1, 1);
            SemanticVersion v2 = new SemanticVersion(2);
            SemanticVersion v202 = new SemanticVersion(2, 0, 2);
            SemanticVersion v000a1 = new SemanticVersion(0, 0, 0, "alpha.1");
            SemanticVersion v000a2 = new SemanticVersion(0, 0, 0, "alpha.2");
            SemanticVersion v000a0 = new SemanticVersion(0, 0, 0, "alpha.0");
            SemanticVersion v090a1 = new SemanticVersion(0, 9, 0, "alpha.1");
            SemanticVersion v090b1 = new SemanticVersion(0, 9, 0, "beta.1");
            SemanticVersion v090b2 = new SemanticVersion(0, 9, 0, "beta.2");
            BinaryExpression e = SemanticVersion.GetComparator(ExpressionType.LessThanOrEqual, v1, v1);            
            Assert.NotNull(e);
            //Assert.Equal(e.NodeType, ExpressionType.LessThanOrEqual);
            BinaryExpression e2 = SemanticVersion.GetComparator(ExpressionType.LessThanOrEqual, v090a1, v000a2);            
            Assert.True(SemanticVersion.InvokeComparator(e2));
            v000a1 = new SemanticVersion(0, 9, 0, "alpha.1");
            SemanticVersion v010a1 = new SemanticVersion(0, 10, 0, "alpha.1");
            e2 = SemanticVersion.GetComparator(ExpressionType.LessThanOrEqual, v000a1, v010a1);
            Assert.True(SemanticVersion.InvokeComparator(e2)); //should compare on prerelease
            e2 = SemanticVersion.GetComparator(ExpressionType.LessThanOrEqual, v000a1, v010a1);
            Assert.True(SemanticVersion.InvokeComparator(e2));
            Assert.False(SemanticVersion.InvokeComparator(SemanticVersion.GetComparator(ExpressionType.LessThanOrEqual, v000a1, v000a0)));
            Assert.False(SemanticVersion.InvokeComparator(SemanticVersion.GetComparator(ExpressionType.LessThanOrEqual, v000a1, v000a0)));
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
