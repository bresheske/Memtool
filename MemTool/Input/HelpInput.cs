
namespace MemTool.Console.Input
{
    public class HelpInput : BaseInput
    {
        public override bool Validate()
        {
            return true;
        }

        public override void PerformAction()
        {
            BaseInput.PrintHelp();
        }
    }
}
