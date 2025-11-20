// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
//*****************************************************************************
// File: executioncontrol.cpp
//
// Implementation of execution control abstractions
//
//*****************************************************************************

#include "stdafx.h"
#include "executioncontrol.h"
#include "controller.h"
#include "../../vm/codeman.h"

#ifdef FEATURE_INTERPRETER
#include "../../interpreter/intops.h"
#endif

#if !defined(DACCESS_COMPILE)
#ifdef FEATURE_INTERPRETER

//=============================================================================
// InterpreterExecutionControl - Interpreter bytecode breakpoints
//=============================================================================

InterpreterExecutionControl InterpreterExecutionControl::s_instance;

InterpreterExecutionControl* InterpreterExecutionControl::GetInstance()
{
    return &s_instance;
}

bool InterpreterExecutionControl::ApplyPatch(DebuggerControllerPatch* patch)
{
    _ASSERTE(patch != NULL);
    _ASSERTE(!patch->IsActivated());
    _ASSERTE(patch->IsBound());

    LOG((LF_CORDB, LL_INFO10000, "InterpreterEC::ApplyPatch %p at addr %p\n",
        patch, patch->address));

    patch->SetKind(PATCH_KIND_NATIVE_INTERPRETER);
    // TODO: verify that patch->address is valid interpreter address
    patch->opcode = CORDbgGetInstruction(patch->address);
    *(uint32_t*)patch->address = INTOP_BREAKPOINT;

    LOG((LF_CORDB, LL_EVERYTHING, "InterpreterEC::ApplyPatch Breakpoint inserted at %p, saved opcode %x\n",
        patch->address, patch->opcode));

    return true;
}

bool InterpreterExecutionControl::UnapplyPatch(DebuggerControllerPatch* patch)
{
    _ASSERTE(patch != NULL);
    _ASSERTE(patch->address != NULL);
    _ASSERTE(patch->IsActivated());

    LOG((LF_CORDB, LL_INFO1000, "InterpreterEC::UnapplyPatch %p\n", patch));

    // Restore the original opcode
    *(uint32_t*)patch->address = patch->opcode;
    InitializePRD(&(patch->opcode));

    LOG((LF_CORDB, LL_EVERYTHING, "InterpreterEC::UnapplyPatch Restored opcode at %p\n",
        patch->address));

    return true;
}

bool InterpreterExecutionControl::MatchPatch(DebuggerControllerPatch* patch, CONTEXT* context)
{
    _ASSERTE(patch != NULL);
    _ASSERTE(context != NULL);
    LOG((LF_CORDB, LL_INFO1000, "InterpreterEC::MatchPatch %p\n", patch));
    // TODO:
    return true;
}

const BYTE* InterpreterExecutionControl::ExtractBreakpointAddress(EXCEPTION_RECORD* pExceptionRecord, CONTEXT* context)
{
    _ASSERTE(pExceptionRecord != NULL);
    
    // For interpreter breakpoints, the exception parameters contain:
    // ExceptionInformation[0] = PC: bytecode instruction pointer
    // ExceptionInformation[1] = SP: interpreter frame pointer
    // ExceptionInformation[2] = stack pointer
    if (pExceptionRecord->NumberParameters >= 3 && 
        pExceptionRecord->ExceptionInformation[1] != 0)
    {
        return (const BYTE*)pExceptionRecord->ExceptionInformation[0];
    }
    
    // Fallback to context IP (shouldn't happen for valid interpreter breakpoints)
    return (const BYTE*)GetIP(context);
}

bool InterpreterExecutionControl::IsBreakpointPatched(const BYTE* address) const
{
    _ASSERTE(address != nullptr);
    // Check if the bytecode at the address is INTOP_BREAKPOINT
    const int32_t* pBytecode = (const int32_t*)address;
    return (*pBytecode == INTOP_BREAKPOINT);
}

#endif // FEATURE_INTERPRETER
#endif // !DACCESS_COMPILE
