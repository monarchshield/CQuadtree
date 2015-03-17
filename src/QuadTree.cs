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
    class QuadTree
    {
        //Max amount of objects something can hold before it splits
        const int MAX_OBJECTS = 5;
       
        //Deepest level subnode
        const int MAX_LEVELS = 3;

        private int level;
        public List<Entity> objects;

        public int index;

        
        //@D space the node occupies 
        private Rectangle bounds;
        //The four subnodes
        public QuadTree[] nodes;

        public QuadTree()
        {
            objects = new List<Entity>();
        }

        public QuadTree(int pLevel, Rectangle pBounds)
        {
            objects = new List<Entity>();
            level = pLevel;
            bounds = pBounds;


     

          nodes = new QuadTree[4];

        }
           
        //Clears the quad tree
        public void clear()
        {


          objects.Clear();

          if (nodes != null)
          {
              for (int i = 0; i < nodes.Length; i++)
              {
                  if (nodes[i] != null)
                  {
                     
                      nodes[i].objects.Clear();
                      nodes[i] = null;
                  }
              }
          }
        }

        public void split()
        {
            int subWidth = (int)(bounds.Width / 2);
            int subHeight = (int)(bounds.Height / 2);

            int x = bounds.X;
            int y = bounds.Y;

            nodes[0] = new QuadTree(level + 1, new Rectangle(x + subWidth, y, subWidth, subHeight));
            nodes[1] = new QuadTree(level + 1, new Rectangle(x, y, subWidth, subHeight));
            nodes[2] = new QuadTree(level + 1, new Rectangle(x, y + subHeight, subWidth, subHeight));
            nodes[3] = new QuadTree(level + 1, new Rectangle(x + subWidth, y + subHeight, subWidth, subHeight));
        }

        public int getIndex(Entity pRect)
        {
            index = -1;
          
            double verticalMidpoint = bounds.X + (bounds.Width / 2);
            double horizontalMidpoint = bounds.Y + (bounds.Height / 2);

            //Object can completly fit within the top quandrants
            Boolean topQuandrant = (pRect.GetPosition().Y <= horizontalMidpoint && pRect.GetPosition().Y + pRect.GetDimensions().Height <= horizontalMidpoint);

            //Object can completly fit within the bottom quandrants
            Boolean bottomQuadrant = (pRect.GetPosition().Y >= horizontalMidpoint);

            //Object can completly fit within the left quandrants
            if (pRect.GetPosition().X <= verticalMidpoint && pRect.GetPosition().X + pRect.GetDimensions().Width <= verticalMidpoint)
            {
                if (topQuandrant)
                    index = 1;
                
                else if (bottomQuadrant)
                    index = 2;
            }

            //Object can completly fit within the right quandrants

            else if (pRect.GetPosition().X >= verticalMidpoint)
            {
                if (topQuandrant)
                    index = 0;

                else if (bottomQuadrant)
                    index = 3;
            }

            return index;
        }
       
        public void insert(Entity pRect)
        {
            if (nodes == null)
            {
                index = getIndex(pRect);
           
                if (index != -1)
                {
                    
                    nodes[index].objects.Add(pRect);
                    return;
                }
            }
            
            objects.Add(pRect);

            
            if (objects.Count > MAX_OBJECTS && level < MAX_LEVELS)
            {
                if ( nodes[0] == null)
                {
                    split();
                }
            }

      
            for (int i = 0; i < objects.Count; )
            {
                index = getIndex(objects[i]);
                if (index != -1 && nodes[0] != null)
                {
                    nodes[index].insert(objects[i]);
                    objects.Remove(objects[i]);

                }
                    
                else
                    i++;
            }
        }

        public List<Entity>retrieve(List<Entity> returnObjects, Entity pRect)
        {
            int index = getIndex(pRect);

            if (index != -1 && nodes[0] != null)
            {
                nodes[index].retrieve(returnObjects, pRect);

                foreach (Entity obj in nodes[index].objects)
                {
                    returnObjects.Add(obj);
                }
            }
             
         
            //TODO: Retrieve all the objects within the quad its found;
            
           // returnObjects.Add(pRect);
            

             return returnObjects;
        }


        //Recursive Return
        public List<QuadTree> Childrenof(QuadTree parent, List<QuadTree> result)
        {
            if (parent != null)
            {
                foreach (QuadTree child in parent.nodes)
                {
                    if (child != null)
                    {
                        result.Add(child);

                        Childrenof(child, result);
                    }
                }
            }

            return result;
        }



        public void Draw(SpriteBatch spritebatch)
        {
            spritebatch.Begin();

            switch (level)
            {
                case 0: RectangleSprite.DrawRectangle(spritebatch, bounds, Color.Red, 5); break;
                case 1: RectangleSprite.DrawRectangle(spritebatch, bounds, Color.Blue, 5); break;
                case 2: RectangleSprite.DrawRectangle(spritebatch, bounds, Color.LimeGreen, 5); break;
                case 3: RectangleSprite.DrawRectangle(spritebatch, bounds, Color.Orange, 5); break;
              //  case 4: RectangleSprite.DrawRectangle(spritebatch, bounds, Color.Purple, 5); break;
            }


           

            spritebatch.End();

        }

    }

    class RectangleSprite
    {
        static Texture2D _pointTexture;
        public static void DrawRectangle(SpriteBatch spritebatch, Rectangle rectangle, Color color, int lineWidth)
        {

            if (_pointTexture == null)
            {
                _pointTexture = new Texture2D(spritebatch.GraphicsDevice, 1, 1);
                _pointTexture.SetData<Color>(new Color[] { Color.White });
            }
            spritebatch.Draw(_pointTexture, new Rectangle(rectangle.X, rectangle.Y, lineWidth, rectangle.Height + lineWidth), color);
            spritebatch.Draw(_pointTexture, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width + lineWidth, lineWidth), color);
            spritebatch.Draw(_pointTexture, new Rectangle(rectangle.X + rectangle.Width, rectangle.Y, lineWidth, rectangle.Height + lineWidth), color);
            spritebatch.Draw(_pointTexture, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height, rectangle.Width + lineWidth, lineWidth), color);
        }

    }
}
