using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsEngine.Charts.CandleChart.Indicators;
using OsEngine.Robots;

namespace OsEngine.MyEntity
{
    public class Level:BaseVM
    {

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
                OnPropertyChanged(nameof(Volume));
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
                OnPropertyChanged(nameof(LimitVolume));
            }
        }
        private decimal _limitVolume = 0;

        /// <summary>
        /// Лимитка на тейк
        /// </summary>
        public decimal LimitTake
        {
            get => _limitTake;

            set
            {
                _limitTake = value;
                OnPropertyChanged(nameof(LimitTake));
            }
        }
        private decimal _limitTake = 0;

        /// <summary>
        /// Флаг разрешение на выставление ордера
        /// </summary>
        public bool PassVolume
        {
            get => _passVolume;

            set
            {
                _passVolume = value;
                OnPropertyChanged(nameof(PassVolume));
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
                OnPropertyChanged(nameof(PassTake));
            }
        }
        private bool _passTake = true;

    }
}
