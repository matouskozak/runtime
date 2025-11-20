// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
//*****************************************************************************
// File: executioncontrol.h
//
// Abstraction for breakpoint and single-step operations across different
// code execution strategies (JIT, interpreter, R2R).
//
//*****************************************************************************

#ifndef EXECUTIONCONTROL_H_
#define EXECUTIONCONTROL_H_

struct DebuggerControllerPatch;

#ifdef FEATURE_INTERPRETER


class IExecutionControl
{
public:
    virtual ~IExecutionControl() = default;

    virtual bool ApplyPatch(DebuggerControllerPatch* patch) = 0;
    virtual bool UnapplyPatch(DebuggerControllerPatch* patch) = 0;
    virtual bool MatchPatch(DebuggerControllerPatch* patch, CONTEXT* context) = 0;
    virtual const BYTE* ExtractBreakpointAddress(EXCEPTION_RECORD* pExceptionRecord, CONTEXT* context) = 0;
    virtual bool IsBreakpointPatched(const BYTE* address) const = 0;
};

typedef DPTR(IExecutionControl) PTR_IExecutionControl;

// Interpreter execution control using bytecode patching
class InterpreterExecutionControl : public IExecutionControl
{
public:
    static InterpreterExecutionControl* GetInstance();

    // Apply a breakpoint patch - replaces bytecode opcode with INTOP_BREAKPOINT
    virtual bool ApplyPatch(DebuggerControllerPatch* patch) override;

    // Remove a breakpoint patch - restores original bytecode opcode
    virtual bool UnapplyPatch(DebuggerControllerPatch* patch) override;

    // Check if a patch matches the current execution context
    virtual bool MatchPatch(DebuggerControllerPatch* patch, CONTEXT* context) override;

    // Extract the breakpoint address from an exception record
    // Returns bytecode address from ExceptionInformation[0]
    virtual const BYTE* ExtractBreakpointAddress(EXCEPTION_RECORD* pExceptionRecord, CONTEXT* context) override;

    // Check if a breakpoint is currently installed at the given address
    // Checks if bytecode at address is INTOP_BREAKPOINT
    virtual bool IsBreakpointPatched(const BYTE* address) const override;

private:
    InterpreterExecutionControl() = default;
    static InterpreterExecutionControl s_instance;
};

#endif // FEATURE_INTERPRETER
#endif // EXECUTIONCONTROL_H_
