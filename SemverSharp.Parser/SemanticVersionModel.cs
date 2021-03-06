﻿using System;
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
                    int last;
                    if (!Int32.TryParse(this.PreRelease[this.PreRelease.Count - 1], out last) || last < 0)
                    {
                        this.PreRelease.Add(this.PreRelease.Count, "0");
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
                    int last;
                    if (!Int32.TryParse(this.PreRelease[this.PreRelease.Count - 1], out last) || last < 0)
                    {
                        this.PreRelease.Add(this.PreRelease.Count, "0");
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
            return Expression.Lambda<Func<bool>>(GetBinaryExpression(ExpressionType.Equal, left, right)).Compile().Invoke();
        }

        public static bool operator !=(SemanticVersion left, SemanticVersion right)
        {
            return Expression.Lambda<Func<bool>>(GetBinaryExpression(ExpressionType.NotEqual, left, right)).Compile().Invoke();
        }

        public static bool operator <(SemanticVersion left, SemanticVersion right)
        {
            return Expression.Lambda<Func<bool>>(GetBinaryExpression(ExpressionType.LessThan, left, right)).Compile().Invoke();
        }
       

        public static bool operator >(SemanticVersion left, SemanticVersion right)
        {
            return Expression.Lambda<Func<bool>>(GetBinaryExpression(ExpressionType.GreaterThan, left, right)).Compile().Invoke();
        }

             
        public static bool operator <=(SemanticVersion left, SemanticVersion right)
        {
            return Expression.Lambda<Func<bool>>(GetBinaryExpression(ExpressionType.LessThanOrEqual, left, right)).Compile().Invoke();
        }

        public static bool operator >=(SemanticVersion left, SemanticVersion right)
        {
            return Expression.Lambda<Func<bool>>(GetBinaryExpression(ExpressionType.GreaterThanOrEqual, left, right)).Compile().Invoke();
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
            return Expression.Lambda<Func<bool>>(GetBinaryExpression(ExpressionType.Equal, (SemanticVersion)obj, this)).Compile().Invoke();
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

        public static BinaryExpression GetBinaryExpression(ExpressionType et, SemanticVersion left, SemanticVersion right)
        {
            if (ReferenceEquals(right, null)) throw new ArgumentNullException("Right operand cannot be null.");
            if (ReferenceEquals(left, null)) throw new ArgumentNullException("Left operand cannot be null.");
            ConstantExpression zero = Expression.Constant(0, typeof(int));
            ConstantExpression l = Expression.Constant(left, typeof(SemanticVersion));
            ConstantExpression r = Expression.Constant(right, typeof(SemanticVersion));
            ConstantExpression l_major = Expression.Constant(left.Major, typeof(int));
            ConstantExpression l_minor = left.Minor.HasValue ? Expression.Constant(left.Minor, typeof(int)) : Expression.Constant(0, typeof(int));
            ConstantExpression l_patch = left.Patch.HasValue ? Expression.Constant(left.Patch, typeof(int)) : Expression.Constant(0, typeof(int));
            ConstantExpression l_prerelease = Expression.Constant(left.PreRelease, typeof(PreRelease));
            ConstantExpression r_major = Expression.Constant(right.Major, typeof(int));
            ConstantExpression r_minor = right.Minor.HasValue ? Expression.Constant(right.Minor, typeof(int)) : zero;
            ConstantExpression r_patch = right.Patch.HasValue ? Expression.Constant(right.Patch, typeof(int)) : zero;
            ConstantExpression r_prerelease = Expression.Constant(right.PreRelease, typeof(PreRelease));
            BinaryExpression a = Expression.MakeBinary(et, l_major, r_major);
            BinaryExpression b = Expression.MakeBinary(et, l_minor, r_minor);
            BinaryExpression c = Expression.MakeBinary(et, l_patch, r_patch);
            BinaryExpression d = Expression.MakeBinary(et, l_prerelease, r_prerelease);
            if (et == ExpressionType.Equal || et == ExpressionType.NotEqual)
            {
                return Expression.AndAlso(Expression.AndAlso(a, Expression.AndAlso(b, c)), d);
            }
            else if (et == ExpressionType.LessThan || et == ExpressionType.GreaterThan)
            {
                if (!ReferenceEquals(right.PreRelease, null) && !ReferenceEquals(left.PreRelease, null))//Major + minor + patch + prerelease
                {
                    return Expression.AndAlso(Expression.AndAlso(Expression.AndAlso(Expression.MakeBinary(ExpressionType.Equal, l_major, r_major),
                        Expression.MakeBinary(ExpressionType.Equal, l_minor, r_minor)), Expression.MakeBinary(ExpressionType.Equal, l_patch, r_patch)), d);
                }
                else
                {
                    return Expression.OrElse(a,
                                Expression.OrElse(//  or major = major and b
                                    Expression.OrElse(Expression.AndAlso(Expression.MakeBinary(ExpressionType.Equal, l_major, r_major), b),
                                        Expression.AndAlso(Expression.AndAlso(Expression.MakeBinary(ExpressionType.Equal, l_major, r_major),
                                        Expression.MakeBinary(ExpressionType.Equal, l_minor, r_minor)), c)),
                                    Expression.AndAlso(Expression.AndAlso(Expression.AndAlso(Expression.MakeBinary(ExpressionType.Equal, l_major, r_major),
                                    Expression.MakeBinary(ExpressionType.Equal, l_minor, r_minor)), Expression.MakeBinary(ExpressionType.Equal, l_patch, r_patch)), d))
                            );
                }
                /*
                if (!right.Minor.HasValue) //Major only
                {
                    return a;
                }
                else if (!right.Patch.HasValue) //Major + minor
                {
                    return Expression.OrElse(a,
                        Expression.AndAlso(Expression.MakeBinary(ExpressionType.Equal, l_major, r_major), b));
                }

                else if (ReferenceEquals(right.PreRelease, null) && ReferenceEquals(left.PreRelease, null))//Major + minor + patch only
                {
                    return Expression.OrElse(a,
                            Expression.OrElse(Expression.AndAlso(Expression.MakeBinary(ExpressionType.Equal, l_major, r_major), b),
                                Expression.AndAlso(Expression.AndAlso(Expression.MakeBinary(ExpressionType.Equal, l_major, r_major), 
                                Expression.MakeBinary(ExpressionType.Equal, l_minor, r_minor)), c)
                            ));
                }

                else if (!ReferenceEquals(right.PreRelease, null) && !ReferenceEquals(left.PreRelease, null))//Major + minor + patch + prerelease
                {
                    return Expression.AndAlso(Expression.AndAlso(Expression.AndAlso(Expression.MakeBinary(ExpressionType.Equal, l_major, r_major),
                        Expression.MakeBinary(ExpressionType.Equal, l_minor, r_minor)), Expression.MakeBinary(ExpressionType.Equal, l_patch, r_patch)), d);
                }

                else //Major + minor + patch + 1 preonly
                {
                    return Expression.OrElse(a,
                            Expression.OrElse(//  or major = major and b
                                Expression.OrElse(Expression.AndAlso(Expression.MakeBinary(ExpressionType.Equal, l_major, r_major), b),
                                    Expression.AndAlso(Expression.AndAlso(Expression.MakeBinary(ExpressionType.Equal, l_major, r_major), 
                                    Expression.MakeBinary(ExpressionType.Equal, l_minor, r_minor)), c)),         
                                Expression.AndAlso(Expression.AndAlso(Expression.AndAlso(Expression.MakeBinary(ExpressionType.Equal, l_major, r_major),
                                Expression.MakeBinary(ExpressionType.Equal, l_minor, r_minor)), Expression.MakeBinary(ExpressionType.Equal, l_patch, r_patch)), d))
                        );
                }
                //                left.Major > right.Major ||
                //                    (left.Major == right.Major) && (left.Minor > right.Minor) ||
                //                    (left.Major == right.Major) && (left.Minor == right.Minor) && (left.Patch > right.Patch);
                */
 
            }

            else if (et == ExpressionType.LessThanOrEqual)
            {
                return Expression.OrElse(GetBinaryExpression(ExpressionType.LessThan, left, right), GetBinaryExpression(ExpressionType.Equal, left, right));
            }

            else if (et == ExpressionType.GreaterThanOrEqual)
            {
                return Expression.OrElse(GetBinaryExpression(ExpressionType.GreaterThan, left, right), GetBinaryExpression(ExpressionType.Equal, left, right));
            }


            else if (et == ExpressionType.OnesComplement)
            {
                return Expression.AndAlso(Expression.MakeBinary(ExpressionType.GreaterThan, l, r),
                    Expression.MakeBinary(ExpressionType.LessThanOrEqual, l, Expression.MakeUnary(ExpressionType.OnesComplement, r, null)));
            }
                       
            else throw new ArgumentException("Unimplemented expression type: " + et.ToString() + ".");
        }

        public static BinaryExpression GetBinaryExpression(SemanticVersion left, ComparatorSet right)
        {
            if (right.Count() == 0)
            {
                return GetBinaryExpression(ExpressionType.Equal, left, left);
            }
            else
            {
                BinaryExpression c = null;
                foreach (Comparator r in right)
                {
                    if (c == null)
                    {
                        c = GetBinaryExpression(r.Operator, left, r.Version);
                    }
                    else
                    {
                        c = Expression.AndAlso(c, GetBinaryExpression(r.Operator, left, r.Version));
                    }
                }
                return c;
            }
        }
    
        public static bool InvokeBinaryExpression(BinaryExpression be)
        {
            return Expression.Lambda<Func<bool>>(be).Compile().Invoke();
        }

        public static bool RangeIntersect(ExpressionType left_operator, SemanticVersion left, ExpressionType right_operator, SemanticVersion right)
        {
            if (left_operator != ExpressionType.LessThan && left_operator != ExpressionType.LessThanOrEqual &&
                    left_operator != ExpressionType.GreaterThan && left_operator != ExpressionType.GreaterThanOrEqual
                    && left_operator != ExpressionType.Equal)
                throw new ArgumentException("Unsupported left operator expression type " + left_operator.ToString() + ".");
            if (right_operator != ExpressionType.LessThan && right_operator != ExpressionType.LessThanOrEqual &&
                   right_operator != ExpressionType.GreaterThan && right_operator != ExpressionType.GreaterThanOrEqual
                   && right_operator != ExpressionType.Equal)
                throw new ArgumentException("Unsupported left operator expression type " + left_operator.ToString() + ".");

            if (left_operator == ExpressionType.Equal)
            {
                return InvokeBinaryExpression(GetBinaryExpression(right_operator, left, right));
            }
            else if (right_operator == ExpressionType.Equal)
            {
                return InvokeBinaryExpression(GetBinaryExpression(left_operator, right, left));
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
        }
       
        public static bool RangeIntersect(string left, string right)
        {
            Comparator l = Grammar.Comparator.Parse(left);
            Comparator r = Grammar.Comparator.Parse(right);
            return RangeIntersect(l.Operator, l.Version, r.Operator, r.Version);
        }

        public static bool Satisfies(SemanticVersion v, ComparatorSet s)
        {
            return InvokeBinaryExpression(GetBinaryExpression(v, s));
        }

    }
}
