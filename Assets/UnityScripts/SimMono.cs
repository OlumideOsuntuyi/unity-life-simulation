using System.Collections.Generic;

using TMPro;

using UnityEngine;

namespace Simulation.Unity
{
    public class SimMono : MonoBehaviour
    {
        public SimObject simObject => SimObject._objects[hash];
        public uint hash;
        public Animal animal;
        public LifeModel model;
        public UnityEngine.Vector3 force;
        public bool addForce;
        public bool addTorque;
        public LifeInfo info;
        public UIElements UI;
        private void Awake()
        {
            info = new();
        }
        private void Update()
        {
            if(SimObject._objects.ContainsKey(hash))
            {
                transform.position = simObject.transform.position.ToUnity();
                transform.eulerAngles = simObject.transform.rotation.ToUnity();
                transform.eulerAngles = new UnityEngine.Vector3(0, transform.eulerAngles.y, 0);
                if (addForce)
                {
                    simObject.rigidBody.AddForce(new Vector3(force.x, force.y, force.z) * Time.deltaTime);
                }
                if(addTorque)
                {
                    simObject.rigidBody.AddTorque(new Vector3(force.x, force.y, force.z) * Time.deltaTime);
                }
                if(animal != null && animal.data != null && animal.data.status != null)
                {
                    info.Update(animal, UI);
                    var relationships = info.relationships;
                    var friends = relationships.FindAll(f => f.type == Relationship.Type.Friend);
                    var enemies = relationships.FindAll(f => f.type == Relationship.Type.Enemy);
                    UI.friends.text = friends == null || friends.Count == 0 ? "0" : $"{friends.Count}";
                    UI.enemies.text = enemies == null || enemies.Count == 0 ? "0" : $"{enemies.Count}";
                }
            }
        }
        public void Set(uint hash, LifeModel model)
        {
            this.hash = hash;
            this.model = model;
            model.transform.parent = transform;
            model.transform.localPosition = new();
            model.Set(hash);
            GetLife();
        }
        void GetLife()
        {
            name = simObject.UUID;
            animal = simObject.GetLife();
        }
        private void OnDrawGizmos()
        {
            try
            {
                if (animal != null && SimObject._objects.Count > 0 && Application.isPlaying)
                {
                    Gizmos.color = Color.white;
                    var ori = simObject.transform.position.ToUnity();
                    var des = ori + (simObject.transform.Forward().ToUnity() * animal.data.genes.modifications.sense * 10);
                    //Gizmos.DrawLine(ori, des);


                    Gizmos.color = Color.white;
                    Gizmos.DrawWireCube(animal.gameObject.transform.position.ToUnity(), animal.gameObject.collider.size.ToUnity());
                    //Gizmos.DrawWireSphere(life.gameObject.transform.position.ToUnity(), life.gameObject.collider.radius);


                    if (animal.data.actions.target != null)
                    {
                        Gizmos.color = Color.red;
                        var target = animal.data.actions.target.target.ToUnity();
                        Gizmos.DrawLine(ori, target);
                    }
                }
            }
            catch (System.Exception)
            {

            }
        }

        [System.Serializable]
        public struct LifeInfo
        {
            public float hp;
            public float sta;
            public float saturation, reproductive;
            public ActionType action;
            public string UUID;
            public SearchTarget searchType;
            public Simulation.GenePotential mods;
            public Collider.Contacts range, contacts;

            public float mass, density;
            public UnityEngine.Vector3 position, rotation, velocity, angularVelocity;
            public int componentCount;
            public List<Relationship> relationships;
            public void Update(Animal life, UIElements UI)
            {
                if(life == null || life.data == null || life.data.status == null)
                {
                    return;
                }
                var status = life.data.status;
                hp = status.health / status.maxHealth;
                sta = status.stamina / status.maxStamina;
                reproductive = status.reproductiveUrge / status.maxReproductiveUrge;
                saturation = status.saturation / status.maxSaturation;
                action = life.data.actions.action;
                mods = life.data.genes.modifications;
                range = life.gameObject.collider.inRange;
                componentCount = life.gameObject.componentCount;
                contacts = life.gameObject.collider.contacts;

                mass = life.gameObject.rigidBody.mass;
                density = life.gameObject.rigidBody.density;
                position = life.transform.position.ToUnity();
                rotation = life.transform.rotation.ToUnity();
                velocity = life.gameObject.rigidBody.velocity.ToUnity();
                angularVelocity = life.gameObject.rigidBody.angularVelocity.ToUnity();
                relationships = life.data.memory.relationship.relationshipsList;

                searchType = life.data.actions.target;
                if(searchType.life == null)
                {
                    UUID = "";
                }
                else
                {
                    UUID = searchType.life.UUID;
                }

                UI.health.currentValue = hp;
                UI.stamina.currentValue = sta;
                UI.reproduction.currentValue = reproductive;
                UI.hunger.currentValue = saturation;
            }
        }
        [System.Serializable]
        public class UIElements
        {
            public ProgressBar health, stamina, hunger, reproduction;
            public TMP_Text friends, enemies;
        }
    }
}