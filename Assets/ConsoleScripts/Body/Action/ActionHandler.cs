using System.Collections.Generic;

namespace Simulation
{
    public class ActionHandler
    {
        private readonly uint gameObjectHash;
        private SimObject gameObject => SimObject._objects[gameObjectHash];
        private Animal animal => gameObject.GetLife();
        public ActionType action { get; private set; }
        private SearchTarget _target;
        public SearchTarget target
        {
            get
            {
                return _target;
            }
        }
        public ActionHandler(Animal life)
        {
            gameObjectHash = life.gameObject.hash;
        }
        public void Update()
        {
            if (animal.data.status.health == 0)
            {

            }
            else
            {
                if (target != null && target.life != null)
                {
                    if (target.life.data.status.health == 0)
                    {
                        target.life = null;
                    }
                }
                switch (action)
                {
                    case ActionType.Idle:
                        {
                            IdleActions();
                        }
                        break;
                    case ActionType.Searching:
                        {
                            Search();
                        }
                        break;
                    case ActionType.Attacking:
                        {
                            Attacking();
                        }
                        break;
                    case ActionType.Fleeing:
                        {
                            //if at target stop fleeing
                            //if has been fleeing for too long, stop fleeing
                            if (Time.time > _target.time || IsAtTarget(_target.target))
                            {
                                action = ActionType.Idle;
                            }
                            else
                            {
                                animal.data.locomotion.MoveToTarget(animal, _target.target);

                            }
                        }
                        break;
                    case ActionType.Sleeping:
                        {
                            if ((animal.data.status.Stamina() < 1f || animal.data.status.Health() < 1f) && animal.data.status.Saturation() > .6f)
                            {
                                animal.data.status.stamina += animal.data.genes.modifications.recovery * Benchmarks.Instance.benchmark.recovery * Time.deltaTime;
                                animal.data.status.health += animal.data.genes.modifications.recovery * Benchmarks.Instance.benchmark.recovery * Time.deltaTime;
                            }
                            else
                            {
                                action = ActionType.Idle;
                            }
                        }
                        break;
                }
            }
        }
        void IdleActions()
        {
            var status = animal.data.status;
            _target = new SearchTarget { type = SearchTarget.Type.None };
            if (status.Saturation() < .5f)
            {
                action = ActionType.Searching;
                _target = new SearchTarget { life = null, type = SearchTarget.Type.Food};
            }
            else if (status.Reproduction() > .5f)
            {
                action = ActionType.Searching;
                _target = new SearchTarget { life = null, type = SearchTarget.Type.Mate };
            }
            else if (status.Stamina() < .1f)
            {
                //TODO: if enemy nearby, move to save position
                //TODO: at safe position, sleep
                action = ActionType.Sleeping;
            }
            else
            {
                action = ActionType.Searching;
                _target = new SearchTarget()
                {
                    life = null,
                    type = SearchTarget.Type.None, 
                    time = 0
                };
            }
        }
        public void Attack(Animal target)
        {
            if(target == animal)
            {
                return;
            }
            action = ActionType.Attacking;
            _target.life = target;
            _target.type = SearchTarget.Type.Food;
            _target.target = _target.life.transform.position + Vector3.XZ;
            target.Damaged(animal.data.genes.modifications.power * Benchmarks.Instance.benchmark.power * Time.deltaTime, animal);
            if (target.data.status.Health() == 0)
            {
                //If defeated opponent
                //gain all their nutrition
                //TODO: Use a stomach gene to improve digestion
                //TODO: Stomach gene affects ratio of nutrition gained
                animal.data.status.Eat(target.data.status.maxSaturation);
                UnityEngine.Debug.Log($"{this.animal.UUID} with {animal.data.status.health} hp eats {target.UUID} with {target.data.status.maxSaturation} nutrition");
            }
            else
            {
                UnityEngine.Debug.Log($"{this.animal.UUID} with {animal.data.status.health} hp attacks {target.UUID} with {target.data.status.health}");
            }
        }
        public void OnAttack(Animal target)
        {
            bool iPredator = StrengthScore(animal) > StrengthScore(target);
            if (action == ActionType.Attacking && iPredator && animal.data.status.Stamina() > .3f)
            {
                //on defense
                //continue attack
                _target.life = target;
                _target.type = SearchTarget.Type.Food;
                _target.target = target.transform.position + Vector3.XZ;
            }
            else
            {
                if(iPredator && animal.data.status.Stamina() > .3f)
                {
                    //on attacked by prey, counter attack
                    action = ActionType.Attacking;
                    _target.life = target;
                    _target.type = SearchTarget.Type.Food;
                    _target.target = _target.life.transform.position + Vector3.XZ;
                }
                else
                {
                    //on attacked by predator
                    //if can attack ? attack
                    //flee from predator
                    //Attack(target);
                    FleeFrom(target);
                }
            }
        }
        private void Attacking()
        {
            Animal target = _target.life;
            float time = _target.time;
            float stamina = animal.data.status.Stamina();
            if (stamina > 0 && target != null)
            {
                if(!CanAttack(animal, target))
                {
                    animal.data.locomotion.MoveToTarget(animal, target.transform.position + Vector3.XZ);
                }
                else
                {
                    if (Time.time > time)
                    {
                        _target.time = Time.time + 1f;
                        Attack(target);
                    }
                }
            }
            else
            {
                FleeFrom(target);
            }
        }
        public bool CanAttack(Animal self, Animal life)
        {
            return Vector3.Distance(life.transform.position, self.transform.position) < 2f;
        }
        public void Search()
        {
            action = ActionType.Searching;
            if(animal.data.status.Stamina() == 0)
            {
                action = ActionType.Sleeping;
                return;
            }
            //if has search type in mind, find target
            if (_target.type is SearchTarget.Type.Food or SearchTarget.Type.Mate)
            {
                SearchFoodMate();
            }
            else
            {
                //if searching for something other than food or mate like sleeping spot
                if (_target.type is not SearchTarget.Type.None && Time.time < _target.time)
                {
                    if (_target.life != null)
                    {
                        //track target current position
                        _target.target = _target.life.transform.position + Vector3.XZ;
                    }
                    animal.data.locomotion.MoveToTarget(animal, _target.target);
                    if (IsAtTarget(_target.target))
                    {
                        //TODO: Add valid response for search type
                        action = ActionType.Idle;
                    }
                }
                else
                {
                    var list = GetInRange();
                    if (Time.time < target.time)
                    {
                        if(list.Count == 0)
                        {
                            animal.data.locomotion.MoveToTarget(animal, target.target);
                            return;
                        }
                    }
                    //if target fouund in aimless search analyze
                    foreach (var life in list)
                    {
                        Animal target = life.Item2;
                        if (IsFood(target))
                        {
                            action = ActionType.Searching;
                            _target = new SearchTarget { life = target, target = target.transform.position, time = Time.time + 60f, type = SearchTarget.Type.Food };
                            return;
                        }
                        else if (IsMate(target, out bool canMate))
                        {
                            action = ActionType.Searching;
                            _target = new SearchTarget { life = target, target = target.transform.position, time = Time.time + 30f, type = SearchTarget.Type.Mate };
                            return;
                        }
                    }

                    //if no target is found
                    //continue aimless search
                    float range = (animal.data.genes.modifications.sense * Benchmarks.Instance.benchmark.sense) + 100f;
                    float time = Time.time + 5f;
                    //find a target within range at least half of range not far from world origin
                    Vector3 targetPosition = Vector3.Rand(-range, range, Vector3.XZ, range * .5f);
                    _target = new SearchTarget
                    {
                        life = null,
                        target = targetPosition,
                        type = SearchTarget.Type.None,
                        time = time
                    };
                }
            }
        }
        private List<(float, Animal)> GetInRange()
        {
            var inRange = animal.gameObject.collider.inRange;
            List<(float, Animal)> list = new();
            foreach (var r in inRange.contacts)
            {
                Animal visualTarget = r.collider.gameObject.GetLife();
                list.Add((r.distance, visualTarget));
            }
            list.Sort((a, b) =>
            {
                return a.Item1.CompareTo(b.Item1);
            });
            return list;
        }
        private void SearchFoodMate()
        {
            //move towards target
            if(_target.life != null)
            {
                _target.target = _target.life.transform.position + Vector3.XZ;
                animal.data.locomotion.MoveToTarget(animal, _target.target);
                if (IsAtTarget(_target.target))
                {
                    if (_target.type == SearchTarget.Type.Food)
                    {
                        if (CanAttack(animal, _target.life))
                        {
                            if (IsFood(_target.life))
                            {
                                Attack(_target.life);
                                //caught prey
                                //increase speed gene based on how long it takes to catch prey
                            }
                            else
                            {
                                //flee from prey
                                //improve power gene in gestation
                                FleeFrom(_target.life);
                            }
                            return;
                        }
                        else
                        {
                            //can no longer attack prey
                            //is weak
                            //increase stamina gene in gestation
                        }
                    }
                    else if(_target.type == SearchTarget.Type.Mate)
                    {
                        if (IsMate(_target.life, out bool canMate))
                        {
                            if (canMate)
                            {
                                if (animal.data.chromosome.type == Chromosome.Type.XX)
                                {
                                    animal.Reproduce(_target.life);
                                }
                                else
                                {
                                    target.life.Reproduce(animal);
                                }
                            }
                            else
                            {
                                //meets mate but cannot mate
                                UnityEngine.Debug.Log($"rejected");
                            }
                            action = ActionType.Idle;
                        }
                    }
                }
                else
                {
                    //if not at target and target not out of range
                    //TODO: add choice later on

                    if (Time.time < _target.time)
                    {

                    }
                    else
                    {
                        //failed to catch prey
                        //increase speed gene in gestation
                        NewSearchTarget();
                    }
                }
            }
            else
            {
                animal.data.locomotion.MoveToTarget(animal, _target.target);
                var inRange = animal.gameObject.collider.inRange;
                bool targetInRange = false;

                List<(float, Animal)> list = new();
                foreach (var r in inRange.contacts)
                {
                    Animal visualTarget = r.collider.gameObject.GetLife();
                    if (_target.type is SearchTarget.Type.Food && IsFood(visualTarget))
                    {
                        if (visualTarget == _target.life)
                        {

                        }
                        targetInRange = true;
                        list.Add((r.distance, visualTarget));
                    }
                    else if (_target.type is SearchTarget.Type.Mate && IsMate(visualTarget, out bool canMate))
                    {
                        if (visualTarget == _target.life)
                        {

                        }
                        targetInRange = true;
                        list.Add((r.distance, visualTarget));
                    }
                }
                if (targetInRange && list.Count > 0)
                {
                    //sort list to look at closest target
                    list.Sort((a, b) =>
                    {
                        return a.Item1.CompareTo(b.Item1);
                    });
                    //look at closest target
                    foreach (var l in list)
                    {
                        var target = l.Item2;
                        var isFood = IsFood(target);
                        var isMate = IsMate(target, out bool canMate);
                        if (_target.type is SearchTarget.Type.Food)
                        {
                            if (isFood)
                            {
                                _target = new SearchTarget()
                                {
                                    life = target,
                                    time = Time.time + 30,
                                    type = SearchTarget.Type.Food
                                };
                                break;
                            }
                        }
                        else
                        {
                            if (canMate)
                            {
                                _target = new SearchTarget()
                                {
                                    life = target,
                                    time = Time.time + 30,
                                    type = SearchTarget.Type.Mate
                                };
                                break;
                            }
                        }
                    }
                    if (_target.life == null && Time.time > target.time)
                    {
                        //find new target is no target in range after searching for particular time
                        NewSearchTarget();
                    }
                }
                else
                {
                    if (Time.time > target.time)
                    {
                        NewSearchTarget();
                    }
                }
            }

        }
        private void NewSearchTarget()
        {
            target.life = null;
            target.type = GetSearchType();
            target.time = Time.time + 10;
            target.target = Vector3.Rand(-100, 100, Vector3.XZ);
        }
        private SearchTarget.Type GetSearchType()
        {
            if(animal.data.status.Saturation() < .4f)
            {
                return SearchTarget.Type.Food;
            }else if(animal.data.status.Reproduction() >= 1f)
            {
                return SearchTarget.Type.Mate;
            }
            if(animal.data.status.Reproduction() == 0)
            {
                return SearchTarget.Type.None;
            }
            return Math.RandomInt(1, 8) == 1 ? SearchTarget.Type.Mate : SearchTarget.Type.Food; 
        }
        public bool IsAtTarget(Vector3 target)
        {
            return Vector3.Distance(target, animal.transform.position) < 2;
        }
        public bool IsFood(Animal life)
        {
            if(life == null)
            {
                return false;
            }
            if(life.data.genes.id == this.animal.data.genes.id)
            {
                return false;
            }
            return StrengthScore(this.animal) > StrengthScore(life);
        }
        public static float StrengthScore(Animal life)
        {
            if (life == null)
            {
                return 0;
            }
            return (life.data.genes.modifications.power * life.data.status.stamina * life.data.status.health);
        }
        public bool IsMate(Animal life, out bool canMate)
        {
            canMate = false;
            if (life == null)
            {
                return false;
            }
            if (life.data.genes.id != this.animal.data.genes.id)
            {
                return false;
            }
            if (life.data.chromosome.type == this.animal.data.chromosome.type)
            {
                return false;
            }
            if (this.animal.data.status.Reproduction() == 0)
            {
                return false;
            }
            if (life.data.status.Reproduction() == 0)
            {
                return false;
            }
            canMate = this.animal.data.status.Reproduction() >= 1 && life.data.status.Reproduction() >= 1;
            return true;
        }


        public void FleeFrom(Animal life)
        {
            action = ActionType.Fleeing;
            _target.life = life;
            _target.type = SearchTarget.Type.Predator;
            _target.target = Vector3.Rand(-200, 200, Vector3.XZ, 80f);
            _target.time = Time.time + 20f;
            UnityEngine.Debug.Log($"{this.animal.UUID} flees from {life.UUID}");
        }
    }

    [System.Serializable]
    public class SearchTarget
    {
        public Animal life { get; set; }
        public Vector3 target;
        public float time;
        public Type type;
        public enum Type { None, Target, Food, Mate, Predator}
    }

    public enum ActionType { Idle, Attacking, Searching, Fleeing, Sleeping }
}