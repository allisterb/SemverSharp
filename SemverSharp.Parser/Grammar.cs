using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using Sprache;

namespace SemverSharp
{
    public class Grammar
    {
        public static Parser<string> Digits
        {
            get
            {
                return Parse.Digit.AtLeastOnce().Text().Token();
            }
        }

        public static Parser<char> NonDigit
        {
            get
            {
                return Parse.Letter.Or(Parse.Char('-')).Token();
            }
        }
        
        public static Parser<char> PositiveDigit
        {
            get
            {
                return Parse.Digit.Except(Parse.Char('0')).Token();
            }
        }

        public static Parser<string> NumericIdentifier
        {
            get
            {
                return 
                    PositiveDigit.AtLeastOnce().Text()
                    .Or(PositiveDigit.Once().Text())
                    .Or(Parse.Char('0').Once().Text())
                    .Token();
            }
        }

        public static Parser<string> Major
        {
            get
            {
                return NumericIdentifier.Named("major");
            }
        }

        public static Parser<string> Minor
        {
            get
            {

                return
                    from dot in Parse.Char('.')
                    from m in NumericIdentifier.Named("minor")
                    select m;
            }
        }

        public static Parser<string> Patch
        {
            get
            {

                return
                    from dot in Parse.Char('.')
                    from m in NumericIdentifier.Named("patch")
                    select m;
            }
        }
       
        public static Parser<string> PreReleaseSuffix
        {
            get
            {

                return
                    from dot in Parse.Char('-')
                    from m in PreRelease.Select(b => string.Join(".", b.ToArray()))
                    select m;
            }
        }

        public static Parser<string> BuildSuffix
        {
            get
            {

                return
                    from dot in Parse.Char('+')
                    from b in Build.Select(b => string.Join(".", b.ToArray()))
                    select b;
            }
        }
      
        public static Parser<char> IdentifierCharacter
        {
            get
            {
                return Parse.Digit.Or(NonDigit).Token();
            }
        }

        public static Parser<string> IdentifierCharacters
        {
            get
            {
                return IdentifierCharacter.AtLeastOnce().Token().Text();
            }
        }


        public static Parser<string> AlphaNumericIdentifier
        {
            get
            {
                return
                    IdentifierCharacters.Concat(NonDigit.Once()).Concat(IdentifierCharacters)
                    .Or(IdentifierCharacters.Concat(NonDigit.Once()))
                    .Or(NonDigit.Once().Concat(IdentifierCharacters))
                    .Or(NonDigit.Once())
                    .Token()
                    .Text();                                                     
            }
        }
       
        public static Parser<string> BuildIdentifier
        {
            get
            {
                return AlphaNumericIdentifier.Or(Digits);
            }
        }

        public static Parser<string> PreReleaseIdentifier
        {
            get
            {
                return AlphaNumericIdentifier.Or(NumericIdentifier);
            }
        }


        public static Parser<IEnumerable<string>> DotSeparatedBuildIdentifier
        {
            get
            {
                return
                    BuildIdentifier.DelimitedBy(Parse.String("."));                                        
            }
        }


        public static Parser<IEnumerable<string>> Build
        {
            get
            {
                return DotSeparatedBuildIdentifier;

            }
        }

        public static Parser<IEnumerable<string>> DotSeparatedPreReleaseIdentifiers
        {
            get
            {
                return
                   PreReleaseIdentifier.DelimitedBy(Parse.String("."));
            }
        }
        
        public static Parser<IEnumerable<string>> PreRelease
        {
            get
            {
                return DotSeparatedPreReleaseIdentifiers;
            }
        }

        public static Parser<IEnumerable<string>> VersionCore
        {
            get
            {
                return
                    Major
                    .Then(major => (Minor.XOr(Parse.Return(string.Empty)))
                    .Select(minor => major + "|" + minor))
                    .Then(minor => (Patch.XOr(Parse.Return(string.Empty)))
                    .Select(patch => (minor + "|" + patch)))
                     .Select(v => v.Split('|').ToList());
            }
        }

        public static Parser<IEnumerable<string>> SemanticVersion
        {
            get
            {
                return

                    Major
                    .Then(major => (Minor.XOr(Parse.Return(string.Empty)))
                    .Select(minor => major + "|" + minor))
                    .Then(minor => (Patch.XOr(Parse.Return(string.Empty)))
                    .Select(patch => (minor + "|" + patch)))
                    .Then(patch => (PreReleaseSuffix.XOr(Parse.Return(string.Empty)))
                    .Select(prs => (patch + "|" + prs)))
                    .Then(prs => (BuildSuffix.XOr(Parse.Return(string.Empty)))
                    .Select(bs => (prs + "|" + bs)))
                    .Select(v => v.Split('|').ToList());
            }
        }
        
        public static Parser<SemanticVersion> SemanticVersionModel
        {
            get
            {
                return SemanticVersion.Select(v => new SemanticVersion(v.ToList()));

            }
        }
        
        public static Parser<ExpressionType> LessThan
        {
            get
            {
                return Parse.String("<").Token().Return(ExpressionType.LessThan);
            }
        }

        public static Parser<ExpressionType> LessThanOrEqual
        {
            get
            {

                return Parse.String("<=").Token().Return(ExpressionType.LessThanOrEqual);
            }
        }

        public static Parser<ExpressionType> GreaterThan
        {
            get
            {
                return Parse.String(">").Token().Return(ExpressionType.GreaterThan);
            }
        }

        public static Parser<ExpressionType> GreaterThanOrEqual
        {
            get
            {

                return Parse.String(">=").Token().Return(ExpressionType.GreaterThanOrEqual);
            }
        }

        public static Parser<ExpressionType> Equal
        {
            get
            {

                return Parse.String("=").Token().Return(ExpressionType.LessThanOrEqual);
            }
        }

        public static Parser<ExpressionType> Tilde
        {
            get
            {

                return Parse.String("~").Token().Return(ExpressionType.OnesComplement);
            }
        }

        public static Parser<ExpressionType> VersionOperator
        {
            get
            {
                return LessThanOrEqual.Or(LessThan).Or(GreaterThan).Or(GreaterThanOrEqual).Or(Equal).Or(Tilde);
            }
        }


        public static Parser<Tuple<ExpressionType, SemanticVersion>> Comparator
        {
            get
            {
                return VersionOperator.Then(o =>
                    SemanticVersionModel.Select(version
                    => new Tuple<ExpressionType, SemanticVersion>(o, version)));                                                            
            }
        }

        public static Parser<Expression> ComparatorSet
        {
            get
            {
                return
                    from l in SemanticVersionModel
                    from r in Comparator.AtLeastOnce()
                    select SemverSharp.SemanticVersion.GetComparator(l, r);                    
            }
        }

        public static Func<bool> GetCompareExpression(string op, SemanticVersion l, SemanticVersion r = null)
        {
            ConstantExpression left = Expression.Constant(l, typeof(SemanticVersion));
            ConstantExpression right = Expression.Constant(r, typeof(SemanticVersion));
            switch (op)
            {
                case "<":
                    List<Expression> expressions = new List<Expression>();
                    expressions.Add(Expression.MakeBinary(ExpressionType.LessThan, left, right));
                    BlockExpression block = Expression.Block(expressions);
                    return Expression.Lambda<Func<bool>>(Expression.Block(expressions)).Compile();

                default:
                    throw new ArgumentException("Unsupported operator: " + op);

                
                    
            }


            
        }


        //<valid semver> ::= <version core> | <version core> "-" <pre-release> | <version core> "+" <build> | <version core> "-" <pre-release> "+" <build>
    }
}
