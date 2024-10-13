﻿using MagicHat.Core.Dats;
using MagicHat.Core.Render;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System.Numerics;
using System.Text.RegularExpressions;
using WaveEngine.Bindings.OpenGL;
using static SDL2.SDL;
using Color = SixLabors.ImageSharp.Color;
using System.Reflection;

namespace Launcher.Render {
    unsafe public class OpenGLRenderer : IRenderInterface {
        private static Regex _datFileRegex = new Regex(@"^dat:\/\/");
        private Dictionary<nint, GeometryBufferRef> _geometryBuffers = new();
        private List<ManagedGLTexture> _textures = new();
        internal SDL_DisplayMode CurrentDisplayMode;
        private nint SDLWindowHandle;
        private nint SDLGLContext;
        private int _nextGeometryId = 1;
        private readonly ILogger _log;
        private readonly IDatReaderInterface _datReader;

        private struct GeometryBufferRef : IDisposable {
            public readonly uint VAO;
            public readonly uint VBO;
            public readonly uint IBO;
            public readonly VertexPositionColorTexture[] Verts;
            public readonly int[] Indices;

            public GeometryBufferRef(uint vao, uint vbo, uint ebo, VertexPositionColorTexture[] verts, int[] inds) {
                VAO = vao;
                VBO = vbo;
                Verts = verts;
                Indices = inds;
            }

            public void Dispose() {
                fixed (uint* _VAOPtr = &VAO) {
                    GL.glDeleteVertexArrays(1, _VAOPtr);
                }
                fixed (uint* _VBOPtr = &VBO) {
                    GL.glDeleteBuffers(1, _VBOPtr);
                }
                fixed (uint* _IBOPtr = &IBO) {
                    GL.glDeleteBuffers(1, _IBOPtr);
                }
            }
        }

        public int Width { get; protected set; } = 400;
        public int Height { get; protected set; } = 340;
        public List<string> Extensions { get; } = [];
        public Vector2 ViewportSize => new(Width, Height);

        public GLSLShader ColorShader { get; }
        public GLSLShader TextureShader { get; }

        public event EventHandler<EventArgs>? OnRender2D;
        public event EventHandler<EventArgs>? OnGraphicsPreReset;
        public event EventHandler<EventArgs>? OnGraphicsPostReset;

        public OpenGLRenderer(ILogger<OpenGLRenderer> log, IDatReaderInterface datReader) {
            _log = log;
            _datReader = datReader;
            SetupSDL();
            SetupOpenGL();

            var shaderDir = Path.GetFullPath($"./../../Launcher/Launcher/Render/Shaders");

            ColorShader = new GLSLShader("VertexPositionColor", GetEmbeddedResource("Render.Shaders.VertexPositionColor.vert"), GetEmbeddedResource("Render.Shaders.VertexPositionColor.frag"),  _log, shaderDir);
            TextureShader = new GLSLShader("VertexPositionColorTexture", GetEmbeddedResource("Render.Shaders.VertexPositionColorTexture.vert"), GetEmbeddedResource("Render.Shaders.VertexPositionColorTexture.frag"), _log, shaderDir);
        }

        private string GetEmbeddedResource(string filename) {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Launcher." + filename;

            using var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream);
            string result = reader.ReadToEnd();
            return result;
        }

        #region graphics / input setup
        unsafe private void SetupOpenGL() {
            GL.LoadAllFunctions(SDL_GL_GetProcAddress);
        }

        private void SetupSDL() {
            if (SDL_Init(SDL_INIT_EVENTS | SDL_INIT_VIDEO | SDL_INIT_TIMER) < 0) {
                _log.LogError($"SDL_Init failed: {SDL_GetError()}");
            }

            SDL_SetHint(SDL_HINT_RENDER_DRIVER, "opengl");
            SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_DEPTH_SIZE, 24);
            SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_STENCIL_SIZE, 8);
            SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_DOUBLEBUFFER, 1);
            SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_CONTEXT_PROFILE_MASK, SDL_GLprofile.SDL_GL_CONTEXT_PROFILE_CORE);
            SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_CONTEXT_MAJOR_VERSION, 3);
            SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_CONTEXT_MINOR_VERSION, 3);
            SDL_GetCurrentDisplayMode(0, out CurrentDisplayMode);

            var windowFlags = SDL_WindowFlags.SDL_WINDOW_SHOWN | SDL_WindowFlags.SDL_WINDOW_OPENGL | SDL_WindowFlags.SDL_WINDOW_RESIZABLE;
            SDLWindowHandle = SDL_CreateWindow("Launcher", SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED, Width, Height, windowFlags);
            if (SDLWindowHandle == nint.Zero) {
                _log.LogError($"Failed to create window: {SDL_GetError()}");
                return;
            }

            SDLGLContext = SDL_GL_CreateContext(SDLWindowHandle);
            if (SDLGLContext == nint.Zero) {
                _log.LogError($"Failed to create gl context: {SDL_GetError()}");
                return;
            }
        }
        #endregion // graphics / input setup

        internal unsafe string CheckErrors() {
            var error = GL.glGetError();
            if (error != ErrorCode.NoError) {
                _log.LogError($"OpenGL Error: {error}");
            }

            return error.ToString();
        }

        public void Resize(int width, int height) {
            if (width > 0 && height > 0) {
                if (Width != width) {
                    Width = width;
                }
                if (Height != height) {
                    Height = height;
                }
            }
        }

        public void Update() {
            SDL_SetWindowSize(SDLWindowHandle, Width, Height);
            GL.glViewport(0, 0, Width, Height);
        }

        public void Render() {
            SDL_GL_MakeCurrent(SDLWindowHandle, SDLGLContext);

            GL.glBindFramebuffer(FramebufferTarget.Framebuffer, 0);

            GL.glClearColor(0.02f, 0.118f, 0.188f, 1.0f);
            GL.glClear((uint)(AttribMask.ColorBufferBit | AttribMask.DepthBufferBit | AttribMask.StencilBufferBit));

            GL.glEnable(EnableCap.Blend);
            GL.glDisable(EnableCap.ScissorTest);
            GL.glDisable(EnableCap.CullFace);
            GL.glBlendEquation(BlendEquationModeEXT.FuncAdd);
            GL.glBlendFunc(BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);
            GL.glEnable(EnableCap.StencilTest);
            GL.glStencilFunc(StencilFunction.Always, 1, 0xFFFFFFFF);
            GL.glStencilMask(0xFFFFFFFF);
            GL.glStencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Keep);
            GL.glDisable(EnableCap.DepthTest);

            ColorShader.SetUniform("xProjection", Matrix4x4.CreateOrthographicOffCenter(0, Width, Height, 0, -10000, 10000));
            TextureShader.SetUniform("xProjection", Matrix4x4.CreateOrthographicOffCenter(0, Width, Height, 0, -10000, 10000));

            OnRender2D?.Invoke(this, EventArgs.Empty);

            SDL_GL_SwapWindow(SDLWindowHandle);

            _x = true;
        }

        public unsafe IntPtr CompileGeometry(IEnumerable<VertexPositionColorTexture> vertices, IEnumerable<int> indices) {
            _log?.LogTrace($"CompileGeometry: verts: {vertices.Count()} inds: {indices.Count()}");

            var verts = vertices.ToArray();
            var vSize = VertexPositionColorTexture.Size;

            uint VBO, VAO, IBO;
            GL.glGenVertexArrays(1, &VAO);
            GL.glGenBuffers(1, &VBO);
            GL.glGenBuffers(1, &IBO);
            GL.glBindVertexArray(VAO);

            GL.glBindBuffer(BufferTargetARB.ArrayBuffer, VBO);
            fixed (void* vertsPtr = &verts[0]) {
                GL.glBufferData(BufferTargetARB.ArrayBuffer, vSize * verts.Length, vertsPtr, BufferUsageARB.StaticDraw);
            }

            // Position
            GL.glEnableVertexAttribArray(0);
            GL.glVertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, vSize, (void*)0);

            // Color
            GL.glEnableVertexAttribArray(1);
            GL.glVertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, vSize, (void*)12);

            // Texture Coordinate
            GL.glEnableVertexAttribArray(2);
            GL.glVertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, vSize, (void*)28);

            // Bind and set index buffer data
            var indicesArray = indices.ToArray();
            GL.glBindBuffer(BufferTargetARB.ElementArrayBuffer, IBO);
            fixed (int* indicesPtr = &indicesArray[0]) {
                GL.glBufferData(BufferTargetARB.ElementArrayBuffer, indicesArray.Length * sizeof(uint), indicesPtr, BufferUsageARB.StaticDraw);
            }

            GL.glBindVertexArray(0);
            GL.glBindBuffer(BufferTargetARB.ArrayBuffer, 0);
            GL.glBindBuffer(BufferTargetARB.ElementArrayBuffer, 0);

            var id = _nextGeometryId++;
            var geometry = new GeometryBufferRef(VAO, VBO, IBO, verts, indicesArray);
            _geometryBuffers.Add(id, geometry);

            return new IntPtr(id);
        }

        private bool _x = false;

        public void RenderGeometry(IntPtr geometry, Matrix4x4 translation, ITexture? texture) {
            var geom = _geometryBuffers[geometry];

            if (texture is ManagedGLTexture dxTexture && dxTexture is not null) {
                if (!_x) {
                    return;
                }
                GL.glBindTexture(TextureTarget.Texture2d, (uint)dxTexture.TexturePtr);
                TextureShader.SetActive();
                TextureShader.SetUniform("xWorld", translation);
            }
            else {
                ColorShader.SetActive();
                ColorShader.SetUniform("xWorld", translation);
                GL.glBindTexture(TextureTarget.Texture2d, 0);
            }

            GL.glBindVertexArray(geom.VAO);
            GL.glDrawElements(PrimitiveType.Triangles, geom.Indices.Length, DrawElementsType.UnsignedInt, (void*)0);

            GL.glBindTexture(TextureTarget.Texture2d, 0);
            GL.glBindVertexArray(0);
        }

        public void ReleaseGeometry(IntPtr geometry) {
            _log?.LogTrace($"ReleaseGeometry: 0x{geometry:X8}");
            if (_geometryBuffers.TryGetValue(geometry, out var geom)) {
                _geometryBuffers.Remove(geometry);
                geom.Dispose();
            }
        }

        public ITexture GenerateTexture(byte[] source, Vector2 dimensions) {
            _log?.LogTrace($"Generate texture: {dimensions.X}x{dimensions.Y}");
            var dx = (int)dimensions.X;
            var dy = (int)dimensions.Y;

            var texture = new ManagedGLTexture(source, dx, dy);
            _textures.Add(texture);
            return texture;
        }

        public ITexture? LoadTexture(string source, out Vector2 textureDimensions) {
            try {
                _log?.LogTrace($"LoadTexture: {source}");
                ManagedGLTexture texture;

                if (_datFileRegex.IsMatch(source)) {
                    texture = new ManagedGLTexture(source, _datReader);
                }
                else {
                    texture = new ManagedGLTexture(source);
                }

                if (texture is null) {
                    _log?.LogError($"Failed to load texture: {source}");
                    textureDimensions = Vector2.Zero;
                    return default;
                }

                _log?.LogTrace($"Loaded texture: 0x{texture.TexturePtr:X8} {source}");

                textureDimensions = new Vector2(texture.Width, texture.Height);
                _textures.Add(texture);
                return texture;
            }
            catch (Exception ex) {
                _log?.LogError($"Error loading texture ({source}): {ex}");
                textureDimensions = System.Numerics.Vector2.Zero;
                return default;
            }
        }

        public void ReleaseTexture(ITexture textureHandle) {
            _log?.LogTrace($"Disping texture: 0x{textureHandle.TexturePtr:X8}");
            textureHandle.Dispose();
            _textures.Remove((ManagedGLTexture)textureHandle);
        }

        public void EnableScissorRegion(bool enable) {
            if (enable) {
                GL.glEnable(EnableCap.ScissorTest);
            }
            else {
                GL.glDisable(EnableCap.ScissorTest);
            }
        }

        public void SetScissorRegion(int x, int y, int width, int height) {
           GL.glScissor(x, y, width, height);
        }

        public void Dispose() {
            foreach (var texture in _textures) {
                texture.Dispose();
            }
            _textures.Clear();
        }
    }
}