// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Tests;
using Xunit;

namespace System.Numerics.Tests
{
    public sealed class Vector4Tests
    {
        private const int ElementCount = 4;

        /// <summary>Verifies that two <see cref="Vector4" /> values are equal, within the <paramref name="variance" />.</summary>
        /// <param name="expected">The expected value</param>
        /// <param name="actual">The value to be compared against</param>
        /// <param name="variance">The total variance allowed between the expected and actual results.</param>
        /// <exception cref="EqualException">Thrown when the values are not equal</exception>
        internal static void AssertEqual(Vector4 expected, Vector4 actual, Vector4 variance)
        {
            AssertExtensions.Equal(expected.X, actual.X, variance.X);
            AssertExtensions.Equal(expected.Y, actual.Y, variance.Y);
            AssertExtensions.Equal(expected.Z, actual.Z, variance.Z);
            AssertExtensions.Equal(expected.W, actual.W, variance.W);
        }

        [Fact]
        public void Vector4MarshalSizeTest()
        {
            Assert.Equal(16, Marshal.SizeOf<Vector4>());
            Assert.Equal(16, Marshal.SizeOf<Vector4>(new Vector4()));
        }

        [Theory]
        [InlineData(0.0f, 1.0f, 0.0f, 1.0f)]
        [InlineData(1.0f, 0.0f, 1.0f, 0.0f)]
        [InlineData(3.1434343f, 1.1234123f, 0.1234123f, -0.1234123f)]
        [InlineData(1.0000001f, 0.0000001f, 2.0000001f, 0.0000002f)]
        public void Vector4IndexerGetTest(float x, float y, float z, float w)
        {
            var vector = new Vector4(x, y, z, w);

            Assert.Equal(x, vector[0]);
            Assert.Equal(y, vector[1]);
            Assert.Equal(z, vector[2]);
            Assert.Equal(w, vector[3]);
        }

        [Theory]
        [InlineData(0.0f, 1.0f, 0.0f, 1.0f)]
        [InlineData(1.0f, 0.0f, 1.0f, 0.0f)]
        [InlineData(3.1434343f, 1.1234123f, 0.1234123f, -0.1234123f)]
        [InlineData(1.0000001f, 0.0000001f, 2.0000001f, 0.0000002f)]
        public void Vector4IndexerSetTest(float x, float y, float z, float w)
        {
            var vector = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);

            vector[0] = x;
            vector[1] = y;
            vector[2] = z;
            vector[3] = w;

            Assert.Equal(x, vector[0]);
            Assert.Equal(y, vector[1]);
            Assert.Equal(z, vector[2]);
            Assert.Equal(w, vector[3]);
        }

        [Fact]
        public void Vector4CopyToTest()
        {
            Vector4 v1 = new Vector4(2.5f, 2.0f, 3.0f, 3.3f);

            float[] a = new float[5];
            float[] b = new float[4];

            Assert.Throws<NullReferenceException>(() => v1.CopyTo(null, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => v1.CopyTo(a, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => v1.CopyTo(a, a.Length));
            Assert.Throws<ArgumentException>(() => v1.CopyTo(a, a.Length - 2));

            v1.CopyTo(a, 1);
            v1.CopyTo(b);
            Assert.Equal(0.0f, a[0]);
            Assert.Equal(2.5f, a[1]);
            Assert.Equal(2.0f, a[2]);
            Assert.Equal(3.0f, a[3]);
            Assert.Equal(3.3f, a[4]);
            Assert.Equal(2.5f, b[0]);
            Assert.Equal(2.0f, b[1]);
            Assert.Equal(3.0f, b[2]);
            Assert.Equal(3.3f, b[3]);
        }

        [Fact]
        public void Vector4CopyToSpanTest()
        {
            Vector4 vector = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
            Span<float> destination = new float[4];

            Assert.Throws<ArgumentException>(() => vector.CopyTo(new Span<float>(new float[3])));
            vector.CopyTo(destination);

            Assert.Equal(1.0f, vector.X);
            Assert.Equal(2.0f, vector.Y);
            Assert.Equal(3.0f, vector.Z);
            Assert.Equal(4.0f, vector.W);
            Assert.Equal(vector.X, destination[0]);
            Assert.Equal(vector.Y, destination[1]);
            Assert.Equal(vector.Z, destination[2]);
            Assert.Equal(vector.W, destination[3]);
        }

        [Fact]
        public void Vector4TryCopyToTest()
        {
            Vector4 vector = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
            Span<float> destination = new float[4];

            Assert.False(vector.TryCopyTo(new Span<float>(new float[3])));
            Assert.True(vector.TryCopyTo(destination));

            Assert.Equal(1.0f, vector.X);
            Assert.Equal(2.0f, vector.Y);
            Assert.Equal(3.0f, vector.Z);
            Assert.Equal(4.0f, vector.W);
            Assert.Equal(vector.X, destination[0]);
            Assert.Equal(vector.Y, destination[1]);
            Assert.Equal(vector.Z, destination[2]);
            Assert.Equal(vector.W, destination[3]);
        }

        [Fact]
        public void Vector4GetHashCodeTest()
        {
            Vector4 v1 = new Vector4(2.5f, 2.0f, 3.0f, 3.3f);
            Vector4 v2 = new Vector4(2.5f, 2.0f, 3.0f, 3.3f);
            Vector4 v3 = new Vector4(2.5f, 2.0f, 3.0f, 3.3f);
            Vector4 v5 = new Vector4(3.3f, 3.0f, 2.0f, 2.5f);
            Assert.Equal(v1.GetHashCode(), v1.GetHashCode());
            Assert.Equal(v1.GetHashCode(), v2.GetHashCode());
            Assert.NotEqual(v1.GetHashCode(), v5.GetHashCode());
            Assert.Equal(v1.GetHashCode(), v3.GetHashCode());
            Vector4 v4 = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
            Vector4 v6 = new Vector4(1.0f, 0.0f, 0.0f, 0.0f);
            Vector4 v7 = new Vector4(0.0f, 1.0f, 0.0f, 0.0f);
            Vector4 v8 = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            Vector4 v9 = new Vector4(1.0f, 1.0f, 0.0f, 0.0f);
            Assert.NotEqual(v4.GetHashCode(), v6.GetHashCode());
            Assert.NotEqual(v4.GetHashCode(), v7.GetHashCode());
            Assert.NotEqual(v4.GetHashCode(), v8.GetHashCode());
            Assert.NotEqual(v7.GetHashCode(), v6.GetHashCode());
            Assert.NotEqual(v8.GetHashCode(), v6.GetHashCode());
            Assert.NotEqual(v8.GetHashCode(), v7.GetHashCode());
            Assert.NotEqual(v9.GetHashCode(), v7.GetHashCode());
        }

        [Fact]
        public void Vector4ToStringTest()
        {
            string separator = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
            CultureInfo enUsCultureInfo = new CultureInfo("en-US");

            Vector4 v1 = new Vector4(2.5f, 2.0f, 3.0f, 3.3f);

            string v1str = v1.ToString();
            string expectedv1 = string.Format(CultureInfo.CurrentCulture
                , "<{1:G}{0} {2:G}{0} {3:G}{0} {4:G}>"
                , separator, 2.5, 2, 3, 3.3);
            Assert.Equal(expectedv1, v1str);

            string v1strformatted = v1.ToString("c", CultureInfo.CurrentCulture);
            string expectedv1formatted = string.Format(CultureInfo.CurrentCulture
                , "<{1:c}{0} {2:c}{0} {3:c}{0} {4:c}>"
                , separator, 2.5, 2, 3, 3.3);
            Assert.Equal(expectedv1formatted, v1strformatted);

            string v2strformatted = v1.ToString("c", enUsCultureInfo);
            string expectedv2formatted = string.Format(enUsCultureInfo
                , "<{1:c}{0} {2:c}{0} {3:c}{0} {4:c}>"
                , enUsCultureInfo.NumberFormat.NumberGroupSeparator, 2.5, 2, 3, 3.3);
            Assert.Equal(expectedv2formatted, v2strformatted);

            string v3strformatted = v1.ToString("c");
            string expectedv3formatted = string.Format(CultureInfo.CurrentCulture
                , "<{1:c}{0} {2:c}{0} {3:c}{0} {4:c}>"
                , separator, 2.5, 2, 3, 3.3);
            Assert.Equal(expectedv3formatted, v3strformatted);
        }

        // A test for DistanceSquared (Vector4f, Vector4f)
        [Fact]
        public void Vector4DistanceSquaredTest()
        {
            Vector4 a = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
            Vector4 b = new Vector4(5.0f, 6.0f, 7.0f, 8.0f);

            float expected = 64.0f;
            float actual;

            actual = Vector4.DistanceSquared(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.DistanceSquared did not return the expected value.");
        }

        // A test for Distance (Vector4f, Vector4f)
        [Fact]
        public void Vector4DistanceTest()
        {
            Vector4 a = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
            Vector4 b = new Vector4(5.0f, 6.0f, 7.0f, 8.0f);

            float expected = 8.0f;
            float actual;

            actual = Vector4.Distance(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.Distance did not return the expected value.");
        }

        // A test for Distance (Vector4f, Vector4f)
        // Distance from the same point
        [Fact]
        public void Vector4DistanceTest1()
        {
            Vector4 a = new Vector4(new Vector2(1.051f, 2.05f), 3.478f, 1.0f);
            Vector4 b = new Vector4(new Vector3(1.051f, 2.05f, 3.478f), 0.0f);
            b.W = 1.0f;

            float actual = Vector4.Distance(a, b);
            Assert.Equal(0.0f, actual);
        }

        // A test for Dot (Vector4f, Vector4f)
        [Fact]
        public void Vector4DotTest()
        {
            Vector4 a = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
            Vector4 b = new Vector4(5.0f, 6.0f, 7.0f, 8.0f);

            float expected = 70.0f;
            float actual;

            actual = Vector4.Dot(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.Dot did not return the expected value.");
        }

        // A test for Dot (Vector4f, Vector4f)
        // Dot test for perpendicular vector
        [Fact]
        public void Vector4DotTest1()
        {
            Vector3 a = new Vector3(1.55f, 1.55f, 1);
            Vector3 b = new Vector3(2.5f, 3, 1.5f);
            Vector3 c = Vector3.Cross(a, b);

            Vector4 d = new Vector4(a, 0);
            Vector4 e = new Vector4(c, 0);

            float actual = Vector4.Dot(d, e);
            Assert.True(MathHelper.Equal(0.0f, actual), "Vector4f.Dot did not return the expected value.");
        }

        [Fact]
        public void Vector4CrossTest()
        {
            Vector3 a3 = new Vector3(1.0f, 0.0f, 0.0f);
            Vector3 b3 = new Vector3(0.0f, 1.0f, 0.0f);
            Vector3 e3 = Vector3.Cross(a3, b3);

            Vector4 a4 = new Vector4(a3, 2.0f);
            Vector4 b4 = new Vector4(b3, 3.0f);
            Vector4 e4 = new Vector4(e3, a4.W * b4.W);

            Vector4 actual = Vector4.Cross(a4, b4);
            Assert.True(MathHelper.Equal(e4, actual), "Vector4f.Cross did not return the expected value.");
        }

        [Fact]
        public void Vector4CrossTest1()
        {
            // Cross test of the same vector
            Vector3 a3 = new Vector3(0.0f, 1.0f, 0.0f);
            Vector3 b3 = new Vector3(0.0f, 1.0f, 0.0f);
            Vector3 e3 = Vector3.Cross(a3, b3);

            Vector4 a4 = new Vector4(a3, 3.0f);
            Vector4 b4 = new Vector4(b3, 3.0f);
            Vector4 e4 = new Vector4(e3, a4.W * b4.W);

            Vector4 actual = Vector4.Cross(a4, b4);
            Assert.True(MathHelper.Equal(e4, actual), "Vector4f.Cross did not return the expected value.");
        }

        // A test for Length ()
        [Fact]
        public void Vector4LengthTest()
        {
            Vector3 a = new Vector3(1.0f, 2.0f, 3.0f);
            float w = 4.0f;

            Vector4 target = new Vector4(a, w);

            float expected = (float)System.Math.Sqrt(30.0f);
            float actual;

            actual = target.Length();

            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.Length did not return the expected value.");
        }

        // A test for Length ()
        // Length test where length is zero
        [Fact]
        public void Vector4LengthTest1()
        {
            Vector4 target = new Vector4();

            float expected = 0.0f;
            float actual = target.Length();

            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.Length did not return the expected value.");
        }

        // A test for LengthSquared ()
        [Fact]
        public void Vector4LengthSquaredTest()
        {
            Vector3 a = new Vector3(1.0f, 2.0f, 3.0f);
            float w = 4.0f;

            Vector4 target = new Vector4(a, w);

            float expected = 30;
            float actual;

            actual = target.LengthSquared();

            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.LengthSquared did not return the expected value.");
        }

        // A test for Min (Vector4f, Vector4f)
        [Fact]
        public void Vector4MinTest()
        {
            Vector4 a = new Vector4(-1.0f, 4.0f, -3.0f, 1000.0f);
            Vector4 b = new Vector4(2.0f, 1.0f, -1.0f, 0.0f);

            Vector4 expected = new Vector4(-1.0f, 1.0f, -3.0f, 0.0f);
            Vector4 actual;
            actual = Vector4.Min(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.Min did not return the expected value.");
        }

        // A test for Max (Vector4f, Vector4f)
        [Fact]
        public void Vector4MaxTest()
        {
            Vector4 a = new Vector4(-1.0f, 4.0f, -3.0f, 1000.0f);
            Vector4 b = new Vector4(2.0f, 1.0f, -1.0f, 0.0f);

            Vector4 expected = new Vector4(2.0f, 4.0f, -1.0f, 1000.0f);
            Vector4 actual;
            actual = Vector4.Max(a, b);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.Max did not return the expected value.");
        }

        [Fact]
        public void Vector4MinMaxCodeCoverageTest()
        {
            Vector4 min = Vector4.Zero;
            Vector4 max = Vector4.One;
            Vector4 actual;

            // Min.
            actual = Vector4.Min(min, max);
            Assert.Equal(actual, min);

            actual = Vector4.Min(max, min);
            Assert.Equal(actual, min);

            // Max.
            actual = Vector4.Max(min, max);
            Assert.Equal(actual, max);

            actual = Vector4.Max(max, min);
            Assert.Equal(actual, max);
        }

        // A test for Clamp (Vector4f, Vector4f, Vector4f)
        [Fact]
        public void Vector4ClampTest()
        {
            Vector4 a = new Vector4(0.5f, 0.3f, 0.33f, 0.44f);
            Vector4 min = new Vector4(0.0f, 0.1f, 0.13f, 0.14f);
            Vector4 max = new Vector4(1.0f, 1.1f, 1.13f, 1.14f);

            // Normal case.
            // Case N1: specified value is in the range.
            Vector4 expected = new Vector4(0.5f, 0.3f, 0.33f, 0.44f);
            Vector4 actual = Vector4.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.Clamp did not return the expected value.");

            // Normal case.
            // Case N2: specified value is bigger than max value.
            a = new Vector4(2.0f, 3.0f, 4.0f, 5.0f);
            expected = max;
            actual = Vector4.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.Clamp did not return the expected value.");

            // Case N3: specified value is smaller than max value.
            a = new Vector4(-2.0f, -3.0f, -4.0f, -5.0f);
            expected = min;
            actual = Vector4.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.Clamp did not return the expected value.");

            // Case N4: combination case.
            a = new Vector4(-2.0f, 0.5f, 4.0f, -5.0f);
            expected = new Vector4(min.X, a.Y, max.Z, min.W);
            actual = Vector4.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.Clamp did not return the expected value.");

            // User specified min value is bigger than max value.
            max = new Vector4(0.0f, 0.1f, 0.13f, 0.14f);
            min = new Vector4(1.0f, 1.1f, 1.13f, 1.14f);

            // Case W1: specified value is in the range.
            a = new Vector4(0.5f, 0.3f, 0.33f, 0.44f);
            expected = max;
            actual = Vector4.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.Clamp did not return the expected value.");

            // Normal case.
            // Case W2: specified value is bigger than max and min value.
            a = new Vector4(2.0f, 3.0f, 4.0f, 5.0f);
            expected = max;
            actual = Vector4.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.Clamp did not return the expected value.");

            // Case W3: specified value is smaller than min and max value.
            a = new Vector4(-2.0f, -3.0f, -4.0f, -5.0f);
            expected = max;
            actual = Vector4.Clamp(a, min, max);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.Clamp did not return the expected value.");
        }

        // A test for Lerp (Vector4f, Vector4f, float)
        [Fact]
        public void Vector4LerpTest()
        {
            Vector4 a = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
            Vector4 b = new Vector4(5.0f, 6.0f, 7.0f, 8.0f);

            float t = 0.5f;

            Vector4 expected = new Vector4(3.0f, 4.0f, 5.0f, 6.0f);
            Vector4 actual;

            actual = Vector4.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector4f, Vector4f, float)
        // Lerp test with factor zero
        [Fact]
        public void Vector4LerpTest1()
        {
            Vector4 a = new Vector4(new Vector3(1.0f, 2.0f, 3.0f), 4.0f);
            Vector4 b = new Vector4(4.0f, 5.0f, 6.0f, 7.0f);

            float t = 0.0f;
            Vector4 expected = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
            Vector4 actual = Vector4.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector4f, Vector4f, float)
        // Lerp test with factor one
        [Fact]
        public void Vector4LerpTest2()
        {
            Vector4 a = new Vector4(new Vector3(1.0f, 2.0f, 3.0f), 4.0f);
            Vector4 b = new Vector4(4.0f, 5.0f, 6.0f, 7.0f);

            float t = 1.0f;
            Vector4 expected = new Vector4(4.0f, 5.0f, 6.0f, 7.0f);
            Vector4 actual = Vector4.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector4f, Vector4f, float)
        // Lerp test with factor > 1
        [Fact]
        public void Vector4LerpTest3()
        {
            Vector4 a = new Vector4(new Vector3(0.0f, 0.0f, 0.0f), 0.0f);
            Vector4 b = new Vector4(4.0f, 5.0f, 6.0f, 7.0f);

            float t = 2.0f;
            Vector4 expected = new Vector4(8.0f, 10.0f, 12.0f, 14.0f);
            Vector4 actual = Vector4.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector4f, Vector4f, float)
        // Lerp test with factor < 0
        [Fact]
        public void Vector4LerpTest4()
        {
            Vector4 a = new Vector4(new Vector3(0.0f, 0.0f, 0.0f), 0.0f);
            Vector4 b = new Vector4(4.0f, 5.0f, 6.0f, 7.0f);

            float t = -2.0f;
            Vector4 expected = -(b * 2);
            Vector4 actual = Vector4.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector4f, Vector4f, float)
        // Lerp test with special float value
        [Fact]
        public void Vector4LerpTest5()
        {
            Vector4 a = new Vector4(45.67f, 90.0f, 0, 0);
            Vector4 b = new Vector4(float.PositiveInfinity, float.NegativeInfinity, 0, 0);

            float t = 0.408f;
            Vector4 actual = Vector4.Lerp(a, b, t);
            Assert.True(float.IsPositiveInfinity(actual.X), "Vector4f.Lerp did not return the expected value.");
            Assert.True(float.IsNegativeInfinity(actual.Y), "Vector4f.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector4f, Vector4f, float)
        // Lerp test from the same point
        [Fact]
        public void Vector4LerpTest6()
        {
            Vector4 a = new Vector4(4.0f, 5.0f, 6.0f, 7.0f);
            Vector4 b = new Vector4(4.0f, 5.0f, 6.0f, 7.0f);

            float t = 0.85f;
            Vector4 expected = a;
            Vector4 actual = Vector4.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector4f, Vector4f, float)
        // Lerp test with values known to be inaccurate with the old lerp impl
        [Fact]
        public void Vector4LerpTest7()
        {
            Vector4 a = new Vector4(0.44728136f);
            Vector4 b = new Vector4(0.46345946f);

            float t = 0.26402435f;

            Vector4 expected = new Vector4(0.45155275f);
            Vector4 actual = Vector4.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.Lerp did not return the expected value.");
        }

        // A test for Lerp (Vector4f, Vector4f, float)
        // Lerp test with values known to be inaccurate with the old lerp impl
        // (Old code incorrectly gets 0.33333588)
        [Fact]
        public void Vector4LerpTest8()
        {
            Vector4 a = new Vector4(-100);
            Vector4 b = new Vector4(0.33333334f);

            float t = 1f;

            Vector4 expected = new Vector4(0.33333334f);
            Vector4 actual = Vector4.Lerp(a, b, t);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.Lerp did not return the expected value.");
        }

        // A test for Transform (Vector2f, Matrix4x4)
        [Fact]
        public void Vector4TransformTest1()
        {
            Vector2 v = new Vector2(1.0f, 2.0f);

            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0f));
            m.M41 = 10.0f;
            m.M42 = 20.0f;
            m.M43 = 30.0f;

            Vector4 expected = new Vector4(10.316987f, 22.183012f, 30.3660259f, 1.0f);
            Vector4 actual;

            actual = Vector4.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.Transform did not return the expected value.");
        }

        // A test for Transform (Vector3f, Matrix4x4)
        [Fact]
        public void Vector4TransformTest2()
        {
            Vector3 v = new Vector3(1.0f, 2.0f, 3.0f);

            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0f));
            m.M41 = 10.0f;
            m.M42 = 20.0f;
            m.M43 = 30.0f;

            Vector4 expected = new Vector4(12.19198728f, 21.53349376f, 32.61602545f, 1.0f);
            Vector4 actual;

            actual = Vector4.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "vector4.Transform did not return the expected value.");
        }

        // A test for Transform (Vector4f, Matrix4x4)
        [Fact]
        public void Vector4TransformVector4Test()
        {
            Vector4 v = new Vector4(1.0f, 2.0f, 3.0f, 0.0f);

            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0f));
            m.M41 = 10.0f;
            m.M42 = 20.0f;
            m.M43 = 30.0f;

            Vector4 expected = new Vector4(2.19198728f, 1.53349376f, 2.61602545f, 0.0f);
            Vector4 actual;

            actual = Vector4.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.Transform did not return the expected value.");

            //
            v.W = 1.0f;

            expected = new Vector4(12.19198728f, 21.53349376f, 32.61602545f, 1.0f);
            actual = Vector4.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.Transform did not return the expected value.");
        }

        // A test for Transform (Vector4f, Matrix4x4)
        // Transform vector4 with zero matrix
        [Fact]
        public void Vector4TransformVector4Test1()
        {
            Vector4 v = new Vector4(1.0f, 2.0f, 3.0f, 0.0f);
            Matrix4x4 m = new Matrix4x4();
            Vector4 expected = new Vector4(0, 0, 0, 0);

            Vector4 actual = Vector4.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.Transform did not return the expected value.");
        }

        // A test for Transform (Vector4f, Matrix4x4)
        // Transform vector4 with identity matrix
        [Fact]
        public void Vector4TransformVector4Test2()
        {
            Vector4 v = new Vector4(1.0f, 2.0f, 3.0f, 0.0f);
            Matrix4x4 m = Matrix4x4.Identity;
            Vector4 expected = new Vector4(1.0f, 2.0f, 3.0f, 0.0f);

            Vector4 actual = Vector4.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.Transform did not return the expected value.");
        }

        // A test for Transform (Vector3f, Matrix4x4)
        // Transform Vector3f test
        [Fact]
        public void Vector4TransformVector3Test()
        {
            Vector3 v = new Vector3(1.0f, 2.0f, 3.0f);

            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0f));
            m.M41 = 10.0f;
            m.M42 = 20.0f;
            m.M43 = 30.0f;

            Vector4 expected = Vector4.Transform(new Vector4(v, 1.0f), m);
            Vector4 actual = Vector4.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.Transform did not return the expected value.");
        }

        // A test for Transform (Vector3f, Matrix4x4)
        // Transform vector3 with zero matrix
        [Fact]
        public void Vector4TransformVector3Test1()
        {
            Vector3 v = new Vector3(1.0f, 2.0f, 3.0f);
            Matrix4x4 m = new Matrix4x4();
            Vector4 expected = new Vector4(0, 0, 0, 0);

            Vector4 actual = Vector4.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.Transform did not return the expected value.");
        }

        // A test for Transform (Vector3f, Matrix4x4)
        // Transform vector3 with identity matrix
        [Fact]
        public void Vector4TransformVector3Test2()
        {
            Vector3 v = new Vector3(1.0f, 2.0f, 3.0f);
            Matrix4x4 m = Matrix4x4.Identity;
            Vector4 expected = new Vector4(1.0f, 2.0f, 3.0f, 1.0f);

            Vector4 actual = Vector4.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.Transform did not return the expected value.");
        }

        // A test for Transform (Vector2f, Matrix4x4)
        // Transform Vector2f test
        [Fact]
        public void Vector4TransformVector2Test()
        {
            Vector2 v = new Vector2(1.0f, 2.0f);

            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0f));
            m.M41 = 10.0f;
            m.M42 = 20.0f;
            m.M43 = 30.0f;

            Vector4 expected = Vector4.Transform(new Vector4(v, 0.0f, 1.0f), m);
            Vector4 actual = Vector4.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.Transform did not return the expected value.");
        }

        // A test for Transform (Vector2f, Matrix4x4)
        // Transform Vector2f with zero matrix
        [Fact]
        public void Vector4TransformVector2Test1()
        {
            Vector2 v = new Vector2(1.0f, 2.0f);
            Matrix4x4 m = new Matrix4x4();
            Vector4 expected = new Vector4(0, 0, 0, 0);

            Vector4 actual = Vector4.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.Transform did not return the expected value.");
        }

        // A test for Transform (Vector2f, Matrix4x4)
        // Transform vector2 with identity matrix
        [Fact]
        public void Vector4TransformVector2Test2()
        {
            Vector2 v = new Vector2(1.0f, 2.0f);
            Matrix4x4 m = Matrix4x4.Identity;
            Vector4 expected = new Vector4(1.0f, 2.0f, 0, 1.0f);

            Vector4 actual = Vector4.Transform(v, m);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.Transform did not return the expected value.");
        }

        // A test for Transform (Vector2f, Quaternion)
        [Fact]
        public void Vector4TransformVector2QuatanionTest()
        {
            Vector2 v = new Vector2(1.0f, 2.0f);

            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0f));

            Quaternion q = Quaternion.CreateFromRotationMatrix(m);

            Vector4 expected = Vector4.Transform(v, m);
            Vector4 actual;

            actual = Vector4.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.Transform did not return the expected value.");
        }

        // A test for Transform (Vector3f, Quaternion)
        [Fact]
        public void Vector4TransformVector3Quaternion()
        {
            Vector3 v = new Vector3(1.0f, 2.0f, 3.0f);

            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0f));
            Quaternion q = Quaternion.CreateFromRotationMatrix(m);

            Vector4 expected = Vector4.Transform(v, m);
            Vector4 actual;

            actual = Vector4.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "vector4.Transform did not return the expected value.");
        }

        // A test for Transform (Vector4f, Quaternion)
        [Fact]
        public void Vector4TransformVector4QuaternionTest()
        {
            Vector4 v = new Vector4(1.0f, 2.0f, 3.0f, 0.0f);

            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0f));
            Quaternion q = Quaternion.CreateFromRotationMatrix(m);

            Vector4 expected = Vector4.Transform(v, m);
            Vector4 actual;

            actual = Vector4.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.Transform did not return the expected value.");

            //
            v.W = 1.0f;
            expected.W = 1.0f;
            actual = Vector4.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.Transform did not return the expected value.");
        }

        // A test for Transform (Vector4f, Quaternion)
        // Transform vector4 with zero quaternion
        [Fact]
        public void Vector4TransformVector4QuaternionTest1()
        {
            Vector4 v = new Vector4(1.0f, 2.0f, 3.0f, 0.0f);
            Quaternion q = new Quaternion();
            Vector4 expected = Vector4.Zero;

            Vector4 actual = Vector4.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.Transform did not return the expected value.");
        }

        // A test for Transform (Vector4f, Quaternion)
        // Transform vector4 with identity matrix
        [Fact]
        public void Vector4TransformVector4QuaternionTest2()
        {
            Vector4 v = new Vector4(1.0f, 2.0f, 3.0f, 0.0f);
            Quaternion q = Quaternion.Identity;
            Vector4 expected = new Vector4(1.0f, 2.0f, 3.0f, 0.0f);

            Vector4 actual = Vector4.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.Transform did not return the expected value.");
        }

        // A test for Transform (Vector3f, Quaternion)
        // Transform Vector3f test
        [Fact]
        public void Vector4TransformVector3QuaternionTest()
        {
            Vector3 v = new Vector3(1.0f, 2.0f, 3.0f);

            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0f));
            Quaternion q = Quaternion.CreateFromRotationMatrix(m);

            Vector4 expected = Vector4.Transform(v, m);
            Vector4 actual = Vector4.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.Transform did not return the expected value.");
        }

        // A test for Transform (Vector3f, Quaternion)
        // Transform vector3 with zero quaternion
        [Fact]
        public void Vector4TransformVector3QuaternionTest1()
        {
            Vector3 v = new Vector3(1.0f, 2.0f, 3.0f);
            Quaternion q = new Quaternion();
            Vector4 expected = Vector4.Zero;

            Vector4 actual = Vector4.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.Transform did not return the expected value.");
        }

        // A test for Transform (Vector3f, Quaternion)
        // Transform vector3 with identity quaternion
        [Fact]
        public void Vector4TransformVector3QuaternionTest2()
        {
            Vector3 v = new Vector3(1.0f, 2.0f, 3.0f);
            Quaternion q = Quaternion.Identity;
            Vector4 expected = new Vector4(1.0f, 2.0f, 3.0f, 1.0f);

            Vector4 actual = Vector4.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.Transform did not return the expected value.");
        }

        // A test for Transform (Vector2f, Quaternion)
        // Transform Vector2f by quaternion test
        [Fact]
        public void Vector4TransformVector2QuaternionTest()
        {
            Vector2 v = new Vector2(1.0f, 2.0f);

            Matrix4x4 m =
                Matrix4x4.CreateRotationX(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationY(MathHelper.ToRadians(30.0f)) *
                Matrix4x4.CreateRotationZ(MathHelper.ToRadians(30.0f));
            Quaternion q = Quaternion.CreateFromRotationMatrix(m);

            Vector4 expected = Vector4.Transform(v, m);
            Vector4 actual = Vector4.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.Transform did not return the expected value.");
        }

        // A test for Transform (Vector2f, Quaternion)
        // Transform Vector2f with zero quaternion
        [Fact]
        public void Vector4TransformVector2QuaternionTest1()
        {
            Vector2 v = new Vector2(1.0f, 2.0f);
            Quaternion q = new Quaternion();
            Vector4 expected = Vector4.Zero;

            Vector4 actual = Vector4.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.Transform did not return the expected value.");
        }

        // A test for Transform (Vector2f, Matrix4x4)
        // Transform vector2 with identity Quaternion
        [Fact]
        public void Vector4TransformVector2QuaternionTest2()
        {
            Vector2 v = new Vector2(1.0f, 2.0f);
            Quaternion q = Quaternion.Identity;
            Vector4 expected = new Vector4(1.0f, 2.0f, 0, 1.0f);

            Vector4 actual = Vector4.Transform(v, q);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.Transform did not return the expected value.");
        }

        // A test for Normalize (Vector4f)
        [Fact]
        public void Vector4NormalizeTest()
        {
            Vector4 a = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);

            Vector4 expected = new Vector4(
                0.1825741858350553711523232609336f,
                0.3651483716701107423046465218672f,
                0.5477225575051661134569697828008f,
                0.7302967433402214846092930437344f);
            Vector4 actual;

            actual = Vector4.Normalize(a);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.Normalize did not return the expected value.");
        }

        // A test for Normalize (Vector4f)
        // Normalize vector of length one
        [Fact]
        public void Vector4NormalizeTest1()
        {
            Vector4 a = new Vector4(1.0f, 0.0f, 0.0f, 0.0f);

            Vector4 expected = new Vector4(1.0f, 0.0f, 0.0f, 0.0f);
            Vector4 actual = Vector4.Normalize(a);
            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.Normalize did not return the expected value.");
        }

        // A test for Normalize (Vector4f)
        // Normalize vector of length zero
        [Fact]
        public void Vector4NormalizeTest2()
        {
            Vector4 a = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);

            Vector4 expected = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
            Vector4 actual = Vector4.Normalize(a);
            Assert.True(float.IsNaN(actual.X) && float.IsNaN(actual.Y) && float.IsNaN(actual.Z) && float.IsNaN(actual.W), "Vector4f.Normalize did not return the expected value.");
        }

        // A test for operator - (Vector4f)
        [Fact]
        public void Vector4UnaryNegationTest()
        {
            Vector4 a = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);

            Vector4 expected = new Vector4(-1.0f, -2.0f, -3.0f, -4.0f);
            Vector4 actual;

            actual = -a;

            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.operator - did not return the expected value.");
        }

        // A test for operator - (Vector4f, Vector4f)
        [Fact]
        public void Vector4SubtractionTest()
        {
            Vector4 a = new Vector4(1.0f, 6.0f, 3.0f, 4.0f);
            Vector4 b = new Vector4(5.0f, 2.0f, 3.0f, 9.0f);

            Vector4 expected = new Vector4(-4.0f, 4.0f, 0.0f, -5.0f);
            Vector4 actual;

            actual = a - b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.operator - did not return the expected value.");
        }

        // A test for operator * (Vector4f, float)
        [Fact]
        public void Vector4MultiplyOperatorTest()
        {
            Vector4 a = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);

            const float factor = 2.0f;

            Vector4 expected = new Vector4(2.0f, 4.0f, 6.0f, 8.0f);
            Vector4 actual;

            actual = a * factor;
            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.operator * did not return the expected value.");
        }

        // A test for operator * (float, Vector4f)
        [Fact]
        public void Vector4MultiplyOperatorTest2()
        {
            Vector4 a = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);

            const float factor = 2.0f;
            Vector4 expected = new Vector4(2.0f, 4.0f, 6.0f, 8.0f);
            Vector4 actual;

            actual = factor * a;
            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.operator * did not return the expected value.");
        }

        // A test for operator * (Vector4f, Vector4f)
        [Fact]
        public void Vector4MultiplyOperatorTest3()
        {
            Vector4 a = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
            Vector4 b = new Vector4(5.0f, 6.0f, 7.0f, 8.0f);

            Vector4 expected = new Vector4(5.0f, 12.0f, 21.0f, 32.0f);
            Vector4 actual;

            actual = a * b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.operator * did not return the expected value.");
        }

        // A test for operator / (Vector4f, float)
        [Fact]
        public void Vector4DivisionTest()
        {
            Vector4 a = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);

            float div = 2.0f;

            Vector4 expected = new Vector4(0.5f, 1.0f, 1.5f, 2.0f);
            Vector4 actual;

            actual = a / div;

            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.operator / did not return the expected value.");
        }

        // A test for operator / (Vector4f, Vector4f)
        [Fact]
        public void Vector4DivisionTest1()
        {
            Vector4 a = new Vector4(1.0f, 6.0f, 7.0f, 4.0f);
            Vector4 b = new Vector4(5.0f, 2.0f, 3.0f, 8.0f);

            Vector4 expected = new Vector4(1.0f / 5.0f, 6.0f / 2.0f, 7.0f / 3.0f, 4.0f / 8.0f);
            Vector4 actual;

            actual = a / b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.operator / did not return the expected value.");
        }

        // A test for operator / (Vector4f, Vector4f)
        // Divide by zero
        [Fact]
        public void Vector4DivisionTest2()
        {
            Vector4 a = new Vector4(-2.0f, 3.0f, float.MaxValue, float.NaN);

            float div = 0.0f;

            Vector4 actual = a / div;

            Assert.True(float.IsNegativeInfinity(actual.X), "Vector4f.operator / did not return the expected value.");
            Assert.True(float.IsPositiveInfinity(actual.Y), "Vector4f.operator / did not return the expected value.");
            Assert.True(float.IsPositiveInfinity(actual.Z), "Vector4f.operator / did not return the expected value.");
            Assert.True(float.IsNaN(actual.W), "Vector4f.operator / did not return the expected value.");
        }

        // A test for operator / (Vector4f, Vector4f)
        // Divide by zero
        [Fact]
        public void Vector4DivisionTest3()
        {
            Vector4 a = new Vector4(0.047f, -3.0f, float.NegativeInfinity, float.MinValue);
            Vector4 b = new Vector4();

            Vector4 actual = a / b;

            Assert.True(float.IsPositiveInfinity(actual.X), "Vector4f.operator / did not return the expected value.");
            Assert.True(float.IsNegativeInfinity(actual.Y), "Vector4f.operator / did not return the expected value.");
            Assert.True(float.IsNegativeInfinity(actual.Z), "Vector4f.operator / did not return the expected value.");
            Assert.True(float.IsNegativeInfinity(actual.W), "Vector4f.operator / did not return the expected value.");
        }

        // A test for operator + (Vector4f, Vector4f)
        [Fact]
        public void Vector4AdditionTest()
        {
            Vector4 a = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
            Vector4 b = new Vector4(5.0f, 6.0f, 7.0f, 8.0f);

            Vector4 expected = new Vector4(6.0f, 8.0f, 10.0f, 12.0f);
            Vector4 actual;

            actual = a + b;

            Assert.True(MathHelper.Equal(expected, actual), "Vector4f.operator + did not return the expected value.");
        }

        [Fact]
        public void OperatorAddTest()
        {
            Vector4 v1 = new Vector4(2.5f, 2.0f, 3.0f, 3.3f);
            Vector4 v2 = new Vector4(5.5f, 4.5f, 6.5f, 7.5f);

            Vector4 v3 = v1 + v2;
            Vector4 v5 = new Vector4(-1.0f, 0.0f, 0.0f, float.NaN);
            Vector4 v4 = v1 + v5;
            Assert.Equal(8.0f, v3.X);
            Assert.Equal(6.5f, v3.Y);
            Assert.Equal(9.5f, v3.Z);
            Assert.Equal(10.8f, v3.W);
            Assert.Equal(1.5f, v4.X);
            Assert.Equal(2.0f, v4.Y);
            Assert.Equal(3.0f, v4.Z);
            Assert.Equal(float.NaN, v4.W);
        }

        // A test for Vector4f (float, float, float, float)
        [Fact]
        public void Vector4ConstructorTest()
        {
            float x = 1.0f;
            float y = 2.0f;
            float z = 3.0f;
            float w = 4.0f;

            Vector4 target = new Vector4(x, y, z, w);

            Assert.True(MathHelper.Equal(target.X, x) && MathHelper.Equal(target.Y, y) && MathHelper.Equal(target.Z, z) && MathHelper.Equal(target.W, w),
                "Vector4f constructor(x,y,z,w) did not return the expected value.");
        }

        // A test for Vector4f (Vector2f, float, float)
        [Fact]
        public void Vector4ConstructorTest1()
        {
            Vector2 a = new Vector2(1.0f, 2.0f);
            float z = 3.0f;
            float w = 4.0f;

            Vector4 target = new Vector4(a, z, w);
            Assert.True(MathHelper.Equal(target.X, a.X) && MathHelper.Equal(target.Y, a.Y) && MathHelper.Equal(target.Z, z) && MathHelper.Equal(target.W, w),
                "Vector4f constructor(Vector2f,z,w) did not return the expected value.");
        }

        // A test for Vector4f (Vector3f, float)
        [Fact]
        public void Vector4ConstructorTest2()
        {
            Vector3 a = new Vector3(1.0f, 2.0f, 3.0f);
            float w = 4.0f;

            Vector4 target = new Vector4(a, w);

            Assert.True(MathHelper.Equal(target.X, a.X) && MathHelper.Equal(target.Y, a.Y) && MathHelper.Equal(target.Z, a.Z) && MathHelper.Equal(target.W, w),
                "Vector4f constructor(Vector3f,w) did not return the expected value.");
        }

        // A test for Vector4f ()
        // Constructor with no parameter
        [Fact]
        public void Vector4ConstructorTest4()
        {
            Vector4 a = new Vector4();

            Assert.Equal(0.0f, a.X);
            Assert.Equal(0.0f, a.Y);
            Assert.Equal(0.0f, a.Z);
            Assert.Equal(0.0f, a.W);
        }

        // A test for Vector4f ()
        // Constructor with special floating values
        [Fact]
        public void Vector4ConstructorTest5()
        {
            Vector4 target = new Vector4(float.NaN, float.MaxValue, float.PositiveInfinity, float.Epsilon);

            Assert.True(float.IsNaN(target.X), "Vector4f.constructor (float, float, float, float) did not return the expected value.");
            Assert.True(float.Equals(float.MaxValue, target.Y), "Vector4f.constructor (float, float, float, float) did not return the expected value.");
            Assert.True(float.IsPositiveInfinity(target.Z), "Vector4f.constructor (float, float, float, float) did not return the expected value.");
            Assert.True(float.Equals(float.Epsilon, target.W), "Vector4f.constructor (float, float, float, float) did not return the expected value.");
        }

        // A test for Vector4f (ReadOnlySpan<float>)
        [Fact]
        public void Vector4ConstructorTest7()
        {
            float value = 1.0f;
            Vector4 target = new Vector4(new[] { value, value, value, value });
            Vector4 expected = new Vector4(value);

            Assert.Equal(expected, target);
            Assert.Throws<ArgumentOutOfRangeException>(() => new Vector4(new float[3]));
        }

        // A test for Add (Vector4f, Vector4f)
        [Fact]
        public void Vector4AddTest()
        {
            Vector4 a = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
            Vector4 b = new Vector4(5.0f, 6.0f, 7.0f, 8.0f);

            Vector4 expected = new Vector4(6.0f, 8.0f, 10.0f, 12.0f);
            Vector4 actual;

            actual = Vector4.Add(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Divide (Vector4f, float)
        [Fact]
        public void Vector4DivideTest()
        {
            Vector4 a = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
            float div = 2.0f;
            Vector4 expected = new Vector4(0.5f, 1.0f, 1.5f, 2.0f);
            Vector4 actual;
            actual = Vector4.Divide(a, div);
            Assert.Equal(expected, actual);
        }

        // A test for Divide (Vector4f, Vector4f)
        [Fact]
        public void Vector4DivideTest1()
        {
            Vector4 a = new Vector4(1.0f, 6.0f, 7.0f, 4.0f);
            Vector4 b = new Vector4(5.0f, 2.0f, 3.0f, 8.0f);

            Vector4 expected = new Vector4(1.0f / 5.0f, 6.0f / 2.0f, 7.0f / 3.0f, 4.0f / 8.0f);
            Vector4 actual;

            actual = Vector4.Divide(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Equals (object)
        [Fact]
        public void Vector4EqualsTest()
        {
            Vector4 a = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
            Vector4 b = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);

            // case 1: compare between same values
            object obj = b;

            bool expected = true;
            bool actual = a.Equals(obj);
            Assert.Equal(expected, actual);

            // case 2: compare between different values
            b.X = 10.0f;
            obj = b;
            expected = false;
            actual = a.Equals(obj);
            Assert.Equal(expected, actual);

            // case 3: compare between different types.
            obj = new Quaternion();
            expected = false;
            actual = a.Equals(obj);
            Assert.Equal(expected, actual);

            // case 3: compare against null.
            obj = null;
            expected = false;
            actual = a.Equals(obj);
            Assert.Equal(expected, actual);
        }

        // A test for Multiply (float, Vector4f)
        [Fact]
        public void Vector4MultiplyTest()
        {
            Vector4 a = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
            const float factor = 2.0f;
            Vector4 expected = new Vector4(2.0f, 4.0f, 6.0f, 8.0f);
            Vector4 actual = Vector4.Multiply(factor, a);
            Assert.Equal(expected, actual);
        }

        // A test for Multiply (Vector4f, float)
        [Fact]
        public void Vector4MultiplyTest2()
        {
            Vector4 a = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
            const float factor = 2.0f;
            Vector4 expected = new Vector4(2.0f, 4.0f, 6.0f, 8.0f);
            Vector4 actual = Vector4.Multiply(a, factor);
            Assert.Equal(expected, actual);
        }

        // A test for Multiply (Vector4f, Vector4f)
        [Fact]
        public void Vector4MultiplyTest3()
        {
            Vector4 a = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
            Vector4 b = new Vector4(5.0f, 6.0f, 7.0f, 8.0f);

            Vector4 expected = new Vector4(5.0f, 12.0f, 21.0f, 32.0f);
            Vector4 actual;

            actual = Vector4.Multiply(a, b);
            Assert.Equal(expected, actual);
        }

        // A test for Negate (Vector4f)
        [Fact]
        public void Vector4NegateTest()
        {
            Vector4 a = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);

            Vector4 expected = new Vector4(-1.0f, -2.0f, -3.0f, -4.0f);
            Vector4 actual;

            actual = Vector4.Negate(a);
            Assert.Equal(expected, actual);
        }

        // A test for operator != (Vector4f, Vector4f)
        [Fact]
        public void Vector4InequalityTest()
        {
            Vector4 a = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
            Vector4 b = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);

            // case 1: compare between same values
            bool expected = false;
            bool actual = a != b;
            Assert.Equal(expected, actual);

            // case 2: compare between different values
            b.X = 10.0f;
            expected = true;
            actual = a != b;
            Assert.Equal(expected, actual);
        }

        // A test for operator == (Vector4f, Vector4f)
        [Fact]
        public void Vector4EqualityTest()
        {
            Vector4 a = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
            Vector4 b = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);

            // case 1: compare between same values
            bool expected = true;
            bool actual = a == b;
            Assert.Equal(expected, actual);

            // case 2: compare between different values
            b.X = 10.0f;
            expected = false;
            actual = a == b;
            Assert.Equal(expected, actual);
        }

        // A test for Subtract (Vector4f, Vector4f)
        [Fact]
        public void Vector4SubtractTest()
        {
            Vector4 a = new Vector4(1.0f, 6.0f, 3.0f, 4.0f);
            Vector4 b = new Vector4(5.0f, 2.0f, 3.0f, 9.0f);

            Vector4 expected = new Vector4(-4.0f, 4.0f, 0.0f, -5.0f);
            Vector4 actual;

            actual = Vector4.Subtract(a, b);

            Assert.Equal(expected, actual);
        }

        // A test for UnitW
        [Fact]
        public void Vector4UnitWTest()
        {
            Vector4 val = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
            Assert.Equal(val, Vector4.UnitW);
        }

        // A test for UnitX
        [Fact]
        public void Vector4UnitXTest()
        {
            Vector4 val = new Vector4(1.0f, 0.0f, 0.0f, 0.0f);
            Assert.Equal(val, Vector4.UnitX);
        }

        // A test for UnitY
        [Fact]
        public void Vector4UnitYTest()
        {
            Vector4 val = new Vector4(0.0f, 1.0f, 0.0f, 0.0f);
            Assert.Equal(val, Vector4.UnitY);
        }

        // A test for UnitZ
        [Fact]
        public void Vector4UnitZTest()
        {
            Vector4 val = new Vector4(0.0f, 0.0f, 1.0f, 0.0f);
            Assert.Equal(val, Vector4.UnitZ);
        }

        // A test for One
        [Fact]
        public void Vector4OneTest()
        {
            Vector4 val = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            Assert.Equal(val, Vector4.One);
        }

        // A test for Zero
        [Fact]
        public void Vector4ZeroTest()
        {
            Vector4 val = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
            Assert.Equal(val, Vector4.Zero);
        }

        // A test for Equals (Vector4f)
        [Fact]
        public void Vector4EqualsTest1()
        {
            Vector4 a = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);
            Vector4 b = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);

            // case 1: compare between same values
            Assert.True(a.Equals(b));

            // case 2: compare between different values
            b.X = 10.0f;
            Assert.False(a.Equals(b));
        }

        // A test for Vector4f (float)
        [Fact]
        public void Vector4ConstructorTest6()
        {
            float value = 1.0f;
            Vector4 target = new Vector4(value);

            Vector4 expected = new Vector4(value, value, value, value);
            Assert.Equal(expected, target);

            value = 2.0f;
            target = new Vector4(value);
            expected = new Vector4(value, value, value, value);
            Assert.Equal(expected, target);
        }

        // A test for Vector4f comparison involving NaN values
        [Fact]
        public void Vector4EqualsNaNTest()
        {
            Vector4 a = new Vector4(float.NaN, 0, 0, 0);
            Vector4 b = new Vector4(0, float.NaN, 0, 0);
            Vector4 c = new Vector4(0, 0, float.NaN, 0);
            Vector4 d = new Vector4(0, 0, 0, float.NaN);

            Assert.False(a == Vector4.Zero);
            Assert.False(b == Vector4.Zero);
            Assert.False(c == Vector4.Zero);
            Assert.False(d == Vector4.Zero);

            Assert.True(a != Vector4.Zero);
            Assert.True(b != Vector4.Zero);
            Assert.True(c != Vector4.Zero);
            Assert.True(d != Vector4.Zero);

            Assert.False(a.Equals(Vector4.Zero));
            Assert.False(b.Equals(Vector4.Zero));
            Assert.False(c.Equals(Vector4.Zero));
            Assert.False(d.Equals(Vector4.Zero));

            Assert.True(a.Equals(a));
            Assert.True(b.Equals(b));
            Assert.True(c.Equals(c));
            Assert.True(d.Equals(d));
        }

        [Fact]
        public void Vector4AbsTest()
        {
            Vector4 v1 = new Vector4(-2.5f, 2.0f, 3.0f, 3.3f);
            Vector4 v3 = Vector4.Abs(new Vector4(float.PositiveInfinity, 0.0f, float.NegativeInfinity, float.NaN));
            Vector4 v = Vector4.Abs(v1);
            Assert.Equal(2.5f, v.X);
            Assert.Equal(2.0f, v.Y);
            Assert.Equal(3.0f, v.Z);
            Assert.Equal(3.3f, v.W);
            Assert.Equal(float.PositiveInfinity, v3.X);
            Assert.Equal(0.0f, v3.Y);
            Assert.Equal(float.PositiveInfinity, v3.Z);
            Assert.Equal(float.NaN, v3.W);
        }

        [Fact]
        public void Vector4SqrtTest()
        {
            Vector4 v1 = new Vector4(-2.5f, 2.0f, 3.0f, 3.3f);
            Vector4 v2 = new Vector4(5.5f, 4.5f, 6.5f, 7.5f);
            Assert.Equal(2, (int)Vector4.SquareRoot(v2).X);
            Assert.Equal(2, (int)Vector4.SquareRoot(v2).Y);
            Assert.Equal(2, (int)Vector4.SquareRoot(v2).Z);
            Assert.Equal(2, (int)Vector4.SquareRoot(v2).W);
            Assert.Equal(float.NaN, Vector4.SquareRoot(v1).X);
        }

        // A test to make sure these types are blittable directly into GPU buffer memory layouts
        [Fact]
        public unsafe void Vector4SizeofTest()
        {
            Assert.Equal(16, sizeof(Vector4));
            Assert.Equal(32, sizeof(Vector4_2x));
            Assert.Equal(20, sizeof(Vector4PlusFloat));
            Assert.Equal(40, sizeof(Vector4PlusFloat_2x));
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Vector4_2x
        {
            private Vector4 _a;
            private Vector4 _b;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Vector4PlusFloat
        {
            private Vector4 _v;
            private float _f;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Vector4PlusFloat_2x
        {
            private Vector4PlusFloat _a;
            private Vector4PlusFloat _b;
        }

        [Fact]
        public void SetFieldsTest()
        {
            Vector4 v3 = new Vector4(4f, 5f, 6f, 7f);
            v3.X = 1.0f;
            v3.Y = 2.0f;
            v3.Z = 3.0f;
            v3.W = 4.0f;
            Assert.Equal(1.0f, v3.X);
            Assert.Equal(2.0f, v3.Y);
            Assert.Equal(3.0f, v3.Z);
            Assert.Equal(4.0f, v3.W);
            Vector4 v4 = v3;
            v4.Y = 0.5f;
            v4.Z = 2.2f;
            v4.W = 3.5f;
            Assert.Equal(1.0f, v4.X);
            Assert.Equal(0.5f, v4.Y);
            Assert.Equal(2.2f, v4.Z);
            Assert.Equal(3.5f, v4.W);
            Assert.Equal(2.0f, v3.Y);
        }

        [Fact]
        public void EmbeddedVectorSetFields()
        {
            EmbeddedVectorObject evo = new EmbeddedVectorObject();
            evo.FieldVector.X = 5.0f;
            evo.FieldVector.Y = 5.0f;
            evo.FieldVector.Z = 5.0f;
            evo.FieldVector.W = 5.0f;
            Assert.Equal(5.0f, evo.FieldVector.X);
            Assert.Equal(5.0f, evo.FieldVector.Y);
            Assert.Equal(5.0f, evo.FieldVector.Z);
            Assert.Equal(5.0f, evo.FieldVector.W);
        }

        [Fact]
        public void DeeplyEmbeddedObjectTest()
        {
            DeeplyEmbeddedClass obj = new DeeplyEmbeddedClass();
            obj.L0.L1.L2.L3.L4.L5.L6.L7.EmbeddedVector.X = 5f;
            Assert.Equal(5f, obj.RootEmbeddedObject.X);
            Assert.Equal(5f, obj.RootEmbeddedObject.Y);
            Assert.Equal(1f, obj.RootEmbeddedObject.Z);
            Assert.Equal(-5f, obj.RootEmbeddedObject.W);
            obj.L0.L1.L2.L3.L4.L5.L6.L7.EmbeddedVector = new Vector4(1, 2, 3, 4);
            Assert.Equal(1f, obj.RootEmbeddedObject.X);
            Assert.Equal(2f, obj.RootEmbeddedObject.Y);
            Assert.Equal(3f, obj.RootEmbeddedObject.Z);
            Assert.Equal(4f, obj.RootEmbeddedObject.W);
        }

        [Fact]
        public void DeeplyEmbeddedStructTest()
        {
            DeeplyEmbeddedStruct obj = DeeplyEmbeddedStruct.Create();
            obj.L0.L1.L2.L3.L4.L5.L6.L7.EmbeddedVector.X = 5f;
            Assert.Equal(5f, obj.RootEmbeddedObject.X);
            Assert.Equal(5f, obj.RootEmbeddedObject.Y);
            Assert.Equal(1f, obj.RootEmbeddedObject.Z);
            Assert.Equal(-5f, obj.RootEmbeddedObject.W);
            obj.L0.L1.L2.L3.L4.L5.L6.L7.EmbeddedVector = new Vector4(1, 2, 3, 4);
            Assert.Equal(1f, obj.RootEmbeddedObject.X);
            Assert.Equal(2f, obj.RootEmbeddedObject.Y);
            Assert.Equal(3f, obj.RootEmbeddedObject.Z);
            Assert.Equal(4f, obj.RootEmbeddedObject.W);
        }

        private class EmbeddedVectorObject
        {
            public Vector4 FieldVector;
        }

        private class DeeplyEmbeddedClass
        {
            public readonly Level0 L0 = new Level0();
            public Vector4 RootEmbeddedObject { get { return L0.L1.L2.L3.L4.L5.L6.L7.EmbeddedVector; } }
            public class Level0
            {
                public readonly Level1 L1 = new Level1();
                public class Level1
                {
                    public readonly Level2 L2 = new Level2();
                    public class Level2
                    {
                        public readonly Level3 L3 = new Level3();
                        public class Level3
                        {
                            public readonly Level4 L4 = new Level4();
                            public class Level4
                            {
                                public readonly Level5 L5 = new Level5();
                                public class Level5
                                {
                                    public readonly Level6 L6 = new Level6();
                                    public class Level6
                                    {
                                        public readonly Level7 L7 = new Level7();
                                        public class Level7
                                        {
                                            public Vector4 EmbeddedVector = new Vector4(1, 5, 1, -5);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // Contrived test for strangely-sized and shaped embedded structures, with unused buffer fields.
#pragma warning disable 0169
        private struct DeeplyEmbeddedStruct
        {
            public static DeeplyEmbeddedStruct Create()
            {
                var obj = new DeeplyEmbeddedStruct();
                obj.L0 = new Level0();
                obj.L0.L1 = new Level0.Level1();
                obj.L0.L1.L2 = new Level0.Level1.Level2();
                obj.L0.L1.L2.L3 = new Level0.Level1.Level2.Level3();
                obj.L0.L1.L2.L3.L4 = new Level0.Level1.Level2.Level3.Level4();
                obj.L0.L1.L2.L3.L4.L5 = new Level0.Level1.Level2.Level3.Level4.Level5();
                obj.L0.L1.L2.L3.L4.L5.L6 = new Level0.Level1.Level2.Level3.Level4.Level5.Level6();
                obj.L0.L1.L2.L3.L4.L5.L6.L7 = new Level0.Level1.Level2.Level3.Level4.Level5.Level6.Level7();
                obj.L0.L1.L2.L3.L4.L5.L6.L7.EmbeddedVector = new Vector4(1, 5, 1, -5);

                return obj;
            }

            public Level0 L0;
            public Vector4 RootEmbeddedObject { get { return L0.L1.L2.L3.L4.L5.L6.L7.EmbeddedVector; } }
            public struct Level0
            {
                private float _buffer0, _buffer1;
                public Level1 L1;
                private float _buffer2;
                public struct Level1
                {
                    private float _buffer0, _buffer1;
                    public Level2 L2;
                    private byte _buffer2;
                    public struct Level2
                    {
                        public Level3 L3;
                        private float _buffer0;
                        private byte _buffer1;
                        public struct Level3
                        {
                            public Level4 L4;
                            public struct Level4
                            {
                                private float _buffer0;
                                public Level5 L5;
                                private long _buffer1;
                                private byte _buffer2;
                                private double _buffer3;
                                public struct Level5
                                {
                                    private byte _buffer0;
                                    public Level6 L6;
                                    public struct Level6
                                    {
                                        private byte _buffer0;
                                        public Level7 L7;
                                        private byte _buffer1, _buffer2;
                                        public struct Level7
                                        {
                                            public Vector4 EmbeddedVector;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
#pragma warning restore 0169

        [Theory]
        [MemberData(nameof(GenericMathTestMemberData.CosSingle), MemberType = typeof(GenericMathTestMemberData))]
        public void CosSingleTest(float value, float expectedResult, float variance)
        {
            Vector4 actualResult = Vector4.Cos(Vector4.Create(value));
            AssertEqual(Vector4.Create(expectedResult), actualResult, Vector4.Create(variance));
        }

        [Theory]
        [MemberData(nameof(GenericMathTestMemberData.ExpSingle), MemberType = typeof(GenericMathTestMemberData))]
        public void ExpSingleTest(float value, float expectedResult, float variance)
        {
            Vector4 actualResult = Vector4.Exp(Vector4.Create(value));
            AssertEqual(Vector4.Create(expectedResult), actualResult, Vector4.Create(variance));
        }

        [Theory]
        [MemberData(nameof(GenericMathTestMemberData.LogSingle), MemberType = typeof(GenericMathTestMemberData))]
        public void LogSingleTest(float value, float expectedResult, float variance)
        {
            Vector4 actualResult = Vector4.Log(Vector4.Create(value));
            AssertEqual(Vector4.Create(expectedResult), actualResult, Vector4.Create(variance));
        }

        [Theory]
        [MemberData(nameof(GenericMathTestMemberData.Log2Single), MemberType = typeof(GenericMathTestMemberData))]
        public void Log2SingleTest(float value, float expectedResult, float variance)
        {
            Vector4 actualResult = Vector4.Log2(Vector4.Create(value));
            AssertEqual(Vector4.Create(expectedResult), actualResult, Vector4.Create(variance));
        }

        [Theory]
        [MemberData(nameof(GenericMathTestMemberData.FusedMultiplyAddSingle), MemberType = typeof(GenericMathTestMemberData))]
        public void FusedMultiplyAddSingleTest(float left, float right, float addend, float expectedResult)
        {
            AssertEqual(Vector4.Create(expectedResult), Vector4.FusedMultiplyAdd(Vector4.Create(left), Vector4.Create(right), Vector4.Create(addend)), Vector4.Zero);
            AssertEqual(Vector4.Create(float.MultiplyAddEstimate(left, right, addend)), Vector4.MultiplyAddEstimate(Vector4.Create(left), Vector4.Create(right), Vector4.Create(addend)), Vector4.Zero);
        }

        [Theory]
        [MemberData(nameof(GenericMathTestMemberData.ClampSingle), MemberType = typeof(GenericMathTestMemberData))]
        public void ClampSingleTest(float x, float min, float max, float expectedResult)
        {
            Vector4 actualResult = Vector4.Clamp(Vector4.Create(x), Vector4.Create(min), Vector4.Create(max));
            AssertEqual(Vector4.Create(expectedResult), actualResult, Vector4.Zero);
        }

        [Theory]
        [MemberData(nameof(GenericMathTestMemberData.CopySignSingle), MemberType = typeof(GenericMathTestMemberData))]
        public void CopySignSingleTest(float x, float y, float expectedResult)
        {
            Vector4 actualResult = Vector4.CopySign(Vector4.Create(x), Vector4.Create(y));
            AssertEqual(Vector4.Create(expectedResult), actualResult, Vector4.Zero);
        }

        [Theory]
        [MemberData(nameof(GenericMathTestMemberData.DegreesToRadiansSingle), MemberType = typeof(GenericMathTestMemberData))]
        public void DegreesToRadiansSingleTest(float value, float expectedResult, float variance)
        {
            AssertEqual(Vector4.Create(-expectedResult), Vector4.DegreesToRadians(Vector4.Create(-value)), Vector4.Create(variance));
            AssertEqual(Vector4.Create(+expectedResult), Vector4.DegreesToRadians(Vector4.Create(+value)), Vector4.Create(variance));
        }

        [Theory]
        [MemberData(nameof(GenericMathTestMemberData.HypotSingle), MemberType = typeof(GenericMathTestMemberData))]
        public void HypotSingleTest(float x, float y, float expectedResult, float variance)
        {
            AssertEqual(Vector4.Create(expectedResult), Vector4.Hypot(Vector4.Create(-x), Vector4.Create(-y)), Vector4.Create(variance));
            AssertEqual(Vector4.Create(expectedResult), Vector4.Hypot(Vector4.Create(-x), Vector4.Create(+y)), Vector4.Create(variance));
            AssertEqual(Vector4.Create(expectedResult), Vector4.Hypot(Vector4.Create(+x), Vector4.Create(-y)), Vector4.Create(variance));
            AssertEqual(Vector4.Create(expectedResult), Vector4.Hypot(Vector4.Create(+x), Vector4.Create(+y)), Vector4.Create(variance));

            AssertEqual(Vector4.Create(expectedResult), Vector4.Hypot(Vector4.Create(-y), Vector4.Create(-x)), Vector4.Create(variance));
            AssertEqual(Vector4.Create(expectedResult), Vector4.Hypot(Vector4.Create(-y), Vector4.Create(+x)), Vector4.Create(variance));
            AssertEqual(Vector4.Create(expectedResult), Vector4.Hypot(Vector4.Create(+y), Vector4.Create(-x)), Vector4.Create(variance));
            AssertEqual(Vector4.Create(expectedResult), Vector4.Hypot(Vector4.Create(+y), Vector4.Create(+x)), Vector4.Create(variance));
        }

        [Theory]
        [MemberData(nameof(GenericMathTestMemberData.LerpSingle), MemberType = typeof(GenericMathTestMemberData))]
        public void LerpSingleTest(float x, float y, float amount, float expectedResult)
        {
            AssertEqual(Vector4.Create(+expectedResult), Vector4.Lerp(Vector4.Create(+x), Vector4.Create(+y), Vector4.Create(amount)), Vector4.Zero);
            AssertEqual(Vector4.Create((expectedResult == 0.0f) ? expectedResult : -expectedResult), Vector4.Lerp(Vector4.Create(-x), Vector4.Create(-y), Vector4.Create(amount)), Vector4.Zero);
        }

        [Theory]
        [MemberData(nameof(GenericMathTestMemberData.MaxSingle), MemberType = typeof(GenericMathTestMemberData))]
        public void MaxSingleTest(float x, float y, float expectedResult)
        {
            Vector4 actualResult = Vector4.Max(Vector4.Create(x), Vector4.Create(y));
            AssertEqual(Vector4.Create(expectedResult), actualResult, Vector4.Zero);
        }

        [Theory]
        [MemberData(nameof(GenericMathTestMemberData.MaxMagnitudeSingle), MemberType = typeof(GenericMathTestMemberData))]
        public void MaxMagnitudeSingleTest(float x, float y, float expectedResult)
        {
            Vector4 actualResult = Vector4.MaxMagnitude(Vector4.Create(x), Vector4.Create(y));
            AssertEqual(Vector4.Create(expectedResult), actualResult, Vector4.Zero);
        }

        [Theory]
        [MemberData(nameof(GenericMathTestMemberData.MaxMagnitudeNumberSingle), MemberType = typeof(GenericMathTestMemberData))]
        public void MaxMagnitudeNumberSingleTest(float x, float y, float expectedResult)
        {
            Vector4 actualResult = Vector4.MaxMagnitudeNumber(Vector4.Create(x), Vector4.Create(y));
            AssertEqual(Vector4.Create(expectedResult), actualResult, Vector4.Zero);
        }

        [Theory]
        [MemberData(nameof(GenericMathTestMemberData.MaxNumberSingle), MemberType = typeof(GenericMathTestMemberData))]
        public void MaxNumberSingleTest(float x, float y, float expectedResult)
        {
            Vector4 actualResult = Vector4.MaxNumber(Vector4.Create(x), Vector4.Create(y));
            AssertEqual(Vector4.Create(expectedResult), actualResult, Vector4.Zero);
        }

        [Theory]
        [MemberData(nameof(GenericMathTestMemberData.MinSingle), MemberType = typeof(GenericMathTestMemberData))]
        public void MinSingleTest(float x, float y, float expectedResult)
        {
            Vector4 actualResult = Vector4.Min(Vector4.Create(x), Vector4.Create(y));
            AssertEqual(Vector4.Create(expectedResult), actualResult, Vector4.Zero);
        }

        [Theory]
        [MemberData(nameof(GenericMathTestMemberData.MinMagnitudeSingle), MemberType = typeof(GenericMathTestMemberData))]
        public void MinMagnitudeSingleTest(float x, float y, float expectedResult)
        {
            Vector4 actualResult = Vector4.MinMagnitude(Vector4.Create(x), Vector4.Create(y));
            AssertEqual(Vector4.Create(expectedResult), actualResult, Vector4.Zero);
        }

        [Theory]
        [MemberData(nameof(GenericMathTestMemberData.MinMagnitudeNumberSingle), MemberType = typeof(GenericMathTestMemberData))]
        public void MinMagnitudeNumberSingleTest(float x, float y, float expectedResult)
        {
            Vector4 actualResult = Vector4.MinMagnitudeNumber(Vector4.Create(x), Vector4.Create(y));
            AssertEqual(Vector4.Create(expectedResult), actualResult, Vector4.Zero);
        }

        [Theory]
        [MemberData(nameof(GenericMathTestMemberData.MinNumberSingle), MemberType = typeof(GenericMathTestMemberData))]
        public void MinNumberSingleTest(float x, float y, float expectedResult)
        {
            Vector4 actualResult = Vector4.MinNumber(Vector4.Create(x), Vector4.Create(y));
            AssertEqual(Vector4.Create(expectedResult), actualResult, Vector4.Zero);
        }

        [Theory]
        [MemberData(nameof(GenericMathTestMemberData.RadiansToDegreesSingle), MemberType = typeof(GenericMathTestMemberData))]
        public void RadiansToDegreesSingleTest(float value, float expectedResult, float variance)
        {
            AssertEqual(Vector4.Create(-expectedResult), Vector4.RadiansToDegrees(Vector4.Create(-value)), Vector4.Create(variance));
            AssertEqual(Vector4.Create(+expectedResult), Vector4.RadiansToDegrees(Vector4.Create(+value)), Vector4.Create(variance));
        }

        [Theory]
        [MemberData(nameof(GenericMathTestMemberData.RoundSingle), MemberType = typeof(GenericMathTestMemberData))]
        public void RoundSingleTest(float value, float expectedResult)
        {
            Vector4 actualResult = Vector4.Round(Vector4.Create(value));
            AssertEqual(Vector4.Create(expectedResult), actualResult, Vector4.Zero);
        }

        [Theory]
        [MemberData(nameof(GenericMathTestMemberData.RoundAwayFromZeroSingle), MemberType = typeof(GenericMathTestMemberData))]
        public void RoundAwayFromZeroSingleTest(float value, float expectedResult)
        {
            Vector4 actualResult = Vector4.Round(Vector4.Create(value), MidpointRounding.AwayFromZero);
            AssertEqual(Vector4.Create(expectedResult), actualResult, Vector4.Zero);
        }

        [Theory]
        [MemberData(nameof(GenericMathTestMemberData.RoundToEvenSingle), MemberType = typeof(GenericMathTestMemberData))]
        public void RoundToEvenSingleTest(float value, float expectedResult)
        {
            Vector4 actualResult = Vector4.Round(Vector4.Create(value), MidpointRounding.ToEven);
            AssertEqual(Vector4.Create(expectedResult), actualResult, Vector4.Zero);
        }

        [Theory]
        [MemberData(nameof(GenericMathTestMemberData.SinSingle), MemberType = typeof(GenericMathTestMemberData))]
        public void SinSingleTest(float value, float expectedResult, float variance)
        {
            Vector4 actualResult = Vector4.Sin(Vector4.Create(value));
            AssertEqual(Vector4.Create(expectedResult), actualResult, Vector4.Create(variance));
        }

        [Theory]
        [MemberData(nameof(GenericMathTestMemberData.SinCosSingle), MemberType = typeof(GenericMathTestMemberData))]
        public void SinCosSingleTest(float value, float expectedResultSin, float expectedResultCos, float allowedVarianceSin, float allowedVarianceCos)
        {
            (Vector4 resultSin, Vector4 resultCos) = Vector4.SinCos(Vector4.Create(value));
            AssertEqual(Vector4.Create(expectedResultSin), resultSin, Vector4.Create(allowedVarianceSin));
            AssertEqual(Vector4.Create(expectedResultCos), resultCos, Vector4.Create(allowedVarianceCos));
        }

        [Theory]
        [MemberData(nameof(GenericMathTestMemberData.TruncateSingle), MemberType = typeof(GenericMathTestMemberData))]
        public void TruncateSingleTest(float value, float expectedResult)
        {
            Vector4 actualResult = Vector4.Truncate(Vector4.Create(value));
            AssertEqual(Vector4.Create(expectedResult), actualResult, Vector4.Zero);
        }

        [Fact]
        public void AllAnyNoneTest()
        {
            Test(3, 2);

            [MethodImpl(MethodImplOptions.NoInlining)]
            void Test(float value1, float value2)
            {
                var input1 = Vector4.Create(value1);
                var input2 = Vector4.Create(value2);

                Assert.True(Vector4.All(input1, value1));
                Assert.True(Vector4.All(input2, value2));
                Assert.False(Vector4.All(input1.WithElement(0, value2), value1));
                Assert.False(Vector4.All(input2.WithElement(0, value1), value2));
                Assert.False(Vector4.All(input1, value2));
                Assert.False(Vector4.All(input2, value1));
                Assert.False(Vector4.All(input1.WithElement(0, value2), value2));
                Assert.False(Vector4.All(input2.WithElement(0, value1), value1));

                Assert.True(Vector4.Any(input1, value1));
                Assert.True(Vector4.Any(input2, value2));
                Assert.True(Vector4.Any(input1.WithElement(0, value2), value1));
                Assert.True(Vector4.Any(input2.WithElement(0, value1), value2));
                Assert.False(Vector4.Any(input1, value2));
                Assert.False(Vector4.Any(input2, value1));
                Assert.True(Vector4.Any(input1.WithElement(0, value2), value2));
                Assert.True(Vector4.Any(input2.WithElement(0, value1), value1));

                Assert.False(Vector4.None(input1, value1));
                Assert.False(Vector4.None(input2, value2));
                Assert.False(Vector4.None(input1.WithElement(0, value2), value1));
                Assert.False(Vector4.None(input2.WithElement(0, value1), value2));
                Assert.True(Vector4.None(input1, value2));
                Assert.True(Vector4.None(input2, value1));
                Assert.False(Vector4.None(input1.WithElement(0, value2), value2));
                Assert.False(Vector4.None(input2.WithElement(0, value1), value1));
            }
        }

        [Fact]
        public void AllAnyNoneTest_AllBitsSet()
        {
            Test(BitConverter.Int32BitsToSingle(-1));

            [MethodImpl(MethodImplOptions.NoInlining)]
            void Test(float value)
            {
                var input = Vector4.Create(value);

                Assert.False(Vector4.All(input, value));
                Assert.False(Vector4.Any(input, value));
                Assert.True(Vector4.None(input, value));
            }
        }

        [Fact]
        public void AllAnyNoneWhereAllBitsSetTest()
        {
            Test(BitConverter.Int32BitsToSingle(-1), 2);

            [MethodImpl(MethodImplOptions.NoInlining)]
            void Test(float allBitsSet, float value2)
            {
                var input1 = Vector4.Create(allBitsSet);
                var input2 = Vector4.Create(value2);

                Assert.True(Vector4.AllWhereAllBitsSet(input1));
                Assert.False(Vector4.AllWhereAllBitsSet(input2));
                Assert.False(Vector4.AllWhereAllBitsSet(input1.WithElement(0, value2)));
                Assert.False(Vector4.AllWhereAllBitsSet(input2.WithElement(0, allBitsSet)));

                Assert.True(Vector4.AnyWhereAllBitsSet(input1));
                Assert.False(Vector4.AnyWhereAllBitsSet(input2));
                Assert.True(Vector4.AnyWhereAllBitsSet(input1.WithElement(0, value2)));
                Assert.True(Vector4.AnyWhereAllBitsSet(input2.WithElement(0, allBitsSet)));

                Assert.False(Vector4.NoneWhereAllBitsSet(input1));
                Assert.True(Vector4.NoneWhereAllBitsSet(input2));
                Assert.False(Vector4.NoneWhereAllBitsSet(input1.WithElement(0, value2)));
                Assert.False(Vector4.NoneWhereAllBitsSet(input2.WithElement(0, allBitsSet)));
            }
        }

        [Fact]
        public void CountIndexOfLastIndexOfSingleTest()
        {
            Test(3, 2);

            [MethodImpl(MethodImplOptions.NoInlining)]
            void Test(float value1, float value2)
            {
                var input1 = Vector4.Create(value1);
                var input2 = Vector4.Create(value2);

                Assert.Equal(ElementCount, Vector4.Count(input1, value1));
                Assert.Equal(ElementCount, Vector4.Count(input2, value2));
                Assert.Equal(ElementCount - 1, Vector4.Count(input1.WithElement(0, value2), value1));
                Assert.Equal(ElementCount - 1, Vector4.Count(input2.WithElement(0, value1), value2));
                Assert.Equal(0, Vector4.Count(input1, value2));
                Assert.Equal(0, Vector4.Count(input2, value1));
                Assert.Equal(1, Vector4.Count(input1.WithElement(0, value2), value2));
                Assert.Equal(1, Vector4.Count(input2.WithElement(0, value1), value1));

                Assert.Equal(0, Vector4.IndexOf(input1, value1));
                Assert.Equal(0, Vector4.IndexOf(input2, value2));
                Assert.Equal(1, Vector4.IndexOf(input1.WithElement(0, value2), value1));
                Assert.Equal(1, Vector4.IndexOf(input2.WithElement(0, value1), value2));
                Assert.Equal(-1, Vector4.IndexOf(input1, value2));
                Assert.Equal(-1, Vector4.IndexOf(input2, value1));
                Assert.Equal(0, Vector4.IndexOf(input1.WithElement(0, value2), value2));
                Assert.Equal(0, Vector4.IndexOf(input2.WithElement(0, value1), value1));

                Assert.Equal(ElementCount - 1, Vector4.LastIndexOf(input1, value1));
                Assert.Equal(ElementCount - 1, Vector4.LastIndexOf(input2, value2));
                Assert.Equal(ElementCount - 1, Vector4.LastIndexOf(input1.WithElement(0, value2), value1));
                Assert.Equal(ElementCount - 1, Vector4.LastIndexOf(input2.WithElement(0, value1), value2));
                Assert.Equal(-1, Vector4.LastIndexOf(input1, value2));
                Assert.Equal(-1, Vector4.LastIndexOf(input2, value1));
                Assert.Equal(0, Vector4.LastIndexOf(input1.WithElement(0, value2), value2));
                Assert.Equal(0, Vector4.LastIndexOf(input2.WithElement(0, value1), value1));
            }
        }

        [Fact]
        public void CountIndexOfLastIndexOfSingleTest_AllBitsSet()
        {
            Test(BitConverter.Int32BitsToSingle(-1));

            [MethodImpl(MethodImplOptions.NoInlining)]
            void Test(float value)
            {
                var input = Vector4.Create(value);

                Assert.Equal(0, Vector4.Count(input, value));
                Assert.Equal(-1, Vector4.IndexOf(input, value));
                Assert.Equal(-1, Vector4.LastIndexOf(input, value));
            }
        }

        [Fact]
        public void CountIndexOfLastIndexOfWhereAllBitsSetSingleTest()
        {
            Test(BitConverter.Int32BitsToSingle(-1), 2);

            [MethodImpl(MethodImplOptions.NoInlining)]
            void Test(float allBitsSet, float value2)
            {
                var input1 = Vector4.Create(allBitsSet);
                var input2 = Vector4.Create(value2);

                Assert.Equal(ElementCount, Vector4.CountWhereAllBitsSet(input1));
                Assert.Equal(0, Vector4.CountWhereAllBitsSet(input2));
                Assert.Equal(ElementCount - 1, Vector4.CountWhereAllBitsSet(input1.WithElement(0, value2)));
                Assert.Equal(1, Vector4.CountWhereAllBitsSet(input2.WithElement(0, allBitsSet)));

                Assert.Equal(0, Vector4.IndexOfWhereAllBitsSet(input1));
                Assert.Equal(-1, Vector4.IndexOfWhereAllBitsSet(input2));
                Assert.Equal(1, Vector4.IndexOfWhereAllBitsSet(input1.WithElement(0, value2)));
                Assert.Equal(0, Vector4.IndexOfWhereAllBitsSet(input2.WithElement(0, allBitsSet)));

                Assert.Equal(ElementCount - 1, Vector4.LastIndexOfWhereAllBitsSet(input1));
                Assert.Equal(-1, Vector4.LastIndexOfWhereAllBitsSet(input2));
                Assert.Equal(ElementCount - 1, Vector4.LastIndexOfWhereAllBitsSet(input1.WithElement(0, value2)));
                Assert.Equal(0, Vector4.LastIndexOfWhereAllBitsSet(input2.WithElement(0, allBitsSet)));
            }
        }

        [Theory]
        [MemberData(nameof(GenericMathTestMemberData.IsTestSingle), MemberType = typeof(GenericMathTestMemberData))]
        public void IsEvenIntegerTest(float value) => Assert.Equal(float.IsEvenInteger(value) ? Vector4.AllBitsSet : Vector4.Zero, Vector4.IsEvenInteger(Vector4.Create(value)));

        [Theory]
        [MemberData(nameof(GenericMathTestMemberData.IsTestSingle), MemberType = typeof(GenericMathTestMemberData))]
        public void IsFiniteTest(float value) => Assert.Equal(float.IsFinite(value) ? Vector4.AllBitsSet : Vector4.Zero, Vector4.IsFinite(Vector4.Create(value)));

        [Theory]
        [MemberData(nameof(GenericMathTestMemberData.IsTestSingle), MemberType = typeof(GenericMathTestMemberData))]
        public void IsInfinityTest(float value) => Assert.Equal(float.IsInfinity(value) ? Vector4.AllBitsSet : Vector4.Zero, Vector4.IsInfinity(Vector4.Create(value)));

        [Theory]
        [MemberData(nameof(GenericMathTestMemberData.IsTestSingle), MemberType = typeof(GenericMathTestMemberData))]
        public void IsIntegerTest(float value) => Assert.Equal(float.IsInteger(value) ? Vector4.AllBitsSet : Vector4.Zero, Vector4.IsInteger(Vector4.Create(value)));

        [Theory]
        [MemberData(nameof(GenericMathTestMemberData.IsTestSingle), MemberType = typeof(GenericMathTestMemberData))]
        public void IsNaNTest(float value) => Assert.Equal(float.IsNaN(value) ? Vector4.AllBitsSet : Vector4.Zero, Vector4.IsNaN(Vector4.Create(value)));

        [Theory]
        [MemberData(nameof(GenericMathTestMemberData.IsTestSingle), MemberType = typeof(GenericMathTestMemberData))]
        public void IsNegativeTest(float value) => Assert.Equal(float.IsNegative(value) ? Vector4.AllBitsSet : Vector4.Zero, Vector4.IsNegative(Vector4.Create(value)));

        [Theory]
        [MemberData(nameof(GenericMathTestMemberData.IsTestSingle), MemberType = typeof(GenericMathTestMemberData))]
        public void IsNegativeInfinityTest(float value) => Assert.Equal(float.IsNegativeInfinity(value) ? Vector4.AllBitsSet : Vector4.Zero, Vector4.IsNegativeInfinity(Vector4.Create(value)));

        [Theory]
        [MemberData(nameof(GenericMathTestMemberData.IsTestSingle), MemberType = typeof(GenericMathTestMemberData))]
        public void IsNormalTest(float value) => Assert.Equal(float.IsNormal(value) ? Vector4.AllBitsSet : Vector4.Zero, Vector4.IsNormal(Vector4.Create(value)));

        [Theory]
        [MemberData(nameof(GenericMathTestMemberData.IsTestSingle), MemberType = typeof(GenericMathTestMemberData))]
        public void IsOddIntegerTest(float value) => Assert.Equal(float.IsOddInteger(value) ? Vector4.AllBitsSet : Vector4.Zero, Vector4.IsOddInteger(Vector4.Create(value)));

        [Theory]
        [MemberData(nameof(GenericMathTestMemberData.IsTestSingle), MemberType = typeof(GenericMathTestMemberData))]
        public void IsPositiveTest(float value) => Assert.Equal(float.IsPositive(value) ? Vector4.AllBitsSet : Vector4.Zero, Vector4.IsPositive(Vector4.Create(value)));

        [Theory]
        [MemberData(nameof(GenericMathTestMemberData.IsTestSingle), MemberType = typeof(GenericMathTestMemberData))]
        public void IsPositiveInfinityTest(float value) => Assert.Equal(float.IsPositiveInfinity(value) ? Vector4.AllBitsSet : Vector4.Zero, Vector4.IsPositiveInfinity(Vector4.Create(value)));

        [Theory]
        [MemberData(nameof(GenericMathTestMemberData.IsTestSingle), MemberType = typeof(GenericMathTestMemberData))]
        public void IsSubnormalTest(float value) => Assert.Equal(float.IsSubnormal(value) ? Vector4.AllBitsSet : Vector4.Zero, Vector4.IsSubnormal(Vector4.Create(value)));

        [Theory]
        [MemberData(nameof(GenericMathTestMemberData.IsTestSingle), MemberType = typeof(GenericMathTestMemberData))]
        public void IsZeroSingleTest(float value) => Assert.Equal((value == 0) ? Vector4.AllBitsSet : Vector4.Zero, Vector4.IsZero(Vector4.Create(value)));

        [Fact]
        public void AllBitsSetTest()
        {
            Assert.Equal(-1, BitConverter.SingleToInt32Bits(Vector4.AllBitsSet.X));
            Assert.Equal(-1, BitConverter.SingleToInt32Bits(Vector4.AllBitsSet.Y));
            Assert.Equal(-1, BitConverter.SingleToInt32Bits(Vector4.AllBitsSet.Z));
            Assert.Equal(-1, BitConverter.SingleToInt32Bits(Vector4.AllBitsSet.W));
        }

        [Fact]
        public void ConditionalSelectTest()
        {
            Test(Vector4.Create(1, 2, 3, 4), Vector4.AllBitsSet, Vector4.Create(1, 2, 3, 4), Vector4.Create(5, 6, 7, 8));
            Test(Vector4.Create(5, 6, 7, 8), Vector4.Zero, Vector4.Create(1, 2, 3, 4), Vector4.Create(5, 6, 7, 8));
            Test(Vector4.Create(1, 6, 3, 8), Vector128.Create(-1, 0, -1, 0).AsSingle().AsVector4(), Vector4.Create(1, 2, 3, 4), Vector4.Create(5, 6, 7, 8));

            [MethodImpl(MethodImplOptions.NoInlining)]
            void Test(Vector4 expectedResult, Vector4 condition, Vector4 left, Vector4 right)
            {
                Assert.Equal(expectedResult, Vector4.ConditionalSelect(condition, left, right));
            }
        }

        [Theory]
        [InlineData(+0.0f, +0.0f, +0.0f, +0.0f, 0b0000)]
        [InlineData(-0.0f, +1.0f, -0.0f, +0.0f, 0b0101)]
        [InlineData(-0.0f, -0.0f, -0.0f, -0.0f, 0b1111)]
        public void ExtractMostSignificantBitsTest(float x, float y, float z, float w, uint expectedResult)
        {
            Assert.Equal(expectedResult, Vector4.Create(x, y, z, w).ExtractMostSignificantBits());
        }

        [Theory]
        [InlineData(1.0f, 2.0f, 3.0f, 4.0f)]
        [InlineData(5.0f, 6.0f, 7.0f, 8.0f)]
        public void GetElementTest(float x, float y, float z, float w)
        {
            Assert.Equal(x, Vector4.Create(x, y, z, w).GetElement(0));
            Assert.Equal(y, Vector4.Create(x, y, z, w).GetElement(1));
            Assert.Equal(z, Vector4.Create(x, y, z, w).GetElement(2));
            Assert.Equal(w, Vector4.Create(x, y, z, w).GetElement(3));
        }

        [Theory]
        [InlineData(1.0f, 2.0f, 3.0f, 4.0f)]
        [InlineData(5.0f, 6.0f, 7.0f, 8.0f)]
        public void ShuffleTest(float x, float y, float z, float w)
        {
            Assert.Equal(Vector4.Create(w, z, y, x), Vector4.Shuffle(Vector4.Create(x, y, z, w), 3, 2, 1, 0));
            Assert.Equal(Vector4.Create(y, x, w, z), Vector4.Shuffle(Vector4.Create(x, y, z, w), 1, 0, 3, 2));
        }

        [Theory]
        [InlineData(1.0f, 2.0f, 3.0f, 4.0f, 10.0f)]
        [InlineData(5.0f, 6.0f, 7.0f, 8.0f, 26.0f)]
        public void SumTest(float x, float y, float z, float w, float expectedResult)
        {
            Assert.Equal(expectedResult, Vector4.Sum(Vector4.Create(x, y, z, w)));
        }

        [Theory]
        [InlineData(1.0f, 2.0f, 3.0f, 4.0f)]
        [InlineData(5.0f, 6.0f, 7.0f, 8.0f)]
        public void ToScalarTest(float x, float y, float z, float w)
        {
            Assert.Equal(x, Vector4.Create(x, y, z, w).ToScalar());
        }

        [Theory]
        [InlineData(1.0f, 2.0f, 3.0f, 4.0f)]
        [InlineData(5.0f, 6.0f, 7.0f, 8.0f)]
        public void WithElementTest(float x, float y, float z, float w)
        {
            var vector = Vector4.Create(10);

            Assert.Equal(10, vector.X);
            Assert.Equal(10, vector.Y);
            Assert.Equal(10, vector.Z);
            Assert.Equal(10, vector.W);

            vector = vector.WithElement(0, x);

            Assert.Equal(x, vector.X);
            Assert.Equal(10, vector.Y);
            Assert.Equal(10, vector.Z);
            Assert.Equal(10, vector.W);

            vector = vector.WithElement(1, y);

            Assert.Equal(x, vector.X);
            Assert.Equal(y, vector.Y);
            Assert.Equal(10, vector.Z);
            Assert.Equal(10, vector.W);

            vector = vector.WithElement(2, z);

            Assert.Equal(x, vector.X);
            Assert.Equal(y, vector.Y);
            Assert.Equal(z, vector.Z);
            Assert.Equal(10, vector.W);

            vector = vector.WithElement(3, w);

            Assert.Equal(x, vector.X);
            Assert.Equal(y, vector.Y);
            Assert.Equal(z, vector.Z);
            Assert.Equal(w, vector.W);
        }

        [Fact]
        public void CreateScalarTest()
        {
            var vector = Vector4.CreateScalar(float.Pi);

            Assert.Equal(float.Pi, vector.X);
            Assert.Equal(0, vector.Y);
            Assert.Equal(0, vector.Z);
            Assert.Equal(0, vector.W);

            vector = Vector4.CreateScalar(float.E);

            Assert.Equal(float.E, vector.X);
            Assert.Equal(0, vector.Y);
            Assert.Equal(0, vector.Z);
            Assert.Equal(0, vector.W);
        }

        [Fact]
        public void CreateScalarUnsafeTest()
        {
            var vector = Vector4.CreateScalarUnsafe(float.Pi);
            Assert.Equal(float.Pi, vector.X);

            vector = Vector4.CreateScalarUnsafe(float.E);
            Assert.Equal(float.E, vector.X);
        }
    }
}
