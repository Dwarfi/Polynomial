using System;
using System.Collections.Generic;
using PolynomialObject.Exceptions;
using System.Linq;

namespace PolynomialObject
{
    public sealed class Polynomial
    {
        private List<PolynomialMember> poly_array;
        public Polynomial()
        {
            poly_array = null;
        }

        public Polynomial(PolynomialMember member)
        {
            poly_array = new List<PolynomialMember> { member };
        }

        public Polynomial(IEnumerable<PolynomialMember> members)
        {
            poly_array = members.ToList();
        }

        public Polynomial((double degree, double coefficient) member)
            : this(new PolynomialMember(member.degree, member.coefficient))
        {
        }
        public Polynomial(IEnumerable<(double degree, double coefficient)> members) :
            this(members.Select(member => new PolynomialMember(member.degree, member.coefficient)))
        {

        }

        /// <summary>
        /// The amount of not null polynomial members in polynomial
        /// </summary>
        public int Count
        {
            get
            {
                int count = 0;
                foreach (var item in poly_array)
                {
                    if (item != null)
                        count++;
                }
                return count;
            }
        }

        /// <summary>
        /// The biggest degree of polynomial member in polynomial
        /// </summary>
        public double Degree
        {
            get
            {
                return poly_array.Max(mem => mem.Degree);
            }
        }

        /// <summary>
        /// Adds new unique member to polynomial 
        /// </summary>
        /// <param name="member">The member to be added</param>
        /// <exception cref="PolynomialArgumentException">Throws when member to add with such degree already exist in polynomial</exception>
        /// <exception cref="PolynomialArgumentNullException">Throws when trying to member to add is null</exception>
        public void AddMember(PolynomialMember member)
        {

            if (poly_array == null)
                poly_array = new List<PolynomialMember>();
            if (member == null)
                throw new PolynomialArgumentNullException("null");
            if (poly_array.Any(same => same.Degree == member.Degree || member.Coefficient == 0.0))
                throw new PolynomialArgumentException();
            poly_array.Add(member);
        }

        /// <summary>
        /// Adds new unique member to polynomial from tuple
        /// </summary>
        /// <param name="member">The member to be added</param>
        /// <exception cref="PolynomialArgumentException">Throws when member to add with such degree already exist in polynomial</exception>
        public void AddMember((double degree, double coefficient) member)
        {
            if (poly_array.Any(same => same.Degree == member.degree || member.coefficient == 0.0))
                throw new PolynomialArgumentException();
            poly_array.Add(new PolynomialMember(member.degree, member.coefficient));
        }

        /// <summary>
        /// Removes member of specified degree
        /// </summary>
        /// <param name="degree">The degree of member to be deleted</param>
        /// <returns>True if member has been deleted</returns>
        public bool RemoveMember(double degree)
        {
            bool IsDeleted = false;
            if (ContainsMember(degree))
            {
                List<PolynomialMember> temp = poly_array;
                poly_array = poly_array.Where(deg => deg.Degree != degree).ToList();
                if (temp != poly_array)
                {
                    IsDeleted = true;
                }
                return IsDeleted;
            }
            else
                return IsDeleted;
        }

        /// <summary>
        /// Searches the polynomial for a method of specified degree
        /// </summary>
        /// <param name="degree">Degree of member</param>
        /// <returns>True if polynomial contains member</returns>
        public bool ContainsMember(double degree)
        {
            return poly_array.Any(member => member.Degree == degree);
        }

        /// <summary>
        /// Finds member of specified degree
        /// </summary>
        /// <param name="degree">Degree of member</param>
        /// <returns>Returns the found member or null</returns>
        public PolynomialMember Find(double degree)
        {
            PolynomialMember result = null;
            try
            {
                result = poly_array.Single(deg => deg.Degree == degree);
            }
            catch (Exception)
            {
                return null;
            }
            return result;

        }

        /// <summary>
        /// Gets and sets the coefficient of member with provided degree
        /// If there is no null member for searched degree - return 0 for get and add new member for set
        /// </summary>
        /// <param name="degree">The degree of searched member</param>
        /// <returns>Coefficient of found member</returns>
        public double this[double degree]
        {
            get
            {
                bool SearchResult = false;
                double coef = 0;
                foreach (var item in poly_array)
                {
                    if (item.Degree == degree)
                    {
                        SearchResult = true;
                        coef = item.Coefficient;
                        break;
                    }
                    if (!SearchResult)
                        coef = 0;
                }
                return coef;
            }
            set
            {
                bool SearchResult = false;
                foreach (var item in poly_array)
                {
                    if (item.Degree == degree)
                    {
                        if (value != 0)
                        {
                            item.Coefficient = value;
                            SearchResult = true;
                        }
                        if (value == 0)
                        {
                            RemoveMember(degree);
                            SearchResult = true;
                        }
                    }
                }
                if (!SearchResult && value != 0)
                {
                    AddMember(new PolynomialMember(degree, value));
                }
            }
        }
        /// <summary>
        /// Convert polynomial to array of included polynomial members 
        /// </summary>
        /// <returns>Array with not null polynomial members</returns>
        public PolynomialMember[] ToArray()
        {
            if (poly_array == null)
                poly_array = new List<PolynomialMember>();
            return poly_array.ToArray();
        }

        /// <summary>
        /// Adds two polynomials
        /// </summary>
        /// <param name="a">The first polynomial</param>
        /// <param name="b">The second polynomial</param>
        /// <returns>New polynomial after adding</returns>
        /// <exception cref="PolynomialArgumentNullException">Throws if either of provided polynomials is null</exception>
        public static Polynomial operator +(Polynomial a, Polynomial b)
        {
            if (a == null || b == null) throw new PolynomialArgumentNullException("Number is null");
            PolynomialMember[] polynom1 = a.ToArray();
            PolynomialMember[] polynom2 = b.ToArray();

            List<PolynomialMember> result = new List<PolynomialMember>(polynom1);
            foreach (var item in polynom2)
            {
                if (!a.ToArray().Any(o => o.Degree == item.Degree))
                    result.Add(item);
                else
                    result.Find(deg => deg.Degree == item.Degree).Coefficient += item.Coefficient;
            }
            return new Polynomial(result.Where(member => member.Coefficient != 0));
        }

        /// <summary>
        /// Subtracts two polynomials
        /// </summary>
        /// <param name="a">The first polynomial</param>
        /// <param name="b">The second polynomial</param>
        /// <returns>Returns new polynomial after subtraction</returns>
        /// <exception cref="PolynomialArgumentNullException">Throws if either of provided polynomials is null</exception>
        public static Polynomial operator -(Polynomial a, Polynomial b)
        {
            if (a == null || b == null) throw new PolynomialArgumentNullException("Number is null");
            PolynomialMember[] polynom1 = a.ToArray();
            PolynomialMember[] polynom2 = b.ToArray();

            List<PolynomialMember> result = new List<PolynomialMember>(polynom1);
            foreach (var item in polynom2)
            {
                if (!a.ToArray().Any(o => o.Degree == item.Degree))
                {
                    item.Coefficient *= -1;
                    result.Insert(0, item);
                }
                else
                    result.Find(deg => deg.Degree == item.Degree).Coefficient -= item.Coefficient;
            }
            return new Polynomial(result.Where(member => member.Coefficient != 0));
        }

        /// <summary>
        /// Multiplies two polynomials
        /// </summary>
        /// <param name="a">The first polynomial</param>
        /// <param name="b">The second polynomial</param>
        /// <returns>Returns new polynomial after multiplication</returns>
        /// <exception cref="PolynomialArgumentNullException">Throws if either of provided polynomials is null</exception>
        public static Polynomial operator *(Polynomial a, Polynomial b)
        {
            if (a == null) throw new PolynomialArgumentNullException("Number is null");
            if (b == null) throw new PolynomialArgumentNullException("Number is null");
            PolynomialMember[] poly1 = a.ToArray();
            PolynomialMember[] poly2 = b.ToArray();
            double coef = 0;
            double deg = 0;
            List<PolynomialMember> result = new List<PolynomialMember>();
            for (int i = 0; i < poly2.Length; i++)
            {
                for (int j = 0; j < poly1.Length; j++)
                {
                    coef = poly2[i].Coefficient * poly1[j].Coefficient;
                    deg = poly2[i].Degree + poly1[j].Degree;
                    result.Add(new PolynomialMember(deg, coef));
                }
            }
            for (int i = 0; i < result.Count - 1; i++)
            {
                if (result[i].Degree == result[i + 1].Degree)
                {
                    result[i].Coefficient += result[i + 1].Coefficient;
                    result.RemoveAt(i + 1);
                }
            }
            return new Polynomial(result.Where(member => member.Coefficient != 0));
        }

        /// <summary>
        /// Adds polynomial to polynomial
        /// </summary>
        /// <param name="polynomial">The polynomial to add</param>
        /// <returns>Returns new polynomial after adding</returns>
        /// <exception cref="PolynomialArgumentNullException">Throws if provided polynomial is null</exception>
        public Polynomial Add(Polynomial polynomial)
        {
            if (polynomial == null)
                throw new PolynomialArgumentNullException($"{polynomial} is null");
            PolynomialMember[] polynom1 = this.ToArray();
            PolynomialMember[] polynom2 = polynomial.ToArray();

            List<PolynomialMember> result = new List<PolynomialMember>(polynom1);

            foreach (var item in polynom2)
            {
                if (!ContainsMember(item.Degree))
                    result.Add(item);
                else
                    result.Find(deg => deg.Degree == item.Degree).Coefficient += item.Coefficient;
            }
            return new Polynomial(result.Where(member => member.Coefficient != 0));
        }

        /// <summary>
        /// Subtracts polynomial from polynomial
        /// </summary>
        /// <param name="polynomial">The polynomial to subtract</param>
        /// <returns>Returns new polynomial after subtraction</returns>
        /// <exception cref="PolynomialArgumentNullException">Throws if provided polynomial is null</exception>
        public Polynomial Subtraction(Polynomial polynomial)
        {
            if (polynomial == null)
                throw new PolynomialArgumentNullException($"{polynomial} is null");
            PolynomialMember[] polynom1 = this.ToArray();
            PolynomialMember[] polynom2 = polynomial.ToArray();
            List<PolynomialMember> result = new List<PolynomialMember>(polynom1);

            foreach (var item in polynom2)
            {
                if (!ContainsMember(item.Degree))
                {
                    item.Coefficient *= -1;
                    result.Add(item);
                }
                else
                    result.Find(deg => deg.Degree == item.Degree).Coefficient -= item.Coefficient;
            }
            return new Polynomial(result.Where(member => member.Coefficient != 0));
        }

        /// <summary>
        /// Multiplies polynomial with polynomial
        /// </summary>
        /// <param name="polynomial">The polynomial for multiplication </param>
        /// <returns>Returns new polynomial after multiplication</returns>
        /// <exception cref="PolynomialArgumentNullException">Throws if provided polynomial is null</exception>
        public Polynomial Multiply(Polynomial polynomial)
        {
            if (poly_array == null)
                poly_array = new List<PolynomialMember>();
            if (polynomial == null)
                throw new PolynomialArgumentNullException("Number is null");
            PolynomialMember[] poly1 = this.ToArray();
            PolynomialMember[] poly2 = polynomial.ToArray();
            double coef = 0;
            double deg = 0;
            List<PolynomialMember> result = new List<PolynomialMember>();
            for (int i = 0; i < poly2.Length; i++)
            {
                for (int j = 0; j < poly1.Length; j++)
                {
                    coef = poly2[i].Coefficient * poly1[j].Coefficient;
                    deg = poly2[i].Degree + poly1[j].Degree;
                    result.Add(new PolynomialMember(deg, coef));
                }
            }
            for (int i = 0; i < result.Count - 1; i++)
            {
                if (result[i].Degree == result[i + 1].Degree)
                {
                    result[i].Coefficient += result[i + 1].Coefficient;
                    result.RemoveAt(i + 1);
                }
            }
            return new Polynomial(result.Where(member => member.Coefficient != 0));
        }

        /// <summary>
        /// Adds polynomial and tuple
        /// </summary>
        /// <param name="a">The polynomial</param>
        /// <param name="b">The tuple</param>
        /// <returns>Returns new polynomial after adding</returns>
        public static Polynomial operator +(Polynomial a, (double degree, double coefficient) b)
        {
            Polynomial polynomial = new Polynomial(b);
            if (a != null && polynomial != null)
            {
                PolynomialMember[] polynom1 = a.ToArray();
                PolynomialMember[] polynom2 = polynomial.ToArray();

                List<PolynomialMember> result = new List<PolynomialMember>(polynom1);
                foreach (var item in polynom2)
                {
                    if (!a.ToArray().Any(o => o.Degree == item.Degree))
                        result.Add(item);
                    else
                        result.Find(deg => deg.Degree == item.Degree).Coefficient += item.Coefficient;
                }
                return new Polynomial(result.Where(member => member.Coefficient != 0));
            }
            
            throw new PolynomialArgumentNullException("Number is null");
        }

        /// <summary>
        /// Subtract polynomial and tuple
        /// </summary>
        /// <param name="a">The polynomial</param>
        /// <param name="b">The tuple</param>
        /// <returns>Returns new polynomial after subtraction</returns>
        public static Polynomial operator -(Polynomial a, (double degree, double coefficient) b)
        {
            Polynomial polynomial = new Polynomial(b);
            if (a != null && polynomial != null)
            {
                PolynomialMember[] polynom1 = a.ToArray();
                PolynomialMember[] polynom2 = polynomial.ToArray();

                List<PolynomialMember> result = new List<PolynomialMember>(polynom1);
                foreach (var item in polynom2)
                {
                    if (!a.ToArray().Any(o => o.Degree == item.Degree))
                    {
                        item.Coefficient *= -1;
                        result.Insert(0, item);
                    }
                    else
                        result.Find(deg => deg.Degree == item.Degree).Coefficient -= item.Coefficient;
                }
                return new Polynomial(result.Where(member => member.Coefficient != 0));
            }

            throw new PolynomialArgumentNullException("Number is null");
        }

        /// <summary>
        /// Multiplies polynomial and tuple
        /// </summary>
        /// <param name="a">The polynomial</param>
        /// <param name="b">The tuple</param>
        /// <returns>Returns new polynomial after multiplication</returns>
        public static Polynomial operator *(Polynomial a, (double degree, double coefficient) b)
        {
            Polynomial polynomial = new Polynomial(b);
            if (a == null) throw new PolynomialArgumentNullException("Number is null");
            if (polynomial != null)
            {
                PolynomialMember[] poly1 = a.ToArray();
                PolynomialMember[] poly2 = polynomial.ToArray();
                double coef = 0;
                double deg = 0;
                List<PolynomialMember> result = new List<PolynomialMember>();
                for (int i = 0; i < poly2.Length; i++)
                {
                    for (int j = 0; j < poly1.Length; j++)
                    {
                        coef = poly2[i].Coefficient * poly1[j].Coefficient;
                        deg = poly2[i].Degree + poly1[j].Degree;
                        result.Add(new PolynomialMember(deg, coef));
                    }
                }
                for (int i = 0; i < result.Count - 1; i++)
                {
                    if (result[i].Degree == result[i + 1].Degree)
                    {
                        result[i].Coefficient += result[i + 1].Coefficient;
                        result.RemoveAt(i + 1);
                    }
                }
                return new Polynomial(result.Where(member => member.Coefficient != 0));
            }

            throw new PolynomialArgumentNullException("Number is null");
        }

        /// <summary>
        /// Adds tuple to polynomial
        /// </summary>
        /// <param name="member">The tuple to add</param>
        /// <returns>Returns new polynomial after adding</returns>
        public Polynomial Add((double degree, double coefficient) member)
        {
            Polynomial polynomial = new Polynomial(member);
            if (polynomial != null)
            {
                PolynomialMember[] polynom1 = this.ToArray();
                PolynomialMember[] polynom2 = polynomial.ToArray();

                List<PolynomialMember> result = new List<PolynomialMember>(polynom1);

                foreach (var item in polynom2)
                {
                    if (!ContainsMember(item.Degree))
                        result.Add(item);
                    else
                        result.Find(deg => deg.Degree == item.Degree).Coefficient += item.Coefficient;
                }
                return new Polynomial(result.Where(member => member.Coefficient != 0));
            }

            throw new PolynomialArgumentNullException($"{polynomial} is null");
        }

        /// <summary>
        /// Subtracts tuple from polynomial
        /// </summary>
        /// <param name="member">The tuple to subtract</param>
        /// <returns>Returns new polynomial after subtraction</returns>
        public Polynomial Subtraction((double degree, double coefficient) member)
        {
            Polynomial polynomial = new Polynomial(member);
            if (polynomial != null)
            {
                PolynomialMember[] polynom1 = this.ToArray();
                PolynomialMember[] polynom2 = polynomial.ToArray();
                List<PolynomialMember> result = new List<PolynomialMember>(polynom1);

                foreach (var item in polynom2)
                {
                    if (!ContainsMember(item.Degree))
                    {
                        item.Coefficient *= -1;
                        result.Add(item);
                    }
                    else
                        result.Find(deg => deg.Degree == item.Degree).Coefficient -= item.Coefficient;
                }
                return new Polynomial(result.Where(member => member.Coefficient != 0));
            }

            throw new PolynomialArgumentNullException($"{polynomial} is null");
        }

        /// <summary>
        /// Multiplies tuple with polynomial
        /// </summary>
        /// <param name="member">The tuple for multiplication </param>
        /// <returns>Returns new polynomial after multiplication</returns>
        public Polynomial Multiply((double degree, double coefficient) member)
        {
            Polynomial polynomial = new Polynomial(member);
            if (poly_array == null)
                poly_array = new List<PolynomialMember>();
            if (polynomial != null)
            {
                PolynomialMember[] poly1 = this.ToArray();
                PolynomialMember[] poly2 = polynomial.ToArray();
                double coef = 0;
                double deg = 0;
                List<PolynomialMember> result = new List<PolynomialMember>();
                for (int i = 0; i < poly2.Length; i++)
                {
                    for (int j = 0; j < poly1.Length; j++)
                    {
                        coef = poly2[i].Coefficient * poly1[j].Coefficient;
                        deg = poly2[i].Degree + poly1[j].Degree;
                        result.Add(new PolynomialMember(deg, coef));
                    }
                }
                for (int i = 0; i < result.Count - 1; i++)
                {
                    if (result[i].Degree == result[i + 1].Degree)
                    {
                        result[i].Coefficient += result[i + 1].Coefficient;
                        result.RemoveAt(i + 1);
                    }
                }
                return new Polynomial(result.Where(member => member.Coefficient != 0));
            }

            throw new PolynomialArgumentNullException("Number is null");
        }
    }
}