using System.Collections.Generic;

namespace CollectionUpdater
{
    public static class CollectionExtensionMethods
    {
        public static CollectionUpdater<TSrc, TDest> AsCollectionUpdater<TSrc, TDest>(this ICollection<TDest> destination, ICollection<TSrc> source) where TDest : new() where TSrc : class
            => new CollectionUpdater<TSrc, TDest>()
                .SetDestination(destination)
                .SetSource(source);
    }
}