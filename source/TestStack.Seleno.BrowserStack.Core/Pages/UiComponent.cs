using System;
using System.Linq;
using System.Reflection;
using IWait = TestStack.Seleno.BrowserStack.Core.Actions.IWait;
using Wait = TestStack.Seleno.BrowserStack.Core.Actions.Wait;

namespace TestStack.Seleno.BrowserStack.Core.Pages
{
    public abstract class UiComponent : PageObjects.UiComponent
    {
        private IWait _waitFor;

        protected new IWait WaitFor
        {
            get { return _waitFor ?? (_waitFor = new Wait(Execute)); }
        }

      protected internal TBaseComponent GetResponsiveComponent<TBaseComponent, TComponent>(bool useOriginalComponent)
           where TComponent : TBaseComponent, new()
           where TBaseComponent : UiComponent
        {
            var componentType = typeof(TComponent);
            if (useOriginalComponent)
            {
                return GetComponent<TComponent>();
            }

          var responsiveComponentType =
              componentType.Assembly.GetTypes()
                  .FirstOrDefault(t => t != componentType &&
                                        typeof (IResponsiveComponent).IsAssignableFrom(t) &&
                                        !t.IsAbstract &&
                                        t.IsSubclassOf(typeof (TBaseComponent)) &&
                                        t.Name.EndsWith(componentType.Name));

          return responsiveComponentType != null ? (TBaseComponent)GetComponent(responsiveComponentType) : GetComponent<TComponent>();
        }

        protected internal virtual TComponent GetComponent<TComponent>() where TComponent : UiComponent, new()
        {
            return base.GetComponent<TComponent>();
        }

        protected internal virtual UiComponent GetComponent(Type componentType)
        {
            var methodInfo =
                typeof (UiComponent)
                    .GetMethod("GetComponent", BindingFlags.NonPublic | BindingFlags.Instance, null,
                        CallingConventions.HasThis, new Type[0], new[] {new ParameterModifier(3)})
                    .MakeGenericMethod(componentType);
            return (UiComponent)methodInfo.Invoke(this, new object[0]);
        }
    }
}