Shader "Unlit/MonsterShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_testVal ("Test Val", float) = 0.6
		_ratio ("Ratio", float) = 1.4
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		ZTest Always

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
				float2 screen : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;

			float monsterDist = 1.0;
			float2 monsterPos;

			float2 forward;
			float2 position;
			float fov;

			float _testVal;

			float _ratio;


			float4 stripDistances[512];

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = ((v.uv - 0.5) * monsterDist * fixed2(_ratio, 1));// recalculate based on distance and position?
				o.uv = o.uv + 0.5 - (monsterPos - 0.5) * monsterDist * fixed2(_ratio, 1);
				o.screen = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				

				//float monsterDist = 1.0;
				//float2 monsterPos;
				fixed4 col = tex2D(_MainTex, i.uv);
				//fixed2 screenPos = ComputeScreenPos(i.vertex);
				fixed4 dist = stripDistances[floor(i.screen.x * 512)];
				col.a = (col.a * dist.x>monsterDist);
				
				//col.b = i.screen.y > monsterPos.y;

				clip(col.a);
				//col.r = (floor(floor(i.uv.x * 512)) % 50 < 25);
			//compare to monsterDist
                return col;
            }
            ENDCG
        }
    }
}
