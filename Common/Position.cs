using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Stocks.Common
{
    public class Position : INotifyPropertyChanged
    {
        public Position()
        { }

        private string _symbol;
        public string Symbol 
        {
            get
            {
                return _symbol;
            }
            set
            {
                _symbol = value;
                this.NotifyPropertyChanged("Symbol");
            }
        }
        
        public string Description { get; set; }
        public decimal Quantity { get; set; }

        private decimal _basis;
        public decimal Basis 
        {
            get
            {
                return _basis;
            }

            set
            {
                _basis = value;

                this.NotifyPropertyChanged("Basis");
            }
        }
        
        public Quote Quote { get; set; }
        public bool OutsandingOrdersExist { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
