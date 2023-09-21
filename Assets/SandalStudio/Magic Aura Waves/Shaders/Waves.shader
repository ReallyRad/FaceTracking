Shader "SandalStudio/Waves"
{
	Properties
	{
		_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
		_TextureSample0("Texture 1", 2D) = "white" {}
		_TextureSample1("Texture 2", 2D) = "white" {}
		_Emissive("Emissive", Float) = 1
		_Power("Power", Range( 0 , 15)) = 1
		_Alpha("Alpha", Float) = 1
		[HDR]_Color(" Color", Color) = (1,1,1,1)
		_Test("Size Noise", Vector) = (1,1,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}


	Category 
	{
		SubShader
		{
		LOD 0

			Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
			Blend SrcAlpha One
			ColorMask RGB
			Cull Off
			Lighting Off 
			ZWrite Off
			ZTest LEqual
			
			Pass {
			
				CGPROGRAM
				
				#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
				#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
				#endif
				
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 2.0
				#pragma multi_compile_instancing
				#pragma multi_compile_particles
				#pragma multi_compile_fog
				#define ASE_NEEDS_FRAG_COLOR


				#include "UnityCG.cginc"

				struct appdata_t 
				{
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float4 texcoord : TEXCOORD0;
					UNITY_VERTEX_INPUT_INSTANCE_ID
					
				};

				struct v2f 
				{
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR;
					float4 texcoord : TEXCOORD0;
					UNITY_FOG_COORDS(1)
					#ifdef SOFTPARTICLES_ON
					float4 projPos : TEXCOORD2;
					#endif
					UNITY_VERTEX_INPUT_INSTANCE_ID
					UNITY_VERTEX_OUTPUT_STEREO
					
				};
				
				
				#if UNITY_VERSION >= 560
				UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
				#else
				uniform sampler2D_float _CameraDepthTexture;
				#endif

				//Don't delete this comment
				// uniform sampler2D_float _CameraDepthTexture;

				uniform sampler2D _MainTex;
				uniform fixed4 _TintColor;
				uniform float4 _MainTex_ST;
				uniform float _InvFade;
				uniform sampler2D _TextureSample0;
				uniform float2 _Test;
				uniform float _Emissive;
				uniform float4 _Color;
				uniform sampler2D _TextureSample1;
				uniform float4 _TextureSample1_ST;
				uniform float _Power;
				uniform float _Alpha;


				v2f vert ( appdata_t v  )
				{
					v2f o;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
					UNITY_TRANSFER_INSTANCE_ID(v, o);
					

					v.vertex.xyz +=  float3( 0, 0, 0 ) ;
					o.vertex = UnityObjectToClipPos(v.vertex);
					#ifdef SOFTPARTICLES_ON
						o.projPos = ComputeScreenPos (o.vertex);
						COMPUTE_EYEDEPTH(o.projPos.z);
					#endif
					o.color = v.color;
					o.texcoord = v.texcoord;
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				fixed4 frag ( v2f i  ) : SV_Target
				{
					UNITY_SETUP_INSTANCE_ID( i );
					UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( i );

					#ifdef SOFTPARTICLES_ON
						float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
						float partZ = i.projPos.z;
						float fade = saturate (_InvFade * (sceneZ-partZ));
						i.color.a *= fade;
					#endif

					float4 texCoord7 = i.texcoord;
					texCoord7.xy = i.texcoord.xy * _Test + float2( 0,0 );
					float2 appendResult38 = (float2(texCoord7.z , texCoord7.w));
					float2 temp_output_9_0 = ( (texCoord7).xy + appendResult38 );
					float4 tex2DNode5 = tex2D( _TextureSample0, temp_output_9_0 );
					float2 uv_TextureSample1 = i.texcoord.xy * _TextureSample1_ST.xy + _TextureSample1_ST.zw;
					float4 tex2DNode11 = tex2D( _TextureSample1, uv_TextureSample1 );
					float2 break12 = temp_output_9_0;
					float ifLocalVar15 = 0;
					if( 1.0 == ceil( break12.x ) )
					ifLocalVar15 = 0.0;
					else
					ifLocalVar15 = 1.0;
					float ifLocalVar16 = 0;
					if( 1.0 == ceil( break12.y ) )
					ifLocalVar16 = 0.0;
					else
					ifLocalVar16 = 1.0;
					float4 appendResult3 = (float4(( tex2DNode5.r * i.color * _Emissive * _Color * tex2DNode11.r ).rgb , saturate( ( ( pow( tex2DNode5.b , _Power ) * i.color.a * ( 1.0 - max( ifLocalVar15 , ifLocalVar16 ) ) ) * _Alpha ) )));
					

					fixed4 col = appendResult3;
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG 
			}
		}	
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
