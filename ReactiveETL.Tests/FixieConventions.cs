using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Fixie;

namespace ReactiveETL.Tests
{
    public class FixieConventions : Convention
    {
        private static readonly Dictionary<MethodInfo, object> fixtures = new Dictionary<MethodInfo, object>();

        public FixieConventions()
        {
            Classes
                 .Where(HasAnyFactMethods);

            Methods
                .HasOrInherits<FactAttribute>();

            ClassExecution
                .CreateInstancePerCase()
                .Wrap<PrepareAndDisposeFixtureData>()
                .ShuffleCases();

            FixtureExecution
                .Wrap<InjectFixtureData>();
        }

        private bool HasAnyFactMethods(Type type)
        {
            return type.GetMethods(BindingFlags.Public | BindingFlags.Instance).Any(x => x.HasOrInherits<FactAttribute>());
        }

        internal class PrepareAndDisposeFixtureData : ClassBehavior
        {
            public void Execute(Class testClass, Action next)
            {
                SetUp(testClass);
                next();
                TearDown();
            }

            void SetUp(Class testClass)
            {
                fixtures.Clear();

                foreach (var @interface in FixtureInterfaces(testClass.Type))
                {
                    var fixtureDataType = @interface.GetGenericArguments()[0];

                    var fixtureInstance = Activator.CreateInstance(fixtureDataType);

                    var method = @interface.GetMethod("SetFixture", new[] { fixtureDataType });
                    fixtures[method] = fixtureInstance;
                }
            }

            void TearDown()
            {
                foreach (var fixtureInstance in fixtures.Values)
                {
                    var disposable = fixtureInstance as IDisposable;
                    if (disposable != null)
                        disposable.Dispose();
                }

                fixtures.Clear();
            }
        }

        internal class InjectFixtureData : FixtureBehavior
        {
            public void Execute(Fixture fixture, Action next)
            {
                foreach (var injectionMethod in fixtures.Keys)
                    injectionMethod.Invoke(fixture.Instance, new[] { fixtures[injectionMethod] });

                next();
            }
        }

        private static IEnumerable<Type> FixtureInterfaces(Type testClass)
        {
            return testClass.GetInterfaces()
                            .Where(@interface => @interface.IsGenericType &&
                                                 @interface.GetGenericTypeDefinition() == typeof(IUseFixture<>));
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class FactAttribute : Attribute { }

    public interface IUseFixture<T> where T : new()
    {
        void SetFixture(T data);
    }
}
