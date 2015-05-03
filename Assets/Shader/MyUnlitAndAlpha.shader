Shader "Custom/MyUnlitAndAlpha" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("BASE (RGB) Alpha(A)", 2D) = "" {}
		//_Glossiness ("Smoothness", Range(0,1)) = 0.0
		//_Metallic ("Metallic", Range(0,1)) = 0.0
	} 
	
	SubShader {
	    Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	    LOD 100
	    
	    ZWrite Off
	    Blend SrcAlpha OneMinusSrcAlpha 
	    
	    Pass {  
	        CGPROGRAM
	            #pragma vertex vert
	            #pragma fragment frag
	            
	            #include "UnityCG.cginc"

	            struct appdata_t {
	                float4 vertex : POSITION;
	                float2 texcoord : TEXCOORD0;
	            };

	            struct v2f {
	                float4 vertex : SV_POSITION;
	                half2 texcoord : TEXCOORD0;
	            };

	            sampler2D _MainTex;
	            fixed4 _Color;
	            
	            float4 _MainTex_ST;
	            
	            v2f vert (appdata_t v)
	            {
	                v2f o;
	                o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
	                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
	                return o;
	            }
	            
	            fixed4 frag (v2f i) : SV_Target
	            {
	                fixed4 col = tex2D(_MainTex, i.texcoord);
	      			col.a = _Color.a;
	      			
	                return col;
	            }
	        ENDCG
	    }
	}


	FallBack "Diffuse"
}



