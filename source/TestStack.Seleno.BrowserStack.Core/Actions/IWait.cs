using TestStack.Seleno.PageObjects.Actions;

namespace TestStack.Seleno.BrowserStack.Core.Actions
{
    public interface IWait : PageObjects.Actions.IWait
    {
        IExecutor Executor { get; }
    }
}