Shader "Custom/WaveSurface"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _WaveFrequencyX ("Wave Frequency X", Range(0, 40)) = 1.0
        _WaveFrequencyZ ("Wave Frequency Z", Range(0, 40)) = 1.0
        _WaveAmplitude ("Wave Amplitude", Range(0, 1)) = 0.5
        _SpeedX ("Wave Speed X", Range(0, 10)) = 1.0 // Speed for X axis
        _SpeedZ ("Wave Speed Z", Range(0, 10)) = 1.0 // Speed for Z axis
        _MainTex ("Texture", 2D) = "white" {}
        _TileSize ("Tile Size", Float) = 1.0 // Control how big the tiles are
        _OffsetSpeedX ("Offset Speed X", Range(-10, 10)) = 0.1 // New Speed for X texture offset
        _OffsetSpeedY ("Offset Speed Y", Range(-10, 10)) = 0.1 // New Speed for Y texture offset
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #pragma multi_compile_fog // Ensure fog support

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                UNITY_FOG_COORDS(1) // Add fog coordinates
            };

            float _WaveFrequencyX;
            float _WaveFrequencyZ;
            float _WaveAmplitude;
            float _SpeedX; // Speed for X-axis waves
            float _SpeedZ; // Speed for Z-axis waves
            float _TileSize; // Tiling size
            float _OffsetSpeedX; // Speed for texture offset in X axis
            float _OffsetSpeedY; // Speed for texture offset in Y axis
            fixed4 _Color;
            sampler2D _MainTex; // Texture sampler
            float4 _MainTex_ST; // Texture scale and offset

            v2f vert(appdata_t v)
            {
                v2f o;

                // Normalize the vertex position based on tile size
                float normalizedX = v.vertex.x / _TileSize;
                float normalizedZ = v.vertex.z / _TileSize;

                // Ensure the wave completes a full cycle over the tile by adjusting frequency
                float waveX = sin(normalizedX * _WaveFrequencyX * 2 * UNITY_PI + _Time.y * _SpeedX) * _WaveAmplitude; // X-axis wave
                float waveZ = sin(normalizedZ * _WaveFrequencyZ * 2 * UNITY_PI + _Time.y * _SpeedZ) * _WaveAmplitude; // Z-axis wave

                // Move vertex along the Y direction by adding the wave distortions from both axes
                v.vertex.y += waveX + waveZ;

                // Calculate the time-based offset for UVs
                float2 uvOffset;
                uvOffset.x = _Time.y * _OffsetSpeedX; // Offset X by time with speed
                uvOffset.y = _Time.y * _OffsetSpeedY; // Offset Y by time with speed

                // Apply the offset to the UV coordinates
                o.uv = TRANSFORM_TEX(v.uv, _MainTex) + uvOffset;

                // Transform vertex to clip space
                o.vertex = UnityObjectToClipPos(v.vertex);

                UNITY_TRANSFER_FOG(o, o.vertex); // Transfer fog data to the fragment shader

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Sample the texture using the updated UVs
                fixed4 texColor = tex2D(_MainTex, i.uv);
                fixed4 finalColor = texColor * _Color; // Combine texture color with base color

                UNITY_APPLY_FOG(i.fogCoord, finalColor); // Apply fog to the final color

                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
