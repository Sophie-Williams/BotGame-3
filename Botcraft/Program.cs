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
            using(AppWindow window = new AppWindow())
            {
                window.Run(30,30);
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
            game.tick();
        }
        
        public void HandleRenderEvent(object sender, FrameEventArgs e)
        {
            //Console.WriteLine("HandleRenderEevent"); 
            GL.Clear(ClearBufferMask.DepthBufferBit |
                             ClearBufferMask.ColorBufferBit |
                             ClearBufferMask.AccumBufferBit |
                             ClearBufferMask.StencilBufferBit);
            SwapBuffers();
        }

    }
}
