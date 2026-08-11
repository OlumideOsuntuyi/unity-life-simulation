namespace Simulation
{
    [System.Serializable]
    public struct GeneStrength
    {
        public Alleles alleles;
        public GeneStrength(Alleles alleles)
        {
            this.alleles = alleles;
        }
        public GeneStrength Increase()
        {
            if(alleles is Alleles.Extinct)
            {
                return new GeneStrength(Alleles.Extinct);
            }
            if(alleles is Alleles.Recessive)
            {
                return new GeneStrength(Alleles.Tt);
            }
            return new GeneStrength(Alleles.Dominant);
        }
        public GeneStrength Reduce()
        {
            if(alleles is Alleles.Dominant)
            {
                return new GeneStrength(Alleles.Tt);
            }else if(alleles is Alleles.Tt)
            {
                return new GeneStrength(Alleles.Recessive);
            }
            return new GeneStrength(Alleles.Extinct);
        }
        public static GeneStrength CombineType(GeneStrength a, GeneStrength b)
        {
            Alleles alleles = Alleles.Tt;
            switch (a.alleles)
            {
                case Alleles.Dominant:
                    {
                        switch (b.alleles)
                        {
                            case Alleles.Dominant:
                                {
                                    alleles = Alleles.Dominant;
                                }
                                break;
                            case Alleles.Tt:
                                {

                                }
                                break;
                            case Alleles.Recessive:
                                {

                                }
                                break;
                        }
                    }
                    break;
                case Alleles.Tt:
                    {
                        switch (b.alleles)
                        {
                            case Alleles.Dominant:
                                {

                                }
                                break;
                            case Alleles.Tt:
                                {
                                    alleles = Rand();
                                }
                                break;
                            case Alleles.Recessive:
                                {

                                }
                                break;
                        }
                    }
                    break;
                case Alleles.Recessive:
                    {
                        switch (b.alleles)
                        {
                            case Alleles.Dominant:
                                {

                                }
                                break;
                            case Alleles.Tt:
                                {

                                }
                                break;
                            case Alleles.Recessive:
                                {
                                    alleles = Alleles.Recessive;
                                }
                                break;
                        }
                    }
                    break;
            }
            return new GeneStrength(alleles);
        }
        public static Alleles Rand()
        {
            return Math.RandomInt(0, 2) switch
            {
                0 => Alleles.Dominant,
                1 => Alleles.Tt,
                2 => Alleles.Recessive,
                _ => Alleles.Tt,
            };
        }
        public Alleles Next()
        {
            return alleles is Alleles.Dominant ? Alleles.Tt : Alleles.Recessive;
        }
    }
    /// <summary>
    /// The strength of a gene
    /// </summary>
    public enum Alleles
    {
        Dominant, Tt, Recessive, Extinct
    }
}