Shader "BatchingTools/Transparent Diffuse" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	//_ColorTex ("Color", 2D) = "gray" {}
	
}
SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 200

CGPROGRAM
#pragma surface surf Lambert

//sampler2D _ColorTex;
sampler2D _MainTex;

struct Input {
	float2 uv_MainTex:TEXCOORD0;
	//float2 uv_ColorTex:TEXCOORD1;
	float4 color : COLOR;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	//fixed4 mat = tex2D(_ColorTex, IN.uv_ColorTex);
	
	o.Albedo = tex.rgb * IN.color.rgb;
	o.Alpha = tex.a;
}
ENDCG
}

Fallback "VertexLit"
}
