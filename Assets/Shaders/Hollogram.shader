/*
	Written in Shaderlab language. Telling Unity how to render things.



*/
// New shader.. .this is a folder and file name ie could change to Unlit/Custom/Hollogram
Shader "Unlit/Hollogram"
{
	/*
		- Hold public variables in unity. 
	*/
	Properties
	{
		_MainTex ("Albedo Texture", 2D) = "white" {}   // '{}' are left over from legacy shader code. 
		_TintColour("Tint Colour", Color) = (1, 1, 1, 1)
		_Transparency("Transparency", Range(0.0, 0.5)) = 0.25
		_CutoutThresh("Cutout Threshold", Range(0.0, 1.0)) = 0.2
		_Distance("Distance", Float) = 1
		_Amplitude("Amplitude", Float) = 1
		_Speed("Speed", Float) = 1
		_Amount("Amount", Float) = 1
	}
	/*
		- Contains instructs for Unity aboout to set up the Renderer.
		- Contains the 'Pass' this single draw call instruction to the GPU. 
		- Can haver multiple SubShaders. Maybe multiple for different platform builds.
	*/
	SubShader
	{
		// Talk to unity renderer
		// Tags { "RenderType"="Opaque" } 

		// change the render order so thing behind the object are rendered first.
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		// set level of detail and change how shader behaves based on LOD
		LOD 100

		// Tell us not to write to the depth buffer.
		zWrite Off

		Blend SrcAlpha OneMinusSrcAlpha
		
		Pass
		{
			// This is telling it to run on GPU
			CGPROGRAM 
			
			// Preprocessor Directives functions.
			#pragma vertex vert
			#pragma fragment frag
			
			// Shader classes dont use Inheritance. Here we call the things we need at compile time.
			#include "UnityCG.cginc"


			/*
				- pass this to two functions 'vert' and 'frag'
			*/
			struct appdata
			{
				float4 vertex : POSITION;  // Info about vertices of the 3D model; passed in with a 'Packed Array'. ':POSITION' is a semantic binding for positin in local space. 
				float2 uv : TEXCOORD0;	   // uv coordinates. 
			};

			struct v2f // (vert2frag)
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;  // ': SV_POSITION' is a semantic binding for screen space pos.  
			};


			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _TintColour;
			float _Transparency;
			float _CutoutThresh;
			float _Distance; 
			float _Amplitude;
			float _Speed;
			float _Amount;


			/*
				- Vertex Function (vertexshader): Takes the shape of the model, potentially modifies it. 
				- Can modify the positions and vertices in the vertex function before being passed onto the fragment function. 
				- Could remove the struct appdata and pass it in as v2f vert(float4 vertex, float2 uv) {}
			*/
			v2f vert (appdata v)
			{
				// create new v2f struct.
				v2f o;	

				v.vertex.y += _Amplitude * sin(_Time.y * _Speed + v.vertex.x) * _Distance * _Amount;

				// - v.vertex is the model in vertex space.
				// - Take vertex from model into clip space.
				o.vertex = UnityObjectToClipPos(v.vertex); 

				// take uv data from model and take data from main texture. 
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				// return the struct. 
				return o;
				
			}
			
			/*
				- Fragment Function (fragment shader): Applies color to the shape output by the vert funcion
				- takes data from vert and paints in the pixels.
				- Frag and Vert can pull in data from the Properties Data
			*/
			fixed4 frag (v2f i) : SV_Target // 'SV_Target' - This is going to output to a render target. 
			{
				// sample the texture
				// Read in colour from main texture property. 
				fixed4 col = tex2D(_MainTex, i.uv) + _TintColour;
				col.a = _Transparency;
				clip(col.r - _CutoutThresh); // discard certain pixel data. if(col.r < cutoutThresh) discard; clip does this. 
				return col;
			}
			ENDCG
		}
	}
}
