// Upgrade NOTE: upgraded instancing buffer 'MyProperties' to new syntax.

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

Shader "SgLib/Single color"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1)
		_Tint("Tint color", Color) = (1,1,1,1)
	}

		SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"

			float4 _LightColor0;
			float4 _Tint;
			struct appdata
			{
				float4 localPos : POSITION;
				float3 localNormal: NORMAL;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 clipPos : SV_POSITION;
				float4 color:COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			UNITY_INSTANCING_BUFFER_START(MyProperties)
			UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
#define _Color_arr MyProperties
			UNITY_INSTANCING_BUFFER_END(MyProperties)

			v2f vert(appdata v)
			{
				v2f o;

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				o.clipPos = UnityObjectToClipPos(v.localPos);

				float4 worldNormal = normalize(mul(unity_ObjectToWorld, float4(v.localNormal,0)));
				float4 lightDir = normalize(_WorldSpaceLightPos0);

				float4 diffuseColor = UNITY_ACCESS_INSTANCED_PROP(_Color_arr, _Color)*_LightColor0*max(0, dot(worldNormal, lightDir));
				float4 ambientColor = UNITY_ACCESS_INSTANCED_PROP(_Color_arr, _Color)*unity_AmbientSky;
				o.color = (diffuseColor + ambientColor)*_Tint;
				return o;
			}
	
			fixed4 frag(v2f i) : COLOR
			{
				return i.color;
			}
			ENDCG
		}
	}
}