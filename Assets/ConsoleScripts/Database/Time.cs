using System;

namespace Simulation
{
    public static class Time
    {
        private static DateTime init, prevUpdate;
        private static float _deltaTime, _time;
        public static float time
        {
            get
            {
                return _time;
            }
        }
        public static float deltaTime
        {
            get
            {
                return _deltaTime;
            }
        }
        public static void Init()
        {
            init = DateTime.Now;
            prevUpdate = init;
        }
        public static void Update()
        {
            _deltaTime = TimeFunctions.GetMilliseconds(prevUpdate, DateTime.Now) * 0.001f;
            _time = TimeFunctions.GetMilliseconds(init, DateTime.Now) * 0.001f;
            prevUpdate = DateTime.Now;
        }
    }
}