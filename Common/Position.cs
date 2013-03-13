
namespace Stocks.Common
{
    using System.ComponentModel;

    public class Position : INotifyPropertyChanged
    {
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
                NotifyPropertyChanged("Symbol");
            }
        }
        
        public string Description { get; set; }

        private decimal _quantity;
        public decimal Quantity 
        { 
            get
            {
                return _quantity;
            }
            set
            {
                _quantity = value;
                NotifyPropertyChanged("Quantity");
                NotifyPropertyChanged("Spread");
            }
        }

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

                NotifyPropertyChanged("Basis");
            }
        }

        public decimal CurrentValue
        {
            get
            {
                if (Quote != null)
                {
                    return Quantity * Quote.Price;
                }

                return 0m;
            }
        }

        private Quote _quote;
        public Quote Quote 
        {
            get
            {
                return _quote;
            }
            set
            {
                _quote = value;
                NotifyPropertyChanged("Quote");
                NotifyPropertyChanged("CurrentValue");
                NotifyPropertyChanged("Spread");
            }
        }

        public decimal Spread
        {
            get
            {
                return CurrentValue - Basis;
            }
        }

        public bool OutsandingOrdersExist { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
