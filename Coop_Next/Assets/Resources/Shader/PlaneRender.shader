// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

Shader "CoopGame/PlaneRender" {
	Properties{
		_MainTex("MainTex", 2D) = "white" {}
		//_Opacity("Opacity", 2D) = "white" {}
		_Color("Color", Color) = (0,0,0,0)
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent" "RenderQueue" = "AlphaTest" }
		LOD 100

		Pass
		{
			Name "MeshPass"
			Tags{ "LightMode" = "ForwardBase" }

			ZTest on
			ZWrite off
			Cull off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"
			#include "AutoLight.cginc"

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

			sampler2D _MainTex;
			float4 _Color;


			v2f vert(appdata v)
			{
				v2f o;

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col;
				col = _Color * tex2D(_MainTex, i.uv);

				return col;
			}
			ENDCG
		}	
	}
}

