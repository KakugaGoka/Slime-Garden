#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
#pragma multi_compile _ _SHADOWS_SOFT
#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE


void MainLighting_float(float3 WorldPos, out half3 Direction, out half3 Color, out half DistanceAtten, out half ShadowAtten)
{
#if SHADERGRAPH_PREVIEW
   Direction = half3(0, 1, 0);
   Color = 1;
   DistanceAtten = 1;
   ShadowAtten = 1;

#else
   half4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
   
   Light mainLight = GetMainLight(shadowCoord);
   Direction = mainLight.direction;
   Color = mainLight.color;
   DistanceAtten = mainLight.distanceAttenuation;
   ShadowAtten = mainLight.shadowAttenuation;
#endif
}

void DirectSpecular_float(half3 Specular, half Smoothness, half3 Direction, half3 Color, half3 WorldNormal, half3 WorldView, out half3 Out)
{
#if SHADERGRAPH_PREVIEW
   Out = 0;
#else
   Smoothness = exp2(10 * Smoothness + 1);
   WorldNormal = normalize(WorldNormal);
   WorldView = SafeNormalize(WorldView);
   Out = LightingSpecular(Color, Direction, WorldNormal, WorldView, half4(Specular, 0), Smoothness);
   // float3 halfwayDir = normalize(Direction + WorldView);
   // float spec = pow(max(dot(WorldNormal, halfwayDir), 0.0), Smoothness);
   // Out = Color * spec;
   
#endif
}

void AdditionalLights_float(half3 SpecColor, half Smoothness, half3 WorldPosition, half3 WorldNormal, half3 WorldView, out half3 Diffuse, out half3 Specular)
{
   half3 diffuseColor = 0;
   half3 specularColor = 0;
 
#ifndef SHADERGRAPH_PREVIEW
   Smoothness = exp2(10 * Smoothness + 1);
   WorldNormal = normalize(WorldNormal);
   WorldView = SafeNormalize(WorldView);
   int pixelLightCount = GetAdditionalLightsCount();
   for (int i = 0; i < pixelLightCount; ++i)
   {
       Light light = GetAdditionalLight(i, WorldPosition);
       half3 attenuatedLightColor = light.color * (light.distanceAttenuation * light.shadowAttenuation);
       diffuseColor += LightingLambert(attenuatedLightColor, light.direction, WorldNormal);
       specularColor += LightingSpecular(attenuatedLightColor, light.direction, WorldNormal, WorldView, half4(SpecColor, 0), Smoothness);
   }
#endif
 
   Diffuse = diffuseColor;
   Specular = specularColor;
}

void GetLights_float(half3 WorldPosition, out half3 Light1Direction, out half3 Light1Color,
                                                   out half3 Light2Direction, out half3 Light2Color,
                                                    out half3 Light3Direction, out half3 Light3Color,
                                                     out half3 Light4Direction, out half3 Light4Color )
{
         Light1Direction = half3(0,0,0);
          Light1Color = half3(0,0,0);
         Light2Direction = half3(0,0,0);
          Light2Color = half3(0,0,0);
         Light3Direction = half3(0,0,0);
          Light3Color = half3(0,0,0);
         Light4Direction = half3(0,0,0);
          Light4Color = half3(0,0,0);
#ifndef SHADERGRAPH_PREVIEW
   int pixelLightCount = GetAdditionalLightsCount();
   bool next = pixelLightCount>=0;
   Light light;
   #ifndef next
           light = GetAdditionalLight(0, WorldPosition);
          Light1Direction = _AdditionalLightsPosition[0]- WorldPosition;


          Light1Color = light.color; 
   #else
          
          Light1Direction = half3(0,0,0);
          Light1Color = half3(0,0,0);
   #endif
       next = pixelLightCount>=1;
      #ifndef next
          light = GetAdditionalLight(1, WorldPosition);
          Light2Direction = _AdditionalLightsPosition[1]- WorldPosition;
          Light2Color = light.color; 
   #else
          
          Light2Direction = half3(0,0,0);
          Light2Color = half3(0,0,0);
   #endif
       next = pixelLightCount>=2;
      #ifndef next
                 light = GetAdditionalLight(2, WorldPosition);
          Light3Direction = _AdditionalLightsPosition[2]- WorldPosition;
          Light3Color = light.color; 
   #else
          
          Light3Direction = half3(0,0,0);
          Light3Color = half3(0,0,0);
   #endif
       next = pixelLightCount>=3;
   #ifndef next
       light = GetAdditionalLight(4, WorldPosition);
          Light4Direction = _AdditionalLightsPosition[3]- WorldPosition;
          Light4Color = light.color; 
   #else
          
          Light4Direction = half3(0,0,0);
          Light4Color = half3(0,0,0);
   #endif
#endif
 
}

void LightTest_float(float3 WorldPos, out half3 Direction, out half3 Color, out half DistanceAtten, out half ShadowAtten)
{
    #if SHADERGRAPH_PREVIEW
        Direction = half3(0.5, 0.5, 0);
        Color = 1;
        DistanceAtten = 1;
        ShadowAtten = 1;
    #else

      half4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
      Light mainLight = GetMainLight(shadowCoord);
      Direction = normalize(mainLight.direction);
      Color = mainLight.color;
      DistanceAtten = mainLight.distanceAttenuation;
   #if defined(_RECEIVE_SHADOWS_OFF)
      ShadowAtten = 1.0h;
   #endif

      ShadowSamplingData shadowSamplingData = GetMainLightShadowSamplingData();
      half shadowStrength = GetMainLightShadowStrength();
      ShadowAtten = SampleShadowmap(shadowCoord, TEXTURE2D_ARGS(_MainLightShadowmapTexture,
      sampler_MainLightShadowmapTexture),
      shadowSamplingData, shadowStrength, false);
    #endif
}

#endif

