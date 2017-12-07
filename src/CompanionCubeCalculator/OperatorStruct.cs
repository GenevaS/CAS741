﻿/*
 * Operator Data Structure
 * ---------------------------------------------------------------------
 * Updated 2017/12/07
 * Corresponds to Operator Data Structure MIS from
 * https://github.com/GenevaS/CAS741/blob/master/Doc/Design/MIS/MIS.pdf
 * ---------------------------------------------------------------------
 */
namespace CompanionCubeCalculator
{
    public struct OperatorStruct
    {
        private string operatr;
        private int precedence;
        private bool isUnary;
        private bool isBinary;
        private bool isTernary;
        private bool leftAssociative;

        /* CONSTRUCTOR */
        public OperatorStruct (string op, int prec, bool isUnary, bool isBinary, bool isTernary, bool isLeftAssociative)
        {
            if (op == "")
            {
                throw new System.ArgumentException("Error: Cannot have an operator with no representative symbol.");
            }
            else
            {
                operatr = op;
            }
            
            precedence = prec;

            if ((isUnary == isBinary && isUnary == true) || (isUnary == isTernary && isUnary == true) || (isBinary == isTernary && isBinary == true))
            {
                throw new System.ArgumentException("Error: An operator cannot be overloaded to be unary, binary, and ternary.");
            }
            else if (isUnary == false && isBinary == false && isTernary == false)
            {
                throw new System.ArgumentException("Error: Operators must be assigned a number of operands type.");
            }
            else
            {
                this.isUnary = isUnary;
                this.isBinary = isBinary;
                this.isTernary = isTernary;
            }
            
            leftAssociative = isLeftAssociative;

            return;
        }

        /* GETTERS */
        public string GetOperator()
        {
            return operatr;
        }

        public int GetPrecedence()
        {
            return precedence;
        }

        public bool IsUnary()
        {
            return isUnary;
        }

        public bool IsBinary()
        {
            return isBinary;
        }

        public bool IsTernary()
        {
            return isTernary;
        }

        public bool IsLeftAssociative()
        {
            return leftAssociative;
        }
    }
}
