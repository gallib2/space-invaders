using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Game1
{
    public class EnemyCollection : EnemyComponent, IEnumerable<EnemyComponent>
    {
        List<EnemyComponent> children;

        public EnemyCollection(Game game) : base(game)
        {
            children = new List<EnemyComponent>();
        }

        public EnemyComponent this[int index]
        {
            get
            {
                return children[index];
            }
        }


        public IEnumerator<EnemyComponent> GetEnumerator()
        {
            foreach (EnemyComponent child in children)
            {
                yield return child;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public override void Add(EnemyComponent c)
        {
            children.Add(c);
        }


        public override void Remove(EnemyComponent c)
        {
            children.Remove(c);
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            foreach (var item in children)
            {
                item.Initialize();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var item in children)
            {
                if (item.Visible)
                {
                    item.Draw(gameTime);
                }
            }

            //base.Draw(gameTime);

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime); // ?

            foreach (var item in children)
            {
                item.Update(gameTime);
            }
        }


    }
}
