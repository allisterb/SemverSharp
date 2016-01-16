using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;
using SemverSharp;

namespace SemverSharp.Tests
{
    public class ModelTests
    {
        [Fact]
        public void CanCompareEqual()
        {
            SemanticVersion v1 = new SemanticVersion(1);
            SemanticVersion v2 = new SemanticVersion(1, 0);
            Assert.Equal(v1, v2);
        }

        [Fact]
        public void CanCompareLessThan()
        {
            SemanticVersion v1 = new SemanticVersion(1);
            SemanticVersion v2 = new SemanticVersion(1, 3, 4);
            SemanticVersion v3 = new SemanticVersion(3);
            SemanticVersion v4 = new SemanticVersion(3, 0, 2);
            SemanticVersion v5 = new SemanticVersion(0,0,9);
            Assert.True(v5 < v1);
            Assert.True(v1 < v2);
            Assert.True(v5 < v3);
        }
    }
}
