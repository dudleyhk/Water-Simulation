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
		_WaterScale    		("Water Scale",    Float) = 1
		_WaterSpeed    		("Water Speed",    Float) = 1
		_WaterDistance 		("Water Distance", Float) = 1
		_WaterNoiseStrength ("Noise Strength", Float) = 1
		_WaterNoiseWalk		("Noise Walk",     Float) = 1
	}

	SubShader 
	{
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


		float3 getWavePos(float3 pos)
		{
			pos.y = 0.0;
			float offset = pos.z;

			// Dont understand the /WaterDistance bit..
			pos.y +=  sin((_Time.y * _WaterSpeed + offset) / _WaterDistance) * _WaterScale;

			// Add noise... I dont understand what the x value of float4 below is doing over time in a sin func?
			pos.y += tex2Dlod(_NoiseTex, float4(pos.x, pos.z + sin(_Time.y * 0.1), 0.0, 0.0) * _WaterNoiseWalk).a * _WaterNoiseStrength;

			return pos;

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
			worldPos.xyz += getWavePos(worldPos.xyz);
			xVector += getWavePos(xVector);
			zVector += getWavePos(zVector);


			v.normal = recalculateNormals(worldPos, xVector, zVector);

			float4 localPos = mul(unity_WorldToObject, float4(worldPos.xyz, worldPos.w));
			v.vertex = localPos;
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
