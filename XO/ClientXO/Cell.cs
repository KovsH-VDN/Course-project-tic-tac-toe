using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientXO
{
    public class Cell : BindableBase
    {
        private byte xo;
        public byte XO
        {
            get => xo;
            set
            {
                if (xo.Equals(value)) return;
                xo = value;
                RaisePropertyChanged(nameof(XO));
            }
        }

        private bool isWin;
        public bool IsWin
        {
            get => isWin;
            set
            {
                if (isWin.Equals(value)) return;
                isWin = value;
                RaisePropertyChanged(nameof(IsWin));
            }
        }

        public Cell(byte xo)
        {
            XO = xo;
        }
    }
}
