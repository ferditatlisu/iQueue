using System;
using System.Collections.Generic;
using System.Text;

namespace iModel.Customs
{
    public class LazyQueue<T> : Lazy<T>, IDisposable where T : class, IDisposable
    {
        public LazyQueue(Func<T> valueFactory) : base(valueFactory)
        { 
            
        }

        public virtual void Dispose()
        {
            if (IsValueCreated)
                Value.Dispose();
        }
    }
}
