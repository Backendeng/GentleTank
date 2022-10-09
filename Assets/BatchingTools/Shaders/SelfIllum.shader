    Shader "BT/SelfIllum"
    {
        Properties
        {
            _Color ("Color Tint", Color) = (1,1,1,1)
            _MainTex ("Base (RGB)", 2D) = "white" {}
        }
        Category
        {
            Tags {"Queue"="Geometry" "IgnoreProjector"="True" "RenderType"="Transparent"}
            
           
            SubShader
            {
                Pass
                {
                    BindChannels
                    {
                           Bind "Vertex", vertex
                          Bind "texcoord", texcoord
                          Bind "Normal", normal
                           Bind "Color", color
                    }
     
                    ZWrite On
                    Cull Back
                    SetTexture [_MainTex]
                    {
                          constantColor [_Color]
                          Combine Texture * constant
                    }
     
                   
                }
            }
         }  
    }