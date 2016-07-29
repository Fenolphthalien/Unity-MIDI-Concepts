namespace PrototypeFour
{
    [System.Serializable]
    public struct Constraint
    {
        public float min, max;

        public Constraint(float _min, float _max)
        {
            min = _min;
            max = _max;
        }

        public static Constraint Identity()
        {
            return new Constraint(0, 1);
        }

        public bool InConstraint(float value)
        {
            return value > min && value < max;
        }

        public bool InConstraint(float value, out float overShoot)
        {
            bool inrange = value > min && value < max;
            if (value <= min)
                overShoot = value - min;
            else if (value >= min)
                overShoot = value - max;
            else
                overShoot = 0;
            return inrange;
        }

        public float Evaluate(float value)
        {
            float overShoot = 0;
             if (value <= min)
                overShoot = value - min;
            else if (value >= min)
                overShoot = value - max;
             return value - overShoot;
        }
    }
}