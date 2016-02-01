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
                return Digits;                    
            }
        }

        public static Parser<string> Major
        {
            get
            {
                return NumericIdentifier;
            }
        }

        public static Parser<string> Minor
        {
            get
            {

                return
                    from dot in Parse.Char('.')
                    from m in NumericIdentifier
                    select m;
            }
        }

        public static Parser<string> Patch
        {
            get
            {

                return
                    from dot in Parse.Char('.')
                    from m in NumericIdentifier
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

        public static Parser<List<string>> SemanticVersionIdentifier
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
        
        public static Parser<SemanticVersion> SemanticVersion
        {
            get
            {
                return SemanticVersionIdentifier.Select(v => new SemanticVersion(v.ToList()));

            }
        }
        
        public static Parser<ExpressionType> LessThan
        {
            get
            {
                return Parse.String("<").Once().Token().Return(ExpressionType.LessThan);
            }
        }

        public static Parser<ExpressionType> LessThanOrEqual
        {
            get
            {

                return Parse.String("<=").Once().Token().Return(ExpressionType.LessThanOrEqual);
            }
        }

        public static Parser<ExpressionType> GreaterThan
        {
            get
            {
                return Parse.String(">").Once().Token().Return(ExpressionType.GreaterThan);
            }
        }

        public static Parser<ExpressionType> GreaterThanOrEqual
        {
            get
            {

                return Parse.String(">=").Once().Token().Return(ExpressionType.GreaterThanOrEqual);
            }
        }

        public static Parser<ExpressionType> Equal
        {
            get
            {

                return Parse.String("=").Once().Token().Return(ExpressionType.LessThanOrEqual);
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
                return LessThanOrEqual.Or(GreaterThanOrEqual).Or(LessThan).Or(GreaterThan).Or(Equal).Or(Tilde);
            }
        }


        public static Parser<Tuple<ExpressionType, SemanticVersion>> Comparator
        {
            get
            {
                return VersionOperator.Then(o =>
                    SemanticVersion.Select(version
                    => new Tuple<ExpressionType, SemanticVersion>(o, version)))
                    .Or(SemanticVersion.Select(s => new Tuple<ExpressionType, SemanticVersion>(ExpressionType.Equal, s)));                                                            
            }
        }

        /*
        public static Parser<BinaryExpression> IntervalExpression
        {
            get
            {
                return RangeExpression.Then(l => RangeExpression
                    .Select(r => SemverSharp.SemanticVersion.GetIntervalExpression(l.Item1, l.Item2, r.Item1, r.Item2)));
            }
        }*/

        /*
        public static Parser<Expression> ComparatorSet
        {
            get
            {
                return
                    from l in SemanticVersion
                    from r in Comparator.AtLeastOnce()
                    select SemverSharp.SemanticVersion.GetBinaryExpression(l, r);                    
            }
        }*/

        
        public static Parser<string> XIdentifier
        {
            get
            {
                return
                    Parse.Char('*').XOr(Parse.Char('x')).XOr(Parse.Char('X')).Once().Text().Token();
            }
        }

        
        public static Parser<ComparatorSet> MajorXRangeExpression
        {
            get
            {
                return
                    from major in Major.Select(m =>
                    {
                        int num;
                        Int32.TryParse(m.ToString(), out num);
                        return num;

                    })
                    from dot in Parse.Char('.').Once().Text().Token()
                    from x in XIdentifier
                    select new ComparatorSet //List<Tuple<ExpressionType, SemanticVersion>>(2)
                    {
                        new Comparator(ExpressionType.GreaterThanOrEqual, new SemanticVersion(major)),
                        new Comparator(ExpressionType.LessThan, new SemanticVersion(major + 1))
                    };                    
            }
        }

        public static Parser<List<Tuple<ExpressionType, SemanticVersion>>> MajorMinorXRangeExpression
        {
            get
            {
                return
                    from major in Major.Select(m =>
                    {
                        int num;
                        Int32.TryParse(m.ToString(), out num);
                        return num;

                    })
                    from minor in Minor.Select(m =>
                    {
                        int num;
                        Int32.TryParse(m.ToString(), out num);
                        return num;

                    })
                    from dot in Parse.Char('.').Once().Text().Token()
                    from x in XIdentifier
                    select new List<Tuple<ExpressionType, SemanticVersion>>(2)
                    {
                        new Tuple<ExpressionType, SemverSharp.SemanticVersion>(ExpressionType.GreaterThanOrEqual, new SemanticVersion(major, minor)),
                        new Tuple<ExpressionType, SemverSharp.SemanticVersion>(ExpressionType.LessThan, new SemanticVersion(major, minor  + 1))
                    };
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
