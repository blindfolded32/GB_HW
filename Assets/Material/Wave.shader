Shader "Unlit/Wave"
{
    Properties
    {
    _Tex1("Texture1", 2D) = "white" {} // ��������1
    _Tex2("Texture2", 2D) = "white" {} // ��������2
    _MixValue("Mix Value", Range(0,1)) = 0.5 // �������� ���������� �������
    _Color("Main Color", COLOR) = (1,1,1,1) // ���� �����������
    _Height("Height", Range(0,20)) = 0.5 // ���� ������
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
         sampler2D _Tex1; // ��������1
            float4 _Tex1_ST;
            sampler2D _Tex2; // ��������2
            float4 _Tex2_ST;
            float _MixValue; // �������� ����������
            float4 _Color; // ����, ������� ����� ������������ �����������
            float _Height; // ���� ������
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
            //���������
            v2f vert (appdata_full v)
            {
                v2f o;
                
              v.vertex.y = v.texcoord.x/5-1;
               v.vertex.xyz += sin(v.normal * _Height * v.texcoord.x);     
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
               
                return o;
            }
            //�������������
            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                return col;
            }
            ENDCG
        }
    }
}
