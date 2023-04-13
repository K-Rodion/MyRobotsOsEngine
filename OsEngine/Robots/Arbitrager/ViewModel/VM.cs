using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using OsEngine.Commands;
using OsEngine.Robots.Arbitrager.Model;
using OsEngine.Robots.FrontRunner.Model;

namespace OsEngine.Robots.Arbitrager.ViewModel
{
    public class VM:BaseVM
    {
        public VM(ArbitragerBot bot)
        {
            _bot = bot;
        }

        #region Fields========================================================================

        private ArbitragerBot _bot;

        #endregion

        #region Properties ===================================================================

        public decimal Lot
        {
            get => _bot.Lot;

            set
            {
                _bot.Lot = value;
                OnPropertyChanged(nameof(Lot));
            }
        }

        public decimal Take
        {
            get => _bot.Take;

            set
            {
                _bot.Take = value;
                OnPropertyChanged(nameof(Take));
            }
        }

        public decimal Step
        {
            get => _bot.Step;

            set
            {
                _bot.Step = value;
                OnPropertyChanged(nameof(Step));
            }
        }

        public decimal Spread
        {
            get => _bot.Spread;

            set
            {
                _bot.Spread = value;
                OnPropertyChanged(nameof(Spread));
            }
        }

        public Edit EDIT
        {
            get => _bot.EDIT;

            set
            {
                _bot.EDIT = value;
                OnPropertyChanged(nameof(EDIT));
            }
        }

        #endregion

        #region Commands ===================================================================

        private DelegateCommand _commandStart;

        public ICommand CommandStart
        {
            get
            {
                if (_commandStart == null)
                {
                    _commandStart = new DelegateCommand(Start);
                }

                return _commandStart;
            }
        }

        #endregion

        #region Methods ======================================================================

        private void Start(object obj)
        {
            if (EDIT == Edit.Start)
            {
                EDIT = Edit.Stop;
            }
            else
            {
                EDIT = Edit.Start;
            }
        }

        #endregion

        public enum Edit
        {
            Start,
            Stop
        }
    }
}
