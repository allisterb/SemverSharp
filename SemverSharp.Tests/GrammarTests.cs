using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Sprache;
using Xunit;

namespace SemverSharp.Tests
{
    public class GrammarTests
    {

        [Fact]
        public void CanParseDigits()
        {
            Assert.Equal(Grammar.Digits.Parse("1"), "1");
            Assert.Equal(Grammar.Digits.Parse("01"), "01");
            Assert.Equal(Grammar.Digits.Parse("1004"), "1004");
            Assert.Equal(Grammar.Digits.Parse("44"), "44");
            Assert.Throws<ParseException>(() => Grammar.Digits.Parse("d44"));
        }

        [Fact]
        public void CanParseNonDigit()
        {
            Assert.True(Grammar.NonDigit.Parse("-") == '-');
            Assert.True(Grammar.NonDigit.Parse("a") == 'a');
            Assert.Throws<ParseException>(() => Grammar.NonDigit.Parse("4"));
        }


        [Fact]
        public void CanParseIdentifierCharacter()
        {
            Assert.True(Grammar.IdentifierCharacter.Parse("-") == '-');
            Assert.True(Grammar.IdentifierCharacter.Parse("a") == 'a');
            Assert.True(Grammar.IdentifierCharacter.Parse("9") == '9');
            Assert.Throws<ParseException>(() => Grammar.NonDigit.Parse("."));
        }

        [Fact]
        public void CanParseIdentifierCharacters()
        {
            Assert.True(Grammar.IdentifierCharacters.Parse("23-") == "23-");
            Assert.True(Grammar.IdentifierCharacters.Parse("alpha1") == "alpha1");
        }

        [Fact]
        public void CanParsePreleaseSuffix()
        {
            string p = Grammar.PreReleaseSuffix.Parse("-alpha.1");
        }


        [Fact]
        public void CanParseDotSeparatedBuildIdentifiers()
        {
            IEnumerable<string> v = Grammar.DotSeparatedBuildIdentifier.Parse("2.3.4");
            Assert.True(v.Count() == 3);
            v = Grammar.DotSeparatedBuildIdentifier.Parse("1.2.3.4.alpha1");
            Assert.True(v.Count() == 5);
        }

        [Fact]
        public void CanParseAlphaNumericIdentifier()
        {
            Assert.True(Grammar.IdentifierCharacters.Parse("23") == "23");
            Assert.True(Grammar.IdentifierCharacters.Parse("23-") == "23-");
            Assert.True(Grammar.IdentifierCharacters.Parse("alpha1") == "alpha1");
        }

        [Fact]
        public void CanParseVersionCore()
        {
            List<string> v = Grammar.VersionCore.Parse("2.3.4").ToList();
            Assert.NotEmpty(v);
            Assert.Equal(v[0], "2");
            v = Grammar.VersionCore.Parse("4").ToList();
            Assert.Equal(v[0], "4");
            Assert.Equal(v[1], "");
            Assert.Equal(v[2], "");
        }

        [Fact]
        public void CanParseVersionIdentifier()
        {
            var v = Grammar.SemanticVersionIdentifier.Parse("0.0.1+build.12");
            Assert.NotEmpty(v);
        }

        [Fact]
        public void CanParseComparator()
        {
            Comparator re = Grammar.Comparator.Parse("<10.3.4");
            Assert.Equal(ExpressionType.LessThan, re.Operator);
            Assert.Equal(10, re.Version.Major);
            Assert.Equal(3, re.Version.Minor);
            Assert.Equal(4, re.Version.Patch);
            re = Grammar.Comparator.Parse("<=0.0.4-alpha");
            Assert.Equal(ExpressionType.LessThanOrEqual, re.Operator);
            Assert.Equal(0, re.Version.Major);
            Assert.Equal(4, re.Version.Patch);
            Assert.Equal("alpha.0", re.Version.PreRelease.ToString());
            re = Grammar.Comparator.Parse(">10.0.100-beta.0");
            Assert.Equal(ExpressionType.GreaterThan, re.Operator);
            Assert.Equal(10, re.Version.Major);
            Assert.Equal(100, re.Version.Patch);
            Assert.Equal("beta.0", re.Version.PreRelease.ToString());
            re = Grammar.Comparator.Parse("10.6");
            Assert.Equal(ExpressionType.Equal, re.Operator);
            Assert.Equal(10, re.Version.Major);
            Assert.Equal(6, re.Version.Minor);
            Assert.Equal(null, re.Version.PreRelease);
        }

        [Fact]
        public void CanParseLessThan()
        {
            Comparator c = Grammar.Comparator.Parse("<1.5.4");
            Assert.Equal(c.Operator, ExpressionType.LessThan);
            Assert.Equal(c.Version.Major, 1);
            Assert.Equal(c.Version.Minor, 5);
            c = Grammar.Comparator.Parse("<1.0");
            Assert.Equal(c.Operator, ExpressionType.LessThan);
            Assert.Equal(c.Version.Major, 1);
            Assert.Equal(c.Version.Minor, 0);
            c = Grammar.Comparator.Parse("<1.0.0-alpha.1.0");
            Assert.Equal(c.Operator, ExpressionType.LessThan);
            Assert.Equal(c.Version.Major, 1);
            Assert.Equal(c.Version.Minor, 0);
            Assert.Equal(c.Version.PreRelease.ToString(), "alpha.1.0");
        }

        [Fact]
        public void CanParseXRangeExpression()
        {
            ComparatorSet xr1 = Grammar.MajorXRange.Parse("4.x");
            Assert.NotNull(xr1);
            Assert.Equal(xr1[0].Operator, ExpressionType.GreaterThanOrEqual);
            Assert.Equal(xr1[0].Version, new SemanticVersion(4));
            Assert.Equal(xr1[1].Operator, ExpressionType.LessThan);
            Assert.Equal(xr1[1].Version, new SemanticVersion(5));
            ComparatorSet xr2 = Grammar.MajorMinorXRange.Parse("4.3.x");
            Assert.NotNull(xr1);
            Assert.Equal(xr1[0].Operator, ExpressionType.GreaterThanOrEqual);
            Assert.Throws(typeof(Sprache.ParseException), () => Grammar.MajorXRange.Parse("*"));
            Assert.Throws(typeof(Sprache.ParseException), () => Grammar.MajorXRange.Parse("4.3.x"));
        }

        [Fact]
        public void CanParseTildeRangeExpression()
        {
            ComparatorSet tr1 = Grammar.MajorTildeRange.Parse("~4");
            ComparatorSet tr2 = Grammar.MajorTildeRange.Parse("~14.4");
            ComparatorSet tr3 = Grammar.MajorTildeRange.Parse("~7.0.1");
            Assert.NotNull(tr1);
            Assert.NotNull(tr2);
            Assert.NotNull(tr3);
        }
    }
}
