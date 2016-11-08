using System;
using System.Linq;
using System.Windows.Input;
using Calculator.Models;

namespace Calculator.ViewModels
{
    public class MainViewModel : BindableBase 
    {      
        public ICommand NumberCommand { get; private set; }
        public ICommand OperandCommand { get; private set; }
        public ICommand CalculateCommand { get; private set; }
        public ICommand ClearCommand { get; private set; }
        public ICommand MakeDecimalCommand { get; private set; }
        public ICommand ChangeSignCommand { get; private set; }


        public string MainDisplayText
        {
            get { return _mainDisplayText; }
            set
            {
                SetProperty(ref _mainDisplayText, value);
                if (_mainDisplayText.Count() >= 19) {
                    _displayFull = true;
                }
            }       
        }
        
        public string HistoryDisplayText
        {
            get { return _historyDisplayText; }
            set { SetProperty(ref _historyDisplayText, value); }
        }

        
        public MainViewModel()
        {
            NumberCommand = new ActionCommand(DoNumberCommand, ()=>true);
            OperandCommand = new ActionCommand(DoOperandCommand, ()=>true);
            CalculateCommand = new ActionCommand(DoCalculateCommand, ()=> _currentOperand.IsEntered);
            ClearCommand = new ActionCommand(DoClearCommand, ()=>true);
            MakeDecimalCommand = new ActionCommand(DoMakeDecimalCommand, ()=>!_currentOperand.IsDecimal);
            ChangeSignCommand = new ActionCommand(DoChangeSignCommand, ()=>true);

            _calc = new ArithmeticModel();
            _operand1 = new Operand();
            _operand2 = new Operand();

            DoClearCommand();
        }

        #region Commands
        private void DoNumberCommand(object param)
        {
            if (_displayFull) { return; }

            string newDigit = param.ToString();

            if (_currentOperand.IsNew) {
                if (newDigit != _currentOperand.Text) {_currentOperand.IsNew = false;}
                MainDisplayText = _currentOperand.Text = newDigit;
                return;
            }

            MainDisplayText += newDigit;
            _currentOperand.Text = MainDisplayText;
        }


        private void DoOperandCommand(object param)
        {
            string newOperator = param.ToString();
           
            //If only one operand present - enter new operator 
            if (!_operand2.IsEntered) {
                EnterNewOperator(newOperator);
                return;
            }

            //If both operators present but second is new - just update operator
            if (_operand2.IsNew) {
                UpdateLastOperator(newOperator);
                return;
            }

            //If both operators are final - calculate and update operator
            CalculateWithNewOperator(newOperator);
        }

        
        private void DoCalculateCommand()
        {
            double result = _calc.Calculate(_operand1.Value, _operand2.Value, _operator);
            DoClearCommand();
            _currentOperand.Value = result;
            MainDisplayText = _currentOperand.Text;
        }


        private void DoMakeDecimalCommand()
        {
            if (_currentOperand.IsDecimal) { return; }

            MainDisplayText += ".";
            _currentOperand.IsNew = false;
            _currentOperand.Text = MainDisplayText;
        }

        
        private void DoChangeSignCommand()
        {
            // No -0 please
            if (Math.Abs(_currentOperand.Value) < double.Epsilon) { return; }

            _currentOperand.Value *= -1.0;
            _currentOperand.IsNew = false;
            MainDisplayText = _currentOperand.Text;
        }


        private void DoClearCommand()
        {
            _operand1.Clear();
            _operand2.Invalidate();
            _currentOperand = _operand1;
            
            MainDisplayText = _operand1.Text;
            HistoryDisplayText = "";
            _displayFull = false;
        }
        #endregion

        #region Helper Methods
        private void EnterNewOperator(string newOperator)
        {
            _operator = newOperator;
            HistoryDisplayText += _currentOperand.Text + " " + _operator + " ";

            _operand2.Value = _operand1.Value;
            _operand2.IsEntered = true;
            _operand2.IsNew = true;
            _currentOperand = _operand2;
            MainDisplayText = _operand2.Text;
        }


        private void UpdateLastOperator(string newOperator)
        {
            if (newOperator == _operator) { return; }
            _operator = newOperator;

            //replace operator in history string - meh code... let's hide it down below, under region
            HistoryDisplayText = HistoryDisplayText.Remove(HistoryDisplayText.Length - 2) + _operator + " ";
        }

        private void CalculateWithNewOperator(string newOperator)
        {
            HistoryDisplayText += _currentOperand.Text + " " + newOperator + " ";

            _operand1.Value = _calc.Calculate(_operand1.Value, _operand2.Value, _operator);

            _operand2.Clear();
            _operand2.Value = _operand1.Value;
            _operand2.IsNew = true;

            _currentOperand = _operand2;
            MainDisplayText = _currentOperand.Text;

            _operator = newOperator;
        }
        #endregion


        #region Private Fields
        private readonly ArithmeticModel _calc;

        private readonly Operand _operand1;
        private readonly Operand _operand2;
        private Operand _currentOperand;
        private string _operator;

        private string _mainDisplayText;
        private string _historyDisplayText;

        private bool _displayFull;
        #endregion
    }
}
