using System.Globalization;

namespace Calculator.Models
{
    public class Operand
    {
        public Operand()
        {
            Clear();
        }


        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                _value = double.Parse(_text, CultureInfo.InvariantCulture);
                IsDecimal = !(_value % 1 == 0d);
            }
        }


        private double _value;
        public double Value
        {
            get { return _value; }
            set
            {
                _value = value;
                _text = _value.ToString(CultureInfo.InvariantCulture);
                IsDecimal = !(_value % 1 == 0d);
            }
        }


        public bool IsEntered { get; set; }
        public bool IsNew { get; set; }
        public bool IsDecimal { get; set; }


        public void Clear()
        {
            Value = 0;
            IsEntered = true;
            IsNew = true;
        }


        public void Invalidate()
        {
            Clear();
            IsEntered = false;
        }

        public override string ToString()
        {
            return Text;
        }

        public override int GetHashCode()
        {
            return Text.GetHashCode();
        }
    }
}
