// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Runtime.Intrinsics.Arm
{
    // Used to specify or limit the number of elements used within an method.
    // Matches the field "pattern" within the Arm Architecture Reference Manual
    public enum SveMaskPattern : byte
    {
        /// <summary>POW2</summary>
        LargestPowerOf2 = 0,      // The largest power of 2.

        /// <summary>VL1</summary>
        VectorCount1 = 1,         // Exactly 1 element.

        /// <summary>VL2</summary>
        VectorCount2 = 2,         // Exactly 2 elements.

        /// <summary>VL3</summary>
        VectorCount3 = 3,         // Exactly 3 elements.

        /// <summary>VL4</summary>
        VectorCount4 = 4,         // Exactly 4 elements.

        /// <summary>VL5</summary>
        VectorCount5 = 5,         // Exactly 5 elements.

        /// <summary>VL6</summary>
        VectorCount6 = 6,         // Exactly 6 elements.

        /// <summary>VL7</summary>
        VectorCount7 = 7,         // Exactly 7 elements.

        /// <summary>VL8</summary>
        VectorCount8 = 8,         // Exactly 8 elements.

        /// <summary>VL16</summary>
        VectorCount16 = 9,        // Exactly 16 elements.

        /// <summary>VL32</summary>
        VectorCount32 = 10,       // Exactly 32 elements.

        /// <summary>VL64</summary>
        VectorCount64 = 11,       // Exactly 64 elements.

        /// <summary>VL128</summary>
        VectorCount128 = 12,      // Exactly 128 elements.

        /// <summary>VL256</summary>
        VectorCount256 = 13,      // Exactly 256 elements.

        /// <summary>MUL4</summary>
        LargestMultipleOf4 = 29,  // The largest multiple of 4.

        /// <summary>MUL3</summary>
        LargestMultipleOf3 = 30,  // The largest multiple of 3.

        /// <summary>ALL</summary>
        All  = 31                 // All available (implicitly a multiple of two).
    }

    public enum SvePrefetchType : byte
    {
        /// <summary>PLDL1KEEP</summary>
        LoadL1Temporal = 0,

        /// <summary>PLDL1STRM</summary>
        LoadL1NonTemporal = 1,

        /// <summary>PLDL2KEEP</summary>
        LoadL2Temporal = 2,

        /// <summary>PLDL2STRM</summary>
        LoadL2NonTemporal = 3,

        /// <summary>PLDL3KEEP</summary>
        LoadL3Temporal = 4,

        /// <summary>PLDL3STRM</summary>
        LoadL3NonTemporal = 5,

        /// <summary>PSTL1KEEP</summary>
        StoreL1Temporal = 8,

        /// <summary>PSTL1STRM</summary>
        StoreL1NonTemporal = 9,

        /// <summary>PSTL2KEEP</summary>
        StoreL2Temporal = 10,

        /// <summary>PSTL2STRM</summary>
        StoreL2NonTemporal = 11,

        /// <summary>PSTL3KEEP</summary>
        StoreL3Temporal = 12,

        /// <summary>PSTL3STRM</summary>
        StoreL3NonTemporal = 13
    };
}
