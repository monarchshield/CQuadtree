#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
#endregion

namespace Creating_a_Quad_Tree
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    /// 
    public class Game1 : Game
    {
        float m_deltatime;
        List<Entity> m_entityList;
        QuadTree quad;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public Texture2D m_EntityTexture;
        public Texture2D m_frame;

        
        public int Improvement;
        public int Collisions;
        public int RawCollisionChecks;

        SpriteFont Font;

        float Cooldown;
        float Distance;

        int quadtreeCollision;

        Point MousePosition;
        Vector2 MousePos;

        List<Entity> ReturnObjects;
        List<QuadTree> QuadList;

        public Game1()
            : base()
        {
            IsMouseVisible = true;
            graphics = new GraphicsDeviceManager(this);
           
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        /// 
      

        protected override void Initialize()
        {
            LoadContent();

            Cooldown = 1.0f;
            m_deltatime = 0;
            Improvement = 0;
            // TODO: Add your initialization logic here
            quadtreeCollision = 0;
            Collisions = 0;
            RawCollisionChecks = 0;

            quad = new QuadTree(0, new Rectangle(0, 0, 795, 495));
            
            ReturnObjects = new List<Entity>();
            QuadList = new List<QuadTree>();

            m_entityList = new List<Entity>();

            
           for (int i = 0; i < 10; i++)
           {
             m_entityList.Add(new Entity(m_EntityTexture, new Vector2(400, 200)));
           }

           
            graphics.PreferredBackBufferWidth = 1200;
            graphics.PreferredBackBufferHeight = 500;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            m_EntityTexture = Content.Load<Texture2D>("Dot");
            Font = Content.Load<SpriteFont>("Arial_18");
           // m_frame = Content.Load<Texture2D>("Frame");

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
    

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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            m_deltatime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Cooldown -= m_deltatime;

           
            Collisions = 0;
            quadtreeCollision = 0;
         


            RawCollisionChecks = m_entityList.Count * m_entityList.Count;
           

            MousePosition = Mouse.GetState().Position;
            MousePos = new Vector2(MousePosition.X,MousePosition.Y);

            if (Mouse.GetState().LeftButton == ButtonState.Pressed && Cooldown < 0)
            {
                m_entityList.Add(new Entity(m_EntityTexture, MousePos));
                Cooldown = .25f;

            }
            QuadList.Clear();
            quad.clear();

            for (int i = 0; i < m_entityList.Count; i++)
            {
                quad.insert(m_entityList[i]);
                m_entityList[i].Collision = false;

                //Insert objects here
            }


           for (int i = 0; i < m_entityList.Count; i++)
           {
               ReturnObjects.Clear();
               quad.retrieve(ReturnObjects, m_entityList[i]);

               for (int x = 0; x < ReturnObjects.Count; x++)
               {
                   for (int j = 0; j < ReturnObjects.Count; j++)
                   {
                       if (x == j)
                       {
                           break;
                       }
                      
                       Distance = (ReturnObjects[x].GetPosition() - ReturnObjects[j].GetPosition()).Length();
                                   
                       if (Distance < 50 )      
                       {
                            ReturnObjects[x].Collision = true;
                            ReturnObjects[j].Collision = true;
                       }

                       quadtreeCollision += 1;
                   }
                 
                    //Run Collision etection algorithmn between objects here
               }
               
           }
         

            foreach (Entity enemy in m_entityList)
            {
                if (enemy.Collision == true)
                {
                    Collisions += 1;
                }
                enemy.Update(m_deltatime);
            }

            quad.Childrenof(quad, QuadList);
            Improvement = RawCollisionChecks / quadtreeCollision;
            

            base.Update(gameTime);
        }


 

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);
            
            // TODO: Add your drawing code here
            //quad.Draw(spriteBatch);
           
            

            foreach (Entity enemy in m_entityList)
            {
                enemy.Draw(spriteBatch);
            }

           

            // Childrenof(quad, QuadList);

             foreach (QuadTree kid in QuadList)
             {
                 if (kid != null)
                 {
                     kid.Draw(spriteBatch);
                 }
             }
            
            

            spriteBatch.Begin();
            spriteBatch.DrawString(Font, "Amount of Objects: " + m_entityList.Count.ToString(), new Vector2(810, 0), Color.White);
            spriteBatch.DrawString(Font, "Amount of Quads: " + QuadList.Count.ToString(), new Vector2(810, 120), Color.White);


            spriteBatch.DrawString(Font, "~Improvement: " + Improvement.ToString() + "x ~", new Vector2(810, 180), Color.White);
            spriteBatch.DrawString(Font, "QuadTree Collision Checks: " + quadtreeCollision.ToString(), new Vector2(810, 150), Color.White);
            spriteBatch.DrawString(Font, "Current Collision Checks: " + Collisions.ToString(), new Vector2(810, 60), Color.White);
            spriteBatch.DrawString(Font, "Raw Collision Checks: " +  RawCollisionChecks.ToString(), new Vector2(810, 90), Color.White);
            spriteBatch.End();
            //spriteBatch.Begin();
            //RectangleSprite.DrawRectangle(spriteBatch, new Rectangle(0,0,50,50), Color.Pink, 3);
            //spriteBatch.End();


            quad.Draw(spriteBatch);

            base.Draw(gameTime);
        }
    }
}
//for (int i = 0; i < m_entityList.Count; i++)
//{
//    ReturnObjects.Clear();
//    quad.retrieve(ReturnObjects, m_entityList[i]);
//    for (int x = 0; x < ReturnObjects.Count; x++)
//    {
//        for (int j = 0; j < ReturnObjects.Count; j++)
//        {
//
//            if (j == x)
//            {
//                break;
//            }
//             Distance = (ReturnObjects[x].GetPosition() - ReturnObjects[j].GetPosition()).Length();
//            if (Distance < 10 && Distance != 0.0f)
//            {
//                ReturnObjects[x].Collision = true;
//                ReturnObjects[j].Collision = true;
//            }
//            //Run collision detect algorithmn between objects here
//        }
//      
//    }
//}