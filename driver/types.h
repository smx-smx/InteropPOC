#ifndef __TYPES_H
#define __TYPES_H

#ifdef _WINDOWS
#include <objbase.h>
#define EXPORT(type) __declspec(dllexport) type __cdecl
#define PACK( __Declaration__ ) __pragma( pack(push, 1) ) __Declaration__ __pragma( pack(pop) )
#else
#define EXPORT extern
#define PACK( __Declaration__ ) __Declaration__ __attribute__((__packed__))
#define STDAPICALLTYPE

typedef unsigned long UINT;
typedef int INT;
typedef long BOOL;
typedef char BYTE;
typedef long LONG;
typedef unsigned long ULONG;
typedef unsigned short WORD;
typedef unsigned long DWORD;
typedef unsigned short VARTYPE;
typedef unsigned short USHORT;
typedef DWORD LCID;
typedef LONG SCODE;
typedef short SHORT;
typedef wchar_t WCHAR;
typedef WCHAR TCHAR;
typedef WCHAR OLECHAR;

typedef struct _GUID {
DWORD Data1;
WORD  Data2;
WORD  Data3;
BYTE  Data4[8];
} GUID;

typedef GUID IID;
typedef IID* LPIID;

typedef /*[ptr]*/ void* HWND;
typedef /*[ptr]*/ void* HMENU;
typedef /*[ptr]*/ void* HANDLE;
typedef /*[ref]*/ GUID* REFGUID;
typedef /*[ref]*/ IID* REFIID;



typedef char* LPWSTR;

struct IUnknown {
     virtual int QueryInterface(REFIID, void ** out) = 0;
     virtual int AddRef();
     virtual int Release();
};
#endif 

#endif
