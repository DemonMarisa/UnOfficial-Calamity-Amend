// 传入的拖尾材质
sampler NeedColorTexture : register(s0);

float4 BeginColor;
float4 EndColor;

float4 ColorFunction(float2 coords : TEXCOORD0) : COLOR0
{
        // 根据材质与坐标指定的基础颜色
    float4 baseColor = tex2D(NeedColorTexture, coords);
    
    // 如果没有颜色，则不做处理
    if (!any(baseColor))
        discard;
    
    baseColor = lerp(BeginColor, EndColor, baseColor.r);
    
    return baseColor;
}

// 最终通道
technique Technique1
{
    pass UCAColorPass
    {
        PixelShader = compile ps_2_0 ColorFunction();
    }
}