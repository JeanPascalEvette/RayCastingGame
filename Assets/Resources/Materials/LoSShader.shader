Shader "Falloff/GhostFlat" {
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
	_RimPower("Rim Exponent", Range(0.5,25.0)) = 3.0
		_BaseAlpha("Base Alpha", Range(0.0,1.0)) = 1.0
	}



		SubShader{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Stencil{
		Ref 1
		ReadMask 1
		Comp NotEqual
		Pass Replace
	}
		CGPROGRAM
#pragma surface surf Lambert alpha


		sampler2D _MainTex;
	fixed4 _Color;
	float _RimPower;
	float _BaseAlpha;

	struct Input {
		float2 uv_MainTex;
		float3 viewDir;
	};

	void surf(Input IN, inout SurfaceOutput o) {
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		half rim = saturate(dot(normalize(IN.viewDir), o.Normal));
		//o.Albedo = c.rgb;        //No albedo, only emission
		o.Emission = c.rgb;
		o.Alpha = pow(rim, _RimPower) * _BaseAlpha * c.a;
	}
	ENDCG
	}
		FallBack "Self-Illumin/Diffuse"
}
