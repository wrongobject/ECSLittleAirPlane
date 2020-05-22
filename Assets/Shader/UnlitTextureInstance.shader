Shader "Unlit/UnlitTextureInstance"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_UVOffset("UVOffset",Vector) = (0,0,0,0)
		[Enum(UnityEngine.Rendering.BlendMode)]_SrcBlend("SrcBlend",float) = 0
		[Enum(UnityEngine.Rendering.BlendMode)]_DstBlend("DstBlend",float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
			Blend [_SrcBlend][_DstBlend]
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
			#pragma multi_compile_instancing
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			
UNITY_INSTANCING_BUFFER_START(UnityPerMaterial)
	UNITY_DEFINE_INSTANCED_PROP(float4,_UVOffset)
UNITY_INSTANCING_BUFFER_END(UnityPerMaterial)
            v2f vert (appdata v)
            {
                v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v,o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
				
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float4 off =UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial,_UVOffset);
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv + off.zw);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
