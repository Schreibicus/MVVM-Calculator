using System;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using Calculator.Models;

namespace Calculator.ViewModels
{
    [Flags]
    public enum InputStringState
    {
        None = 0, ReceivingNewOperand = 1, HasTwoOperands = 2, IsDecimal = 4, IsFull = 8, LastInputIsOperator = 16
    }

    public class MainViewModel : BindableBase 
    {
        
        public ICommand NumKeyCommand { get; private set; }
        public ICommand OperandKeyCommand { get; private set; }
        public ICommand ClearKeyCommand { get; private set; }
        public ICommand DecimalKeyCommand { get; private set; }
        public ICommand PlusMinusKeyCommand { get; private set; }


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
            NumKeyCommand = new ActionCommand(DoNumKeyCommand, ()=>true);
            OperandKeyCommand = new ActionCommand(DoOperandKeyCommand, ()=>true);
            ClearKeyCommand = new ActionCommand(DoClearKeyCommand, ()=>true);
            DecimalKeyCommand = new ActionCommand(DoDecimalKeyCommand, ()=>!_currentOperand.IsDecimal);
            PlusMinusKeyCommand = new ActionCommand(DoPlusMinusKeyCommand, ()=>true);

            _calc = new ArithmeticModel();
            _operand1 = new Operand();
            _operand2 = new Operand();

            DoClearKeyCommand();
        }


        private void DoNumKeyCommand(object param)
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


        private void DoOperandKeyCommand(object param)
        {
            string newOperator = param.ToString();
           
            //If only one operand present - save operator and begin entering second operand
            if (!_operand2.IsEntered) {
                _operator = newOperator;
                HistoryDisplayText += _currentOperand.Text + " " + _operator + " ";

                _operand2.Value = _operand1.Value;
                _operand2.IsEntered = true;
                _operand2.IsNew = true;
                _currentOperand = _operand2;
                MainDisplayText = _operand2.Text;
                return;
            }


            //If second operand is new - update operator
            if (_operand2.IsNew) {
                if (newOperator == _operator) { return; }
                _operator = newOperator;

                //replace operator ion history string - meh code...
                HistoryDisplayText = HistoryDisplayText.Remove(HistoryDisplayText.Length - 2) + _operator + " ";

                return;
            }


            //Else - calculate and update history string
            HistoryDisplayText += _currentOperand.Text + " " + newOperator + " ";
            
            _operand1.Value = _calc.Calculate(_operand1.Value, _operand2.Value, _operator);
            
            _operand2.Clear();
            _operand2.Value = _operand1.Value;
            _operand1.IsNew = true;

            _currentOperand = _operand2;
            MainDisplayText = _currentOperand.Text;

            _operator = newOperator;
        }

        
        private void DoDecimalKeyCommand()
        {
            if (_currentOperand.IsDecimal) { return; }

            MainDisplayText += ".";
            _currentOperand.IsNew = false;
            _currentOperand.Text = MainDisplayText;
        }

        
        private void DoPlusMinusKeyCommand()
        {
            if (_currentOperand.Value == 0d) { return; }

            _currentOperand.Value *= -1.0;
            _currentOperand.IsNew = false;
            MainDisplayText = _currentOperand.Text;
        }


        private void DoClearKeyCommand()
        {
            _operand1.Clear();
            _operand2.Invalidate();
            _currentOperand = _operand1;
            
            MainDisplayText = _operand1.Text;
            HistoryDisplayText = "";
            _displayFull = false;
        }


        private string _mainDisplayText;
        private string _historyDisplayText;

        private bool _displayFull;
        
        private ArithmeticModel _calc;

        private Operand _operand1;
        private Operand _operand2;
        private Operand _currentOperand;
        private string _operator;

    }
}
