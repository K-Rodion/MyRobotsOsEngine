using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsEngine.Entity;
using OsEngine.OsTrader.Panels;

namespace OsEngine.Robots.FrontRunner.Model
{
    public class FrontRunnerBot : BotPanel
    {
        public FrontRunnerBot(string name, StartProgram startProgram) : base(name, startProgram)
        {
        }

        public override string GetNameStrategyType()
        {
            return nameof(FrontRunnerBot);
        }

        public override void ShowIndividualSettingsDialog()
        {
            
        }
    }
}
