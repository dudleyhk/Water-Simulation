Shader "Custom/WaveShader" 
{
	Properties 
	{
		// Predefined properties.
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_Transparency ("Opacity", Range(0, 1)) = 1.0


		// Wave Properties
		_NoiseTex           ("Texture", 2D) = "white" {}
		_WaterScale    		("Water Scale",      Float) = 1
		_WaterSpeed    		("Water Speed",      Float) = 1
		_WaterDistance 		("Water Distance",   Float) = 1
		_WaterNoiseStrength ("Noise Strength",   Float) = 1
		_WaterNoiseWalk		("Noise Walk",       Float) = 1

		// Gerstners Properties
		_WaveLength("Wave Length", Float) = 1
		_Amplitude("Wave Height", Float) = 1
		_WaveSpeed("Wave Speed", Float) = 1
		_WaveDirection("Wave Direction", Vector) = (1, 0, 0)
		_WaveSteepness("Wave Steepness", Range(0, 1)) = 0.5
	}

	SubShader 
	{
		// TODO: Create a different Pass for the ripple code.

		// TODO: Implement opacity code.
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Standard vertex:vert fullforwardshadows
		#pragma target 4.0


		// -------------------------------------------------------------------

		// HLSL Predefined variables.
		sampler2D _MainTex;
		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		// Waves variables
		sampler2D 	_NoiseTex			;
		float		_WaterScale    		;
		float 		_WaterSpeed    		;	
		float		_WaterDistance 		;	
		float		_WaterNoiseStrength ;	
		float 		_WaterNoiseWalk		; 	

		// Create multiple waves at different frequencies for realistic effect. 
		// Gerstner Attributes
		float _WaveLength;
		float _Amplitude;
		float _WaveSpeed;
		float3 _WaveDirection;
		float _WaveSteepness;



		struct Input 
		{
			float2 uv_MainTex;
		};

		// --------------------------------------------------------------------

		/*Move the object from current space into world space, calculate the normals and change back. */
		float3 recalculateNormals(float3 worldPos, float3 xVector, float3 zVector)
		{			
			// calculate the direction of its normal.
			float3 vertexNormalLocal = cross(zVector - worldPos, xVector - worldPos);
			float3 vertexNormalWorld = mul(unity_WorldToObject, vertexNormalLocal);
			float3 newNormal = normalize(vertexNormalWorld);

			return newNormal;
		}

		// TODO: Implement Ripple code. 


		float3 basicWave(float3 pos)
		{
			pos.y = 0.0;
			float offset = pos.z;

			// Dont understand the /WaterDistance bit..
			pos.y += sin((_Time.y * _WaterSpeed + offset) / _WaterDistance) * _WaterScale;

			// Add noise... I dont understand what the x value of float4 below is doing over time in a sin func?
			pos.y += tex2Dlod(_NoiseTex, float4(pos.x, pos.z + sin(_Time.y * 0.1), 0.0, 0.0) * _WaterNoiseWalk).a * _WaterNoiseStrength;

		
			return pos;
		}



		float3 gerstnerWave(float3 worldPos)
		{
			// w
			float freq = 2 / _WaveLength;      

			// alpha
			float phase = 2 * 3.1416 / _WaveLength;

			// Q
			float pinch = _WaveSteepness / (freq * _Amplitude);

			// D
			float dir = dot(worldPos.xz, _WaveDirection.xz);


			float X = worldPos.x + (pinch * _Amplitude) * (_WaveDirection.x * cos(freq * dir + phase * _Time.y));
			float Z = worldPos.z + (pinch * _Amplitude) * (_WaveDirection.z * cos(freq * dir + phase * _Time.y));
			float Y = _Amplitude * sin(freq * dir + phase * _Time.y);


			return float3(X, Y, Z);
		}



		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);

			float4 worldPos = mul(unity_ObjectToWorld, v.vertex);

			// because we don't know the exact vertice but we need two extra vectors for a cross product
			//	make two short vectors... 
			float3 xVector = worldPos + float3(0.05, 0, 0);
			float3 zVector = worldPos + float3(0, 0, 0.05);

			// Any effects.

			// Anything done to the worldPos vertice has to be done to the friend vectors.
			worldPos.xyz += gerstnerWave(worldPos.xyz);
			xVector      += gerstnerWave(xVector);
			zVector      += gerstnerWave(zVector);

			v.normal = recalculateNormals(worldPos, xVector, zVector);

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




