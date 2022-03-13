Shader "Unlit/StripsRender"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

		_WallTex("Wall Texture", 2D) = "white" {}

		_FloorColour1("Floor Colour 1", COLOR) = (1,1,1,1)
		_FloorColour2("Floor Colour 2", COLOR) = (1,1,1,1)
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
            float4 _MainTex_ST;

			sampler2D _WallTex;
			float4 _WallTex_ST;

			float4 _FloorColour1;
			float4 _FloorColour2;

			float2 forward;
			float2 position;

			//uniform float3 _Points [100]
			float4 stripDistances[512];

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
			fixed4 dist = stripDistances[floor(i.uv.x * 512)];
			//use i.uv.y to calc amount of thing?
			fixed ypos = abs(i.uv.y - 0.5);
			fixed isWall = (ypos < (10.0 / dist.x));
			fixed4 wallColour = float4(0, dist.gb, 1) / (dist.x*0.03);
			
			//_FloorColour1
			return lerp(_FloorColour1 * ypos, wallColour, isWall);
            }
            ENDCG
        }
    }
}
