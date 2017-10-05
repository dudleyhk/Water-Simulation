// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/WaterEffect"
{
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0

		phase("Phase", Float) = 1
		speed("Speed", Float) = 1
		scale("Scale", Float) = 1
		depth("Depth", Float) = 1
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard vertex:vert fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input 
		{
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		float phase;
		float speed;
		float scale;
		float depth;



		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END


		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);

			float3 v0 = mul(unity_ObjectToWorld, v.vertex).xyz;

			float3 v1 = v0 + float3(0.05, 0, 0);
			float3 v2 = v0 + float3(0, 0, 0.05);


			v0.y += sin(phase + speed + (v0.x * scale)) * depth;
			v1.y += sin(phase + speed + (v1.y * scale)) * depth;
			v2.y += sin(phase + speed + (v2.y * scale)) * depth;


			float3 vna = cross(v2 - v0, v1 - v0);
			float3 vn = mul(float4x4(unity_WorldToObject), vna);

			v.normal = normalize(vn);

			v.vertex.xyz = mul(float4x4(unity_WorldToObject), v0);
		}


		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
