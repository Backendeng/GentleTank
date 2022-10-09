    Shader "BT/BT Transparent SelfIllum"
    {
        Properties
        {
            //_Color ("Color Tint", Color) = (1,1,1,1)
            _MainTex ("Base (RGB)", 2D) = "white" {}
        }
        Category
        {
            Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
            Blend SrcAlpha OneMinusSrcAlpha
           
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
                         // constantColor [_Color]
                          Combine Texture * primary
                    }
     
                   
                }
            }
         }  
    }