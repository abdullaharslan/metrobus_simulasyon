using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Threading;

namespace xna_metrobus
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        Random r = new Random(DateTime.Now.Millisecond);
        GraphicsDeviceManager graphics;
        Camera camera;

        Model yol;
        Model zemin;
        Model agac;

        Timer timer;
         
        float aspectRatio;
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = Ayar.Default.EkranGenisligi;  // set this value to the desired width of your window
            graphics.PreferredBackBufferHeight = Ayar.Default.EkranYuksekligi;   // set this value to the desired height of your window
            graphics.ApplyChanges();
            graphics.PreferMultiSampling = true;
            Content.RootDirectory = "Content";
            camera = new Camera(this);            
        }
 
        protected override void Initialize()
        {
            float period = Ayar.Default.FrekansSn * 1000f / Ayar.Default.SimulasyonHizi;
            timer = new Timer(new TimerCallback(OtobusGonder), null, 3 * 1000, (int) period);

            camera.Initialize();
            
            Otobus.initialize(this);
            Durak.initialize(this);            

            Durak.DuraklariOlustur();
             
            this.TargetElapsedTime = TimeSpan.FromSeconds(1.0f / Ayar.Default.FrameRate);
            //this.IsFixedTimeStep = false;
            base.Initialize();
            
        }

        

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {             
            aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;
            camera.LoadGraphicsContent();

            Otobus.LoadContent();
            Durak.LoadContent();
             
            yol = Content.Load<Model>("Models\\SingleRoad");
            zemin = Content.Load<Model>("Models\\plain");  
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            //if (keyboardState.IsKeyDown(Keys.Escape))
            //    Exit();

            camera.Update(gameTime);
            
            bool seyehatTamamlandý1 = true;
            bool seyehatTamamlandý2 = true;
            Otobus.otobuslerYeniModel.ForEach(o => { 
                    //if( o.renk!=Renk.Kýrmýzý || gameTime.TotalGameTime.Seconds>5) 
                o.Update(gameTime);
                if (!o.seyehatTamamlandi) seyehatTamamlandý1 = false;
            });
            
            Otobus.otobuslerEskiModel.ForEach(o =>
            {
                o.Update(gameTime);
                if (!o.seyehatTamamlandi) seyehatTamamlandý2 = false;
            });

            if (seyehatTamamlandý1 && seyehatTamamlandý2 && Otobus.otobuslerYeniModel.Count > 0) 
                Exit();

            base.Update(gameTime);
        } 

        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            Durak.duraklar.ForEach(o => o.Draw(gameTime));
            Otobus.otobuslerYeniModel.ForEach(o => o.Draw(gameTime));
            Otobus.otobuslerEskiModel.ForEach(o => o.Draw(gameTime));
             
            ZeminCiz(zemin);
            YolCiz(yol);
             
            
            GraphicsDevice.BlendState = BlendState.AlphaBlend;        
             
            base.Draw(gameTime);
        }
         
        private void YolCiz(Model yol)
        {
            int uzatma = 5200;
            float genislik = 10;
            int tekrar = 1;
            Vector3 position = new Vector3(-60, 0.5f, 0);
            for (int i = 0; i < tekrar; i++)
            {
                ModelCiz(yol, MathHelper.ToRadians(-90), position+new Vector3(0,-0.1f,6.5f),new Vector3(1,1,uzatma));
                ModelCiz(yol, MathHelper.ToRadians(-90), position + new Vector3(0, -0.1f, -2.7f), new Vector3(1, 1, uzatma));
                
                position += new Vector3(genislik*uzatma, 0, 0);
            }
        }

        Vector3 position;
        Vector3 zeminScale;
        private void ZeminCiz(Model zemin)
        {
            float cameraLeftEdge = camera.position.X - camera.farPlaneDistance;
            float cameraRigthEdge = camera.position.X + camera.farPlaneDistance;
          
            int scale = 4;
            float modelGenisligi = 140;
            int toplamCizilmesiGerekenUzunluk = 53000;
            int tekrar = toplamCizilmesiGerekenUzunluk / (int)(modelGenisligi*scale);

            position = new Vector3(-camera.farPlaneDistance, 0, -20);
            if (zeminScale == Vector3.Zero) 
                zeminScale = new Vector3(scale, 1, 1);

            for (int i = 0; i < tekrar; i++)
            {                
                position += new Vector3(modelGenisligi*scale, 0, 0);
                if (position.X > -500 && position.X > cameraLeftEdge - (modelGenisligi * scale) 
                    && position.X < toplamCizilmesiGerekenUzunluk && position.X<cameraRigthEdge+(modelGenisligi*scale))
                    ModelCiz(zemin, 0, position, zeminScale);
            }
        }

        private void YolZeminCiz(Model yolZeminModel, int times)
        {
            float modelGenisligi = 23f;

            Vector3 position = new Vector3(-1 * modelGenisligi * times * 1f/30f, 0, 0);
            // Draw the model. A model can have multiple meshes, so loop.
            for (int i = 1; i <= times; i++)
            {    // Copy any parent transforms.
            Matrix[] transforms = new Matrix[yolZeminModel.Bones.Count];
            yolZeminModel.CopyAbsoluteBoneTransformsTo(transforms);             
                foreach (ModelMesh mesh in yolZeminModel.Meshes)
                {
                    // This is where the mesh orientation is set, as well 
                    // as our camera and projection.
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World = transforms[mesh.ParentBone.Index]
                            * Matrix.CreateScale(1)
                            * Matrix.CreateRotationY(0)
                            * Matrix.CreateTranslation(position)
                            ;
                        effect.View = Camera.ActiveCamera.View;
                        effect.Projection = Camera.ActiveCamera.Projection;

                        effect.LightingEnabled = true; // turn on the lighting subsystem.
                        effect.DirectionalLight0.DiffuseColor = new Vector3(1f, 1, 1); // a red light
                        effect.DirectionalLight0.Direction = new Vector3(0, 50, 0);  // coming along the x-axis
                        effect.DirectionalLight0.SpecularColor = new Vector3(1, 1, 1); // with green highlights

                        effect.DirectionalLight1.DiffuseColor = new Vector3(100f, 100, 100); // a red light
                        effect.DirectionalLight1.Direction = new Vector3(0, 50, 50);  // coming along the x-axis
                        effect.DirectionalLight1.SpecularColor = new Vector3(1, 1, 1); // with green highlights


                        effect.AmbientLightColor = new Vector3(100f, 100f, 100f);
                    }
                    // Draw the mesh, using the effects set above.
                    mesh.Draw();
                    position += new Vector3(modelGenisligi, 0, 0);
                }
            }
        }
        
        private void ModelCiz(Model myModel, float modelRotation, Vector3 modelPosition, Vector3 scale )
        {
            if (scale == null)
            {
                scale = new Vector3(1, 1, 1);
            }

            // Copy any parent transforms.
            Matrix[] transforms = new Matrix[myModel.Bones.Count];
            myModel.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in myModel.Meshes)
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index]
                        * Matrix.CreateScale(scale)
                        * Matrix.CreateRotationY(modelRotation)
                        * Matrix.CreateTranslation(modelPosition);
                        
                    effect.View = Camera.ActiveCamera.View;
                    effect.Projection = Camera.ActiveCamera.Projection;

                    effect.LightingEnabled = true; // turn on the lighting subsystem.
                    effect.DirectionalLight0.DiffuseColor = new Vector3(1f, 1, 1); // a red light
                    effect.DirectionalLight0.Direction = new Vector3(0, 50, 0);  // coming along the x-axis
                    effect.DirectionalLight0.SpecularColor = new Vector3(1, 1, 1); // with green highlights

                    effect.DirectionalLight1.DiffuseColor = new Vector3(100f, 100, 100); // a red light
                    effect.DirectionalLight1.Direction = new Vector3(0, 50, 50);  // coming along the x-axis
                    effect.DirectionalLight1.SpecularColor = new Vector3(1, 1, 1); // with green highlights


                    effect.AmbientLightColor = new Vector3(100f, 100f, 100f);
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
        }

        Renk sonGonderilenOtobusRengi = Renk.Kýrmýzý;
        int otobusSayisi = 0;
        private  void OtobusGonder(Object o)
        {
            if (otobusSayisi == Ayar.Default.ToplamOtobusSayisi)
            {
                timer.Dispose();
                return;
            }

            Renk renk = sonGonderilenOtobusRengi==Renk.Kýrmýzý?Renk.Mavi:Renk.Kýrmýzý;
            int startPoint = -50;
            Otobus.otobuslerYeniModel.Add(new Otobus(camera, renk, startPoint, true));
            Otobus.otobuslerEskiModel.Add(new Otobus(camera, Renk.Gri, startPoint, false));

            sonGonderilenOtobusRengi = renk;
            otobusSayisi++;
        }
    }
}
