namespace OmicronFSM
{
    public class StringIdentifier : IIdentifier
    {
        public int Id { get; private set; }

        public string Name { get; private set; }

        public StringIdentifier(string name)
        {
            Name = name;
            Id = name.GetHashCode();
        }
    }

}