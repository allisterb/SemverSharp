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
    public class RangeTests
    {
        [Fact]
        public void CanQueryLessThan ()
        {
            Expression e = Grammar.RangeExpression.Parse("1.3.4 < 1.5.4");
            Assert.NotNull(e);
        }
    }
}
