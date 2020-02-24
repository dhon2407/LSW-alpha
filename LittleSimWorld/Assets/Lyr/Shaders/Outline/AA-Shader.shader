Shader "Custom/Outline_Only"
{
	Properties {
		_MainTex("Texture", 2D) = "white" {}
		_Color("Outline",Color) = (1,1,1,1)
		_Width("Width",float) = 1
	}
	
    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    ENDHLSL

	SubShader {
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" }
		LOD 100

		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		ZWrite Off

		Pass
		{
		//ZWrite On
		//ZTest Always
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag

		#include "UnityCG.cginc"

		struct appdata {
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
		};

		struct v2f {
			float2 uv : TEXCOORD0;
			float4 vertex : SV_POSITION;
		};

		sampler2D _MainTex;
		float4 _MainTex_ST;
		float _Columns,_Width;
		float _Rows;


		float4 _MainTex_TexelSize;
		fixed4 _Color;

		v2f vert(appdata v) {
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.uv = TRANSFORM_TEX(v.uv, _MainTex);
			return o;
		}


		fixed IsEdgeCoord(float2 uv) {

			if (_Width <= 0) { return false; }

			float x = _MainTex_TexelSize.x;
			float y = _MainTex_TexelSize.y;

			fixed left = tex2D(_MainTex, uv - fixed2(x, 0) * _Width).a;
			fixed right = tex2D(_MainTex, uv + fixed2(x, 0) * _Width).a;
			fixed up = tex2D(_MainTex, uv + fixed2(0, y) * _Width).a;
			fixed down = tex2D(_MainTex, uv - fixed2(0, y) * _Width).a;

			fixed dTR = tex2D(_MainTex, uv + fixed2(x, y) * _Width).a;
			fixed dTL = tex2D(_MainTex, uv + fixed2(-x, y) * _Width).a;
			fixed dBR = tex2D(_MainTex, uv + fixed2(x, -y) * _Width).a;
			fixed dBL = tex2D(_MainTex, uv + fixed2(-x, -y) * _Width).a;

			return (left + right + down + up + dTR + dTL + dBR + dBL);
		}

		fixed4 frag(v2f i) : COLOR {
			v2f x = i;
			float2 uv = i.uv;
			fixed edge = IsEdgeCoord(uv);
			fixed4 col = tex2D(_MainTex, uv);
			if (edge > 0.01f && col.a<0.1f) { col = _Color; }
			else { discard; }

			return col;
		}
		ENDCG
		}
	}
}