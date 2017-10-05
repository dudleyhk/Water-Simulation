Shader "Custom/WaterRipple"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Colour("Colour", Color) = (1, 1, 1, 1)
	
		_Amplitude("Scale", Float) = 1
		_Speed("Speed", Float) = 1
		_Frequency("Waves per Second", Float) = 1
		 
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert
		#pragma target 4.0

		#include "UnityCG.cginc"
		
		// HLSL Definitions
		sampler2D _MainTex;
		half4 _Colour;
  		float _Amplitude;
  		float _Speed    ;
  		float _Frequency;



		struct Input
		{
			float2 uv_MainTex;
			float3 customValue;
		};


		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);

			half offsetVert = (v.vertex.x * v.vertex.x) + (v.vertex.z * v.vertex.z);
			half value = _Amplitude * sin(_Time.y * _Speed + offsetVert * _Frequency);

			v.vertex.y += value;
			//o.customValue = value;
		}


		// Surf is about surface
		void surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 col = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = _Colour;
			//o.Normal.y += IN.customValue;
		}
		ENDCG
	}
	Fallback "Diffuse"
}