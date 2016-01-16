using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using Sprache;

namespace SemverSharp
{
    public class SemanticVersion
    {
        public int? Major { get; set; } = null;
        public int? Minor { get; set; } = null;
        public int? Patch { get; set; } = null;
        public IEnumerable<string> PreRelease { get; set; } = null;
        public IEnumerable<string> Build { get; set; } = null;

        public SemanticVersion(int? major, int? minor = null, int? patch = null, string prerelease = "", string build = "")
        {
            if (!major.HasValue) throw new ArgumentNullException("Major component cannot be null.");
            this.Major = major;
            this.Minor = minor;
            this.Patch = patch;
            if (!string.IsNullOrEmpty(prerelease))
            {
                if (Grammar.PreRelease.Parse(prerelease).Count() > 0)
                {
                    this.PreRelease = Grammar.PreRelease.Parse(prerelease);
                }
                else throw new ArgumentException("The prerelease identifier is not valid: " + prerelease + ".");
            }
            
            if (!string.IsNullOrEmpty(build))
            {
                if (Grammar.PreRelease.Parse(build).Count() > 0)
                {
                    this.Build = Grammar.PreRelease.Parse(build);
                }
                else throw new ArgumentException("The build identifier is not valid: " + build + ".");
            }
            
        }

        public SemanticVersion(List<string> v)
        {
            if (v.Count() != 5) throw new ArgumentException("List length is not 5.");
            this.Major = null;
            this.Minor = null;
            this.Patch = null;            
            int major, minor, patch;
            if (Int32.TryParse(v[0], out major))
            {
                this.Major = major;
            }
            else
            {
                throw new ArgumentNullException("Could not parse major component or major component cannot be null.");
            }
            if (Int32.TryParse(v[1], out minor)) this.Minor = minor;
            if (Int32.TryParse(v[2], out patch)) this.Patch = minor;
            if (string.IsNullOrEmpty(v[3]))
            {
                if (Grammar.PreRelease.Parse(v[3]).Count() > 0)
                {
                    this.PreRelease = Grammar.PreRelease.Parse(v[3]);
                }
                else throw new ArgumentException("The prerelease identifier is not valid: " + v[3] + ".");
            }

            if (!string.IsNullOrEmpty(v[4]))
            {
                if (Grammar.PreRelease.Parse(v[4]).Count() > 0)
                {
                    this.Build = Grammar.PreRelease.Parse(v[4]);
                }
                else throw new ArgumentException("The build identifier is not valid: " + v[4] + ".");
            }

            else throw new ArgumentException("The build identifier is not valid: " + v[4] + ".");            
        }


        public static bool operator == (SemanticVersion left, SemanticVersion right)
        {
            return Expression.Lambda<Func<bool>>(GetComparator(ExpressionType.Equal, right, left)).Compile().Invoke();
        }

        public static bool operator != (SemanticVersion left, SemanticVersion right)
        {
            return Expression.Lambda<Func<bool>>(GetComparator(ExpressionType.NotEqual, right, left)).Compile().Invoke();
        }

        public static bool operator < (SemanticVersion left, SemanticVersion right)
        {
            return Expression.Lambda<Func<bool>>(GetComparator(ExpressionType.LessThan, right, left)).Compile().Invoke();
        }

        public static bool operator > (SemanticVersion left, SemanticVersion right)
        {
            return Expression.Lambda<Func<bool>>(GetComparator(ExpressionType.GreaterThanOrEqual, right, left)).Compile().Invoke();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;
            if (ReferenceEquals(this, obj))
                return true;            
            return Expression.Lambda<Func<bool>>(GetComparator(ExpressionType.Equal, (SemanticVersion)obj, this)).Compile().Invoke(); 
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = this.Major.GetHashCode();
                if (this.Minor.HasValue)
                {
                    result = result * 31 + this.Minor.GetHashCode();
                }
                if (this.Patch.HasValue)
                {
                    result = result * 31 + this.Patch.GetHashCode();
                }

                if (this.PreRelease != null)
                {
                    result = result * 31 + this.PreRelease.GetHashCode();
                }
                if (this.Build == null)
                {
                    result = result * 31 + this.Build.GetHashCode();
                }

                return result;
            }
        }

        /*
        public static bool operator <(SemanticVersion left, SemanticVersion right)
        {
            if (!right.Minor.HasValue)
            {
                right.Minor = 0;
                right.Patch = 0;

            }
            else if (!right.Patch.HasValue)
            {
                right.Patch = 0;
            }

            if (!left.Minor.HasValue) //Major component only
            {
                return left.Major < right.Major;
            }
            else if (!left.Patch.HasValue) //Major + minor component only
            {
                return left.Major < right.Major || (left.Major == right.Major) && (left.Minor < right.Minor);
            }
            else //Major + minor + patch component
            {
                return left.Major < right.Major ||
                    (left.Major == right.Major) && (left.Minor < right.Minor) ||
                    (left.Major == right.Major) && (left.Minor == right.Minor) && (left.Patch < right.Patch);
            }
        }
            

        
        public static bool operator > (SemanticVersion left, SemanticVersion right)
        {
            if (!right.Minor.HasValue)
            {
                right.Minor = 0;
                right.Patch = 0;

            }
            else if (!right.Patch.HasValue)
            {
                right.Patch = 0;
            }

            if (!left.Minor.HasValue) //Major component only
            {
                return left.Major > right.Major;
            }
            else if (!left.Patch.HasValue) //Major + minor component only
            {
                return left.Major > right.Major || (left.Major == right.Major) && (left.Minor > right.Minor);
            }
            else //Major + minor + patch component
            {
                return left.Major > right.Major ||
                    (left.Major == right.Major) && (left.Minor > right.Minor) ||
                    (left.Major == right.Major) && (left.Minor == right.Minor) && (left.Patch > right.Patch);
            }
        }
        */

        public static bool operator <= (SemanticVersion left, SemanticVersion right)
        {
            return Expression.Lambda<Func<bool>>(GetComparator(ExpressionType.LessThanOrEqual, right, left)).Compile().Invoke();
        }

        public static bool operator >=(SemanticVersion left, SemanticVersion right)
        {
            return Expression.Lambda<Func<bool>>(GetComparator(ExpressionType.GreaterThanOrEqual, right, left)).Compile().Invoke();
        }


        public static Expression GetComparator(ExpressionType et, SemanticVersion right, SemanticVersion left = null)
        {
            if (ReferenceEquals(right, null)) throw new ArgumentNullException("Right operand cannot be null.");
            if (!right.Minor.HasValue)
            {
                right.Minor = 0;
                right.Patch = 0;

            }
            else if (!right.Patch.HasValue)
            {
                right.Patch = 0;
            }

            ConstantExpression r_major = Expression.Constant(right.Major, typeof(int));
            ConstantExpression r_minor = Expression.Constant(right.Minor, typeof(int));
            ConstantExpression r_patch = Expression.Constant(right.Patch, typeof(int));
            ConstantExpression l_major = Expression.Constant(left.Major, typeof(int));
            ConstantExpression l_minor = null;
            ConstantExpression l_patch = null;

            if (!left.Minor.HasValue) //Major component only
            {
                //Expression e = Expression.MakeBinary(et, l_major, r_major);
                //return e;
                return GetComparator(et, right, new SemanticVersion(left.Major, 0, 0));
            }
            else if (!left.Patch.HasValue) //Major + minor component only
            {
                return GetComparator(et, right, new SemanticVersion(left.Major, left.Minor, 0));

                //l_minor = Expression.Constant(left.Minor, typeof(int));
                //Expression a = Expression.MakeBinary(et, l_major, r_major);
                //Expression b = Expression.MakeBinary(et, l_minor, r_minor);
                //left.Major > right.Major || (left.Major == right.Major) && (left.Minor > right.Minor);
                //Expression e = Expression.Or(a, Expression.AndAlso(Expression.MakeBinary(ExpressionType.Equal, l_major, r_major), b));
                //return e;
            }
            else //Major + minor + patch component
            {

                l_minor = Expression.Constant(left.Minor, typeof(int));
                l_patch = Expression.Constant(left.Patch, typeof(int));
                Expression a = Expression.MakeBinary(et, l_major, r_major);
                Expression b = Expression.MakeBinary(et, l_minor, r_minor);
                Expression c = Expression.MakeBinary(et, l_patch, r_patch);
                Expression e = Expression.OrElse(a,
                    Expression.OrElse(
                        Expression.OrElse(Expression.AndAlso(Expression.MakeBinary(ExpressionType.Equal, l_major, r_major), b), a),
                            Expression.AndAlso(Expression.AndAlso(Expression.MakeBinary(ExpressionType.Equal, l_major, r_major), Expression.MakeBinary(ExpressionType.Equal, l_minor, r_minor)), c)
                       ));

                //                return left.Major > right.Major ||
                //                    (left.Major == right.Major) && (left.Minor > right.Minor) ||
                //                    (left.Major == right.Major) && (left.Minor == right.Minor) && (left.Patch > right.Patch);
                return e;
            }

        }


    }
}
