Shader "Unlit/RoadRender"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

		_SkyTex("Sky Texture", 2D) = "white" {}

		_RoadTex("Road Texture", 2D) = "white" {}

		_horizon("Horizon", float) = 0.6

		_skyWidthMult("Sky Width Multiplier", float) = 4.0

		_WallTex("Wall Texture", 2D) = "white" {}


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

			sampler2D _RoadTex;
			float4 _RoadTex_ST;


			sampler2D _SkyTex;
			float4 _SkyTex_ST;

			float _horizon;
			float _skyWidthMult;

			float2 forward;
			float2 position;

			float skyAngle;

			float fov;
			
			float _screenHeight;

			//uniform float3 _Points [100]
			float4 stripDistances[512];

			/*
			just throwing this in here for no reason

			https://bottosson.github.io/posts/colorwrong/
			
			float f(float x)
			{
				if (x >= 0.0031308)
					return (1.055) * x^(1.0/2.4) - 0.055
				else
					return 12.92 * x
			}

			float f_inv(float x)
			{
				if (x >= 0.04045)
					return ((x + 0.055)/(1 + 0.055))^2.4
				else 
					return x / 12.92
			}*/

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }


			float inverseLerp(float A, float B, float T)
			{
				 return (T - A) / (B - A);
			}

            fixed4 frag (v2f i) : SV_Target
            {
			fixed4 dist = stripDistances[floor((1-i.uv.y/ _horizon) * 256.0)];

			fixed xpos = abs(i.uv.x - dist.y);
			fixed roadWidth = 0.5 / dist.x;
			fixed isRoad = (xpos < roadWidth);
			fixed isSky = i.uv.y > _horizon;

			fixed2 roadUV = fixed2((i.uv.x - dist.y )/roadWidth / 2.0  + 0.5, dist.z);
			fixed4 roadColour = tex2D(_RoadTex, roadUV); //sample from road texture instead?
			fixed4 grassColour = float4(0.4,0.7,0.2, 1);

			fixed4 groundColor = lerp(grassColour, roadColour, isRoad);

			fixed2 skyUv = fixed2(i.uv.x / _skyWidthMult + skyAngle, (i.uv.y - _horizon) / (1-_horizon));
			fixed4 skyColour = tex2D(_SkyTex, skyUv); //sample from sky tex
			fixed4 colour = lerp(groundColor, skyColour, isSky);

			return colour;
            }
            ENDCG
        }
    }
}
