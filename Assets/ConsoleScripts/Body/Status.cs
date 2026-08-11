using System.Diagnostics;

namespace Simulation
{
    public class Status
    {
        private float _health;
        private float _saturation;
        private float _stamina;
        private float _reproductiveUrge;


        public float maxHealth { get; private set; }
        public float maxSaturation { get; private set; }
        public float maxStamina { get; private set; }
        public float maxReproductiveUrge { get; private set; }
        public float health
        {
            get
            {
                return _health;
            }
            set
            {
                _health = Math.Clamp(value, 0, maxHealth);
            }
        }
        public float saturation
        {
            get
            {
                return _saturation;
            }
            set
            {
                _saturation = Math.Clamp(value, 0, maxSaturation);
            }
        }

        public float stamina
        {
            get
            {
                return _stamina;
            }
            set
            {
                _stamina = Math.Clamp(value, 0, maxStamina);
            }
        }

        public float reproductiveUrge
        {
            get
            {
                return _reproductiveUrge;
            }
            set
            {
                _reproductiveUrge = Math.Clamp(value, 0, maxReproductiveUrge);
            }
        }
        public Status(Genes genes)
        {
            maxHealth = genes.modifications.health;
            maxStamina = genes.modifications.stamina;
            maxReproductiveUrge = genes.GestationPeriod;
            maxSaturation = maxHealth;

            saturation = maxSaturation;
            health = maxHealth;
            stamina = maxStamina;
            reproductiveUrge = maxReproductiveUrge;
        }
        public void Update(Animal life)
        {
            float recovery = Benchmarks.Benchmark.recovery * life.data.genes.modifications.recovery;
            if (health < maxHealth && saturation > 0)
            {
                health += Time.deltaTime;
                saturation -= Benchmarks.InverseBenchmark.saturation * Time.deltaTime;
            }
            if(saturation == 0)
            {
                health -= Time.deltaTime;
            }
        }
        public void UseStamina()
        {
            stamina -= .1f * Time.deltaTime;
        }
        public void Eat(float nutrition)
        {
            saturation += nutrition;
            stamina += nutrition;
            reproductiveUrge += maxReproductiveUrge * .5f;
        }

        public float Health()
        {
            return health / maxHealth;
        }
        public float Stamina()
        {
            return stamina / maxStamina;
        }
        public float Saturation()
        {
            return saturation / maxSaturation;
        }
        public float Reproduction()
        {
            return reproductiveUrge / maxReproductiveUrge;
        }
    }
}