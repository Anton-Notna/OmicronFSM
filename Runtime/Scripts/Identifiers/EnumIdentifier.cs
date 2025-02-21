using System;

namespace OmicronFSM
{
    public class EnumIdentifier<T> : IIdentifier where T : Enum
    {
        public int Id { get; private set; }

        public string Name { get; private set; }

        public EnumIdentifier(T value)
        {
            Name = value.ToString();
            Id = (int)Enum.Parse(typeof(T), value.ToString());
        }
    }

}