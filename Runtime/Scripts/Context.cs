using UnityEngine;

namespace OmicronFSM
{
    public class Context
    {
        public GameObject GameObject { get; private set; }

        public Transform Transform { get; private set; }

        public Context(GameObject owner) 
        {
            GameObject = owner;
            Transform = owner.transform;
        }
    }
}