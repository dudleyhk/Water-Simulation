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

		speed("Speed", Float) = -2
		frequency("Waves per Second", Float) = 0.1
		amp("Amplitude", Float) = 0.5

		// originally 0.05
		neighbourDist("Neighbour Sample Distance", Range(0, 10)) = 0.05
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


		struct Input 
		{
			float2 uv_MainTex;
		};

		sampler2D _MainTex;
		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		float speed;
		float frequency;
		float amp;
		float neighbourDist;



		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

		float sineffect(float3 v)
		{
			// (v.x * v.x) + (v.z * v.z);  // start at centre
			// (v.x + v.z); // start at corner.

			half voffset = (v.x + v.z);
			return amp * sin(_Time.y * speed + voffset * frequency);
		}

		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);

			float3 v0 = mul(float4x4(unity_ObjectToWorld), v.vertex).xyz;

			// neighbour sampling \\
			// Create two fake neighbour vertices.
			// The imp[ortant thins is that they are distorted in the same way that a real vertex in their location would.
			// This is pretty easy as we're just going to do some trig based on position, so really any samples will do. 
			float3 v1 = v0 + float3(neighbourDist, 0, 0);
			float3 v2 = v0 + float3(0, 0, neighbourDist);

			v0 += sineffect(v0);
			v1 += sineffect(v1);
			v2 += sineffect(v2);

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
