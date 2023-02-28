using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using OsEngine.Charts.CandleChart.Indicators;
using OsEngine.Entity;
using OsEngine.Robots;

namespace OsEngine.MyEntity
{
    public class Level:BaseVM
    {
        #region Properties ====================================================================================

        /// <summary>
        /// Расчетная цена уровня
        /// </summary>
        public decimal PriceLevel
        {
            get => _priceLevel;

            set
            {
                _priceLevel = value;
                OnPropertyChanged(nameof(PriceLevel));
            }
        }
        private decimal _priceLevel = 0;

        public Side Side
        {
            get => _side;

            set
            {
                _side = value;
                OnPropertyChanged(nameof(Side));
            }
        }
        private Side _side = 0;

        /// <summary>
        /// Реальная цена открытой позиции
        /// </summary>
        public decimal OpenPrice
        {
            get => _openPrice;

            set
            {
                _openPrice = value;
                OnPropertyChanged(nameof(OpenPrice));
            }
        }
        private decimal _openPrice = 0;

        /// <summary>
        /// Объем открытой позиции
        /// </summary>
        public decimal Volume
        {
            get => _volume;

            set
            {
                _volume = value;
                Change();
            }
        }
        private decimal _volume = 0;

        
        public decimal Accum
        {
            get => _accum;

            set
            {
                _accum = value;
                OnPropertyChanged(nameof(Accum));
            }
        }
        private decimal _accum = 0;

        public decimal Margin
        {
            get => _margin;

            set
            {
                _margin = value;
                OnPropertyChanged(nameof(Margin));
            }
        }
        private decimal _margin = 0;

        public decimal TakePrice
        {
            get => _takePrice;

            set
            {
                _takePrice = value;
                OnPropertyChanged(nameof(TakePrice));
            }
        }
        private decimal _takePrice = 0;

        /// <summary>
        /// Лимитка на позицию
        /// </summary>
        public decimal LimitVolume
        {
            get => _limitVolume;

            set
            {
                _limitVolume = value;
                Change();
            }
        }
        private decimal _limitVolume = 0;

        /// <summary>
        /// Лимитка на тейк
        /// </summary>
        public decimal TakeVolume
        {
            get => _takeVolume;

            set
            {
                _takeVolume = value;
                Change();
            }
        }
        private decimal _takeVolume = 0;

        /// <summary>
        /// Флаг разрешение на выставление ордера
        /// </summary>
        public bool PassVolume
        {
            get => _passVolume;

            set
            {
                _passVolume = value;
                Change();
            }
        }
        private bool _passVolume = true;

        /// <summary>
        /// Флаг разрешение на выставление отейк-ордера
        /// </summary>
        public bool PassTake
        {
            get => _passTake;

            set
            {
                _passTake = value;
                Change();
            }
        }
        private bool _passTake = true;

        #endregion


        #region Fields =============================================================================

        private CultureInfo CultureInfo = new CultureInfo("ru-RU");

        /// <summary>
        /// Лимтка на тейк
        /// </summary>
        public List<Order> OrdersForClose = new List<Order>();

        /// <summary>
        /// Лимтка на открытие
        /// </summary>
        public List<Order> OrdersForOpen = new List<Order>();

        private List<MyTrade> _myTrades = new List<MyTrade>();

        private decimal _calcVolume = 0;

        #endregion


        #region Methods ====================================================================================

        private void Change()
        {
            OnPropertyChanged(nameof(Volume));
            OnPropertyChanged(nameof(OpenPrice));
            OnPropertyChanged(nameof(LimitVolume));
            OnPropertyChanged(nameof(TakeVolume));
            OnPropertyChanged(nameof(PassTake));
            OnPropertyChanged(nameof(PassVolume));
            OnPropertyChanged(nameof(TakePrice));
        }

        public string GetStringForSave()
        {
            string str = "";

            str += "Volume = " + Volume.ToString(CultureInfo) + " | ";
            str += "PriceLevel = " + PriceLevel.ToString(CultureInfo) + " | ";
            str += "OpenPrice = " + OpenPrice.ToString(CultureInfo) + " | ";
            str += Side + " | ";
            str += "PassVolume = " + PassVolume.ToString(CultureInfo) + " | ";
            str += "PassTake = " + PassTake.ToString(CultureInfo) + " | ";
            str += "LimitVolume = " + LimitVolume.ToString(CultureInfo) + " | ";
            str += "TakeVolume = " + TakeVolume.ToString(CultureInfo) + " | ";
            str += "TakePrice = " + TakePrice.ToString(CultureInfo) + " | ";

            return str;
        }

        #endregion


    }
}
