// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
// courtesy of Dan Moran
Shader "CloverSwatch/BinstonBarrier"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (0,0,0,0)
	}

	SubShader
	{
		Blend One One
		ZWrite Off
		Cull Off

		Tags
		{
			"RenderType"="Transparent"
			"Queue"="Transparent"
		}

		Pass
		{
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 screenuv : TEXCOORD1;
				float3 viewDir : TEXCOORD2;
				float3 objectPos : TEXCOORD3;
				float4 vertex : SV_POSITION;
				float depth : DEPTH;
				float3 normal : NORMAL;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				o.screenuv = ((o.vertex.xy / o.vertex.w) + 1)/2;
				o.screenuv.y = 1 - o.screenuv.y;
				o.depth = -mul(UNITY_MATRIX_MV, v.vertex).z *_ProjectionParams.w;

				o.objectPos = v.vertex.xyz;		
				o.normal = UnityObjectToWorldNormal(v.normal);
				o.viewDir = normalize(UnityWorldSpaceViewDir(mul(unity_ObjectToWorld, v.vertex)));

				return o;
			}
			
			sampler2D _CameraDepthNormalsTexture;
			fixed4 _Color;

			float triWave(float t, float offset, float yOffset)
			{
				return saturate(abs(frac(offset + t) * 2 - 1) + yOffset);
			}

			static const float PI = 3.14159265f;
			fixed4 frag (v2f i) : SV_Target
			{
				float screenDepth = DecodeFloatRG(tex2D(_CameraDepthNormalsTexture, i.screenuv).zw);
				float diff = screenDepth - i.depth;
				float intersect = 0;
				
				if (diff > 0)
					intersect = 1 - smoothstep(0, _ProjectionParams.w * 0.5, diff);

				float glow = intersect;
				fixed4 glowColor = smoothstep(.9, .95, glow);
				glow *= .5;

				float ySpacing = PI / 120;
				float yPos = sin(abs(i.objectPos.y) * .5);
				float yWire = abs(ySpacing - (yPos % (ySpacing * 2))) / ySpacing;
				yWire = smoothstep(.95, .96, yWire);
				float theta = atan2(i.objectPos.x, i.objectPos.z);
				float xzSpacing = PI / 24;
				float xzWire = abs(xzSpacing - ((theta + 3.14) % (xzSpacing * 2))) / xzSpacing;
				xzWire = smoothstep(.95, .96, xzWire);
				float wire = max(yWire, xzWire);
				fixed4 col = _Color * _Color.a + wire;
				col *= pow(1 - abs(i.objectPos.y * 2), 2);
				glow *= pow(1 - abs(i.objectPos.y * 2), 1);
				col = max(col, glowColor * glow);
				col *= .33;
				return col;
			}
			ENDCG
		}
	}
}
