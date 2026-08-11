using System;

using UnityEngine;

namespace Simulation.Unity
{
    public class GameWorld : MonoBehaviour
    {
        public int initialPopulation;
        World world;
        public void Begin()
        {
            var species = SpeciesCreator.Instance.GetSimulationSpecies();

            if(species.Count < 2)
            {
                return;
            }

            Action<SimObject> onCreate = (SimObject obj) =>
            {
                MainThreadDispatcher.QueueMain(() =>
                {
                    OnCreateObject(obj);
                });
            };
            Action<SimObject> onDestroy = (SimObject obj) =>
            {
                MainThreadDispatcher.QueueMain(() =>
                {
                    OnDestroyObject(obj);
                });
            };

            world = new(onCreate, onDestroy);

            SpeciesBank bank = new SpeciesBank(SpeciesLibrary.Instance.species);
            foreach (var specie in species)
            {
                for (int number = 0; number < initialPopulation; number++)
                {
                    Animal member = Animal.Create();
                    member.Create(Chromosome.Rand(), specie.FirstGeneration());
                }
            }
        }
        public void OnCreateObject(SimObject obj)
        {
            SimMono mono = Instantiate(SpeciesCreator.Instance.UI.lifePrefab, transform);
            mono.Set(obj.hash, Instantiate(SpeciesCreator.Instance.GetModel(0)));
        }
        public void OnDestroyObject(SimObject obj)
        {
            var objs = FindObjectsOfType<SimMono>();
            foreach(var o in objs)
            {
                if(o.hash == obj.hash)
                {
                    Destroy(o.gameObject);
                    return;
                }
            }
        }
        private void Update()
        {
            world?.Update();
        }
    }
}