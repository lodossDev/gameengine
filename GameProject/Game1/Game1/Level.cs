using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game1
{
    public abstract class Level {
        private Dictionary<int, List<Entity>> layers;
        private List<Entity> misc;
        private String name;
        private int maxLeft, maxRight;


        public Level(String name) {
            layers = new Dictionary<int, List<Entity>>();
            misc = new List<Entity>();

            this.name = name;
            maxLeft = maxRight = 0;

            Load();
        }

        public abstract void Load();

        public void AddLayer(int z, Entity layer) {
            if (!layers.ContainsKey(z)) {
                layers.Add(z, new List<Entity>());
            }

            layers[z].Add(layer);
        }

        public void AddMisc(Entity entity) {
            misc.Add(entity);
        }

        public void SetName(string name) {
            this.name = name;
        }

        public void SetXScrollBoundry(int maxLeft, int maxRight) {
            this.maxLeft = maxLeft;
            this.maxRight = maxRight;
        }

        public virtual void ScrollX(float velX) {
            List<Entity> entities = this.layers.SelectMany(item => item.Value).ToList();
            entities.AddRange(misc);

            foreach (Entity entity in entities) {
                if ((int)velX < 0 && (int)entity.GetPosX() <= maxRight 
                        || (int)velX > 0 && (int)entity.GetPosX() >= maxLeft) { 
                    
                    return;
                }

                entity.MoveX(velX);
            }
        }

        public void ScrollY(float velY) {
            List<Entity> entities = this.layers.SelectMany(item => item.Value).ToList();
            entities.AddRange(misc);

            foreach (Entity entity in entities) {
                entity.MoveY(velY);
            }
        }

        public String GetName() {
            return name;
        }

        public List<Entity> GetLayers(int z) {
            if (layers.ContainsKey(z)) { 
                return layers[z];
            }

            return null;
        }

         public List<Entity> GetMisc() {
            return misc;
        }

        public int GetMaxLeft() {
            return maxLeft;
        }

        public int GetMaxRight() {
            return maxRight;
        }
    }
}
