using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;
using SemverSharp;

namespace SemverSharp.Tests
{
    public class PreReleaseModelTests
    {

        [Fact]
        public void CanConstruct()
        {
            SemanticVersion v1 = new SemanticVersion(1, 2, 3, "alpha.1");
            Assert.NotNull(v1.PreRelease);
            Assert.Equal(v1.PreRelease[0], "alpha");
            Assert.Equal(v1.PreRelease[1], "1");

            SemanticVersion v2 = new SemanticVersion(1, 2, 3, "alpha.decker.3.6.mafia");
            Assert.NotNull(v2.PreRelease);
            Assert.Equal(v2.PreRelease[0], "alpha");
            Assert.Equal(v2.PreRelease[1], "decker");
            Assert.Equal(v2.PreRelease[2], "3");

        }

        [Fact]
        public void CanConvertToString()
        {
            SemanticVersion v1 = new SemanticVersion(1, 2, 3, "alpha.bravo.1");
            SemanticVersion v2 = new SemanticVersion(1, 2, 3, "alpha.decker.3.6.mafia");
            SemanticVersion v3 = new SemanticVersion(1, 2, 3, "1.bravo.alpha");
            Assert.Equal("alpha.bravo.1", v1.PreRelease.ToString());
            Assert.NotEqual(v3.PreRelease.ToString(), v1.PreRelease.ToString());
            Assert.NotEqual(v3.PreRelease.GetHashCode(), v1.PreRelease.GetHashCode());
        }

        [Fact]
        public void CanCompareEqual()
        {
            SemanticVersion v1 = new SemanticVersion(1, 0, 0, "alpha2");
            SemanticVersion v2 = new SemanticVersion(3, 5, 0, "alpha2");
            Assert.Equal(v1.PreRelease, v2.PreRelease);
        }

        [Fact]
        public void CanCompareLessThan()
        {
            SemanticVersion v100a = new SemanticVersion(1, 0, 0, "alpha");
            SemanticVersion v100a1 = new SemanticVersion(1, 0, 0, "alpha.1");
            SemanticVersion v2 = new SemanticVersion(1, 3, 4, "alpha.2.0");
            SemanticVersion v3 = new SemanticVersion(0, 0, 0, "beta.0");
            SemanticVersion v4 = new SemanticVersion(0, 0, 0, "beta.x.0");
            SemanticVersion v5 = new SemanticVersion(0, 0, 0, "beta");
            Assert.False(v100a1.PreRelease < v100a.PreRelease);
            Assert.True(v100a1.PreRelease < v2.PreRelease);
            Assert.True(v2.PreRelease < v3.PreRelease);
            Assert.True(v3.PreRelease < v4.PreRelease);
            Assert.True(v5.PreRelease < v4.PreRelease && v4.PreRelease > v3.PreRelease);
        }

        [Fact]
        public void CanIncrement()
        {
            SemanticVersion v1 = new SemanticVersion(1, 0, 0, "alpha.1");
            SemanticVersion v2 = new SemanticVersion(1, 3, 4, "alpha.2.0");
            SemanticVersion v3 = new SemanticVersion(0, 0, 0, "beta");
            SemanticVersion v4 = new SemanticVersion(0, 0, 0, "beta.x.0");
            SemanticVersion bx860new = new SemanticVersion(0, 0, 0, "beta.x86.0.new");
            PreRelease v11 = ++(v1.PreRelease);
            PreRelease v21 = ++(v2.PreRelease);
            Assert.Equal(v11.ToString(), "alpha.2");
            Assert.Equal(v11[1], "2");
            Assert.Equal(v21[1], "3");
            Assert.Equal(v21.ToString(), "alpha.3");
            ++(v3.PreRelease);
            Assert.Equal(v3.PreRelease.Count, 2);
            Assert.Equal(v3.PreRelease[1], "1");
            ++(v4.PreRelease);
            Assert.Equal(v4.PreRelease[2], "1");
            Assert.Equal((++bx860new).PreRelease.ToString(), "beta.x86.0.new.1");

        }

        [Fact]
        public void CanDecrement()
        {
            SemanticVersion v1 = new SemanticVersion(1, 0, 0, "alpha.1");
            SemanticVersion v2 = new SemanticVersion(1, 3, 4, "alpha.2.0");
            SemanticVersion v3 = new SemanticVersion(0, 0, 0, "beta");
            SemanticVersion v4 = new SemanticVersion(0, 0, 0, "beta.x.0");
            PreRelease v11 = --(v1.PreRelease);
            PreRelease v21 = --(v2.PreRelease);
            Assert.Equal(v11.Count, 1);
            Assert.Equal(v11.ToString(), "alpha");
            Assert.Equal(v21.ToString(), "alpha.1");
            Assert.Equal((--v3).ToString(), "0.0.0");
            Assert.Equal((--v4).ToString(), "0.0.0.beta");            
        }
    }
}