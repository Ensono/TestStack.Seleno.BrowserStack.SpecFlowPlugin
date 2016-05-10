using TestStack.Seleno.PageObjects.Actions;

namespace TestStack.Seleno.BrowserStack.Core.Actions
{
    public class Wait : PageObjects.Actions.Wait, IWait
    {
        public IExecutor Executor { get; }

        public Wait(IExecutor executor) : base(executor)
        {
            Executor = executor;
        }
      
    }
}