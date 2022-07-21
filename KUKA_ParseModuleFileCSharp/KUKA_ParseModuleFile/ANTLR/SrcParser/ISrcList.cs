using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseModuleFile.ANTLR.SrcParser
{
    public interface ISrcList<T> : ICollection<T>, INotifyCollectionChanged, INotifyPropertyChanged, ISrcItemBlock
    {
    }
}
