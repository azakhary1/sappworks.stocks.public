
namespace Sappworks.Stocks.Numerics
{
    public static class Precision
    {
        // 2^-24
        private const float FLOAT_EPSILON = 0.0000000596046448f;

        // 2^-53
        private const double DOUBLE_EPSILON = 0.00000000000000011102230246251565d;

        public static bool AlmostEquals(this double a, double b, double epsilon = DOUBLE_EPSILON)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            if (a == b)
            {
                return true;
            }
            // ReSharper restore CompareOfFloatsByEqualityOperator

            return (System.Math.Abs(a - b) < epsilon);
        }

        public static bool AlmostEquals(this float a, float b, float epsilon = FLOAT_EPSILON)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            if (a == b)
            {
                return true;
            }
            // ReSharper restore CompareOfFloatsByEqualityOperator

            return (System.Math.Abs(a - b) < epsilon);
        }
    }
}
