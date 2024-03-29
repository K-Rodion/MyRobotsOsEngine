﻿using System;
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

        public decimal EntryPrice
        {
            get => _bot.EntryPrice;

            set
            {
                _bot.EntryPrice = value;
                OnPropertyChanged(nameof(EntryPrice));
            }
        }

        public decimal CurrentLot
        {
            get => _bot.CurrentLot;

            set
            {
                _bot.CurrentLot = value;
                OnPropertyChanged(nameof(CurrentLot));
            }
        }

        public decimal ClosePrice
        {
            get => _bot.ClosePrice;

            set
            {
                _bot.ClosePrice = value;
                OnPropertyChanged(nameof(ClosePrice));
            }
        }

        public string SecururityName
        {
            get => _bot.SecururityName;

            set
            {
                _bot.SecururityName = value;
                OnPropertyChanged(nameof(SecururityName));
            }
        }

        public string State
        {
            get => _bot.State;

            set
            {
                _bot.State = value;
                OnPropertyChanged(nameof(State));
            }
        }


        public Edit Edit
        {
            get => _bot.Edit;

            set
            {
                _bot.Edit = value;
                OnPropertyChanged(nameof(Edit));
            }
        }
      
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
