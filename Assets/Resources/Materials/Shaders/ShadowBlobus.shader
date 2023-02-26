Shader "Custom/ShadowBlobus"
{
    Properties
    {
        _Color ("Color", Color) = (0,0,0,1)
        _EllipseA ("Ellipse a", Float) = 1.0
        _EllipseB ("Ellipse b", Float) = 1.0
        _EllipseAngle ("Ellipse angle", Float) = 0.0
        _Dist ("Distance", Float) = 20.0
    }

    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "ShadowBlobus"

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            fixed4 _Color;
            float _EllipseA;
            float _EllipseB;
            float _EllipseAngle;
            float2 _EllipseOffset;
            float _Dist;

            struct appdata {
                float4 vertex : POSITION;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.vertex;
                return o;
            }

            float ellipse(float2 pos, float a, float b)
            {
                return pow(b, 2.0) * pow(pos.x, 2.0) + pow(a, 2.0) * pow(pos.y, 2.0) - pow(a, 2.0) * pow(b, 2.0);
            }

            float2 retransform(float2 pos, float2 offset, float2 angle)
            {
                float2 pos_off = pos + offset;
                float angle_rad = radians(angle);
                float2x2 transform = float2x2(cos(angle_rad), -sin(angle_rad), sin(angle_rad), cos(angle_rad));
                return mul(pos_off, transform);
                //return float2(
                //    basis_x.x * pos_off.x - basis_y.y * pos_off.y, 
                //    basis_x.x * pos_off.y + basis_y.y * pos_off.x);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv_t = retransform(i.uv, _EllipseOffset, _EllipseAngle);
                float near = ellipse(uv_t, _EllipseA, _EllipseB);

                fixed alpha = 0;
                if (near < 0)
                {
                    alpha = -near / _Dist;
                }
                
                fixed4 texcol = fixed4(1, 1, 1, alpha);
                return texcol * _Color;
            }
            ENDCG
        }
    }
}
