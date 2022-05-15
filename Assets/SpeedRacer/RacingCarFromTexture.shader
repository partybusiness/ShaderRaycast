Shader "Unlit/RacingCarFromTexture"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

		_DistTex("Distance Texture", 2D) = "white" {}

		_WallTex("Wall Texture", 2D) = "white" {}

		_TileCount("Number of tiles", Int) = 8

		_fadeStrength("Fade Strength", float) = 0.3

		_FloorColour("Floor Colour", COLOR) = (1,1,1,1)

		_CeilingColour("Ceiling Colour", COLOR) = (1,1,1,1)

		_CeilingFade("Ceiling Fade", float) = 1.2

		_CarColour("Car Colours", 2D) = "white" {}

		_CarGloss("Car Gloss", 2D) = "white" {}
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

			sampler2D _DistTex;

			sampler2D _CarColour;

			sampler2D _CarGloss;
			

			sampler2D _MapTex;

			sampler2D _WallTex;

			float4 _FloorColour;

			float4 _CeilingColour;

			float _CeilingFade;

			float _TileCount;

			float _fadeStrength;



			float2 position;
			

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
				float posY = 0.0 + (1.0/512.0*30.0) * _Time.x;

				
				float angleX = atan2(reflectedDir.z, reflectedDir.x) / (3.14159 * 2.0) +0.5; // get into range 0 to 1
				float angleY = asin(reflectedDir.y) / (3.14159 * 2.0) +0.5;
				//return (angleX * 512) > 256;
				fixed4 dist = tex2D(_DistTex, float2(angleX, posY)); //TODO should use time for y pos //
				fixed ypos = abs(angleY - 0.5);
				fixed wallHeight = (0.5 / (dist.x * 256.0));
				fixed isWall = (ypos < wallHeight);//why was it *3


				float2 newForward = normalize(reflectedDir.xz);
				fixed yt = 1 / (abs(ypos) * 2);//how to project y angle ypos to distance to floor
				fixed2 floorCoord = float2(0.5, posY / 512.0) + newForward * yt;
				//fixed checkVal = (floor(floorCoord.x) + floor(floorCoord.y)) % 2;
				//fixed4 checkColour = lerp(0, 1, checkVal);
				fixed2 floorUV = (floorCoord + 1.0) / 512.0;
				fixed2 dotOffset = tex2D(_MainTex, floorUV).gb - 0.5;
				fixed isDot = length(floorCoord % 1 - 0.5 + dotOffset*0.5 ) < 0.2;
				fixed4 floorColour = lerp(_FloorColour, _FloorColour*1.1, isDot);
				fixed4 ceilingColour = lerp(_CeilingColour, _CeilingColour*_CeilingFade, ypos*2.0);
				fixed4 skyColour = lerp(ceilingColour, ceilingColour*0.9, isDot);// lerp(tex2D(_MainTex, floorCoord), tex2D(_MainTex, floorUV), 0.5);
				//lerp(_FloorColour, _FloorColour2, checkVal);
				fixed4 noWallColour = lerp(floorColour, skyColour, angleY > 0.5);

				fixed2 singleWallUv = float2(dist.a, inverseLerp(0.5 - wallHeight, 0.5 + wallHeight, i.uv.y));
				fixed2 wallUv = (floor(dist.gb*_TileCount) + singleWallUv) / _TileCount; //invlerp (0.5-wallHeight,0.5+wallHeight, yt); ??

																						 //fixed4 wallColour = float4(0, dist.gb, 1) / (dist.r*0.3) * dist.a;
				fixed4 wallColour = tex2D(_WallTex, wallUv) / clamp((dist.r)*_fadeStrength, 1, 512);// float4(0, dist.gb, 1) / (dist.r*0.3) * dist.a;
																									//_TileCount
																									//_FloorColour1
				fixed4 reflectedColour = lerp(noWallColour, wallColour, isWall);

				//so then we can combine it with the paint job and how glossy it is, etc.

				fixed4 carColour = tex2D(_CarColour, i.uv);
				fixed4 carGloss = tex2D(_CarGloss, i.uv); //rg is max and min from view angle

				//get angle to surface
				float glance = 1 - abs(dot(normalize(i.normalDir), normalize(i.viewDir)));
				
				fixed glossAmount = lerp(carGloss.g, carGloss.r, glance*glance*glance);

				return lerp(carColour, reflectedColour, glossAmount);
            }
            ENDCG
        }
    }
}
