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
        public PreRelease PreRelease { get; set; } = null;
        public IEnumerable<string> Build { get; set; } = null;

        public SemanticVersion(int? major, int? minor = null, int? patch = null, string prerelease = "", string build = "")
        {
            if (!major.HasValue) throw new ArgumentNullException("Major component cannot be null.");
            if (!minor.HasValue && patch.HasValue) throw new ArgumentNullException("Minor component cannot be null if patch is non-null.");
            if (!string.IsNullOrEmpty(prerelease) && !patch.HasValue) throw new ArgumentNullException("Patch component cannot be null if pre-release is non-null.");
            if (!string.IsNullOrEmpty(build) && !patch.HasValue) throw new ArgumentNullException("Patch component cannot be null if pre-release is non-null.");
            this.Major = major;
            this.Minor = minor;
            this.Patch = patch;
            if (!string.IsNullOrEmpty(prerelease))
            {
                this.PreRelease = new PreRelease();
                if (Grammar.PreRelease.Parse(prerelease).Count() > 0)
                {
                    IEnumerable<string> p = Grammar.PreRelease.Parse(prerelease);
                    for (int i = 0; i < p.Count(); i++)
                    {
                        this.PreRelease.Add(i, p.ElementAt(i));
                    }                    
                }
                else throw new ArgumentException("The prerelease identifier is not valid: " + prerelease + ".");
            }

            if (!string.IsNullOrEmpty(build))
            {
                if (Grammar.Build.Parse(build).Count() > 0)
                {
                    this.Build = Grammar.Build.Parse(build);
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
            if (Int32.TryParse(v[2], out patch)) this.Patch = patch;
            if (!string.IsNullOrEmpty(v[3]))
            {                
                if (Grammar.PreRelease.Parse(v[3]).Count() > 0)
                {
                    this.PreRelease = new PreRelease();
                    IEnumerable<string> p = Grammar.PreRelease.Parse(v[3]);
                    for (int i = 0; i < p.Count(); i++)
                    {
                        this.PreRelease.Add(i, p.ElementAt(i));
                    }
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

        }

        public static int CompareComponent(string a, string b, bool lower = false)
        {
            var aEmpty = String.IsNullOrEmpty(a);
            var bEmpty = String.IsNullOrEmpty(b);
            if (aEmpty && bEmpty)
                return 0;

            if (aEmpty)
                return lower ? 1 : -1;
            if (bEmpty)
                return lower ? -1 : 1;

            var aComps = a.Split('.');
            var bComps = b.Split('.');

            var minLen = Math.Min(aComps.Length, bComps.Length);
            for (int i = 0; i < minLen; i++)
            {
                var ac = aComps[i];
                var bc = bComps[i];
                int anum, bnum;
                var isanum = Int32.TryParse(ac, out anum);
                var isbnum = Int32.TryParse(bc, out bnum);
                int r;
                if (isanum && isbnum)
                {
                    r = anum.CompareTo(bnum);
                    if (r != 0) return anum.CompareTo(bnum);
                }
                else
                {
                    if (isanum)
                        return -1;
                    if (isbnum)
                        return 1;
                    r = String.CompareOrdinal(ac, bc);
                    if (r != 0)
                        return r;
                }
            }

            return aComps.Length.CompareTo(bComps.Length);
        }


        public static bool operator == (SemanticVersion left, SemanticVersion right)
        {
            return Expression.Lambda<Func<bool>>(GetComparator(ExpressionType.Equal, right, left)).Compile().Invoke();
        }

        public static bool operator !=(SemanticVersion left, SemanticVersion right)
        {
            return Expression.Lambda<Func<bool>>(GetComparator(ExpressionType.NotEqual, right, left)).Compile().Invoke();
        }

        public static bool operator <(SemanticVersion left, SemanticVersion right)
        {
            return Expression.Lambda<Func<bool>>(GetComparator(ExpressionType.LessThan, right, left)).Compile().Invoke();
        }

        public static bool operator >(SemanticVersion left, SemanticVersion right)
        {
            return Expression.Lambda<Func<bool>>(GetComparator(ExpressionType.GreaterThan, right, left)).Compile().Invoke();
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

        public static bool operator <=(SemanticVersion left, SemanticVersion right)
        {
            return Expression.Lambda<Func<bool>>(GetComparator(ExpressionType.LessThanOrEqual, right, left)).Compile().Invoke();
        }

        public static bool operator >=(SemanticVersion left, SemanticVersion right)
        {
            return Expression.Lambda<Func<bool>>(GetComparator(ExpressionType.GreaterThanOrEqual, right, left)).Compile().Invoke();
        }

        public static SemanticVersion operator ~ (SemanticVersion right)
        {
            if (!right.Minor.HasValue)
            {
                return new SemanticVersion(++right.Major, 0, 0);
            }


            else 
            {
                return new SemanticVersion(right.Major, ++right.Minor, 0);
            }
            
        }


        public static Expression GetComparator(ExpressionType et, SemanticVersion right, SemanticVersion left)
        {
            if (ReferenceEquals(right, null)) throw new ArgumentNullException("Right operand cannot be null.");
            /*
            if (!right.Minor.HasValue)
            {
                right.Minor = 0;
                right.Patch = 0;

            }
            
            else if (!right.Patch.HasValue)
            {
                right.Patch = 0;
            }
            */
            if (!left.Minor.HasValue) //Major component only
            {
                left.Minor = 0;
                left.Patch = 0;
            }
            else if (!left.Patch.HasValue) //Major + minor component only
            {
                left.Patch = 0;

            }            
            ConstantExpression zero = Expression.Constant(0, typeof(int));
            ConstantExpression l = Expression.Constant(left, typeof(SemanticVersion));
            ConstantExpression r = Expression.Constant(right, typeof(SemanticVersion));
            ConstantExpression l_major = Expression.Constant(left.Major, typeof(int));
            ConstantExpression l_minor = Expression.Constant(left.Minor, typeof(int));
            ConstantExpression l_patch = Expression.Constant(left.Patch, typeof(int));
            ConstantExpression r_major = Expression.Constant(right.Major, typeof(int));
            ConstantExpression r_minor = right.Minor.HasValue ? Expression.Constant(right.Minor, typeof(int)) : null;
            ConstantExpression r_patch = right.Patch.HasValue ? Expression.Constant(right.Patch, typeof(int)) : null;            

            if (et == ExpressionType.Equal || et == ExpressionType.NotEqual)
            {
                Expression a = Expression.MakeBinary(et, l_major, r_major);
                Expression b = right.Minor.HasValue ? Expression.MakeBinary(et, l_minor, r_minor) : null;
                Expression c = right.Patch.HasValue ? Expression.MakeBinary(et, l_patch, r_patch) : null;

                if (!right.Minor.HasValue)
                {
                    return a;
                }
                else if (!right.Patch.HasValue)
                {

                    return Expression.AndAlso(a, b);
                }
                else
                {
                    return Expression.AndAlso(a, Expression.AndAlso(b, c));
                }

            }
            else if (et == ExpressionType.LessThan || et == ExpressionType.LessThanOrEqual || et == ExpressionType.GreaterThan || et == ExpressionType.GreaterThanOrEqual)
            {
                Expression a = Expression.MakeBinary(et, l_major, r_major);
                Expression b = right.Minor.HasValue ? Expression.MakeBinary(et, l_minor, r_minor) : null;
                Expression c = right.Patch.HasValue ? Expression.MakeBinary(et, l_patch, r_patch) : null;

                if (!right.Minor.HasValue)
                {
                    return a;
                }
                else if (!right.Patch.HasValue)
                {
                    return Expression.OrElse(a,
                        Expression.AndAlso(Expression.MakeBinary(ExpressionType.Equal, l_major, r_major), b));
                }

                else
                {
                    return Expression.OrElse(a,
                            Expression.OrElse(Expression.AndAlso(Expression.MakeBinary(ExpressionType.Equal, l_major, r_major), b),
                                Expression.AndAlso(Expression.AndAlso(Expression.MakeBinary(ExpressionType.Equal, l_major, r_major), Expression.MakeBinary(ExpressionType.Equal, l_minor, r_minor)), c)
                            ));
                }

                //                left.Major > right.Major ||
                //                    (left.Major == right.Major) && (left.Minor > right.Minor) ||
                //                    (left.Major == right.Major) && (left.Minor == right.Minor) && (left.Patch > right.Patch);

            }

            else if (et == ExpressionType.OnesComplement)
            {

                return Expression.AndAlso(Expression.MakeBinary(ExpressionType.GreaterThan, l, r),
                    Expression.MakeBinary(ExpressionType.LessThanOrEqual, l,
                    Expression.MakeUnary(ExpressionType.OnesComplement, r, null)));
            }
            /*
            else if (!right.Patch.HasValue)
            {

                return Expression.AndAlso(a, b);
            }
            else
            {
                return Expression.AndAlso(a, Expression.AndAlso(b, c));
            }

                return null;
                Expression d1 = Expression.AndAlso(Expression.MakeBinary(ExpressionType.GreaterThan, r_patch, zero),
                    Expression.MakeBinary(ExpressionType.GreaterThan, l_major, r_major));
                Expression d2 = Expression.AndAlso(Expression.MakeBinary(ExpressionType.GreaterThan, r_minor, zero),
                    Expression.MakeBinary(ExpressionType.GreaterThan, l_major, r_major));

            }
            */
            else throw new ArgumentException("Unimplemented expression type: " + et.ToString() + ".");
        }

        public static Expression GetComparator(string et, SemanticVersion right, SemanticVersion left = null)
        {
            switch(et)
            {
                case "<": return GetComparator(ExpressionType.LessThan, right, left);
                case "<=": return GetComparator(ExpressionType.LessThanOrEqual, right, left);
                default: throw new ArgumentException("Unimplemented operator.");

            }
        }

        public static Expression GetComparator(SemanticVersion right, IEnumerable<Tuple<ExpressionType, SemanticVersion>> left)
        {
            Expression c = null;
            foreach (Tuple<ExpressionType, SemanticVersion> l in left)
            {
                if (c == null)
                {
                    c = GetComparator(l.Item1, right, l.Item2);
                }
                else
                {
                    c = Expression.AndAlso(c, GetComparator(l.Item1, right, l.Item2));
                }                                                                                                                                                                        
            }
            return c;
        }
    }
}
