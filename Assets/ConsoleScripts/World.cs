using System;
using System.Collections.Generic;

namespace Simulation
{
    public class World
    {
        public static World Instance;
        public Random random;
        Database db;
        Queue<Action> dataBaseAction;
        Action<SimObject> onCreate, onDestroy;
        public World(Action<SimObject> onCreate, Action<SimObject> onDestroy)
        {
            Instance = this;
            db = new();
            dataBaseAction = new();
            this.onCreate = onCreate;
            this.onDestroy = onDestroy;
            Time.Init();
            random = new Random(1337);
        }
        public void Update()
        {
            SimObject.Update();
            DatabaseUpdate();
        }
        public void DatabaseUpdate()
        {
            while(dataBaseAction.Count > 0)
            {
                dataBaseAction.Dequeue()?.Invoke();
            }
            Time.Update();
        }
        public void New(Animal life, Action start)
        {
            dataBaseAction.Enqueue(() =>
            {
                db.Add(life, life.UUID.Split(':')[0]);
                start?.Invoke();
                onCreate?.Invoke(life.gameObject);
            });
        }
        public void Dead(Animal life)
        {
            onDestroy?.Invoke(life.gameObject);
            db.database[life.data.genes.id].Remove(life);
            life = null;
        }
        public List<Animal> GetAll()
        {
            List<Animal> all = new();
            foreach(var k in db.database)
            {
                all.AddRange(k.Value);
            }
            return all;
        }
        public List<Animal> Get(string hash)
        {
            return db.database[hash];
        }
        public class Database
        {
            public Dictionary<string, List<Animal>> database = new();
            public Database()
            {
                database = new();
            }
            public List<Animal> GetObject(string species)
            {
                if(database.TryGetValue(species, out var list))
                {
                    return list;
                }
                return new();
            }
            public bool Contains(string species)
            {
                return database.ContainsKey(species);
            }
            public void Add(Animal value, string species)
            {
                if(!Contains(species))
                {
                    database.Add(species, new());
                }
                database[species].Add(value);
            }
        }
    }
}
