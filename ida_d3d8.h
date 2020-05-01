/*==========================================================================;
 *
 *  Copyright (C) Microsoft Corporation.  All Rights Reserved.
 *
 *  File:   d3d8.h
 *  Content:    Direct3D include file
 *
 ****************************************************************************/

#ifndef _D3D8_H_
#define _D3D8_H_

#ifndef D3DVECTOR_DEFINED
typedef struct _D3DVECTOR {
    float x;
    float y;
    float z;
} D3DVECTOR;
#define D3DVECTOR_DEFINED
#endif

#ifndef D3DCOLORVALUE_DEFINED
typedef struct _D3DCOLORVALUE {
    float r;
    float g;
    float b;
    float a;
} D3DCOLORVALUE;
#define D3DCOLORVALUE_DEFINED
#endif

#ifndef D3DRECT_DEFINED
typedef struct _D3DRECT {
    LONG x1;
    LONG y1;
    LONG x2;
    LONG y2;
} D3DRECT;
#define D3DRECT_DEFINED
#endif

#ifndef D3DMATRIX_DEFINED
typedef struct _D3DMATRIX {
    union {
        struct {
            float        _11, _12, _13, _14;
            float        _21, _22, _23, _24;
            float        _31, _32, _33, _34;
            float        _41, _42, _43, _44;

        };
        float m[4][4];
    };
} D3DMATRIX;
#define D3DMATRIX_DEFINED
#endif

typedef struct _D3DVIEWPORT8 {
    DWORD       X;
    DWORD       Y;            /* Viewport Top left */
    DWORD       Width;
    DWORD       Height;       /* Viewport Dimensions */
    float       MinZ;         /* Min/max of clip Volume */
    float       MaxZ;
} D3DVIEWPORT8;


typedef struct _D3DCLIPSTATUS8 {
    DWORD ClipUnion;
    DWORD ClipIntersection;
} D3DCLIPSTATUS8;

typedef struct _D3DMATERIAL8 {
    D3DCOLORVALUE   Diffuse;        /* Diffuse color RGBA */
    D3DCOLORVALUE   Ambient;        /* Ambient color RGB */
    D3DCOLORVALUE   Specular;       /* Specular 'shininess' */
    D3DCOLORVALUE   Emissive;       /* Emissive color RGB */
    float           Power;          /* Sharpness if specular highlight */
} D3DMATERIAL8;

typedef enum _D3DLIGHTTYPE {
    D3DLIGHT_POINT          = 1,
    D3DLIGHT_SPOT           = 2,
    D3DLIGHT_DIRECTIONAL    = 3,
    D3DLIGHT_FORCE_DWORD    = 0x7fffffff, /* force 32-bit size enum */
} D3DLIGHTTYPE;

typedef struct _D3DLIGHT8 {
    D3DLIGHTTYPE    Type;            /* Type of light source */
    D3DCOLORVALUE   Diffuse;         /* Diffuse color of light */
    D3DCOLORVALUE   Specular;        /* Specular color of light */
    D3DCOLORVALUE   Ambient;         /* Ambient color of light */
    D3DVECTOR       Position;         /* Position in world space */
    D3DVECTOR       Direction;        /* Direction in world space */
    float           Range;            /* Cutoff range */
    float           Falloff;          /* Falloff */
    float           Attenuation0;     /* Constant attenuation */
    float           Attenuation1;     /* Linear attenuation */
    float           Attenuation2;     /* Quadratic attenuation */
    float           Theta;            /* Inner angle of spotlight cone */
    float           Phi;              /* Outer angle of spotlight cone */
} D3DLIGHT8;

/*
 * Options for clearing
 */
#define D3DCLEAR_TARGET            0x00000001l  /* Clear target surface */
#define D3DCLEAR_ZBUFFER           0x00000002l  /* Clear target z buffer */
#define D3DCLEAR_STENCIL           0x00000004l  /* Clear stencil planes */

/*
 * The following defines the rendering states
 */

typedef enum _D3DSHADEMODE {
    D3DSHADE_FLAT               = 1,
    D3DSHADE_GOURAUD            = 2,
    D3DSHADE_PHONG              = 3,
    D3DSHADE_FORCE_DWORD        = 0x7fffffff, /* force 32-bit size enum */
} D3DSHADEMODE;

typedef enum _D3DFILLMODE {
    D3DFILL_POINT               = 1,
    D3DFILL_WIREFRAME           = 2,
    D3DFILL_SOLID               = 3,
    D3DFILL_FORCE_DWORD         = 0x7fffffff, /* force 32-bit size enum */
} D3DFILLMODE;

typedef struct _D3DLINEPATTERN {
    WORD    wRepeatFactor;
    WORD    wLinePattern;
} D3DLINEPATTERN;

typedef enum _D3DBLEND {
    D3DBLEND_ZERO               = 1,
    D3DBLEND_ONE                = 2,
    D3DBLEND_SRCCOLOR           = 3,
    D3DBLEND_INVSRCCOLOR        = 4,
    D3DBLEND_SRCALPHA           = 5,
    D3DBLEND_INVSRCALPHA        = 6,
    D3DBLEND_DESTALPHA          = 7,
    D3DBLEND_INVDESTALPHA       = 8,
    D3DBLEND_DESTCOLOR          = 9,
    D3DBLEND_INVDESTCOLOR       = 10,
    D3DBLEND_SRCALPHASAT        = 11,
    D3DBLEND_BOTHSRCALPHA       = 12,
    D3DBLEND_BOTHINVSRCALPHA    = 13,
    D3DBLEND_FORCE_DWORD        = 0x7fffffff, /* force 32-bit size enum */
} D3DBLEND;

typedef enum _D3DBLENDOP {
    D3DBLENDOP_ADD              = 1,
    D3DBLENDOP_SUBTRACT         = 2,
    D3DBLENDOP_REVSUBTRACT      = 3,
    D3DBLENDOP_MIN              = 4,
    D3DBLENDOP_MAX              = 5,
    D3DBLENDOP_FORCE_DWORD      = 0x7fffffff, /* force 32-bit size enum */
} D3DBLENDOP;

typedef enum _D3DTEXTUREADDRESS {
    D3DTADDRESS_WRAP            = 1,
    D3DTADDRESS_MIRROR          = 2,
    D3DTADDRESS_CLAMP           = 3,
    D3DTADDRESS_BORDER          = 4,
    D3DTADDRESS_MIRRORONCE      = 5,
    D3DTADDRESS_FORCE_DWORD     = 0x7fffffff, /* force 32-bit size enum */
} D3DTEXTUREADDRESS;

typedef enum _D3DCULL {
    D3DCULL_NONE                = 1,
    D3DCULL_CW                  = 2,
    D3DCULL_CCW                 = 3,
    D3DCULL_FORCE_DWORD         = 0x7fffffff, /* force 32-bit size enum */
} D3DCULL;

typedef enum _D3DCMPFUNC {
    D3DCMP_NEVER                = 1,
    D3DCMP_LESS                 = 2,
    D3DCMP_EQUAL                = 3,
    D3DCMP_LESSEQUAL            = 4,
    D3DCMP_GREATER              = 5,
    D3DCMP_NOTEQUAL             = 6,
    D3DCMP_GREATEREQUAL         = 7,
    D3DCMP_ALWAYS               = 8,
    D3DCMP_FORCE_DWORD          = 0x7fffffff, /* force 32-bit size enum */
} D3DCMPFUNC;

typedef enum _D3DSTENCILOP {
    D3DSTENCILOP_KEEP           = 1,
    D3DSTENCILOP_ZERO           = 2,
    D3DSTENCILOP_REPLACE        = 3,
    D3DSTENCILOP_INCRSAT        = 4,
    D3DSTENCILOP_DECRSAT        = 5,
    D3DSTENCILOP_INVERT         = 6,
    D3DSTENCILOP_INCR           = 7,
    D3DSTENCILOP_DECR           = 8,
    D3DSTENCILOP_FORCE_DWORD    = 0x7fffffff, /* force 32-bit size enum */
} D3DSTENCILOP;

typedef enum _D3DFOGMODE {
    D3DFOG_NONE                 = 0,
    D3DFOG_EXP                  = 1,
    D3DFOG_EXP2                 = 2,
    D3DFOG_LINEAR               = 3,
    D3DFOG_FORCE_DWORD          = 0x7fffffff, /* force 32-bit size enum */
} D3DFOGMODE;

typedef enum _D3DZBUFFERTYPE {
    D3DZB_FALSE                 = 0,
    D3DZB_TRUE                  = 1, // Z buffering
    D3DZB_USEW                  = 2, // W buffering
    D3DZB_FORCE_DWORD           = 0x7fffffff, /* force 32-bit size enum */
} D3DZBUFFERTYPE;

// Primitives supported by draw-primitive API
typedef enum _D3DPRIMITIVETYPE {
    D3DPT_POINTLIST             = 1,
    D3DPT_LINELIST              = 2,
    D3DPT_LINESTRIP             = 3,
    D3DPT_TRIANGLELIST          = 4,
    D3DPT_TRIANGLESTRIP         = 5,
    D3DPT_TRIANGLEFAN           = 6,
    D3DPT_FORCE_DWORD           = 0x7fffffff, /* force 32-bit size enum */
} D3DPRIMITIVETYPE;

typedef enum _D3DTRANSFORMSTATETYPE {
    D3DTS_VIEW          = 2,
    D3DTS_PROJECTION    = 3,
    D3DTS_TEXTURE0      = 16,
    D3DTS_TEXTURE1      = 17,
    D3DTS_TEXTURE2      = 18,
    D3DTS_TEXTURE3      = 19,
    D3DTS_TEXTURE4      = 20,
    D3DTS_TEXTURE5      = 21,
    D3DTS_TEXTURE6      = 22,
    D3DTS_TEXTURE7      = 23,
    D3DTS_FORCE_DWORD     = 0x7fffffff, /* force 32-bit size enum */
} D3DTRANSFORMSTATETYPE;

#define D3DTS_WORLDMATRIX(index) (D3DTRANSFORMSTATETYPE)(index + 256)
#define D3DTS_WORLD  D3DTS_WORLDMATRIX(0)
#define D3DTS_WORLD1 D3DTS_WORLDMATRIX(1)
#define D3DTS_WORLD2 D3DTS_WORLDMATRIX(2)
#define D3DTS_WORLD3 D3DTS_WORLDMATRIX(3)

typedef enum _D3DRENDERSTATETYPE {
    D3DRS_ZENABLE                   = 7,    /* D3DZBUFFERTYPE (or TRUE/FALSE for legacy) */
    D3DRS_FILLMODE                  = 8,    /* D3DFILLMODE */
    D3DRS_SHADEMODE                 = 9,    /* D3DSHADEMODE */
    D3DRS_LINEPATTERN               = 10,   /* D3DLINEPATTERN */
    D3DRS_ZWRITEENABLE              = 14,   /* TRUE to enable z writes */
    D3DRS_ALPHATESTENABLE           = 15,   /* TRUE to enable alpha tests */
    D3DRS_LASTPIXEL                 = 16,   /* TRUE for last-pixel on lines */
    D3DRS_SRCBLEND                  = 19,   /* D3DBLEND */
    D3DRS_DESTBLEND                 = 20,   /* D3DBLEND */
    D3DRS_CULLMODE                  = 22,   /* D3DCULL */
    D3DRS_ZFUNC                     = 23,   /* D3DCMPFUNC */
    D3DRS_ALPHAREF                  = 24,   /* D3DFIXED */
    D3DRS_ALPHAFUNC                 = 25,   /* D3DCMPFUNC */
    D3DRS_DITHERENABLE              = 26,   /* TRUE to enable dithering */
    D3DRS_ALPHABLENDENABLE          = 27,   /* TRUE to enable alpha blending */
    D3DRS_FOGENABLE                 = 28,   /* TRUE to enable fog blending */
    D3DRS_SPECULARENABLE            = 29,   /* TRUE to enable specular */
    D3DRS_ZVISIBLE                  = 30,   /* TRUE to enable z checking */
    D3DRS_FOGCOLOR                  = 34,   /* D3DCOLOR */
    D3DRS_FOGTABLEMODE              = 35,   /* D3DFOGMODE */
    D3DRS_FOGSTART                  = 36,   /* Fog start (for both vertex and pixel fog) */
    D3DRS_FOGEND                    = 37,   /* Fog end      */
    D3DRS_FOGDENSITY                = 38,   /* Fog density  */
    D3DRS_EDGEANTIALIAS             = 40,   /* TRUE to enable edge antialiasing */
    D3DRS_ZBIAS                     = 47,   /* LONG Z bias */
    D3DRS_RANGEFOGENABLE            = 48,   /* Enables range-based fog */
    D3DRS_STENCILENABLE             = 52,   /* BOOL enable/disable stenciling */
    D3DRS_STENCILFAIL               = 53,   /* D3DSTENCILOP to do if stencil test fails */
    D3DRS_STENCILZFAIL              = 54,   /* D3DSTENCILOP to do if stencil test passes and Z test fails */
    D3DRS_STENCILPASS               = 55,   /* D3DSTENCILOP to do if both stencil and Z tests pass */
    D3DRS_STENCILFUNC               = 56,   /* D3DCMPFUNC fn.  Stencil Test passes if ((ref & mask) stencilfn (stencil & mask)) is true */
    D3DRS_STENCILREF                = 57,   /* Reference value used in stencil test */
    D3DRS_STENCILMASK               = 58,   /* Mask value used in stencil test */
    D3DRS_STENCILWRITEMASK          = 59,   /* Write mask applied to values written to stencil buffer */
    D3DRS_TEXTUREFACTOR             = 60,   /* D3DCOLOR used for multi-texture blend */
    D3DRS_WRAP0                     = 128,  /* wrap for 1st texture coord. set */
    D3DRS_WRAP1                     = 129,  /* wrap for 2nd texture coord. set */
    D3DRS_WRAP2                     = 130,  /* wrap for 3rd texture coord. set */
    D3DRS_WRAP3                     = 131,  /* wrap for 4th texture coord. set */
    D3DRS_WRAP4                     = 132,  /* wrap for 5th texture coord. set */
    D3DRS_WRAP5                     = 133,  /* wrap for 6th texture coord. set */
    D3DRS_WRAP6                     = 134,  /* wrap for 7th texture coord. set */
    D3DRS_WRAP7                     = 135,  /* wrap for 8th texture coord. set */
    D3DRS_CLIPPING                  = 136,
    D3DRS_LIGHTING                  = 137,
    D3DRS_AMBIENT                   = 139,
    D3DRS_FOGVERTEXMODE             = 140,
    D3DRS_COLORVERTEX               = 141,
    D3DRS_LOCALVIEWER               = 142,
    D3DRS_NORMALIZENORMALS          = 143,
    D3DRS_DIFFUSEMATERIALSOURCE     = 145,
    D3DRS_SPECULARMATERIALSOURCE    = 146,
    D3DRS_AMBIENTMATERIALSOURCE     = 147,
    D3DRS_EMISSIVEMATERIALSOURCE    = 148,
    D3DRS_VERTEXBLEND               = 151,
    D3DRS_CLIPPLANEENABLE           = 152,
    D3DRS_SOFTWAREVERTEXPROCESSING  = 153,
    D3DRS_POINTSIZE                 = 154,   /* float point size */
    D3DRS_POINTSIZE_MIN             = 155,   /* float point size min threshold */
    D3DRS_POINTSPRITEENABLE         = 156,   /* BOOL point texture coord control */
    D3DRS_POINTSCALEENABLE          = 157,   /* BOOL point size scale enable */
    D3DRS_POINTSCALE_A              = 158,   /* float point attenuation A value */
    D3DRS_POINTSCALE_B              = 159,   /* float point attenuation B value */
    D3DRS_POINTSCALE_C              = 160,   /* float point attenuation C value */
    D3DRS_MULTISAMPLEANTIALIAS      = 161,  // BOOL - set to do FSAA with multisample buffer
    D3DRS_MULTISAMPLEMASK           = 162,  // DWORD - per-sample enable/disable
    D3DRS_PATCHEDGESTYLE            = 163,  // Sets whether patch edges will use float style tessellation
    D3DRS_PATCHSEGMENTS             = 164,  // Number of segments per edge when drawing patches
    D3DRS_DEBUGMONITORTOKEN         = 165,  // DEBUG ONLY - token to debug monitor
    D3DRS_POINTSIZE_MAX             = 166,   /* float point size max threshold */
    D3DRS_INDEXEDVERTEXBLENDENABLE  = 167,
    D3DRS_COLORWRITEENABLE          = 168,  // per-channel write enable
    D3DRS_TWEENFACTOR               = 170,   // float tween factor
    D3DRS_BLENDOP                   = 171,   // D3DBLENDOP setting
    D3DRS_POSITIONORDER             = 172,   // NPatch position interpolation order. D3DORDER_LINEAR or D3DORDER_CUBIC (default)
    D3DRS_NORMALORDER               = 173,   // NPatch normal interpolation order. D3DORDER_LINEAR (default) or D3DORDER_QUADRATIC

    D3DRS_FORCE_DWORD               = 0x7fffffff, /* force 32-bit size enum */
} D3DRENDERSTATETYPE;

// Values for material source
typedef enum _D3DMATERIALCOLORSOURCE
{
    D3DMCS_MATERIAL         = 0,            // Color from material is used
    D3DMCS_COLOR1           = 1,            // Diffuse vertex color is used
    D3DMCS_COLOR2           = 2,            // Specular vertex color is used
    D3DMCS_FORCE_DWORD      = 0x7fffffff,   // force 32-bit size enum
} D3DMATERIALCOLORSOURCE;

// Bias to apply to the texture coordinate set to apply a wrap to.
#define D3DRENDERSTATE_WRAPBIAS                 128UL

/* Flags to construct the WRAP render states */
#define D3DWRAP_U   0x00000001L
#define D3DWRAP_V   0x00000002L
#define D3DWRAP_W   0x00000004L

/* Flags to construct the WRAP render states for 1D thru 4D texture coordinates */
#define D3DWRAPCOORD_0   0x00000001L    // same as D3DWRAP_U
#define D3DWRAPCOORD_1   0x00000002L    // same as D3DWRAP_V
#define D3DWRAPCOORD_2   0x00000004L    // same as D3DWRAP_W
#define D3DWRAPCOORD_3   0x00000008L

/* Flags to construct D3DRS_COLORWRITEENABLE */
#define D3DCOLORWRITEENABLE_RED     (1L<<0)
#define D3DCOLORWRITEENABLE_GREEN   (1L<<1)
#define D3DCOLORWRITEENABLE_BLUE    (1L<<2)
#define D3DCOLORWRITEENABLE_ALPHA   (1L<<3)

/*
 * State enumerants for per-stage texture processing.
 */
typedef enum _D3DTEXTURESTAGESTATETYPE
{
    D3DTSS_COLOROP        =  1, /* D3DTEXTUREOP - per-stage blending controls for color channels */
    D3DTSS_COLORARG1      =  2, /* D3DTA_* (texture arg) */
    D3DTSS_COLORARG2      =  3, /* D3DTA_* (texture arg) */
    D3DTSS_ALPHAOP        =  4, /* D3DTEXTUREOP - per-stage blending controls for alpha channel */
    D3DTSS_ALPHAARG1      =  5, /* D3DTA_* (texture arg) */
    D3DTSS_ALPHAARG2      =  6, /* D3DTA_* (texture arg) */
    D3DTSS_BUMPENVMAT00   =  7, /* float (bump mapping matrix) */
    D3DTSS_BUMPENVMAT01   =  8, /* float (bump mapping matrix) */
    D3DTSS_BUMPENVMAT10   =  9, /* float (bump mapping matrix) */
    D3DTSS_BUMPENVMAT11   = 10, /* float (bump mapping matrix) */
    D3DTSS_TEXCOORDINDEX  = 11, /* identifies which set of texture coordinates index this texture */
    D3DTSS_ADDRESSU       = 13, /* D3DTEXTUREADDRESS for U coordinate */
    D3DTSS_ADDRESSV       = 14, /* D3DTEXTUREADDRESS for V coordinate */
    D3DTSS_BORDERCOLOR    = 15, /* D3DCOLOR */
    D3DTSS_MAGFILTER      = 16, /* D3DTEXTUREFILTER filter to use for magnification */
    D3DTSS_MINFILTER      = 17, /* D3DTEXTUREFILTER filter to use for minification */
    D3DTSS_MIPFILTER      = 18, /* D3DTEXTUREFILTER filter to use between mipmaps during minification */
    D3DTSS_MIPMAPLODBIAS  = 19, /* float Mipmap LOD bias */
    D3DTSS_MAXMIPLEVEL    = 20, /* DWORD 0..(n-1) LOD index of largest map to use (0 == largest) */
    D3DTSS_MAXANISOTROPY  = 21, /* DWORD maximum anisotropy */
    D3DTSS_BUMPENVLSCALE  = 22, /* float scale for bump map luminance */
    D3DTSS_BUMPENVLOFFSET = 23, /* float offset for bump map luminance */
    D3DTSS_TEXTURETRANSFORMFLAGS = 24, /* D3DTEXTURETRANSFORMFLAGS controls texture transform */
    D3DTSS_ADDRESSW       = 25, /* D3DTEXTUREADDRESS for W coordinate */
    D3DTSS_COLORARG0      = 26, /* D3DTA_* third arg for triadic ops */
    D3DTSS_ALPHAARG0      = 27, /* D3DTA_* third arg for triadic ops */
    D3DTSS_RESULTARG      = 28, /* D3DTA_* arg for result (CURRENT or TEMP) */
    D3DTSS_FORCE_DWORD   = 0x7fffffff, /* force 32-bit size enum */
} D3DTEXTURESTAGESTATETYPE;

// Values, used with D3DTSS_TEXCOORDINDEX, to specify that the vertex data(position
// and normal in the camera space) should be taken as texture coordinates
// Low 16 bits are used to specify texture coordinate index, to take the WRAP mode from
//
#define D3DTSS_TCI_PASSTHRU                             0x00000000
#define D3DTSS_TCI_CAMERASPACENORMAL                    0x00010000
#define D3DTSS_TCI_CAMERASPACEPOSITION                  0x00020000
#define D3DTSS_TCI_CAMERASPACEREFLECTIONVECTOR          0x00030000

/*
 * Enumerations for COLOROP and ALPHAOP texture blending operations set in
 * texture processing stage controls in D3DTSS.
 */
typedef enum _D3DTEXTUREOP
{
    // Control
    D3DTOP_DISABLE              = 1,      // disables stage
    D3DTOP_SELECTARG1           = 2,      // the default
    D3DTOP_SELECTARG2           = 3,

    // Modulate
    D3DTOP_MODULATE             = 4,      // multiply args together
    D3DTOP_MODULATE2X           = 5,      // multiply and  1 bit
    D3DTOP_MODULATE4X           = 6,      // multiply and  2 bits

    // Add
    D3DTOP_ADD                  =  7,   // add arguments together
    D3DTOP_ADDSIGNED            =  8,   // add with -0.5 bias
    D3DTOP_ADDSIGNED2X          =  9,   // as above but left  1 bit
    D3DTOP_SUBTRACT             = 10,   // Arg1 - Arg2, with no saturation
    D3DTOP_ADDSMOOTH            = 11,   // add 2 args, subtract product
                                        // Arg1 + Arg2 - Arg1*Arg2
                                        // = Arg1 + (1-Arg1)*Arg2

    // Linear alpha blend: Arg1*(Alpha) + Arg2*(1-Alpha)
    D3DTOP_BLENDDIFFUSEALPHA    = 12, // iterated alpha
    D3DTOP_BLENDTEXTUREALPHA    = 13, // texture alpha
    D3DTOP_BLENDFACTORALPHA     = 14, // alpha from D3DRS_TEXTUREFACTOR

    // Linear alpha blend with pre-multiplied arg1 input: Arg1 + Arg2*(1-Alpha)
    D3DTOP_BLENDTEXTUREALPHAPM  = 15, // texture alpha
    D3DTOP_BLENDCURRENTALPHA    = 16, // by alpha of current color

    // Specular mapping
    D3DTOP_PREMODULATE            = 17,     // modulate with next texture before use
    D3DTOP_MODULATEALPHA_ADDCOLOR = 18,     // Arg1.RGB + Arg1.A*Arg2.RGB
                                            // COLOROP only
    D3DTOP_MODULATECOLOR_ADDALPHA = 19,     // Arg1.RGB*Arg2.RGB + Arg1.A
                                            // COLOROP only
    D3DTOP_MODULATEINVALPHA_ADDCOLOR = 20,  // (1-Arg1.A)*Arg2.RGB + Arg1.RGB
                                            // COLOROP only
    D3DTOP_MODULATEINVCOLOR_ADDALPHA = 21,  // (1-Arg1.RGB)*Arg2.RGB + Arg1.A
                                            // COLOROP only

    // Bump mapping
    D3DTOP_BUMPENVMAP           = 22, // per pixel env map perturbation
    D3DTOP_BUMPENVMAPLUMINANCE  = 23, // with luminance channel

    // This can do either diffuse or specular bump mapping with correct input.
    // Performs the function (Arg1.R*Arg2.R + Arg1.G*Arg2.G + Arg1.B*Arg2.B)
    // where each component has been scaled and offset to make it signed.
    // The result is replicated into all four (including alpha) channels.
    // This is a valid COLOROP only.
    D3DTOP_DOTPRODUCT3          = 24,

    // Triadic ops
    D3DTOP_MULTIPLYADD          = 25, // Arg0 + Arg1*Arg2
    D3DTOP_LERP                 = 26, // (Arg0)*Arg1 + (1-Arg0)*Arg2

    D3DTOP_FORCE_DWORD = 0x7fffffff,
} D3DTEXTUREOP;

/*
 * Values for COLORARG0,1,2, ALPHAARG0,1,2, and RESULTARG texture blending
 * operations set in texture processing stage controls in D3DRENDERSTATE.
 */
#define D3DTA_SELECTMASK        0x0000000f  // mask for arg selector
#define D3DTA_DIFFUSE           0x00000000  // select diffuse color (read only)
#define D3DTA_CURRENT           0x00000001  // select stage destination register (read/write)
#define D3DTA_TEXTURE           0x00000002  // select texture color (read only)
#define D3DTA_TFACTOR           0x00000003  // select D3DRS_TEXTUREFACTOR (read only)
#define D3DTA_SPECULAR          0x00000004  // select specular color (read only)
#define D3DTA_TEMP              0x00000005  // select temporary register color (read/write)
#define D3DTA_COMPLEMENT        0x00000010  // take 1.0 - x (read modifier)
#define D3DTA_ALPHAREPLICATE    0x00000020  // replicate alpha to color components (read modifier)

//
// Values for D3DTSS_***FILTER texture stage states
//
typedef enum _D3DTEXTUREFILTERTYPE
{
    D3DTEXF_NONE            = 0,    // filtering disabled (valid for mip filter only)
    D3DTEXF_POINT           = 1,    // nearest
    D3DTEXF_LINEAR          = 2,    // linear interpolation
    D3DTEXF_ANISOTROPIC     = 3,    // anisotropic
    D3DTEXF_FLATCUBIC       = 4,    // cubic
    D3DTEXF_GAUSSIANCUBIC   = 5,    // different cubic kernel
    D3DTEXF_FORCE_DWORD     = 0x7fffffff,   // force 32-bit size enum
} D3DTEXTUREFILTERTYPE;

/* Bits for Flags in ProcessVertices call */

#define D3DPV_DONOTCOPYDATA     (1 << 0)

//-------------------------------------------------------------------

// Flexible vertex format bits
//
#define D3DFVF_RESERVED0        0x001
#define D3DFVF_POSITION_MASK    0x00E
#define D3DFVF_XYZ              0x002
#define D3DFVF_XYZRHW           0x004
#define D3DFVF_XYZB1            0x006
#define D3DFVF_XYZB2            0x008
#define D3DFVF_XYZB3            0x00a
#define D3DFVF_XYZB4            0x00c
#define D3DFVF_XYZB5            0x00e

#define D3DFVF_NORMAL           0x010
#define D3DFVF_PSIZE            0x020
#define D3DFVF_DIFFUSE          0x040
#define D3DFVF_SPECULAR         0x080

#define D3DFVF_TEXCOUNT_MASK    0xf00
#define D3DFVF_TEXCOUNT_SHIFT   8
#define D3DFVF_TEX0             0x000
#define D3DFVF_TEX1             0x100
#define D3DFVF_TEX2             0x200
#define D3DFVF_TEX3             0x300
#define D3DFVF_TEX4             0x400
#define D3DFVF_TEX5             0x500
#define D3DFVF_TEX6             0x600
#define D3DFVF_TEX7             0x700
#define D3DFVF_TEX8             0x800

#define D3DFVF_LASTBETA_UBYTE4  0x1000

#define D3DFVF_RESERVED2        0xE000  // 4 reserved bits

//---------------------------------------------------------------------
// Vertex Shaders
//

/*
Vertex Shader Declaration
The declaration portion of a vertex shader defines the static external
interface of the shader.  The information in the declaration includes:
- Assignments of vertex shader input registers to data streams.  These
assignments bind a specific vertex register to a single component within a
vertex stream.  A vertex stream element is identified by a byte offset
within the stream and a type.  The type specifies the arithmetic data type
plus the dimensionality (1, 2, 3, or 4 values).  Stream data which is
less than 4 values are always expanded out to 4 values with zero or more
0.F values and one 1.F value.
- Assignment of vertex shader input registers to implicit data from the
primitive tessellator.  This controls the loading of vertex data which is
not loaded from a stream, but rather is generated during primitive
tessellation prior to the vertex shader.
- Loading data into the constant memory at the time a shader is set as the
current shader.  Each token specifies values for one or more contiguous 4
DWORD constant registers.  This allows the shader to update an arbitrary
subset of the constant memory, overwriting the device state (which
contains the current values of the constant memory).  Note that these
values can be subsequently overwritten (between DrawPrimitive calls)
during the time a shader is bound to a device via the
SetVertexShaderConstant method.
Declaration arrays are single-dimensional arrays of DWORDs composed of
multiple tokens each of which is one or more DWORDs.  The single-DWORD
token value 0xFFFFFFFF is a special token used to indicate the end of the
declaration array.  The single DWORD token value 0x00000000 is a NOP token
with is ignored during the declaration parsing.  Note that 0x00000000 is a
valid value for DWORDs following the first DWORD for multiple word tokens.
[31:29] TokenType
    0x0 - NOP (requires all DWORD bits to be zero)
    0x1 - stream selector
    0x2 - stream data definition (map to vertex input memory)
    0x3 - vertex input memory from tessellator
    0x4 - constant memory from shader
    0x5 - extension
    0x6 - reserved
    0x7 - end-of-array (requires all DWORD bits to be 1)
NOP Token (single DWORD token)
    [31:29] 0x0
    [28:00] 0x0
Stream Selector (single DWORD token)
    [31:29] 0x1
    [28]    indicates whether this is a tessellator stream
    [27:04] 0x0
    [03:00] stream selector (0..15)
Stream Data Definition (single DWORD token)
    Vertex Input Register Load
      [31:29] 0x2
      [28]    0x0
      [27:20] 0x0
      [19:16] type (dimensionality and data type)
      [15:04] 0x0
      [03:00] vertex register address (0..15)
    Data Skip (no register load)
      [31:29] 0x2
      [28]    0x1
      [27:20] 0x0
      [19:16] count of DWORDS to skip over (0..15)
      [15:00] 0x0
    Vertex Input Memory from Tessellator Data (single DWORD token)
      [31:29] 0x3
      [28]    indicates whether data is normals or u/v
      [27:24] 0x0
      [23:20] vertex register address (0..15)
      [19:16] type (dimensionality)
      [15:04] 0x0
      [03:00] vertex register address (0..15)
Constant Memory from Shader (multiple DWORD token)
    [31:29] 0x4
    [28:25] count of 4*DWORD constants to load (0..15)
    [24:07] 0x0
    [06:00] constant memory address (0..95)
Extension Token (single or multiple DWORD token)
    [31:29] 0x5
    [28:24] count of additional DWORDs in token (0..31)
    [23:00] extension-specific information
End-of-array token (single DWORD token)
    [31:29] 0x7
    [28:00] 0x1fffffff
The stream selector token must be immediately followed by a contiguous set of stream data definition tokens.  This token sequence fully defines that stream, including the set of elements within the stream, the order in which the elements appear, the type of each element, and the vertex register into which to load an element.
Streams are allowed to include data which is not loaded into a vertex register, thus allowing data which is not used for this shader to exist in the vertex stream.  This skipped data is defined only by a count of DWORDs to skip over, since the type information is irrelevant.
The token sequence:
Stream Select: stream=0
Stream Data Definition (Load): type=FLOAT3; register=3
Stream Data Definition (Load): type=FLOAT3; register=4
Stream Data Definition (Skip): count=2
Stream Data Definition (Load): type=FLOAT2; register=7
defines stream zero to consist of 4 elements, 3 of which are loaded into registers and the fourth skipped over.  Register 3 is loaded with the first three DWORDs in each vertex interpreted as FLOAT data.  Register 4 is loaded with the 4th, 5th, and 6th DWORDs interpreted as FLOAT data.  The next two DWORDs (7th and 8th) are skipped over and not loaded into any vertex input register.   Register 7 is loaded with the 9th and 10th DWORDS interpreted as FLOAT data.
Placing of tokens other than NOPs between the Stream Selector and Stream Data Definition tokens is disallowed.
*/

typedef enum _D3DVSD_TOKENTYPE
{
    D3DVSD_TOKEN_NOP        = 0,    // NOP or extension
    D3DVSD_TOKEN_STREAM,            // stream selector
    D3DVSD_TOKEN_STREAMDATA,        // stream data definition (map to vertex input memory)
    D3DVSD_TOKEN_TESSELLATOR,       // vertex input memory from tessellator
    D3DVSD_TOKEN_CONSTMEM,          // constant memory from shader
    D3DVSD_TOKEN_EXT,               // extension
    D3DVSD_TOKEN_END = 7,           // end-of-array (requires all DWORD bits to be 1)
    D3DVSD_FORCE_DWORD = 0x7fffffff,// force 32-bit size enum
} D3DVSD_TOKENTYPE;

#define D3DVSD_TOKENTYPESHIFT   29
#define D3DVSD_TOKENTYPEMASK    (7 << D3DVSD_TOKENTYPESHIFT)

#define D3DVSD_STREAMNUMBERSHIFT 0
#define D3DVSD_STREAMNUMBERMASK (0xF << D3DVSD_STREAMNUMBERSHIFT)

#define D3DVSD_DATALOADTYPESHIFT 28
#define D3DVSD_DATALOADTYPEMASK (0x1 << D3DVSD_DATALOADTYPESHIFT)

#define D3DVSD_DATATYPESHIFT 16
#define D3DVSD_DATATYPEMASK (0xF << D3DVSD_DATATYPESHIFT)

#define D3DVSD_SKIPCOUNTSHIFT 16
#define D3DVSD_SKIPCOUNTMASK (0xF << D3DVSD_SKIPCOUNTSHIFT)

#define D3DVSD_VERTEXREGSHIFT 0
#define D3DVSD_VERTEXREGMASK (0x1F << D3DVSD_VERTEXREGSHIFT)

#define D3DVSD_VERTEXREGINSHIFT 20
#define D3DVSD_VERTEXREGINMASK (0xF << D3DVSD_VERTEXREGINSHIFT)

#define D3DVSD_CONSTCOUNTSHIFT 25
#define D3DVSD_CONSTCOUNTMASK (0xF << D3DVSD_CONSTCOUNTSHIFT)

#define D3DVSD_CONSTADDRESSSHIFT 0
#define D3DVSD_CONSTADDRESSMASK (0x7F << D3DVSD_CONSTADDRESSSHIFT)

#define D3DVSD_CONSTRSSHIFT 16
#define D3DVSD_CONSTRSMASK (0x1FFF << D3DVSD_CONSTRSSHIFT)

#define D3DVSD_EXTCOUNTSHIFT 24
#define D3DVSD_EXTCOUNTMASK (0x1F << D3DVSD_EXTCOUNTSHIFT)

#define D3DVSD_EXTINFOSHIFT 0
#define D3DVSD_EXTINFOMASK (0xFFFFFF << D3DVSD_EXTINFOSHIFT)

#define D3DVSD_MAKETOKENTYPE(tokenType) ((tokenType << D3DVSD_TOKENTYPESHIFT) & D3DVSD_TOKENTYPEMASK)

// macros for generation of CreateVertexShader Declaration token array

// Set current stream
// _StreamNumber [0..(MaxStreams-1)] stream to get data from
//
#define D3DVSD_STREAM( _StreamNumber ) \
    (D3DVSD_MAKETOKENTYPE(D3DVSD_TOKEN_STREAM) | (_StreamNumber))

// Set tessellator stream
//
#define D3DVSD_STREAMTESSSHIFT 28
#define D3DVSD_STREAMTESSMASK (1 << D3DVSD_STREAMTESSSHIFT)
#define D3DVSD_STREAM_TESS( ) \
    (D3DVSD_MAKETOKENTYPE(D3DVSD_TOKEN_STREAM) | (D3DVSD_STREAMTESSMASK))

// bind single vertex register to vertex element from vertex stream
//
// _VertexRegister [0..15] address of the vertex register
// _Type [D3DVSDT_*] dimensionality and arithmetic data type

#define D3DVSD_REG( _VertexRegister, _Type ) \
    (D3DVSD_MAKETOKENTYPE(D3DVSD_TOKEN_STREAMDATA) |            \
     ((_Type) << D3DVSD_DATATYPESHIFT) | (_VertexRegister))

// Skip _DWORDCount DWORDs in vertex
//
#define D3DVSD_SKIP( _DWORDCount ) \
    (D3DVSD_MAKETOKENTYPE(D3DVSD_TOKEN_STREAMDATA) | 0x10000000 | \
     ((_DWORDCount) << D3DVSD_SKIPCOUNTSHIFT))

// load data into vertex shader constant memory
//
// _ConstantAddress [0..95] - address of constant array to begin filling data
// _Count [0..15] - number of constant vectors to load (4 DWORDs each)
// followed by 4*_Count DWORDS of data
//
#define D3DVSD_CONST( _ConstantAddress, _Count ) \
    (D3DVSD_MAKETOKENTYPE(D3DVSD_TOKEN_CONSTMEM) | \
     ((_Count) << D3DVSD_CONSTCOUNTSHIFT) | (_ConstantAddress))

// enable tessellator generated normals
//
// _VertexRegisterIn  [0..15] address of vertex register whose input stream
//                            will be used in normal computation
// _VertexRegisterOut [0..15] address of vertex register to output the normal to
//
#define D3DVSD_TESSNORMAL( _VertexRegisterIn, _VertexRegisterOut ) \
    (D3DVSD_MAKETOKENTYPE(D3DVSD_TOKEN_TESSELLATOR) | \
     ((_VertexRegisterIn) << D3DVSD_VERTEXREGINSHIFT) | \
     ((0x02) << D3DVSD_DATATYPESHIFT) | (_VertexRegisterOut))

// enable tessellator generated surface parameters
//
// _VertexRegister [0..15] address of vertex register to output parameters
//
#define D3DVSD_TESSUV( _VertexRegister ) \
    (D3DVSD_MAKETOKENTYPE(D3DVSD_TOKEN_TESSELLATOR) | 0x10000000 | \
     ((0x01) << D3DVSD_DATATYPESHIFT) | (_VertexRegister))

// Generates END token
//
#define D3DVSD_END() 0xFFFFFFFF

// Generates NOP token
#define D3DVSD_NOP() 0x00000000

// bit declarations for _Type fields
#define D3DVSDT_FLOAT1      0x00    // 1D float expanded to (value, 0., 0., 1.)
#define D3DVSDT_FLOAT2      0x01    // 2D float expanded to (value, value, 0., 1.)
#define D3DVSDT_FLOAT3      0x02    // 3D float expanded to (value, value, value, 1.)
#define D3DVSDT_FLOAT4      0x03    // 4D float
#define D3DVSDT_D3DCOLOR    0x04    // 4D packed unsigned bytes mapped to 0. to 1. range
                                    // Input is in D3DCOLOR format (ARGB) expanded to (R, G, B, A)
#define D3DVSDT_UBYTE4      0x05    // 4D unsigned byte
#define D3DVSDT_SHORT2      0x06    // 2D signed short expanded to (value, value, 0., 1.)
#define D3DVSDT_SHORT4      0x07    // 4D signed short

// assignments of vertex input registers for fixed function vertex shader
//
#define D3DVSDE_POSITION        0
#define D3DVSDE_BLENDWEIGHT     1
#define D3DVSDE_BLENDINDICES    2
#define D3DVSDE_NORMAL          3
#define D3DVSDE_PSIZE           4
#define D3DVSDE_DIFFUSE         5
#define D3DVSDE_SPECULAR        6
#define D3DVSDE_TEXCOORD0       7
#define D3DVSDE_TEXCOORD1       8
#define D3DVSDE_TEXCOORD2       9
#define D3DVSDE_TEXCOORD3       10
#define D3DVSDE_TEXCOORD4       11
#define D3DVSDE_TEXCOORD5       12
#define D3DVSDE_TEXCOORD6       13
#define D3DVSDE_TEXCOORD7       14
#define D3DVSDE_POSITION2       15
#define D3DVSDE_NORMAL2         16

// Maximum supported number of texture coordinate sets
#define D3DDP_MAXTEXCOORD   8


//
// Instruction Token Bit Definitions
//
#define D3DSI_OPCODE_MASK       0x0000FFFF

typedef enum _D3DSHADER_INSTRUCTION_OPCODE_TYPE
{
    D3DSIO_NOP          = 0,    // PS/VS
    D3DSIO_MOV          ,       // PS/VS
    D3DSIO_ADD          ,       // PS/VS
    D3DSIO_SUB          ,       // PS
    D3DSIO_MAD          ,       // PS/VS
    D3DSIO_MUL          ,       // PS/VS
    D3DSIO_RCP          ,       // VS
    D3DSIO_RSQ          ,       // VS
    D3DSIO_DP3          ,       // PS/VS
    D3DSIO_DP4          ,       // PS/VS
    D3DSIO_MIN          ,       // VS
    D3DSIO_MAX          ,       // VS
    D3DSIO_SLT          ,       // VS
    D3DSIO_SGE          ,       // VS
    D3DSIO_EXP          ,       // VS
    D3DSIO_LOG          ,       // VS
    D3DSIO_LIT          ,       // VS
    D3DSIO_DST          ,       // VS
    D3DSIO_LRP          ,       // PS
    D3DSIO_FRC          ,       // VS
    D3DSIO_M4x4         ,       // VS
    D3DSIO_M4x3         ,       // VS
    D3DSIO_M3x4         ,       // VS
    D3DSIO_M3x3         ,       // VS
    D3DSIO_M3x2         ,       // VS

    D3DSIO_TEXCOORD     = 64,   // PS
    D3DSIO_TEXKILL      ,       // PS
    D3DSIO_TEX          ,       // PS
    D3DSIO_TEXBEM       ,       // PS
    D3DSIO_TEXBEML      ,       // PS
    D3DSIO_TEXREG2AR    ,       // PS
    D3DSIO_TEXREG2GB    ,       // PS
    D3DSIO_TEXM3x2PAD   ,       // PS
    D3DSIO_TEXM3x2TEX   ,       // PS
    D3DSIO_TEXM3x3PAD   ,       // PS
    D3DSIO_TEXM3x3TEX   ,       // PS
    D3DSIO_TEXM3x3DIFF  ,       // PS
    D3DSIO_TEXM3x3SPEC  ,       // PS
    D3DSIO_TEXM3x3VSPEC ,       // PS
    D3DSIO_EXPP         ,       // VS
    D3DSIO_LOGP         ,       // VS
    D3DSIO_CND          ,       // PS
    D3DSIO_DEF          ,       // PS
    D3DSIO_TEXREG2RGB   ,       // PS
    D3DSIO_TEXDP3TEX    ,       // PS
    D3DSIO_TEXM3x2DEPTH ,       // PS
    D3DSIO_TEXDP3       ,       // PS
    D3DSIO_TEXM3x3      ,       // PS
    D3DSIO_TEXDEPTH     ,       // PS
    D3DSIO_CMP          ,       // PS
    D3DSIO_BEM          ,       // PS

    D3DSIO_PHASE        = 0xFFFD,
    D3DSIO_COMMENT      = 0xFFFE,
    D3DSIO_END          = 0xFFFF,

    D3DSIO_FORCE_DWORD  = 0x7fffffff,   // force 32-bit size enum
} D3DSHADER_INSTRUCTION_OPCODE_TYPE;

//
// Co-Issue Instruction Modifier - if set then this instruction is to be
// issued in parallel with the previous instruction(s) for which this bit
// is not set.
//
#define D3DSI_COISSUE           0x40000000

//
// Parameter Token Bit Definitions
//
#define D3DSP_REGNUM_MASK       0x00001FFF

// destination parameter write mask
#define D3DSP_WRITEMASK_0       0x00010000  // Component 0 (X;Red)
#define D3DSP_WRITEMASK_1       0x00020000  // Component 1 (Y;Green)
#define D3DSP_WRITEMASK_2       0x00040000  // Component 2 (Z;Blue)
#define D3DSP_WRITEMASK_3       0x00080000  // Component 3 (W;Alpha)
#define D3DSP_WRITEMASK_ALL     0x000F0000  // All Components

// destination parameter modifiers
#define D3DSP_DSTMOD_SHIFT      20
#define D3DSP_DSTMOD_MASK       0x00F00000

typedef enum _D3DSHADER_PARAM_DSTMOD_TYPE
{
    D3DSPDM_NONE    = 0<<D3DSP_DSTMOD_SHIFT, // nop
    D3DSPDM_SATURATE= 1<<D3DSP_DSTMOD_SHIFT, // clamp to 0. to 1. range
    D3DSPDM_FORCE_DWORD  = 0x7fffffff,      // force 32-bit size enum
} D3DSHADER_PARAM_DSTMOD_TYPE;

// destination parameter 
#define D3DSP_DSTSHIFT_SHIFT    24
#define D3DSP_DSTSHIFT_MASK     0x0F000000

// destination/source parameter register type
#define D3DSP_REGTYPE_SHIFT     28
#define D3DSP_REGTYPE_MASK      0x70000000

typedef enum _D3DSHADER_PARAM_REGISTER_TYPE
{
    D3DSPR_TEMP     = 0<<D3DSP_REGTYPE_SHIFT, // Temporary Register File
    D3DSPR_INPUT    = 1<<D3DSP_REGTYPE_SHIFT, // Input Register File
    D3DSPR_CONST    = 2<<D3DSP_REGTYPE_SHIFT, // Constant Register File
    D3DSPR_ADDR     = 3<<D3DSP_REGTYPE_SHIFT, // Address Register (VS)
    D3DSPR_TEXTURE  = 3<<D3DSP_REGTYPE_SHIFT, // Texture Register File (PS)
    D3DSPR_RASTOUT  = 4<<D3DSP_REGTYPE_SHIFT, // Rasterizer Register File
    D3DSPR_ATTROUT  = 5<<D3DSP_REGTYPE_SHIFT, // Attribute Output Register File
    D3DSPR_TEXCRDOUT= 6<<D3DSP_REGTYPE_SHIFT, // Texture Coordinate Output Register File
    D3DSPR_FORCE_DWORD  = 0x7fffffff,         // force 32-bit size enum
} D3DSHADER_PARAM_REGISTER_TYPE;

// Register offsets in the Rasterizer Register File
//
typedef enum _D3DVS_RASTOUT_OFFSETS
{
    D3DSRO_POSITION = 0,
    D3DSRO_FOG,
    D3DSRO_POINT_SIZE,
    D3DSRO_FORCE_DWORD  = 0x7fffffff,         // force 32-bit size enum
} D3DVS_RASTOUT_OFFSETS;

// Source operand addressing modes

#define D3DVS_ADDRESSMODE_SHIFT 13
#define D3DVS_ADDRESSMODE_MASK  (1 << D3DVS_ADDRESSMODE_SHIFT)

typedef enum _D3DVS_ADDRESSMODE_TYPE
{
    D3DVS_ADDRMODE_ABSOLUTE  = (0 << D3DVS_ADDRESSMODE_SHIFT),
    D3DVS_ADDRMODE_RELATIVE  = (1 << D3DVS_ADDRESSMODE_SHIFT),   // Relative to register A0
    D3DVS_ADDRMODE_FORCE_DWORD = 0x7fffffff, // force 32-bit size enum
} D3DVS_ADDRESSMODE_TYPE;

// Source operand swizzle definitions
//
#define D3DVS_SWIZZLE_SHIFT     16
#define D3DVS_SWIZZLE_MASK      0x00FF0000

// The following bits define where to take component X from:

#define D3DVS_X_X       (0 << D3DVS_SWIZZLE_SHIFT)
#define D3DVS_X_Y       (1 << D3DVS_SWIZZLE_SHIFT)
#define D3DVS_X_Z       (2 << D3DVS_SWIZZLE_SHIFT)
#define D3DVS_X_W       (3 << D3DVS_SWIZZLE_SHIFT)

// The following bits define where to take component Y from:

#define D3DVS_Y_X       (0 << (D3DVS_SWIZZLE_SHIFT + 2))
#define D3DVS_Y_Y       (1 << (D3DVS_SWIZZLE_SHIFT + 2))
#define D3DVS_Y_Z       (2 << (D3DVS_SWIZZLE_SHIFT + 2))
#define D3DVS_Y_W       (3 << (D3DVS_SWIZZLE_SHIFT + 2))

// The following bits define where to take component Z from:

#define D3DVS_Z_X       (0 << (D3DVS_SWIZZLE_SHIFT + 4))
#define D3DVS_Z_Y       (1 << (D3DVS_SWIZZLE_SHIFT + 4))
#define D3DVS_Z_Z       (2 << (D3DVS_SWIZZLE_SHIFT + 4))
#define D3DVS_Z_W       (3 << (D3DVS_SWIZZLE_SHIFT + 4))

// The following bits define where to take component W from:

#define D3DVS_W_X       (0 << (D3DVS_SWIZZLE_SHIFT + 6))
#define D3DVS_W_Y       (1 << (D3DVS_SWIZZLE_SHIFT + 6))
#define D3DVS_W_Z       (2 << (D3DVS_SWIZZLE_SHIFT + 6))
#define D3DVS_W_W       (3 << (D3DVS_SWIZZLE_SHIFT + 6))

// Value when there is no swizzle (X is taken from X, Y is taken from Y,
// Z is taken from Z, W is taken from W
//
#define D3DVS_NOSWIZZLE (D3DVS_X_X | D3DVS_Y_Y | D3DVS_Z_Z | D3DVS_W_W)

// source parameter swizzle
#define D3DSP_SWIZZLE_SHIFT     16
#define D3DSP_SWIZZLE_MASK      0x00FF0000

#define D3DSP_NOSWIZZLE \
    ( (0 << (D3DSP_SWIZZLE_SHIFT + 0)) | \
      (1 << (D3DSP_SWIZZLE_SHIFT + 2)) | \
      (2 << (D3DSP_SWIZZLE_SHIFT + 4)) | \
      (3 << (D3DSP_SWIZZLE_SHIFT + 6)) )

// pixel-shader swizzle ops
#define D3DSP_REPLICATERED \
    ( (0 << (D3DSP_SWIZZLE_SHIFT + 0)) | \
      (0 << (D3DSP_SWIZZLE_SHIFT + 2)) | \
      (0 << (D3DSP_SWIZZLE_SHIFT + 4)) | \
      (0 << (D3DSP_SWIZZLE_SHIFT + 6)) )

#define D3DSP_REPLICATEGREEN \
    ( (1 << (D3DSP_SWIZZLE_SHIFT + 0)) | \
      (1 << (D3DSP_SWIZZLE_SHIFT + 2)) | \
      (1 << (D3DSP_SWIZZLE_SHIFT + 4)) | \
      (1 << (D3DSP_SWIZZLE_SHIFT + 6)) )

#define D3DSP_REPLICATEBLUE \
    ( (2 << (D3DSP_SWIZZLE_SHIFT + 0)) | \
      (2 << (D3DSP_SWIZZLE_SHIFT + 2)) | \
      (2 << (D3DSP_SWIZZLE_SHIFT + 4)) | \
      (2 << (D3DSP_SWIZZLE_SHIFT + 6)) )

#define D3DSP_REPLICATEALPHA \
    ( (3 << (D3DSP_SWIZZLE_SHIFT + 0)) | \
      (3 << (D3DSP_SWIZZLE_SHIFT + 2)) | \
      (3 << (D3DSP_SWIZZLE_SHIFT + 4)) | \
      (3 << (D3DSP_SWIZZLE_SHIFT + 6)) )

// source parameter modifiers
#define D3DSP_SRCMOD_SHIFT      24
#define D3DSP_SRCMOD_MASK       0x0F000000

typedef enum _D3DSHADER_PARAM_SRCMOD_TYPE
{
    D3DSPSM_NONE    = 0<<D3DSP_SRCMOD_SHIFT, // nop
    D3DSPSM_NEG     = 1<<D3DSP_SRCMOD_SHIFT, // negate
    D3DSPSM_BIAS    = 2<<D3DSP_SRCMOD_SHIFT, // bias
    D3DSPSM_BIASNEG = 3<<D3DSP_SRCMOD_SHIFT, // bias and negate
    D3DSPSM_SIGN    = 4<<D3DSP_SRCMOD_SHIFT, // sign
    D3DSPSM_SIGNNEG = 5<<D3DSP_SRCMOD_SHIFT, // sign and negate
    D3DSPSM_COMP    = 6<<D3DSP_SRCMOD_SHIFT, // complement
    D3DSPSM_X2      = 7<<D3DSP_SRCMOD_SHIFT, // *2
    D3DSPSM_X2NEG   = 8<<D3DSP_SRCMOD_SHIFT, // *2 and negate
    D3DSPSM_DZ      = 9<<D3DSP_SRCMOD_SHIFT, // divide through by z component 
    D3DSPSM_DW      = 10<<D3DSP_SRCMOD_SHIFT, // divide through by w component
    D3DSPSM_FORCE_DWORD = 0x7fffffff,        // force 32-bit size enum
} D3DSHADER_PARAM_SRCMOD_TYPE;

// pixel shader version token
#define D3DPS_VERSION(_Major,_Minor) (0xFFFF0000|((_Major)<<8)|(_Minor))

// vertex shader version token
#define D3DVS_VERSION(_Major,_Minor) (0xFFFE0000|((_Major)<<8)|(_Minor))

// extract major/minor from version cap
#define D3DSHADER_VERSION_MAJOR(_Version) (((_Version)>>8)&0xFF)
#define D3DSHADER_VERSION_MINOR(_Version) (((_Version)>>0)&0xFF)

// destination/source parameter register type
#define D3DSI_COMMENTSIZE_SHIFT     16
#define D3DSI_COMMENTSIZE_MASK      0x7FFF0000
#define D3DSHADER_COMMENT(_DWordSize) \
    ((((_DWordSize)<<D3DSI_COMMENTSIZE_SHIFT)&D3DSI_COMMENTSIZE_MASK)|D3DSIO_COMMENT)

// pixel/vertex shader end token
#define D3DPS_END()  0x0000FFFF
#define D3DVS_END()  0x0000FFFF

//---------------------------------------------------------------------

// High order surfaces
//
typedef enum _D3DBASISTYPE
{
   D3DBASIS_BEZIER      = 0,
   D3DBASIS_BSPLINE     = 1,
   D3DBASIS_INTERPOLATE = 2,
   D3DBASIS_FORCE_DWORD = 0x7fffffff,
} D3DBASISTYPE;

typedef enum _D3DORDERTYPE
{
   D3DORDER_LINEAR      = 1,
   D3DORDER_QUADRATIC   = 2,
   D3DORDER_CUBIC       = 3,
   D3DORDER_QUINTIC     = 5,
   D3DORDER_FORCE_DWORD = 0x7fffffff,
} D3DORDERTYPE;

typedef enum _D3DPATCHEDGESTYLE
{
   D3DPATCHEDGE_DISCRETE    = 0,
   D3DPATCHEDGE_CONTINUOUS  = 1,
   D3DPATCHEDGE_FORCE_DWORD = 0x7fffffff,
} D3DPATCHEDGESTYLE;

typedef enum _D3DSTATEBLOCKTYPE
{
    D3DSBT_ALL           = 1, // capture all state
    D3DSBT_PIXELSTATE    = 2, // capture pixel state
    D3DSBT_VERTEXSTATE   = 3, // capture vertex state
    D3DSBT_FORCE_DWORD   = 0x7fffffff,
} D3DSTATEBLOCKTYPE;

// The D3DVERTEXBLENDFLAGS type is used with D3DRS_VERTEXBLEND state.
//
typedef enum _D3DVERTEXBLENDFLAGS
{
    D3DVBF_DISABLE  = 0,     // Disable vertex blending
    D3DVBF_1WEIGHTS = 1,     // 2 matrix blending
    D3DVBF_2WEIGHTS = 2,     // 3 matrix blending
    D3DVBF_3WEIGHTS = 3,     // 4 matrix blending
    D3DVBF_TWEENING = 255,   // blending using D3DRS_TWEENFACTOR
    D3DVBF_0WEIGHTS = 256,   // one matrix is used with weight 1.0
    D3DVBF_FORCE_DWORD = 0x7fffffff, // force 32-bit size enum
} D3DVERTEXBLENDFLAGS;

typedef enum _D3DTEXTURETRANSFORMFLAGS {
    D3DTTFF_DISABLE         = 0,    // texture coordinates are passed directly
    D3DTTFF_COUNT1          = 1,    // rasterizer should expect 1-D texture coords
    D3DTTFF_COUNT2          = 2,    // rasterizer should expect 2-D texture coords
    D3DTTFF_COUNT3          = 3,    // rasterizer should expect 3-D texture coords
    D3DTTFF_COUNT4          = 4,    // rasterizer should expect 4-D texture coords
    D3DTTFF_PROJECTED       = 256,  // texcoords to be divided by COUNTth element
    D3DTTFF_FORCE_DWORD     = 0x7fffffff,
} D3DTEXTURETRANSFORMFLAGS;

// Macros to set texture coordinate format bits in the FVF id

#define D3DFVF_TEXTUREFORMAT2 0         // Two floating point values
#define D3DFVF_TEXTUREFORMAT1 3         // One floating point value
#define D3DFVF_TEXTUREFORMAT3 1         // Three floating point values
#define D3DFVF_TEXTUREFORMAT4 2         // Four floating point values

#define D3DFVF_TEXCOORDSIZE3(CoordIndex) (D3DFVF_TEXTUREFORMAT3 << (CoordIndex*2 + 16))
#define D3DFVF_TEXCOORDSIZE2(CoordIndex) (D3DFVF_TEXTUREFORMAT2)
#define D3DFVF_TEXCOORDSIZE4(CoordIndex) (D3DFVF_TEXTUREFORMAT4 << (CoordIndex*2 + 16))
#define D3DFVF_TEXCOORDSIZE1(CoordIndex) (D3DFVF_TEXTUREFORMAT1 << (CoordIndex*2 + 16))


//---------------------------------------------------------------------

/* Direct3D8 Device types */
typedef enum _D3DDEVTYPE
{
    D3DDEVTYPE_HAL         = 1,
    D3DDEVTYPE_REF         = 2,
    D3DDEVTYPE_SW          = 3,

    D3DDEVTYPE_FORCE_DWORD  = 0x7fffffff
} D3DDEVTYPE;

/* Multi-Sample buffer types */
typedef enum _D3DMULTISAMPLE_TYPE
{
    D3DMULTISAMPLE_NONE            =  0,
    D3DMULTISAMPLE_2_SAMPLES       =  2,
    D3DMULTISAMPLE_3_SAMPLES       =  3,
    D3DMULTISAMPLE_4_SAMPLES       =  4,
    D3DMULTISAMPLE_5_SAMPLES       =  5,
    D3DMULTISAMPLE_6_SAMPLES       =  6,
    D3DMULTISAMPLE_7_SAMPLES       =  7,
    D3DMULTISAMPLE_8_SAMPLES       =  8,
    D3DMULTISAMPLE_9_SAMPLES       =  9,
    D3DMULTISAMPLE_10_SAMPLES      = 10,
    D3DMULTISAMPLE_11_SAMPLES      = 11,
    D3DMULTISAMPLE_12_SAMPLES      = 12,
    D3DMULTISAMPLE_13_SAMPLES      = 13,
    D3DMULTISAMPLE_14_SAMPLES      = 14,
    D3DMULTISAMPLE_15_SAMPLES      = 15,
    D3DMULTISAMPLE_16_SAMPLES      = 16,

    D3DMULTISAMPLE_FORCE_DWORD     = 0x7fffffff
} D3DMULTISAMPLE_TYPE;

/* Formats
 * Most of these names have the following convention:
 *      A = Alpha
 *      R = Red
 *      G = Green
 *      B = Blue
 *      X = Unused Bits
 *      P = Palette
 *      L = Luminance
 *      U = dU coordinate for BumpMap
 *      V = dV coordinate for BumpMap
 *      S = Stencil
 *      D = Depth (e.g. Z or W buffer)
 *
 *      Further, the order of the pieces are from MSB first; hence
 *      D3DFMT_A8L8 indicates that the high byte of this two byte
 *      format is alpha.
 *
 *      D16 indicates:
 *           - An integer 16-bit value.
 *           - An app-lockable surface.
 *
 *      All Depth/Stencil formats except D3DFMT_D16_LOCKABLE indicate:
 *          - no particular bit ordering per pixel, and
 *          - are not app lockable, and
 *          - the driver is allowed to consume more than the indicated
 *            number of bits per Depth channel (but not Stencil channel).
 */
#ifndef MAKEFOURCC
    #define MAKEFOURCC(ch0, ch1, ch2, ch3)                              \
                ((DWORD)(BYTE)(ch0) | ((DWORD)(BYTE)(ch1) << 8) |       \
                ((DWORD)(BYTE)(ch2) << 16) | ((DWORD)(BYTE)(ch3) << 24 ))
#endif /* defined(MAKEFOURCC) */


typedef enum _D3DFORMAT
{
    D3DFMT_UNKNOWN              =  0,

    D3DFMT_R8G8B8               = 20,
    D3DFMT_A8R8G8B8             = 21,
    D3DFMT_X8R8G8B8             = 22,
    D3DFMT_R5G6B5               = 23,
    D3DFMT_X1R5G5B5             = 24,
    D3DFMT_A1R5G5B5             = 25,
    D3DFMT_A4R4G4B4             = 26,
    D3DFMT_R3G3B2               = 27,
    D3DFMT_A8                   = 28,
    D3DFMT_A8R3G3B2             = 29,
    D3DFMT_X4R4G4B4             = 30,
    D3DFMT_A2B10G10R10          = 31,
    D3DFMT_G16R16               = 34,

    D3DFMT_A8P8                 = 40,
    D3DFMT_P8                   = 41,

    D3DFMT_L8                   = 50,
    D3DFMT_A8L8                 = 51,
    D3DFMT_A4L4                 = 52,

    D3DFMT_V8U8                 = 60,
    D3DFMT_L6V5U5               = 61,
    D3DFMT_X8L8V8U8             = 62,
    D3DFMT_Q8W8V8U8             = 63,
    D3DFMT_V16U16               = 64,
    D3DFMT_W11V11U10            = 65,
    D3DFMT_A2W10V10U10          = 67,

    D3DFMT_UYVY                 = MAKEFOURCC('U', 'Y', 'V', 'Y'),
    D3DFMT_YUY2                 = MAKEFOURCC('Y', 'U', 'Y', '2'),
    D3DFMT_DXT1                 = MAKEFOURCC('D', 'X', 'T', '1'),
    D3DFMT_DXT2                 = MAKEFOURCC('D', 'X', 'T', '2'),
    D3DFMT_DXT3                 = MAKEFOURCC('D', 'X', 'T', '3'),
    D3DFMT_DXT4                 = MAKEFOURCC('D', 'X', 'T', '4'),
    D3DFMT_DXT5                 = MAKEFOURCC('D', 'X', 'T', '5'),

    D3DFMT_D16_LOCKABLE         = 70,
    D3DFMT_D32                  = 71,
    D3DFMT_D15S1                = 73,
    D3DFMT_D24S8                = 75,
    D3DFMT_D16                  = 80,
    D3DFMT_D24X8                = 77,
    D3DFMT_D24X4S4              = 79,


    D3DFMT_VERTEXDATA           =100,
    D3DFMT_INDEX16              =101,
    D3DFMT_INDEX32              =102,

    D3DFMT_FORCE_DWORD          =0x7fffffff
} D3DFORMAT;

/* Display Modes */
typedef struct _D3DDISPLAYMODE
{
    UINT            Width;
    UINT            Height;
    UINT            RefreshRate;
    D3DFORMAT       Format;
} D3DDISPLAYMODE;

/* Creation Parameters */
typedef struct _D3DDEVICE_CREATION_PARAMETERS
{
    UINT            AdapterOrdinal;
    D3DDEVTYPE      DeviceType;
    HWND            hFocusWindow;
    DWORD           BehaviorFlags;
} D3DDEVICE_CREATION_PARAMETERS;


/* SwapEffects */
typedef enum _D3DSWAPEFFECT
{
    D3DSWAPEFFECT_DISCARD           = 1,
    D3DSWAPEFFECT_FLIP              = 2,
    D3DSWAPEFFECT_COPY              = 3,
    D3DSWAPEFFECT_COPY_VSYNC        = 4,

    D3DSWAPEFFECT_FORCE_DWORD       = 0x7fffffff
} D3DSWAPEFFECT;

/* Pool types */
typedef enum _D3DPOOL {
    D3DPOOL_DEFAULT                 = 0,
    D3DPOOL_MANAGED                 = 1,
    D3DPOOL_SYSTEMMEM               = 2,
    D3DPOOL_SCRATCH                 = 3,

    D3DPOOL_FORCE_DWORD             = 0x7fffffff
} D3DPOOL;


/* RefreshRate pre-defines */
#define D3DPRESENT_RATE_DEFAULT         0x00000000
#define D3DPRESENT_RATE_UNLIMITED       0x7fffffff


/* Resize Optional Parameters */
typedef struct _D3DPRESENT_PARAMETERS_
{
    UINT                BackBufferWidth;
    UINT                BackBufferHeight;
    D3DFORMAT           BackBufferFormat;
    UINT                BackBufferCount;

    D3DMULTISAMPLE_TYPE MultiSampleType;

    D3DSWAPEFFECT       SwapEffect;
    HWND                hDeviceWindow;
    BOOL                Windowed;
    BOOL                EnableAutoDepthStencil;
    D3DFORMAT           AutoDepthStencilFormat;
    DWORD               Flags;

    /* Following elements must be zero for Windowed mode */
    UINT                FullScreen_RefreshRateInHz;
    UINT                FullScreen_PresentationInterval;

} D3DPRESENT_PARAMETERS;

// Values for D3DPRESENT_PARAMETERS.Flags

#define D3DPRESENTFLAG_LOCKABLE_BACKBUFFER  0x00000001


/* Gamma Ramp: Same as DX7 */

typedef struct _D3DGAMMARAMP
{
    WORD                red  [256];
    WORD                green[256];
    WORD                blue [256];
} D3DGAMMARAMP;

/* Back buffer types */
typedef enum _D3DBACKBUFFER_TYPE
{
    D3DBACKBUFFER_TYPE_MONO         = 0,
    D3DBACKBUFFER_TYPE_LEFT         = 1,
    D3DBACKBUFFER_TYPE_RIGHT        = 2,

    D3DBACKBUFFER_TYPE_FORCE_DWORD  = 0x7fffffff
} D3DBACKBUFFER_TYPE;


/* Types */
typedef enum _D3DRESOURCETYPE {
    D3DRTYPE_SURFACE                =  1,
    D3DRTYPE_VOLUME                 =  2,
    D3DRTYPE_TEXTURE                =  3,
    D3DRTYPE_VOLUMETEXTURE          =  4,
    D3DRTYPE_CUBETEXTURE            =  5,
    D3DRTYPE_VERTEXBUFFER           =  6,
    D3DRTYPE_INDEXBUFFER            =  7,


    D3DRTYPE_FORCE_DWORD            = 0x7fffffff
} D3DRESOURCETYPE;

/* Usages */
#define D3DUSAGE_RENDERTARGET       (0x00000001L)
#define D3DUSAGE_DEPTHSTENCIL       (0x00000002L)

/* Usages for Vertex/Index buffers */
#define D3DUSAGE_WRITEONLY          (0x00000008L)
#define D3DUSAGE_SOFTWAREPROCESSING (0x00000010L)
#define D3DUSAGE_DONOTCLIP          (0x00000020L)
#define D3DUSAGE_POINTS             (0x00000040L)
#define D3DUSAGE_RTPATCHES          (0x00000080L)
#define D3DUSAGE_NPATCHES           (0x00000100L)
#define D3DUSAGE_DYNAMIC            (0x00000200L)









/* CubeMap Face identifiers */
typedef enum _D3DCUBEMAP_FACES
{
    D3DCUBEMAP_FACE_POSITIVE_X     = 0,
    D3DCUBEMAP_FACE_NEGATIVE_X     = 1,
    D3DCUBEMAP_FACE_POSITIVE_Y     = 2,
    D3DCUBEMAP_FACE_NEGATIVE_Y     = 3,
    D3DCUBEMAP_FACE_POSITIVE_Z     = 4,
    D3DCUBEMAP_FACE_NEGATIVE_Z     = 5,

    D3DCUBEMAP_FACE_FORCE_DWORD    = 0x7fffffff
} D3DCUBEMAP_FACES;


/* Lock flags */

#define D3DLOCK_READONLY           0x00000010L
#define D3DLOCK_DISCARD             0x00002000L
#define D3DLOCK_NOOVERWRITE        0x00001000L
#define D3DLOCK_NOSYSLOCK          0x00000800L

#define D3DLOCK_NO_DIRTY_UPDATE     0x00008000L






/* Vertex Buffer Description */
typedef struct _D3DVERTEXBUFFER_DESC
{
    D3DFORMAT           Format;
    D3DRESOURCETYPE     Type;
    DWORD               Usage;
    D3DPOOL             Pool;
    UINT                Size;

    DWORD               FVF;

} D3DVERTEXBUFFER_DESC;

/* Index Buffer Description */
typedef struct _D3DINDEXBUFFER_DESC
{
    D3DFORMAT           Format;
    D3DRESOURCETYPE     Type;
    DWORD               Usage;
    D3DPOOL             Pool;
    UINT                Size;
} D3DINDEXBUFFER_DESC;


/* Surface Description */
typedef struct _D3DSURFACE_DESC
{
    D3DFORMAT           Format;
    D3DRESOURCETYPE     Type;
    DWORD               Usage;
    D3DPOOL             Pool;
    UINT                Size;

    D3DMULTISAMPLE_TYPE MultiSampleType;
    UINT                Width;
    UINT                Height;
} D3DSURFACE_DESC;

typedef struct _D3DVOLUME_DESC
{
    D3DFORMAT           Format;
    D3DRESOURCETYPE     Type;
    DWORD               Usage;
    D3DPOOL             Pool;
    UINT                Size;

    UINT                Width;
    UINT                Height;
    UINT                Depth;
} D3DVOLUME_DESC;

/* Structure for LockRect */
typedef struct _D3DLOCKED_RECT
{
    INT                 Pitch;
    void*               pBits;
} D3DLOCKED_RECT;

/* Structures for LockBox */
typedef struct _D3DBOX
{
    UINT                Left;
    UINT                Top;
    UINT                Right;
    UINT                Bottom;
    UINT                Front;
    UINT                Back;
} D3DBOX;

typedef struct _D3DLOCKED_BOX
{
    INT                 RowPitch;
    INT                 SlicePitch;
    void*               pBits;
} D3DLOCKED_BOX;

/* Structures for LockRange */
typedef struct _D3DRANGE
{
    UINT                Offset;
    UINT                Size;
} D3DRANGE;

/* Structures for high order primitives */
typedef struct _D3DRECTPATCH_INFO
{
    UINT                StartVertexOffsetWidth;
    UINT                StartVertexOffsetHeight;
    UINT                Width;
    UINT                Height;
    UINT                Stride;
    D3DBASISTYPE        Basis;
    D3DORDERTYPE        Order;
} D3DRECTPATCH_INFO;

typedef struct _D3DTRIPATCH_INFO
{
    UINT                StartVertexOffset;
    UINT                NumVertices;
    D3DBASISTYPE        Basis;
    D3DORDERTYPE        Order;
} D3DTRIPATCH_INFO;

/* Adapter Identifier */

#define MAX_DEVICE_IDENTIFIER_STRING        512
typedef struct _D3DADAPTER_IDENTIFIER8
{
    char            Driver[MAX_DEVICE_IDENTIFIER_STRING];
    char            Description[MAX_DEVICE_IDENTIFIER_STRING];

#ifdef _WIN32
    LARGE_INTEGER   DriverVersion;            /* Defined for 32 bit components */
#else
    DWORD           DriverVersionLowPart;     /* Defined for 16 bit driver components */
    DWORD           DriverVersionHighPart;
#endif

    DWORD           VendorId;
    DWORD           DeviceId;
    DWORD           SubSysId;
    DWORD           Revision;

    GUID            DeviceIdentifier;

    DWORD           WHQLLevel;

} D3DADAPTER_IDENTIFIER8;


/* Raster Status structure returned by GetRasterStatus */
typedef struct _D3DRASTER_STATUS
{
    BOOL            InVBlank;
    UINT            ScanLine;
} D3DRASTER_STATUS;



/* Debug monitor tokens (DEBUG only)
   Note that if D3DRS_DEBUGMONITORTOKEN is set, the call is treated as
   passing a token to the debug monitor.  For example, if, after passing
   D3DDMT_ENABLE/DISABLE to D3DRS_DEBUGMONITORTOKEN other token values
   are passed in, the enabled/disabled state of the debug
   monitor will still persist.
   The debug monitor defaults to enabled.
   Calling GetRenderState on D3DRS_DEBUGMONITORTOKEN is not of any use.
*/
typedef enum _D3DDEBUGMONITORTOKENS {
    D3DDMT_ENABLE            = 0,    // enable debug monitor
    D3DDMT_DISABLE           = 1,    // disable debug monitor
    D3DDMT_FORCE_DWORD     = 0x7fffffff,
} D3DDEBUGMONITORTOKENS;

// GetInfo IDs

#define D3DDEVINFOID_RESOURCEMANAGER    5           /* Used with D3DDEVINFO_RESOURCEMANAGER */
#define D3DDEVINFOID_VERTEXSTATS        6           /* Used with D3DDEVINFO_D3DVERTEXSTATS */

typedef struct _D3DRESOURCESTATS
{
// Data collected since last Present()
    BOOL    bThrashing;             /* indicates if thrashing */
    DWORD   ApproxBytesDownloaded;  /* Approximate number of bytes downloaded by resource manager */
    DWORD   NumEvicts;              /* number of objects evicted */
    DWORD   NumVidCreates;          /* number of objects created in video memory */
    DWORD   LastPri;                /* priority of last object evicted */
    DWORD   NumUsed;                /* number of objects set to the device */
    DWORD   NumUsedInVidMem;        /* number of objects set to the device, which are already in video memory */
// Persistent data
    DWORD   WorkingSet;             /* number of objects in video memory */
    DWORD   WorkingSetBytes;        /* number of bytes in video memory */
    DWORD   TotalManaged;           /* total number of managed objects */
    DWORD   TotalBytes;             /* total number of bytes of managed objects */
} D3DRESOURCESTATS;

#define D3DRTYPECOUNT (D3DRTYPE_INDEXBUFFER+1)

typedef struct _D3DDEVINFO_RESOURCEMANAGER
{
    D3DRESOURCESTATS    stats[D3DRTYPECOUNT];
} D3DDEVINFO_RESOURCEMANAGER, *LPD3DDEVINFO_RESOURCEMANAGER;

typedef struct _D3DDEVINFO_D3DVERTEXSTATS
{
    DWORD   NumRenderedTriangles;       /* total number of triangles that are not clipped in this frame */
    DWORD   NumExtraClippingTriangles;  /* Number of new triangles generated by clipping */
} D3DDEVINFO_D3DVERTEXSTATS, *LPD3DDEVINFO_D3DVERTEXSTATS;


typedef struct _D3DCAPS8
{
    /* Device Info */
    D3DDEVTYPE  DeviceType;
    UINT    AdapterOrdinal;

    /* Caps from DX7 Draw */
    DWORD   Caps;
    DWORD   Caps2;
    DWORD   Caps3;
    DWORD   PresentationIntervals;

    /* Cursor Caps */
    DWORD   CursorCaps;

    /* 3D Device Caps */
    DWORD   DevCaps;

    DWORD   PrimitiveMiscCaps;
    DWORD   RasterCaps;
    DWORD   ZCmpCaps;
    DWORD   SrcBlendCaps;
    DWORD   DestBlendCaps;
    DWORD   AlphaCmpCaps;
    DWORD   ShadeCaps;
    DWORD   TextureCaps;
    DWORD   TextureFilterCaps;          // D3DPTFILTERCAPS for IDirect3DTexture8's
    DWORD   CubeTextureFilterCaps;      // D3DPTFILTERCAPS for IDirect3DCubeTexture8's
    DWORD   VolumeTextureFilterCaps;    // D3DPTFILTERCAPS for IDirect3DVolumeTexture8's
    DWORD   TextureAddressCaps;         // D3DPTADDRESSCAPS for IDirect3DTexture8's
    DWORD   VolumeTextureAddressCaps;   // D3DPTADDRESSCAPS for IDirect3DVolumeTexture8's

    DWORD   LineCaps;                   // D3DLINECAPS

    DWORD   MaxTextureWidth, MaxTextureHeight;
    DWORD   MaxVolumeExtent;

    DWORD   MaxTextureRepeat;
    DWORD   MaxTextureAspectRatio;
    DWORD   MaxAnisotropy;
    float   MaxVertexW;

    float   GuardBandLeft;
    float   GuardBandTop;
    float   GuardBandRight;
    float   GuardBandBottom;

    float   ExtentsAdjust;
    DWORD   StencilCaps;

    DWORD   FVFCaps;
    DWORD   TextureOpCaps;
    DWORD   MaxTextureBlendStages;
    DWORD   MaxSimultaneousTextures;

    DWORD   VertexProcessingCaps;
    DWORD   MaxActiveLights;
    DWORD   MaxUserClipPlanes;
    DWORD   MaxVertexBlendMatrices;
    DWORD   MaxVertexBlendMatrixIndex;

    float   MaxPointSize;

    DWORD   MaxPrimitiveCount;          // max number of primitives per DrawPrimitive call
    DWORD   MaxVertexIndex;
    DWORD   MaxStreams;
    DWORD   MaxStreamStride;            // max stride for SetStreamSource

    DWORD   VertexShaderVersion;
    DWORD   MaxVertexShaderConst;       // number of vertex shader constant registers

    DWORD   PixelShaderVersion;
    float   MaxPixelShaderValue;        // max value of pixel shader arithmetic component

} D3DCAPS8;

//
// BIT DEFINES FOR D3DCAPS8 DWORD MEMBERS
//

//
// Caps
//
#define D3DCAPS_READ_SCANLINE           0x00020000L

//
// Caps2
//
#define D3DCAPS2_NO2DDURING3DSCENE      0x00000002L
#define D3DCAPS2_FULLSCREENGAMMA        0x00020000L
#define D3DCAPS2_CANRENDERWINDOWED      0x00080000L
#define D3DCAPS2_CANCALIBRATEGAMMA      0x00100000L
#define D3DCAPS2_RESERVED               0x02000000L
#define D3DCAPS2_CANMANAGERESOURCE      0x10000000L
#define D3DCAPS2_DYNAMICTEXTURES        0x20000000L

//
// Caps3
//
#define D3DCAPS3_RESERVED               0x8000001fL

// Indicates that the device can respect the ALPHABLENDENABLE render state
// when fullscreen while using the FLIP or DISCARD swap effect.
// COPY and COPYVSYNC swap effects work whether or not this flag is set.
#define D3DCAPS3_ALPHA_FULLSCREEN_FLIP_OR_DISCARD   0x00000020L

//
// PresentationIntervals
//
#define D3DPRESENT_INTERVAL_DEFAULT     0x00000000L
#define D3DPRESENT_INTERVAL_ONE         0x00000001L
#define D3DPRESENT_INTERVAL_TWO         0x00000002L
#define D3DPRESENT_INTERVAL_THREE       0x00000004L
#define D3DPRESENT_INTERVAL_FOUR        0x00000008L
#define D3DPRESENT_INTERVAL_IMMEDIATE   0x80000000L

//
// CursorCaps
//
// Driver supports HW color cursor in at least hi-res modes(height >=400)
#define D3DCURSORCAPS_COLOR             0x00000001L
// Driver supports HW cursor also in low-res modes(height < 400)
#define D3DCURSORCAPS_LOWRES            0x00000002L

//
// DevCaps
//
#define D3DDEVCAPS_EXECUTESYSTEMMEMORY  0x00000010L /* Device can use execute buffers from system memory */
#define D3DDEVCAPS_EXECUTEVIDEOMEMORY   0x00000020L /* Device can use execute buffers from video memory */
#define D3DDEVCAPS_TLVERTEXSYSTEMMEMORY 0x00000040L /* Device can use TL buffers from system memory */
#define D3DDEVCAPS_TLVERTEXVIDEOMEMORY  0x00000080L /* Device can use TL buffers from video memory */
#define D3DDEVCAPS_TEXTURESYSTEMMEMORY  0x00000100L /* Device can texture from system memory */
#define D3DDEVCAPS_TEXTUREVIDEOMEMORY   0x00000200L /* Device can texture from device memory */
#define D3DDEVCAPS_DRAWPRIMTLVERTEX     0x00000400L /* Device can draw TLVERTEX primitives */
#define D3DDEVCAPS_CANRENDERAFTERFLIP   0x00000800L /* Device can render without waiting for flip to complete */
#define D3DDEVCAPS_TEXTURENONLOCALVIDMEM 0x00001000L /* Device can texture from nonlocal video memory */
#define D3DDEVCAPS_DRAWPRIMITIVES2      0x00002000L /* Device can support DrawPrimitives2 */
#define D3DDEVCAPS_SEPARATETEXTUREMEMORIES 0x00004000L /* Device is texturing from separate memory pools */
#define D3DDEVCAPS_DRAWPRIMITIVES2EX    0x00008000L /* Device can support Extended DrawPrimitives2 i.e. DX7 compliant driver*/
#define D3DDEVCAPS_HWTRANSFORMANDLIGHT  0x00010000L /* Device can support transformation and lighting in hardware and DRAWPRIMITIVES2EX must be also */
#define D3DDEVCAPS_CANBLTSYSTONONLOCAL  0x00020000L /* Device supports a Tex Blt from system memory to non-local vidmem */
#define D3DDEVCAPS_HWRASTERIZATION      0x00080000L /* Device has HW acceleration for rasterization */
#define D3DDEVCAPS_PUREDEVICE           0x00100000L /* Device supports D3DCREATE_PUREDEVICE */
#define D3DDEVCAPS_QUINTICRTPATCHES     0x00200000L /* Device supports quintic Beziers and BSplines */
#define D3DDEVCAPS_RTPATCHES            0x00400000L /* Device supports Rect and Tri patches */
#define D3DDEVCAPS_RTPATCHHANDLEZERO    0x00800000L /* Indicates that RT Patches may be drawn efficiently using handle 0 */
#define D3DDEVCAPS_NPATCHES             0x01000000L /* Device supports N-Patches */

//
// PrimitiveMiscCaps
//
#define D3DPMISCCAPS_MASKZ              0x00000002L
#define D3DPMISCCAPS_LINEPATTERNREP     0x00000004L
#define D3DPMISCCAPS_CULLNONE           0x00000010L
#define D3DPMISCCAPS_CULLCW             0x00000020L
#define D3DPMISCCAPS_CULLCCW            0x00000040L
#define D3DPMISCCAPS_COLORWRITEENABLE   0x00000080L
#define D3DPMISCCAPS_CLIPPLANESCALEDPOINTS 0x00000100L /* Device correctly clips scaled points to clip planes */
#define D3DPMISCCAPS_CLIPTLVERTS        0x00000200L /* device will clip post-transformed vertex primitives */
#define D3DPMISCCAPS_TSSARGTEMP         0x00000400L /* device supports D3DTA_TEMP for temporary register */
#define D3DPMISCCAPS_BLENDOP            0x00000800L /* device supports D3DRS_BLENDOP */
#define D3DPMISCCAPS_NULLREFERENCE      0x00001000L /* Reference Device that doesnt render */

//
// LineCaps
//
#define D3DLINECAPS_TEXTURE             0x00000001L
#define D3DLINECAPS_ZTEST               0x00000002L
#define D3DLINECAPS_BLEND               0x00000004L
#define D3DLINECAPS_ALPHACMP            0x00000008L
#define D3DLINECAPS_FOG                 0x00000010L

//
// RasterCaps
//
#define D3DPRASTERCAPS_DITHER           0x00000001L
#define D3DPRASTERCAPS_PAT              0x00000008L
#define D3DPRASTERCAPS_ZTEST            0x00000010L
#define D3DPRASTERCAPS_FOGVERTEX        0x00000080L
#define D3DPRASTERCAPS_FOGTABLE         0x00000100L
#define D3DPRASTERCAPS_ANTIALIASEDGES   0x00001000L
#define D3DPRASTERCAPS_MIPMAPLODBIAS    0x00002000L
#define D3DPRASTERCAPS_ZBIAS            0x00004000L
#define D3DPRASTERCAPS_ZBUFFERLESSHSR   0x00008000L
#define D3DPRASTERCAPS_FOGRANGE         0x00010000L
#define D3DPRASTERCAPS_ANISOTROPY       0x00020000L
#define D3DPRASTERCAPS_WBUFFER          0x00040000L
#define D3DPRASTERCAPS_WFOG             0x00100000L
#define D3DPRASTERCAPS_ZFOG             0x00200000L
#define D3DPRASTERCAPS_COLORPERSPECTIVE 0x00400000L /* Device iterates colors perspective correct */
#define D3DPRASTERCAPS_STRETCHBLTMULTISAMPLE  0x00800000L

//
// ZCmpCaps, AlphaCmpCaps
//
#define D3DPCMPCAPS_NEVER               0x00000001L
#define D3DPCMPCAPS_LESS                0x00000002L
#define D3DPCMPCAPS_EQUAL               0x00000004L
#define D3DPCMPCAPS_LESSEQUAL           0x00000008L
#define D3DPCMPCAPS_GREATER             0x00000010L
#define D3DPCMPCAPS_NOTEQUAL            0x00000020L
#define D3DPCMPCAPS_GREATEREQUAL        0x00000040L
#define D3DPCMPCAPS_ALWAYS              0x00000080L

//
// SourceBlendCaps, DestBlendCaps
//
#define D3DPBLENDCAPS_ZERO              0x00000001L
#define D3DPBLENDCAPS_ONE               0x00000002L
#define D3DPBLENDCAPS_SRCCOLOR          0x00000004L
#define D3DPBLENDCAPS_INVSRCCOLOR       0x00000008L
#define D3DPBLENDCAPS_SRCALPHA          0x00000010L
#define D3DPBLENDCAPS_INVSRCALPHA       0x00000020L
#define D3DPBLENDCAPS_DESTALPHA         0x00000040L
#define D3DPBLENDCAPS_INVDESTALPHA      0x00000080L
#define D3DPBLENDCAPS_DESTCOLOR         0x00000100L
#define D3DPBLENDCAPS_INVDESTCOLOR      0x00000200L
#define D3DPBLENDCAPS_SRCALPHASAT       0x00000400L
#define D3DPBLENDCAPS_BOTHSRCALPHA      0x00000800L
#define D3DPBLENDCAPS_BOTHINVSRCALPHA   0x00001000L

//
// ShadeCaps
//
#define D3DPSHADECAPS_COLORGOURAUDRGB       0x00000008L
#define D3DPSHADECAPS_SPECULARGOURAUDRGB    0x00000200L
#define D3DPSHADECAPS_ALPHAGOURAUDBLEND     0x00004000L
#define D3DPSHADECAPS_FOGGOURAUD            0x00080000L

//
// TextureCaps
//
#define D3DPTEXTURECAPS_PERSPECTIVE         0x00000001L /* Perspective-correct texturing is supported */
#define D3DPTEXTURECAPS_POW2                0x00000002L /* Power-of-2 texture dimensions are required - applies to non-Cube/Volume textures only. */
#define D3DPTEXTURECAPS_ALPHA               0x00000004L /* Alpha in texture pixels is supported */
#define D3DPTEXTURECAPS_SQUAREONLY          0x00000020L /* Only square textures are supported */
#define D3DPTEXTURECAPS_TEXREPEATNOTSCALEDBYSIZE 0x00000040L /* Texture indices are not scaled by the texture size prior to interpolation */
#define D3DPTEXTURECAPS_ALPHAPALETTE        0x00000080L /* Device can draw alpha from texture palettes */
// Device can use non-POW2 textures if:
//  1) D3DTEXTURE_ADDRESS is set to CLAMP for this texture's stage
//  2) D3DRS_WRAP(N) is zero for this texture's coordinates
//  3) mip mapping is not enabled (use magnification filter only)
#define D3DPTEXTURECAPS_NONPOW2CONDITIONAL  0x00000100L
#define D3DPTEXTURECAPS_PROJECTED           0x00000400L /* Device can do D3DTTFF_PROJECTED */
#define D3DPTEXTURECAPS_CUBEMAP             0x00000800L /* Device can do cubemap textures */
#define D3DPTEXTURECAPS_VOLUMEMAP           0x00002000L /* Device can do volume textures */
#define D3DPTEXTURECAPS_MIPMAP              0x00004000L /* Device can do mipmapped textures */
#define D3DPTEXTURECAPS_MIPVOLUMEMAP        0x00008000L /* Device can do mipmapped volume textures */
#define D3DPTEXTURECAPS_MIPCUBEMAP          0x00010000L /* Device can do mipmapped cube maps */
#define D3DPTEXTURECAPS_CUBEMAP_POW2        0x00020000L /* Device requires that cubemaps be power-of-2 dimension */
#define D3DPTEXTURECAPS_VOLUMEMAP_POW2      0x00040000L /* Device requires that volume maps be power-of-2 dimension */

//
// TextureFilterCaps
//
#define D3DPTFILTERCAPS_MINFPOINT           0x00000100L /* Min Filter */
#define D3DPTFILTERCAPS_MINFLINEAR          0x00000200L
#define D3DPTFILTERCAPS_MINFANISOTROPIC     0x00000400L
#define D3DPTFILTERCAPS_MIPFPOINT           0x00010000L /* Mip Filter */
#define D3DPTFILTERCAPS_MIPFLINEAR          0x00020000L
#define D3DPTFILTERCAPS_MAGFPOINT           0x01000000L /* Mag Filter */
#define D3DPTFILTERCAPS_MAGFLINEAR          0x02000000L
#define D3DPTFILTERCAPS_MAGFANISOTROPIC     0x04000000L
#define D3DPTFILTERCAPS_MAGFAFLATCUBIC      0x08000000L
#define D3DPTFILTERCAPS_MAGFGAUSSIANCUBIC   0x10000000L

//
// TextureAddressCaps
//
#define D3DPTADDRESSCAPS_WRAP           0x00000001L
#define D3DPTADDRESSCAPS_MIRROR         0x00000002L
#define D3DPTADDRESSCAPS_CLAMP          0x00000004L
#define D3DPTADDRESSCAPS_BORDER         0x00000008L
#define D3DPTADDRESSCAPS_INDEPENDENTUV  0x00000010L
#define D3DPTADDRESSCAPS_MIRRORONCE     0x00000020L

//
// StencilCaps
//
#define D3DSTENCILCAPS_KEEP             0x00000001L
#define D3DSTENCILCAPS_ZERO             0x00000002L
#define D3DSTENCILCAPS_REPLACE          0x00000004L
#define D3DSTENCILCAPS_INCRSAT          0x00000008L
#define D3DSTENCILCAPS_DECRSAT          0x00000010L
#define D3DSTENCILCAPS_INVERT           0x00000020L
#define D3DSTENCILCAPS_INCR             0x00000040L
#define D3DSTENCILCAPS_DECR             0x00000080L

//
// TextureOpCaps
//
#define D3DTEXOPCAPS_DISABLE                    0x00000001L
#define D3DTEXOPCAPS_SELECTARG1                 0x00000002L
#define D3DTEXOPCAPS_SELECTARG2                 0x00000004L
#define D3DTEXOPCAPS_MODULATE                   0x00000008L
#define D3DTEXOPCAPS_MODULATE2X                 0x00000010L
#define D3DTEXOPCAPS_MODULATE4X                 0x00000020L
#define D3DTEXOPCAPS_ADD                        0x00000040L
#define D3DTEXOPCAPS_ADDSIGNED                  0x00000080L
#define D3DTEXOPCAPS_ADDSIGNED2X                0x00000100L
#define D3DTEXOPCAPS_SUBTRACT                   0x00000200L
#define D3DTEXOPCAPS_ADDSMOOTH                  0x00000400L
#define D3DTEXOPCAPS_BLENDDIFFUSEALPHA          0x00000800L
#define D3DTEXOPCAPS_BLENDTEXTUREALPHA          0x00001000L
#define D3DTEXOPCAPS_BLENDFACTORALPHA           0x00002000L
#define D3DTEXOPCAPS_BLENDTEXTUREALPHAPM        0x00004000L
#define D3DTEXOPCAPS_BLENDCURRENTALPHA          0x00008000L
#define D3DTEXOPCAPS_PREMODULATE                0x00010000L
#define D3DTEXOPCAPS_MODULATEALPHA_ADDCOLOR     0x00020000L
#define D3DTEXOPCAPS_MODULATECOLOR_ADDALPHA     0x00040000L
#define D3DTEXOPCAPS_MODULATEINVALPHA_ADDCOLOR  0x00080000L
#define D3DTEXOPCAPS_MODULATEINVCOLOR_ADDALPHA  0x00100000L
#define D3DTEXOPCAPS_BUMPENVMAP                 0x00200000L
#define D3DTEXOPCAPS_BUMPENVMAPLUMINANCE        0x00400000L
#define D3DTEXOPCAPS_DOTPRODUCT3                0x00800000L
#define D3DTEXOPCAPS_MULTIPLYADD                0x01000000L
#define D3DTEXOPCAPS_LERP                       0x02000000L

//
// FVFCaps
//
#define D3DFVFCAPS_TEXCOORDCOUNTMASK    0x0000ffffL /* mask for texture coordinate count field */
#define D3DFVFCAPS_DONOTSTRIPELEMENTS   0x00080000L /* Device prefers that vertex elements not be stripped */
#define D3DFVFCAPS_PSIZE                0x00100000L /* Device can receive point size */

//
// VertexProcessingCaps
//
#define D3DVTXPCAPS_TEXGEN              0x00000001L /* device can do texgen */
#define D3DVTXPCAPS_MATERIALSOURCE7     0x00000002L /* device can do DX7-level colormaterialsource ops */
#define D3DVTXPCAPS_DIRECTIONALLIGHTS   0x00000008L /* device can do directional lights */
#define D3DVTXPCAPS_POSITIONALLIGHTS    0x00000010L /* device can do positional lights (includes point and spot) */
#define D3DVTXPCAPS_LOCALVIEWER         0x00000020L /* device can do local viewer */
#define D3DVTXPCAPS_TWEENING            0x00000040L /* device can do vertex tweening */
#define D3DVTXPCAPS_NO_VSDT_UBYTE4      0x00000080L /* device does not support D3DVSDT_UBYTE4 */

struct IDirect3D8;
struct IDirect3D8_vtbl;
struct IDirect3DDevice8;
struct IDirect3DDevice8_vtbl;

struct IDirect3DResource8;
struct IDirect3DResource8_vtbl;
struct IDirect3DBaseTexture8;
struct IDirect3DBaseTexture8_vtbl;
struct IDirect3DTexture8;
struct IDirect3DTexture8_vtbl;
struct IDirect3DVolumeTexture8;
struct IDirect3DVolumeTexture8_vtbl;
struct IDirect3DCubeTexture8;
struct IDirect3DCubeTexture8_vtbl;

struct IDirect3DVertexBuffer8;
struct IDirect3DVertexBuffer8_vtbl;
struct IDirect3DIndexBuffer8;
struct IDirect3DIndexBuffer8_vtbl;

struct IDirect3DSurface8;
struct IDirect3DSurface8_vtbl;
struct IDirect3DVolume8;
struct IDirect3DVolume8_vtbl;

struct IDirect3DSwapChain8;
struct IDirect3DSwapChain8_vtbl;

struct IDirect3D8 { IDirect3D8_vtbl* vtptr; };
struct IDirect3D8_vtbl

{
    /*** IUnknown methods ***/
    DWORD (__stdcall *QueryInterface) (void* this, void* riid, void** ppvObj);
    ULONG (__stdcall *AddRef) (void* this);
    ULONG (__stdcall *Release) (void* this);

    /*** IDirect3D8 methods ***/
    DWORD (__stdcall *RegisterSoftwareDevice) (void* this, void* pInitializeFunction);
    UINT (__stdcall *GetAdapterCount) (void* this);
    DWORD (__stdcall *GetAdapterIdentifier) (void* this, UINT Adapter,DWORD Flags,D3DADAPTER_IDENTIFIER8* pIdentifier);
    UINT (__stdcall *GetAdapterModeCount) (void* this, UINT Adapter);
    DWORD (__stdcall *EnumAdapterModes) (void* this, UINT Adapter,UINT Mode,D3DDISPLAYMODE* pMode);
    DWORD (__stdcall *GetAdapterDisplayMode) (void* this, UINT Adapter,D3DDISPLAYMODE* pMode);
    DWORD (__stdcall *CheckDeviceType) (void* this, UINT Adapter,D3DDEVTYPE CheckType,D3DFORMAT DisplayFormat,D3DFORMAT BackBufferFormat,BOOL Windowed);
    DWORD (__stdcall *CheckDeviceFormat) (void* this, UINT Adapter,D3DDEVTYPE DeviceType,D3DFORMAT AdapterFormat,DWORD Usage,D3DRESOURCETYPE RType,D3DFORMAT CheckFormat);
    DWORD (__stdcall *CheckDeviceMultiSampleType) (void* this, UINT Adapter,D3DDEVTYPE DeviceType,D3DFORMAT SurfaceFormat,BOOL Windowed,D3DMULTISAMPLE_TYPE MultiSampleType);
    DWORD (__stdcall *CheckDepthStencilMatch) (void* this, UINT Adapter,D3DDEVTYPE DeviceType,D3DFORMAT AdapterFormat,D3DFORMAT RenderTargetFormat,D3DFORMAT DepthStencilFormat);
    DWORD (__stdcall *GetDeviceCaps) (void* this, UINT Adapter,D3DDEVTYPE DeviceType,D3DCAPS8* pCaps);
    HMONITOR (__stdcall *GetAdapterMonitor) (void* this, UINT Adapter);
    DWORD (__stdcall *CreateDevice) (void* this, UINT Adapter,D3DDEVTYPE DeviceType,HWND hFocusWindow,DWORD BehaviorFlags,D3DPRESENT_PARAMETERS* pPresentationParameters,IDirect3DDevice8** ppReturnedDeviceInterface);
};



#undef INTERFACE
#define INTERFACE IDirect3DDevice8

struct IDirect3DDevice8 { IDirect3DDevice8_vtbl* vtptr; };
struct IDirect3DDevice8_vtbl

{
    /*** IUnknown methods ***/
    DWORD (__stdcall *QueryInterface) (void* this, void* riid, void** ppvObj);
    ULONG (__stdcall *AddRef) (void* this);
    ULONG (__stdcall *Release) (void* this);

    /*** IDirect3DDevice8 methods ***/
    DWORD (__stdcall *TestCooperativeLevel) (void* this);
    UINT (__stdcall *GetAvailableTextureMem) (void* this);
    DWORD (__stdcall *ResourceManagerDiscardBytes) (void* this, DWORD Bytes);
    DWORD (__stdcall *GetDirect3D) (void* this, IDirect3D8** ppD3D8);
    DWORD (__stdcall *GetDeviceCaps) (void* this, D3DCAPS8* pCaps);
    DWORD (__stdcall *GetDisplayMode) (void* this, D3DDISPLAYMODE* pMode);
    DWORD (__stdcall *GetCreationParameters) (void* this, D3DDEVICE_CREATION_PARAMETERS *pParameters);
    DWORD (__stdcall *SetCursorProperties) (void* this, UINT XHotSpot,UINT YHotSpot,IDirect3DSurface8* pCursorBitmap);
    void (__stdcall *SetCursorPosition) (void* this, int X,int Y,DWORD Flags);
    BOOL (__stdcall *ShowCursor) (void* this, BOOL bShow);
    DWORD (__stdcall *CreateAdditionalSwapChain) (void* this, D3DPRESENT_PARAMETERS* pPresentationParameters,IDirect3DSwapChain8** pSwapChain);
    DWORD (__stdcall *Reset) (void* this, D3DPRESENT_PARAMETERS* pPresentationParameters);
    DWORD (__stdcall *Present) (void* this,  RECT* pSourceRect, RECT* pDestRect,HWND hDestWindowOverride, RGNDATA* pDirtyRegion);
    DWORD (__stdcall *GetBackBuffer) (void* this, UINT BackBuffer,D3DBACKBUFFER_TYPE Type,IDirect3DSurface8** ppBackBuffer);
    DWORD (__stdcall *GetRasterStatus) (void* this, D3DRASTER_STATUS* pRasterStatus);
    void (__stdcall *SetGammaRamp) (void* this, DWORD Flags, D3DGAMMARAMP* pRamp);
    void (__stdcall *GetGammaRamp) (void* this, D3DGAMMARAMP* pRamp);
    DWORD (__stdcall *CreateTexture) (void* this, UINT Width,UINT Height,UINT Levels,DWORD Usage,D3DFORMAT Format,D3DPOOL Pool,IDirect3DTexture8** ppTexture);
    DWORD (__stdcall *CreateVolumeTexture) (void* this, UINT Width,UINT Height,UINT Depth,UINT Levels,DWORD Usage,D3DFORMAT Format,D3DPOOL Pool,IDirect3DVolumeTexture8** ppVolumeTexture);
    DWORD (__stdcall *CreateCubeTexture) (void* this, UINT EdgeLength,UINT Levels,DWORD Usage,D3DFORMAT Format,D3DPOOL Pool,IDirect3DCubeTexture8** ppCubeTexture);
    DWORD (__stdcall *CreateVertexBuffer) (void* this, UINT Length,DWORD Usage,DWORD FVF,D3DPOOL Pool,IDirect3DVertexBuffer8** ppVertexBuffer);
    DWORD (__stdcall *CreateIndexBuffer) (void* this, UINT Length,DWORD Usage,D3DFORMAT Format,D3DPOOL Pool,IDirect3DIndexBuffer8** ppIndexBuffer);
    DWORD (__stdcall *CreateRenderTarget) (void* this, UINT Width,UINT Height,D3DFORMAT Format,D3DMULTISAMPLE_TYPE MultiSample,BOOL Lockable,IDirect3DSurface8** ppSurface);
    DWORD (__stdcall *CreateDepthStencilSurface) (void* this, UINT Width,UINT Height,D3DFORMAT Format,D3DMULTISAMPLE_TYPE MultiSample,IDirect3DSurface8** ppSurface);
    DWORD (__stdcall *CreateImageSurface) (void* this, UINT Width,UINT Height,D3DFORMAT Format,IDirect3DSurface8** ppSurface);
    DWORD (__stdcall *CopyRects) (void* this, IDirect3DSurface8* pSourceSurface, RECT* pSourceRectsArray,UINT cRects,IDirect3DSurface8* pDestinationSurface, POINT* pDestPointsArray);
    DWORD (__stdcall *UpdateTexture) (void* this, IDirect3DBaseTexture8* pSourceTexture,IDirect3DBaseTexture8* pDestinationTexture);
    DWORD (__stdcall *GetFrontBuffer) (void* this, IDirect3DSurface8* pDestSurface);
    DWORD (__stdcall *SetRenderTarget) (void* this, IDirect3DSurface8* pRenderTarget,IDirect3DSurface8* pNewZStencil);
    DWORD (__stdcall *GetRenderTarget) (void* this, IDirect3DSurface8** ppRenderTarget);
    DWORD (__stdcall *GetDepthStencilSurface) (void* this, IDirect3DSurface8** ppZStencilSurface);
    DWORD (__stdcall *BeginScene) (void* this);
    DWORD (__stdcall *EndScene) (void* this);
    DWORD (__stdcall *Clear) (void* this, DWORD Count, D3DRECT* pRects,DWORD Flags,D3DCOLOR Color,float Z,DWORD Stencil);
    DWORD (__stdcall *SetTransform) (void* this, D3DTRANSFORMSTATETYPE State, D3DMATRIX* pMatrix);
    DWORD (__stdcall *GetTransform) (void* this, D3DTRANSFORMSTATETYPE State,D3DMATRIX* pMatrix);
    DWORD (__stdcall *MultiplyTransform) (void* this, D3DTRANSFORMSTATETYPE, D3DMATRIX*);
    DWORD (__stdcall *SetViewport) (void* this,  D3DVIEWPORT8* pViewport);
    DWORD (__stdcall *GetViewport) (void* this, D3DVIEWPORT8* pViewport);
    DWORD (__stdcall *SetMaterial) (void* this,  D3DMATERIAL8* pMaterial);
    DWORD (__stdcall *GetMaterial) (void* this, D3DMATERIAL8* pMaterial);
    DWORD (__stdcall *SetLight) (void* this, DWORD Index, D3DLIGHT8*);
    DWORD (__stdcall *GetLight) (void* this, DWORD Index,D3DLIGHT8*);
    DWORD (__stdcall *LightEnable) (void* this, DWORD Index,BOOL Enable);
    DWORD (__stdcall *GetLightEnable) (void* this, DWORD Index,BOOL* pEnable);
    DWORD (__stdcall *SetClipPlane) (void* this, DWORD Index, float* pPlane);
    DWORD (__stdcall *GetClipPlane) (void* this, DWORD Index,float* pPlane);
    DWORD (__stdcall *SetRenderState) (void* this, D3DRENDERSTATETYPE State,DWORD Value);
    DWORD (__stdcall *GetRenderState) (void* this, D3DRENDERSTATETYPE State,DWORD* pValue);
    DWORD (__stdcall *BeginStateBlock) (void* this);
    DWORD (__stdcall *EndStateBlock) (void* this, DWORD* pToken);
    DWORD (__stdcall *ApplyStateBlock) (void* this, DWORD Token);
    DWORD (__stdcall *CaptureStateBlock) (void* this, DWORD Token);
    DWORD (__stdcall *DeleteStateBlock) (void* this, DWORD Token);
    DWORD (__stdcall *CreateStateBlock) (void* this, D3DSTATEBLOCKTYPE Type,DWORD* pToken);
    DWORD (__stdcall *SetClipStatus) (void* this,  D3DCLIPSTATUS8* pClipStatus);
    DWORD (__stdcall *GetClipStatus) (void* this, D3DCLIPSTATUS8* pClipStatus);
    DWORD (__stdcall *GetTexture) (void* this, DWORD Stage,IDirect3DBaseTexture8** ppTexture);
    DWORD (__stdcall *SetTexture) (void* this, DWORD Stage,IDirect3DBaseTexture8* pTexture);
    DWORD (__stdcall *GetTextureStageState) (void* this, DWORD Stage,D3DTEXTURESTAGESTATETYPE Type,DWORD* pValue);
    DWORD (__stdcall *SetTextureStageState) (void* this, DWORD Stage,D3DTEXTURESTAGESTATETYPE Type,DWORD Value);
    DWORD (__stdcall *ValidateDevice) (void* this, DWORD* pNumPasses);
    DWORD (__stdcall *GetInfo) (void* this, DWORD DevInfoID,void* pDevInfoStruct,DWORD DevInfoStructSize);
    DWORD (__stdcall *SetPaletteEntries) (void* this, UINT PaletteNumber, PALETTEENTRY* pEntries);
    DWORD (__stdcall *GetPaletteEntries) (void* this, UINT PaletteNumber,PALETTEENTRY* pEntries);
    DWORD (__stdcall *SetCurrentTexturePalette) (void* this, UINT PaletteNumber);
    DWORD (__stdcall *GetCurrentTexturePalette) (void* this, UINT *PaletteNumber);
    DWORD (__stdcall *DrawPrimitive) (void* this, D3DPRIMITIVETYPE PrimitiveType,UINT StartVertex,UINT PrimitiveCount);
    DWORD (__stdcall *DrawIndexedPrimitive) (void* this, D3DPRIMITIVETYPE,UINT minIndex,UINT NumVertices,UINT startIndex,UINT primCount);
    DWORD (__stdcall *DrawPrimitiveUP) (void* this, D3DPRIMITIVETYPE PrimitiveType,UINT PrimitiveCount, void* pVertexStreamZeroData,UINT VertexStreamZeroStride);
    DWORD (__stdcall *DrawIndexedPrimitiveUP) (void* this, D3DPRIMITIVETYPE PrimitiveType,UINT MinVertexIndex,UINT NumVertexIndices,UINT PrimitiveCount, void* pIndexData,D3DFORMAT IndexDataFormat, void* pVertexStreamZeroData,UINT VertexStreamZeroStride);
    DWORD (__stdcall *ProcessVertices) (void* this, UINT SrcStartIndex,UINT DestIndex,UINT VertexCount,IDirect3DVertexBuffer8* pDestBuffer,DWORD Flags);
    DWORD (__stdcall *CreateVertexShader) (void* this,  DWORD* pDeclaration, DWORD* pFunction,DWORD* pHandle,DWORD Usage);
    DWORD (__stdcall *SetVertexShader) (void* this, DWORD Handle);
    DWORD (__stdcall *GetVertexShader) (void* this, DWORD* pHandle);
    DWORD (__stdcall *DeleteVertexShader) (void* this, DWORD Handle);
    DWORD (__stdcall *SetVertexShaderConstant) (void* this, DWORD Register, void* pConstantData,DWORD ConstantCount);
    DWORD (__stdcall *GetVertexShaderConstant) (void* this, DWORD Register,void* pConstantData,DWORD ConstantCount);
    DWORD (__stdcall *GetVertexShaderDeclaration) (void* this, DWORD Handle,void* pData,DWORD* pSizeOfData);
    DWORD (__stdcall *GetVertexShaderFunction) (void* this, DWORD Handle,void* pData,DWORD* pSizeOfData);
    DWORD (__stdcall *SetStreamSource) (void* this, UINT StreamNumber,IDirect3DVertexBuffer8* pStreamData,UINT Stride);
    DWORD (__stdcall *GetStreamSource) (void* this, UINT StreamNumber,IDirect3DVertexBuffer8** ppStreamData,UINT* pStride);
    DWORD (__stdcall *SetIndices) (void* this, IDirect3DIndexBuffer8* pIndexData,UINT BaseVertexIndex);
    DWORD (__stdcall *GetIndices) (void* this, IDirect3DIndexBuffer8** ppIndexData,UINT* pBaseVertexIndex);
    DWORD (__stdcall *CreatePixelShader) (void* this,  DWORD* pFunction,DWORD* pHandle);
    DWORD (__stdcall *SetPixelShader) (void* this, DWORD Handle);
    DWORD (__stdcall *GetPixelShader) (void* this, DWORD* pHandle);
    DWORD (__stdcall *DeletePixelShader) (void* this, DWORD Handle);
    DWORD (__stdcall *SetPixelShaderConstant) (void* this, DWORD Register, void* pConstantData,DWORD ConstantCount);
    DWORD (__stdcall *GetPixelShaderConstant) (void* this, DWORD Register,void* pConstantData,DWORD ConstantCount);
    DWORD (__stdcall *GetPixelShaderFunction) (void* this, DWORD Handle,void* pData,DWORD* pSizeOfData);
    DWORD (__stdcall *DrawRectPatch) (void* this, UINT Handle, float* pNumSegs, D3DRECTPATCH_INFO* pRectPatchInfo);
    DWORD (__stdcall *DrawTriPatch) (void* this, UINT Handle, float* pNumSegs, D3DTRIPATCH_INFO* pTriPatchInfo);
    DWORD (__stdcall *DeletePatch) (void* this, UINT Handle);
};


struct IDirect3DSwapChain8 { IDirect3DSwapChain8_vtbl* vtptr; };
struct IDirect3DSwapChain8_vtbl

{
    /*** IUnknown methods ***/
    DWORD (__stdcall *QueryInterface) (void* this, void* riid, void** ppvObj);
    ULONG (__stdcall *AddRef) (void* this);
    ULONG (__stdcall *Release) (void* this);

    /*** IDirect3DSwapChain8 methods ***/
    DWORD (__stdcall *Present) (void* this,  RECT* pSourceRect, RECT* pDestRect,HWND hDestWindowOverride, RGNDATA* pDirtyRegion);
    DWORD (__stdcall *GetBackBuffer) (void* this, UINT BackBuffer,D3DBACKBUFFER_TYPE Type,IDirect3DSurface8** ppBackBuffer);
};


struct IDirect3DResource8 { IDirect3DResource8_vtbl* vtptr; };
struct IDirect3DResource8_vtbl

{
    /*** IUnknown methods ***/
    DWORD (__stdcall *QueryInterface) (void* this, void* riid, void** ppvObj);
    ULONG (__stdcall *AddRef) (void* this);
    ULONG (__stdcall *Release) (void* this);

    /*** IDirect3DResource8 methods ***/
    DWORD (__stdcall *GetDevice) (void* this, IDirect3DDevice8** ppDevice);
    DWORD (__stdcall *SetPrivateData) (void* this, void* refguid, void* pData,DWORD SizeOfData,DWORD Flags);
    DWORD (__stdcall *GetPrivateData) (void* this, void* refguid,void* pData,DWORD* pSizeOfData);
    DWORD (__stdcall *FreePrivateData) (void* this, void* refguid);
    DWORD (__stdcall *SetPriority) (void* this, DWORD PriorityNew);
    DWORD (__stdcall *GetPriority) (void* this);
    void (__stdcall *PreLoad) (void* this);
    D3DRESOURCETYPE (__stdcall *GetType) (void* this);
};


struct IDirect3DBaseTexture8 { IDirect3DBaseTexture8_vtbl* vtptr; };
struct IDirect3DBaseTexture8_vtbl

{
    /*** IUnknown methods ***/
    DWORD (__stdcall *QueryInterface) (void* this, void* riid, void** ppvObj);
    ULONG (__stdcall *AddRef) (void* this);
    ULONG (__stdcall *Release) (void* this);

    /*** IDirect3DResource8 methods ***/
    DWORD (__stdcall *GetDevice) (void* this, IDirect3DDevice8** ppDevice);
    DWORD (__stdcall *SetPrivateData) (void* this, void* refguid, void* pData,DWORD SizeOfData,DWORD Flags);
    DWORD (__stdcall *GetPrivateData) (void* this, void* refguid,void* pData,DWORD* pSizeOfData);
    DWORD (__stdcall *FreePrivateData) (void* this, void* refguid);
    DWORD (__stdcall *SetPriority) (void* this, DWORD PriorityNew);
    DWORD (__stdcall *GetPriority) (void* this);
    void (__stdcall *PreLoad) (void* this);
    D3DRESOURCETYPE (__stdcall *GetType) (void* this);
    DWORD (__stdcall *SetLOD) (void* this, DWORD LODNew);
    DWORD (__stdcall *GetLOD) (void* this);
    DWORD (__stdcall *GetLevelCount) (void* this);
};

typedef struct IDirect3DBaseTexture8 *LPDIRECT3DBASETEXTURE8, *PDIRECT3DBASETEXTURE8;


struct IDirect3DTexture8 { IDirect3DTexture8_vtbl* vtptr; };
struct IDirect3DTexture8_vtbl

{
    /*** IUnknown methods ***/
    DWORD (__stdcall *QueryInterface) (void* this, void* riid, void** ppvObj);
    ULONG (__stdcall *AddRef) (void* this);
    ULONG (__stdcall *Release) (void* this);

    /*** IDirect3DBaseTexture8 methods ***/
    DWORD (__stdcall *GetDevice) (void* this, IDirect3DDevice8** ppDevice);
    DWORD (__stdcall *SetPrivateData) (void* this, void* refguid, void* pData,DWORD SizeOfData,DWORD Flags);
    DWORD (__stdcall *GetPrivateData) (void* this, void* refguid,void* pData,DWORD* pSizeOfData);
    DWORD (__stdcall *FreePrivateData) (void* this, void* refguid);
    DWORD (__stdcall *SetPriority) (void* this, DWORD PriorityNew);
    DWORD (__stdcall *GetPriority) (void* this);
    void (__stdcall *PreLoad) (void* this);
    D3DRESOURCETYPE (__stdcall *GetType) (void* this);
    DWORD (__stdcall *SetLOD) (void* this, DWORD LODNew);
    DWORD (__stdcall *GetLOD) (void* this);
    DWORD (__stdcall *GetLevelCount) (void* this);
    DWORD (__stdcall *GetLevelDesc) (void* this, UINT Level,D3DSURFACE_DESC *pDesc);
    DWORD (__stdcall *GetSurfaceLevel) (void* this, UINT Level,IDirect3DSurface8** ppSurfaceLevel);
    DWORD (__stdcall *LockRect) (void* this, UINT Level,D3DLOCKED_RECT* pLockedRect, RECT* pRect,DWORD Flags);
    DWORD (__stdcall *UnlockRect) (void* this, UINT Level);
    DWORD (__stdcall *AddDirtyRect) (void* this,  RECT* pDirtyRect);
};

struct IDirect3DVolumeTexture8 { IDirect3DVolumeTexture8_vtbl* vtptr; };
struct IDirect3DVolumeTexture8_vtbl

{
    /*** IUnknown methods ***/
    DWORD (__stdcall *QueryInterface) (void* this, void* riid, void** ppvObj);
    ULONG (__stdcall *AddRef) (void* this);
    ULONG (__stdcall *Release) (void* this);

    /*** IDirect3DBaseTexture8 methods ***/
    DWORD (__stdcall *GetDevice) (void* this, IDirect3DDevice8** ppDevice);
    DWORD (__stdcall *SetPrivateData) (void* this, void* refguid, void* pData,DWORD SizeOfData,DWORD Flags);
    DWORD (__stdcall *GetPrivateData) (void* this, void* refguid,void* pData,DWORD* pSizeOfData);
    DWORD (__stdcall *FreePrivateData) (void* this, void* refguid);
    DWORD (__stdcall *SetPriority) (void* this, DWORD PriorityNew);
    DWORD (__stdcall *GetPriority) (void* this);
    void (__stdcall *PreLoad) (void* this);
    D3DRESOURCETYPE (__stdcall *GetType) (void* this);
    DWORD (__stdcall *SetLOD) (void* this, DWORD LODNew);
    DWORD (__stdcall *GetLOD) (void* this);
    DWORD (__stdcall *GetLevelCount) (void* this);
    DWORD (__stdcall *GetLevelDesc) (void* this, UINT Level,D3DVOLUME_DESC *pDesc);
    DWORD (__stdcall *GetVolumeLevel) (void* this, UINT Level,IDirect3DVolume8** ppVolumeLevel);
    DWORD (__stdcall *LockBox) (void* this, UINT Level,D3DLOCKED_BOX* pLockedVolume, D3DBOX* pBox,DWORD Flags);
    DWORD (__stdcall *UnlockBox) (void* this, UINT Level);
    DWORD (__stdcall *AddDirtyBox) (void* this,  D3DBOX* pDirtyBox);
};


struct IDirect3DCubeTexture8 { IDirect3DCubeTexture8_vtbl* vtptr; };
struct IDirect3DCubeTexture8_vtbl

{
    /*** IUnknown methods ***/
    DWORD (__stdcall *QueryInterface) (void* this, void* riid, void** ppvObj);
    ULONG (__stdcall *AddRef) (void* this);
    ULONG (__stdcall *Release) (void* this);

    /*** IDirect3DBaseTexture8 methods ***/
    DWORD (__stdcall *GetDevice) (void* this, IDirect3DDevice8** ppDevice);
    DWORD (__stdcall *SetPrivateData) (void* this, void* refguid, void* pData,DWORD SizeOfData,DWORD Flags);
    DWORD (__stdcall *GetPrivateData) (void* this, void* refguid,void* pData,DWORD* pSizeOfData);
    DWORD (__stdcall *FreePrivateData) (void* this, void* refguid);
    DWORD (__stdcall *SetPriority) (void* this, DWORD PriorityNew);
    DWORD (__stdcall *GetPriority) (void* this);
    void (__stdcall *PreLoad) (void* this);
    D3DRESOURCETYPE (__stdcall *GetType) (void* this);
    DWORD (__stdcall *SetLOD) (void* this, DWORD LODNew);
    DWORD (__stdcall *GetLOD) (void* this);
    DWORD (__stdcall *GetLevelCount) (void* this);
    DWORD (__stdcall *GetLevelDesc) (void* this, UINT Level,D3DSURFACE_DESC *pDesc);
    DWORD (__stdcall *GetCubeMapSurface) (void* this, D3DCUBEMAP_FACES FaceType,UINT Level,IDirect3DSurface8** ppCubeMapSurface);
    DWORD (__stdcall *LockRect) (void* this, D3DCUBEMAP_FACES FaceType,UINT Level,D3DLOCKED_RECT* pLockedRect, RECT* pRect,DWORD Flags);
    DWORD (__stdcall *UnlockRect) (void* this, D3DCUBEMAP_FACES FaceType,UINT Level);
    DWORD (__stdcall *AddDirtyRect) (void* this, D3DCUBEMAP_FACES FaceType, RECT* pDirtyRect);
};

struct IDirect3DVertexBuffer8 { IDirect3DVertexBuffer8_vtbl* vtptr; };
struct IDirect3DVertexBuffer8_vtbl

{
    /*** IUnknown methods ***/
    DWORD (__stdcall *QueryInterface) (void* this, void* riid, void** ppvObj);
    ULONG (__stdcall *AddRef) (void* this);
    ULONG (__stdcall *Release) (void* this);

    /*** IDirect3DResource8 methods ***/
    DWORD (__stdcall *GetDevice) (void* this, IDirect3DDevice8** ppDevice);
    DWORD (__stdcall *SetPrivateData) (void* this, void* refguid, void* pData,DWORD SizeOfData,DWORD Flags);
    DWORD (__stdcall *GetPrivateData) (void* this, void* refguid,void* pData,DWORD* pSizeOfData);
    DWORD (__stdcall *FreePrivateData) (void* this, void* refguid);
    DWORD (__stdcall *SetPriority) (void* this, DWORD PriorityNew);
    DWORD (__stdcall *GetPriority) (void* this);
    void (__stdcall *PreLoad) (void* this);
    D3DRESOURCETYPE (__stdcall *GetType) (void* this);
    DWORD (__stdcall *Lock) (void* this, UINT OffsetToLock,UINT SizeToLock,BYTE** ppbData,DWORD Flags);
    DWORD (__stdcall *Unlock) (void* this);
    DWORD (__stdcall *GetDesc) (void* this, D3DVERTEXBUFFER_DESC *pDesc);
};


struct IDirect3DIndexBuffer8 { IDirect3DIndexBuffer8_vtbl* vtptr; };
struct IDirect3DIndexBuffer8_vtbl

{
    /*** IUnknown methods ***/
    DWORD (__stdcall *QueryInterface) (void* this, void* riid, void** ppvObj);
    ULONG (__stdcall *AddRef) (void* this);
    ULONG (__stdcall *Release) (void* this);

    /*** IDirect3DResource8 methods ***/
    DWORD (__stdcall *GetDevice) (void* this, IDirect3DDevice8** ppDevice);
    DWORD (__stdcall *SetPrivateData) (void* this, void* refguid, void* pData,DWORD SizeOfData,DWORD Flags);
    DWORD (__stdcall *GetPrivateData) (void* this, void* refguid,void* pData,DWORD* pSizeOfData);
    DWORD (__stdcall *FreePrivateData) (void* this, void* refguid);
    DWORD (__stdcall *SetPriority) (void* this, DWORD PriorityNew);
    DWORD (__stdcall *GetPriority) (void* this);
    void (__stdcall *PreLoad) (void* this);
    D3DRESOURCETYPE (__stdcall *GetType) (void* this);
    DWORD (__stdcall *Lock) (void* this, UINT OffsetToLock,UINT SizeToLock,BYTE** ppbData,DWORD Flags);
    DWORD (__stdcall *Unlock) (void* this);
    DWORD (__stdcall *GetDesc) (void* this, D3DINDEXBUFFER_DESC *pDesc);
};

struct IDirect3DSurface8 { IDirect3DSurface8_vtbl* vtptr; };
struct IDirect3DSurface8_vtbl

{
    /*** IUnknown methods ***/
    DWORD (__stdcall *QueryInterface) (void* this, void* riid, void** ppvObj);
    ULONG (__stdcall *AddRef) (void* this);
    ULONG (__stdcall *Release) (void* this);

    /*** IDirect3DSurface8 methods ***/
    DWORD (__stdcall *GetDevice) (void* this, IDirect3DDevice8** ppDevice);
    DWORD (__stdcall *SetPrivateData) (void* this, void* refguid, void* pData,DWORD SizeOfData,DWORD Flags);
    DWORD (__stdcall *GetPrivateData) (void* this, void* refguid,void* pData,DWORD* pSizeOfData);
    DWORD (__stdcall *FreePrivateData) (void* this, void* refguid);
    DWORD (__stdcall *GetContainer) (void* this, void* riid,void** ppContainer);
    DWORD (__stdcall *GetDesc) (void* this, D3DSURFACE_DESC *pDesc);
    DWORD (__stdcall *LockRect) (void* this, D3DLOCKED_RECT* pLockedRect, RECT* pRect,DWORD Flags);
    DWORD (__stdcall *UnlockRect) (void* this);
};


struct IDirect3DVolume8 { IDirect3DVolume8_vtbl* vtptr; };
struct IDirect3DVolume8_vtbl

{
    /*** IUnknown methods ***/
    DWORD (__stdcall *QueryInterface) (void* this, void* riid, void** ppvObj);
    ULONG (__stdcall *AddRef) (void* this);
    ULONG (__stdcall *Release) (void* this);

    /*** IDirect3DVolume8 methods ***/
    DWORD (__stdcall *GetDevice) (void* this, IDirect3DDevice8** ppDevice);
    DWORD (__stdcall *SetPrivateData) (void* this, void* refguid, void* pData,DWORD SizeOfData,DWORD Flags);
    DWORD (__stdcall *GetPrivateData) (void* this, void* refguid,void* pData,DWORD* pSizeOfData);
    DWORD (__stdcall *FreePrivateData) (void* this, void* refguid);
    DWORD (__stdcall *GetContainer) (void* this, void* riid,void** ppContainer);
    DWORD (__stdcall *GetDesc) (void* this, D3DVOLUME_DESC *pDesc);
    DWORD (__stdcall *LockBox) (void* this, D3DLOCKED_BOX * pLockedVolume, D3DBOX* pBox,DWORD Flags);
    DWORD (__stdcall *UnlockBox) (void* this);
};

typedef struct IDirect3DVolume8 *LPDIRECT3DVOLUME8, *PDIRECT3DVOLUME8;

/****************************************************************************
 * Flags for SetPrivateData method on all D3D8 interfaces
 *
 * The passed pointer is an IUnknown ptr. The SizeOfData argument to SetPrivateData
 * must be set to sizeof(IUnknown*). Direct3D will call AddRef through this
 * pointer and Release when the private data is destroyed. The data will be
 * destroyed when another SetPrivateData with the same GUID is set, when
 * FreePrivateData is called, or when the D3D8 object is freed.
 ****************************************************************************/
#define D3DSPD_IUNKNOWN                         0x00000001L

/****************************************************************************
 *
 * Parameter for IDirect3D8 Enum and GetCaps8 functions to get the info for
 * the current mode only.
 *
 ****************************************************************************/

#define D3DCURRENT_DISPLAY_MODE                 0x00EFFFFFL

/****************************************************************************
 *
 * Flags for IDirect3D8::CreateDevice's BehaviorFlags
 *
 ****************************************************************************/

#define D3DCREATE_FPU_PRESERVE                  0x00000002L
#define D3DCREATE_MULTITHREADED                 0x00000004L

#define D3DCREATE_PUREDEVICE                    0x00000010L
#define D3DCREATE_SOFTWARE_VERTEXPROCESSING     0x00000020L
#define D3DCREATE_HARDWARE_VERTEXPROCESSING     0x00000040L
#define D3DCREATE_MIXED_VERTEXPROCESSING        0x00000080L

#define D3DCREATE_DISABLE_DRIVER_MANAGEMENT     0x00000100L


/****************************************************************************
 *
 * Parameter for IDirect3D8::CreateDevice's iAdapter
 *
 ****************************************************************************/

#define D3DADAPTER_DEFAULT                     0

/****************************************************************************
 *
 * Flags for IDirect3D8::EnumAdapters
 *
 ****************************************************************************/

#define D3DENUM_NO_WHQL_LEVEL                   0x00000002L

/****************************************************************************
 *
 * Maximum number of back-buffers supported in DX8
 *
 ****************************************************************************/

#define D3DPRESENT_BACK_BUFFERS_MAX             3L

/****************************************************************************
 *
 * Flags for IDirect3DDevice8::SetGammaRamp
 *
 ****************************************************************************/

#define D3DSGR_NO_CALIBRATION                  0x00000000L
#define D3DSGR_CALIBRATE                       0x00000001L

/****************************************************************************
 *
 * Flags for IDirect3DDevice8::SetCursorPosition
 *
 ****************************************************************************/

#define D3DCURSOR_IMMEDIATE_UPDATE             0x00000001L

/****************************************************************************
 *
 * Flags for DrawPrimitive/DrawIndexedPrimitive
 *   Also valid for Begin/BeginIndexed
 *   Also valid for VertexBuffer::CreateVertexBuffer
 ****************************************************************************/


/*
 *  DirectDraw error codes
 */
#define _FACD3D  0x876
#define MAKE_D3DHRESULT( code )  MAKE_HRESULT( 1, _FACD3D, code )

/*
 * Direct3D Errors
 */
#define D3D_OK                              S_OK

#define D3DERR_WRONGTEXTUREFORMAT               MAKE_D3DHRESULT(2072)
#define D3DERR_UNSUPPORTEDCOLOROPERATION        MAKE_D3DHRESULT(2073)
#define D3DERR_UNSUPPORTEDCOLORARG              MAKE_D3DHRESULT(2074)
#define D3DERR_UNSUPPORTEDALPHAOPERATION        MAKE_D3DHRESULT(2075)
#define D3DERR_UNSUPPORTEDALPHAARG              MAKE_D3DHRESULT(2076)
#define D3DERR_TOOMANYOPERATIONS                MAKE_D3DHRESULT(2077)
#define D3DERR_CONFLICTINGTEXTUREFILTER         MAKE_D3DHRESULT(2078)
#define D3DERR_UNSUPPORTEDFACTORVALUE           MAKE_D3DHRESULT(2079)
#define D3DERR_CONFLICTINGRENDERSTATE           MAKE_D3DHRESULT(2081)
#define D3DERR_UNSUPPORTEDTEXTUREFILTER         MAKE_D3DHRESULT(2082)
#define D3DERR_CONFLICTINGTEXTUREPALETTE        MAKE_D3DHRESULT(2086)
#define D3DERR_DRIVERINTERNALERROR              MAKE_D3DHRESULT(2087)

#define D3DERR_NOTFOUND                         MAKE_D3DHRESULT(2150)
#define D3DERR_MOREDATA                         MAKE_D3DHRESULT(2151)
#define D3DERR_DEVICELOST                       MAKE_D3DHRESULT(2152)
#define D3DERR_DEVICENOTRESET                   MAKE_D3DHRESULT(2153)
#define D3DERR_NOTAVAILABLE                     MAKE_D3DHRESULT(2154)
#define D3DERR_OUTOFVIDEOMEMORY                 MAKE_D3DHRESULT(380)
#define D3DERR_INVALIDDEVICE                    MAKE_D3DHRESULT(2155)
#define D3DERR_INVALIDCALL                      MAKE_D3DHRESULT(2156)
#define D3DERR_DRIVERINVALIDCALL                MAKE_D3DHRESULT(2157)

#endif /* _D3D_H_ */

