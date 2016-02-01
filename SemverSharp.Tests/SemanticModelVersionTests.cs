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
            Assert.Equal(v026a.PreRelease.ToString(), "alpha.0");
            SemanticVersion v1001b3 = new SemanticVersion("10.0.1-beta.3");
            Assert.Equal(v1001b3.Major, 10);
            Assert.Equal(v1001b3.Minor, 0);
            Assert.Equal(v1001b3.Patch, 1);
            Assert.Equal(v1001b3.PreRelease.ToString(), "beta.3");

        }

        [Fact]
        public void CanCompareEqual()
        {
            SemanticVersion v1 = new SemanticVersion(1);
            SemanticVersion v2 = new SemanticVersion(1, 0);
            SemanticVersion v3 = new SemanticVersion(3, 4);
            SemanticVersion v4 = new SemanticVersion(3, 5);
            SemanticVersion v5 = new SemanticVersion(3, 5, 0);
            SemanticVersion v352 = new SemanticVersion(3, 5, 2);
            SemanticVersion v352a1 = new SemanticVersion(3, 5, 2, "alpha.1");
            SemanticVersion v352a1_ = new SemanticVersion(3, 5, 2, "alpha.1.0");
            Assert.Equal(v1, v2);
            Assert.NotEqual(v2, v3);
            Assert.NotEqual(v3, v4);
            Assert.Equal(v4, v5);
            Assert.NotEqual(v352, v352a1);
            Assert.NotEqual(v352a1_, v352a1);
        }

        [Fact]
        public void CanCompareLessThan()
        {
            SemanticVersion v1 = new SemanticVersion(1);
            SemanticVersion v13 = new SemanticVersion(1, 3);
            SemanticVersion v134 = new SemanticVersion(1, 3, 4);
            SemanticVersion v3 = new SemanticVersion(3);
            SemanticVersion v302 = new SemanticVersion(3, 0, 2);
            SemanticVersion v009 = new SemanticVersion(0,0,9);
            SemanticVersion v079 = new SemanticVersion(0, 7, 9);
            SemanticVersion v11 = new SemanticVersion(11);
            Assert.True(v009 < v1);
            Assert.True(v1 < v134);
            Assert.True(v134 < v3);
            Assert.True(v009 < v079);
            SemanticVersion v100a1 = new SemanticVersion(1, 0, 0, "alpha.1");
            SemanticVersion v100a4 = new SemanticVersion(1, 0, 0, "alpha.4");
            SemanticVersion v130b = new SemanticVersion(1, 3, 0, "beta");
            SemanticVersion v130bx = new SemanticVersion(1, 3, 0, "beta.x");
            Assert.True(v100a1 < v100a4); //Compare on pre-release
            Assert.False(v100a1 < v130b); //False, pre-release doesn't match
            Assert.True(v130b < v3); //Only one identifier has pre-release so do normal compare
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

        [Fact]
        public void CanRangeIntersect()
        {
            Assert.True(SemanticVersion.RangeIntersect("<10.3.2.alpha.1", "<11.0"));
            Assert.True(SemanticVersion.RangeIntersect("<10.3.2.alpha.1", ">1.3"));
            Assert.True(SemanticVersion.RangeIntersect(">=1.2.2", ">1.2.0-alpha.0"));
            Assert.False(SemanticVersion.RangeIntersect(">=1.2.0-alpha.0", "<1.2.0-alpha.0"));
            Assert.True(SemanticVersion.RangeIntersect(">=1.2.0-alpha.0", "<=1.2.0-alpha.0"));
            Assert.True(SemanticVersion.RangeIntersect(">1.4.0-alpha.0", "<=1.7.0"));
            Assert.True(SemanticVersion.RangeIntersect("1.4.0", "<=1.7.0"));
            Assert.False(SemanticVersion.RangeIntersect("10.4", "<=1.7.0.alpha.1"));
            Assert.True(SemanticVersion.RangeIntersect("1.7.0-alpha.1", "<1.7.0"));
            Assert.True(SemanticVersion.RangeIntersect(">1.9.0-beta.0", "1.9.0-beta.1"));
            Assert.True(SemanticVersion.RangeIntersect(">=10", "11.9.0-beta.0"));
        }

    }
}
