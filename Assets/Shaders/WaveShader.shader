Shader "Custom/WaveShader" 
{
	Properties 
	{
		// Predefined properties.
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0

		// Wave Properties
	}

	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Standard vertex:vert fullforwardshadows
		#pragma target 4.0


		// HLSL Predefined variables.
		sampler2D _MainTex;
		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		// Waves variables


		struct Input 
		{
			float2 uv_MainTex;
		};


		// this doesn't seem to be needed at the moment... the normals are updating anyway.
		void recalculateNormals(in appdata_full data)
		{
			float3 currentVertex = mul(float4x4(unity_ObjectToWorld), data.vertex).xyz;

			// create fake vertices around current vertex.
			float3 xFriend = currentVertex + float3(0.05, 0, 0);
			float3 zFriend = currentVertex + float3(0, 0, 0.05);

			// calculate the direction of its normal.
			float3 vertexNormalLocal = cross(xFriend - currentVertex, zFriend - currentVertex);
			float3 vertexNormalWorld = mul(float4x4(unity_WorldToObject), vertexNormalLocal);
			data.normal = normalize(vertexNormalWorld);

			data.vertex.xyz = mul(float4x4(unity_WorldToObject), currentVertex);
		}


		void vert(inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);

			// TEST
			v.vertex.y += sin(5 + v.vertex.x * _Time.w);

			//recalculateNormals(v);
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
