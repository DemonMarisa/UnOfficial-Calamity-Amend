// �������β����
sampler NeedColorTexture : register(s0);

float4 BeginColor;
float4 EndColor;

float4 ColorFunction(float2 coords : TEXCOORD0) : COLOR0
{
        // ���ݲ���������ָ���Ļ�����ɫ
    float4 baseColor = tex2D(NeedColorTexture, coords);
    
    // ���û����ɫ����������
    if (!any(baseColor))
        discard;
    
    baseColor = lerp(BeginColor, EndColor, baseColor.r);
    
    return baseColor;
}

// ����ͨ��
technique Technique1
{
    pass UCAColorPass
    {
        PixelShader = compile ps_2_0 ColorFunction();
    }
}