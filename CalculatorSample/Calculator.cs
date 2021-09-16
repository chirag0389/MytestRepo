using System;

namespace CalculatorSample
{
    public class Calculator
    {
        public int Add(int a, int b)
        {
          return a+b;
        }

        public int Substract(int a, int b)
        {
            return a-b;
        }

        public int Multiply(int a, int b)
        {
            return a*b;
        }

        public double Divide(int a, int b)
        {
            return (double)a / b;
        }
    }
}
