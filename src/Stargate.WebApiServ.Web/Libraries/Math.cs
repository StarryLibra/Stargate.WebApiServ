using System;

namespace Stargate.WebApiServ.Web.Libraries
{
    // Reference: https://docs.microsoft.com/zh-cn/dotnet/csharp/codedoc

    /*
        The main Math class
        Contains all methods for performing basic math functions
    */
    /// <summary>
    /// The main <c>Math</c> class.
    /// Contains all methods for performing basic math functions.
    /// <list type="bullet">
    /// <item>
    /// <term>Add</term>
    /// <description>Addition Operation</description>
    /// </item>
    /// <item>
    /// <term>Subtract</term>
    /// <description>Subtraction Operation</description>
    /// </item>
    /// <item>
    /// <term>Multiply</term>
    /// <description>Multiplication Operation</description>
    /// </item>
    /// <item>
    /// <term>Divide</term>
    /// <description>Division Operation</description>
    /// </item>
    /// </list>
    /// </summary>
    /// <remarks>
    /// <para>This class can add, subtract, multiply and divide.</para>
    /// <para>These operations can be performed on both integers and doubles.</para>
    /// </remarks>
    public class Math
    {
        /// <value>Gets the value of PI.</value>
        public static double PI { get; }

        // Adds two integers and returns the result
        /// <summary>
        /// Adds two integers and returns the result.
        /// </summary>
        /// <returns>
        /// The sum of two integers.
        /// </returns>
        /// <example>
        /// <code>
        /// int c = Math.Add(4, 5);
        /// if (c > 10)
        /// {
        ///     Console.WriteLine(c);
        /// }
        /// </code>
        /// </example>
        /// <exception cref="System.OverflowException">Thrown when one parameter is max
        /// and the other is greater than 0.</exception>
        /// See <see cref="Math.Add(double, double)"/> to add doubles.
        /// <seealso cref="Math.Subtract(int, int)"/>
        /// <seealso cref="Math.Multiply(int, int)"/>
        /// <seealso cref="Math.Divide(int, int)"/>
        public static int Add(int a, int b)
        {
            // If any parameter is equal to the max value of an integer
            // and the other is greater than zero
            if ((a == int.MaxValue && b > 0) || (b == int.MaxValue && a > 0))
                throw new System.OverflowException();

            return a + b;
        }

        // Adds two doubles and returns the result
        /// <summary>
        /// Adds two doubles <paramref name="a"/> and <paramref name="b"/> and returns the result.
        /// </summary>
        /// <returns>
        /// The sum of two doubles.
        /// </returns>
        /// <exception cref="System.OverflowException">Thrown when one parameter is max
        /// and the other is greater than zero.</exception>
        /// See <see cref="Math.Add(int, int)"/> to add integers.
        /// <param name="a">A double precision number.</param>
        /// <param name="b">A double precision number.</param>

        public static double Add(double a, double b)
        {
            if ((a == double.MaxValue && b > 0) || (b == double.MaxValue && a > 0))
                throw new System.OverflowException();

            return a + b;
        }

        // Subtracts an integer from another and returns the result
        /// <include file='docs.xml' path='docs/members[@name="math"]/SubtractInt/*'/>
        public static int Subtract(int a, int b)
        {
            return a - b;
        }

        // Subtracts a double from another and returns the result
        /// <include file='docs.xml' path='docs/members[@name="math"]/SubtractDouble/*'/>
        public static double Subtract(double a, double b)
        {
            return a - b;
        }

        // Multiplies two integers and returns the result
        /// <include file='docs.xml' path='docs/members[@name="math"]/MultiplyInt/*'/>
        public static int Multiply(int a, int b)
        {
            return a * b;
        }

        // Multiplies two doubles and returns the result
        /// <include file='docs.xml' path='docs/members[@name="math"]/MultiplyDouble/*'/>
        public static double Multiply(double a, double b)
        {
            return a * b;
        }

        // Divides an integer by another and returns the result
        /// <summary>
        /// Divides an integer by another and returns the result.
        /// </summary>
        /// <returns>
        /// The division of two integers.
        /// </returns>
        /// <exception cref="System.DivideByZeroException">Thrown when a division by zero occurs.</exception>
        public static int Divide(int a, int b)
        {
            return a / b;
        }

        // Divides a double by another and returns the result
        /// <summary>
        /// Divides a double by another and returns the result.
        /// </summary>
        /// <returns>
        /// The division of two doubles.
        /// </returns>
        /// <exception cref="System.DivideByZeroException">Thrown when a division by zero occurs.</exception>
        public static double Divide(double a, double b)
        {
            return a / b;
        }
        
        /// <summary>
        /// Checks if an IComparable <typeparamref name="T"/> is greater than another.
        /// </summary>
        /// <typeparam name="T">A type that inherits from the IComparable interface.</typeparam>
        public static bool GreaterThan<T>(T a, T b) where T : IComparable
        {
            return a.CompareTo(b) > 0;
        }
    }

    /*
        The IMath interface
        The main Math class
        Contains all methods for performing basic math functions
    */
    /// <summary>
    /// This is the IMath interface.
    /// </summary>
    public interface IMath
    {
    }

    /// <inheritdoc/>
    public class MathInherit : IMath
    {
    }
}
