Shader "Unlit/RacingCarMaterial"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

		_WallTex("Wall Texture", 2D) = "white" {}

		_TileCount("Number of tiles", Int) = 8

		_fadeStrength("Fade Strength", float) = 0.3


		_CeilingColour("Ceiling Colour", COLOR) = (1,1,1,1)
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
				float3 normal : NORMAL;
            };

            struct v2f
            {
				float4 pos : SV_POSITION;
				float2 uv: TEXCOORD0;
				float3 normalDir : TEXCOORD1;
				float3 viewDir : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

			sampler2D _MapTex;

			sampler2D _WallTex;

			float4 _CeilingColour;

			float _TileCount;

			float _fadeStrength;



			float2 position;

			float4 stripDistances[512];


			float inverseLerp(float A, float B, float T)
			{
				return (T - A) / (B - A);
			}

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.viewDir = mul(unity_ObjectToWorld, v.vertex).xyz - _WorldSpaceCameraPos;
				o.normalDir = normalize( mul(float4(v.normal, 0.0), unity_WorldToObject).xyz);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float3 reflectedDir = reflect(normalize(i.viewDir), normalize(i.normalDir));

				//use the reflectedDir as the direction when rendering strips
				
				float angleX = atan2(reflectedDir.z, reflectedDir.x) / (3.14159 * 2.0) +0.5; // get into range 0 to 1
				float angleY = asin(reflectedDir.y) / (3.14159 * 2.0) +0.5;
				//return (angleX * 512) > 256;
				fixed4 dist = stripDistances[floor(angleX * 512)];
				fixed ypos = abs(angleY - 0.5);
				fixed wallHeight = (0.5 / dist.x);
				fixed isWall = (ypos*3 < wallHeight);


				float2 newForward = normalize(reflectedDir.xz);
				fixed yt = 1 / (abs(ypos) * 2);//how to project y angle ypos to distance to floor
				fixed2 floorCoord = position + newForward * yt;
				fixed checkVal = (floor(floorCoord.x) + floor(floorCoord.y)) % 2;
				fixed2 floorUV = (floorCoord + 1) / 512.0;
				fixed4 floorColour = tex2D(_MapTex, floorUV);
				//lerp(_FloorColour1, _FloorColour2, checkVal);
				fixed4 noWallColour = lerp(floorColour, _CeilingColour, angleY > 0.5);

				fixed2 singleWallUv = float2(dist.a, inverseLerp(0.5 - wallHeight, 0.5 + wallHeight, i.uv.y));
				fixed2 wallUv = (floor(dist.gb*_TileCount) + singleWallUv) / _TileCount; //invlerp (0.5-wallHeight,0.5+wallHeight, yt); ??

																						 //fixed4 wallColour = float4(0, dist.gb, 1) / (dist.r*0.3) * dist.a;
				fixed4 wallColour = tex2D(_WallTex, wallUv) / clamp((dist.r)*_fadeStrength, 1, 512);// float4(0, dist.gb, 1) / (dist.r*0.3) * dist.a;
																									//_TileCount
																									//_FloorColour1
				fixed4 result = lerp(noWallColour, wallColour, isWall);
				//result.r = (floor(floor(i.uv.x * 512) % 50) < 25);
				return result;
            }
            ENDCG
        }
    }
}
