namespace Marine.Redux.API
{
    public class Shield
    {
        public Shield()
        {

        }

        public Shield(float amount, float limit, float decay, float efficacy, float sus, bool persistent)
        {
            Amount = amount;
            Limit = limit;
            Decay = decay;
            Efficacy = efficacy;
            Sustain = sus;
            Persistent = persistent;
        }

        public float Amount { get; set; }

        public float Limit { get; set; }

        public float Decay { get; set; }

        public float Efficacy { get; set; }

        public float Sustain { get; set; }

        public bool Persistent { get; set; }
    }
}
