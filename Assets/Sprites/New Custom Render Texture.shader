Shader "Custom/VideoBlackMaskTransparent"
{
    Properties
    {
        _MainTex("Video Texture", 2D) = "white" {}
        _BlackThreshold("Black threshold", Range(0,1)) = 0.05
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

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
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _BlackThreshold;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                // if nearly black (all channels low), make it transparent
                float brightness = max(max(col.r, col.g), col.b);
                if (brightness < _BlackThreshold)
                {
                    // fully transparent
                    col.a = 0;
                }

                return col;
            }
            ENDCG
        }
    }
}
