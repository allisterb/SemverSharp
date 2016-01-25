using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;
using SemverSharp;

namespace SemverSharp.Tests
{
    public class SemanticVersionModelTests
    {

        [Fact]
        public void CanConstruct()
        {
            SemanticVersion v1 = new SemanticVersion(3, 5, 0, "alpha.1");
            Assert.NotNull(v1);
            Assert.Equal(v1.Major, 3);
            Assert.Equal(v1.Minor, 5);
            Assert.Equal(v1.Patch, 0);
            Assert.Equal(v1.PreRelease.ToString(), "alpha.1");
            SemanticVersion v026a = new SemanticVersion("0.2.6-alpha");
            Assert.Equal(v026a.Major, 0);
            Assert.Equal(v026a.Minor, 2);
            Assert.Equal(v026a.Patch, 6);
            Assert.Equal(v026a.PreRelease.ToString(), "alpha");

        }

        [Fact]
        public void CanCompareEqual()
        {
            SemanticVersion v1 = new SemanticVersion(1);
            SemanticVersion v2 = new SemanticVersion(1, 0);
            SemanticVersion v3 = new SemanticVersion(3, 4);
            SemanticVersion v4 = new SemanticVersion(3, 5);
            SemanticVersion v5 = new SemanticVersion(3, 5, 0);
            SemanticVersion v352a1 = new SemanticVersion(3, 5, 2, "alpha.1");
            Assert.Equal(v1, v2);
            Assert.NotEqual(v2, v3);
            Assert.NotEqual(v3, v4);
            Assert.Equal(v4, v5);
        }

        [Fact]
        public void CanCompareLessThan()
        {
            SemanticVersion v1 = new SemanticVersion(1);
            SemanticVersion v2 = new SemanticVersion(1, 3, 4);
            SemanticVersion v3 = new SemanticVersion(3);
            SemanticVersion v4 = new SemanticVersion(3, 0, 2);
            SemanticVersion v5 = new SemanticVersion(0,0,9);
            SemanticVersion v6 = new SemanticVersion(0, 7, 9);
            Assert.True(v5 < v1);
            Assert.True(v1 < v2);
            Assert.True(v5 < v3);
            Assert.True(v5 < v6);
            Assert.False(v1 < v5);
            Assert.False(v2 < v1);
            Assert.False(v3 < v2);

        }

        [Fact]
        public void CanComparePrelease()
        {
            SemanticVersion v1 = new SemanticVersion(3, 5, 0, "alpha.1");
            SemanticVersion v2 = new SemanticVersion(3, 5, 0, "alpha.2");
            Assert.NotNull(v1);
        }

        [Fact]
        public void CanIncrement()
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
            Assert.Equal((++v1).ToString(), "2");
            Assert.Equal((++v11).ToString(), "1.2");
            Assert.Equal((++v202).ToString(), "2.0.3");
            Assert.Equal((++v000a1).ToString(), "0.0.0.alpha.2");
            Assert.Equal((++v090b2).ToString(), "0.9.0.beta.3");
        }

        [Fact]
        public void CanDecrement()
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
            Assert.Equal((--v1).ToString(), "0");
            Assert.Equal((--v11).ToString(), "1.0");
            Assert.Equal((--v202).ToString(), "2.0.1");
            Assert.Equal((--v000a1).ToString(), "0.0.0.alpha");
            Assert.Equal((--v090b2).ToString(), "0.9.0.beta.1");
        }
    }
}
