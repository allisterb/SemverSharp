using System;
using System.Collections.Generic;
using System.Linq;
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
            Assert.True(Grammar.Digits.Parse("44") == "44");
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
            var v = Grammar.SemanticVersion.Parse("0.0.1+build.12");
            Assert.NotEmpty(v);
        }


    }
}
