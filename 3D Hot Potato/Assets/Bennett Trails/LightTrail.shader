Shader "Light Trail" {
Properties {
	//_MainTex ("Particle Texture", 2D) = "white" {}
	//_Color ("Main Color", Color) = (1,1,1,1)

}

Category {
	Tags { "Queue"="Overlay" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha One
	Cull Off Lighting Off ZWrite Off ZTest Always Fog { Mode Off }
	
	BindChannels {
		Bind "Color", color
		Bind "Vertex", vertex
		//Bind "TexCoord", texcoord
	}
	
	SubShader {
		Pass{
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				struct appdata_t {
					float4 vertex : POSITION;
					float4 color : COLOR;
				};

				struct v2f {
					float4 vertex : SV_POSITION;
					float4 color : COLOR;
				};

			//	fixed4 _Color;

				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.color = v.color;
					return o;
				}

				fixed4 frag (v2f i) : COLOR
				{
					fixed4 returnColor = i.color;
					returnColor.a = 1.0;
					//returnColor.rgb *= i.color.a;
					return returnColor;
				}
			ENDCG
		}
	}
}
}
