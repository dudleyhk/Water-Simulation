// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/GerstnerWave"
{
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0

		// Wave Parameters
		_WaveLength   ("Wave Length", Float) = 1
		_Amplitude    ("Wave Height", Float) = 1
		_WaveSpeed    ("Wave Speed", Float) = 1
		_WaveDirection("Wave Direction", Vector) = (1, 0, 0)
		_WaveSteepness("Wave Steepness", Range(0, 1)) = 0.5



	}
	SubShader
		{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert

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

		// Wave Attributes
		float _WaveLength;
		float _Amplitude;
		float _WaveSpeed;
		float3 _WaveDirection;
		float _WaveSteepness;




		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
			
			// Gerstner algorithm
			// -----------------------------------------
			//float wave = _Amplitude * sin(direction * frequency + phaseTime);

			float frequency  = 2 / _WaveLength;
			float phase      = _WaveSpeed * frequency;
			float direction  = dot(_WaveDirection.xz, worldPos.xz);
			float phaseTime  = _Time.y * phase;
			float steepness  = _WaveSteepness / (frequency * _Amplitude); // _WaveLength is my guess to wat 'number of waves' is.
			float waveShape  = steepness * _Amplitude;
			float cosineWave = cos(frequency * direction + phaseTime);


			float partX = worldPos.x + (waveShape * _WaveDirection.x * cosineWave);
			float partY = worldPos.z + (waveShape * _WaveDirection.z * cosineWave);
			float partZ = _Amplitude * sin(frequency * direction + phaseTime);



			worldPos = float3(partX, partZ, partY);





			v.vertex.xyz = mul(unity_WorldToObject, worldPos);
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
