using MoonWorks;
using MoonWorks.Graphics;
using System.Numerics;
using System.Runtime.InteropServices;
using Buffer = MoonWorks.Graphics.Buffer;

namespace BasicTriangle;

[StructLayout(LayoutKind.Sequential)]
public struct PositionVertex : IVertexType
{
    public Vector3 Position;

    public PositionVertex(Vector3 position)
    {
        Position = position;
    }

    public static VertexElementFormat[] Formats =>
    [
        VertexElementFormat.Float3
    ];


    public static uint[] Offsets { get; } = [0];
}

public class BasicTriangleGame : Game
{
    private Buffer _vertexBuffer;
    private GraphicsPipeline _pipeline;

    public BasicTriangleGame(
        AppInfo appInfo,
        WindowCreateInfo windowCreateInfo,
        FramePacingSettings framePacingSettings
        ) : base(
            appInfo,
            windowCreateInfo,
            framePacingSettings,
            ShaderFormat.SPIRV | ShaderFormat.DXIL | ShaderFormat.MSL | ShaderFormat.DXBC)
    {
        ShaderCross.Initialize();

        var vertexShader = ShaderCross.Create(
            GraphicsDevice,
            RootTitleStorage,
            "Content/Shaders/Vertex.vert.hlsl",
            "main",
            ShaderCross.ShaderFormat.HLSL,
            ShaderStage.Vertex);

        var fragmentShader = ShaderCross.Create(
            GraphicsDevice,
            RootTitleStorage,
            "Content/Shaders/Color.frag.hlsl",
            "main",
            ShaderCross.ShaderFormat.HLSL,
            ShaderStage.Fragment);

        var pipelineCreateInfo = new GraphicsPipelineCreateInfo
        {
            TargetInfo = new GraphicsPipelineTargetInfo
			{
				ColorTargetDescriptions = [
					new ColorTargetDescription
					{
						Format = MainWindow.SwapchainFormat,
						BlendState = ColorTargetBlendState.Opaque
					}
				]
			},
			DepthStencilState = DepthStencilState.Disable,
			MultisampleState = MultisampleState.None,
			PrimitiveType = PrimitiveType.TriangleList,
			RasterizerState = RasterizerState.CCW_CullNone,
			VertexInputState = VertexInputState.Empty,
			VertexShader = vertexShader,
			FragmentShader = fragmentShader
        };

        pipelineCreateInfo.VertexInputState = VertexInputState.CreateSingleBinding<PositionVertex>();
        _pipeline = GraphicsPipeline.Create(GraphicsDevice, pipelineCreateInfo);

        ReadOnlySpan<PositionVertex> vertexData = [
           new PositionVertex(new Vector3(-0.5f, -0.5f, 0.0f)),
           new PositionVertex(new Vector3(0.5f, -0.5f, 0.0f)),
           new PositionVertex(new Vector3(0.0f, 0.5f, 0.0f))
        ];

        var resourceUploader = new ResourceUploader(GraphicsDevice);
        _vertexBuffer = resourceUploader.CreateBuffer(vertexData, BufferUsageFlags.Vertex);

        resourceUploader.Upload();
        resourceUploader.Dispose();
    }

    protected override void Update(TimeSpan delta)
    {
    }

    protected override void Draw(double alpha)
    {
        CommandBuffer cmdbuf = GraphicsDevice.AcquireCommandBuffer();
        Texture swapchainTexture = cmdbuf.AcquireSwapchainTexture(MainWindow);
        if (swapchainTexture != null)
        {
            var renderPass = cmdbuf.BeginRenderPass(
                new ColorTargetInfo(swapchainTexture, Color.Black)
            );
            cmdbuf.EndRenderPass(renderPass);
        }
        GraphicsDevice.Submit(cmdbuf);
    }
}
