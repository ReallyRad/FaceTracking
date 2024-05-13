Shader "Custom/HoleShader"
{
    Properties
    {
        _MainTex ("Surface Texture", 2D) = "white" {}
        _SurfaceColor ("Surface Color", Color) = (1,1,1,1) // Default to white
        _HoleRadius ("Hole Radius", Range(0.0, 1.0)) = 0.0
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _HoleRadius;
            fixed4 _SurfaceColor;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float dist = length(i.uv - 0.5); // Calculate distance from center
                fixed4 col = _SurfaceColor; // Set default surface color

                if (dist < _HoleRadius) // Check if inside hole radius
                    discard; // Discard fragments inside the hole

                // Sample surface texture
                col *= tex2D(_MainTex, i.uv);

                return col; // Return surface color with texture
            }
            ENDCG
        }
    }
}
