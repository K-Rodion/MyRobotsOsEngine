using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsEngine.Robots;

namespace OsEngine.MyEntity
{
    public class EmitClass:BaseVM
    {
        public EmitClass(string str)
        {
            ClassEmit = str;
        }

        public string ClassEmit
        {
            get => _classEmit;

            set
            {
                _classEmit = value;
                OnPropertyChanged(nameof(ClassEmit));
            }
        }

        private string _classEmit;
    }
}
