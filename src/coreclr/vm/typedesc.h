// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
//
// File: typedesc.h
//
//
//
// ============================================================================


#ifndef TYPEDESC_H
#define TYPEDESC_H
#include <specstrings.h>

class TypeHandleList;

/*************************************************************************/
/* TypeDesc is a discriminated union of all types that can not be directly
   represented by a simple MethodTable*.   The discrimintor of the union at the present
   time is the CorElementType numeration.  The subclass of TypeDesc are
   the possible variants of the union.


   ParamTypeDescs only include byref, array and pointer types.  They do NOT
   include instantiations of generic types, which are represented by MethodTables.
*/


typedef DPTR(class TypeDesc) PTR_TypeDesc;

class TypeDesc
{
public:
#ifndef DACCESS_COMPILE
    TypeDesc(CorElementType type) {
        LIMITED_METHOD_CONTRACT;

        _typeAndFlags = type;
    }
#endif

    // This is the ELEMENT_TYPE* that would be used in the type sig for this type
    // For enums this is the underlying type
    inline CorElementType GetInternalCorElementType() {
        LIMITED_METHOD_DAC_CONTRACT;

        return (CorElementType) (_typeAndFlags & 0xff);
    }

    // Get the exact parent (superclass) of this type
    TypeHandle GetParent();

    // Returns the name of the array.  Note that it returns
    // the length of the returned string
    static void ConstructName(CorElementType kind,
                              TypeHandle param,
                              int rank,
                              SString &ssBuff);

    void GetName(SString &ssBuf);

    //-------------------------------------------------------------------
    // CASTING
    //
    // There are two variants of the "CanCastTo" method:
    //
    // CanCastTo
    // - might throw, might trigger GC
    // - return type is boolean (FALSE = cannot cast, TRUE = can cast)
    //
    // CanCastToCached
    // - does not throw, does not trigger GC
    // - return type is three-valued (CanCast, CannotCast, MaybeCast)
    //
    // MaybeCast indicates an inconclusive result
    // - the test result could not be obtained from a cache
    //   so the caller should now call CanCastTo if it cares
    //

    BOOL CanCastTo(TypeHandle type, TypeHandlePairList *pVisited);
    TypeHandle::CastResult CanCastToCached(TypeHandle type);

    static BOOL CanCastParam(TypeHandle fromParam, TypeHandle toParam, TypeHandlePairList *pVisited);

#ifndef DACCESS_COMPILE
    BOOL IsEquivalentTo(TypeHandle type COMMA_INDEBUG(TypeHandlePairList *pVisited));
#endif

    // BYREF
    BOOL IsByRef() {              // BYREFS are often treated specially
        WRAPPER_NO_CONTRACT;

        return(GetInternalCorElementType() == ELEMENT_TYPE_BYREF);
    }

    // PTR
    BOOL IsPointer() {
        WRAPPER_NO_CONTRACT;

        return(GetInternalCorElementType() == ELEMENT_TYPE_PTR);
    }

    // VAR, MVAR
    BOOL IsGenericVariable();

    // ELEMENT_TYPE_FNPTR
    BOOL IsFnPtr();

    // VALUETYPE
    BOOL IsNativeValueType();

    // Is actually ParamTypeDesc (BYREF, PTR)
    BOOL HasTypeParam();


    BOOL HasTypeEquivalence() const
    {
        LIMITED_METHOD_CONTRACT;
        return (_typeAndFlags & TypeDesc::enum_flag_HasTypeEquivalence) != 0;
    }

    BOOL IsFullyLoaded() const
    {
        LIMITED_METHOD_CONTRACT;

        return (_typeAndFlags & TypeDesc::enum_flag_IsNotFullyLoaded) == 0;
    }

    VOID SetIsFullyLoaded()
    {
        LIMITED_METHOD_CONTRACT;
        InterlockedAnd((LONG*)&_typeAndFlags, ~TypeDesc::enum_flag_IsNotFullyLoaded);
    }

    ClassLoadLevel GetLoadLevel();

    void DoFullyLoad(Generics::RecursionGraph *pVisited, ClassLoadLevel level,
                     DFLPendingList *pPending, BOOL *pfBailed, const InstantiationContext *pInstContext);

    // The module that defined the underlying type
    PTR_Module GetModule();

    // The module where this type lives for the purposes of loading and prejitting
    // See ComputeLoaderModule for more information
    PTR_Module GetLoaderModule();

    // The assembly that defined this type (== GetModule()->GetAssembly())
    Assembly* GetAssembly();

    PTR_MethodTable  GetMethodTable();               // only meaningful for ParamTypeDesc
    TypeHandle GetTypeParam();                       // only meaningful for ParamTypeDesc
    Instantiation GetClassOrArrayInstantiation();    // only meaningful for ParamTypeDesc; see above
    TypeHandle GetRootTypeParam();                   // only allowed for ParamTypeDesc, helper method used to avoid recursion

    PTR_LoaderAllocator GetLoaderAllocator()
    {
        SUPPORTS_DAC;

        return GetLoaderModule()->GetLoaderAllocator();
    }

    BOOL IsSharedByGenericInstantiations();

    BOOL ContainsGenericVariables(BOOL methodOnly);

#ifndef DACCESS_COMPILE
    OBJECTREF GetManagedClassObject();

    OBJECTREF GetManagedClassObjectIfExists() const
    {
        CONTRACTL
        {
            NOTHROW;
            GC_NOTRIGGER;
            MODE_COOPERATIVE;
        }
        CONTRACTL_END;

        const RUNTIMETYPEHANDLE handle = _exposedClassObject;
        OBJECTREF retVal = ObjectToOBJECTREF(handle);
        return retVal;
    }
#endif

 protected:
    // See methodtable.h for details of the flags with the same name there
    enum
    {
        // unused                        = 0x00000100,
        // unused                        = 0x00000200,
        // unused                        = 0x00000400,
        // unused                        = 0x00000800,
        enum_flag_IsNotFullyLoaded       = 0x00001000,
        enum_flag_DependenciesLoaded     = 0x00002000,
        enum_flag_HasTypeEquivalence     = 0x00004000
    };
    //
    // Low-order 8 bits of this flag are used to store the CorElementType, which
    // discriminates what kind of TypeDesc we are
    //
    // The remaining bits are available for flags
    //
    DWORD _typeAndFlags;

    // internal RuntimeType object handle
    RUNTIMETYPEHANDLE _exposedClassObject;
    friend struct ::cdac_data<TypeDesc>;
};

template<>
struct cdac_data<TypeDesc>
{
    static constexpr size_t TypeAndFlags = offsetof(TypeDesc, _typeAndFlags);
};

/*************************************************************************/
// This variant is used for parameterized types that have exactly one argument
// type.  This includes arrays, byrefs, pointers.

typedef DPTR(class ParamTypeDesc) PTR_ParamTypeDesc;


class ParamTypeDesc : public TypeDesc {
    friend class TypeDesc;
    friend class JIT_TrialAlloc;
    friend class CheckAsmOffsets;

public:
#ifndef DACCESS_COMPILE
    ParamTypeDesc(CorElementType type, TypeHandle arg)
        : TypeDesc(type), m_Arg(arg) {

        LIMITED_METHOD_CONTRACT;

        // ParamTypeDescs start out life not fully loaded
        _typeAndFlags |= TypeDesc::enum_flag_IsNotFullyLoaded;

        // Param type descs can only be equivalent if their constituent bits are equivalent.
        if (arg.HasTypeEquivalence())
        {
            _typeAndFlags |= TypeDesc::enum_flag_HasTypeEquivalence;
        }

        INDEBUGIMPL(Verify());
    }
#endif

    INDEBUGIMPL(BOOL Verify();)

    TypeHandle GetModifiedType()
    {
        LIMITED_METHOD_CONTRACT;

        return m_Arg;
    }

    TypeHandle GetTypeParam();

#ifdef DACCESS_COMPILE
    void EnumMemoryRegions(CLRDataEnumMemoryFlags flags);
#endif

    friend class StubLinkerCPU;

    friend class ArrayOpLinker;
protected:

    // the _typeAndFlags field in TypeDesc tell what kind of parameterized type we have

    // The type that is being modified
    TypeHandle        m_Arg;
    friend struct ::cdac_data<ParamTypeDesc>;
};

template<>
struct cdac_data<ParamTypeDesc>
{
    static constexpr size_t TypeArg = offsetof(ParamTypeDesc, m_Arg);
};

/*************************************************************************/
// These are for verification of generic code and reflection over generic code.
// Each TypeVarTypeDesc represents a class or method type variable, as specified by a GenericParam entry.
// The type variables are tied back to the class or method that *defines* them.
// This is done through typedef or methoddef tokens.

class TypeVarTypeDesc : public TypeDesc
{
public:

#ifndef DACCESS_COMPILE

    TypeVarTypeDesc(PTR_Module pModule, mdToken typeOrMethodDef, unsigned int index, mdGenericParam token) :
        TypeDesc(TypeFromToken(typeOrMethodDef) == mdtTypeDef ? ELEMENT_TYPE_VAR : ELEMENT_TYPE_MVAR)
    {
        CONTRACTL
        {
            NOTHROW;
            GC_NOTRIGGER;
            PRECONDITION(CheckPointer(pModule));
            PRECONDITION(TypeFromToken(typeOrMethodDef) == mdtTypeDef || TypeFromToken(typeOrMethodDef) == mdtMethodDef);
            PRECONDITION(index >= 0);
            PRECONDITION(TypeFromToken(token) == mdtGenericParam);
        }
        CONTRACTL_END;

        m_pModule = pModule;
        m_typeOrMethodDef = typeOrMethodDef;
        m_token = token;
        m_index = index;
        m_constraints = NULL;
        m_numConstraints = (DWORD)-1;
    }
#endif // #ifndef DACCESS_COMPILE

    // placement new operator
    void* operator new(size_t size, void* spot) { LIMITED_METHOD_CONTRACT;  return (spot); }

    PTR_Module GetModule()
    {
        LIMITED_METHOD_CONTRACT;
        SUPPORTS_DAC;

        return m_pModule;
    }

    unsigned int GetIndex()
    {
        LIMITED_METHOD_CONTRACT;
        SUPPORTS_DAC;
        return m_index;
    }

    mdGenericParam GetToken()
    {
        LIMITED_METHOD_CONTRACT;
        SUPPORTS_DAC;
        return m_token;
    }

    mdToken GetTypeOrMethodDef()
    {
        LIMITED_METHOD_CONTRACT;
        SUPPORTS_DAC;
        return m_typeOrMethodDef;
    }

    // Load the owning type. Note that the result is not guaranteed to be full loaded
    MethodDesc * LoadOwnerMethod();
    TypeHandle LoadOwnerType();

    BOOL ConstraintsLoaded() { LIMITED_METHOD_CONTRACT; return m_numConstraints != (DWORD)-1; }

    // Return NULL if no constraints are specified
    // Return an array of type handles if constraints are specified,
    // with the number of constraints returned in pNumConstraints
    TypeHandle* GetCachedConstraints(DWORD *pNumConstraints);
    TypeHandle* GetConstraints(DWORD *pNumConstraints, ClassLoadLevel level = CLASS_LOADED);

    // Load the constraints if not already loaded
    void LoadConstraints(ClassLoadLevel level = CLASS_LOADED);

    // Check the constraints on this type parameter hold in the supplied context for the supplied type
    BOOL SatisfiesConstraints(SigTypeContext *pTypeContext, TypeHandle thArg,
                              const InstantiationContext *pInstContext = NULL);

    // Check whether the constraints on this type force it to be a reference type (i.e. it is impossible
    // to instantiate it with a value type).
    BOOL ConstrainedAsObjRef();

    // Check whether the constraints on this type force it to be a value type (i.e. it is impossible to
    // instantiate it with a reference type).
    BOOL ConstrainedAsValueType();

#ifdef DACCESS_COMPILE
    void EnumMemoryRegions(CLRDataEnumMemoryFlags flags);
#endif

protected:
    BOOL ConstrainedAsObjRefHelper();

    // Module containing the generic definition, also the loader module for this type desc
    PTR_Module m_pModule;

    // Declaring type or method
    mdToken m_typeOrMethodDef;

    // Constraints, determined on first call to GetConstraints
    Volatile<DWORD> m_numConstraints;    // -1 until number has been determined
    PTR_TypeHandle m_constraints;

    // token for GenericParam entry
    mdGenericParam    m_token;

    // index within declaring type or method, numbered from zero
    unsigned int m_index;

    friend struct ::cdac_data<TypeVarTypeDesc>;
};

template<>
struct cdac_data<TypeVarTypeDesc>
{
    static constexpr size_t Module = offsetof(TypeVarTypeDesc, m_pModule);
    static constexpr size_t Token = offsetof(TypeVarTypeDesc, m_token);
};

/*************************************************************************/
/* represents a function type.  */

typedef SPTR(class FnPtrTypeDesc) PTR_FnPtrTypeDesc;

class FnPtrTypeDesc : public TypeDesc
{

public:
#ifndef DACCESS_COMPILE
    FnPtrTypeDesc(BYTE callConv, DWORD numArgs, TypeHandle * retAndArgTypes, PTR_Module pLoaderModule)
        : TypeDesc(ELEMENT_TYPE_FNPTR), m_pLoaderModule(pLoaderModule), m_NumArgs(numArgs), m_CallConv(callConv)
    {
        LIMITED_METHOD_CONTRACT;
        for (DWORD i = 0; i <= numArgs; i++)
        {
            m_RetAndArgTypes[i] = retAndArgTypes[i];
        }
    }
#endif //!DACCESS_COMPILE

    DWORD GetNumArgs()
    {
        LIMITED_METHOD_CONTRACT;
        SUPPORTS_DAC;
        return m_NumArgs;
    }

    BYTE GetCallConv()
    {
        LIMITED_METHOD_CONTRACT;
        SUPPORTS_DAC;
        _ASSERTE(FitsIn<BYTE>(m_CallConv));
        return static_cast<BYTE>(m_CallConv);
    }

    // Return a pointer to the types of the signature, return type followed by argument types
    // The type handles are guaranteed to be fixed up
    TypeHandle * GetRetAndArgTypes();
    // As above, but const version
    const TypeHandle * GetRetAndArgTypes() const
    {
        WRAPPER_NO_CONTRACT;
        return const_cast<FnPtrTypeDesc *>(this)->GetRetAndArgTypes();
    }

    // As above, but the type handles might be zap-encodings that need fixing up explicitly
    PTR_TypeHandle GetRetAndArgTypesPointer()
    {
        LIMITED_METHOD_CONTRACT;
        SUPPORTS_DAC;

        return PTR_TypeHandle(m_RetAndArgTypes);
    }

    BOOL IsSharedByGenericInstantiations();

    BOOL ContainsGenericVariables(BOOL methodOnly);

#ifndef DACCESS_COMPILE
    // Returns TRUE if all return and argument types are externally visible.
    BOOL IsExternallyVisible() const;
#endif //DACCESS_COMPILE

    PTR_Module GetLoaderModule() const { LIMITED_METHOD_DAC_CONTRACT; return m_pLoaderModule; }

#ifdef DACCESS_COMPILE
    static ULONG32 DacSize(TADDR addr)
    {
        DWORD numArgs = *PTR_DWORD(addr + offsetof(FnPtrTypeDesc, m_NumArgs));
        return (offsetof(FnPtrTypeDesc, m_RetAndArgTypes) +
            (numArgs * sizeof(TypeHandle)));
    }

    void EnumMemoryRegions(CLRDataEnumMemoryFlags flags);
#endif //DACCESS_COMPILE

protected:
    // LoaderModule of the TypeDesc
    PTR_Module m_pLoaderModule;

    // Number of arguments
    DWORD m_NumArgs;

    // Calling convention (actually just a single byte)
    DWORD m_CallConv;

    // Return type first, then argument types
    TypeHandle m_RetAndArgTypes[1];

    friend struct ::cdac_data<FnPtrTypeDesc>;
}; // class FnPtrTypeDesc

template<>
struct cdac_data<FnPtrTypeDesc>
{
    static constexpr size_t NumArgs = offsetof(FnPtrTypeDesc, m_NumArgs);
    static constexpr size_t RetAndArgTypes = offsetof(FnPtrTypeDesc, m_RetAndArgTypes);
    static constexpr size_t CallConv = offsetof(FnPtrTypeDesc, m_CallConv);
    static constexpr size_t LoaderModule = offsetof(FnPtrTypeDesc, m_pLoaderModule);
};

#endif // TYPEDESC_H
