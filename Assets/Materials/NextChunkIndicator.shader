Shader "thquinn/NextChunkIndicator"
{
    Properties
    {
        _Color ("Color", Color) = (0,0,0,0)
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 objectPos : TEXCOORD1;
                float3 normal : TEXCOORD2;
            };

            fixed4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                o.objectPos = v.vertex.xyz;		
                o.normal = v.normal;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float a = max(0, -i.objectPos.z * .5 + .2);
                float mult = sin(_Time.y * 2) * .5 + .5;
                float n = 1 - abs(dot(i.normal, float3(0, 0, 1)));
                fixed4 c = _Color;
                c.a *= a * mult * n;
                return c;
            }
            ENDCG
        }
    }
}
