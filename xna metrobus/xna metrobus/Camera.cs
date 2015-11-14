using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace xna_metrobus
{
    class Camera : DrawableGameComponent
    {
        public static Camera ActiveCamera;

        // View and projection
        public Matrix Projection = Matrix.Identity;
        public Matrix View = Matrix.Identity;

        public Vector3 position = new Vector3(Mesafe.ToPixel(50), 80, 80);
        public Vector3 angle = new Vector3(MathHelper.ToRadians(50),0,0);
        public float speedUpDown = 100f;
        public float speedLeftRight = 300f;
        public float turnSpeed = 10f;
        public float farPlaneDistance = 3000;

        bool controlEnabled = true;
        MouseState _currentMouseState;
        MouseState _previousMouseState;
        KeyboardState _currentKeyboardState;
        KeyboardState _previousKeyboardState;

        public Camera(Game game)
            : base(game)
        { 
            if (ActiveCamera == null)
                ActiveCamera = this;
        }

        public override void Initialize()
        {
            
            int centerX = Game.Window.ClientBounds.Width / 2;
            int centerY = Game.Window.ClientBounds.Height / 2;
            //
            Mouse.SetPosition(centerX, centerY);
            //
            base.Initialize();
        }

        public void LoadGraphicsContent()
        {
            float ratio = (float)GraphicsDevice.Viewport.Width / (float)GraphicsDevice.Viewport.Height;
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, ratio, 5, farPlaneDistance);

            _currentMouseState = Mouse.GetState();
            _previousMouseState = _currentMouseState;

            _currentKeyboardState = Keyboard.GetState();
            _previousKeyboardState = _currentKeyboardState;

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!Game.IsActive)
                return;

            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            KeyboardState keyboard = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            _previousMouseState = _currentMouseState;
            _currentMouseState = Mouse.GetState();

            _previousKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();

            bool mouseClicked = (_previousMouseState.LeftButton == ButtonState.Released && _currentMouseState.LeftButton == ButtonState.Pressed);
            bool escapeClicked = !_previousKeyboardState.IsKeyDown(Keys.Escape) && _currentKeyboardState.IsKeyDown(Keys.Escape);

            //Now we have the current state, let’s just set the mouse cursor back in the center of the screen, so we don’t forget it later on.
            int centerX = Game.Window.ClientBounds.Width / 2;
            int centerY = Game.Window.ClientBounds.Height / 2;

            if (!controlEnabled && mouseClicked)
            {
                controlEnabled = true;                
            }

            if (!controlEnabled && escapeClicked)
            {
                Game.Exit();
            }
                

            if (controlEnabled && escapeClicked)
                controlEnabled = false;

            

            if (!controlEnabled)
                return;

            Mouse.SetPosition(centerX, centerY);

            //Let’s adjust our angle variable according to the mouse movement.
            angle.X += MathHelper.ToRadians((mouse.Y - centerY) * turnSpeed * 0.01f); // pitch
            angle.Y += MathHelper.ToRadians((mouse.X - centerX) * turnSpeed * 0.01f); // yaw

            //Now that we have our angle, we can easily calculate our pitch and yaw vector (relative movement). I have to warn you, you’ll need your basic trigonometry, if you’re a bit rusty, fresh it up ;-). We’ll need this so we can move in the direction we’re looking at.
            Vector3 forward = Vector3.Normalize(new Vector3((float)Math.Sin(-angle.Y), (float)Math.Sin(angle.X), (float)Math.Cos(-angle.Y)));
            Vector3 left = Vector3.Normalize(new Vector3((float)Math.Cos(angle.Y), 0f, (float)Math.Sin(angle.Y)));


            //Ok, so let’s update our position according to the keys pressed.
            if (keyboard.IsKeyDown(Keys.Up))
                //position -= forward * speed * delta;
                position += new Vector3(0, -1 * speedUpDown * delta, -1 * speedUpDown * delta);

            if (keyboard.IsKeyDown(Keys.Down))
                //position += forward * speed * delta;
                position += new Vector3(0, +1 * speedUpDown * delta, +1 * speedUpDown * delta);

            if (keyboard.IsKeyDown(Keys.Right))
            //position += left * speed * delta;
                position += new Vector3(+1 * speedLeftRight * delta, 0, 0);

            if (keyboard.IsKeyDown(Keys.Left))
            //position -= left * speed * delta;
                position += new Vector3(-1 * speedLeftRight * delta, 0, 0);

            if (keyboard.IsKeyDown(Keys.Add))
                speedLeftRight+=3;

            if (keyboard.IsKeyDown(Keys.Subtract))
                speedLeftRight-=3;

            if (position.Y < 5)
                position.Y = 5;
            if (position.Z < 5)
                position.Z = 5;
            //So, that’s it, we have all we need to calculate our view. We’ll translate to our position and multiply that by our rotations, just that simple.
            View = Matrix.Identity;
            View *= Matrix.CreateTranslation(-position);
            View *= Matrix.CreateRotationZ(angle.Z);
            View *= Matrix.CreateRotationY(angle.Y);
            View *= Matrix.CreateRotationX(angle.X);
        }
    }
}
