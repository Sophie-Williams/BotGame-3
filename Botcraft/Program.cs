using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input; 

namespace Botcraft
{
    
    class Program
    {
        static void Main(string[] args)
        {
            using(AppWindow doobawindow = new AppWindow())
            {
                doobawindow.Run(30,30);
            }
        }
    }
    
    class AppWindow : GameWindow
    {
        Game game = new Game();
        public AppWindow()
            : base()
        {
            KeyPress += HandleKeyPressEvent;
            Load += HandleOnLoadEvent;
            RenderFrame += HandleRenderEvent;
            UpdateFrame += HandleUpdateEvent;
        }
        public AppWindow(int width, int height)
            : base(width, height)
        {
            KeyPress += HandleKeyPressEvent;
            Load += HandleOnLoadEvent;
            RenderFrame += HandleRenderEvent;
            UpdateFrame += HandleUpdateEvent;
        }

        public void HandleKeyPressEvent(object sender, KeyPressEventArgs e)
        {
            GL.ClearColor(new Color4(0, 0, 33, 0));
        }

        public void HandleOnLoadEvent(object sender, EventArgs e)
        {
            Title = "BotCraft";
            WindowBorder = WindowBorder.Fixed;
            GL.ClearColor(new Color4(0,0,0,0));            
        }

        public void HandleUpdateEvent(object sender, FrameEventArgs e)
        {
            game.Tick();
        }
        
        public void HandleRenderEvent(object sender, FrameEventArgs e)
        {
        //'First Clear Buffers
        GL.Clear(ClearBufferMask.ColorBufferBit);
        GL.Clear(ClearBufferMask.DepthBufferBit);
 
        //'Basic Setup for viewing
        Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(1.04f, 4f / 3f, 1f, 10000f); //'Setup Perspective
        Matrix4 lookat = Matrix4.LookAt(100, 20, 0, 0, 50, 0, 0, 1, 0); //'Setup camera
        GL.MatrixMode(MatrixMode.Projection);// 'Load Perspective
        GL.LoadIdentity();
        GL.LoadMatrix(ref perspective);
        GL.MatrixMode(MatrixMode.Modelview); //'Load Camera
        GL.LoadIdentity();
        GL.LoadMatrix(ref lookat);
        GL.Viewport(0, 0, Width, Height); //'Size of window
        GL.Enable(EnableCap.DepthTest); //'Enable correct Z Drawings
        GL.DepthFunc(DepthFunction.Less); //'Enable correct Z Drawings
 
        //'Rotating
        //GL.Rotate(NumericUpDown1.Value, 0, 0, 1);
        //GL.Rotate(NumericUpDown2.Value, 0, 1, 0);
 
        //'Draw pyramid, Y is up, Z is twards you, X is left and right
        //'Vertex goes (X,Y,Z)
        GL.Begin(PrimitiveType.Triangles);
        //'Face 1
        GL.Color3(Color.Red);
        GL.Vertex3(50, 0, 0);
        GL.Color3(Color.White);
        GL.Vertex3(0, 25, 0);
        GL.Color3(Color.Blue);
        GL.Vertex3(0, 0, 50);
        //'Face 2
        GL.Color3(Color.Green);
        GL.Vertex3(-50, 0, 0);
        GL.Color3(Color.White);
        GL.Vertex3(0, 25, 0);
        GL.Color3(Color.Blue);
        GL.Vertex3(0, 0, 50);
        //'Face 3
        GL.Color3(Color.Red);
        GL.Vertex3(50, 0, 0);
        GL.Color3(Color.White);
        GL.Vertex3(0, 25, 0);
        GL.Color3(Color.Blue);
        GL.Vertex3(0, 0, -50);
        //'Face 4
        GL.Color3(Color.Green);
        GL.Vertex3(-50, 0, 0);
        GL.Color3(Color.White);
        GL.Vertex3(0, 25, 0);
        GL.Color3(Color.Blue);
        GL.Vertex3(0, 0, -50);
 
        //'Finish the begin mode with "end"
        GL.End();
 
        //'Finally...
        //GraphicsContext. = True; //'Caps frame rate as to not over run GPU

            this.SwapBuffers();
        }

    }
}
