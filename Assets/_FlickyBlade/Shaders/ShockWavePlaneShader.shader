Shader "FX/Shock Wave" {
Properties {
	_BumpAmt  ("Distortion", range (0,128)) = 10
	_MainTex ("Tint Color (RGB)", 2D) = "white" {}
	_Color ("Tint Color", Color) = (0,0,0,0)
	_ShockWaveRad("Shock wave Radius", float) = 6
	_Ring ("Ring", range(0,1))=0.2
	_GrabTexture ("_GrabTexture", 2D) = "white" {}
}

Category {
	SubShader {
		Pass {
			Name "BASE"
			
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_fog
#include "UnityCG.cginc"

struct appdata_t {
	float4 vertex : POSITION;
	float2 texcoord: TEXCOORD0;
};

struct v2f {
	float4 vertex : SV_POSITION;
	float4 uvgrab : TEXCOORD0;
	float2 uvmain : TEXCOORD2;
	float4 vertextObjectPos : TEXCOORD3;
};

float _BumpAmt;
float4 _MainTex_ST;

v2f vert (appdata_t v)
{
	
	v2f o;
	o.vertex = UnityObjectToClipPos(v.vertex);
	o.uvgrab = ComputeGrabScreenPos(o.vertex);
	o.uvmain = TRANSFORM_TEX( v.texcoord, _MainTex );
	o.vertextObjectPos = v.vertex;
	return o;
}

sampler2D _GrabTexture;
float4 _GrabTexture_TexelSize;
sampler2D _MainTex;
float _ShockWaveRad;
float _Ring;
float4 _Color;

half4 frag (v2f i) : SV_Target
{
	#if UNITY_SINGLE_PASS_STEREO
	i.uvgrab.xy = TransformStereoScreenSpaceTex(i.uvgrab.xy, i.uvgrab.w);
	#endif
	
	float2 offset = float2(i.vertextObjectPos.x,i.vertextObjectPos.z);
	float distance = length(offset);
	float offsetAmount = pow(max(distance - _ShockWaveRad*(1- _Ring),0),2)/distance	;
	offsetAmount = max(max(1 - offsetAmount,0) + max(distance - _ShockWaveRad, 0),0);
	offsetAmount = clamp((1 - offsetAmount)*_BumpAmt/100,0,1)*2;
	offset /= length(offset);
	offset *= -1;
	offset *= offsetAmount;
	float4 uvGrabOffset = ComputeGrabScreenPos(UnityObjectToClipPos(float4(i.vertextObjectPos.x + offset.x, i.vertextObjectPos.y, i.vertextObjectPos.z + offset.y, i.vertextObjectPos.w)));
	#if UNITY_UV_STARTS_AT_TOP
		uvGrabOffset.y = 1-uvGrabOffset.y;
	#endif
	half4 col = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(uvGrabOffset));
	half4 tint = tex2D(_MainTex, i.uvmain);
	col *= tint;
	col += _Color*offsetAmount;
	return col;
}
ENDCG
		}
	}

	// ------------------------------------------------------------------
	// Fallback for older cards and Unity non-Pro

	SubShader {
		Blend DstColor Zero
		Pass {
			Name "BASE"
			SetTexture [_MainTex] {	combine texture }
		}
	}
}

}
