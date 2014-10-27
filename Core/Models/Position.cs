
namespace Sappworks.Stocks
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

        private double _quantity;
        public double Quantity 
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

        private double _basis;
        public double Basis 
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

        public double CurrentValue
        {
            get
            {
                if (Quote != null)
                {
                    return Quantity * Quote.Price;
                }

                return 0d;
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

        public double Spread
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
