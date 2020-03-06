Shader "GPUCube/GPUInstancingRender"
{
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        Cull OFF
        Lighting Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"
            #include "CommonData.cginc"

        struct appdata
        {
            float4 vertex : POSITION;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct v2f
        {
            float4 vertex : SV_POSITION;
            float4 color : COLOR;
        };

        StructuredBuffer<Cube> _CubeBuffer;

        float4x4 eulerAnglesToRotationMatrix(float3 angles)
        {
            float ch = cos(angles.y); float sh = sin(angles.y); // heading
            float ca = cos(angles.z); float sa = sin(angles.z); // attitude
            float cb = cos(angles.x); float sb = sin(angles.x); // bank

                                                                //RxRyRz
            return float4x4 
            (

                ch * ca + sh * sb * sa, -ch * sa + sh * sb * ca, sh * cb, 0,
                cb * sa, cb * ca, -sb, 0,
                -sh * ca + ch * sb * sa, sh * sa + ch * sb * ca, ch * cb, 0,
                0, 0, 0, 1
            );
        }

        v2f vert (appdata v, uint instanceID : SV_InstanceID)
        {
            v2f o;
            UNITY_SETUP_INSTANCE_ID(v);
            Cube c = _CubeBuffer[instanceID];

            // apply color
            o.color = c.color;

            // apply position
            float4x4 object2world = (float4x4)0;
            object2world._11_22_33_44 = float4(c.size, 1.0);
            object2world._14_24_34 = c.position;
            v.vertex = mul(object2world, v.vertex);
            o.vertex = UnityObjectToClipPos(v.vertex);
            return o;
        }

        fixed4 frag (v2f i) : SV_Target
        {
            return i.color;
        }
            ENDCG
        }
    }
}