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
                    AddInputState(InputStringState.IsFull);
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
            DecimalKeyCommand = new ActionCommand(DoDecimalKeyCommand, ()=>!HasInputState(InputStringState.IsDecimal));
            PlusMinusKeyCommand = new ActionCommand(DoPlusMinusKeyCommand, ()=>true);

            _calc = new CalculationModel();

            DoClearKeyCommand();
        }


        private void DoNumKeyCommand(object param)
        {
            if (HasInputState(InputStringState.IsFull)) { return; }
            RemoveInputState(InputStringState.LastInputIsOperator);

            if (HasInputState(InputStringState.ReceivingNewOperand)) {
                MainDisplayText = "";

                if (param.ToString() != "0") {
                    RemoveInputState(InputStringState.ReceivingNewOperand);
                }              
            }

            MainDisplayText += param.ToString();
         }


        private void DoOperandKeyCommand(object param)
        {
            if(HasInputState(InputStringState.LastInputIsOperator)) {
                string newOp = param.ToString();
                if (newOp == _operator) { return; }

                HistoryDisplayText = HistoryDisplayText.Remove(HistoryDisplayText.Length - 2) + newOp + " ";
                _operator = newOp;
                return;
            }

            AddInputState(InputStringState.LastInputIsOperator);

            if (!HasInputState(InputStringState.ReceivingNewOperand)) {


                AddInputState(InputStringState.ReceivingNewOperand);
            }
            
            

            double op = double.Parse(MainDisplayText, CultureInfo.InvariantCulture);
            
            _operand1 = op;
            _operand2 = op;

            HistoryDisplayText += MainDisplayText + " " + param + " ";
            MainDisplayText = _operand2.ToString(CultureInfo.InvariantCulture);
            
        }

        
        private void DoDecimalKeyCommand()
        {
            if (HasInputState(InputStringState.IsDecimal)) { return; }

            MainDisplayText += ".";

            AddInputState(InputStringState.IsDecimal);
            RemoveInputState(InputStringState.ReceivingNewOperand);        
        }

        
        private void DoPlusMinusKeyCommand()
        {
            if ((MainDisplayText == "0") || (MainDisplayText == "0.")) { return; } 

            MainDisplayText = MainDisplayText.Contains('-') ? MainDisplayText.Remove(0, 1) 
                                                            : MainDisplayText.Insert(0, "-");

            RemoveInputState(InputStringState.ReceivingNewOperand);
        }


        private void DoClearKeyCommand()
        {
            MainDisplayText = "0";
            HistoryDisplayText = "";
            SetInputState(InputStringState.None);
            AddInputState(InputStringState.ReceivingNewOperand);
            _operand1 = 0;
            _operand2 = 0;
            _operator = "";
            //_result = 0;
        }

        private void AddInputState(InputStringState state) {_inputState |= state; }
        private void RemoveInputState(InputStringState state) { _inputState &= ~state; }
        private void SetInputState(InputStringState state) { _inputState = state; }
        private bool HasInputState(InputStringState state) { return _inputState.HasFlag(state); }

        private InputStringState _inputState;
        private string _mainDisplayText;
        private string _historyDisplayText;
        private double _operand1;
        private double _operand2;
        private string _operator;
        private CalculationModel _calc;
        //private double _result;
    }
}
