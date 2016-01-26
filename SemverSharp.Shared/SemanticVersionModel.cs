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

        #region Constructors
        public SemanticVersion(int? major, int? minor = null, int? patch = null, string prerelease = "", string build = "")
        {
            if (!major.HasValue && minor.HasValue) throw new ArgumentNullException("Major component cannot be null if minor is non-null.");
            if (!minor.HasValue && patch.HasValue) throw new ArgumentNullException("Minor component cannot be null if patch is non-null.");
            if (!string.IsNullOrEmpty(prerelease) && !patch.HasValue) throw new ArgumentNullException("Patch component cannot be null if pre-release is non-null.");
            if (!string.IsNullOrEmpty(build) && !patch.HasValue) throw new ArgumentNullException("Patch component cannot be null if pre-release is non-null.");
            this.Major = major;
            this.Minor = minor;
            this.Patch = patch;
            if (!string.IsNullOrEmpty(prerelease))
            {                
                if (Grammar.PreRelease.Parse(prerelease).Count() > 0)
                {
                    this.PreRelease = new PreRelease();
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
        
        public SemanticVersion(string v) : this(Grammar.SemanticVersionIdentifier.Parse(v)) { }

        #endregion

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
            return Expression.Lambda<Func<bool>>(GetComparator(ExpressionType.Equal, left, right)).Compile().Invoke();
        }

        public static bool operator !=(SemanticVersion left, SemanticVersion right)
        {
            return Expression.Lambda<Func<bool>>(GetComparator(ExpressionType.NotEqual, left, right)).Compile().Invoke();
        }

        public static bool operator <(SemanticVersion left, SemanticVersion right)
        {
            return Expression.Lambda<Func<bool>>(GetComparator(ExpressionType.LessThan, left, right)).Compile().Invoke();
        }
       

        public static bool operator >(SemanticVersion left, SemanticVersion right)
        {
            return Expression.Lambda<Func<bool>>(GetComparator(ExpressionType.GreaterThan, left, right)).Compile().Invoke();
        }

             
        public static bool operator <=(SemanticVersion left, SemanticVersion right)
        {
            return Expression.Lambda<Func<bool>>(GetComparator(ExpressionType.LessThanOrEqual, left, right)).Compile().Invoke();
        }

        public static bool operator >=(SemanticVersion left, SemanticVersion right)
        {
            return Expression.Lambda<Func<bool>>(GetComparator(ExpressionType.GreaterThanOrEqual, left, right)).Compile().Invoke();
        }

        public static SemanticVersion operator ++(SemanticVersion s)
        {
            if (s.PreRelease != null && s.PreRelease.Count > 0)
            {
                ++s.PreRelease;
                return s;
            }
            else if (s.Patch.HasValue)
            {
                ++s.Patch;
                return s;
            }
            else if (s.Minor.HasValue)
            {
                ++s.Minor;
                return s;
            }
            else
            {
                ++s.Major;
                return s;
            }
        }

        public static SemanticVersion operator -- (SemanticVersion s)
        {
            if (s.PreRelease != null && s.PreRelease.Count > 0)
            {
                --s.PreRelease;
                return s;
            }
            else if (s.Patch.HasValue)
            {
                --s.Patch;
                return s;
            }
            else if (s.Minor.HasValue)
            {
                --s.Minor;
                return s;
            }
            else
            {
                --s.Major;
                return s;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return Expression.Lambda<Func<bool>>(GetComparator(ExpressionType.Equal, (SemanticVersion)obj, this)).Compile().Invoke();
        }

        public override string ToString()
        {
            string result = this.Major.ToString();
            if (this.Minor.HasValue)
            {
                result = result + "." + this.Minor.ToString();
            }
            if (this.Patch.HasValue)
            {
                result = result + "." + this.Patch.ToString();
            }

            if (this.PreRelease != null && this.PreRelease.Count() > 0)
            {
                result = result + "." + this.PreRelease.ToString();
            }
            if (this.Build != null)
            {
                result = result + "." + this.Build.ToString();
            }

            return result;

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

        public static BinaryExpression GetComparator(ExpressionType et, SemanticVersion left, SemanticVersion right)
        {
            if (ReferenceEquals(right, null)) throw new ArgumentNullException("Right operand cannot be null.");
            if (ReferenceEquals(left, null)) throw new ArgumentNullException("Left operand cannot be null.");
            if (!left.Minor.HasValue) //Major component only

            {
                left.Minor = 0;
                left.Patch = 0;
            }
            else if (!left.Patch.HasValue) //Major + minor component only
            {
                left.Patch = 0;

            } 
            else if (left.PreRelease == null)
            {
                left.PreRelease = new PreRelease();
            }

            ConstantExpression zero = Expression.Constant(0, typeof(int));
            ConstantExpression l = Expression.Constant(left, typeof(SemanticVersion));
            ConstantExpression r = Expression.Constant(right, typeof(SemanticVersion));
            ConstantExpression l_major = Expression.Constant(left.Major, typeof(int));
            ConstantExpression l_minor = left.Minor.HasValue ? Expression.Constant(left.Minor, typeof(int)) : Expression.Constant(0, typeof(int));
            ConstantExpression l_patch = right.Patch.HasValue ? Expression.Constant(left.Patch, typeof(int)) : Expression.Constant(0, typeof(int));
            ConstantExpression l_prerelease = Expression.Constant(left.PreRelease, typeof(PreRelease));
            ConstantExpression r_major = Expression.Constant(right.Major, typeof(int));
            ConstantExpression r_minor = right.Minor.HasValue ? Expression.Constant(right.Minor, typeof(int)) : Expression.Constant(0, typeof(int));
            ConstantExpression r_patch = right.Patch.HasValue ? Expression.Constant(right.Patch, typeof(int)) : Expression.Constant(0, typeof(int));
            ConstantExpression r_prerelease = Expression.Constant(right.PreRelease, typeof(PreRelease));

            if (et == ExpressionType.Equal || et == ExpressionType.NotEqual)
            {
                BinaryExpression a = Expression.MakeBinary(et, l_major, r_major);
                BinaryExpression b = Expression.MakeBinary(et, l_minor, r_minor);
                BinaryExpression c = Expression.MakeBinary(et, l_patch, r_patch);
                BinaryExpression d = Expression.MakeBinary(et, l_prerelease, r_prerelease);
                return Expression.AndAlso(Expression.AndAlso(a, Expression.AndAlso(b, c)), d);
                /*
                if (!right.Minor.HasValue)
                {
                    return a;
                }
                else if (!right.Patch.HasValue)
                {

                    return Expression.AndAlso(a, b);
                }
                else if (right.PreRelease == null)
                {
                    return Expression.AndAlso(a, Expression.AndAlso(b, c));
                }
                else//one pre-PreRelease not null
                {
                    return Expression.AndAlso(Expression.AndAlso(a, Expression.AndAlso(b, c)), d);
                }
                */
            }
            else if (et == ExpressionType.LessThan || et == ExpressionType.GreaterThan)
            {
                BinaryExpression a = Expression.MakeBinary(et, l_major, r_major);
                BinaryExpression b = right.Minor.HasValue ? Expression.MakeBinary(et, l_minor, r_minor) : null;
                BinaryExpression c = right.Patch.HasValue ? Expression.MakeBinary(et, l_patch, r_patch) : null;
                BinaryExpression d = Expression.MakeBinary(et, l_prerelease, r_prerelease);

                if (!right.Minor.HasValue) //Major only
                {
                    return a;
                }
                else if (!right.Patch.HasValue) //Major + minor
                {
                    return Expression.OrElse(a,
                        Expression.AndAlso(Expression.MakeBinary(ExpressionType.Equal, l_major, r_major), b));
                }
                
                /*
                else if (right.PreRelease == null) //Major + minor + patch
                {
                    return Expression.OrElse(a,
                            Expression.OrElse(Expression.AndAlso(Expression.MakeBinary(ExpressionType.Equal, l_major, r_major), b),
                                Expression.AndAlso(Expression.AndAlso(Expression.MakeBinary(ExpressionType.Equal, l_major, r_major), Expression.MakeBinary(ExpressionType.Equal, l_minor, r_minor)), c)
                            ));
                }
                */
                //                left.Major > right.Major ||
                //                    (left.Major == right.Major) && (left.Minor > right.Minor) ||
                //                    (left.Major == right.Major) && (left.Minor == right.Minor) && (left.Patch > right.Patch);
                else //Major + minor + patch + prerelease
                {                    
                    return Expression.OrElse(
                     (Expression.OrElse(a,
                        Expression.OrElse(Expression.AndAlso(Expression.MakeBinary(ExpressionType.Equal, l_major, r_major), b),
                            Expression.AndAlso(Expression.AndAlso(Expression.MakeBinary(ExpressionType.Equal, l_major, r_major), Expression.MakeBinary(ExpressionType.Equal, l_minor, r_minor)), c)
                         ))),
                     Expression.AndAlso(Expression.AndAlso(Expression.AndAlso(Expression.MakeBinary(ExpressionType.Equal, l_major, r_major),
                        Expression.MakeBinary(ExpressionType.Equal, l_minor, r_minor)), Expression.MakeBinary(ExpressionType.Equal, l_patch, r_patch)), d));
                }
            }
            
            else if (et == ExpressionType.LessThanOrEqual)
            {
                return Expression.OrElse(GetComparator(ExpressionType.LessThan, left, right), GetComparator(ExpressionType.Equal, left, right));
            }

            else if (et == ExpressionType.GreaterThanOrEqual)
            {
                return Expression.OrElse(GetComparator(ExpressionType.GreaterThan, left, right), GetComparator(ExpressionType.Equal, left, right));
            }


            else if (et == ExpressionType.OnesComplement)
            {
                return Expression.AndAlso(Expression.MakeBinary(ExpressionType.GreaterThan, l, r),
                    Expression.MakeBinary(ExpressionType.LessThanOrEqual, l, Expression.MakeUnary(ExpressionType.OnesComplement, r, null)));
            }
                       
            else throw new ArgumentException("Unimplemented expression type: " + et.ToString() + ".");
        }

        public static BinaryExpression GetComparator(SemanticVersion left, IEnumerable<Tuple<ExpressionType, SemanticVersion>> right)
        {
            BinaryExpression c = null;
            foreach (Tuple<ExpressionType, SemanticVersion> r in right)
            {
                if (c == null)
                {
                    c = GetComparator(r.Item1, left, r.Item2);
                }
                else
                {
                    c = Expression.AndAlso(c, GetComparator(r.Item1, left, r.Item2));
                }                                                                                                                                                                        
            }
            return c;
        }

        public static Expression GetComparator(string et, SemanticVersion left, SemanticVersion right)
        {
            switch (et)
            {
                case "<": return GetComparator(ExpressionType.LessThan, left, right);
                case "<=": return GetComparator(ExpressionType.LessThanOrEqual, left, right);
                default: throw new ArgumentException("Unimplemented operator.");

            }
        }


        public static bool InvokeComparator(BinaryExpression be)
        {
            return Expression.Lambda<Func<bool>>(be).Compile().Invoke();
        }

        public static bool RangeIntersect(ExpressionType left_operator, SemanticVersion left, ExpressionType right_operator, SemanticVersion right)
        {
            if (left_operator != ExpressionType.LessThan && left_operator != ExpressionType.LessThanOrEqual &&
                    left_operator != ExpressionType.GreaterThan && left_operator != ExpressionType.GreaterThanOrEqual
                    && left_operator != ExpressionType.Equal)
                throw new ArgumentException("Unsupported left operator expression type " + left_operator.ToString() + ".");
            if (left_operator == ExpressionType.Equal)
            {
                return InvokeComparator(GetComparator(right_operator, left, right));
            }
            else if (right_operator == ExpressionType.Equal)
            {
                return InvokeComparator(GetComparator(left_operator, left, right));
            }

            if ((left_operator == ExpressionType.LessThan || left_operator == ExpressionType.LessThanOrEqual)
                && (right_operator == ExpressionType.LessThan || right_operator == ExpressionType.LessThanOrEqual))
            {
                return true;
            }
            else if ((left_operator == ExpressionType.GreaterThan || left_operator == ExpressionType.GreaterThanOrEqual)
                && (right_operator == ExpressionType.GreaterThan || right_operator == ExpressionType.GreaterThanOrEqual))
            {
                return true;
            }

            else if ((left_operator == ExpressionType.LessThanOrEqual) && (right_operator == ExpressionType.GreaterThanOrEqual))
            {
                return right <= left;
            }

            else if ((left_operator == ExpressionType.GreaterThanOrEqual) && (right_operator == ExpressionType.LessThanOrEqual))
            {
                return right >= left;
            }


            else if ((left_operator == ExpressionType.LessThan || left_operator == ExpressionType.LessThanOrEqual)
                && (right_operator == ExpressionType.GreaterThan || right_operator == ExpressionType.GreaterThanOrEqual))
            {
                return right < left;
            }

            else
            {
                return right > left;
            }

            /*
            if (right < left)
            {
                e1 = right;
                op = left_operator;
                e2 = left;                
            }
            else
            {
                e1 = left;
                op = right_operator;
                e2 = right;
            }

            if (left_operator == ExpressionType.LessThan)
            {
                e1 = --e1;
            }


            else if (left_operator == ExpressionType.GreaterThan)
            {
                e1 = ++left;

            }
            */
            //return Expression.Lambda<Func<bool>>(GetComparator(op, e1, e2)).Compile().Invoke();

        }

        public static bool RangeIntersect(string left, string right)
        {
            Tuple<ExpressionType, SemanticVersion> l = Grammar.RangeExpression.Parse(left);
            Tuple<ExpressionType, SemanticVersion> r = Grammar.RangeExpression.Parse(right);
            return RangeIntersect(l.Item1, l.Item2, r.Item1, r.Item2);
        }

    }
}
