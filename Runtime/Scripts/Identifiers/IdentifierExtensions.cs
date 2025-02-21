namespace OmicronFSM
{
    public static class IdentifierExtensions
    {
        public static bool Same(this IIdentifier identifier0, IIdentifier identifier1)
             => identifier0.GetType().Equals(identifier1.GetType()) && identifier0.Id.Equals(identifier1.Id);
    }

}