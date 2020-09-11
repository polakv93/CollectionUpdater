using System;
using System.Collections.Generic;
using System.Linq;

namespace CollectionUpdater
{
    public class CollectionUpdater<TSrc, TDest> where TDest : new() where TSrc : class
    {
        private ICollection<TDest> _destination = new List<TDest>();
        private ICollection<TSrc> _source = new List<TSrc>();

        private Func<TSrc, TDest, bool> _pairFunc = (_, __)
            => throw new NotImplementedException("You must add pair function in CollectionUpdater.PairUsing(Func<TSrc, TDest, bool> pairFunc)");

        private Action<TSrc, TDest> _mapFunc = (_, __)
            => throw new NotImplementedException("You must add mapFunc in CollectionUpdater.SetMapFunc(Action<TSrc, TDest> mapFunc)");

        private Action<TSrc, TDest> _doOnAddedDest = (_, __) => { };

        private Action<TDest> _doOnRemovedDest = _ => { };

        public CollectionUpdater<TSrc, TDest> SetDestination(ICollection<TDest> entities)
        {
            _destination = entities;
            return this;
        }

        public CollectionUpdater<TSrc, TDest> SetSource(ICollection<TSrc> viewModels)
        {
            _source = viewModels;
            return this;
        }

        /// <summary>
        /// Set function used for pair elements between source and destination
        /// ex: (src, dest) => src.Id == dest.Id
        /// </summary>
        /// <param name="pairFunc"></param>
        /// <returns></returns>
        public CollectionUpdater<TSrc, TDest> PairUsing(Func<TSrc, TDest, bool> pairFunc)
        {
            _pairFunc = pairFunc;
            return this;
        }

        /// <summary>
        /// Set function used for mapping data from source to destination item
        /// ex: (src, dest) => dest.Data = src.Data
        /// </summary>
        /// <param name="mapFunc"></param>
        /// <returns></returns>
        public CollectionUpdater<TSrc, TDest> SetMapFunc(Action<TSrc, TDest> mapFunc)
        {
            _mapFunc = mapFunc;
            return this;
        }

        /// <summary>
        /// ex: (src, dest) => dest.Data = data.Data + 1
        /// </summary>
        /// <param name="doOnAddedEntity"></param>
        /// <returns></returns>
        public CollectionUpdater<TSrc, TDest> DoOnAddedItem(Action<TSrc, TDest> doOnAddedEntity)
        {
            _doOnAddedDest = doOnAddedEntity;
            return this;
        }

        public CollectionUpdater<TSrc, TDest> DoOnRemovedItem(Action<TDest> doOnRemovedEntity)
        {
            _doOnRemovedDest = doOnRemovedEntity;
            return this;
        }

        public void Update()
        {
            foreach (var dest in _destination.ToList())
            {
                var srcEq = _source.SingleOrDefault(src => _pairFunc(src, dest));
                if (srcEq == null)
                {
                    RemoveDest(dest);
                    continue;
                }

                UpdateDest(srcEq, dest);
            }
            
            var toAdd = new List<TSrc>();
            foreach (var src in _source)
            {
                var destEq = _destination.SingleOrDefault(dest => _pairFunc(src, dest));
                if (destEq == null)
                {
                    toAdd.Add(src);
                }
            }

            toAdd.ForEach(Add);
        }

        private void Add(TSrc src)
        {
            var newDest = new TDest();
            _mapFunc(src, newDest);
            _destination.Add(newDest);
            _doOnAddedDest(src, newDest);
        }

        private void UpdateDest(TSrc src, TDest dest)
            => _mapFunc(src, dest);

        private void RemoveDest(TDest dest)
            => _doOnRemovedDest(dest);
    }
}
