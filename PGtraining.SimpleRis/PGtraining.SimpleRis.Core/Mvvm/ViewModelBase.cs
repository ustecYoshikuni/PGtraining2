using Prism.Mvvm;
using Prism.Navigation;

namespace PGtraining.SimpleRis.Core.Mvvm
{
    public abstract class ViewModelBase : BindableBase, IDestructible
    {
        protected ViewModelBase()
        {
        }

        public virtual void Destroy()
        {
        }
    }
}