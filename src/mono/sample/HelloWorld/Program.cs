// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace HelloWorld
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Starting Hello World");
            var x = 1;
            System.Diagnostics.Debugger.Break();
            Console.WriteLine("Ending Hello World!");
            return;
        }
    }
}
