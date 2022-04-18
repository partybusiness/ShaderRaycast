Shader "Unlit/DrawVisMap"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

		Blend One One

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

			float2 monsterForward;
			float2 monsterWorldPos;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
			
			float2 Pos2UV(float2 pos) {
				return pos / 512.0;
			}

			float IsVisible(float2 startPos, float2 goalPos, float dist) {

				float2 forw = normalize(goalPos - startPos);
				float2 currentPos = startPos;
				float2 tile = floor(startPos) + 1;
				float2 dTile = lerp(-1, 1, forw>0);
				float2 dt = ((tile + lerp(-1, 0, forw>0)) - startPos) / forw;
				float2 ddt = dTile / forw;
				float t = 0;

				[unroll(25)]
				while (t<dist) {
					if (dt.x < dt.y) {
						tile.x = tile.x + dTile.x;
						float sdt = dt.x;
						t = min(dist, t + sdt);
						dt.x = dt.x + ddt.x - dt.x;
						dt.y = dt.y - sdt;
					}
					else {
						tile.y = tile.y + dTile.y;
						float sdt = dt.y;
						t = min(dist, t + sdt);
						dt.x = dt.x - sdt;
						dt.y = dt.y + ddt.y - sdt;
					}
					float4 col = tex2D(_MainTex, Pos2UV(tile), 0, 0);
					if (col.r > 0.1) {
						return 0.1;
					}
				}

				return 1.0;
			}

            fixed4 frag (v2f i) : SV_Target
            {
				fixed2 thisPos = i.uv.xy*512.0;
				fixed dist = length(monsterWorldPos + 1 - thisPos);
				fixed bright = clamp(1-sqrt(dist / 15.0), 0 , 1 );
				//try to raycast from this square back to the monst position
				

                fixed4 col = fixed4(bright * IsVisible(thisPos+0.5, monsterWorldPos, dist),0,0,1);
			
                return col;
            }
            ENDCG
        }
    }
}
