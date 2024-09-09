Shader "ImageEffect/ShockWaveShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ExplosionPos("Explosion Position", Vector) = (0,0,0,0)
		_Magnitude("Magnitude", float) = 0.5
		_ExplosionRad("Explosion Radius", float) = 0.1
		_Ring("Ring", Range(0, 0.9)) = 0.1
		_ScreenRatio("ScreenRatio",float) = 1.778
		_ShockWaveColorTint("ShockWaveColor",Color) = (0,0,0,0)
		_ShockWaveNormal("ShockWave Normal", Vector) = (0,1,0)
		_ScreenRayVector("ScreenRayVector", Vector) = (0,0,1)

	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			float2 _ExplosionPos;
			float _Magnitude;
			float _ExplosionRad;
			float _Ring;
			float _ScreenRatio;
			float4 _ShockWaveColorTint;
			float3 _ShockWaveNormal;
			float3 _ScreenRayVector;
			fixed4 frag (v2f i) : SV_Target
			{
#if UNITY_UV_STARTS_AT_TOP
				_ExplosionPos = float2(_ExplosionPos.x,1 - _ExplosionPos.y);
#endif
				float _ScaleX = 1-abs((_ScreenRayVector-_ShockWaveNormal).x);
				float _ScaleY = 1-abs((_ScreenRayVector-_ShockWaveNormal).y);
				fixed4 col = tex2D(_MainTex, i.uv);
				float2 offset = i.uv - _ExplosionPos;
				offset = float2(offset.x / (_ScreenRatio*_ScaleX), offset.y / _ScaleY);
				float disp = length(offset);
				if (disp <=_ExplosionRad&&disp>=_ExplosionRad*(1- _Ring))
				{
					offset /= length(offset);//direction only
					disp = (0.5 - abs((_ExplosionRad - disp)/(_ExplosionRad*_Ring)-0.5))*2*(disp- _ExplosionRad*(1 - _Ring))/ (_ExplosionRad*_Ring);//Generate offset gradient and keep ring egde offset 0
					offset = offset*disp*(-1)*_Magnitude*_ExplosionRad*_Ring;
					col = tex2D(_MainTex, i.uv + offset);
					col += _ShockWaveColorTint*length(offset)*_ShockWaveColorTint.a*10;
				}
				return col;
			}
			ENDCG
		}
	}
}
