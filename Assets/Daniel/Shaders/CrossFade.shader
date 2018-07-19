Shader "Unlit/CrossFade"
{
	Properties
	{
		_Tex1 ("Texture1", 2D) = "white" {}
		_Tex2 ("Texture2", 2D) = "white" {}
		_FadeAmount ("Fade Amount", Range(0,1)) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" }
		LOD 100

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
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _Tex1;
			sampler2D _Tex2;
			float _FadeAmount;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_Tex1, i.uv);
				fixed4 col2 = tex2D(_Tex2, i.uv);
				col = lerp(col, col2, _FadeAmount)
					//return col;
					col.w = 0.5f;
				return col;
			}
			ENDCG
		}
	}
}
