using Microsoft.Extensions.DependencyInjection;

namespace StickyNotes.Core.Mvvm
{
    public interface IContainer
    {
        void AddSingleton<S, T>() where S : class where T : class, S;
        void AddSingleton<T>(T implementationInstance) where T : class;
        void AddSingleton<S>(Func<IContainer, S> implementationFactory) where S : class;
        void AddTransient<S, T>() where S : class where T : class, S;
        void AddTransient<T>() where T : class;
        void Build();
        T Resolve<T>() where T : class;
        T Resolve<T>(string typeName) where T : class;

    }

    public class Container : IContainer
    {
        public static readonly IContainer Default = new Container();

        private readonly ServiceCollection _services = new();
        private IServiceProvider _provider;

        public void AddSingleton<S, T>()
            where S : class
            where T : class, S
        {
            _services.AddSingleton<S, T>();
        }

        public void AddSingleton<S>(Func<IContainer, S> implementationFactory)
            where S : class
        {
            _services.AddSingleton<S>(x => implementationFactory(this));
        }

        public void AddSingleton<T>(T implementationInstance) where T : class
        {
            _services.AddSingleton(implementationInstance);
        }

        public void AddTransient<S, T>()
            where S : class
            where T : class, S
        {
            _services.AddTransient<S, T>();
        }

        public void AddTransient<T>() where T : class
        {
            _services.AddTransient<T>();
        }

        public void Build()
        {
            _provider = _services.BuildServiceProvider();

        }

        public T Resolve<T>() where T : class
        {
            return _provider?.GetService<T>();
        }

        public T Resolve<T>(string typeName) where T : class
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(typeName, false);
                if (type != null)
                {
                    var instance = (T)_provider?.GetService(type);
                    //if (instance == null)
                    //{
                    //    instance = (T)Activator.CreateInstance(typeof(T));
                    //}
                    return instance;
                }
            }

            return null;
        }
    }
}
