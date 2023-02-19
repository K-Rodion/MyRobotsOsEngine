using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsEngine.Entity;
using OsEngine.OsTrader.Panels;

namespace OsEngine.Robots.Arbitrager.Model
{
    public class ArbitragerBot : BotPanel
    {
        public ArbitragerBot(string name, StartProgram startProgram) : base(name, startProgram)
        {
            TabCreate(BotTabType.Simple);

            TabCreate(BotTabType.Simple);
        }

        public override string GetNameStrategyType()
        {
            return nameof(ArbitragerBot);
        }

        public override void ShowIndividualSettingsDialog()
        {
            
        }
    }
}
