Shader "Custom/BoxMaskShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,0.1) // Set alpha to make the box transparent
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Geometry+1" }
        LOD 100

        // Write to the stencil buffer to mask out objects inside the box
        Stencil
        {
            Ref 1
            Comp Always
            Pass Replace
        }

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 color : COLOR;
            };

            v2f vert (float4 pos : POSITION, float4 color : COLOR)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(pos);
                o.color = color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return fixed4(1, 1, 1, 0); // Fully transparent to only write to the stencil buffer
            }
            ENDCG
        }
    }
}
