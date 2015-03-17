using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;


namespace Creating_a_Quad_Tree
{
    class Entity
    {
        static Random random = new Random();
        
        Vector2 m_headingPosition = new Vector2(400,200);
        Vector2 m_currentPosition;
        Vector2 m_direction;
        Texture2D m_Texture;

        Color Colours;

        public bool Collision;

        float m_dt;
        float x;
        float y;


        public Entity() {}

        public Entity(Texture2D texture, Vector2 position)
        {
            m_Texture = texture;
            m_currentPosition = position;
            Collision = false;
        }

        public void Update(float _deltaTime)
        {
            float range = (m_currentPosition - m_headingPosition).Length();
            m_dt = _deltaTime;

            if (range < 10)
            {
                MakeRandom(m_headingPosition);
            }

            if (Collision == true)
            {
                Colours = Color.HotPink;
               // Colours = Color.Red;
            }

            if (Collision == false)
            {
              
                Colours = Color.White;
            }

            Seek(m_headingPosition);

            
        }

        public void Draw(SpriteBatch spritebatch)
        {
            spritebatch.Begin();

            spritebatch.Draw(m_Texture, new Rectangle((int)m_currentPosition.X,(int)m_currentPosition.Y, m_Texture.Width, m_Texture.Height), Colours);

            spritebatch.End();
        }

        public void Seek(Vector2 TargetPos)
        {
            Vector2 MaxVeloxity = new Vector2(5, 5);									//The Max Velocity Will always be Constant. 
            m_direction = new Vector2(TargetPos.X - m_currentPosition.X, TargetPos.Y - m_currentPosition.Y);
            m_direction.Normalize();
            m_currentPosition += m_direction * 100 * m_dt;
        }

        Vector2 MakeRandom(Vector2 rand)
        {
             x = random.Next(0, 800);
             y = random.Next(0, 500);
            

            m_headingPosition = new Vector2(x, y);
            return rand;
        }

        public Vector2 GetPosition() { return m_currentPosition; }
        public Rectangle GetDimensions() { return m_Texture.Bounds; }


    }
}
