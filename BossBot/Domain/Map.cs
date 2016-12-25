namespace BossBot
{
    public class Map
    {
        public string Name { get; set; }

        public Map()
        {

        }

        public override bool Equals(object obj)
        {
            Map m = obj as Map;
            if(m != null)
            {
                return Name.ToLower().Equals(m.Name.ToLower());
            }
            return false;
        }
        public bool NameContains(Map map)
        {
            return Name.ToLower().Contains(map.Name.ToLower());
        }
        public override string ToString()
        {
            return Name;
        }
    }
}