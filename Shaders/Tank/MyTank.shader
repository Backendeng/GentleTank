// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/MyTank" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BrokenTex ("BrokenTexture (RGB)", 2D) = "white" {}
		_TotalBrokenCount ("Total BrokenCount", float) = 10
		_CurBrokenCount ("Current BrokenCount", float) = 0
	}
	SubShader {
		Pass {

		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma exclude_renderers xbox360 ps3 gles

		struct v2f {
			half4 pos : SV_POSITION;
			half2 uv : TEXCOORD0;
		};

		v2f vert(appdata_full v) {
			v2f o;
			
			o.pos = UnityObjectToClipPos (v.vertex);	
			o.uv.xy = v.texcoord.xy;
					
			return o; 
		}		
		
		sampler2D _MainTex;
		sampler2D _BrokenTex;
		float _CurBrokenCount;
		float _TotalBrokenCount;
		
		half4 frag(v2f i) : COLOR{
			half4 b = tex2D(_BrokenTex,i.uv);
			half4 m = tex2D(_MainTex,i.uv);
			
			if(_CurBrokenCount < _TotalBrokenCount){
				m = m * (_TotalBrokenCount - _CurBrokenCount) / _TotalBrokenCount;
				m += (b * _CurBrokenCount / _TotalBrokenCount);
			}else{
				m = b;
			}
			return m;
		}
		ENDCG
		} 
	}
	FallBack "Diffuse"
}
