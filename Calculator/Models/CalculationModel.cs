using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Models
{
    public class CalculationModel
    {
        public double Add(double x, double y) { return x + y; }
        public double Subtract(double x, double y) { return x - y; }
        public double Multiply(double x, double y) { return x * y; }
        public double Divide(double x, double y) { return y == 0d  ? 0 : x / y; }
    }
}
