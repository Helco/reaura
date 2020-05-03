///////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 1999 Microsoft Corporation.  All Rights Reserved.
//
//  File:       d3dx8core.h
//  Content:    D3DX core types and functions
//
///////////////////////////////////////////////////////////////////////////

#ifndef __D3DX8CORE_H__
#define __D3DX8CORE_H__


struct ID3DXBuffer; struct ID3DXBuffer_vtbl;
struct ID3DXBuffer; struct ID3DXBuffer_vtbl;

struct ID3DXBuffer { ID3DXBuffer_vtbl* vtptr; };
struct ID3DXBuffer_vtbl 
{
    // IUnknown
    DWORD (__stdcall *QueryInterface)(void* this, void* iid, LPVOID *ppv);
    ULONG (__stdcall *AddRef)(void* this);
    ULONG (__stdcall *Release)(void* this);

    // ID3DXBuffer
    LPVOID (__stdcall *GetBufferPointer)(void* this);
    DWORD (__stdcall *GetBufferSize)(void* this);
};





///////////////////////////////////////////////////////////////////////////
// ID3DXFont:
// ----------
// Font objects contain the textures and resources needed to render
// a specific font on a specific device.
//
// Begin -
//    Prepartes device for drawing text.  This is optional.. if DrawText
//    is called outside of Begin/End, it will call Begin and End for you.
//
// DrawText -
//    Draws formatted text on a D3D device.  Some parameters are 
//    surprisingly similar to those of GDI's DrawText function.  See GDI 
//    documentation for a detailed description of these parameters.
//
// End -
//    Restores device state to how it was when Begin was called.
///////////////////////////////////////////////////////////////////////////

struct ID3DXFont; struct ID3DXFont_vtbl;
struct ID3DXFont; struct ID3DXFont_vtbl;

struct ID3DXFont { ID3DXFont_vtbl* vtptr; };
struct ID3DXFont_vtbl 
{
    // IUnknown
    DWORD (__stdcall *QueryInterface)(void* this, void* iid, LPVOID *ppv);
    ULONG (__stdcall *AddRef)(void* this);
    ULONG (__stdcall *Release)(void* this);

    // ID3DXFont
    DWORD (__stdcall *GetDevice)(void* this, IDirect3DDevice8** ppDevice);
    DWORD (__stdcall *GetLogFont)(void* this, LOGFONT* pLogFont);

    DWORD (__stdcall *Begin)(void* this);

    INT (__stdcall *DrawTextA)(void* this, LPCSTR  pString, INT Count, LPRECT pRect, DWORD Format, D3DCOLOR Color);
    INT (__stdcall *DrawTextW)(void* this, LPCWSTR pString, INT Count, LPRECT pRect, DWORD Format, D3DCOLOR Color);

    DWORD (__stdcall *End)(void* this);
};


struct ID3DXSprite; struct ID3DXSprite_vtbl;
struct ID3DXSprite; struct ID3DXSprite_vtbl;

struct ID3DXSprite { ID3DXSprite_vtbl* vtptr; };
struct ID3DXSprite_vtbl 
{
    // IUnknown
    DWORD (__stdcall *QueryInterface)(void* this, void* iid, LPVOID *ppv);
    ULONG (__stdcall *AddRef)(void* this);
    ULONG (__stdcall *Release)(void* this);

    // ID3DXSprite
    DWORD (__stdcall *GetDevice)(void* this, IDirect3DDevice8** ppDevice);

    DWORD (__stdcall *Begin)(void* this);

    DWORD (__stdcall *Draw)(void* this, IDirect3DTexture8*  pSrcTexture, 
         RECT* pSrcRect,  float* pScaling, 
         float* pRotationCenter, FLOAT Rotation, 
         float* pTranslation, D3DCOLOR Color);

    DWORD (__stdcall *DrawTransform)(void* this, IDirect3DTexture8* pSrcTexture, 
         RECT* pSrcRect,  float* pTransform, 
        D3DCOLOR Color);

    DWORD (__stdcall *End)(void* this);
};


///////////////////////////////////////////////////////////////////////////
// ID3DXRenderToSurface:
// ---------------------
// This object abstracts rendering to surfaces.  These surfaces do not 
// necessarily need to be render targets.  If they are not, a compatible
// render target is used, and the result copied into surface at end scene.
///////////////////////////////////////////////////////////////////////////

typedef struct _D3DXRTS_DESC
{
    UINT                Width;
    UINT                Height;
    D3DFORMAT           Format;
    BOOL                DepthStencil;
    D3DFORMAT           DepthStencilFormat;

} D3DXRTS_DESC;


struct ID3DXRenderToSurface; struct ID3DXRenderToSurface_vtbl;
struct ID3DXRenderToSurface; struct ID3DXRenderToSurface_vtbl;


struct ID3DXRenderToSurface { ID3DXRenderToSurface_vtbl* vtptr; };
struct ID3DXRenderToSurface_vtbl 
{
    // IUnknown
    DWORD (__stdcall *QueryInterface)(void* this, void* iid, LPVOID *ppv);
    ULONG (__stdcall *AddRef)(void* this);
    ULONG (__stdcall *Release)(void* this);

    // ID3DXRenderToSurface
    DWORD (__stdcall *GetDevice)(void* this, IDirect3DDevice8** ppDevice);
    DWORD (__stdcall *GetDesc)(void* this, D3DXRTS_DESC* pDesc);

    DWORD (__stdcall *BeginScene)(void* this, IDirect3DSurface8* pSurface,  D3DVIEWPORT8* pViewport);
    DWORD (__stdcall *EndScene)(void* this);
};


///////////////////////////////////////////////////////////////////////////
// ID3DXRenderToEnvMap:
// --------------------
///////////////////////////////////////////////////////////////////////////

typedef struct _D3DXRTE_DESC
{
    UINT        Size;
    D3DFORMAT   Format;
    BOOL        DepthStencil;
    D3DFORMAT   DepthStencilFormat;
} D3DXRTE_DESC;


struct ID3DXRenderToEnvMap; struct ID3DXRenderToEnvMap_vtbl;
struct ID3DXRenderToEnvMap; struct ID3DXRenderToEnvMap_vtbl;

struct ID3DXRenderToEnvMap { ID3DXRenderToEnvMap_vtbl* vtptr; };
struct ID3DXRenderToEnvMap_vtbl 
{
    // IUnknown
    DWORD (__stdcall *QueryInterface)(void* this, void* iid, LPVOID *ppv);
    ULONG (__stdcall *AddRef)(void* this);
    ULONG (__stdcall *Release)(void* this);

    // ID3DXRenderToEnvMap
    DWORD (__stdcall *GetDevice)(void* this, IDirect3DDevice8** ppDevice);
    DWORD (__stdcall *GetDesc)(void* this, D3DXRTE_DESC* pDesc);

    DWORD (__stdcall *BeginCube)(void* this, 
        IDirect3DCubeTexture8* pCubeTex);

    DWORD (__stdcall *BeginSphere)(void* this,
        IDirect3DTexture8* pTex);

    DWORD (__stdcall *BeginHemisphere)(void* this, 
        IDirect3DTexture8* pTexZPos,
        IDirect3DTexture8* pTexZNeg);

    DWORD (__stdcall *BeginParabolic)(void* this, 
        IDirect3DTexture8* pTexZPos,
        IDirect3DTexture8* pTexZNeg);

    DWORD (__stdcall *Face)(void* this, D3DCUBEMAP_FACES Face);
    DWORD (__stdcall *End)(void* this);
};


#endif //__D3DX8CORE_H__
