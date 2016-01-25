using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using Sprache;
using Xunit;
using SemverSharp;

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
            //Assert.True(Grammar.IdentifierCharacters.Parse("23") == "23-");
            //Assert.True(Grammar.IdentifierCharacters.Parse("alpha1") == "alpha1");
            //Assert.True(Grammar.IdentifierCharacter.Parse("9") == '9');
            //Assert.Throws<ParseException>(() => Grammar.NonDigit.Parse("."));
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
            //v = Grammar.VersionCore.Parse("5");
            //Assert.True(v.Count() == 1);
            //v = Grammar.VersionCore.Parse("0.5.4");
            //Assert.True(v.Count() == 3);

        }

        [Fact]
        public void CanParseVersion()
        {
            var v = Grammar.SemanticVersionIdentifier.Parse("0.0.1+build.12");
            Assert.NotEmpty(v);
        }

        [Fact]
        public void CanParseRangeExpression()
        {
            Tuple<ExpressionType, SemanticVersion> re = Grammar.RangeExpression.Parse("<10.3.4");
            Assert.Equal(ExpressionType.LessThan, re.Item1);
            Assert.Equal(10, re.Item2.Major);
            Assert.Equal(3, re.Item2.Minor);
            Assert.Equal(4, re.Item2.Patch);
            re = Grammar.RangeExpression.Parse("<=0.0.4-alpha");
            Assert.Equal(ExpressionType.LessThanOrEqual, re.Item1);
            Assert.Equal(0, re.Item2.Major);
            Assert.Equal(4, re.Item2.Patch);
            Assert.Equal("alpha", re.Item2.PreRelease.ToString());
            re = Grammar.RangeExpression.Parse(">10.0.100-beta.0");
            Assert.Equal(ExpressionType.GreaterThan, re.Item1);
            Assert.Equal(10, re.Item2.Major);
            Assert.Equal(100, re.Item2.Patch);
            Assert.Equal("beta.0", re.Item2.PreRelease.ToString());
        }


    }
}
