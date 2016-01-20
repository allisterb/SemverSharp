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
        public void CanCompareEqual()
        {
            SemanticVersion v1 = new SemanticVersion(1,0,0,"alpha2");            
            SemanticVersion v2 = new SemanticVersion(3, 5, 0, "alpha2");
            Assert.Equal(v1.PreRelease, v2.PreRelease);            
        }

        [Fact]
        public void CanCompareLessThan()
        {
            SemanticVersion v1 = new SemanticVersion(1, 0, 0, "alpha.1");
            SemanticVersion v2 = new SemanticVersion(1, 3, 4, "alpha.2.0");
            SemanticVersion v3 = new SemanticVersion(0, 0, 0, "beta.0");
            Assert.True(v1.PreRelease < v2.PreRelease);
            Assert.True(v2.PreRelease < v3.PreRelease);
        }

        [Fact]
        public void CanComparePrelease()
        {
            SemanticVersion v1 = new SemanticVersion(3, 5, 0, "alpha.1");
            SemanticVersion v2 = new SemanticVersion(3, 5, 0, "alpha.2");
            Assert.NotNull(v1);
        }
    }
}
