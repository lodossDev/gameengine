using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game1
{
    public abstract class Level {
        protected Dictionary<int, List<Entity>> layers;
        protected String name;


        public Level(String name) {
            this.name = name;
            layers = new Dictionary<int, List<Entity>>();
            Load();
        }

        public abstract void Load();

        public void AddLayer(int z, Entity layer) {
            if (!layers.ContainsKey(z)) {
                layers.Add(z, new List<Entity>());
            }

            layers[z].Add(layer);
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

        public void ScrollX(float velX) {
            List<Entity> layers = this.layers.SelectMany(item => item.Value).ToList();

            foreach (Entity entity in layers) {
                //if (velX < 0 && entity.GetPosX() > (1280-1740))
                    entity.MoveX(velX);

                //if (velX > 0 && entity.GetPosX() < 1740)
                    entity.MoveX(velX);
            }
        }

        public void ScrollY(float velY) {
            List<Entity> layers = this.layers.SelectMany(item => item.Value).ToList();

            foreach (Entity entity in layers) {
                entity.MoveY(velY);
            }
        }
    }
}
