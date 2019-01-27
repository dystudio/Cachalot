using System.Collections.Generic;
using Client.Core;
using Client.Interface;

namespace Server
{
    /// <summary>
    /// Base class for eviction policies
    /// An eviction policy is responsible for deciding if eviction is needed and which items need to be evicted
    /// </summary>
    public abstract class EvictionPolicy
    {
        public abstract EvictionType Type { get; }

        /// <summary>
        /// Returns true if items need to be removed from the cache
        /// </summary>
        public virtual bool IsEvictionRequired
        {
            get { return false; }
        }


        public virtual void AddItem(CachedObject item)
        {
            //ignore in the base class
        }

        /// <summary>
        /// Get the items to remove according to the current policy. They are already removed from the internal data
        /// structures of the policy
        /// </summary>
        /// <returns></returns>
        public virtual IList<CachedObject> DoEviction()
        {
            return new List<CachedObject>();
        }

        /// <summary>
        /// The specified item was accessed, update its priority accordingly
        /// </summary>
        /// <param name="item"></param>
        public virtual void Touch(CachedObject item)
        {
            //ignore in the base class
        }

        /// <summary>
        /// This item is not present in the cache any more. The eviction policy should not compute
        /// its eviction priority any more
        /// </summary>
        /// <param name="item"></param>
        public virtual void TryRemove(CachedObject item)
        {
            //ignore in the base class
        }


        public virtual void Touch(IList<CachedObject> items)
        {
            //ignore in the base class
        }

        public virtual void Clear()
        {
            //ignore in the base class
        }
    }
}