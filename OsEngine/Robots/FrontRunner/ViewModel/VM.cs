using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using OsEngine.Commands;
using OsEngine.Robots.FrontRunner.Model;


namespace OsEngine.Robots.FrontRunner.ViewModel
{
    public class VM:BaseVM
    {
        public VM(FrontRunnerBot bot)
        {
            _bot = bot;
        }

        #region Fields========================================================================

        private FrontRunnerBot _bot;

        #endregion

        #region Propetries ===================================================================

        public decimal BigVolume
        {
            get => _bot.BigVolume;

            set
            {
                _bot.BigVolume = value;
                OnPropertyChanged(nameof(BigVolume));
            }
        }
        

        public int Offset
        {
            get => _bot.Offset;

            set
            {
                _bot.Offset = value;
                OnPropertyChanged(nameof(Offset));
            }
        }
        

        public int Take
        {
            get => _bot.Take;

            set
            {
                _bot.Take = value;
                OnPropertyChanged(nameof(Take));
            }
        }
       

        public decimal Lot
        {
            get => _bot.Lot;

            set
            {
                _bot.Lot = value;
                OnPropertyChanged(nameof(Lot));
            }
        }
       

        public Edit Edit
        {
            get => _edit;

            set
            {
                _edit = value;
                OnPropertyChanged(nameof(Edit));
            }
        }
        private Edit _edit;

        #endregion

        #region Commands =====================================================================

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

        #region Methods =====================================================================

        private void Start(object obj)
        {
            if (Edit == Edit.Start)
            {
                Edit = Edit.Stop;
            }
            else
            {
                Edit = Edit.Start;
            }
        }

        #endregion

    }

    public enum Edit
    {
        Start,
        Stop
    }
}
