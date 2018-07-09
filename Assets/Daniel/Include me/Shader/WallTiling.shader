// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Custom/WallTiling"
{
    Properties
    {
	
		[ToggleOff] _WallTiling("enable Wall Tiling", Float) = 0.0
		_TextureScale("TextureScaleSize", Range(0.000001, 50)) = 1
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo", 2D) = "white" {}

        _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

        _Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
        _GlossMapScale("Smoothness Scale", Range(0.0, 1.0)) = 1.0
        [Enum(Metallic Alpha,0,Albedo Alpha,1)] _SmoothnessTextureChannel ("Smoothness texture channel", Float) = 0

        [Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
        _MetallicGlossMap("Metallic", 2D) = "white" {}

        [ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
        [ToggleOff] _GlossyReflections("Glossy Reflections", Float) = 1.0

        _BumpScale("Scale", Float) = 1.0
        _BumpMap("Normal Map", 2D) = "bump" {}

        _Parallax ("Height Scale", Range (0.005, 0.08)) = 0.02
        _ParallaxMap ("Height Map", 2D) = "black" {}

        _OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
        _OcclusionMap("Occlusion", 2D) = "white" {}

        _EmissionColor("Color", Color) = (0,0,0)
        _EmissionMap("Emission", 2D) = "white" {}

        _DetailMask("Detail Mask", 2D) = "white" {}

        _DetailAlbedoMap("Detail Albedo x2", 2D) = "grey" {}
        _DetailNormalMapScale("Scale", Float) = 1.0
        _DetailNormalMap("Normal Map", 2D) = "bump" {}

        [Enum(UV0,0,UV1,1)] _UVSec ("UV Set for secondary textures", Float) = 0


        // Blending state
        [HideInInspector] _Mode ("__mode", Float) = 0.0
        [HideInInspector] _SrcBlend ("__src", Float) = 1.0
        [HideInInspector] _DstBlend ("__dst", Float) = 0.0
        [HideInInspector] _ZWrite ("__zw", Float) = 1.0
    }

    CGINCLUDE
        #define UNITY_SETUP_BRDF_INPUT MetallicSetup
    ENDCG

    SubShader
    {
        Tags { "RenderType"="Opaque" "PerformanceChecks"="False" }
        LOD 300


        // ------------------------------------------------------------------
        //  Base forward pass (directional light, emission, lightmaps, ...)
        Pass
        {
            Name "FORWARD"
            Tags { "LightMode" = "ForwardBase" }

            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]

            CGPROGRAM
            #pragma target 3.0

            // -------------------------------------

            #pragma shader_feature _NORMALMAP
            #pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _EMISSION
            #pragma shader_feature _METALLICGLOSSMAP
            #pragma shader_feature ___ _DETAIL_MULX2
            #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature _ _GLOSSYREFLECTIONS_OFF
            #pragma shader_feature _PARALLAXMAP

            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            // Uncomment the following line to enable dithering LOD crossfade. Note: there are more in the file to uncomment for other passes.
            //#pragma multi_compile _ LOD_FADE_CROSSFADE
			
			float _WallTiling;
			float _TextureScale;
            #pragma vertex myVertBase
			            #include "UnityStandardCore.cginc"
			//#include "UnityCG.cginc"
			#define TRANSFORM_TEX(tex,name) (tex.xy * name##_ST.xy + name##_ST.zw)
			#ifdef LOD_FADE_CROSSFADE
			#define UNITY_APPLY_DITHER_CROSSFADE(vpos)  UnityApplyDitherCrossFade(vpos)
			sampler2D _DitherMaskLOD2D;
			void UnityApplyDitherCrossFade(float2 vpos)
			{
				vpos /= 4; // the dither mask texture is 4x4
				vpos.y = frac(vpos.y) * 0.0625 /* 1/16 */ + unity_LODFade.y; // quantized lod fade by 16 levels
				clip(tex2D(_DitherMaskLOD2D, vpos).a - 0.5);
			}
			#else
				#define UNITY_APPLY_DITHER_CROSSFADE(vpos)
			#endif


			float4 MyTexCoords(VertexInput v, float4 posWorld)
			{
				float4 texcoord;
				float2 newUV = float2(0,0);
				#ifdef _TANGENT_TO_WORLD
					float4 newTangent = normalize(v.tangent);
					newTangent.w = 0;
					newTangent = mul(unity_ObjectToWorld, newTangent);
					newTangent = normalize(newTangent);

					float4 newNormal = float4(normalize(v.normal), 0);
					newNormal.w = 0;
					newNormal = mul(unity_ObjectToWorld, newNormal);
					newNormal = normalize(newNormal);

					newUV.x = (dot (newTangent.xyz, posWorld.xyz) ) / _TextureScale;
					newUV.y = (dot (cross(newTangent.xyz, newNormal.xyz), posWorld.xyz)) / _TextureScale;
										
										
				#else
					newUV.x = (posWorld.x + posWorld.z) / _TextureScale;
					newUV.y = (posWorld.y + posWorld.z) / _TextureScale;
				#endif
				//texcoord.xy = TRANSFORM_TEX(newUV _MainTex).xy; // Always source from uv0
				texcoord.zw = TRANSFORM_TEX(((_UVSec == 0) ? newUV : v.uv1), _DetailAlbedoMap);
				texcoord.xy = newUV;
				//texcoord.zw = newUV;
				return texcoord;
			}
			VertexOutputForwardBase myVertBase (VertexInput v)
			{
				UNITY_SETUP_INSTANCE_ID(v);
				VertexOutputForwardBase o;
				UNITY_INITIALIZE_OUTPUT(VertexOutputForwardBase, o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float4 posWorld = mul(unity_ObjectToWorld, v.vertex);
				#if UNITY_REQUIRE_FRAG_WORLDPOS
					#if UNITY_PACK_WORLDPOS_WITH_TANGENT
						o.tangentToWorldAndPackedData[0].w = posWorld.x;
						o.tangentToWorldAndPackedData[1].w = posWorld.y;
						o.tangentToWorldAndPackedData[2].w = posWorld.z;
					#else
						o.posWorld = posWorld.xyz;
					#endif
				#endif
				o.pos = UnityObjectToClipPos(v.vertex);

				if(_WallTiling == 0)
					o.tex = TexCoords(v);
				else
					o.tex = MyTexCoords(v, posWorld);
				o.eyeVec = NormalizePerVertexNormal(posWorld.xyz - _WorldSpaceCameraPos);
				float3 normalWorld = UnityObjectToWorldNormal(v.normal);
				#ifdef _TANGENT_TO_WORLD
					float4 tangentWorld = float4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);

					float3x3 tangentToWorld = CreateTangentToWorldPerVertex(normalWorld, tangentWorld.xyz, tangentWorld.w);
					o.tangentToWorldAndPackedData[0].xyz = tangentToWorld[0];
					o.tangentToWorldAndPackedData[1].xyz = tangentToWorld[1];
					o.tangentToWorldAndPackedData[2].xyz = tangentToWorld[2];
				#else
					o.tangentToWorldAndPackedData[0].xyz = 0;
					o.tangentToWorldAndPackedData[1].xyz = 0;
					o.tangentToWorldAndPackedData[2].xyz = normalWorld;
				#endif

				//We need this for shadow receving
				UNITY_TRANSFER_SHADOW(o, v.uv1);

				o.ambientOrLightmapUV = VertexGIForward(v, posWorld, normalWorld);

				#ifdef _PARALLAXMAP
					TANGENT_SPACE_ROTATION;
					half3 viewDirForParallax = mul (rotation, ObjSpaceViewDir(v.vertex));
					o.tangentToWorldAndPackedData[0].w = viewDirForParallax.x;
					o.tangentToWorldAndPackedData[1].w = viewDirForParallax.y;
					o.tangentToWorldAndPackedData[2].w = viewDirForParallax.z;
				#endif

				UNITY_TRANSFER_FOG(o,o.pos);
				return o;
			}

            #pragma fragment fragBase

            #include "UnityStandardCoreForward.cginc"

            ENDCG
        }
        // ------------------------------------------------------------------
        //  Additive forward pass (one light per pass)
        Pass
        {
            Name "FORWARD_DELTA"
            Tags { "LightMode" = "ForwardAdd" }
            Blend [_SrcBlend] One
            Fog { Color (0,0,0,0) } // in additive pass fog should be black
            ZWrite Off
            ZTest LEqual

            CGPROGRAM
            #pragma target 3.0

            // -------------------------------------


            #pragma shader_feature _NORMALMAP
            #pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _METALLICGLOSSMAP
            #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature ___ _DETAIL_MULX2
            #pragma shader_feature _PARALLAXMAP

            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog
            // Uncomment the following line to enable dithering LOD crossfade. Note: there are more in the file to uncomment for other passes.
            //#pragma multi_compile _ LOD_FADE_CROSSFADE

            #pragma vertex myVertAdd
            #pragma fragment fragAdd

			float _WallTiling;
			float _TextureScale;
			            #include "UnityStandardCore.cginc"
			//#include "UnityCG.cginc"
			#define TRANSFORM_TEX(tex,name) (tex.xy * name##_ST.xy + name##_ST.zw)
			#ifdef LOD_FADE_CROSSFADE
			#define UNITY_APPLY_DITHER_CROSSFADE(vpos)  UnityApplyDitherCrossFade(vpos)
			sampler2D _DitherMaskLOD2D;
			void UnityApplyDitherCrossFade(float2 vpos)
			{
				vpos /= 4; // the dither mask texture is 4x4
				vpos.y = frac(vpos.y) * 0.0625 /* 1/16 */ + unity_LODFade.y; // quantized lod fade by 16 levels
				clip(tex2D(_DitherMaskLOD2D, vpos).a - 0.5);
			}
			#else
				#define UNITY_APPLY_DITHER_CROSSFADE(vpos)
			#endif


			float4 MyTexCoords(VertexInput v, float4 posWorld)
			{
				float4 texcoord;
				float2 newUV = float2(0,0);
				#ifdef _TANGENT_TO_WORLD
					float4 newTangent = normalize(v.tangent);
					newTangent.w = 0;
					newTangent = mul(unity_ObjectToWorld, newTangent);
					newTangent = normalize(newTangent);

					float4 newNormal = float4(normalize(v.normal), 0);
					newNormal.w = 0;
					newNormal = mul(unity_ObjectToWorld, newNormal);
					newNormal = normalize(newNormal);

					newUV.x = (dot (newTangent.xyz, posWorld.xyz) ) / _TextureScale;
					newUV.y = (dot (cross(newTangent.xyz, newNormal.xyz), posWorld.xyz)) / _TextureScale;
										
										
				#else
					newUV.x = (posWorld.x + posWorld.z) / _TextureScale;
					newUV.y = (posWorld.y + posWorld.z) / _TextureScale;
				#endif
				//texcoord.xy = TRANSFORM_TEX(newUV _MainTex).xy; // Always source from uv0
				texcoord.zw = TRANSFORM_TEX(((_UVSec == 0) ? newUV : v.uv1), _DetailAlbedoMap);
				texcoord.xy = newUV;
				//texcoord.zw = newUV;
				return texcoord;
			}
			VertexOutputForwardAdd myVertAdd (VertexInput v)
			{
				UNITY_SETUP_INSTANCE_ID(v);
				VertexOutputForwardAdd o;
				UNITY_INITIALIZE_OUTPUT(VertexOutputForwardAdd, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float4 posWorld = mul(unity_ObjectToWorld, v.vertex);
				o.pos = UnityObjectToClipPos(v.vertex);

				if(_WallTiling == 0)
					o.tex = TexCoords(v);
				else
					o.tex = MyTexCoords(v, posWorld);
				o.eyeVec = NormalizePerVertexNormal(posWorld.xyz - _WorldSpaceCameraPos);
				o.posWorld = posWorld.xyz;
				float3 normalWorld = UnityObjectToWorldNormal(v.normal);
				#ifdef _TANGENT_TO_WORLD
					float4 tangentWorld = float4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);

					float3x3 tangentToWorld = CreateTangentToWorldPerVertex(normalWorld, tangentWorld.xyz, tangentWorld.w);
					o.tangentToWorldAndLightDir[0].xyz = tangentToWorld[0];
					o.tangentToWorldAndLightDir[1].xyz = tangentToWorld[1];
					o.tangentToWorldAndLightDir[2].xyz = tangentToWorld[2];
				#else
					o.tangentToWorldAndLightDir[0].xyz = 0;
					o.tangentToWorldAndLightDir[1].xyz = 0;
					o.tangentToWorldAndLightDir[2].xyz = normalWorld;
				#endif
				//We need this for shadow receiving
				UNITY_TRANSFER_SHADOW(o, v.uv1);

				float3 lightDir = _WorldSpaceLightPos0.xyz - posWorld.xyz * _WorldSpaceLightPos0.w;
				#ifndef USING_DIRECTIONAL_LIGHT
					lightDir = NormalizePerVertexNormal(lightDir);
				#endif
				o.tangentToWorldAndLightDir[0].w = lightDir.x;
				o.tangentToWorldAndLightDir[1].w = lightDir.y;
				o.tangentToWorldAndLightDir[2].w = lightDir.z;

				#ifdef _PARALLAXMAP
					TANGENT_SPACE_ROTATION;
					o.viewDirForParallax = mul (rotation, ObjSpaceViewDir(v.vertex));
				#endif

				UNITY_TRANSFER_FOG(o,o.pos);
				return o;
			}
            #include "UnityStandardCoreForward.cginc"

            ENDCG
        }
        // ------------------------------------------------------------------
        //  Shadow rendering pass
        Pass {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            ZWrite On ZTest LEqual

            CGPROGRAM
            #pragma target 3.0

            // -------------------------------------


            #pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _METALLICGLOSSMAP
            #pragma shader_feature _PARALLAXMAP
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_instancing
            // Uncomment the following line to enable dithering LOD crossfade. Note: there are more in the file to uncomment for other passes.
            //#pragma multi_compile _ LOD_FADE_CROSSFADE

            #pragma vertex vertShadowCaster
            #pragma fragment fragShadowCaster

            #include "UnityStandardShadow.cginc"

            ENDCG
        }
        // ------------------------------------------------------------------
        //  Deferred pass
        Pass
        {
            Name "DEFERRED"
            Tags { "LightMode" = "Deferred" }

            CGPROGRAM
            #pragma target 3.0
            #pragma exclude_renderers nomrt

            // -------------------------------------

            #pragma shader_feature _NORMALMAP
            #pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _EMISSION
            #pragma shader_feature _METALLICGLOSSMAP
            #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature ___ _DETAIL_MULX2
            #pragma shader_feature _PARALLAXMAP

            #pragma multi_compile_prepassfinal
            #pragma multi_compile_instancing
            // Uncomment the following line to enable dithering LOD crossfade. Note: there are more in the file to uncomment for other passes.
            //#pragma multi_compile _ LOD_FADE_CROSSFADE

            #pragma vertex vertDeferredo
			float _WallTiling;
			float _TextureScale;
            #include "UnityStandardCore.cginc"
			//#include "UnityCG.cginc"
			#define TRANSFORM_TEX(tex,name) (tex.xy * name##_ST.xy + name##_ST.zw)
			#ifdef LOD_FADE_CROSSFADE
			#define UNITY_APPLY_DITHER_CROSSFADE(vpos)  UnityApplyDitherCrossFade(vpos)
			sampler2D _DitherMaskLOD2D;
			void UnityApplyDitherCrossFade(float2 vpos)
			{
				vpos /= 4; // the dither mask texture is 4x4
				vpos.y = frac(vpos.y) * 0.0625 /* 1/16 */ + unity_LODFade.y; // quantized lod fade by 16 levels
				clip(tex2D(_DitherMaskLOD2D, vpos).a - 0.5);
			}
			#else
				#define UNITY_APPLY_DITHER_CROSSFADE(vpos)
			#endif


			float4 MyTexCoords(VertexInput v, float4 posWorld)
			{
				float4 texcoord;
				float2 newUV = float2(0,0);
				#ifdef _TANGENT_TO_WORLD
					float4 newTangent = normalize(v.tangent);
					newTangent.w = 0;
					newTangent = mul(unity_ObjectToWorld, newTangent);
					newTangent = normalize(newTangent);

					float4 newNormal = float4(normalize(v.normal), 0);
					newNormal.w = 0;
					newNormal = mul(unity_ObjectToWorld, newNormal);
					newNormal = normalize(newNormal);

					newUV.x = (dot (newTangent.xyz, posWorld.xyz) ) / _TextureScale;
					newUV.y = (dot (cross(newTangent.xyz, newNormal.xyz), posWorld.xyz)) / _TextureScale;
										
										
				#else
					newUV.x = (posWorld.x + posWorld.z) / _TextureScale;
					newUV.y = (posWorld.y + posWorld.z) / _TextureScale;
				#endif
				//texcoord.xy = TRANSFORM_TEX(newUV _MainTex).xy; // Always source from uv0
				texcoord.zw = TRANSFORM_TEX(((_UVSec == 0) ? newUV : v.uv1), _DetailAlbedoMap);
				texcoord.xy = newUV;
				//texcoord.zw = newUV;
				return texcoord;
			}
			VertexOutputDeferred vertDeferredo (VertexInput v)
            {
				UNITY_SETUP_INSTANCE_ID(v);
					VertexOutputDeferred o;
					UNITY_INITIALIZE_OUTPUT(VertexOutputDeferred, o);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

					float4 posWorld = mul(unity_ObjectToWorld, v.vertex);
					#if UNITY_REQUIRE_FRAG_WORLDPOS
						#if UNITY_PACK_WORLDPOS_WITH_TANGENT
							o.tangentToWorldAndPackedData[0].w = posWorld.x;
							o.tangentToWorldAndPackedData[1].w = posWorld.y;
							o.tangentToWorldAndPackedData[2].w = posWorld.z;
						#else
							o.posWorld = posWorld.xyz;
						#endif
					#endif
					o.pos = UnityObjectToClipPos(v.vertex);
					if(_WallTiling == 0)
						o.tex = TexCoords(v);
					else
						o.tex = MyTexCoords(v, posWorld);
					o.eyeVec = NormalizePerVertexNormal(posWorld.xyz - _WorldSpaceCameraPos);
					float3 normalWorld = UnityObjectToWorldNormal(v.normal);
					#ifdef _TANGENT_TO_WORLD
						float4 tangentWorld = float4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);

						float3x3 tangentToWorld = CreateTangentToWorldPerVertex(normalWorld, tangentWorld.xyz, tangentWorld.w);
						o.tangentToWorldAndPackedData[0].xyz = tangentToWorld[0];
						o.tangentToWorldAndPackedData[1].xyz = tangentToWorld[1];
						o.tangentToWorldAndPackedData[2].xyz = tangentToWorld[2];
					#else
						o.tangentToWorldAndPackedData[0].xyz = 0;
						o.tangentToWorldAndPackedData[1].xyz = 0;
						o.tangentToWorldAndPackedData[2].xyz = normalWorld;
					#endif

					o.ambientOrLightmapUV = 0;
					#ifdef LIGHTMAP_ON
						o.ambientOrLightmapUV.xy = v.uv1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
					#elif UNITY_SHOULD_SAMPLE_SH
						o.ambientOrLightmapUV.rgb = ShadeSHPerVertex (normalWorld, o.ambientOrLightmapUV.rgb);
					#endif
					#ifdef DYNAMICLIGHTMAP_ON
						o.ambientOrLightmapUV.zw = v.uv2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
					#endif

					#ifdef _PARALLAXMAP
						TANGENT_SPACE_ROTATION;
						half3 viewDirForParallax = mul (rotation, ObjSpaceViewDir(v.vertex));
						o.tangentToWorldAndPackedData[0].w = viewDirForParallax.x;
						o.tangentToWorldAndPackedData[1].w = viewDirForParallax.y;
						o.tangentToWorldAndPackedData[2].w = viewDirForParallax.z;
					#endif

					return o;
            }

            #pragma fragment fragDeferredo
			float4 _planePos;
			float4 _planeNorm;
			float _portalCam = 0;
			bool UsingFrag(VertexOutputDeferred i)
			{
				float planeDist = dot(_planeNorm.xyz, IN_WORLDPOS(i)) - dot(_planeNorm.xyz, _planePos.xyz);
				if(_portalCam == 0)
				{
					return true;
				}
				else if(planeDist >= 0.001)
					return true;
				return false;
			}


			void fragDeferredo (
				VertexOutputDeferred i,
				out half4 outGBuffer0 : SV_Target0,
				out half4 outGBuffer1 : SV_Target1,
				out half4 outGBuffer2 : SV_Target2,
				out half4 outEmission : SV_Target3          // RT3: emission (rgb), --unused-- (a)
			#if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
				,out half4 outShadowMask : SV_Target4       // RT4: shadowmask (rgba)
			#endif
			)
			{				 
				//#if (SHADER_TARGET < 30)
				//	outGBuffer0 = 1;
				//	outGBuffer1 = 1;
				//	outGBuffer2 = 0;
				//	outEmission = 0;
				//	#if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
				//		outShadowMask = 1;
				//	#endif
				//	return;
				//#endif
			 
				UNITY_APPLY_DITHER_CROSSFADE(i.pos.xy);
				
				FRAGMENT_SETUP(s)

			
				// no analytic lights in this pass
				UnityLight dummyLight = DummyLight ();
				half atten = 1;
			
			
				// only GI
				half occlusion = Occlusion(i.tex.xy);
				#if UNITY_ENABLE_REFLECTION_BUFFERS
					bool sampleReflectionsInDeferred = false;
				#else
					bool sampleReflectionsInDeferred = true;
				#endif
			
				UnityGI gi = FragmentGI (s, occlusion, i.ambientOrLightmapUV, atten, dummyLight, sampleReflectionsInDeferred);
			
				half3 emissiveColor = UNITY_BRDF_PBS (s.diffColor, s.specColor, s.oneMinusReflectivity, s.smoothness, s.normalWorld, -s.eyeVec, gi.light, gi.indirect).rgb;
			
				#ifdef _EMISSION
					emissiveColor += Emission (i.tex.xy);
				#endif
			
				#ifndef UNITY_HDR_ON
					emissiveColor.rgb = exp2(-emissiveColor.rgb);
				#endif
				UnityStandardData data;
				data.diffuseColor   = s.diffColor;
				data.occlusion      = occlusion;
				data.specularColor  = s.specColor;
				data.smoothness     = s.smoothness;
				data.normalWorld    = s.normalWorld;
			
				if(!UsingFrag(i))
					clip(-1);
				UnityStandardDataToGbuffer(data, outGBuffer0, outGBuffer1, outGBuffer2);
				// Emissive lighting buffer
				outEmission = half4(emissiveColor, 1);
			
				// Baked direct lighting occlusion if any
				#if defined(SHADOWS_SHADOWMASK) && (UNITY_ALLOWED_MRT_COUNT > 4)
					outShadowMask = UnityGetRawBakedOcclusions(i.ambientOrLightmapUV.xy, IN_WORLDPOS(i));
				#endif
			}	  


            ENDCG
        }

        // ------------------------------------------------------------------
        // Extracts information for lightmapping, GI (emission, albedo, ...)
        // This pass it not used during regular rendering.
        Pass
        {
            Name "META"
            Tags { "LightMode"="Meta" }

            Cull Off

            CGPROGRAM
            #pragma vertex vert_meta
            #pragma fragment frag_meta

            #pragma shader_feature _EMISSION
            #pragma shader_feature _METALLICGLOSSMAP
            #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature ___ _DETAIL_MULX2
            #pragma shader_feature EDITOR_VISUALIZATION

            #include "UnityStandardMeta.cginc"
            ENDCG
        }
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "PerformanceChecks"="False" }
        LOD 150

        // ------------------------------------------------------------------
        //  Base forward pass (directional light, emission, lightmaps, ...)
        Pass
        {
            Name "FORWARD"
            Tags { "LightMode" = "ForwardBase" }

            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]

            CGPROGRAM
            #pragma target 2.0

            #pragma shader_feature _NORMALMAP
            #pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _EMISSION
            #pragma shader_feature _METALLICGLOSSMAP
            #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature _ _GLOSSYREFLECTIONS_OFF
            // SM2.0: NOT SUPPORTED shader_feature ___ _DETAIL_MULX2
            // SM2.0: NOT SUPPORTED shader_feature _PARALLAXMAP

            #pragma skip_variants SHADOWS_SOFT DIRLIGHTMAP_COMBINED

            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog

            #pragma vertex vertBase
            #pragma fragment fragBase
            #include "UnityStandardCoreForward.cginc"

            ENDCG
        }
        // ------------------------------------------------------------------
        //  Additive forward pass (one light per pass)
        Pass
        {
            Name "FORWARD_DELTA"
            Tags { "LightMode" = "ForwardAdd" }
            Blend [_SrcBlend] One
            Fog { Color (0,0,0,0) } // in additive pass fog should be black
            ZWrite Off
            ZTest LEqual

            CGPROGRAM
            #pragma target 2.0

            #pragma shader_feature _NORMALMAP
            #pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _METALLICGLOSSMAP
            #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature ___ _DETAIL_MULX2
            // SM2.0: NOT SUPPORTED shader_feature _PARALLAXMAP
            #pragma skip_variants SHADOWS_SOFT

            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog

            #pragma vertex vertAdd
            #pragma fragment fragAdd
            #include "UnityStandardCoreForward.cginc"

            ENDCG
        }
        // ------------------------------------------------------------------
        //  Shadow rendering pass
        Pass {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            ZWrite On ZTest LEqual

            CGPROGRAM
            #pragma target 2.0

            #pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _METALLICGLOSSMAP
            #pragma skip_variants SHADOWS_SOFT
            #pragma multi_compile_shadowcaster

            #pragma vertex vertShadowCaster
            #pragma fragment fragShadowCaster

            #include "UnityStandardShadow.cginc"

            ENDCG
        }

        // ------------------------------------------------------------------
        // Extracts information for lightmapping, GI (emission, albedo, ...)
        // This pass it not used during regular rendering.
        Pass
        {
            Name "META"
            Tags { "LightMode"="Meta" }

            Cull Off

            CGPROGRAM
            #pragma vertex vert_meta
            #pragma fragment frag_meta

            #pragma shader_feature _EMISSION
            #pragma shader_feature _METALLICGLOSSMAP
            #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature ___ _DETAIL_MULX2
            #pragma shader_feature EDITOR_VISUALIZATION

            #include "UnityStandardMeta.cginc"
            ENDCG
        }
    }


    FallBack "VertexLit"
    CustomEditor "WallTilingShaderGUI"
}
