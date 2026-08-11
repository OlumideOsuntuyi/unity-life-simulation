using System;

namespace Simulation
{
    public class BodyData
    {
        public Chromosome chromosome{ get; private set; }
        public Genes genes { get; private set; }
        public Status status { get; private set; }
        public Memory memory { get; private set; }
        public ActionHandler actions { get; private set; }
        public Locomotion locomotion { get; private set; }
        public Eyes eyes { get; private set; }
        public DateTime birth{ get; private set; }
        public DateTime death{ get; private set; }
        public BodyData(Animal life, Chromosome chromosome, Genes genes)
        {
            this.chromosome = chromosome;
            this.genes = genes;

            birth = DateTime.Now;
            death = DateTime.Now.AddSeconds(genes.Get("lifespan").Value);

            status = new(genes);
            memory = new();
            actions = new(life);
            locomotion = new();
            eyes = new();
        }
        public void Update(Animal life)
        {
            actions.Update();
            status.Update(life);
            memory.Update(life);
        }
    }
}