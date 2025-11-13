// 用于Alpha裁切的材质
sampler AlphaTexture : register(s0);
// 用于底部的噪波
sampler NoiseTexture : register(s1);
// UV的坐标偏移
float2 UVOffset;
// 噪波材质的缩放
float2 NoiseTextureScale;

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

float4 SGSFlowShader(VertexShaderOutput input) : COLOR
{
    // 进行alpha裁切
    float4 baseColor = tex2D(AlphaTexture, input.TextureCoordinates);
    if (baseColor.r == 0)
        discard;
    // 贴上噪波材质
    float2 NoiseTextureUV = input.TextureCoordinates * NoiseTextureScale;
    NoiseTextureUV = frac(NoiseTextureUV);
    float4 NoiseColor = tex2D(NoiseTexture, NoiseTextureUV + UVOffset);
    return NoiseColor.r * input.Color * baseColor.r;
}

technique SpriteDrawing
{
    pass UCASGSShaderPass
    {
        PixelShader = compile ps_3_0 SGSFlowShader();
    }
};