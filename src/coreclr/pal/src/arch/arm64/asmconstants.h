// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#ifndef __PAL_ARM64_ASMCONSTANTS_H__
#define __PAL_ARM64_ASMCONSTANTS_H__

#define CONTEXT_ARM64   0x00400000L

#define CONTEXT_CONTROL_BIT (0)
#define CONTEXT_INTEGER_BIT (1)
#define CONTEXT_FLOATING_POINT_BIT (2)
#define CONTEXT_DEBUG_REGISTERS_BIT (3)

#define CONTEXT_CONTROL (CONTEXT_ARM64 | (1L << CONTEXT_CONTROL_BIT))
#define CONTEXT_INTEGER (CONTEXT_ARM64 | (1 << CONTEXT_INTEGER_BIT))
#define CONTEXT_FLOATING_POINT  (CONTEXT_ARM64 | (1 << CONTEXT_FLOATING_POINT_BIT))
#define CONTEXT_DEBUG_REGISTERS (CONTEXT_ARM64 | (1 << CONTEXT_DEBUG_REGISTERS_BIT))

#define CONTEXT_FULL (CONTEXT_CONTROL | CONTEXT_INTEGER | CONTEXT_FLOATING_POINT)

#define CONTEXT_ARM64_XSTATE_BIT (5)
#define CONTEXT_ARM64_XSTATE (1 << CONTEXT_XSTATE_BIT)

#define CONTEXT_XSTATE_BIT CONTEXT_ARM64_XSTATE_BIT
#define CONTEXT_XSTATE CONTEXT_ARM64_XSTATE

#define XSTATE_ARM64_SVE_BIT (2)
#define XSTATE_MASK_ARM64_SVE (UI64(1) << (XSTATE_ARM64_SVE_BIT))

#define CONTEXT_ContextFlags 0
#define CONTEXT_Cpsr         CONTEXT_ContextFlags+4
#define CONTEXT_X0           CONTEXT_Cpsr+4
#define CONTEXT_X1           CONTEXT_X0+8
#define CONTEXT_X2           CONTEXT_X1+8
#define CONTEXT_X3           CONTEXT_X2+8
#define CONTEXT_X4           CONTEXT_X3+8
#define CONTEXT_X5           CONTEXT_X4+8
#define CONTEXT_X6           CONTEXT_X5+8
#define CONTEXT_X7           CONTEXT_X6+8
#define CONTEXT_X8           CONTEXT_X7+8
#define CONTEXT_X9           CONTEXT_X8+8
#define CONTEXT_X10          CONTEXT_X9+8
#define CONTEXT_X11          CONTEXT_X10+8
#define CONTEXT_X12          CONTEXT_X11+8
#define CONTEXT_X13          CONTEXT_X12+8
#define CONTEXT_X14          CONTEXT_X13+8
#define CONTEXT_X15          CONTEXT_X14+8
#define CONTEXT_X16          CONTEXT_X15+8
#define CONTEXT_X17          CONTEXT_X16+8
#define CONTEXT_X18          CONTEXT_X17+8
#define CONTEXT_X19          CONTEXT_X18+8
#define CONTEXT_X20          CONTEXT_X19+8
#define CONTEXT_X21          CONTEXT_X20+8
#define CONTEXT_X22          CONTEXT_X21+8
#define CONTEXT_X23          CONTEXT_X22+8
#define CONTEXT_X24          CONTEXT_X23+8
#define CONTEXT_X25          CONTEXT_X24+8
#define CONTEXT_X26          CONTEXT_X25+8
#define CONTEXT_X27          CONTEXT_X26+8
#define CONTEXT_X28          CONTEXT_X27+8
#define CONTEXT_Fp           CONTEXT_X28+8
#define CONTEXT_Lr           CONTEXT_Fp+8
#define CONTEXT_Sp           CONTEXT_Lr+8
#define CONTEXT_Pc           CONTEXT_Sp+8

#define CONTEXT_NEON_OFFSET  CONTEXT_Pc+8
#define CONTEXT_V0           0
#define CONTEXT_V1           CONTEXT_V0+16
#define CONTEXT_V2           CONTEXT_V1+16
#define CONTEXT_V3           CONTEXT_V2+16
#define CONTEXT_V4           CONTEXT_V3+16
#define CONTEXT_V5           CONTEXT_V4+16
#define CONTEXT_V6           CONTEXT_V5+16
#define CONTEXT_V7           CONTEXT_V6+16
#define CONTEXT_V8           CONTEXT_V7+16
#define CONTEXT_V9           CONTEXT_V8+16
#define CONTEXT_V10          CONTEXT_V9+16
#define CONTEXT_V11          CONTEXT_V10+16
#define CONTEXT_V12          CONTEXT_V11+16
#define CONTEXT_V13          CONTEXT_V12+16
#define CONTEXT_V14          CONTEXT_V13+16
#define CONTEXT_V15          CONTEXT_V14+16
#define CONTEXT_V16          CONTEXT_V15+16
#define CONTEXT_V17          CONTEXT_V16+16
#define CONTEXT_V18          CONTEXT_V17+16
#define CONTEXT_V19          CONTEXT_V18+16
#define CONTEXT_V20          CONTEXT_V19+16
#define CONTEXT_V21          CONTEXT_V20+16
#define CONTEXT_V22          CONTEXT_V21+16
#define CONTEXT_V23          CONTEXT_V22+16
#define CONTEXT_V24          CONTEXT_V23+16
#define CONTEXT_V25          CONTEXT_V24+16
#define CONTEXT_V26          CONTEXT_V25+16
#define CONTEXT_V27          CONTEXT_V26+16
#define CONTEXT_V28          CONTEXT_V27+16
#define CONTEXT_V29          CONTEXT_V28+16
#define CONTEXT_V30          CONTEXT_V29+16
#define CONTEXT_V31          CONTEXT_V30+16
#define CONTEXT_FLOAT_CONTROL_OFFSET  CONTEXT_V31+16
#define CONTEXT_Fpcr         0
#define CONTEXT_Fpsr         CONTEXT_Fpcr+4
#define CONTEXT_NEON_SIZE    CONTEXT_FLOAT_CONTROL_OFFSET+CONTEXT_Fpsr+4

#define CONTEXT_DEBUG_OFFSET CONTEXT_NEON_OFFSET+CONTEXT_NEON_SIZE
#define CONTEXT_DEBUG_SIZE   120 // (8*4)+(8*8)+(2*4)+(2*8)

#define CONTEXT_XSTATEFEATURESMASK_OFFSET CONTEXT_DEBUG_OFFSET+CONTEXT_DEBUG_SIZE

// TODO-SVE: Support Vector register sizes >128bit

#define CONTEXT_SVE_OFFSET   CONTEXT_XSTATEFEATURESMASK_OFFSET+8
#define CONTEXT_VL_OFFSET    0

// SVE register offsets are multiples of the vector length
#define CONTEXT_SVE_REGS_OFFSET   CONTEXT_VL_OFFSET+4
#define CONTEXT_FFR_VL       0
#define CONTEXT_P0_VL        CONTEXT_FFR_VL+1
#define CONTEXT_P1_VL        CONTEXT_P0_VL+1
#define CONTEXT_P2_VL        CONTEXT_P1_VL+1
#define CONTEXT_P3_VL        CONTEXT_P2_VL+1
#define CONTEXT_P4_VL        CONTEXT_P3_VL+1
#define CONTEXT_P5_VL        CONTEXT_P4_VL+1
#define CONTEXT_P6_VL        CONTEXT_P5_VL+1
#define CONTEXT_P7_VL        CONTEXT_P6_VL+1
#define CONTEXT_P8_VL        CONTEXT_P7_VL+1
#define CONTEXT_P9_VL        CONTEXT_P8_VL+1
#define CONTEXT_P10_VL       CONTEXT_P9_VL+1
#define CONTEXT_P11_VL       CONTEXT_P10_VL+1
#define CONTEXT_P12_VL       CONTEXT_P11_VL+1
#define CONTEXT_P13_VL       CONTEXT_P12_VL+1
#define CONTEXT_P14_VL       CONTEXT_P13_VL+1
#define CONTEXT_P15_VL       CONTEXT_P14_VL+1

#define CONTEXT_SVE_REGS_SIZE     ((CONTEXT_P15_VL+1) * 4)
#define CONTEXT_SVE_SIZE     CONTEXT_SVE_REGS_SIZE + 8

#define CONTEXT_Size         CONTEXT_SVE_OFFSET + CONTEXT_SVE_SIZE

#endif
