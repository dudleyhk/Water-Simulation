Shader "Custom/WaveShader" 
{
	Properties 
	{
		// Predefined properties.
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_Transparency("Opacity", Range(0, 1)) = 1.0

		// Gerstners Properties
		_WindSpeed("Wind Speed", Range(0.1, 5)) = 1
		_WindDirection("Wind Direction", Vector) = (0, 0, 0)
		
		
		_CurrentVertexPos("Current Vertex Position", Vector) = (0, 0, 0)
		_GerstnerWave("Gerstner Wave", Vector) = (0, 0, 0)


		// Main Wave
		/*[HideInInspector]*/ _MainWave_Length      ("Main Wave Length", Float) = 1
		/*[HideInInspector]*/ _MainWave_Amplitude   ("Main Wave Amplitude", Float) = 1
		/*[HideInInspector]*/ _MainWave_Speed       ("Main Wave Speed", Float) = 1
		/*[HideInInspector]*/ _MainWave_Steepness   ("Main Wave Steepness", Float) = 1
		/*[HideInInspector]*/ _MainWave_Direction	("Main Wave Direction", Vector) = (0, 0, 0)

		// Secondary Waves - smaller gerstner wave which roll in a different direction.
		[HideInInspector] _SecondaryWave_Length     ("Secondary Wave Length", Float) = 1
		[HideInInspector] _SecondaryWave_Amplitude  ("Secondary Wave Amplitude", Float) = 1
		[HideInInspector] _SecondaryWave_Speed      ("Secondary Wave Speed", Float) = 1
		[HideInInspector] _SecondaryWave_Steepness  ("Secondary Wave Steepness", Float) = 1
		[HideInInspector] _SecondaryWave_Direction	("Secondary Wave Direction", Vector) = (0, 0, 0)

		// Other secondary Waves - smaller gerstner wave which roll in a different direction.
		[HideInInspector] _SecondaryWave_Length     ("Secondary Wave Length", Float) = 1
		[HideInInspector] _SecondaryWave_Amplitude  ("Secondary Wave Amplitude", Float) = 1
		[HideInInspector] _SecondaryWave_Speed      ("Secondary Wave Speed", Float) = 1
		[HideInInspector] _SecondaryWave_Steepness  ("Secondary Wave Steepness", Float) = 1
		[HideInInspector] _SecondaryWave_Direction	("Secondary Wave Direction", Vector) = (0, 0, 0)

		// Wave Intensifier - high frequency waves.
		[HideInInspector] _WaveIntensifier_Length       ("Wave Intensifier Length", Float) = 1
		[HideInInspector] _WaveIntensifier_Amplitude    ("Wave Intensifier Amplitude", Float) = 1
		[HideInInspector] _WaveIntensifier_Speed        ("Wave Intensifier Speed", Float) = 1
		[HideInInspector] _WaveIntensifier_Steepness    ("Wave Intensifier Steepness", Float) = 1
		[HideInInspector] _WaveIntensifier_Direction	("Wave Intensifier Direction", Vector) = (0, 0, 0)
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


		// User Influencer
		float _WindSpeed;
		float3 _WindDirection;

		float3 _CurrentVertexPos;
		float3  _GerstnerWave;
	
		// Main Wave
		float  _MainWave_Length				= 0	 ;
		float  _MainWave_Amplitude			= 0	 ;
		float  _MainWave_Speed				= 0	 ;
		float  _MainWave_Steepness			= 0	 ;
		float3 _MainWave_Direction				 ;
										
		// Secondary Waves				
		float   _SecondaryWave_Length   	= 0	 ;
		float   _SecondaryWave_Amplitude	= 0	 ;
		float   _SecondaryWave_Speed    	= 0	 ;
		float   _SecondaryWave_Steepness	= 0	 ;
		float3  _SecondaryWave_Direction		 ;

		// Secondary Waves				
		float   _OtherSecondaryWave_Length = 0;
		float   _OtherSecondaryWave_Amplitude = 0;
		float   _OtherSecondaryWave_Speed = 0;
		float   _OtherSecondaryWave_Steepness = 0;
		float3  _OtherSecondaryWave_Direction;
										
		// Wave Intensifiers			
		float _WaveIntensifier_Length   	= 0	 ;
		float _WaveIntensifier_Amplitude	= 0	 ;
		float _WaveIntensifier_Speed    	= 0	 ;
		float _WaveIntensifier_Steepness	= 0	 ;
		float3 _WaveIntensifier_Direction		 ;



		// Vertex Input Struct
		struct Input 
		{
			float2 uv_MainTex;
		};




		// TODO: Implement Ripple code. 
		// ----------------------- SHADER ------------------------- \\
		
		
		/*Move the object from current space into world space, calculate the normals and change back. */
		float3 recalculateNormals(float3 worldPos, float3 xVector, float3 zVector)
		{			
			// calculate the direction of its normal.
			float3 vertexNormalLocal = cross(zVector - worldPos, xVector - worldPos);
			float3 vertexNormalWorld = mul(unity_WorldToObject, vertexNormalLocal);
			float3 newNormal = normalize(vertexNormalWorld);

			return newNormal;
		}


		float3 gerstnerWave(float3 worldPos, float _WaveLength, float _Amplitude, float3 _WaveSpeed, float3 _WaveDirection, float _WaveSteepness)
		{  
			//_Amplitude     = _WindSpeed / 2;
			//_WaveSteepness = _WindSpeed / 8;
			//_WaveSpeed     = _WindSpeed / 2;
			//_WaveLength    = (_WindSpeed - 12) / -2;
			

			// w
			float freq = 2 / _WaveLength;      

			// alpha
			float phase = (2 * _WaveSpeed) / _WaveLength;

			// Q
			float pinch = _WaveSteepness / (freq * _Amplitude);

			// D
			float dir = dot(worldPos.xz, _WaveDirection.xz);

			float X = worldPos.x + (pinch * _Amplitude) * (_WaveDirection.x * cos(freq * dir + phase * _Time.y));
			float Z = worldPos.z + (pinch * _Amplitude) * (_WaveDirection.z * cos(freq * dir + phase * _Time.y));
			float Y = _Amplitude * sin(freq * dir + phase * _Time.y);


			return float3(X, Y, Z);
		}



		float3 gerstnerWave2(float3 worldPos, float _WaveLength, float _Amplitude, float3 _WaveSpeed, float3 _WaveDirection, float _WaveSteepness)
		{

			_CurrentVertexPos = worldPos;
			float magnitude = (2 * 3.1416) / _WaveLength;
			float3 windDirection = _WaveDirection;
			float oceanDepth = 10;
			float freq =  sqrt(9.8 * magnitude); // sqrt(9.8 * magnitude) * tanh(magnitude * oceanDepth);// 9.8 gravitational pull, deep water sim
			float phase = (2 * _WaveSpeed) / _WaveLength;


			float3 windDirection1 = float3(0, 0, 1);
			float3 windDirection2 = float3(1, 0, 0);


			/* Amplitudes */
			float amplitudeX1 = 0.25;
			float amplitudeX2 = 0.25;

			float amplitudeY1 = 1;
			float amplitudeY2 = 1.5;

			float amplitudeZ1 = 0.1;
			float amplitudeZ2 = 1;
			// ----------------------


			/* Wave Length */
			float waveLengthX1 = 25;
			float waveLengthX2 = 50;
						    	   
			float waveLengthY1 = 1;
			float waveLengthY2 = 1;
						    	   
			float waveLengthZ1 = 10;
			float waveLengthZ2 = 25;
			// ----------------------


			/* Magnitudes */
			float magnitudeX1 = (2 * 3.1416) / waveLengthX1;
			float magnitudeX2 = (2 * 3.1416) / waveLengthX2;
						 
			float magnitudeY1 = (2 * 3.1416) / waveLengthY1;
			float magnitudeY2 = (2 * 3.1416) / waveLengthY2;
						   
			float magnitudeZ1 = (2 * 3.1416) / waveLengthZ1;
			float magnitudeZ2 = (2 * 3.1416) / waveLengthZ2;
			// ----------------------



			/* Frequencis */
			float freqX1 = sqrt(9.8 * magnitudeX1);
			float freqX2 = sqrt(9.8 * magnitudeX2);

			float freqY1 = sqrt(9.8 * magnitudeY1);
			float freqY2 = sqrt(9.8 * magnitudeY2);

			float freqZ1 = sqrt(9.8 * magnitudeZ1);
			float freqZ2 = sqrt(9.8 * magnitudeZ2);
			// ----------------------


			/* Gerstner Calculations */
			float waveX1 = (windDirection1 / magnitudeX1) * amplitudeX1 * sin(dot(windDirection1, worldPos.x) - (freqX1 * _Time.x));
			float waveX2 = (windDirection2 / magnitudeX2) * amplitudeX2 * sin(dot(windDirection2, worldPos.x) - (freqX2 * _Time.x));


			float waveY1 = amplitudeY1 * cos(dot(windDirection1.xz, worldPos.xz) - (freqY1 - _Time.y));
			float waveY2 = amplitudeY2 * cos(dot(windDirection2.xz, worldPos.xz) - (freqY2 - _Time.y));

			float waveZ1 = (windDirection1 / magnitudeZ1) * amplitudeZ1 * sin(dot(windDirection1, worldPos.z) - (freqZ1 * _Time.x));
			float waveZ2 = (windDirection2 / magnitudeZ2) * amplitudeZ2 * sin(dot(windDirection2, worldPos.z) - (freqZ2 * _Time.x));
			// ----------------------


			/* Totals */
			float totalX = waveX1 * waveX2;
			float totalY = waveY1 * waveY2;
			float totalZ = waveZ1 * waveZ2;
			// ----------------------

			/* Set values */
			float X = worldPos.x - totalX;
			float Y = totalY;
			float Z = worldPos.z - totalZ;
			// ----------------------



			// (windDirection / magnitude) * _Amplitude * sin(dot(windDirection, worldPos.x) - (freq * _Time.x) + phase);
			// _Amplitude * cos(dot(windDirection.xz, worldPos.xz) - (freq - _Time.y) + phase);
			// (windDirection / magnitude) * _Amplitude * sin(dot(windDirection, worldPos.z) - (freq * _Time.x) + phase);


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
			
			//_MainWave_Direction = _WindDirection;
			//float3 worldPosMain = gerstnerWave(worldPos.xyz, _MainWave_Length, _MainWave_Amplitude, _MainWave_Speed, _MainWave_Direction, _MainWave_Steepness);
			//float3 xVectorMain  = gerstnerWave(xVector, _MainWave_Length, _MainWave_Amplitude, _MainWave_Speed, _MainWave_Direction, _MainWave_Steepness);
			//float3 zVectorMain  = gerstnerWave(zVector, _MainWave_Length, _MainWave_Amplitude, _MainWave_Speed, _MainWave_Direction, _MainWave_Steepness);

			// New Genster Wave
			float3 worldPosMain = gerstnerWave2(worldPos.xyz, _MainWave_Length, _MainWave_Amplitude, _MainWave_Speed, _MainWave_Direction, _MainWave_Steepness);
			float3 xVectorMain = gerstnerWave2(xVector, _MainWave_Length, _MainWave_Amplitude, _MainWave_Speed, _MainWave_Direction, _MainWave_Steepness);
			float3 zVectorMain = gerstnerWave2(zVector, _MainWave_Length, _MainWave_Amplitude, _MainWave_Speed, _MainWave_Direction, _MainWave_Steepness);


			//_SecondaryWave_Direction = float3(0.1, 0, 0.5);
			//float3 worldPosSecondary = gerstnerWave(worldPos.xyz, _SecondaryWave_Length, _SecondaryWave_Amplitude  * 0.5f, _SecondaryWave_Speed, _SecondaryWave_Direction, _SecondaryWave_Steepness);
			//float3 xVectorSecondary  = gerstnerWave(xVector,      _SecondaryWave_Length, _SecondaryWave_Amplitude  * 0.5f, _SecondaryWave_Speed, _SecondaryWave_Direction, _SecondaryWave_Steepness);
			//float3 zVectorSecondary  = gerstnerWave(zVector,      _SecondaryWave_Length, _SecondaryWave_Amplitude  * 0.5f, _SecondaryWave_Speed, _SecondaryWave_Direction, _SecondaryWave_Steepness);


			// Anything done to the worldPos vertice has to be done to the friend vectors.
			worldPos.xyz += worldPosMain; //+ worldPosSecondary;
			xVector      += xVectorMain ; //+ xVectorSecondary ;
			zVector      += zVectorMain ; //+ zVectorSecondary ;
										 

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




