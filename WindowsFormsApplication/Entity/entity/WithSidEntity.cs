namespace Entity.entity
{
    public class WithSidEntity : BaseEntity
    {
        private string sid;
        private string lastSidPart;

        public WithSidEntity() { }
        public WithSidEntity(string name, string sid) : base(name)
        {
            this.sid = sid;
            fillLastSidPart();
        }

        public string Sid
        {
            get
            {
                return sid;
            }

            set
            {
                sid = value;
            }
        }

        public string LastSidPart
        {
            get
            {
                return lastSidPart;
            }

            set
            {
                lastSidPart = value;
            }
        }

        private void fillLastSidPart()
        {
            LastSidPart = getLastValueFromSid(Sid);
        }

        public static string getLastValueFromSid(string sid)
        {
            string[] sidValues = sid.Trim().Split('-');
            return sidValues[sidValues.Length - 1];
        }

        public override string ToString()
        {
            return "[Name: " + Name + "; Sid: " + Sid + "]";
        }

    }
}
