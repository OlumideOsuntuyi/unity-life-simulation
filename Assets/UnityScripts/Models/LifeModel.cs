using UnityEngine;

namespace Simulation.Unity
{
    public class LifeModel : MonoBehaviour
    {
        public Animator animator;
        public uint hash;
        public SimObject simObject => SimObject._objects[hash];
        public void Set(uint hash)
        {
            this.hash = hash;
        }
        private void Update()
        {
            try
            {
                var action = simObject.GetLife().data.actions;

                bool idle = action.action == ActionType.Idle;
                bool sit = action.action == ActionType.Sleeping;
                bool run = action.action == ActionType.Searching;
                bool walk = false;
                bool creep = action.action == ActionType.Attacking;

                animator.SetBool("idle", idle);
                animator.SetBool("sit", sit);
                animator.SetBool("run", run);
                animator.SetBool("walk", walk);
                animator.SetBool("creep", creep);
            }
            catch (System.Exception)
            {

            }
        }
    }
}
