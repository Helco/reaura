//////////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 1998 Microsoft Corporation.  All Rights Reserved.
//
//  File:       d3dx8mesh.h
//  Content:    D3DX mesh types and functions
//
//////////////////////////////////////////////////////////////////////////////

#ifndef __D3DX8MESH_H__
#define __D3DX8MESH_H__


enum _D3DXMESH {
	D3DXMESH_32BIT			        = 0x001, // If set, then use 32 bit indices, if not set use 16 bit indices. 32BIT meshes currently not supported on ID3DXSkinMesh object
    D3DXMESH_DONOTCLIP              = 0x002, // Use D3DUSAGE_DONOTCLIP for VB & IB.
    D3DXMESH_POINTS                 = 0x004, // Use D3DUSAGE_POINTS for VB & IB. 
    D3DXMESH_RTPATCHES              = 0x008, // Use D3DUSAGE_RTPATCHES for VB & IB. 
	D3DXMESH_NPATCHES				= 0x4000,// Use D3DUSAGE_NPATCHES for VB & IB. 
	D3DXMESH_VB_SYSTEMMEM		    = 0x010, // Use D3DPOOL_SYSTEMMEM for VB. Overrides D3DXMESH_MANAGEDVERTEXBUFFER
    D3DXMESH_VB_MANAGED             = 0x020, // Use D3DPOOL_MANAGED for VB. 
    D3DXMESH_VB_WRITEONLY           = 0x040, // Use D3DUSAGE_WRITEONLY for VB.
    D3DXMESH_VB_DYNAMIC             = 0x080, // Use D3DUSAGE_DYNAMIC for VB.
	D3DXMESH_IB_SYSTEMMEM			= 0x100, // Use D3DPOOL_SYSTEMMEM for IB. Overrides D3DXMESH_MANAGEDINDEXBUFFER
    D3DXMESH_IB_MANAGED             = 0x200, // Use D3DPOOL_MANAGED for IB.
    D3DXMESH_IB_WRITEONLY           = 0x400, // Use D3DUSAGE_WRITEONLY for IB.
    D3DXMESH_IB_DYNAMIC             = 0x800, // Use D3DUSAGE_DYNAMIC for IB.

    D3DXMESH_VB_SHARE               = 0x1000, // Valid for Clone* calls only, forces cloned mesh/pmesh to share vertex buffer

    D3DXMESH_USEHWONLY              = 0x2000, // Valid for ID3DXSkinMesh::ConvertToBlendedMesh

    // Helper options
    D3DXMESH_SYSTEMMEM				= 0x110, // D3DXMESH_VB_SYSTEMMEM | D3DXMESH_IB_SYSTEMMEM
    D3DXMESH_MANAGED                = 0x220, // D3DXMESH_VB_MANAGED | D3DXMESH_IB_MANAGED
    D3DXMESH_WRITEONLY              = 0x440, // D3DXMESH_VB_WRITEONLY | D3DXMESH_IB_WRITEONLY
    D3DXMESH_DYNAMIC                = 0x880, // D3DXMESH_VB_DYNAMIC | D3DXMESH_IB_DYNAMIC

};

// option field values for specifying min value in D3DXGeneratePMesh and D3DXSimplifyMesh
enum _D3DXMESHSIMP
{
    D3DXMESHSIMP_VERTEX   = 0x1,
    D3DXMESHSIMP_FACE     = 0x2,

};

enum _MAX_FVF_DECL_SIZE
{
	MAX_FVF_DECL_SIZE = 20
};

typedef struct ID3DXBaseMesh *LPD3DXBASEMESH;
typedef struct ID3DXMesh *LPD3DXMESH;
typedef struct ID3DXPMesh *LPD3DXPMESH;
typedef struct ID3DXSPMesh *LPD3DXSPMESH;
typedef struct ID3DXSkinMesh *LPD3DXSKINMESH;

typedef struct _D3DXATTRIBUTERANGE
{
    DWORD AttribId;
    DWORD FaceStart;
    DWORD FaceCount;
    DWORD VertexStart;
    DWORD VertexCount;
} D3DXATTRIBUTERANGE;

typedef D3DXATTRIBUTERANGE* LPD3DXATTRIBUTERANGE;

#ifdef __cplusplus
extern "C" {
#endif //__cplusplus
struct D3DXMATERIAL
{
    D3DMATERIAL8  MatD3D;
    LPSTR         pTextureFilename;
};
typedef struct D3DXMATERIAL *LPD3DXMATERIAL;
#ifdef __cplusplus
}
#endif //__cplusplus

typedef struct _D3DXATTRIBUTEWEIGHTS
{
    FLOAT Position;
    FLOAT Boundary;
    FLOAT Normal;
    FLOAT Diffuse;
    FLOAT Specular;
    FLOAT Tex[8];
} D3DXATTRIBUTEWEIGHTS;

typedef D3DXATTRIBUTEWEIGHTS* LPD3DXATTRIBUTEWEIGHTS;

struct ID3DXBaseMesh_vtbl;
struct ID3DXBaseMesh { ID3DXBaseMesh_vtbl* vtptr; };
struct ID3DXBaseMesh_vtbl
{
    // IUnknown
    DWORD (__stdcall *QueryInterface)(void* this, void* iid, LPVOID *ppv);
    ULONG (__stdcall *AddRef)(void* this);
    ULONG (__stdcall *Release)(void* this);

    // ID3DXBaseMesh
    DWORD (__stdcall *DrawSubset)(void* this, DWORD AttribId);
    DWORD (__stdcall *GetNumFaces)(void* this);
    DWORD (__stdcall *GetNumVertices)(void* this);
    DWORD (__stdcall *GetFVF)(void* this);
    DWORD (__stdcall *GetDeclaration)(void* this, DWORD Declaration[MAX_FVF_DECL_SIZE]);
    DWORD (__stdcall *GetOptions)(void* this);
    DWORD (__stdcall *GetDevice)(void* this, IDirect3DDevice8** ppDevice);
    DWORD (__stdcall *CloneMeshFVF)(void* this, DWORD Options, DWORD FVF, IDirect3DDevice8* pD3DDevice, LPD3DXMESH* ppCloneMesh);
    DWORD (__stdcall *CloneMesh)(void* this, DWORD Options,  DWORD *pDeclaration, IDirect3DDevice8* pD3DDevice, LPD3DXMESH* ppCloneMesh);
    DWORD (__stdcall *GetVertexBuffer)(void* this, IDirect3DVertexBuffer8** ppVB);
    DWORD (__stdcall *GetIndexBuffer)(void* this, IDirect3DIndexBuffer8** ppIB);
	DWORD (__stdcall *LockVertexBuffer)(void* this, DWORD Flags, BYTE** ppData);
	DWORD (__stdcall *UnlockVertexBuffer)(void* this);
	DWORD (__stdcall *LockIndexBuffer)(void* this, DWORD Flags, BYTE** ppData);
	DWORD (__stdcall *UnlockIndexBuffer)(void* this);
    DWORD (__stdcall *GetAttributeTable)(void* this, D3DXATTRIBUTERANGE *pAttribTable, DWORD* pAttribTableSize);
};

struct ID3DXMesh_vtbl;
struct ID3DXMesh { ID3DXMesh_vtbl* vtptr; };
struct ID3DXMesh_vtbl
{
    // IUnknown
    DWORD (__stdcall *QueryInterface)(void* this, void* iid, LPVOID *ppv);
    ULONG (__stdcall *AddRef)(void* this);
    ULONG (__stdcall *Release)(void* this);

    // ID3DXBaseMesh
    DWORD (__stdcall *DrawSubset)(void* this, DWORD AttribId);
    DWORD (__stdcall *GetNumFaces)(void* this);
    DWORD (__stdcall *GetNumVertices)(void* this);
    DWORD (__stdcall *GetFVF)(void* this);
    DWORD (__stdcall *GetDeclaration)(void* this, DWORD Declaration[MAX_FVF_DECL_SIZE]);
    DWORD (__stdcall *GetOptions)(void* this);
    DWORD (__stdcall *GetDevice)(void* this, IDirect3DDevice8** ppDevice);
    DWORD (__stdcall *CloneMeshFVF)(void* this, DWORD Options, 
                DWORD FVF, IDirect3DDevice8* pD3DDevice, LPD3DXMESH* ppCloneMesh);
    DWORD (__stdcall *CloneMesh)(void* this, DWORD Options, 
                 DWORD *pDeclaration, IDirect3DDevice8* pD3DDevice, LPD3DXMESH* ppCloneMesh);
    DWORD (__stdcall *GetVertexBuffer)(void* this, IDirect3DVertexBuffer8** ppVB);
    DWORD (__stdcall *GetIndexBuffer)(void* this, IDirect3DIndexBuffer8** ppIB);
	DWORD (__stdcall *LockVertexBuffer)(void* this, DWORD Flags, BYTE** ppData);
	DWORD (__stdcall *UnlockVertexBuffer)(void* this);
	DWORD (__stdcall *LockIndexBuffer)(void* this, DWORD Flags, BYTE** ppData);
	DWORD (__stdcall *UnlockIndexBuffer)(void* this);
    DWORD (__stdcall *GetAttributeTable)(
                void* this, D3DXATTRIBUTERANGE *pAttribTable, DWORD* pAttribTableSize);

    // ID3DXMesh
	DWORD (__stdcall *LockAttributeBuffer)(void* this, DWORD Flags, DWORD** ppData);
	DWORD (__stdcall *UnlockAttributeBuffer)(void* this);
    DWORD (__stdcall *ConvertPointRepsToAdjacency)(void* this,  DWORD* pPRep, DWORD* pAdjacency);
    DWORD (__stdcall *ConvertAdjacencyToPointReps)(void* this,  DWORD* pAdjacency, DWORD* pPRep);
    DWORD (__stdcall *GenerateAdjacency)(void* this, FLOAT fEpsilon, DWORD* pAdjacency);
    DWORD (__stdcall *Optimize)(void* this, DWORD Flags,  DWORD* pAdjacencyIn, DWORD* pAdjacencyOut, 
                     DWORD* pFaceRemap, LPD3DXBUFFER *ppVertexRemap,  
                     LPD3DXMESH* ppOptMesh);
    DWORD (__stdcall *OptimizeInplace)(void* this, DWORD Flags,  DWORD* pAdjacencyIn, DWORD* pAdjacencyOut, 
                     DWORD* pFaceRemap, LPD3DXBUFFER *ppVertexRemap);
};

struct ID3DXPMesh_vtbl;
struct ID3DXPMesh { ID3DXPMesh_vtbl* vtptr; };
struct ID3DXPMesh_vtbl
{
    // IUnknown
    DWORD (__stdcall *QueryInterface)(void* this, void* iid, LPVOID *ppv);
    ULONG (__stdcall *AddRef)(void* this);
    ULONG (__stdcall *Release)(void* this);

    // ID3DXBaseMesh
    DWORD (__stdcall *DrawSubset)(void* this, DWORD AttribId);
    DWORD (__stdcall *GetNumFaces)(void* this);
    DWORD (__stdcall *GetNumVertices)(void* this);
    DWORD (__stdcall *GetFVF)(void* this);
    DWORD (__stdcall *GetDeclaration)(void* this, DWORD Declaration[MAX_FVF_DECL_SIZE]);
    DWORD (__stdcall *GetOptions)(void* this);
    DWORD (__stdcall *GetDevice)(void* this, IDirect3DDevice8** ppDevice);
    DWORD (__stdcall *CloneMeshFVF)(void* this, DWORD Options, 
                DWORD FVF, IDirect3DDevice8* pD3DDevice, LPD3DXMESH* ppCloneMesh);
    DWORD (__stdcall *CloneMesh)(void* this, DWORD Options, 
                 DWORD *pDeclaration, IDirect3DDevice8* pD3DDevice, LPD3DXMESH* ppCloneMesh);
    DWORD (__stdcall *GetVertexBuffer)(void* this, IDirect3DVertexBuffer8** ppVB);
    DWORD (__stdcall *GetIndexBuffer)(void* this, IDirect3DIndexBuffer8** ppIB);
	DWORD (__stdcall *LockVertexBuffer)(void* this, DWORD Flags, BYTE** ppData);
	DWORD (__stdcall *UnlockVertexBuffer)(void* this);
	DWORD (__stdcall *LockIndexBuffer)(void* this, DWORD Flags, BYTE** ppData);
	DWORD (__stdcall *UnlockIndexBuffer)(void* this);
    DWORD (__stdcall *GetAttributeTable)(
                void* this, D3DXATTRIBUTERANGE *pAttribTable, DWORD* pAttribTableSize);

    // ID3DXPMesh
    DWORD (__stdcall *ClonePMeshFVF)(void* this, DWORD Options, 
                DWORD FVF, IDirect3DDevice8* pD3D, LPD3DXPMESH* ppCloneMesh);
    DWORD (__stdcall *ClonePMesh)(void* this, DWORD Options, 
                 DWORD *pDeclaration, IDirect3DDevice8* pD3D, LPD3DXPMESH* ppCloneMesh);
    DWORD (__stdcall *SetNumFaces)(void* this, DWORD Faces);
    DWORD (__stdcall *SetNumVertices)(void* this, DWORD Vertices);
    DWORD (__stdcall *GetMaxFaces)(void* this);
    DWORD (__stdcall *GetMinFaces)(void* this);
    DWORD (__stdcall *GetMaxVertices)(void* this);
    DWORD (__stdcall *GetMinVertices)(void* this);
    DWORD (__stdcall *Save)(void* this, IStream *pStream, LPD3DXMATERIAL pMaterials, DWORD NumMaterials);

    DWORD(__stdcall *Optimize)(void* this, DWORD Flags, DWORD* pAdjacencyOut, 
                     DWORD* pFaceRemap, LPD3DXBUFFER *ppVertexRemap,  
                     LPD3DXMESH* ppOptMesh);
    DWORD (__stdcall *GetAdjacency)(void* this, DWORD* pAdjacency);
};

struct ID3DXSPMesh_vtbl;
struct ID3DXSPMesh { ID3DXSPMesh_vtbl* vtptr; };
struct ID3DXSPMesh_vtbl
{
    // IUnknown
    DWORD (__stdcall *QueryInterface)(void* this, void* iid, LPVOID *ppv);
    ULONG (__stdcall *AddRef)(void* this);
    ULONG (__stdcall *Release)(void* this);

    // ID3DXSPMesh
    DWORD (__stdcall *GetNumFaces)(void* this);
    DWORD (__stdcall *GetNumVertices)(void* this);
    DWORD (__stdcall *GetFVF)(void* this);
    DWORD (__stdcall *GetDeclaration)(void* this, DWORD Declaration[MAX_FVF_DECL_SIZE]);
    DWORD (__stdcall *GetOptions)(void* this);
    DWORD (__stdcall *GetDevice)(void* this, IDirect3DDevice8** ppDevice);
    DWORD (__stdcall *CloneMeshFVF)(void* this, DWORD Options, 
                DWORD FVF, IDirect3DDevice8* pD3D, DWORD *pAdjacencyOut, DWORD *pVertexRemapOut, LPD3DXMESH* ppCloneMesh);
    DWORD (__stdcall *CloneMesh)(void* this, DWORD Options, 
                 DWORD *pDeclaration, IDirect3DDevice8* pD3DDevice, DWORD *pAdjacencyOut, DWORD *pVertexRemapOut, LPD3DXMESH* ppCloneMesh);
    DWORD (__stdcall *ClonePMeshFVF)(void* this, DWORD Options, 
                DWORD FVF, IDirect3DDevice8* pD3D, DWORD *pVertexRemapOut, LPD3DXPMESH* ppCloneMesh);
    DWORD (__stdcall *ClonePMesh)(void* this, DWORD Options, 
                 DWORD *pDeclaration, IDirect3DDevice8* pD3D, DWORD *pVertexRemapOut, LPD3DXPMESH* ppCloneMesh);
    DWORD (__stdcall *ReduceFaces)(void* this, DWORD Faces);
    DWORD (__stdcall *ReduceVertices)(void* this, DWORD Vertices);
    DWORD (__stdcall *GetMaxFaces)(void* this);
    DWORD (__stdcall *GetMaxVertices)(void* this);
};

#define UNUSED16 (0xffff)
#define UNUSED32 (0xffffffff)

// ID3DXMesh::Optimize options
enum _D3DXMESHOPT {
	D3DXMESHOPT_COMPACT       = 0x001,
	D3DXMESHOPT_ATTRSORT      = 0x002,
	D3DXMESHOPT_VERTEXCACHE   = 0x004,
	D3DXMESHOPT_STRIPREORDER  = 0x008,
    D3DXMESHOPT_IGNOREVERTS   = 0x010,  // optimize faces only, don't touch vertices
    D3DXMESHOPT_SHAREVB       = 0x020,
};

// Subset of the mesh that has the same attribute and bone combination.
// This subset can be rendered in a single draw call
typedef struct _D3DXBONECOMBINATION
{
    DWORD AttribId;
    DWORD FaceStart;
    DWORD FaceCount;
    DWORD VertexStart;
    DWORD VertexCount;
	DWORD* BoneId;
} D3DXBONECOMBINATION, *LPD3DXBONECOMBINATION;

struct ID3DXSkinMesh_vtbl;
struct ID3DXSkinMesh { ID3DXSkinMesh_vtbl* vtptr; };
struct ID3DXSkinMesh_vtbl
{
    // IUnknown
    DWORD (__stdcall *QueryInterface)(void* this, void* iid, LPVOID *ppv);
    ULONG (__stdcall *AddRef)(void* this);
    ULONG (__stdcall *Release)(void* this);

	// ID3DXMesh
    DWORD (__stdcall *GetNumFaces)(void* this);
    DWORD (__stdcall *GetNumVertices)(void* this);
    DWORD (__stdcall *GetFVF)(void* this);
    DWORD (__stdcall *GetDeclaration)(void* this, DWORD Declaration[MAX_FVF_DECL_SIZE]);
    DWORD (__stdcall *GetOptions)(void* this);
    DWORD (__stdcall *GetDevice)(void* this, IDirect3DDevice8** ppDevice);
    DWORD (__stdcall *GetVertexBuffer)(void* this, IDirect3DVertexBuffer8** ppVB);
    DWORD (__stdcall *GetIndexBuffer)(void* this, IDirect3DIndexBuffer8** ppIB);
	DWORD (__stdcall *LockVertexBuffer)(void* this, DWORD flags, BYTE** ppData);
	DWORD (__stdcall *UnlockVertexBuffer)(void* this);
	DWORD (__stdcall *LockIndexBuffer)(void* this, DWORD flags, BYTE** ppData);
	DWORD (__stdcall *UnlockIndexBuffer)(void* this);
	DWORD (__stdcall *LockAttributeBuffer)(void* this, DWORD flags, DWORD** ppData);
	DWORD (__stdcall *UnlockAttributeBuffer)(void* this);
    // ID3DXSkinMesh
    DWORD (__stdcall *GetNumBones)(void* this);
    DWORD (__stdcall *GetOriginalMesh)(void* this, LPD3DXMESH* ppMesh);
	DWORD (__stdcall *SetBoneInfluence)(void* this, DWORD bone, DWORD numInfluences,  DWORD* vertices,  FLOAT* weights);
	DWORD (__stdcall *GetNumBoneInfluences)(void* this, DWORD bone);
	DWORD (__stdcall *GetBoneInfluence)(void* this, DWORD bone, DWORD* vertices, FLOAT* weights);
	DWORD (__stdcall *GetMaxVertexInfluences)(void* this, DWORD* maxVertexInfluences);
	DWORD (__stdcall *GetMaxFaceInfluences)(void* this, DWORD* maxFaceInfluences);
	DWORD (__stdcall *ConvertToBlendedMesh)(void* this, DWORD options,  LPDWORD pAdjacencyIn, LPDWORD pAdjacencyOut,
							DWORD* pNumBoneCombinations, LPD3DXBUFFER* ppBoneCombinationTable, LPD3DXMESH* ppMesh);
	DWORD (__stdcall *ConvertToIndexedBlendedMesh)(void* this, DWORD options, 
                                            LPDWORD pAdjacencyIn, 
                                           DWORD paletteSize, 
                                           LPDWORD pAdjacencyOut, 
 							               DWORD* pNumBoneCombinations, 
                                           LPD3DXBUFFER* ppBoneCombinationTable, 
                                           LPD3DXMESH* ppMesh);
    DWORD (__stdcall *GenerateSkinnedMesh)(void* this, DWORD options, FLOAT minWeight,  LPDWORD pAdjacencyIn, LPDWORD pAdjacencyOut, LPD3DXMESH* ppMesh);
	DWORD (__stdcall *UpdateSkinnedMesh)(void* this,  D3DXMATRIX* pBoneTransforms, LPD3DXMESH pMesh);
};

#ifdef __cplusplus
extern "C" {
#endif //__cplusplus

HRESULT WINAPI 
    D3DXCreateMesh(
        DWORD NumFaces, 
        DWORD NumVertices, 
        DWORD Options, 
         DWORD *pDeclaration, 
        IDirect3DDevice8* pD3D, 
        LPD3DXMESH* ppMesh);

HRESULT WINAPI 
    D3DXCreateMeshFVF(
        DWORD NumFaces, 
        DWORD NumVertices, 
        DWORD Options, 
        DWORD FVF, 
        IDirect3DDevice8* pD3D, 
        LPD3DXMESH* ppMesh);

HRESULT WINAPI 
    D3DXCreateSPMesh(
        LPD3DXMESH pMesh, 
         DWORD* pAdjacency, 
         LPD3DXATTRIBUTEWEIGHTS pVertexAttributeWeights,
         FLOAT *pVertexWeights,
        LPD3DXSPMESH* ppSMesh);

// clean a mesh up for simplification, try to make manifold
HRESULT WINAPI
    D3DXCleanMesh(
    LPD3DXMESH pMeshIn,
     DWORD* pAdjacency,
    LPD3DXMESH* ppMeshOut);

HRESULT WINAPI
    D3DXValidMesh(
    LPD3DXMESH pMeshIn,
     DWORD* pAdjacency);

HRESULT WINAPI 
    D3DXGeneratePMesh(
        LPD3DXMESH pMesh, 
         DWORD* pAdjacency, 
         LPD3DXATTRIBUTEWEIGHTS pVertexAttributeWeights,
         FLOAT *pVertexWeights,
        DWORD MinValue, 
        DWORD Options, 
        LPD3DXPMESH* ppPMesh);

HRESULT WINAPI 
    D3DXSimplifyMesh(
        LPD3DXMESH pMesh, 
         DWORD* pAdjacency, 
         LPD3DXATTRIBUTEWEIGHTS pVertexAttributeWeights,
         FLOAT *pVertexWeights,
        DWORD MinValue, 
        DWORD Options, 
        LPD3DXMESH* ppMesh);

HRESULT WINAPI 
    D3DXComputeBoundingSphere(
        PVOID pPointsFVF, 
        DWORD NumVertices, 
        DWORD FVF,
        D3DXVECTOR3 *pCenter, 
        FLOAT *pRadius);

HRESULT WINAPI 
    D3DXComputeBoundingBox(
        PVOID pPointsFVF, 
        DWORD NumVertices, 
        DWORD FVF,
        D3DXVECTOR3 *pMin, 
        D3DXVECTOR3 *pMax);

HRESULT WINAPI 
    D3DXComputeNormals(
        LPD3DXBASEMESH pMesh);

HRESULT WINAPI 
    D3DXCreateBuffer(
        DWORD NumBytes, 
        LPD3DXBUFFER *ppBuffer);


HRESULT WINAPI
    D3DXLoadMeshFromX(
        LPSTR pFilename, 
        DWORD Options, 
        IDirect3DDevice8* pD3D, 
        LPD3DXBUFFER *ppAdjacency,
        LPD3DXBUFFER *ppMaterials, 
        PDWORD pNumMaterials,
        LPD3DXMESH *ppMesh);

HRESULT WINAPI 
    D3DXSaveMeshToX(
        LPSTR pFilename,
        LPD3DXMESH pMesh,
         DWORD* pAdjacency,
         LPD3DXMATERIAL pMaterials,
        DWORD NumMaterials,
        DWORD Format
        );

HRESULT WINAPI 
    D3DXCreatePMeshFromStream(
        IStream *pStream, 
	DWORD Options,
        IDirect3DDevice8* pD3DDevice, 
        LPD3DXBUFFER *ppMaterials,
        DWORD* pNumMaterials,
        LPD3DXPMESH *ppPMesh);

HRESULT WINAPI
    D3DXCreateSkinMesh(
        DWORD numFaces, 
        DWORD numVertices, 
        DWORD numBones,
        DWORD options, 
         DWORD *pDeclaration, 
        IDirect3DDevice8* pD3D, 
        LPD3DXSKINMESH* ppSkinMesh);

HRESULT WINAPI
    D3DXCreateSkinMeshFVF(
        DWORD numFaces, 
        DWORD numVertices, 
        DWORD numBones,
        DWORD options, 
        DWORD fvf, 
        IDirect3DDevice8* pD3D, 
        LPD3DXSKINMESH* ppSkinMesh);

HRESULT WINAPI
    D3DXCreateSkinMeshFromMesh(
        LPD3DXMESH pMesh,
        DWORD numBones,
        LPD3DXSKINMESH* ppSkinMesh);

HRESULT WINAPI 
    D3DXLoadMeshFromXof(
        LPDIRECTXFILEDATA pXofObjMesh, 
        DWORD Options, 
        IDirect3DDevice8* pD3DDevice, 
        LPD3DXBUFFER *ppAdjacency,
        LPD3DXBUFFER *ppMaterials, 
        PDWORD pNumMaterials,
        LPD3DXMESH *ppMesh);

HRESULT WINAPI
    D3DXLoadSkinMeshFromXof(
        LPDIRECTXFILEDATA pxofobjMesh, 
        DWORD options,
        IDirect3DDevice8* pD3D,
        LPD3DXBUFFER* ppAdjacency,
        LPD3DXBUFFER* ppMaterials,
        PDWORD pMatOut,
        LPD3DXBUFFER* ppBoneNames,
        LPD3DXBUFFER* ppBoneTransforms,
        LPD3DXSKINMESH* ppMesh);

HRESULT WINAPI
    D3DXTesselateMesh(
        LPD3DXMESH pMeshIn,             
         DWORD* pAdjacency,             
        FLOAT NumSegs,                    
        BOOL  QuadraticInterpNormals,     // if false use linear intrep for normals, if true use quadratic
        LPD3DXMESH *ppMeshOut);         

HRESULT WINAPI
    D3DXDeclaratorFromFVF(
        DWORD FVF,
		DWORD Declaration[MAX_FVF_DECL_SIZE]);

HRESULT WINAPI
    D3DXFVFFromDeclarator(
         DWORD *pDeclarator,
        DWORD *pFVF);

HRESULT WINAPI 
    D3DXWeldVertices(
         LPD3DXMESH pMesh,         
        float fEpsilon,                 
         DWORD *rgdwAdjacencyIn, 
        DWORD *rgdwAdjacencyOut,
        DWORD* pFaceRemap, 
        LPD3DXBUFFER *ppbufVertexRemap);

HRESULT WINAPI
    D3DXIntersect(
        LPD3DXBASEMESH pMesh,
         D3DXVECTOR3 *pRayPos,
         D3DXVECTOR3 *pRayDir,
        BOOL    *pHit,
        DWORD   *pFaceIndex,
        FLOAT   *pU,
        FLOAT   *pV,
        FLOAT   *pDist);

BOOL WINAPI
    D3DXSphereBoundProbe(
         D3DXVECTOR3 *pvCenter,
        FLOAT fRadius,
        D3DXVECTOR3 *pvRayPosition,
        D3DXVECTOR3 *pvRayDirection);

BOOL WINAPI 
    D3DXBoxBoundProbe(
         D3DXVECTOR3 *pvMin, 
         D3DXVECTOR3 *pvMax,
        D3DXVECTOR3 *pvRayPosition,
        D3DXVECTOR3 *pvRayDirection);

enum _D3DXERR {
    D3DXERR_CANNOTMODIFYINDEXBUFFER		= MAKE_DDHRESULT(2900),
	D3DXERR_INVALIDMESH					= MAKE_DDHRESULT(2901),
	D3DXERR_CANNOTATTRSORT              = MAKE_DDHRESULT(2902),
	D3DXERR_SKINNINGNOTSUPPORTED		= MAKE_DDHRESULT(2903),
	D3DXERR_TOOMANYINFLUENCES			= MAKE_DDHRESULT(2904),
    D3DXERR_INVALIDDATA                 = MAKE_DDHRESULT(2905),
};

#ifdef __cplusplus
}
#endif //__cplusplus

#endif //__D3DX8MESH_H__
