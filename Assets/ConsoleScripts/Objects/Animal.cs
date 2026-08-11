using System;


namespace Simulation
{
    [System.Serializable]
    public class Animal : Component
    {
        public string UUID { get; private set; }
        public BodyData data;
        public static Animal Create()
        {
            var obj = new SimObject();
            return obj.AddComponent<Animal>();
        }
        public void Birth(ChildData data)
        {
            var childObj = new SimObject();
            Animal child = childObj.AddComponent<Animal>();
            child.Create(data.chromosome, data.genes);
        }
        public void Create(Chromosome chromosome, Genes genes)
        {
            data = new BodyData(this, chromosome, genes);
            PostNataity();
        }
        public void PostNataity()
        {
            gameObject.collider.radius = data.genes.modifications.sense * 10;
            UUID = $"{data.genes.id}:{Math.Random(-10000f, 10000f)}";
            gameObject.UUID = UUID;
            World.Instance.New(this, () =>
            {
                gameObject.name = $"{UUID}";
            });
        }
        public override void Update()
        {
            if (data.status.health == 0)
            {
                gameObject.Destroy();
            }
            else if (data.status.health > 0)
            {
                ReproductionUpdate();
                data.Update(this);
            }
        }
        public override void OnDestroy()
        {
            World.Instance.Dead(this);
            base.OnDestroy();
        }
        public bool Damaged(float damage, Animal attacker)
        {
            data.status.health -= damage;
            data.actions.OnAttack(attacker);
            return data.status.health <= 0;
        }

        #region Reproduction

        public ChildData childNode;
        public void ReproductionUpdate()
        {
            if(data.chromosome.type is Chromosome.Type.XX && childNode.inGestation)
            {
                if(childNode.natality < DateTime.Now)
                {
                    Conceve();
                }
            }
        }
        public void Conceve()
        {
            Animal child = new();
            child.Birth(childNode);
            childNode = new();
        }
        public void Reproduce(Animal partner)
        {
            if(CanReproduce(partner))
            {
                childNode = new ChildData(this, partner);
                UnityEngine.Debug.Log($"{UUID} reproduces with {partner.UUID}");

                data.status.reproductiveUrge = 0;
                partner.data.status.reproductiveUrge = 0;

                data.status.saturation = 0;
                partner.data.status.saturation = 0;

                data.status.stamina = 0;
                partner.data.status.stamina = 0;
            }
        }
        public bool CanReproduce(Animal life)
        {
            return life.data.chromosome.type != data.chromosome.type & Affinity(life) > 0f; 
        }
        public float Affinity(Animal life)
        {
            return 1;
        }
        #endregion

    }
}
