using System;
using System.Linq;
using System.Windows.Input;

namespace Calculator.ViewModels
{
    [Flags]
    public enum InputStringState
    {
        None = 0, NextNumIsNewOp = 1, IsDecimal = 2, IsFull = 4
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

        private InputStringState _inputState;
        private string _mainDisplayText;
        private string _historyDisplayText;
        //private double _operand1;
        //private double _operand2;
        //private double _result;


        public MainViewModel()
        {
            NumKeyCommand = new ActionCommand(DoNumKeyCommand, ()=>true);
            OperandKeyCommand = new ActionCommand(DoOperandKeyCommand, ()=>true);
            ClearKeyCommand = new ActionCommand(DoClearKeyCommand, ()=>true);
            DecimalKeyCommand = new ActionCommand(DoDecimalKeyCommand, ()=>!HasInputState(InputStringState.IsDecimal));
            PlusMinusKeyCommand = new ActionCommand(DoPlusMinusKeyCommand, ()=>true);

            DoClearKeyCommand();
        }


        private void DoNumKeyCommand(object param)
        {
            if (HasInputState(InputStringState.IsFull)) { return; }

            if (HasInputState(InputStringState.NextNumIsNewOp)) {
                MainDisplayText = "";

                if (param.ToString() != "0") {
                    RemoveInputState(InputStringState.NextNumIsNewOp);
                }              
            }

            MainDisplayText += param.ToString();
         }


        private void DoOperandKeyCommand(object param)
        {
            MainDisplayText += param.ToString();
            HistoryDisplayText += param.ToString();
        }

        
        private void DoDecimalKeyCommand()
        {
            if (HasInputState(InputStringState.IsDecimal)) { return; }

            MainDisplayText += ".";
            AddInputState(InputStringState.IsDecimal);

            if (HasInputState(InputStringState.NextNumIsNewOp)) {
                RemoveInputState(InputStringState.NextNumIsNewOp);
            }
        }

        
        private void DoPlusMinusKeyCommand()
        {
            if (MainDisplayText == "0" || MainDisplayText == "0.") { return; } 

            MainDisplayText = MainDisplayText.Contains('-') ? MainDisplayText.Remove(0, 1) 
                                                            : MainDisplayText.Insert(0, "-");
        }


        private void DoClearKeyCommand()
        {
            MainDisplayText = "0";
            HistoryDisplayText = "";
            SetInputState(InputStringState.None);
            AddInputState(InputStringState.NextNumIsNewOp);
            //_operand1 = 0;
            //_operand2 = 0;
            //_result = 0;
        }

        private void AddInputState(InputStringState state) {_inputState |= state; }
        private void RemoveInputState(InputStringState state) { _inputState &= ~state; }
        private void SetInputState(InputStringState state) { _inputState = state; }
        private bool HasInputState(InputStringState state) { return _inputState.HasFlag(state); }
    }
}
