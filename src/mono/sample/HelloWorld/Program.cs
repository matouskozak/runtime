// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.Intrinsics;
using System.Runtime.CompilerServices;

namespace HelloWorld
{
    internal class Program
    {

        static Vector128<short> vecShort = Vector128.Create(
                0x0001,
                0x8000,
                0x0001,
                0x8000,
                0x0001,
                0x8000,
                0x0001,
                0x8000
            ).AsInt16();
        

        [MethodImplAttribute(MethodImplOptions.NoInlining)]
        private static uint TestShort()
        {
            return Vector128.ExtractMostSignificantBits(vecShort);
        }


/*

        [MethodImplAttribute(MethodImplOptions.NoInlining)]
        private static uint TestInt(Vector128<int> vecInt)
        {
            return Vector128.ExtractMostSignificantBits(vecInt);
        }

        [MethodImplAttribute(MethodImplOptions.NoInlining)]
        private static int Test2Int(Vector128<int> vecInt)
        {
            return Vector128.Sum(vecInt);
        }

*/

/*

        static Vector128<byte> vecByte = Vector128.Create(
                0x01,
                0x80,
                0x01,
                0x80,
                0x01,
                0x80,
                0x01,
                0x80,
                0x01,
                0x80,
                0x01,
                0x80,
                0x01,
                0x80,
                0x01,
                0x80
        );

        [MethodImplAttribute(MethodImplOptions.NoInlining)]
        private static uint TestByte()
        {
            return Vector128.ExtractMostSignificantBits(vecByte);
        }

        static Vector128<sbyte> vecSByte = Vector128.Create(
                0x01,
                0x80,
                0x01,
                0x80,
                0x01,
                0x80,
                0x01,
                0x80,
                0x01,
                0x80,
                0x01,
                0x80,
                0x01,
                0x80,
                0x01,
                0x80
            ).AsSByte();
        
        
        [MethodImplAttribute(MethodImplOptions.NoInlining)]

        private static uint TestSByte()
        {
            return Vector128.ExtractMostSignificantBits(vecSByte);
        }
*/

/*
        static Vector64<byte> vec64Byte = Vector64.Create(
                0x01,
                0x80,
                0x01,
                0x80,
                0x01,
                0x80,
                0x01,
                0x80
            );

        [MethodImplAttribute(MethodImplOptions.NoInlining)]
        private static uint Test64Byte()
        {
            return Vector64.ExtractMostSignificantBits(vec64Byte);
        }

*/

/*
        static Vector128<float> vecFloat = Vector128.Create(
                +1.0f,
                -0.0f,
                +1.0f,
                -0.0f
            );

        [MethodImplAttribute(MethodImplOptions.NoInlining)]
        private static uint TestFloat()
        {
            return Vector128.ExtractMostSignificantBits(vecFloat);
        }
*/

        private static void Main(string[] args)
        {
            bool isMono = typeof(object).Assembly.GetType("Mono.RuntimeStructs") != null;
            Console.WriteLine($"Hello World {(isMono ? "from Mono!" : "from CoreCLR!")}");
            Console.WriteLine(typeof(object).Assembly.FullName);
            Console.WriteLine(System.Reflection.Assembly.GetEntryAssembly ());
            Console.WriteLine(System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription);
         
/*         
            Console.WriteLine("Test Int");
            Vector128<int> vecInt = Vector128.Create(
                0x00000001U,
                0x80000000U,
                0x00000001U,
                0x80000000U
            ).AsInt32();

            Console.WriteLine(vecInt);
            uint resInt = TestInt(vecInt);
            Console.WriteLine(Convert.ToString(0b1010u, 2));
            Console.WriteLine(Convert.ToString(resInt, 2));


            if (resInt != 0b1010u)
                throw new Exception("TestInt failed");
*/



            Console.WriteLine("Test Short");
            Console.WriteLine(vecShort);
            uint resShort = TestShort();
            Console.WriteLine(Convert.ToString(0b10101010u, 2));
            Console.WriteLine(Convert.ToString(resShort, 2));

            if (resShort != 0b10101010u)
                throw new Exception("TestShort failed");


/*
            Console.WriteLine("Test Byte");
            Console.WriteLine(vecByte);
            uint resByte = TestByte();
            Console.WriteLine(Convert.ToString(0b10101010_10101010u, 2));
            Console.WriteLine(Convert.ToString(resByte, 2));

            if (resByte != 0b10101010_10101010u)
                throw new Exception("TestByte failed"); 


            Console.WriteLine("Test Sign Byte");
            Console.WriteLine(vecSByte);
            uint resSByte = TestSByte();
            Console.WriteLine(Convert.ToString(0b10101010_10101010u, 2));
            Console.WriteLine(Convert.ToString(resSByte, 2));

            if (resSByte != 0b10101010_10101010u)
                throw new Exception("TestSByte failed"); 
*/

/*

            Console.WriteLine("Test Float");
            Console.WriteLine(vecFloat);
            uint resFloat = TestFloat();
            Console.WriteLine(Convert.ToString(0b1010u, 2));
            Console.WriteLine(Convert.ToString(resFloat, 2));

            if (resFloat != 0b1010u)
                throw new Exception("TestFloat failed");         

*/

/*
            Console.WriteLine("Test 64 Byte");
            Console.WriteLine(vec64Byte);
            uint res64Byte = Test64Byte();
            Console.WriteLine(Convert.ToString(0b10101010u, 2));
            Console.WriteLine(Convert.ToString(res64Byte, 2));

            if (res64Byte != 0b10101010u)
                throw new Exception("Test64Byte failed"); 
                */
        }
    }
}